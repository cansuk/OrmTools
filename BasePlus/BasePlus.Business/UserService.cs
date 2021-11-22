using BasePlus.BusinessContracts;
using BasePlus.Common.Entities;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Npgsql;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using BasePlus.Common.DTO;
using System.Diagnostics;
using System.Data;
using BasePlus.Common.API;
using System.Linq;
using System.Data.SqlClient;
using BasePlus.Common;
using Dapper.Contrib.Extensions;
using BasePlus.Data;
using Microsoft.EntityFrameworkCore;

namespace BasePlus.Business
{
    public class UserService : IUserService
    {
        readonly IConfiguration configuration;
        delegate string EncodeForSearch(string s);
        public UserService(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public bool SetCurrentDb(string s)
        {
            try
            {
                if (String.Equals(s.Trim(), s))
                {
                    configuration["PostgreConnection"] = configuration.GetConnectionString("SqlConnection");
                }
                else if (String.Equals(s.Trim(), "p"))
                {
                    configuration["PostgreConnection"] = configuration.GetConnectionString("PostgreConnection");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool TestEF(IEnumerable<BasePlus.Common.API.Object> _users)
        {
            
            List<TestResult> testResults = new List<TestResult>();
            try {
                List<mockuser> userlist = new List<mockuser>();
                _users.ToList().ForEach(x => userlist.Add(new mockuser()
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
                
                int id = 1;
                long deleteId = 0;

                // SELECT

                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();
                using (var context = new ApplicationDbContext())
                {
                    var users = context.Users.AsNoTracking().AsQueryable();
                    deleteId = users.LastOrDefault().id;
                    stopwatch.Stop();
                    testResults.Add(new TestResult()
                    {
                        OperationOrderNo = 15,
                        OperationType = OperationType.Select,
                        DbName = DbName.MsSql,
                        OrmType = OrmType.EFCore,
                        TransactionType = TransactionType.UNKNOWN,
                        TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                        AdditionalMessage = "Method : context.Users.AsQueryable(), Row count : " + users.ToList().Count()
                    });
                }

                // SELECT WITH PARAM
                stopwatch.Reset();
                stopwatch.Start();
                using (var context = new ApplicationDbContext())
                {
                    var users = context.Users.AsNoTracking().Where(x => x.id == Convert.ToInt32(id));
                    stopwatch.Stop();
                    testResults.Add(new TestResult()
                    {
                        OperationOrderNo = 15,
                        OperationType = OperationType.SelectWithParameter,
                        DbName = DbName.MsSql,
                        OrmType = OrmType.EFCore,
                        TransactionType = TransactionType.UNKNOWN,
                        TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                        AdditionalMessage = "Method : linq where " + users.ToList().Count()
                    });
                }

                // INSERT

                using (var context = new ApplicationDbContext())
                {
                    context.Users.AddRange(userlist);
                    context.SaveChanges();
                    stopwatch.Stop();
                    testResults.Add(new TestResult()
                    {
                        OperationOrderNo = 15,
                        OperationType = OperationType.SelectWithParameter,
                        DbName = DbName.MsSql,
                        OrmType = OrmType.EFCore,
                        TransactionType = TransactionType.UNKNOWN,
                        TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                        AdditionalMessage = "Method : addrange/save changes "
                    });
                }

                // UPDATE
                stopwatch.Reset();
                stopwatch.Start();
                using (var context = new ApplicationDbContext())
                {
                    var user = context.Users.Where(x => x.id == Convert.ToInt32(id)).FirstOrDefault();
                    user.name = "Molly Allcott";
                    user.jobtitle = "Doctor";
                    user.address = "Arlington  Alley, 3354";
                    user.country = "Liechtenstein";
                    context.SaveChanges();
                    stopwatch.Stop();
                    testResults.Add(new TestResult()
                    {
                        OperationOrderNo = 15,
                        OperationType = OperationType.Update,
                        DbName = DbName.MsSql,
                        OrmType = OrmType.EFCore,
                        TransactionType = TransactionType.UNKNOWN,
                        TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                        AdditionalMessage = "Method : save changes"
                    });                    
                }

                // DELETE
                stopwatch.Reset();
                stopwatch.Start();
                using (var context = new ApplicationDbContext())
                {
                    var user = context.Users.Where(x => x.id == Convert.ToInt32(deleteId)).FirstOrDefault();
                    context.Remove(user);
                    context.SaveChanges();
                    stopwatch.Stop();
                    testResults.Add(new TestResult()
                    {
                        OperationOrderNo = 15,
                        OperationType = OperationType.Delete,
                        DbName = DbName.MsSql,
                        OrmType = OrmType.EFCore,
                        TransactionType = TransactionType.UNKNOWN,
                        TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                        AdditionalMessage = "Method : Remove/save changes"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 15,
                    OperationType = OperationType.Select,
                    DbName = DbName.MsSql,
                    OrmType = OrmType.EFCore,
                    TransactionType = TransactionType.UNKNOWN,
                    TotalMilisecond = "",
                    AdditionalMessage = "!!! Exception : " + ex.Message.ToString()
                });
                return false;
            }

            LogToFile(testResults, "C:\\ef-log.txt");

            return true;
        }

        /// <summary>
        /// ALL SCNERORIES
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public async Task<RestResult<int>> TestAll(IEnumerable<BasePlus.Common.API.Object> users)
        {
            string id = "3800374";
            string deleteid = "3800459";
            
            Stopwatch stopwatch = new Stopwatch();
            RestResult<int> result = new RestResult<int>();
            List<User> userlist = new List<User>();
            List<TestResult> testResults = new List<TestResult>();

         //   testResults.AddRange(TestEF(id, deleteid));

            DataTable dt = new DataTable();

            users.ToList().ForEach(x => userlist.Add(new User()
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

            string sqlcon = configuration.GetConnectionString("SqlConnection");
            string postgrecon = configuration.GetConnectionString("PostgreConnection");
            
            try
            {
                // MSSQL TESTS
                #region MSSQL TESTS

                #region ADO.NET  
                SqlConnection connection = new SqlConnection(sqlcon);
                connection.Open();
                DateTime operationStart = DateTime.Now;
                //1- insert without transaction :
                
                var query = "INSERT INTO MOCKUSER (jobtitle, email, name, createdate, gender, phone, city, address, country, active) VALUES (@jobtitle, @email, @name, @createdate, @gender, @phone, @city, @address, @country, @active)";

                stopwatch.Start();
                
                {
                    
                    //   NpgsqlTransaction trans = connection.BeginTransaction();
                    foreach (var x in userlist)
                    {
                        //  using (var cmd = new NpgsqlCommand(query, connection, trans))
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
                    // trans.Commit();
                }
                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 1,
                    OperationType = OperationType.Insert,
                    OperationEndTime = DateTime.Now,
                    TransactionType = TransactionType.NO,
                    OrmType = OrmType.Ado,
                    DbName = DbName.MsSql,
                    OperationStartTime = operationStart,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    TimeElsapsed = stopwatch.Elapsed.TotalMinutes + " min / " + stopwatch.Elapsed.TotalSeconds + " sn / " + stopwatch.Elapsed.TotalMilliseconds + " ms / ",
                    AdditionalMessage = "Method : ExecuteNonQuery"
                });

                //2- insert with transaction :
                operationStart = DateTime.Now;
                stopwatch.Reset();
                stopwatch.Start();
                
                {
                    SqlTransaction trans = connection.BeginTransaction();
                    foreach (var x in userlist)
                    {
                        using (var cmd = new SqlCommand(query, connection, trans))
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
                    trans.Commit();
                    stopwatch.Stop();
                }

                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 2,
                    OperationType = OperationType.Insert,
                    DbName = DbName.MsSql,
                    OrmType = OrmType.Ado,
                    TransactionType = TransactionType.YES,
                    OperationStartTime = operationStart,
                    OperationEndTime = DateTime.Now,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    TimeElsapsed = stopwatch.Elapsed.TotalMinutes + " min / " + stopwatch.Elapsed.TotalSeconds + " sn / " + stopwatch.Elapsed.TotalMilliseconds + " ms / ",
                    AdditionalMessage = "Method : ExecuteNonQuery"
                });

                //3- select
                // datatable a load edilmeden :
                operationStart = DateTime.Now;
                query = $"SELECT * FROM mockuser";
                stopwatch.Reset();
                stopwatch.Start();
                
                {
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        {
                            cmd.CommandText = query;
                            SqlDataReader reader1= cmd.ExecuteReader();
                            reader1.Close();
                        }
                    }
                }
                stopwatch.Stop();

                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 3,
                    OperationType = OperationType.Select,
                    DbName = DbName.MsSql,
                    OrmType = OrmType.Ado,
                    TransactionType = TransactionType.NO,
                    OperationStartTime = operationStart,
                    OperationEndTime = DateTime.Now,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    TimeElsapsed = stopwatch.Elapsed.TotalMinutes + " min / " + stopwatch.Elapsed.TotalSeconds + " sn / " + stopwatch.Elapsed.TotalMilliseconds + " ms / ",
                    AdditionalMessage = "Method : ExecuteReader, data table a veya dataset e load edilmeden"
                });

                // datatable a load edildiğinde :
                operationStart = DateTime.Now;
                query = $"SELECT * FROM mockuser";
                stopwatch.Reset();
                stopwatch.Start();
                
                {
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        {
                            cmd.CommandText = query;
                            SqlDataReader reader2 = cmd.ExecuteReader();
                            dt.Load(reader2);
                            reader2.Close();
                        }
                    }
                }
                stopwatch.Stop();

                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 4,
                    OperationType = OperationType.Select,
                    DbName = DbName.MsSql,
                    OrmType = OrmType.Ado,
                    TransactionType = TransactionType.NO,
                    OperationStartTime = operationStart,
                    OperationEndTime = DateTime.Now,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    TimeElsapsed = stopwatch.Elapsed.TotalMinutes + " min / " + stopwatch.Elapsed.TotalSeconds + " sn / " + stopwatch.Elapsed.TotalMilliseconds + " ms / ",
                    AdditionalMessage = "Method : ExecuteReader, data table a veya dataset e load edildiğinde"
                });

                //4- Select with id datatable a load edilmeden ve edildiğinde:

                // load edilmeden :
                operationStart = DateTime.Now;
                query = $"SELECT * FROM mockuser where id = 10000000";
                stopwatch.Reset();
                stopwatch.Start();
                
                {
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        {
                            cmd.CommandText = query;
                            SqlDataReader reader3 = cmd.ExecuteReader();
                            reader3.Close();
                        }
                    }
                }
                stopwatch.Stop();

                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 5,
                    OperationType = OperationType.SelectWithParameter,
                    DbName = DbName.MsSql,
                    OrmType = OrmType.Ado,
                    TransactionType = TransactionType.NO,
                    OperationStartTime = operationStart,
                    OperationEndTime = DateTime.Now,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    TimeElsapsed = stopwatch.Elapsed.TotalMinutes + " min / " + stopwatch.Elapsed.TotalSeconds + " sn / " + stopwatch.Elapsed.TotalMilliseconds + " ms / ",
                    AdditionalMessage = "Method : ExecuteReader, Select with id datatable a load edilmeden"
                });

