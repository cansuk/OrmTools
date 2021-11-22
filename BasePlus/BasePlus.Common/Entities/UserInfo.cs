using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace BasePlus.Common.Entities
{
    [Table("userinfo")]
    public class UserInfo 
    {
        public int id { get; set; }
        public int userid { get; set; }
        public string creditcardnumber { get; set; }
        public string iban { get; set; }
        public string cardtype { get; set; }
        public decimal currentbalance { get; set; }
    }
    [Table("userinfo")]
    public class userinfo
    {
        public int id { get; set; }
        public int userid { get; set; }
        public string creditcardnumber { get; set; }
        public string iban { get; set; }
        public string cardtype { get; set; }
        public decimal currentbalance { get; set; }
    }
}
