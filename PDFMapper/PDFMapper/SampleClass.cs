using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFMapper
{
    public class SampleClass
    {

        public SampleClass()
        {
            ListsubclassVal = new List<SubClass>();
            subclassVal = new SubClass();

        }
        public string stringval { get; set; }

        public int intval { get; set; }

        public bool boolval { get; set; }

        public decimal decimalval { get; set; }

        public DateTime datetimeval { get; set; }

        public List<SubClass> ListsubclassVal { get; set; }

        public SubClass subclassVal { get; set; }

        public Enumcustomer enumtype { get; set; }

    }

    public class SubClass
    {
        public string substringval { get; set; }
    }

    public enum Enumcustomer
    {
        enum1 = 0,
        enum2 = 1
    }
}