                // load edildiğinde :
                operationStart = DateTime.Now;
                query = $"SELECT * FROM mockuser where id = 10000000";
                stopwatch.Reset();
                stopwatch.Start();
                //using (SqlConnection connection = new SqlConnection(sqlcon))
                {
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        {
                            cmd.CommandText = query;
                            SqlDataReader reader4 = cmd.ExecuteReader();
                            dt.Load(reader4);
                            reader4.Close();
                        }
                    }
                }
                stopwatch.Stop();

                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 6,
                    OperationType = OperationType.SelectWithParameter,
                    DbName = DbName.MsSql,
                    OrmType = OrmType.Ado,
                    TransactionType = TransactionType.NO,
                    OperationStartTime = operationStart,
                    OperationEndTime = DateTime.Now,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    TimeElsapsed = stopwatch.Elapsed.TotalMinutes + " min / " + stopwatch.Elapsed.TotalSeconds + " sn / " + stopwatch.Elapsed.TotalMilliseconds + " ms / ",
                    AdditionalMessage = "Method : ExecuteReader, Select with id datatable a load edildiğinde"
                }); 

                #endregion

                #region DAPPER

                //1- insert without transaction :

                // Execute ile :
                operationStart = DateTime.Now;
                query = "INSERT INTO MOCKUSER (jobtitle, email, name, createdate, gender, phone, city, address, country, active) VALUES (@jobtitle, @email, @name, @createdate, @gender, @phone, @city, @address, @country, @active)";
                stopwatch.Reset();
                stopwatch.Start();
                //using (SqlConnection connection = new SqlConnection(sqlcon))
                {
                    int affectedRows = connection.Execute(query, userlist);

                    result = new RestResult<int>()
                    {
                        IsSuccessful = true,
                        Result = affectedRows,
                        TimeElapsed = stopwatch.Elapsed.ToString()
                    };

                }
                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 7,
                    OperationType = OperationType.Insert,
                    DbName = DbName.MsSql,
                    OrmType = OrmType.Dapper,
                    TransactionType = TransactionType.NO,
                    OperationStartTime = operationStart,
                    OperationEndTime = DateTime.Now,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    TimeElsapsed = stopwatch.Elapsed.TotalMinutes + " min / " + stopwatch.Elapsed.TotalSeconds + " sn / " + stopwatch.Elapsed.TotalMilliseconds + " ms / ",
                    AdditionalMessage = "Method : Execute"
                });

                // ExecuteAsync ile :
                operationStart = DateTime.Now;                
                stopwatch.Reset();
                stopwatch.Start();
                {
                    int affectedRows = await connection.ExecuteAsync(query, userlist);

                    result = new RestResult<int>()
                    {
                        IsSuccessful = true,
                        Result = affectedRows,
                        TimeElapsed = stopwatch.Elapsed.ToString()
                    };

                }
                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 8,
                    OperationType = OperationType.Insert,
                    DbName = DbName.MsSql,
                    OrmType = OrmType.Dapper,
                    TransactionType = TransactionType.NO,
                    OperationStartTime = operationStart,
                    OperationEndTime = DateTime.Now,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    TimeElsapsed = stopwatch.Elapsed.TotalMinutes + " min / " + stopwatch.Elapsed.TotalSeconds + " sn / " + stopwatch.Elapsed.TotalMilliseconds + " ms / ",
                    AdditionalMessage = "Method : ExecuteAsync"
                });

                //2- insert with transaction :
                // Execute ile :
                operationStart = DateTime.Now;
                stopwatch.Reset();
                stopwatch.Start();
                {
                    using (var tran = connection.BeginTransaction())
                    {
                        int affectedRows = connection.Execute(query, userlist, transaction: tran);
                        tran.Commit();
                        result = new RestResult<int>()
                        {
                            IsSuccessful = true,
                            Result = affectedRows,
                            TimeElapsed = stopwatch.Elapsed.ToString()
                        };
                    }
                }
                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 9,
                    OperationType = OperationType.Insert,
                    DbName = DbName.MsSql,
                    OrmType = OrmType.Dapper,
                    TransactionType = TransactionType.YES,
                    OperationStartTime = operationStart,
                    OperationEndTime = DateTime.Now,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    TimeElsapsed = stopwatch.Elapsed.TotalMinutes + " min / " + stopwatch.Elapsed.TotalSeconds + " sn / " + stopwatch.Elapsed.TotalMilliseconds + " ms / ",
                    AdditionalMessage = "Method : Execute"
                });

                // ExecuteAsync ile :
                operationStart = DateTime.Now;
                stopwatch.Reset();
                stopwatch.Start();
                
                {
                    using (var tran = connection.BeginTransaction())
                    {
                        int affectedRows = await connection.ExecuteAsync(query, userlist, transaction: tran);
                        tran.Commit();
                        result = new RestResult<int>()
                        {
                            IsSuccessful = true,
                            Result = affectedRows,
                            TimeElapsed = stopwatch.Elapsed.ToString()
                        };
                    }
                }
                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 10,
                    OperationType = OperationType.Insert,
                    DbName = DbName.MsSql,
                    OrmType = OrmType.Dapper,
                    TransactionType = TransactionType.YES,
                    OperationStartTime = operationStart,
                    OperationEndTime = DateTime.Now,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    TimeElsapsed = stopwatch.Elapsed.TotalMinutes + " min / " + stopwatch.Elapsed.TotalSeconds + " sn / " + stopwatch.Elapsed.TotalMilliseconds + " ms / ",
                    AdditionalMessage = "Method : ExecuteAsync"
                });

                //3- select
                // method : query
                operationStart = DateTime.Now;
                query = $"SELECT * FROM mockuser";
                IEnumerable<User> selectUsers;
                stopwatch.Start();
                {
                    selectUsers = connection.Query<User>(query);
                }
                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 11,
                    OperationType = OperationType.Select,
                    DbName = DbName.MsSql,
                    OrmType = OrmType.Dapper,
                    TransactionType = TransactionType.NO,
                    OperationStartTime = operationStart,
                    OperationEndTime = DateTime.Now,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    TimeElsapsed = stopwatch.Elapsed.TotalMinutes + " min / " + stopwatch.Elapsed.TotalSeconds + " sn / " + stopwatch.Elapsed.TotalMilliseconds + " ms / ",
                    AdditionalMessage = "Method : Query, Return row count : " + selectUsers.Count().ToString()
                });

                // method : QueryAsync
                operationStart = DateTime.Now;
                query = $"SELECT * FROM mockuser";
                IEnumerable<User> selectUsersAsync;
                stopwatch.Start();
                {
                    selectUsersAsync = await connection.QueryAsync<User>(query);
                }
                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 12,
                    OperationType = OperationType.Select,
                    DbName = DbName.MsSql,
                    OrmType = OrmType.Dapper,
                    TransactionType = TransactionType.NO,
                    OperationStartTime = operationStart,
                    OperationEndTime = DateTime.Now,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    TimeElsapsed = stopwatch.Elapsed.TotalMinutes + " min / " + stopwatch.Elapsed.TotalSeconds + " sn / " + stopwatch.Elapsed.TotalMilliseconds + " ms / ",
                    AdditionalMessage = "Method : QueryAsync, Return row count : " + selectUsersAsync.Count().ToString()
                });

                //4- Select with id :

                // Query Method :
                operationStart = DateTime.Now;
                query = $"SELECT * FROM mockuser where id = " + id;
                stopwatch.Start();
                {
                    selectUsers = connection.Query<User>(query);
                }
                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 13,
                    OperationType = OperationType.SelectWithParameter,
                    DbName = DbName.MsSql,
                    OrmType = OrmType.Dapper,
                    TransactionType = TransactionType.NO,
                    OperationStartTime = operationStart,
                    OperationEndTime = DateTime.Now,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    TimeElsapsed = stopwatch.Elapsed.TotalMinutes + " min / " + stopwatch.Elapsed.TotalSeconds + " sn / " + stopwatch.Elapsed.TotalMilliseconds + " ms / ",
                    AdditionalMessage = "Method : Query, Return row count : " + selectUsers.Count().ToString()
                });


                // QueryAsync Method
                operationStart = DateTime.Now;
                query = $"SELECT * FROM mockuser where id = " + id;
                stopwatch.Start();
                {
                    selectUsersAsync = await connection.QueryAsync<User>(query);
                    
                }
                stopwatch.Stop();
                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 14,
                    OperationType = OperationType.SelectWithParameter,
                    DbName = DbName.MsSql,
                    OrmType = OrmType.Dapper,
                    TransactionType = TransactionType.NO,
                    OperationStartTime = operationStart,
                    OperationEndTime = DateTime.Now,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    TimeElsapsed = stopwatch.Elapsed.TotalMinutes + " min / " + stopwatch.Elapsed.TotalSeconds + " sn / " + stopwatch.Elapsed.TotalMilliseconds + " ms / ",
                    AdditionalMessage = "Method : QueryAsync, Return row count : " + selectUsersAsync.Count().ToString()
                });

                connection.Close();

                // DAPPER WITH CONTRIB
                // GetAll
                stopwatch.Reset();
                stopwatch = GetDapperWithContrib();
                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 15,
                    OperationType = OperationType.Select,
                    DbName = DbName.MsSql,
                    OrmType = OrmType.Dapper,
                    TransactionType = TransactionType.YES,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    TimeElsapsed = stopwatch.Elapsed.TotalMinutes + " min / " + stopwatch.Elapsed.TotalSeconds + " sn / " + stopwatch.Elapsed.TotalMilliseconds + " ms / ",
                    AdditionalMessage = "Method : Contrib.GetAll()"
                });

                // GetDapperWithContrib(id)
                stopwatch.Reset();
                stopwatch = GetDapperWithContrib(id);
                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 15,
                    OperationType = OperationType.Select,
                    DbName = DbName.MsSql,
                    OrmType = OrmType.Dapper,
                    TransactionType = TransactionType.YES,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    TimeElsapsed = stopwatch.Elapsed.TotalMinutes + " min / " + stopwatch.Elapsed.TotalSeconds + " sn / " + stopwatch.Elapsed.TotalMilliseconds + " ms / ",
                    AdditionalMessage = "Method : Contrib.Get(id)"
                });

                // InsertSingleDapperWithContrib
                stopwatch.Reset();
                stopwatch = InsertSingleDapperWithContrib();
                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 16,
                    OperationType = OperationType.Insert,
                    DbName = DbName.MsSql,
                    OrmType = OrmType.Dapper,
                    TransactionType = TransactionType.YES,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    TimeElsapsed = stopwatch.Elapsed.TotalMinutes + " min / " + stopwatch.Elapsed.TotalSeconds + " sn / " + stopwatch.Elapsed.TotalMilliseconds + " ms / ",
                    AdditionalMessage = "Method : Contrib.InsertSingleDapperWithContrib()"
                });

                #endregion

                #endregion MSSQL TESTS

                // POSTGRE TESTS
                #region POSTGRE TESTS

                #region ADO.NET  

                //1- insert without transaction :


                //2- insert with transaction :


                //3- select

                #endregion

                #region DAPPER

                //1- insert without transaction :


                //2- insert with transaction :


                //3- select

                #endregion

                #endregion POSTGRE TESTS

            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<int>()
                {
                    IsSuccessful = true,
                    Result = 0,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = "Error : " + ex.Message
                };
                testResults.Add(new TestResult()
                {
                    OperationOrderNo = 100,
                    OperationType = OperationType.Select,
                    DbName = DbName.MsSql,
                    OrmType = OrmType.Dapper,
                    TransactionType = TransactionType.NO,
                    OperationEndTime = DateTime.Now,
                    TotalMilisecond = stopwatch.Elapsed.TotalMilliseconds.ToString(),
                    TimeElsapsed = stopwatch.Elapsed.TotalMinutes + " min" + stopwatch.Elapsed.TotalSeconds + " sn" + stopwatch.Elapsed.TotalMilliseconds + " ms",
                    AdditionalMessage = "!!! Exception : " + ex.ToString()
                });
            }

            LogToFile(testResults, "C:\\dapper-log.txt");
            
            return result;
        }

        public void LogToFile(List<TestResult> testResults, string path)
        {
            //var logPath = "C:\\dapper-log.txt";
            var logPath = path;
            var logFile = System.IO.File.Create(logPath);
            var logWriter = new System.IO.StreamWriter(logFile);

            #region özet bilgi

            //minSelectTime
            var minSelectTimes = from res in testResults
                                 where res.OperationType == OperationType.Select
                                 group res by res.OperationOrderNo into resultGroup
                                 select new
                                 {
                                     OperationOrderNo = resultGroup.Key,
                                     OrmType = resultGroup.Select(x => x.OrmType),
                                     minSelectTime = resultGroup.Min(x => Convert.ToDouble(x.TotalMilisecond)),
                                     AdditionalMessages = resultGroup.Select(x => x.AdditionalMessage),
                                     TransactionType = resultGroup.Select(x => x.TransactionType)
                                 };

            var minSelectTime = minSelectTimes.Where(item => item.minSelectTime == minSelectTimes.Select(x => x).Min(x => x.minSelectTime));
            logWriter.WriteLine("SELECT BEST CASE : " + minSelectTime.ToList().FirstOrDefault().OrmType.FirstOrDefault() + "/" + minSelectTime.ToList().FirstOrDefault().minSelectTime + "/"
                                + minSelectTime.ToList().FirstOrDefault().AdditionalMessages.FirstOrDefault() + "/ Transaction : "
                                + minSelectTime.ToList().FirstOrDefault().TransactionType.FirstOrDefault().ToString());
            logWriter.WriteLine("#" + minSelectTime.ToList().FirstOrDefault().OperationOrderNo);

            //minSelectWithParameterTime
            var minSelectWithParameterTimes = from res in testResults
                                              where res.OperationType == OperationType.SelectWithParameter
                                              group res by res.OperationOrderNo into resultGroup
                                              select new
                                              {
                                                  OperationOrderNo = resultGroup.Key,
                                                  OrmType = resultGroup.Select(x => x.OrmType),
                                                  minSelectTime = resultGroup.Min(x => Convert.ToDouble(x.TotalMilisecond)),
                                                  AdditionalMessages = resultGroup.Select(x => x.AdditionalMessage),
                                                  TransactionType = resultGroup.Select(x => x.TransactionType)
                                              };

            var minSelectWithParameterTime = minSelectWithParameterTimes.Where(item => item.minSelectTime == minSelectWithParameterTimes.Select(x => x).Min(x => x.minSelectTime));
            logWriter.WriteLine("SELECT WITH PARAMETER BEST CASE : " + minSelectWithParameterTime.ToList().FirstOrDefault().OrmType.FirstOrDefault() + "/" + minSelectWithParameterTime.ToList().FirstOrDefault().minSelectTime + "/"
                                + minSelectWithParameterTime.ToList().FirstOrDefault().AdditionalMessages.FirstOrDefault() + "/ Transaction : "
                                + minSelectWithParameterTime.ToList().FirstOrDefault().TransactionType.FirstOrDefault().ToString());
            logWriter.WriteLine("#" + minSelectWithParameterTime.ToList().FirstOrDefault().OperationOrderNo);

            //minInsertTime
            var minInsertTimes = from res in testResults
                                 where res.OperationType == OperationType.Insert
                                 group res by res.OperationOrderNo into resultGroup
                                 select new
                                 {
                                     OperationOrderNo = resultGroup.Key,
                                     OrmType = resultGroup.Select(x => x.OrmType),
                                     minSelectTime = resultGroup.Min(x => Convert.ToDouble(x.TotalMilisecond)),
                                     AdditionalMessages = resultGroup.Select(x => x.AdditionalMessage),
                                     TransactionType = resultGroup.Select(x => x.TransactionType)
                                 };

            var minInsertTime = minInsertTimes.Where(item => item.minSelectTime == minInsertTimes.Select(x => x).Min(x => x.minSelectTime));
            logWriter.WriteLine("INSERT BEST CASE : " + minInsertTime.ToList().FirstOrDefault().OrmType.FirstOrDefault() + "/" + minInsertTime.ToList().FirstOrDefault().minSelectTime + "/"
                                + minInsertTime.ToList().FirstOrDefault().AdditionalMessages.FirstOrDefault() + "/ Transaction : "
                                + minInsertTime.ToList().FirstOrDefault().TransactionType.FirstOrDefault().ToString());
            logWriter.WriteLine("#" + minInsertTime.ToList().FirstOrDefault().OperationOrderNo);

            //// minUpdateTime
            //var minUpdateTimes = from res in testResults
            //                     where res.OperationType == OperationType.Insert
            //                     group res by res.OperationOrderNo into resultGroup
            //                     select new
            //                     {
            //                         OperationOrderNo = resultGroup.Key,
            //                         minSelectTime = resultGroup.Min(x => Convert.ToDouble(x.TotalMilisecond)),
            //                         AdditionalMessages = resultGroup.Select(x => x.AdditionalMessage),
            //                         TransactionType = resultGroup.Select(x => x.TransactionType)
            //                     };

            //var minUpdateTime = minUpdateTimes.Where(item => item.minSelectTime == minUpdateTimes.Select(x => x).Min(x => x.minSelectTime));
            //logWriter.WriteLine("UPDATE BEST CASE : " + minSelectTime.ToList().FirstOrDefault().OrmType.FirstOrDefault() + "/" + minUpdateTime.ToList().FirstOrDefault().minSelectTime + "/"
            //                    + minUpdateTime.ToList().FirstOrDefault().AdditionalMessages.FirstOrDefault() + "/ Transaction : " 
            //                    + minUpdateTime.ToList().FirstOrDefault().TransactionType.FirstOrDefault().ToString());
            //logWriter.WriteLine("#" + minUpdateTime.ToList().FirstOrDefault().OperationOrderNo);

            //// minDeleteTime
            //var minDeleteTimes = from res in testResults
            //                     where res.OperationType == OperationType.Insert
            //                     group res by res.OperationOrderNo into resultGroup
            //                     select new
            //                     {
            //                         OperationOrderNo = resultGroup.Key,
            //                         OrmType = resultGroup.Select(x => x.OrmType),
            //                         minSelectTime = resultGroup.Min(x => Convert.ToDouble(x.TotalMilisecond)),
            //                         AdditionalMessages = resultGroup.Select(x => x.AdditionalMessage),
            //                         TransactionType = resultGroup.Select(x => x.TransactionType).FirstOrDefault().ToString()
            //                     };

            //var minDeleteTime = minDeleteTimes.Where(item => item.minSelectTime == minDeleteTimes.Select(x => x).Min(x => x.minSelectTime));
            //logWriter.WriteLine("DELETE BEST CASE : " + minSelectTime.ToList().FirstOrDefault().OrmType.FirstOrDefault() + "/" + minDeleteTime.ToList().FirstOrDefault().minSelectTime + "/"
            //                    + minDeleteTime.ToList().FirstOrDefault().AdditionalMessages.FirstOrDefault() + "/ Transaction : " 
            //                    + minDeleteTime.ToList().FirstOrDefault().TransactionType.FirstOrDefault().ToString());
            //logWriter.WriteLine("#" + minDeleteTime.ToList().FirstOrDefault().OperationOrderNo);


            #endregion özet bilgi

            testResults.ForEach(item =>
            {
                logWriter.WriteLine("============================================================================");
                logWriter.WriteLine("# : " + item.OperationOrderNo);
                logWriter.WriteLine("Db : " + item.DbName);
                logWriter.WriteLine("Orm : " + item.OrmType);
                logWriter.WriteLine("Operation : " + item.OperationType);
                logWriter.WriteLine("Transaction : " + item.TransactionType);
                logWriter.WriteLine("Start Time : " + item.OperationStartTime);
                logWriter.WriteLine("End Time : " + item.OperationEndTime);
                logWriter.WriteLine("TimeElapsed : " + item.TotalMilisecond);
                logWriter.WriteLine(item.AdditionalMessage);
                logWriter.WriteLine("============================================================================");
            });
            logWriter.Dispose();
        }

        public async Task<RestResult<IEnumerable<User>>> GetUsers2()
        {
            Stopwatch stopwatch = new Stopwatch();
            RestResult<IEnumerable<User>> result;

            try
            {
                string connectionString = configuration.GetConnectionString("PostgreConnection");

                IEnumerable<User> users;
                string table = "mockuser";
                var query = $"SELECT * FROM {table} ";
                stopwatch.Start();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    users = await connection.QueryAsync<User>(query);

                }
                stopwatch.Stop();

                result = new RestResult<IEnumerable<User>>()
                {
                    IsSuccessful = true,
                    Result = users,
                    TimeElapsed = stopwatch.Elapsed.ToString()
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<IEnumerable<User>>()
                {
                    IsSuccessful = true,
                    Result = null,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = ex.Message
                };
            }

            return result;
        }

        public async Task<RestResult<IEnumerable<User>>> GetUsers3()
        {
            Stopwatch stopwatch = new Stopwatch();
            RestResult<IEnumerable<User>> result;

            try
            {
                string connectionString = configuration.GetConnectionString("PostgreConnection");

                IEnumerable<User> users;
                string table = "mockuser";
                var query = $"SELECT * FROM {table} ";
                stopwatch.Start();
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    users = await connection.QueryAsync<User>(query);

                }
                stopwatch.Stop();

                result = new RestResult<IEnumerable<User>>()
                {
                    IsSuccessful = true,
                    Result = users,
                    TimeElapsed = stopwatch.Elapsed.ToString()
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<IEnumerable<User>>()
                {
                    IsSuccessful = true,
                    Result = null,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = ex.Message
                };
            }

            return result;
        }

        public async Task<RestResult<IEnumerable<User>>> GetUsers()
        {
            Stopwatch stopwatch = new Stopwatch();
            RestResult<IEnumerable<User>> result;

            try
            {
                string connectionString = configuration.GetConnectionString("PostgreConnection");

                IEnumerable<User> users;
                string table = "mockuser";
                var query = $"SELECT * FROM {table}";
                stopwatch.Start();
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    users = await connection.QueryAsync<User>(query);

                }
                stopwatch.Stop();

                result = new RestResult<IEnumerable<User>>()
                {
                    IsSuccessful = true,
                    Result = users,
                    TimeElapsed = stopwatch.Elapsed.ToString()
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<IEnumerable<User>>()
                {
                    IsSuccessful = true,
                    Result = null,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = ex.Message
                };
            }

            return result;
        }
              
        public Stopwatch GetDapperWithContrib()
        {
            Stopwatch stopwatch = new Stopwatch();
            RestResult<IEnumerable<User>> result;

            try
            {
                string connectionString = configuration.GetConnectionString("SqlConnection");

                stopwatch.Start();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction tran = connection.BeginTransaction();
                    var _users = connection.GetAll<mockuser>(transaction: tran);
                    connection.Close();
                }
                stopwatch.Stop();

            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<IEnumerable<User>>()
                {
                    IsSuccessful = true,
                    Result = null,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = ex.Message
                };
            }

            return stopwatch;
        }

        public Stopwatch GetDapperWithContrib(string id)
        {
            Stopwatch stopwatch = new Stopwatch();
            RestResult<IEnumerable<User>> result;

            try
            {
                string connectionString = configuration.GetConnectionString("SqlConnection");

                stopwatch.Start();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction tran = connection.BeginTransaction();
                    var _user = connection.Get<mockuser>(Convert.ToInt32(id), transaction: tran);
                    connection.Close();
                }
                stopwatch.Stop();

            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<IEnumerable<User>>()
                {
                    IsSuccessful = true,
                    Result = null,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = ex.Message
                };
            }

            return stopwatch;
        }

        public Stopwatch InsertSingleDapperWithContrib()
        {
            Stopwatch stopwatch = new Stopwatch();
            RestResult<IEnumerable<User>> result;

            try
            {
                string connectionString = configuration.GetConnectionString("SqlConnection");

                stopwatch.Start();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction tran = connection.BeginTransaction();
                    var _user = connection.Insert<mockuser>(new mockuser()
                    {
                        jobtitle = "Dentist",
                        email = "Josh_Evans6173@nickia.com",
                        name = "Jayden Bailey",
                        active = true,
                        createdate = DateTime.Now,
                        gender = "Male",
                        phone = "8-288-851-2086",
                        city = "Australia",
                        address = "Vintners  Walk, 3749",
                        country = "Costa Rica"
                    }, transaction: tran);
                    connection.Close();
                }
                stopwatch.Stop();

            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<IEnumerable<User>>()
                {
                    IsSuccessful = true,
                    Result = null,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = ex.Message
                };
            }

            return stopwatch;
        }

        public async Task<RestResult<IEnumerable<User>>> GetFromSqlWithDapper()
        {
            Stopwatch stopwatch = new Stopwatch();
            RestResult<IEnumerable<User>> result;

            try
            {
                string connectionString = configuration.GetConnectionString("SqlConnection");

                IEnumerable<User> users;
                string table = "mockuser";
                var query = $"SELECT * FROM {table}";
                stopwatch.Start();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    users = await connection.QueryAsync<User>(query);

                }
                stopwatch.Stop();

                result = new RestResult<IEnumerable<User>>()
                {
                    IsSuccessful = true,
                    Result = users,
                    TimeElapsed = stopwatch.Elapsed.ToString()
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<IEnumerable<User>>()
                {
                    IsSuccessful = true,
                    Result = null,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = ex.Message
                };
            }

            return result;
        }

        public async Task<RestResult<IEnumerable<User>>> GetFromSqlWithAdo()
        {
            Stopwatch stopwatch = new Stopwatch();
            RestResult<IEnumerable<User>> result;
            SqlDataReader reader;
            DataTable dt=new DataTable();
            try
            {
                string connectionString = configuration.GetConnectionString("SqlConnection");

                IEnumerable<User> users;
                string table = "mockuser";
                var query = $"SELECT * FROM {table}";
                
                //using (SqlConnection connection = new SqlConnection(connectionString))
                //{
                //    users = connection.

                //}
                SqlConnection connection = new SqlConnection(connectionString);
                
                connection.Open();
                stopwatch.Start();
                using (var cmd = new SqlCommand(query, connection))
                {
                    {
                        cmd.CommandText = query;                        
                        reader = cmd.ExecuteReader();
                        dt.Load(reader);
                    }
                }
                stopwatch.Stop();
                connection.Close();

                result = new RestResult<IEnumerable<User>>()
                {
                    IsSuccessful = true,
                    Result = null,
                    TimeElapsed = stopwatch.Elapsed.ToString()
                };
                
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<IEnumerable<User>>()
                {
                    IsSuccessful = true,
                    Result = null,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = ex.Message,                    
                };
            }
            var affectedRows = dt.Rows.Count;
            Console.WriteLine(affectedRows);
            return result;
        }


        public async Task<RestResult<IEnumerable<MockUserInfo>>> GetUsersWithInfo()
        {
            Stopwatch stopwatch = new Stopwatch();
            RestResult<IEnumerable<MockUserInfo>> result;

            try
            {
                string connectionString = configuration.GetConnectionString("PostgreConnection");
                IEnumerable<MockUserInfo> users;
                var query = String.Concat("SELECT public.mockuser.id,jobtitle,email,public.mockuser.name,active,createdate,gender,phone,city,address,country,",
                                        "creditcardnumber,iban, cardtype, currentbalance ",
                                        "FROM public.mockuser INNER JOIN public.userinfo ON public.userinfo.userid = public.mockuser.id");
                stopwatch.Start();
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    users = await connection.QueryAsync<MockUserInfo>(query);

                }
                stopwatch.Stop();

                result = new RestResult<IEnumerable<MockUserInfo>>()
                {
                    IsSuccessful = true,
                    Result = users,
                    TimeElapsed = stopwatch.Elapsed.ToString()
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<IEnumerable<MockUserInfo>>()
                {
                    IsSuccessful = true,
                    Result = null,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = ex.Message
                };
            }

            return result;
        }

        public async Task<RestResult<IEnumerable<MockUserInfo>>> GetUsersWithInfoWithParam(string city, string jobTitle, decimal balanceMin, decimal balanceMax,
                                                                                           bool active, string cardType, string gender, DateTime dateStart, DateTime dateEnd)
        {
            Stopwatch stopwatch = new Stopwatch();
            RestResult<IEnumerable<MockUserInfo>> result;

            try
            {
                string connectionString = configuration.GetConnectionString("PostgreConnection");
                IEnumerable<MockUserInfo> users;
                DynamicParameters parameters = new DynamicParameters();

                EncodeForSearch EncodeForSearchLike = (city) => { return city.Replace("[", "[[]").Replace("%", "[%]"); };
                var query = String.Concat("SELECT public.mockuser.id, public.mockuser.jobtitle, public.mockuser.email, public.mockuser.name, public.mockuser.active, ",
                    "public.mockuser.createdate, public.mockuser.gender, public.mockuser.phone,city, public.mockuser.address, public.mockuser.country,",
                    "public.userinfo.creditcardnumber,public.userinfo.iban, public.userinfo.cardtype, public.userinfo.currentbalance FROM public.mockuser ",
                    "INNER JOIN public.userinfo ON public.userinfo.userid = public.mockuser.id WHERE createdate BETWEEN @dateStart AND @dateEnd ");

                if (!String.IsNullOrWhiteSpace(city))
                {
                    parameters.Add("@city", "%" + EncodeForSearchLike(city) + "%", DbType.String);
                    String.Concat(query, "public.mockuser.city LIKE @city AND ");
                }
                if (!String.IsNullOrWhiteSpace(jobTitle))
                {
                    parameters.Add("@jobTitle", "%" + EncodeForSearchLike(jobTitle) + "%", DbType.String, ParameterDirection.Input);
                    String.Concat(query, "public.mockuser.jobtitle LIKE @jobtitle AND ");
                }
                if (!String.IsNullOrWhiteSpace(cardType))
                {
                    parameters.Add("@cardType", cardType, DbType.String, ParameterDirection.Input);
                    String.Concat(query, "public.userinfo.cardtype = @cardType AND ");
                }
                if (!String.IsNullOrWhiteSpace(gender))
                {
                    parameters.Add("@gender", gender, DbType.String, ParameterDirection.Input);
                    String.Concat(query, "public.mockuser.gender = @gender ");
                }

                parameters.Add("@balanceMin", balanceMin, DbType.Decimal, ParameterDirection.Input);
                parameters.Add("@balanceMax", balanceMax, DbType.Decimal, ParameterDirection.Input);
                parameters.Add("@active", active, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@dateStart", dateStart, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@dateEnd", dateEnd, DbType.DateTime, ParameterDirection.Input);

                stopwatch.Start();
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    users = await connection.QueryAsync<MockUserInfo>(query, parameters, commandType: CommandType.Text);
                }
                stopwatch.Stop();

                result = new RestResult<IEnumerable<MockUserInfo>>()
                {
                    IsSuccessful = true,
                    Result = users,
                    TimeElapsed = stopwatch.Elapsed.ToString()
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<IEnumerable<MockUserInfo>>()
                {
                    IsSuccessful = true,
                    Result = null,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = ex.Message
                };
            }

            return result;
        }
        public async Task<RestResult<object>> UpdateUsersByDate(DateTime dateStart, DateTime dateEnd, DateTime newDate)
        {
            Stopwatch stopwatch = new Stopwatch();
            RestResult<object> result;

            try
            {
                string connectionString = configuration.GetConnectionString("PostgreConnection");

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@dateStart", dateStart, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@dateEnd", dateEnd, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("@newDate", newDate, DbType.DateTime, ParameterDirection.Input);
                var query = String.Concat("UPDATE public.mockuser SET createdate=@newDate WHERE public.mockuser.id IN(SELECT public.mockuser.id ",
                                          "FROM public.mockuser WHERE createdate BETWEEN @dateStart AND @dateEnd)");
                stopwatch.Start();
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    var affectedRows = await connection.ExecuteScalarAsync(query, parameters, commandType: CommandType.Text);
                    result = new RestResult<object>()
                    {
                        IsSuccessful = true,
                        Result = affectedRows,
                        TimeElapsed = stopwatch.Elapsed.ToString()
                    };
                }
                stopwatch.Stop();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<object>()
                {
                    IsSuccessful = true,
                    Result = null,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = ex.Message
                };
            }

            return result;
        }

        public async Task<RestResult<int>> InsertUsers(IEnumerable<BasePlus.Common.API.Object> users)
        {
            Stopwatch stopwatch = new Stopwatch();
            RestResult<int> result = new RestResult<int>();

            try
            {
                List<User> userlist = new List<User>();
                users.ToList().ForEach(x => userlist.Add(new User()
                {
                    jobtitle = x.jobtitle,
                    email = x.email,
                    name = x.name,
                    createdate = DateTime.Parse(x.createdate),
                    gender = x.gender,
                    phone = x.phone,
                    city = x.city,
                    active = true,
                    address = x.address,
                    country = x.country
                }));

                string connectionString = configuration.GetConnectionString("PostgreConnection");

                var query = "INSERT INTO MOCKUSER (jobtitle, email, name, active, createdate, gender, phone, city, address, country) VALUES (@jobtitle, @email, @name, @active, @createdate, @gender, @phone, @city, @address, @country)";

                stopwatch.Reset();
                stopwatch.Start();
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                  //   NpgsqlTransaction trans = connection.BeginTransaction();
                    foreach (var x in userlist)
                    {
                      //  using (var cmd = new NpgsqlCommand(query, connection, trans))
                        using (var cmd = new NpgsqlCommand(query, connection))
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
                    // trans.Commit();
                    connection.Close();
                }
                stopwatch.Stop();
                result = new RestResult<int>()
                {
                    IsSuccessful = true,
                    Result = 0,
                    TimeElapsed = stopwatch.Elapsed.ToString()
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<int>()
                {
                    IsSuccessful = true,
                    Result = 0,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = ex.Message
                };
            }

            return result;

        }

        public async Task<RestResult<int>> InsertUsersDapper(IEnumerable<BasePlus.Common.API.Object> users)
        {
            Stopwatch stopwatch = new Stopwatch();
            RestResult<int> result = new RestResult<int>();

            try
            {
                List<User> userlist = new List<User>();
                users.ToList().ForEach(x => userlist.Add(new User()
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

                string connectionString = configuration.GetConnectionString("PostgreConnection");

                var query = "INSERT INTO MOCKUSER (jobtitle, email, name, createdate, gender, phone, city, address, country, active) VALUES (@jobtitle, @email, @name, @createdate, @gender, @phone, @city, @address, @country, @active)";

                stopwatch.Start();
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        int affectedRows = await connection.ExecuteAsync(query, userlist,transaction:tran);
                        tran.Commit();
                        result = new RestResult<int>()
                        {
                            IsSuccessful = true,
                            Result = affectedRows,
                            TimeElapsed = stopwatch.Elapsed.ToString()
                        };
                    }
                    connection.Close();
                }
                stopwatch.Stop();

            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<int>()
                {
                    IsSuccessful = true,
                    Result = 0,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = "Error : " + ex.Message
                };
            }

            return result;

        }

        public async Task<RestResult<int>> DeleteUsers()
        {
            Stopwatch stopwatch = new Stopwatch();
            RestResult<int> result = new RestResult<int>();

            try
            {
                string connectionString = configuration.GetConnectionString("PostgreConnection");
                var query = "Delete from MOCKUSER";
                stopwatch.Start();
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    int affectedRows = await connection.ExecuteAsync(query);
                    result = new RestResult<int>()
                    {
                        IsSuccessful = true,
                        Result = affectedRows,
                        TimeElapsed = stopwatch.Elapsed.ToString()
                    };
                }
                stopwatch.Stop();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<int>()
                {
                    IsSuccessful = true,
                    Result = 0,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = ex.Message
                };
            }
            return result;
        }

        // Sql server a veri insert testi
        public async Task<RestResult<int>> InsertUsersDapperToSql(IEnumerable<BasePlus.Common.API.Object> users)
        {
            Stopwatch stopwatch = new Stopwatch();
            RestResult<int> result = new RestResult<int>();

            try
            {
                List<User> userlist = new List<User>();
                users.ToList().ForEach(x => userlist.Add(new User()
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

                string connectionString = configuration.GetConnectionString("SqlConnection");

                var query = "INSERT INTO MOCKUSER (jobtitle, email, name, createdate, gender, phone, city, address, country, active) VALUES (@jobtitle, @email, @name, @createdate, @gender, @phone, @city, @address, @country, @active)";

                stopwatch.Start();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    
                        int affectedRows = await connection.ExecuteAsync(query, userlist);
                        
                        result = new RestResult<int>()
                        {
                            IsSuccessful = true,
                            Result = affectedRows,
                            TimeElapsed = stopwatch.Elapsed.ToString()
                        };
                    
                    connection.Close();
                }
                stopwatch.Stop();

            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<int>()
                {
                    IsSuccessful = true,
                    Result = 0,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = "Error : " + ex.Message
                };
            }

            return result;

        }

        public async Task<RestResult<int>> InsertUsersDapperToSqlWithTransaction(IEnumerable<BasePlus.Common.API.Object> users)
        {
            Stopwatch stopwatch = new Stopwatch();
            RestResult<int> result = new RestResult<int>();

            try
            {
                List<User> userlist = new List<User>();
                users.ToList().ForEach(x => userlist.Add(new User()
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

                string connectionString = configuration.GetConnectionString("SqlConnection");

                var query = "INSERT INTO MOCKUSER (jobtitle, email, name, createdate, gender, phone, city, address, country, active) VALUES (@jobtitle, @email, @name, @createdate, @gender, @phone, @city, @address, @country, @active)";

                stopwatch.Start();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (var tran = connection.BeginTransaction())
                    {
                        int affectedRows = await connection.ExecuteAsync(query, userlist, transaction: tran);
                        tran.Commit();
                        result = new RestResult<int>()
                        {
                            IsSuccessful = true,
                            Result = affectedRows,
                            TimeElapsed = stopwatch.Elapsed.ToString()
                        };
                    }
                    connection.Close();
                }
                stopwatch.Stop();

            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<int>()
                {
                    IsSuccessful = true,
                    Result = 0,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = "Error : " + ex.Message
                };
            }

            return result;

        }

        public async Task<RestResult<int>> InsertUsersToSql(IEnumerable<BasePlus.Common.API.Object> users)
        {
            // Dapper kullanılmadan Ado.net ile
            Stopwatch stopwatch = new Stopwatch();
            RestResult<int> result = new RestResult<int>();

            try
            {
                List<User> userlist = new List<User>();
                users.ToList().ForEach(x => userlist.Add(new User()
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

                string connectionString = configuration.GetConnectionString("SqlConnection");

                var query = "INSERT INTO MOCKUSER (jobtitle, email, name, createdate, gender, phone, city, address, country, active) VALUES (@jobtitle, @email, @name, @createdate, @gender, @phone, @city, @address, @country, @active)";

                stopwatch.Start();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //   NpgsqlTransaction trans = connection.BeginTransaction();
                    foreach (var x in userlist)
                    {
                        //  using (var cmd = new NpgsqlCommand(query, connection, trans))
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
                    // trans.Commit();
                    connection.Close();
                }
                stopwatch.Stop();

            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<int>()
                {
                    IsSuccessful = true,
                    Result = 0,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = "Error : " + ex.Message
                };
            }

            return result;

        }

        public async Task<RestResult<int>> InsertUsersToSqlWithTransaction(IEnumerable<BasePlus.Common.API.Object> users)
        {
            // Dapper kullanılmadan Ado.net ile
            Stopwatch stopwatch = new Stopwatch();
            RestResult<int> result = new RestResult<int>();

            try
            {
                List<User> userlist = new List<User>();
                users.ToList().ForEach(x => userlist.Add(new User()
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

                string connectionString = configuration.GetConnectionString("SqlConnection");

                var query = "INSERT INTO MOCKUSER (jobtitle, email, name, createdate, gender, phone, city, address, country, active) VALUES (@jobtitle, @email, @name, @createdate, @gender, @phone, @city, @address, @country, @active)";

                stopwatch.Start();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction trans = connection.BeginTransaction();
                    foreach (var x in userlist)
                    {
                        using (var cmd = new SqlCommand(query, connection, trans))
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

                    trans.Commit();
                    connection.Close();
                }
                stopwatch.Stop();

            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result = new RestResult<int>()
                {
                    IsSuccessful = true,
                    Result = 0,
                    TimeElapsed = stopwatch.Elapsed.ToString(),
                    ErrorMessage = "Error : " + ex.Message
                };
            }

            return result;

        }

    }
}
