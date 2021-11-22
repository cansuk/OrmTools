using System;
using System.Web;

namespace BasePlus.Common.Entities
{
    public class MockUserInfo 
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
        public string creditcardnumber { get; set; }
        public string iban { get; set; }
        public string cardtype { get; set; }
        public decimal currentbalance { get; set; }
    }
}
