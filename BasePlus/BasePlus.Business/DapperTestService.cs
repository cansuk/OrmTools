using BasePlus.Common;
using BasePlus.Common.API;
using BasePlus.Common.DTO;
using BasePlus.Common.Entities;
using Dapper;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace BasePlus.Business
{
    public static class DapperTestService
    {
        static string sqlcon = DbHelper.SqlConnectionString;
        static string postgrecon = DbHelper.PostgreConnectionString;
        static SqlConnection connection = new SqlConnection(sqlcon);
        static NpgsqlConnection connectionPostgre = new NpgsqlConnection(postgrecon);
        public static List<TestResult> TestPostgreDapperSelect()
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

        public static List<TestResult> TestPostgreDapperInsert()
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

        public static List<TestResult> TestPostgreDapperUpdate()
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

        public static List<TestResult> TestMSSQLDapperSelect()
        {
            List<TestResult> testResults = new List<TestResult>();
            DbHelper.ResetMssql();
            testResults.AddRange(SelectMSSQL(1));
            DbHelper.ResetMssql();
            testResults.AddRange(SelectMSSQL(100));
            DbHelper.ResetMssql();
            testResults.AddRange(SelectMSSQL(1000));
            return testResults;

        }

        public static List<TestResult> TestMSSQLDapperInsert()
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

        public static List<TestResult> TestMSSQLDapperUpdate()
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

                var query = "INSERT INTO MOCKUSER (jobtitle, email, name, createdate, gender, phone, city, address, country, active) VALUES (@jobtitle, @email, @name, @createdate, @gender, @phone, @city, @address, @country, @active)";

                stopwatch.Start();

                using (var tran = connection.BeginTransaction())
                {
                    int affectedRows = connection.Execute(query, users, transaction: tran);
                    tran.Commit();
                }

                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Insert,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Dapper,
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
                    OperationType = OperationType.Insert,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Dapper,
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
            connection.Open();
            List<TestResult> testResults = new List<TestResult>();
            Stopwatch stopwatch = new Stopwatch();
            List<mockuser> users = new List<mockuser>();

            try
            {
                var query = $"SELECT top " + count + " * FROM mockuser";

                stopwatch.Start();
                using (SqlTransaction tran = connection.BeginTransaction())
                {
                    IEnumerable<mockuser> mockusers = connection.Query<mockuser>(query, transaction: tran);
                    tran.Commit();
                }
                stopwatch.Stop();

                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Select,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Dapper,
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
                    OperationType = OperationType.Select,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Dapper,
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
            Stopwatch stopwatch = new Stopwatch();
            
            SqlConnection conn = new SqlConnection(sqlcon);
            try
            {
                stopwatch.Start();
                conn.Open();

                foreach (var id in idList)
                {
                    using (SqlTransaction tran = conn.BeginTransaction())
                    {
                        DynamicParameters parameters = new DynamicParameters();

                        string query = "UPDATE mockuser SET jobtitle = @jobtitle, name = @name, email =@email Where id = " + id;
                        parameters.Add("@jobtitle", "Dentist" + Guid.NewGuid().ToString(), DbType.String, ParameterDirection.Input);
                        parameters.Add("@name", "Josh Evans" + Guid.NewGuid().ToString(), DbType.String, ParameterDirection.Input);
                        parameters.Add("@email", "Rae_John298@bretoux.com" + Guid.NewGuid().ToString(), DbType.String, ParameterDirection.Input);


                        var affectedRows = conn.ExecuteScalar(query, parameters,transaction:tran, commandType: CommandType.Text);
                        tran.Commit();
                    }
                }                
                conn.Close();
                stopwatch.Stop();

                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Update,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Dapper,
                    DbName = DbName.MsSql,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    RecordCount = count,
                    IsSuccessful = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                conn.Close();
                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Update,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Dapper,
                    DbName = DbName.MsSql,
                    IsSuccessful = false,
                    TestGuid = "",
                    ErrorMessage = ex.ToString()

                });
            }

            return testResults;
        }


        // -----------------------------------------------------------------------------------------------------------------------------
        public static List<TestResult> InsertPostgre(int count)
        {
            connectionPostgre.Close();
            connectionPostgre.Open();
            List<TestResult> testResults = new List<TestResult>();
            Stopwatch stopwatch = new Stopwatch();
            List<mockuser> users = new List<mockuser>();
            RootObject rootObject = new RootObject();
            try
            {
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

                var query = "INSERT INTO MOCKUSER (jobtitle, email, name, createdate, gender, phone, city, address, country, active) VALUES (@jobtitle, @email, @name, @createdate, @gender, @phone, @city, @address, @country, @active)";

                stopwatch.Start();

                using (var tran = connectionPostgre.BeginTransaction())
                {
                    int affectedRows = connectionPostgre.Execute(query, users, transaction: tran);
                    tran.Commit();
                }

                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Insert,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Dapper,
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
                    OperationType = OperationType.Insert,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Dapper,
                    DbName = DbName.Postgre,
                    IsSuccessful = false,
                    TestGuid = "",
                    ErrorMessage = ex.ToString()

                });
            }

            connectionPostgre.Close();
            return testResults;
        }

        public static List<TestResult> SelectPostgre(int count)
        {
            connectionPostgre.Open();
            List<TestResult> testResults = new List<TestResult>();
            Stopwatch stopwatch = new Stopwatch();
            List<mockuser> users = new List<mockuser>();

            try
            {
                var query = $"SELECT * FROM mockuser limit " + count;
                stopwatch.Start();

                using (NpgsqlTransaction tran = connectionPostgre.BeginTransaction())
                {
                    IEnumerable<mockuser> mockusers = connectionPostgre.Query<mockuser>(query, transaction: tran);
                }

                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Select,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Dapper,
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
                    OperationType = OperationType.Select,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Dapper,
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
            Stopwatch stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                connectionPostgre.Open();
                foreach (var id in idList)
                {
                    DynamicParameters parameters = new DynamicParameters();

                    string query = "UPDATE mockuser SET jobtitle = @jobtitle, name = @name, email =@email Where id = " + id;
                    parameters.Add("@jobtitle", "Dentist" + Guid.NewGuid().ToString(), DbType.String, ParameterDirection.Input);
                    parameters.Add("@name", "Josh Evans" + Guid.NewGuid().ToString(), DbType.String, ParameterDirection.Input);
                    parameters.Add("@email", "Rae_John298@bretoux.com" + Guid.NewGuid().ToString(), DbType.String, ParameterDirection.Input);

                    using (NpgsqlTransaction tran = connectionPostgre.BeginTransaction())
                    {
                        var affectedRows = connectionPostgre.ExecuteScalar(query, parameters, transaction: tran, commandType: CommandType.Text);
                        tran.Commit();
                    }
                }
                stopwatch.Stop();
                connectionPostgre.Close();

                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Update,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Dapper,
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
                    OrmType = OrmType.Dapper,
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
