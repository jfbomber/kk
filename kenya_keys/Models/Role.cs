using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Mono.Data.Sqlite;
using System.Runtime.Serialization;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Security.Principal;

namespace KK
{
    public class Role
    {
        private static String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["KKDB"].ToString();
        
        public int RoleID { get; set; } 
        public string RoleName { get; set; } 
        public string RoleDescription { get; set; }

        // helper method for html select on user view
        public bool IsSelected { get; set; } 
        public static Role Get(int roleID)
        {
            Role role = new Role();
            string query = "SELECT role_id,role_name,role_description FROM kk_user_role WHERE role_id = @role_id;";
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@role_id", System.Data.DbType.Int32).Value = roleID;
                    conn.Open();
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DBFill(reader, role);
                        }
                    }
                }
            }
            return role;
        }

        public static List<Role> GetAll()
        {
            List<Role> roles = new List<Role>();
            string query = "SELECT role_id,role_name,role_description FROM kk_user_role;";
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    conn.Open();
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Role role = new Role();
                            DBFill(reader, role);
                            roles.Add(role);
                        }
                    }
                }
            }
            return roles;
        }
        
        public static string[] GetRolesByUser(int userID) {
        	string query = @"SELECT role_name
							 FROM kk_user_access ua
							 INNER JOIN kk_user_role ur ON ur.role_id = ua.role_id
							 WHERE ua.user_id = @user_id;";
			List<string> roles = new List<string>();
			using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@user_id", System.Data.DbType.Int32).Value = userID;
                    conn.Open();
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string roleName = reader["role_name"].ToString();
                            roles.Add(roleName);
                        }
                    }
                }
            }
            return roles.ToArray();
        }

		// add user access role
		
		// delete user access role



        public static void DBFill(SqliteDataReader reader, Role role)
        {
            role.RoleID = Convert.ToInt32(reader["role_id"]);
            role.RoleName = reader["role_name"].ToString();
            role.RoleDescription = reader["role_description"].ToString();
        }



    }

  
}



/*
namespace KK {
    public class User
    {
        public int user_id { get; set; }
        public string username { get; set; }
        public string firstName { get; set; } 
        public string lastName { get; set; } 
        public string password { get; set; }
        public DateTime created_dt { get; set; }
        public int? created_by { get; set; }
        public int user_type_id { get; set; } 
        public string email { get; set; }
        public byte[] imageData { get; set; }
        public bool isAdmin { get; set; }
        public static String connectionString = connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["KKDB"].ToString();
        public static User Login(string username, string password, ref string error)
        {
            User user = new User();
            string query = "SELECT * FROM kk_user WHERE username = $username AND password = $password LIMIT 1";
            // get database connection
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    // load parameters
                    cmd.Parameters.Add("$username", System.Data.DbType.String).Value = username;
                    cmd.Parameters.Add("$password", System.Data.DbType.String).Value = password;
                    // open database connection
                    
                    // user reader to fill data
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user.user_id = Convert.ToInt32(reader["user_id"]);
                            user.username =reader["username"].ToString();
                            user.firstName =reader["first_name"].ToString();
                            user.lastName =reader["last_name"].ToString();
                            user.email =reader["email"].ToString();
                            user.user_type_id = Convert.ToInt32(reader["user_type_id"]);
                            user.isAdmin = user.user_type_id == 1 ? true : false;
                            
                        }
                    }

                }
            }
            return user;
        }
        
        public static List<User> GetAll() {
            List<User> users = new List<User>();
            string query = "SELECT * FROM kk_user;";
            // get database connection
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    // user reader to fill data
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User();
                            user.user_id = Convert.ToInt32(reader["user_id"]);
                            user.username =reader["username"].ToString();
                            user.firstName =reader["first_name"].ToString();
                            user.lastName =reader["last_name"].ToString();
                            user.email =reader["email"].ToString();
                            user.user_type_id = Convert.ToInt32(reader["user_type_id"]);
                            user.isAdmin = user.user_type_id == 1 ? true : false;
                            users.Add(user);
                        }
                    }

                }
            }
            return users;
        }

        public static User GetById(int userId)
        {
            User user = new User();
            string query = "SELECT * FROM user WHERE user_id = @user_id;";
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@user_id", System.Data.DbType.Int32);
                    conn.Open();

                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user.user_id = Convert.ToInt32(reader["user_id"]);
                            user.username =reader["username"].ToString();
                            user.firstName =reader["first_name"].ToString();
                            user.lastName =reader["last_name"].ToString();
                            user.email =reader["email"].ToString();
                            user.user_type_id = Convert.ToInt32(reader["user_type_id"]);
                            user.isAdmin = user.user_type_id == 1 ? true : false;
                        }
                    }

                }
            }
            return user;
        }

    }
}
*/