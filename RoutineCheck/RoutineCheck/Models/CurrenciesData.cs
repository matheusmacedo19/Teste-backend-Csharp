using System;
using System.Collections.Generic;
using System.Text;

namespace RoutineCheck.Models
{
    public class CurrenciesData
    {
        public List<string> Key{ get; set; }
        public List<DateTime> Value{ get; set; }
        public CurrenciesData()
        {
            Key = new List<string>();
            Value = new List<DateTime>();
        }
    }
}
