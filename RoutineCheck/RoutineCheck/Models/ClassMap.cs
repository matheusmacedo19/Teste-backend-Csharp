using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoutineCheck.Models
{
    public class ClassMap : ClassMap<FileQuotation>
    {
        public ClassMap()
        {
            Map(m => m.IdMoeda).Name("ID_MOEDA");
            Map(m => m.DateRef).Name("DATA_REF");
            Map(m => m.Value).Name("VLR_COTACAO");
        }
    }
}
