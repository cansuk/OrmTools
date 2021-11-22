using BasePlus.BusinessContracts;
using BasePlus.Common;
using BasePlus.Common.DTO;
using BasePlus.Common.Entities;
using BasePlus.Data;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasePlus.Business
{
    public class DataHelper : IDataHelper
    {
        Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public List<TestLog> GetAllTestLogs()
        {
            List<TestLog> testLogs = new List<TestLog>();
            using (var context = new ApplicationDbContext())
            {
                testLogs = context.TestLog.ToList();
            }
            return testLogs;
        }

        public List<Score> GetScores()
        {
            List<Score> scores = new List<Score>();

            try
            {
                scores.AddRange(GetScoresWithOrm(OrmType.Ado));
                scores.AddRange(GetScoresWithOrm(OrmType.Dapper));
                scores.AddRange(GetScoresWithOrm(OrmType.EFCore));

                #region Tüm Score verilerinin dosyaya loglanması
                // logger.Info("{@value1}", scores);  // Tüm Ortalama Skorların yazdırılması 
                #endregion Tüm Score verilerinin dosyaya loglanması

                #region Score verilerinin db ye kaydedilmesi
                try
                {
                    using (var context = new ApplicationDbContext())
                    {
                        context.Score.AddRange(scores);
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                #endregion Score verilerinin db ye kaydedilmesi

                #region best-case loglarının yazdırılması
                //CalculateBestScores(scores, DbName.Postgre);
                //CalculateBestScores(scores, DbName.MsSql);
                //CalculateBestScores(scores, DbName.All);
                #endregion best-case loglarının yazdırılması
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return scores;
        }

        void CalculateBestScores(List<Score> scores, DbName dbType)
        {
            switch (dbType)
            {
                case DbName.All:
                    logger.Info("BEST SCORES");
                    #region BEST SCORES


                    var minSelectTime = from res in scores
                                        where res.OperationType == OperationType.Select
                                        group res by res.OrmType into resultGroup
                                        select new
                                        {
                                            minSelectTime = resultGroup.Min(x => Convert.ToDouble(x.AvgTimeElapsed)),
                                            RecordCount = resultGroup.Select(x => x.RecordCount.ToString()).FirstOrDefault(),
                                            OperationType = resultGroup.Select(x => x.OperationType.ToString()).FirstOrDefault(),
                                            DbName = resultGroup.Select(x => x.DbName.ToString()).FirstOrDefault(),
                                            OrmType = resultGroup.Key
                                        };
                    Console.WriteLine(minSelectTime);
                    
                    logger.Info("{@value1}", minSelectTime);


                    var minInsertTime = from res in scores
                                        where res.OperationType == OperationType.Insert
                                        group res by res.OrmType into resultGroup
                                        select new
                                        {
                                            minInsertTime = resultGroup.Min(x => Convert.ToDouble(x.AvgTimeElapsed)),
                                            RecordCount = resultGroup.Select(x => x.RecordCount.ToString()).FirstOrDefault(),
                                            OperationType = resultGroup.Select(x => x.OperationType.ToString()).FirstOrDefault(),
                                            DbName = resultGroup.Select(x => x.DbName.ToString()).FirstOrDefault(),
                                            OrmType = resultGroup.Key
                                        };
                                                        
                    
                    logger.Info("{@value1}", minInsertTime);

                    var minUpdateTime = from res in scores
                                        where res.OperationType == OperationType.Update
                                        group res by res.OrmType into resultGroup
                                        select new
                                        {
                                            minUpdateTime = resultGroup.Min(x => Convert.ToDouble(x.AvgTimeElapsed)),
                                            RecordCount = resultGroup.Select(x => x.RecordCount.ToString()).FirstOrDefault(),
                                            OperationType = resultGroup.Select(x => x.OperationType.ToString()).FirstOrDefault(),
                                            DbName = resultGroup.Select(x => x.DbName.ToString()).FirstOrDefault(),
                                            OrmType = resultGroup.Key
                                        };
                    Console.WriteLine(minUpdateTime);
                    
                    logger.Info("{@value1}", minUpdateTime);


                    #endregion BEST SCORES

                    // logger.Info("WORST SCORES");
                    #region WORST SCORES

                    //minSelectTime = from res in scores
                    //                where res.OperationType == OperationType.Select
                    //                group res by res.RecordCount into resultGroup
                    //                select new
                    //                {
                    //                    minSelectTime = resultGroup.Max(x => Convert.ToDouble(x.AvgTimeElapsed)),
                    //                    RecordCount = resultGroup.Key,
                    //                    OperationType = resultGroup.Select(x => x.OperationType.ToString()).FirstOrDefault(),
                    //                    DbName = resultGroup.Select(x => x.DbName.ToString()).FirstOrDefault(),
                    //                    OrmType = resultGroup.Select(x => x.OrmType.ToString()).FirstOrDefault()
                    //                };
                    //Console.WriteLine(minSelectTime);
                    
                    //logger.Info("{@value1}", minSelectTime);



                    //minInsertTime = from res in scores
                    //                where res.OperationType == OperationType.Insert
                    //                group res by res.RecordCount into resultGroup
                    //                select new
                    //                {
                    //                    minInsertTime = resultGroup.Max(x => Convert.ToDouble(x.AvgTimeElapsed)),
                    //                    RecordCount = resultGroup.Key,
                    //                    OperationType = resultGroup.Select(x => x.OperationType.ToString()).FirstOrDefault(),
                    //                    DbName = resultGroup.Select(x => x.DbName.ToString()).FirstOrDefault(),
                    //                    OrmType = resultGroup.Select(x => x.OrmType.ToString()).FirstOrDefault()
                    //                };
                    //Console.WriteLine(minInsertTime);
                    
                    //logger.Info("{@value1}", minInsertTime);

                    //minUpdateTime = from res in scores
                    //                where res.OperationType == OperationType.Update
                    //                group res by res.RecordCount into resultGroup
                    //                select new
                    //                {
                    //                    minUpdateTime = resultGroup.Max(x => Convert.ToDouble(x.AvgTimeElapsed)),
                    //                    RecordCount = resultGroup.Key,
                    //                    OperationType = resultGroup.Select(x => x.OperationType.ToString()).FirstOrDefault(),
                    //                    DbName = resultGroup.Select(x => x.DbName.ToString()).FirstOrDefault(),
                    //                    OrmType = resultGroup.Select(x => x.OrmType.ToString()).FirstOrDefault()
                    //                };
                    //Console.WriteLine(minUpdateTime);
                    
                    //logger.Info("{@value1}", minUpdateTime);


                    #endregion WORST SCORES
                    break;
                case DbName.MsSql:
                case DbName.Postgre:
                    logger.Info("BEST SCORES");
                    #region BEST SCORES

                    minSelectTime = from res in scores
                                    where res.OperationType == OperationType.Select &&
                                    res.DbName == dbType
                                    group res by res.OrmType into resultGroup
                                    select new
                                    {
                                        minSelectTime = resultGroup.Min(x => Convert.ToDouble(x.AvgTimeElapsed)),
                                        RecordCount = resultGroup.Select(x => x.RecordCount.ToString()).FirstOrDefault(),
                                        OperationType = resultGroup.Select(x => x.OperationType.ToString()).FirstOrDefault(),
                                        DbName = resultGroup.Select(x => x.DbName.ToString()).FirstOrDefault(),
                                        OrmType = resultGroup.Key
                                    };
                    Console.WriteLine(minSelectTime);
                    
                    logger.Info("{@value1}", minSelectTime);

                    minInsertTime = from res in scores
                                         where res.OperationType == OperationType.Insert
                                         group res by res.OrmType into resultGroup
                                         select new
                                         {
                                             minInsertTime = resultGroup.Min(x => Convert.ToDouble(x.AvgTimeElapsed)),
                                             RecordCount = resultGroup.Select(x => x.RecordCount.ToString()).FirstOrDefault(),
                                             OperationType = resultGroup.Select(x => x.OperationType.ToString()).FirstOrDefault(),
                                             DbName = resultGroup.Select(x => x.DbName.ToString()).FirstOrDefault(),
                                             OrmType = resultGroup.Key
                                         };

                    Console.WriteLine(minInsertTime);

                    logger.Info("{@value1}", minInsertTime);


                    minUpdateTime = from res in scores
                                    where res.OperationType == OperationType.Update &&
                                    res.DbName == dbType
                                    group res by res.OrmType into resultGroup
                                    select new
                                    {
                                        minUpdateTime = resultGroup.Min(x => Convert.ToDouble(x.AvgTimeElapsed)),
                                        RecordCount = resultGroup.Select(x => x.RecordCount.ToString()).FirstOrDefault(),
                                        OperationType = resultGroup.Select(x => x.OperationType.ToString()).FirstOrDefault(),
                                        DbName = resultGroup.Select(x => x.DbName.ToString()).FirstOrDefault(),
                                        OrmType = resultGroup.Key
                                    };
                    Console.WriteLine(minUpdateTime);
                    
                    logger.Info("{@value1}", minUpdateTime);


                    #endregion BEST SCORES

                    //logger.Info("WORST SCORES");
                    //#region WORST SCORES

                    //minSelectTime = from res in scores
                    //                where res.OperationType == OperationType.Select &&
                    //                res.DbName == dbType
                    //                group res by res.RecordCount into resultGroup
                    //                select new
                    //                {
                    //                    minSelectTime = resultGroup.Max(x => Convert.ToDouble(x.AvgTimeElapsed)),
                    //                    RecordCount = resultGroup.Key,
                    //                    OperationType = resultGroup.Select(x => x.OperationType.ToString()).FirstOrDefault(),
                    //                    DbName = resultGroup.Select(x => x.DbName.ToString()).FirstOrDefault(),
                    //                    OrmType = resultGroup.Select(x => x.OrmType.ToString()).FirstOrDefault()                                        
                    //                };
                    //Console.WriteLine(minSelectTime);
                    
                    //logger.Info("{@value1}", minSelectTime);



                    //minInsertTime = from res in scores
                    //                where res.OperationType == OperationType.Insert &&
                    //                res.DbName == dbType
                    //                group res by res.RecordCount into resultGroup
                    //                select new
                    //                {
                    //                    minInsertTime = resultGroup.Max(x => Convert.ToDouble(x.AvgTimeElapsed)),
                    //                    RecordCount = resultGroup.Key,
                    //                    OperationType = resultGroup.Select(x => x.OperationType.ToString()).FirstOrDefault(),
                    //                    DbName = resultGroup.Select(x => x.DbName.ToString()).FirstOrDefault(),
                    //                    OrmType = resultGroup.Select(x => x.OrmType.ToString()).FirstOrDefault()                                        
                    //                };
                    //Console.WriteLine(minInsertTime);
                    
                    //logger.Info("{@value1}", minInsertTime);


                    //minUpdateTime = from res in scores
                    //                where res.OperationType == OperationType.Update &&
                    //                res.DbName == dbType
                    //                group res by res.RecordCount into resultGroup
                    //                select new
                    //                {
                    //                    minUpdateTime = resultGroup.Max(x => Convert.ToDouble(x.AvgTimeElapsed)),
                    //                    RecordCount = resultGroup.Key,
                    //                    OperationType = resultGroup.Select(x => x.OperationType.ToString()).FirstOrDefault(),
                    //                    DbName = resultGroup.Select(x => x.DbName.ToString()).FirstOrDefault(),
                    //                    OrmType = resultGroup.Select(x => x.OrmType.ToString()).FirstOrDefault()
                    //                };
                    //Console.WriteLine(minUpdateTime);
                    
                    //logger.Info("{@value1}", minUpdateTime);

                    //#endregion WORST SCORES
                    break;
                default:
                    break;
            }

        }

        private IEnumerable<Score> GetScoresWithOrm(OrmType ormType)
        {
            List<Score> scores = new List<Score>();
            List<TestLog> logs = GetAllTestLogs();

            try
            {
                #region [ORM TYPE] POSTGRE

                #region insert

                // [ORM TYPE] Postgre Insert 1 
                var orm_postgre_insert_1 = logs.Where(log => log.Orm == ormType.ToString() && log.DbName == DbName.Postgre.ToString() &&
                log.Operation == OperationType.Insert.ToString() && log.RecordCount == Convert.ToInt64(RecordCount.Record_1));

                var average = orm_postgre_insert_1.Sum(log => Convert.ToDouble(log.TimeElapsed)) / orm_postgre_insert_1.Count();

                Score score = new Score();
                score.OrmType = ormType;
                score.DbName = DbName.Postgre;
                score.OperationType = OperationType.Insert;
                score.RecordCount = RecordCount.Record_1;
                score.AvgTimeElapsed = average.ToString();

                scores.Add(score);

                // [ORM TYPE] Postgre Insert 100
                var orm_postgre_insert_100 = logs.Where(log => log.Orm == ormType.ToString() && log.DbName == DbName.Postgre.ToString() &&
                log.Operation == OperationType.Insert.ToString() && log.RecordCount == Convert.ToInt64(RecordCount.Record_100));

                average = orm_postgre_insert_100.Sum(log => Convert.ToDouble(log.TimeElapsed)) / orm_postgre_insert_100.Count();
                score = new Score();
                score.OrmType = ormType;
                score.DbName = DbName.Postgre;
                score.OperationType = OperationType.Insert;
                score.RecordCount = RecordCount.Record_100;
                score.AvgTimeElapsed = average.ToString();
                scores.Add(score);

                // [ORM TYPE] Postgre Insert 1000
                var orm_postgre_insert_1000 = logs.Where(log => log.Orm == ormType.ToString() && log.DbName == DbName.Postgre.ToString() &&
                log.Operation == OperationType.Insert.ToString() && log.RecordCount == Convert.ToInt64(RecordCount.Record_1000));

                average = orm_postgre_insert_1000.Sum(log => Convert.ToDouble(log.TimeElapsed)) / orm_postgre_insert_1000.Count();

                score = new Score();
                score.OrmType = ormType;
                score.DbName = DbName.Postgre;
                score.OperationType = OperationType.Insert;
                score.RecordCount = RecordCount.Record_1000;
                score.AvgTimeElapsed = average.ToString();
                scores.Add(score);

                #endregion insert

                #region select

                // [ORM TYPE] Postgre Select 1 
                var orm_postgre_select_1 = logs.Where(log => log.Orm == ormType.ToString() && log.DbName == DbName.Postgre.ToString() &&
                log.Operation == OperationType.Select.ToString() && log.RecordCount == Convert.ToInt64(RecordCount.Record_1));

                average = orm_postgre_select_1.Sum(log => Convert.ToDouble(log.TimeElapsed)) / orm_postgre_select_1.Count();

                score = new Score();
                score.OrmType = ormType;
                score.DbName = DbName.Postgre;
                score.OperationType = OperationType.Select;
                score.RecordCount = RecordCount.Record_1;
                score.AvgTimeElapsed = average.ToString();
                scores.Add(score);

                // [ORM TYPE] Postgre Select 100
                var orm_postgre_select_100 = logs.Where(log => log.Orm == ormType.ToString() && log.DbName == DbName.Postgre.ToString() &&
                log.Operation == OperationType.Select.ToString() && log.RecordCount == Convert.ToInt64(RecordCount.Record_100));

                average = orm_postgre_select_100.Sum(log => Convert.ToDouble(log.TimeElapsed)) / orm_postgre_select_100.Count();

                score = new Score();
                score.OrmType = ormType;
                score.DbName = DbName.Postgre;
                score.OperationType = OperationType.Select;
                score.RecordCount = RecordCount.Record_100;
                score.AvgTimeElapsed = average.ToString();
                scores.Add(score);

                // [ORM TYPE] Postgre Select 1000
                var orm_postgre_select_1000 = logs.Where(log => log.Orm == ormType.ToString() && log.DbName == DbName.Postgre.ToString() &&
                log.Operation == OperationType.Select.ToString() && log.RecordCount == Convert.ToInt64(RecordCount.Record_1000));

                average = orm_postgre_select_1000.Sum(log => Convert.ToDouble(log.TimeElapsed)) / orm_postgre_select_1000.Count();

                score = new Score();
                score.OrmType = ormType;
                score.DbName = DbName.Postgre;
                score.OperationType = OperationType.Select;
                score.RecordCount = RecordCount.Record_1000;
                score.AvgTimeElapsed = average.ToString();
                scores.Add(score);

                #endregion select

                #region update

                // [ORM TYPE] Postgre Update 1 
                var orm_postgre_update_1 = logs.Where(log => log.Orm == ormType.ToString() && log.DbName == DbName.Postgre.ToString() &&
                log.Operation == OperationType.Update.ToString() && log.RecordCount == Convert.ToInt64(RecordCount.Record_1));

                average = orm_postgre_update_1.Sum(log => Convert.ToDouble(log.TimeElapsed)) / orm_postgre_update_1.Count();

                score = new Score();
                score.OrmType = ormType;
                score.DbName = DbName.Postgre;
                score.OperationType = OperationType.Update;
                score.RecordCount = RecordCount.Record_1;
                score.AvgTimeElapsed = average.ToString();
                scores.Add(score);

                // [ORM TYPE] Postgre Update 100
                var orm_postgre_update_100 = logs.Where(log => log.Orm == ormType.ToString() && log.DbName == DbName.Postgre.ToString() &&
                log.Operation == OperationType.Update.ToString() && log.RecordCount == Convert.ToInt64(RecordCount.Record_100));

                average = orm_postgre_update_100.Sum(log => Convert.ToDouble(log.TimeElapsed)) / orm_postgre_update_100.Count();

                score = new Score();
                score.OrmType = ormType;
                score.DbName = DbName.Postgre;
                score.OperationType = OperationType.Update;
                score.RecordCount = RecordCount.Record_100;
                score.AvgTimeElapsed = average.ToString();
                scores.Add(score);

                // [ORM TYPE] Postgre Update 1000
                var orm_postgre_update_1000 = logs.Where(log => log.Orm == ormType.ToString() && log.DbName == DbName.Postgre.ToString() &&
                log.Operation == OperationType.Update.ToString() && log.RecordCount == Convert.ToInt64(RecordCount.Record_1000));

                average = orm_postgre_update_1000.Sum(log => Convert.ToDouble(log.TimeElapsed)) / orm_postgre_update_1000.Count();

                score = new Score();
                score.OrmType = ormType;
                score.DbName = DbName.Postgre;
                score.OperationType = OperationType.Update;
                score.RecordCount = RecordCount.Record_1000;
                score.AvgTimeElapsed = average.ToString();
                scores.Add(score);

                #endregion update

                #endregion [ORM TYPE] POSTGRE

                #region [ORM TYPE] MSSQL

                #region insert

                // [ORM TYPE] MSSQL Insert 1 
                var orm_mssql_insert_1 = logs.Where(log => log.Orm == ormType.ToString() && log.DbName == DbName.MsSql.ToString() &&
                log.Operation == OperationType.Insert.ToString() && log.RecordCount == Convert.ToInt64(RecordCount.Record_1));

                average = orm_mssql_insert_1.Sum(log => Convert.ToDouble(log.TimeElapsed)) / orm_mssql_insert_1.Count();

                score = new Score();
                score.OrmType = ormType;
                score.DbName = DbName.MsSql;
                score.OperationType = OperationType.Insert;
                score.RecordCount = RecordCount.Record_1;
                score.AvgTimeElapsed = average.ToString();
                scores.Add(score);

                // [ORM TYPE] MSSQL Insert 100
                var orm_mssql_insert_100 = logs.Where(log => log.Orm == ormType.ToString() && log.DbName == DbName.MsSql.ToString() &&
                log.Operation == OperationType.Insert.ToString() && log.RecordCount == Convert.ToInt64(RecordCount.Record_100));

                average = orm_mssql_insert_100.Sum(log => Convert.ToDouble(log.TimeElapsed)) / orm_mssql_insert_100.Count();

                score = new Score();
                score.OrmType = ormType;
                score.DbName = DbName.MsSql;
                score.OperationType = OperationType.Insert;
                score.RecordCount = RecordCount.Record_100;
                score.AvgTimeElapsed = average.ToString();
                scores.Add(score);

                // [ORM TYPE] MSSQL Insert 1000
                var orm_mssql_insert_1000 = logs.Where(log => log.Orm == ormType.ToString() && log.DbName == DbName.MsSql.ToString() &&
                log.Operation == OperationType.Insert.ToString() && log.RecordCount == Convert.ToInt64(RecordCount.Record_1000));

                average = orm_mssql_insert_1000.Sum(log => Convert.ToDouble(log.TimeElapsed)) / orm_mssql_insert_1000.Count();

                score = new Score();
                score.OrmType = ormType;
                score.DbName = DbName.MsSql;
                score.OperationType = OperationType.Insert;
                score.RecordCount = RecordCount.Record_1000;
                score.AvgTimeElapsed = average.ToString();
                scores.Add(score);

                #endregion insert

                #region select

                // [ORM TYPE] MSSQL Select 1 
                var orm_mssql_select_1 = logs.Where(log => log.Orm == ormType.ToString() && log.DbName == DbName.MsSql.ToString() &&
                log.Operation == OperationType.Select.ToString() && log.RecordCount == Convert.ToInt64(RecordCount.Record_1));

                average = orm_mssql_select_1.Sum(log => Convert.ToDouble(log.TimeElapsed)) / orm_mssql_select_1.Count();

                score = new Score();
                score.OrmType = ormType;
                score.DbName = DbName.MsSql;
                score.OperationType = OperationType.Select;
                score.RecordCount = RecordCount.Record_1;
                score.AvgTimeElapsed = average.ToString();
                scores.Add(score);

                // [ORM TYPE] MSSQL Select 100
                var orm_mssql_select_100 = logs.Where(log => log.Orm == ormType.ToString() && log.DbName == DbName.MsSql.ToString() &&
                log.Operation == OperationType.Select.ToString() && log.RecordCount == Convert.ToInt64(RecordCount.Record_100));

                average = orm_mssql_select_100.Sum(log => Convert.ToDouble(log.TimeElapsed)) / orm_mssql_select_100.Count();

                score = new Score();
                score.OrmType = ormType;
                score.DbName = DbName.MsSql;
                score.OperationType = OperationType.Select;
                score.RecordCount = RecordCount.Record_100;
                score.AvgTimeElapsed = average.ToString();
                scores.Add(score);

                // [ORM TYPE] MSSQL Select 1000
                var orm_mssql_select_1000 = logs.Where(log => log.Orm == ormType.ToString() && log.DbName == DbName.MsSql.ToString() &&
                log.Operation == OperationType.Select.ToString() && log.RecordCount == Convert.ToInt64(RecordCount.Record_1000));

                average = orm_mssql_select_1000.Sum(log => Convert.ToDouble(log.TimeElapsed)) / orm_mssql_select_1000.Count();

                score = new Score();
                score.OrmType = ormType;
                score.DbName = DbName.MsSql;
                score.OperationType = OperationType.Select;
                score.RecordCount = RecordCount.Record_1000;
                score.AvgTimeElapsed = average.ToString();
                scores.Add(score);

                #endregion select

                #region update

                // [ORM TYPE] MSSQL Update 1 
                var orm_mssql_update_1 = logs.Where(log => log.Orm == ormType.ToString() && log.DbName == DbName.MsSql.ToString() &&
                log.Operation == OperationType.Update.ToString() && log.RecordCount == Convert.ToInt64(RecordCount.Record_1));

                average = orm_mssql_update_1.Sum(log => Convert.ToDouble(log.TimeElapsed)) / orm_mssql_update_1.Count();

                score = new Score();
                score.OrmType = ormType;
                score.DbName = DbName.MsSql;
                score.OperationType = OperationType.Update;
                score.RecordCount = RecordCount.Record_1;
                score.AvgTimeElapsed = average.ToString();
                scores.Add(score);

                // [ORM TYPE] MSSQL Update 100
                var orm_mssql_update_100 = logs.Where(log => log.Orm == ormType.ToString() && log.DbName == DbName.MsSql.ToString() &&
                log.Operation == OperationType.Update.ToString() && log.RecordCount == Convert.ToInt64(RecordCount.Record_100));

                average = orm_mssql_update_100.Sum(log => Convert.ToDouble(log.TimeElapsed)) / orm_mssql_update_100.Count();

                score = new Score();
                score.OrmType = ormType;
                score.DbName = DbName.MsSql;
                score.OperationType = OperationType.Update;
                score.RecordCount = RecordCount.Record_100;
                score.AvgTimeElapsed = average.ToString();
                scores.Add(score);

                // [ORM TYPE] MSSQL Update 1000
                var orm_mssql_update_1000 = logs.Where(log => log.Orm == ormType.ToString() && log.DbName == DbName.MsSql.ToString() &&
                log.Operation == OperationType.Update.ToString() && log.RecordCount == Convert.ToInt64(RecordCount.Record_1000));

                average = orm_mssql_update_1000.Sum(log => Convert.ToDouble(log.TimeElapsed)) / orm_mssql_update_1000.Count();

                score = new Score();
                score.OrmType = ormType;
                score.DbName = DbName.MsSql;
                score.OperationType = OperationType.Update;
                score.RecordCount = RecordCount.Record_1000;
                score.AvgTimeElapsed = average.ToString();
                scores.Add(score);

                #endregion update

                #endregion [] MSSQL
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            
            return scores;
        }
    
    

    }

}
