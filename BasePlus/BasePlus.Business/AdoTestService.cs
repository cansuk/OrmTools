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
    public static class AdoTestService
    {
        static string sqlcon = DbHelper.SqlConnectionString;
        static string postgrecon = DbHelper.PostgreConnectionString;
        static SqlConnection connection = new SqlConnection(sqlcon);
        static NpgsqlConnection connectionPostgre = new NpgsqlConnection(postgrecon);
        public static List<TestResult> TestPostgreADOSelect()
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

        public static List<TestResult> TestPostgreADOInsert()
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

        public static List<TestResult> TestPostgreADOUpdate()
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

        public static List<TestResult> TestMSSQLADOSelect()
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

        public static List<TestResult> TestMSSQLADOInsert()
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

        public static List<TestResult> TestMSSQLADOUpdate()
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
                connection.Close();
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


                stopwatch.Start();
                var query = "INSERT INTO MOCKUSER (jobtitle, email, name, createdate, gender, phone, city, address, country, active) " +
                            " VALUES (@jobtitle, @email, @name, @createdate, @gender, @phone, @city, @address, @country, @active)";
                foreach (var x in users)
                {
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        {
                            cmd.Parameters.AddWithValue("jobtitle", x.jobtitle);
                            cmd.Parameters.AddWithValue("email", x.email);
                            cmd.Parameters.AddWithValue("name", x.name);
                            cmd.Parameters.AddWithValue("createdate", x.createdate);
                            cmd.Parameters.AddWithValue("gender", x.gender);
                            cmd.Parameters.AddWithValue("phone", x.phone);
                            cmd.Parameters.AddWithValue("city", x.city);
                            cmd.Parameters.AddWithValue("active", true);
                            cmd.Parameters.AddWithValue("address", x.address);
                            cmd.Parameters.AddWithValue("country", x.country);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Insert,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Ado,
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
                    OrmType = OrmType.Ado,
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
            SqlConnection conn = connection;
            try
            {
                conn.Close();
                conn.Open();
                Stopwatch stopwatch = new Stopwatch();
                List<mockuser> users = new List<mockuser>();


                var query = $"SELECT top " + count + " * FROM mockuser";
                stopwatch.Start();

                using (var cmd = new SqlCommand(query, conn))
                {
                    {
                        cmd.CommandText = query;
                        SqlDataReader reader2 = cmd.ExecuteReader();
                    }
                }


                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Select,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Ado,
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
                    OperationType = OperationType.Select,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Ado,
                    DbName = DbName.MsSql,
                    IsSuccessful = false,
                    TestGuid = "",
                    ErrorMessage = ex.ToString()

                });
            }

            return testResults;
        }


        public static List<TestResult> UpdateMSSQL(int count)
        {
            List<TestResult> testResults = new List<TestResult>();
            try
            {
                List<Int64> idList = DbHelper.GetUpdateIdList(DbName.MsSql, count);
                Stopwatch stopwatch = new Stopwatch();


                connection.Close();
                connection.Open();

                stopwatch.Start();

                foreach (var id in idList)
                {
                    string query = "UPDATE mockuser SET jobtitle = @jobtitle, name = @name, email =@email Where id = " + id;

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.Add("jobtitle", SqlDbType.VarChar, 500).Value = "Dentist" + Guid.NewGuid().ToString();
                        cmd.Parameters.Add("name", SqlDbType.VarChar, 500).Value = "Josh Evans" + Guid.NewGuid().ToString();
                        cmd.Parameters.Add("email", SqlDbType.VarChar, 500).Value = "Rae_John298@bretoux.com" + Guid.NewGuid().ToString();
                        cmd.ExecuteNonQuery();
                    }
                }

                stopwatch.Stop();


                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Update,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Ado,
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
                    OrmType = OrmType.Ado,
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
                    // users.Take(count);
                }

                // rootObject to users :
                rootObject.objects.ToList().ForEach(x => users.Add(new mockuser()
                {
                    jobtitle = x.jobtitle,
                    email = x.email,
                    name = x.name,
                    createdate = DateTime.Parse(x.createdate),
                    gender = x.gender,
                    phone = x.phone,
                    city = x.city,
                    address = x.address,
                    country = x.country,
                    active = true
                }));

                stopwatch.Start();
                var query = "INSERT INTO MOCKUSER (jobtitle, email, name, createdate, gender, phone, city, address, country, active) " +
                            " VALUES (@jobtitle, @email, @name, @createdate, @gender, @phone, @city, @address, @country, @active)";
                foreach (var x in users)
                {
                    using (var cmd = new NpgsqlCommand(query, connectionPostgre))
                    {
                        {
                            cmd.Parameters.AddWithValue("jobtitle", x.jobtitle);
                            cmd.Parameters.AddWithValue("email", x.email);
                            cmd.Parameters.AddWithValue("name", x.name);
                            cmd.Parameters.AddWithValue("createdate", DbHelper.RandomDate());
                            cmd.Parameters.AddWithValue("gender", x.gender);
                            cmd.Parameters.AddWithValue("phone", x.phone);
                            cmd.Parameters.AddWithValue("city", x.city);
                            cmd.Parameters.AddWithValue("active", true);
                            cmd.Parameters.AddWithValue("address", x.address);
                            cmd.Parameters.AddWithValue("country", x.country);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Insert,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Ado,
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
                    OrmType = OrmType.Ado,
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
            List<TestResult> testResults = new List<TestResult>();
            NpgsqlConnection conn = connectionPostgre;
            try
            {
                conn.Close();
                conn.Open();
                Stopwatch stopwatch = new Stopwatch();
                List<mockuser> users = new List<mockuser>();


                var query = $"SELECT * FROM mockuser limit " + count;
                stopwatch.Start();

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    {
                        cmd.CommandText = query;
                        NpgsqlDataReader reader2 = cmd.ExecuteReader();
                    }
                }

                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Select,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Ado,
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
                    OrmType = OrmType.Ado,
                    DbName = DbName.Postgre,
                    IsSuccessful = false,
                    TestGuid = "",
                    ErrorMessage = ex.ToString()

                });
            }

            conn.Close();
            return testResults;
        }

        public static List<TestResult> UpdatePostgre(int count)
        {
            List<Int64> idList = DbHelper.GetUpdateIdList(DbName.Postgre, count);
            List<TestResult> testResults = new List<TestResult>();
            Stopwatch stopwatch = new Stopwatch();

            try
            {
                connectionPostgre.Open();

                stopwatch.Start();

                foreach (var id in idList)
                {
                    string query = "UPDATE mockuser SET jobtitle = @jobtitle, name = @name, email =@email Where id = " + id;

                    using (var cmd = new NpgsqlCommand(query, connectionPostgre))
                    {
                        cmd.Parameters.Add("jobtitle", NpgsqlTypes.NpgsqlDbType.Varchar, 500).Value = "Dentist" + Guid.NewGuid().ToString();
                        cmd.Parameters.Add("name", NpgsqlTypes.NpgsqlDbType.Varchar, 500).Value = "Josh Evans" + Guid.NewGuid().ToString();
                        cmd.Parameters.Add("email", NpgsqlTypes.NpgsqlDbType.Varchar, 500).Value = "Rae_John298@bretoux.com" + Guid.NewGuid().ToString();
                        cmd.ExecuteNonQuery();
                    }
                }

                stopwatch.Stop();


                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Update,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Ado,
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
                    OrmType = OrmType.Ado,
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
