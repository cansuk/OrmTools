using BasePlus.BusinessContracts;
using BasePlus.Common;
using BasePlus.Common.API;
using BasePlus.Common.DTO;
using BasePlus.Common.Entities;
using BasePlus.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Newtonsoft.Json;
using NLog;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace BasePlus.Business
{
    public class OrmService : IOrmService
    {        
        readonly IConfiguration configuration;
        readonly IDataHelper dataHelper;
        string sqlcon = DbHelper.SqlConnectionString;
        string postgrecon = DbHelper.PostgreConnectionString;
        Logger logger = NLog.LogManager.GetCurrentClassLogger();
        static int counter = 0;
        public OrmService(IConfiguration _configuration, IDataHelper _dataHelper)
        {
            configuration = _configuration;
            dataHelper = _dataHelper;
        }

        //public List<TestResult> TestOrms(int testCount)
        //{
        //    List<TestResult> results = new List<TestResult>();
        //    try
        //    {
        //        for (int i = 0; i <= testCount; i++)
        //        {
        //            // ilk testin sayılmaması istendi.
        //            try
        //            {
        //                //results.AddRange(TestPostgreContrib());
        //                //results.AddRange(TestPostgreADO());
        //                //results.AddRange(TestPostgreDapper());
        //                //results.AddRange(TestPostgreEF());

        //                ////results.AddRange(TestMSSQLContrib());
        //                //results.AddRange(TestMSSQLADO());
        //                //results.AddRange(TestMSSQLDapper());
        //                results.AddRange(TestMSSQLEF());

        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine(ex);
        //                continue;
        //            }

        //            if (i == 0)
        //            {
        //                continue;
        //            }
        //            logger.Info("{@value1}", results);
        //          //  LogResults(results, i);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }

        //    return results;
        //}

        public List<TestResult> TestOrms()
        {
            List<TestResult> results = new List<TestResult>();
            List<TestLog> testLogs = new List<TestLog>();

            try
            {
                //results.AddRange(TestMSSQLADO());
                //results.AddRange(TestPostgreADO());

                //results.AddRange(TestMSSQLDapper());
                //results.AddRange(TestPostgreDapper());

                //results.AddRange(TestMSSQLEF());
                //results.AddRange(TestPostgreEF());


                // kaldırılacak :
                //results.AddRange(TestMSSQLContrib());
                //results.AddRange(TestPostgreContrib());

                string jsonContent = JsonConvert.SerializeObject(results);

                if (counter == 0)
                {
                    counter++;
                    return new List<TestResult>();
                }                

                // Sonuçların db ye kaydedilmesi :
                ApplicationDbContext context = new ApplicationDbContext();
                string guid = Guid.NewGuid().ToString();

                results.ForEach(item =>
                {
                    item.TestGuid = guid;
                    if (item.IsSuccessful)
                    {
                        context.TestLog.Add(new TestLog()
                        {
                            DbName = item.DbName.ToString(),
                            Orm = item.OrmType.ToString(),
                            Operation = item.OperationType.ToString(),
                            TimeElapsed = item.TotalMilisecond.ToString(),
                            RecordCount = Convert.ToInt32(item.RecordCount),
                            CreateDate = DateTime.Now,
                            JsonFile = jsonContent,
                            TestGuid = guid
                        });
                    }
                });
                context.SaveChanges();

                // Sonuçların dosyaya loglanması :
                logger.Info("{@value1}", results);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


            return results;
        }


        #region POSTGRE
        List<TestResult> TestPostgreADO()
        {
            List<TestResult> results = new List<TestResult>();

            results.AddRange(AdoTestService.TestPostgreADOInsert());
            results.AddRange(AdoTestService.TestPostgreADOUpdate());
            results.AddRange(AdoTestService.TestPostgreADOSelect());

            return results;
        }

        List<TestResult> TestPostgreDapper()
        {
            List<TestResult> results = new List<TestResult>();

            results.AddRange(DapperTestService.TestPostgreDapperInsert());
            results.AddRange(DapperTestService.TestPostgreDapperUpdate());
            results.AddRange(DapperTestService.TestPostgreDapperSelect());

            return results;
        }

        List<TestResult> TestPostgreContrib()
        {
            List<TestResult> results = new List<TestResult>();

            results.AddRange(ContribTestService.TestPostgreContribInsert());
            results.AddRange(ContribTestService.TestPostgreContribUpdate());
            results.AddRange(ContribTestService.TestPostgreContribSelect());

            return results;
        }

        List<TestResult> TestPostgreEF()
        {
            List<TestResult> results = new List<TestResult>();

            results.AddRange(EFTestService.TestPostgreEFInsert());
            results.AddRange(EFTestService.TestPostgreEFUpdate());
            results.AddRange(EFTestService.TestPostgreEFSelect());

            return results;
        }

        List<TestResult> TestPostgreGIS()
        {
            List<TestResult> results = new List<TestResult>();

            results.AddRange(GISTestService.TestPostgreGISInsert());
            results.AddRange(GISTestService.TestPostgreGISUpdate());
            results.AddRange(GISTestService.TestPostgreGISSelect());

            return results;
        }

        #endregion POSTGRE

        #region MSSQL
        List<TestResult> TestMSSQLADO()
        {
            List<TestResult> results = new List<TestResult>();

            results.AddRange(AdoTestService.TestMSSQLADOInsert());
            results.AddRange(AdoTestService.TestMSSQLADOUpdate());
            results.AddRange(AdoTestService.TestMSSQLADOSelect());

            return results;
        }

        List<TestResult> TestMSSQLDapper()
        {
            List<TestResult> results = new List<TestResult>();

            results.AddRange(DapperTestService.TestMSSQLDapperInsert());
            results.AddRange(DapperTestService.TestMSSQLDapperUpdate());
            results.AddRange(DapperTestService.TestMSSQLDapperSelect());

            return results;
        }

        List<TestResult> TestMSSQLContrib()
        {
            List<TestResult> results = new List<TestResult>();

            results.AddRange(ContribTestService.TestMSSQLContribInsert());
            results.AddRange(ContribTestService.TestMSSQLContribUpdate());
            results.AddRange(ContribTestService.TestMSSQLContribSelect());

            return results;
        }

        List<TestResult> TestMSSQLEF()
        {
            List<TestResult> results = new List<TestResult>();

            results.AddRange(EFTestService.TestMSSQLEFInsert());
            results.AddRange(EFTestService.TestMSSQLEFUpdate());
            results.AddRange(EFTestService.TestMSSQLEFSelect());

            return results;
        }

        List<TestResult> TestMSSQLGIS()
        {
            List<TestResult> results = new List<TestResult>();

            results.AddRange(GISTestService.TestMSSQLGISInsert());
            results.AddRange(GISTestService.TestMSSQLGISUpdate());
            results.AddRange(GISTestService.TestMSSQLGISSelect());

            return results;
        }


        #endregion MSSQL

        public List<Score> GetAnalyzes()
        {
            List<Score> scores = dataHelper.GetScores();
            
            return scores;
        }

        #region silinecek
        public void LogResults(List<TestResult> testResults, int testIndex)
        {
            try
            {
                List<TestLog> testLogs = new List<TestLog>();

                // Logların json a yazılması :
                string fileGuid = Guid.NewGuid().ToString();
                string fileName = String.Concat(@"C:\DataFiles\Logs\", fileGuid);
                string path = fileName + ".json";
                string jsonContent = JsonConvert.SerializeObject(testResults);
                File.WriteAllText(path, jsonContent);

                // Logların txt dosyaya kaydedilmesi :
                var logPath = fileName + ".txt";
                var logFile = System.IO.File.Create(logPath);
                var logWriter = new System.IO.StreamWriter(logFile);
                logWriter.WriteLine("###################################################################################");
                logWriter.WriteLine("##########################################TEST-" + testIndex + "###################################");
                logWriter.WriteLine("###################################################################################");
                testResults.ForEach(item =>
                {
                    testLogs.Add(new TestLog()
                    {
                        DbName = item.DbName.ToString(),
                        Orm = item.OrmType.ToString(),
                        Operation = item.OperationType.ToString(),
                        TimeElapsed = item.TotalMilisecond.ToString(),
                        RecordCount = Convert.ToInt32(item.RecordCount),
                        CreateDate = DateTime.Now,
                        JsonFile = jsonContent
                    });

                    logWriter.WriteLine("#CredateDate : " + DateTime.Now);
                    logWriter.WriteLine("IsSuccessful : " + item.IsSuccessful);
                    logWriter.WriteLine("Db : " + item.DbName);
                    logWriter.WriteLine("Orm : " + item.OrmType);
                    logWriter.WriteLine("Operation : " + item.OperationType);
                    logWriter.WriteLine("Transaction : " + item.TransactionType);
                    logWriter.WriteLine("TimeElapsed : " + item.TotalMilisecond);
                    logWriter.WriteLine("RecordCount : " + item.RecordCount);
                    logWriter.WriteLine("============================================================================");
                });

                logWriter.Dispose();

                // Logların veritabanına kaydedilmesi :
                using (var context = new ApplicationDbContext())
                {
                    context.TestLog.AddRange(testLogs);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
        #endregion silinecek

    }
}
