using System;
using System.Collections.Generic;
using System.Web;

namespace BasePlus.Common.API
{
    public class Object
    {
        public string jobtitle { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string createdate { get; set; }
        public string active { get; set; }
        public string gender { get; set; }
        public string phone { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string country { get; set; }
    }

    public class RootObject
    {
        public List<Object> objects { get; set; }
    }

}
