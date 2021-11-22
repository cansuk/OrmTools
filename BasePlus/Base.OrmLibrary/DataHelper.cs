using BasePlus.Common.DTO;
using BasePlus.Common.Entities;
using BasePlus.Data;
using System;
using System.Collections.Generic;

namespace Base.OrmLibrary
{
    public static class DataHelper
    {
        public static bool SaveTestLogs(List<TestLog> logs)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    context.TestLog.AddRange(logs);
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool SaveScores(List<Score> scores)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    context.Score.AddRange(scores);
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
