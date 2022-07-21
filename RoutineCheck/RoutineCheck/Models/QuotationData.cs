using System;
using System.Collections.Generic;
using System.Text;

namespace RoutineCheck.Models
{
    public class QuotationData
    {
        public List<decimal> QuotationValueList{ get; set; }
        public List<DateTime> DateTimeList { get; set; }
        public List<QuotationCode> QuotationCodeList { get; set; }
        public QuotationData()
        {
            QuotationCodeList = new List<QuotationCode>();
            DateTimeList = new List<DateTime>();
            QuotationValueList = new List<decimal>();
        }
    }
}
