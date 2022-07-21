using Newtonsoft.Json;
using ProvaProgramacao.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Data;
using System.Text;
using System.Text.Json.Serialization;
using System.Data.OleDb;

namespace RoutineCheck
{
    public static class Routine
    {
        public static Currency GetItemApi()
        {
            try
            {
                Currency currency = new Currency();
                Uri url = new Uri("http://localhost:7160/currency/GetItemFila");
                using (HttpClient webClient = new HttpClient())
                {
                    var response = webClient.GetAsync(url);
                    string result = response.Result.Content.ReadAsStringAsync().Result;
                    
                    if (response.IsCompletedSuccessfully)
                    {
                        currency = JsonConvert.DeserializeObject<Currency>(result);

                        GetCurrencies(currency);
                    }
                    
                    return currency;
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static Dictionary<string, DateTime> GetCurrencies(Currency currency)
        {
            string filepath = Directory.GetCurrentDirectory();
            string filename = @"\DadosMoeda.csv";
            string directoryFullPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory.ToString()) + filename;
            
            List<string> listKey = new List<string>();
            List<DateTime> listValue = new List<DateTime>();

            StreamReader reader = new StreamReader(File.OpenRead(directoryFullPath));
            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');
                DateTime value = DateTime.Parse(values[1]);
                listKey.Add(values[0]);
                listValue.Add(value);
            }

            Dictionary<string,DateTime> moedas = new Dictionary<string, DateTime>();
            
            for(int i = 0; i < listValue.Count - 1; i++)
            {
                if (listValue[i] <= currency.data_fim && listValue[i] >= currency.data_inicio)
                {
                    //Verificar existencia de chave duplicada
                    moedas.Add(listKey[i], listValue[i]);
                }
            }
            return moedas;
        }
        //Criar metodo para buscar valor cotação no arquivo csv utilizando a tabela de-para item 4

        //Criar metodo para criar aquivo csv com os dados 
    }
}
