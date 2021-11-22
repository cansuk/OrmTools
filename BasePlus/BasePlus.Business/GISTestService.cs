using BasePlus.Common;
using BasePlus.Common.API;
using BasePlus.Common.DTO;
using BasePlus.Common.Entities;
using BasePlus.Data;

using Microsoft.EntityFrameworkCore;
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
    public static class GISTestService
    {
        static string sqlcon = DbHelper.SqlConnectionString;
        static string postgrecon = DbHelper.PostgreConnectionString;
        static SqlConnection connection = new SqlConnection(sqlcon);
        static NpgsqlConnection connectionPostgre = new NpgsqlConnection(postgrecon);

        public static List<TestResult> TestPostgreGISSelect()
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

        public static List<TestResult> TestPostgreGISInsert()
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

        public static List<TestResult> TestPostgreGISUpdate()
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

        public static List<TestResult> TestMSSQLGISSelect()
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

        public static List<TestResult> TestMSSQLGISInsert()
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

        public static List<TestResult> TestMSSQLGISUpdate()
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
            Stopwatch stopwatch = new Stopwatch();
            List<mockuser> users = new List<mockuser>();
            RootObject rootObject = new RootObject();
            try
            {
                connection.Close();
                connection.Open();
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

                ApplicationDbContext context = new ApplicationDbContext();
                stopwatch.Start();
                context.AddRange(users);
                context.SaveChanges();
                stopwatch.Stop();

                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Insert,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.GIS,
                    DbName = DbName.MsSql,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    RecordCount = count
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            connection.Close();
            return testResults;
        }

        public static List<TestResult> SelectMSSQL(int count)
        {
            List<TestResult> testResults = new List<TestResult>();
            Stopwatch stopwatch = new Stopwatch();
            IEnumerable<mockuser> users;

            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                stopwatch.Start();
                users = context.Users.AsNoTracking().Take(count).ToList();
                stopwatch.Stop();

                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Select,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.GIS,
                    DbName = DbName.MsSql,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    RecordCount = count
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return testResults;
        }

        public static List<TestResult> UpdateMSSQL(int count)
        {
            List<TestResult> testResults = new List<TestResult>();
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                stopwatch.Start();
                context.Users.Take(count).ToList().ForEach(user =>
                {
                    user.name = "Molly Allcott" + Guid.NewGuid().ToString();
                    user.jobtitle = "Doctor" + Guid.NewGuid().ToString();
                    user.address = "Arlington  Alley, 3354" + Guid.NewGuid().ToString();
                    context.Entry(user).CurrentValues.SetValues(user);
                });
                context.SaveChanges();

                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Update,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.GIS,
                    DbName = DbName.MsSql,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    RecordCount = count
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }

            connection.Close();
            return testResults;
        }

        // -----------------------------------------------------------------------------------------------------------------------------
        
        public static List<TestResult> InsertPostgre(int count)
        {
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

                PostgreDbContext context = new PostgreDbContext();
                stopwatch.Start();
                context.AddRange(users);
                context.SaveChanges();
                stopwatch.Stop();

                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Insert,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.GIS,
                    DbName = DbName.Postgre,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    RecordCount = count
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return testResults;
        }

        public static List<TestResult> SelectPostgre(int count)
        {
            List<TestResult> testResults = new List<TestResult>();
            Stopwatch stopwatch = new Stopwatch();
            IEnumerable<mockuser> users;

            try
            {
                PostgreDbContext context = new PostgreDbContext();
                stopwatch.Start();
                users = context.Users.AsNoTracking().Take(count).ToList();
                stopwatch.Stop();

                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Select,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.GIS,
                    DbName = DbName.Postgre,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    RecordCount = count
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return testResults;
        }

        public static List<TestResult> UpdatePostgre(int count)
        {
            List<TestResult> testResults = new List<TestResult>();
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                PostgreDbContext context = new PostgreDbContext();
                stopwatch.Start();
                context.Users.Take(count).ToList().ForEach(user =>
                 {
                     user.name = "Molly Allcott" + Guid.NewGuid().ToString();
                     user.jobtitle = "Doctor" + Guid.NewGuid().ToString();
                     user.address = "Arlington  Alley, 3354" + Guid.NewGuid().ToString();
                     context.Entry(user).CurrentValues.SetValues(user);
                 });
                context.SaveChanges();

                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationType = OperationType.Update,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.GIS,
                    DbName = DbName.Postgre,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    RecordCount = count
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return testResults;
        }

    }
}
