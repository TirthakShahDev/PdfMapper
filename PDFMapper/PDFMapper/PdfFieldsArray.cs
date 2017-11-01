using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFMapper
{
    public class PropertiesArray
    {
        public PropertiesArray()
        {
            Key = "";
            IsBool = false;
            IsDecimal = false;
            IsDate = false;
            IsCurrency = false;
        }
        public string Key { get; set; }

        public bool IsBool { get; set; }

        public bool IsDecimal{ get; set; }

        public bool IsDate { get; set; }

        public bool IsCurrency { get; set; }

    }
}
