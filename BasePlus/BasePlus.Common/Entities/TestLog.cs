using System;
using System.Collections.Generic;
using System.Text;

namespace BasePlus.Common.Entities
{
    public class TestLog
    {
        public long Id { get; set; }
        public string DbName { get; set; }
        public string Operation { get; set; }
        public string Orm { get; set; }
        public bool IsTransaction { get; set; }
        public string TimeElapsed{ get; set; }
        public int RecordCount { get; set; }
        public DateTime CreateDate { get; set; }
        public string JsonFile{ get; set; }
        public string TestGuid { get; set; }
    }
}
