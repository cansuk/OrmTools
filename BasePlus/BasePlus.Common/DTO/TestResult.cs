using System;
using System.Collections.Generic;
using System.Text;

namespace BasePlus.Common.DTO
{
    public class TestResult
    {
        public int OperationOrderNo { get; set; }
        public OperationType OperationType { get; set; }
        public OrmType OrmType { get; set; }
        public DbName DbName { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime OperationStartTime { get; set; }
        public DateTime OperationEndTime { get; set; }
        public string TotalMilisecond { get; set; }
        public string ErrorMessage { get; set; }
        public string AdditionalMessage { get; set; }
        public string TimeElsapsed { get; set; }
        public long RecordCount { get; set; }
        public bool IsSuccessful { get; set; }
        public string TestGuid { get; set; }
    }
}
