using Hanssens.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProvaProgramacao.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProvaProgramacao.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : ControllerBase
    {
        private Queue<Currency> _currencyQueueRequests;
        private Currency _lastAdd;
            

        private readonly ILogger<CurrencyController> _logger;
        
        public CurrencyController(ILogger<CurrencyController> logger)
        {
            _logger = logger;
            _currencyQueueRequests = new Queue<Currency>();
            _lastAdd = new Currency();
        }

        [HttpPost]
        [Route("AddItemFila")]
        public IActionResult AddItemFila([FromBody] List<Currency> currencyListRequest)
        {
            List<Currency> currencyErrorList = new List<Currency>();
            Queue<Currency> auxQueue = new Queue<Currency>();
            string errorMessage = string.Empty;
            try
            {
                if(currencyListRequest != null)
                {
                    foreach (Currency currency in currencyListRequest)
                    {
                        if (currency.data_inicio > currency.data_fim)
                        {
                            currencyErrorList.Add(currency);
                        }
                        if(currency.data_inicio == currency.data_fim)
                        {
                            currencyErrorList.Add(currency);
                        }
                        auxQueue.Enqueue(currency);
                    }
                    if(auxQueue.Count > 0)
                    {
                        _lastAdd = auxQueue.LastOrDefault();

                        using (LocalStorage storage = new LocalStorage())
                        {
                            if (storage.Exists("Queue"))
                            {
                                var queue = storage.Query<Currency>("Queue");
                            
                                foreach (Currency item in queue)
                                {
                                    _currencyQueueRequests.Enqueue(item);
                                }

                                foreach (Currency item in auxQueue)
                                {
                                    _currencyQueueRequests.Enqueue(item);
                                }
                            }
                            else
                            {
                                foreach (Currency item in auxQueue)
                                {
                                    _currencyQueueRequests.Enqueue(item);
                                }
                            }
                            storage.Store("Queue", _currencyQueueRequests);
                            storage.Store("LastAdd", _lastAdd);
                            storage.Persist();
                        }
                    }
                    else
                    {
                        return BadRequest("Nenhum objeto adicionado, verificar campo data.");
                    }
                }
                else
                {
                    return BadRequest("Lista sem objetos");
                }
                
                if(currencyErrorList.Count > 0)
                {
                    errorMessage = "Um ou mais objetos não foram adicionados, objetos com intervalo de data errado não foram adicionados.";
                    return Ok(new
                    {
                        Data = new ObjectResult(currencyErrorList),
                        errorMessage
                    });
                }
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("Todos objetos foram adicionados.");
        }
        [HttpGet]
        [Route("GetItemFila")]
        public IActionResult GetItemFila()
        {
            try
            {
                Stack<Currency> stack = new Stack<Currency>();
                Currency currency = new Currency();

                using (LocalStorage storage = new LocalStorage())
                {
                    if (storage.Exists("Queue"))
                    {
                        var queue = storage.Query<Currency>("Queue");

                        if (storage.Exists("LastAdd"))
                        {
                            _lastAdd = storage.Get<Currency>("LastAdd");
                        }

                        if (queue.Count() > 0)
                        {
                            foreach (Currency item in queue)
                            {
                                _currencyQueueRequests.Enqueue(item);
                            }

                            if (_lastAdd != null)
                            {
                                foreach (Currency item in _currencyQueueRequests)
                                {
                                    stack.Push(item);
                                }

                                _currencyQueueRequests.Clear();
                                currency = stack.Pop();

                                if (stack.Count() > 0)
                                {
                                    foreach (Currency item in stack)
                                    {
                                        _currencyQueueRequests.Enqueue(item);
                                    }
                                    _currencyQueueRequests.Enqueue(currency);
                                }
                                else
                                {
                                    _currencyQueueRequests.Enqueue(currency);
                                }

                                storage.Store("Queue", _currencyQueueRequests);
                                storage.Persist();

                                return new ObjectResult(currency);
                            }
                            else
                            {
                                return BadRequest("Não existe objeto a ser retornado");
                            }
                        }
                    }
                    else
                    {
                        return BadRequest("Nenhum objeto armazenado");
                    }
                }
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return NotFound();
        }
    }
}
