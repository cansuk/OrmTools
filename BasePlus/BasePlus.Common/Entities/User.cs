using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace BasePlus.Common.Entities
{
    public class User 
    {
        public int id { get; set; }
        public string jobtitle { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public bool? active { get; set; }
        public DateTime createdate { get; set; }
        public string gender { get; set; }
        public string phone { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string country { get; set; }
    }

    [Table("mockuser")]
    public class mockuser
    {
        public Int64 id { get; set; }
        public string jobtitle { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public bool? active { get; set; }
        public DateTime createdate { get; set; }
        public string gender { get; set; }
        public string phone { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string country { get; set; }
    }

}
