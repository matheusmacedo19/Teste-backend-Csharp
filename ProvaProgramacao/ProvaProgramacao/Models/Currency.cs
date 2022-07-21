using Newtonsoft.Json;
using System;

namespace ProvaProgramacao.Models
{
    public class Currency
    {
        public DateTime data_inicio { get; set; }
        public DateTime data_fim { get; set; }
        public string moeda { get; set; }
    }
}
