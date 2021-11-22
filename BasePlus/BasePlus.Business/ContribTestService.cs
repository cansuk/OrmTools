using BasePlus.Common;
using BasePlus.Common.API;
using BasePlus.Common.DTO;
using BasePlus.Common.Entities;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace BasePlus.Business
{
    public static class ContribTestService
    {
        static string sqlcon = DbHelper.SqlConnectionString;
        static string postgrecon = DbHelper.PostgreConnectionString;
        static SqlConnection connection = new SqlConnection(sqlcon);
        static NpgsqlConnection connectionPostgre = new NpgsqlConnection(postgrecon);

        public static List<TestResult> TestPostgreContribSelect()
        {
            List<TestResult> testResults = new List<TestResult>();
            DbHelper.ResetPostgre();
            testResults.AddRange(SelectPostgre(1));
            DbHelper.ResetPostgre();
            testResults.AddRange(SelectPostgre(100));
            DbHelper.ResetPostgre();
            testResults.AddRange(SelectPostgre(1000));
            return testResults;

        }

        public static List<TestResult> TestPostgreContribInsert()
        {
            List<TestResult> testResults = new List<TestResult>();
            DbHelper.ResetPostgre();
            testResults.AddRange(InsertPostgre(1));
            DbHelper.ResetPostgre();
            testResults.AddRange(InsertPostgre(100));
            DbHelper.ResetPostgre();
            testResults.AddRange(InsertPostgre(1000));
            return testResults;

        }

        public static List<TestResult> TestPostgreContribUpdate()
        {
            List<TestResult> testResults = new List<TestResult>();
            DbHelper.ResetPostgre();
            testResults.AddRange(UpdatePostgre(1));
            DbHelper.ResetPostgre();
            testResults.AddRange(UpdatePostgre(100));
            DbHelper.ResetPostgre();
            testResults.AddRange(UpdatePostgre(1000));
            return testResults;

        }

        public static List<TestResult> TestMSSQLContribSelect()
        {
            List<TestResult> testResults = new List<TestResult>();
            DbHelper.ResetMssql();
            testResults.AddRange(SelectMSSQL(1));
            DbHelper.ResetMssql();
            testResults.AddRange(SelectMSSQL(100));
            DbHelper.ResetMssql();
            testResults.AddRange(SelectMSSQL(1000));
            DbHelper.ResetMssql();
            return testResults;

        }

        public static List<TestResult> TestMSSQLContribInsert()
        {
            List<TestResult> testResults = new List<TestResult>();

            // 1, 100, 1000 kayıt ile :

            DbHelper.ResetMssql();
            testResults.AddRange(InsertMSSQL(1));
            DbHelper.ResetMssql();
            testResults.AddRange(InsertMSSQL(100));
            DbHelper.ResetMssql();
            testResults.AddRange(InsertMSSQL(1000));

            return testResults;
        }

        public static List<TestResult> TestMSSQLContribUpdate()
        {
            List<TestResult> testResults = new List<TestResult>();
            DbHelper.ResetMssql();
            testResults.AddRange(UpdateMSSQL(1));
            DbHelper.ResetMssql();
            testResults.AddRange(UpdateMSSQL(100));
            DbHelper.ResetMssql();
            testResults.AddRange(UpdateMSSQL(1000));
            return testResults;

        }

        // -----------------------------------------------------------------------------------------------------------------------------
        public static List<TestResult> InsertMSSQL(int count)
        {
            List<TestResult> testResults = new List<TestResult>();
                        try
            {
                connection.Open();
            long affectedRows = 0;
            Stopwatch stopwatch = new Stopwatch();
            List<mockuser> users = new List<mockuser>();
            RootObject rootObject = new RootObject();

                if (count == 1)
                {
                    string json = File.ReadAllText(DbHelper.MockUserJson_1);
                    rootObject = JsonConvert.DeserializeObject<RootObject>(json);
                }
                else if (count == 100)
                {
                    string json = File.ReadAllText(DbHelper.MockUserJson_100);
                    rootObject = JsonConvert.DeserializeObject<RootObject>(json);
                }
                else if (count == 1000)
                {
                    string json = File.ReadAllText(DbHelper.MockUserJson_1000);
                    rootObject = JsonConvert.DeserializeObject<RootObject>(json);
                }

                // rootObject to users :
                rootObject.objects.ToList().ForEach(x => users.Add(new mockuser()
                {
                    jobtitle = x.jobtitle,
                    email = x.email,
                    name = x.name,
                    createdate = DbHelper.RandomDate(),
                    gender = x.gender,
                    phone = x.phone,
                    city = x.city,
                    address = x.address,
                    country = x.country,
                    active = true
                }));

                stopwatch.Start();

                affectedRows = connection.Insert(users);

                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Insert,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Contrib,
                    DbName = DbName.MsSql,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    RecordCount = affectedRows,
                    IsSuccessful = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Insert,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Contrib,
                    DbName = DbName.MsSql,
                    IsSuccessful = false,
                    TestGuid = "",
                    ErrorMessage = ex.ToString()

                });
            }

            connection.Close();
            return testResults;
        }

        public static List<TestResult> SelectMSSQL(int count)
        {
            List<TestResult> testResults = new List<TestResult>();

            try
            {
            connection.Close();
            connection.Open();
            Stopwatch stopwatch = new Stopwatch();
            IEnumerable<mockuser> users = new List<mockuser>();
                stopwatch.Start();
                using (SqlTransaction tran = connection.BeginTransaction())
                {
                    users = connection.GetAll<mockuser>(transaction: tran);
                    tran.Commit();
                }
                stopwatch.Stop();

                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Select,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Contrib,
                    DbName = DbName.MsSql,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    RecordCount = users.Count(),
                    IsSuccessful = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Select,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Contrib,
                    DbName = DbName.MsSql,
                    IsSuccessful = false,
                    TestGuid = "",
                    ErrorMessage = ex.ToString()

                });
            }

            connection.Close();
            return testResults;
        }


        public static List<TestResult> UpdateMSSQL(int count)
        {
            List<Int64> idList = DbHelper.GetUpdateIdList(DbName.MsSql, count);
            List<TestResult> testResults = new List<TestResult>();
            List<mockuser> users = new List<mockuser>();
            Stopwatch stopwatch = new Stopwatch();

            try
            {
                foreach (var id in idList)
                {
                    var user = connection.Get<mockuser>(id);
                    user.jobtitle = "Dentist" + Guid.NewGuid().ToString();
                    user.name = "Josh Evans" + Guid.NewGuid().ToString();
                    user.email = "Rae_John298@bretoux.com" + Guid.NewGuid().ToString();

                    users.Add(user);
                }
                stopwatch.Start();
                connection.Close();
                connection.Open();

                connection.Update(users);

                stopwatch.Stop();
                connection.Close();

                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Update,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Contrib,
                    DbName = DbName.MsSql,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    RecordCount = count,
                    IsSuccessful = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Update,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Contrib,
                    DbName = DbName.MsSql,
                    IsSuccessful = false,
                    TestGuid = "",
                    ErrorMessage = ex.ToString()

                });
            }

            connection.Close();
            return testResults;
        }


        // -----------------------------------------------------------------------------------------------------------------------------
        public static List<TestResult> InsertPostgre(int count)
        {
            List<TestResult> testResults = new List<TestResult>();
            try
            {
                connectionPostgre.Close();
                connectionPostgre.Open();
                long affectedRows = 0;
                Stopwatch stopwatch = new Stopwatch();
                List<mockuser> users = new List<mockuser>();
                RootObject rootObject = new RootObject();

                if (count == 1)
                {
                    string json = File.ReadAllText(DbHelper.MockUserJson_1);
                    rootObject = JsonConvert.DeserializeObject<RootObject>(json);
                }
                else if (count == 100)
                {
                    string json = File.ReadAllText(DbHelper.MockUserJson_100);
                    rootObject = JsonConvert.DeserializeObject<RootObject>(json);
                }
                else if (count == 1000)
                {
                    string json = File.ReadAllText(DbHelper.MockUserJson_1000);
                    rootObject = JsonConvert.DeserializeObject<RootObject>(json);
                }

                // rootObject to users :
                rootObject.objects.ToList().ForEach(x => users.Add(new mockuser()
                {
                    jobtitle = x.jobtitle,
                    email = x.email,
                    name = x.name,
                    createdate = DbHelper.RandomDate(),
                    gender = x.gender,
                    phone = x.phone,
                    city = x.city,
                    address = x.address,
                    country = x.country,
                    active = true
                }));

                stopwatch.Start();

                affectedRows = connectionPostgre.Insert(users);

                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Insert,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Contrib,
                    DbName = DbName.Postgre,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    RecordCount = affectedRows,
                    IsSuccessful = true
                });
                connectionPostgre.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                connectionPostgre.Close();
                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Insert,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Contrib,
                    DbName = DbName.Postgre,
                    IsSuccessful = false,
                    TestGuid = "",
                    ErrorMessage = ex.ToString()

                });
            }

            
            return testResults;
        }

        public static List<TestResult> SelectPostgre(int count)
        {
            connectionPostgre.Open();
            List<TestResult> testResults = new List<TestResult>();
            Stopwatch stopwatch = new Stopwatch();
            IEnumerable<mockuser> users = new List<mockuser>();

            try
            {
                stopwatch.Start();
                using (NpgsqlTransaction tran = connectionPostgre.BeginTransaction())
                {
                    users = connectionPostgre.GetAll<mockuser>(transaction: tran);
                    tran.Commit();
                }
                stopwatch.Stop();

                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Select,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Contrib,
                    DbName = DbName.Postgre,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    RecordCount = users.Count(),
                    IsSuccessful = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Select,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Contrib,
                    DbName = DbName.Postgre,
                    IsSuccessful = false,
                    TestGuid = "",
                    ErrorMessage = ex.ToString()

                });
            }

            connectionPostgre.Close();
            return testResults;
        }

        public static List<TestResult> UpdatePostgre(int count)
        {
            List<Int64> idList = DbHelper.GetUpdateIdList(DbName.Postgre, count);
            List<TestResult> testResults = new List<TestResult>();
            List<mockuser> users = new List<mockuser>();
            Stopwatch stopwatch = new Stopwatch();

            try
            {
                foreach (var id in idList)
                {
                    var user = connectionPostgre.Get<mockuser>(id);
                    user.jobtitle = "Dentist" + Guid.NewGuid().ToString();
                    user.name = "Josh Evans" + Guid.NewGuid().ToString();
                    user.email = "Rae_John298@bretoux.com" + Guid.NewGuid().ToString();

                    users.Add(user);
                }
                stopwatch.Start();
                connectionPostgre.Open();

                connectionPostgre.Update(users);

                stopwatch.Stop();
                connectionPostgre.Close();

                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Update,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Contrib,
                    DbName = DbName.Postgre,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    RecordCount = count,
                    IsSuccessful = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Update,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Contrib,
                    DbName = DbName.Postgre,
                    IsSuccessful = false,
                    TestGuid = "",
                    ErrorMessage = ex.ToString()

                });
            }

            connectionPostgre.Close();
            return testResults;
        }



    }
}
