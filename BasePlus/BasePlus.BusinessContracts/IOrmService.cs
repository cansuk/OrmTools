using BasePlus.Common.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using BasePlus;

namespace BasePlus.BusinessContracts
{
    public interface IOrmService
    {
        public List<TestResult> TestOrms();
        public List<Score> GetAnalyzes();
    }
}
