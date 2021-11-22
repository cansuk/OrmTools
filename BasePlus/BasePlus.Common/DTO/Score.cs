using System;
using System.Collections.Generic;
using System.Text;

namespace BasePlus.Common.DTO
{
    public class Score
    {
        public int Id { get; set; }
        public ScoreType ScoreType { get; set; }
        public OperationType OperationType { get; set; }
        public OrmType OrmType { get; set; }
        public DbName DbName { get; set; }
        public RecordCount RecordCount { get; set; }
        public string AvgTimeElapsed { get; set; }

    }
}
