using BasePlus.Common.DTO;
using BasePlus.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasePlus.BusinessContracts
{
    public interface IDataHelper
    {
        public List<TestLog> GetAllTestLogs();
        public List<Score> GetScores();

    }
}
