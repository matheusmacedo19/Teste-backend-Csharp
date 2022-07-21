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
using RoutineCheck.Models;
using CsvHelper;
using System.Globalization;
using System.Threading;

namespace RoutineCheck
{
    public static class Routine
    {
        public static void Initialize()
        {
            Currency currency = GetItemApi();
            
            if(currency.moeda != null)
            {
                CurrenciesData currencies = GetCurrencies(currency);
                QuotationData quotationData = GetQuotation(currencies);
                CreateCsvFile(quotationData);
            }
            Thread.Sleep(120000);
        }
        public static Currency GetItemApi()
        {
            try
            {
                string result;
                Currency currency = new Currency();
                Uri url = new Uri("http://localhost:7160/currency/GetItemFila");
                using (HttpClient webClient = new HttpClient())
                {
                    var response = webClient.GetAsync(url);
                   
                    if(response.Result.Content != null)
                    {
                        result = response.Result.Content.ReadAsStringAsync().Result;
                        
                        if (!string.IsNullOrEmpty(result))
                        {
                            currency = JsonConvert.DeserializeObject<Currency>(result);
                        }
                    }
                    
                    return currency;
                }
            }catch(Exception ex)
            {
                throw ex;
            }
        }
        public static CurrenciesData GetCurrencies(Currency currency)
        {
            string filepath = Directory.GetCurrentDirectory();
            string filename = @"\DadosMoeda.csv";
            string directoryFullPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory.ToString()) + filename;
            
            List<string> listKey = new List<string>();
            List<DateTime> listValue = new List<DateTime>();

            StreamReader reader = new StreamReader(File.OpenRead(directoryFullPath));
            reader.ReadLine();
            CurrenciesData data = new CurrenciesData();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(';');
                DateTime value = DateTime.Parse(values[1]);
                listKey.Add(values[0]);
                listValue.Add(value);
            }


            for (int i = 0; i < listValue.Count; i++)
            {
                if (listValue[i] <= currency.data_fim && listValue[i] >= currency.data_inicio)
                {
                    data.Key.Add(listKey[i].ToString());   
                    data.Value.Add(listValue[i]);    
                }
            }
            return data;
        }
        //Criar metodo para buscar valor cotação no arquivo csv utilizando a tabela de-para item 4
        public static QuotationData GetQuotation(CurrenciesData currencies)
        {
            try
            {
                List<QuotationCode> quotationCodeList = new List<QuotationCode>();
                for(int i = 0; i < currencies.Key.Count; i++)
                { 
                    QuotationCode code;
                    Enum.TryParse(currencies.Key[i], true, out code);

                    quotationCodeList.Add(code);
                }
                string filename = @"\Dadoscotacao.csv";
                string directoryFullPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory.ToString()) + filename;
                
                List<decimal> listValue = new List<decimal>();
                List<DateTime> listDate = new List<DateTime>();
                List<QuotationCode> quotationCodes = new List<QuotationCode>();

                StreamReader reader = new StreamReader(File.OpenRead(directoryFullPath));
                reader.ReadLine();

                QuotationData quotationData = new QuotationData();
                
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(';');

                    decimal quote = decimal.Parse(values[0]);
                    QuotationCode code;
                    Enum.TryParse(values[1], true, out code);
                    DateTime value = DateTime.Parse(values[2]);
                    
                    listValue.Add(quote); 
                    listDate.Add(value);
                    quotationCodes.Add(code);
                }
                for(int i = 0; i < listDate.Count; i++)
                {
                    for(int j = 0; j < currencies.Value.Count; j++)
                    {
                        if (listDate[i] == currencies.Value[j] && quotationCodes[i].ToString() == currencies.Key[j])
                        {
                            quotationData.DateTimeList.Add(listDate[i]);
                            quotationData.QuotationValueList.Add(listValue[i]);
                            quotationData.QuotationCodeList.Add(quotationCodes[i]);
                        }
                    }

                }
                return quotationData;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public static void CreateCsvFile(QuotationData quotationData)
        {
            string nomeArquivo = "Resultado_" + DateTime.Now.ToString("yyyy/MM/dd")+"_"+ DateTime.Now.ToString("HH:mm:ss")+".csv";
            nomeArquivo = nomeArquivo.Replace("/", "").Replace(":", "");
            var nomePasta = "\\DadosCotacao";
            var caminhoArquivo = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory.ToString()) + nomePasta;

            if (!Directory.Exists(caminhoArquivo))
                Directory.CreateDirectory(caminhoArquivo);

            using (var streamWriter = new StreamWriter(Path.Combine(caminhoArquivo, nomeArquivo)))
            using (var csvWriter = new CsvWriter(streamWriter, new CultureInfo("pt-BR", true)))
            {
                csvWriter.Context.RegisterClassMap<ClassMap>();
                csvWriter.WriteRecords(GenerateDataFile(quotationData));
                streamWriter.Flush();
            }
        }
        public static List<FileQuotation> GenerateDataFile(QuotationData quotation)
        {
            List<FileQuotation> data = new List<FileQuotation>();
            
            for(int i = 0; i < quotation.QuotationValueList.Count; i++)
            {
                data.Add(new FileQuotation
                {
                    IdMoeda = quotation.QuotationCodeList[i].ToString(),
                    DateRef = quotation.DateTimeList[i].ToString("dd/MM/yyyy"),
                    Value = quotation.QuotationValueList[i].ToString()
                });
            }
            return data;
        }
    }
}
