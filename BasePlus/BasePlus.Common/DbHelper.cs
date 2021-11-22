using BasePlus.Common.API;
using BasePlus.Common.Entities;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace BasePlus.Common
{
    public static class DbHelper
    {
        //public static string SqlConnectionString = "Data Source=LAPTOP-R5RBF7MO\\SQLEXPRESS;Initial Catalog=mockdb;User ID=sa;Password=***";
        //public static string PostgreConnectionString = "User ID=postgres;Password=***;Server=localhost;Database=mockdb;Integrated Security=true;Pooling=true;";
        //public static string MockUserJson_1 = @"C:\DataFiles\mockuser-1.json";
        //public static string MockUserJson_100 = @"C:\DataFiles\mockuser-100.json";
        //public static string MockUserJson_1000 = @"C:\DataFiles\mockuser-1000.json";
        static Random generatedDay = new Random();
        public static string SqlConnectionString = "Data Source=CANSUKALUC\\SQLEXPRESS;Initial Catalog=mockdb;User ID=sa;Password=***";
        public static string PostgreConnectionString = "User ID=postgres;Password=***;Server=192.168.30.66;Database=mockdb;Integrated Security=true;Pooling=true;";
        public static string MockUserJson_1 = @"C:\DataFiles\mockuser-1.json";
        public static string MockUserJson_100 = @"C:\DataFiles\mockuser-100.json";
        public static string MockUserJson_1000 = @"C:\DataFiles\mockuser-1000.json";

        public static bool ResetMssql()
        {
            try
            {
                RootObject rootObject = new RootObject();
                List<mockuser> users = new List<mockuser>();
                string json = File.ReadAllText(MockUserJson_1000);
                rootObject = JsonConvert.DeserializeObject<RootObject>(json);
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

                #region Reset MSSQL
                //1-Tüm kayıtların silinmesi
                string query = "Delete from mockuser";
                SqlConnection connection = new SqlConnection(SqlConnectionString);
                connection.Open();
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.ExecuteNonQuery();
                }
                //2- Test datalarının insert edilmesi
                query = "INSERT INTO MOCKUSER (jobtitle, email, name, createdate, gender, phone, city, address, country, active) " +
                        " VALUES (@jobtitle, @email, @name, @createdate, @gender, @phone, @city, @address, @country, @active)";
                foreach (var user in users)
                {
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        {
                            cmd.Parameters.AddWithValue("jobtitle", user.jobtitle);
                            cmd.Parameters.AddWithValue("email", user.email);
                            cmd.Parameters.AddWithValue("name", user.name);
                            cmd.Parameters.AddWithValue("createdate", RandomDate());
                            cmd.Parameters.AddWithValue("gender", user.gender);
                            cmd.Parameters.AddWithValue("phone", user.phone);
                            cmd.Parameters.AddWithValue("city", user.city);
                            cmd.Parameters.AddWithValue("active", true);
                            cmd.Parameters.AddWithValue("address", user.address);
                            cmd.Parameters.AddWithValue("country", user.country);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                connection.Close();
                #endregion

                #region Reset POSTGRE

                //1-Tüm kayıtların silinmesi
                query = "Delete from mockuser";
                NpgsqlConnection postgreConnection = new NpgsqlConnection(PostgreConnectionString);
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.ExecuteNonQuery();
                }
                //2- Test datalarının insert edilmesi
                query = "INSERT INTO MOCKUSER (jobtitle, email, name, createdate, gender, phone, city, address, country, active) " +
                        " VALUES (@jobtitle, @email, @name, @createdate, @gender, @phone, @city, @address, @country, @active)";
                foreach (var user in users)
                {
                    using (var cmd = new NpgsqlCommand(query, postgreConnection))
                    {
                        {
                            cmd.Parameters.AddWithValue("jobtitle", user.jobtitle);
                            cmd.Parameters.AddWithValue("email", user.email);
                            cmd.Parameters.AddWithValue("name", user.name);
                            cmd.Parameters.AddWithValue("createdate", RandomDate());
                            cmd.Parameters.AddWithValue("gender", user.gender);
                            cmd.Parameters.AddWithValue("phone", user.phone);
                            cmd.Parameters.AddWithValue("city", user.city);
                            cmd.Parameters.AddWithValue("active", true);
                            cmd.Parameters.AddWithValue("address", user.address);
                            cmd.Parameters.AddWithValue("country", user.country);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                #endregion 

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool ResetPostgre()
        {
            try
            {
                RootObject rootObject = new RootObject();
                List<mockuser> users = new List<mockuser>();
                string json = File.ReadAllText(MockUserJson_1000);
                rootObject = JsonConvert.DeserializeObject<RootObject>(json);
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


                #region Reset POSTGRE

                //1-Tüm kayıtların silinmesi
                string query = "Delete from mockuser";
                NpgsqlConnection postgreConnection = new NpgsqlConnection(PostgreConnectionString);
                postgreConnection.Open();
                using (var cmd = new NpgsqlCommand(query, postgreConnection))
                {
                    cmd.ExecuteNonQuery();
                }
                //2- Test datalarının insert edilmesi
                query = "INSERT INTO MOCKUSER (jobtitle, email, name, createdate, gender, phone, city, address, country, active) " +
                        " VALUES (@jobtitle, @email, @name, @createdate, @gender, @phone, @city, @address, @country, @active)";
                foreach (var user in users)
                {
                    using (var cmd = new NpgsqlCommand(query, postgreConnection))
                    {
                        {
                            cmd.Parameters.AddWithValue("jobtitle", user.jobtitle);
                            cmd.Parameters.AddWithValue("email", user.email);
                            cmd.Parameters.AddWithValue("name", user.name);
                            cmd.Parameters.AddWithValue("createdate", RandomDate());
                            cmd.Parameters.AddWithValue("gender", user.gender);
                            cmd.Parameters.AddWithValue("phone", user.phone);
                            cmd.Parameters.AddWithValue("city", user.city);
                            cmd.Parameters.AddWithValue("active", true);
                            cmd.Parameters.AddWithValue("address", user.address);
                            cmd.Parameters.AddWithValue("country", user.country);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                postgreConnection.Close();
                #endregion 

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static DateTime RandomDate()
        {
            DateTime start = new DateTime(2000, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(generatedDay.Next(range));
        }

        public static List<Int64> GetUpdateIdList(DbName dbName, int count)
        {
            List<Int64> idList = new List<Int64>();
            try
            {                
                if (dbName == DbName.MsSql)
                {
                    SqlConnection connection = new SqlConnection(SqlConnectionString);
                    connection.Open();
                    string query = "select id from mockuser where id IN(select top " + count + " id from mockuser)";
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            idList.Add(reader.GetInt64(0));
                        }
                    }

                    connection.Close();
                }
                else if (dbName == DbName.Postgre)
                {
                    NpgsqlConnection connection = new NpgsqlConnection(PostgreConnectionString);
                    connection.Open();
                    string query = "select id from mockuser where id IN(select id from mockuser limit " + count + ")";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        NpgsqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            idList.Add(reader.GetInt64(0));
                        }
                    }

                    connection.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
          
            return idList;
        }
    
    }
}
