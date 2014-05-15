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
    public class User
    {
        private static String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["KKDB"].ToString();
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public int UserID { get; set; }
        public int? CreatedByID { get; set; } 

        public bool IsAdmin { get; set; }
		public bool IsActive { get; set; } 

        [Display(Name = "User Roles")]
        public string[] Roles { get; set; }
        
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        [Display(Name = "Email")]
        public string Email { get; set; }

		[Display(Name = "Full Name")]
        public string FullName { get { return this.FirstName + " " + this.LastName; } }


        public static List<User> GetAll()
        {
            List<User> users = new List<User>();
            string query = "SELECT * FROM kk_user ORDER BY last_name, first_name;";
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    conn.Open();
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User();
                            DBFill(reader, user);
                            users.Add(user);
                        }
                    }
                }
            }
            return users;
        }

		public static User GetByUserName(string userName)
        {
            User user = new User();
            string query = "SELECT * FROM kk_user WHERE username = @username;";
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@username", System.Data.DbType.String).Value = userName;
                    conn.Open();

                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DBFill(reader, user);
                        }
                    }
                }
            }
            return user;
        }

        // login
        public static User Login(string _username, string _password, ref string error)
        {
            User user = new User();
            string query = @"SELECT * FROM kk_user 
            				 WHERE username = @username 
            				 	AND password = @password LIMIT 1";
            // get database connection
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    // load parameters
                    cmd.Parameters.Add("@username", System.Data.DbType.String).Value = _username;
                    cmd.Parameters.Add("@password", System.Data.DbType.String).Value = _password;
                    // open database connection

                    // user reader to fill data
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DBFill(reader, user);
                        }
                    }

                }
            }
            return user;
        }


        public void Save()
        {
            if (this.UserID == 0)
            {
                this.Insert();
            }
            else
            {
                this.Update();
            }
        }


        // create new user
        private void Insert()
        {
            string query = @"INSERT INTO kk_user (username, password, first_name, last_name, created_by, email) 
                             VALUES (@username, @password, @first_name, @last_name, @created_by, @email);

                             SELECT last_insert_rowid() FROM kk_user; ";

            // get database connection
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    // load parameters
                    LoadParams(cmd, this);
                    cmd.Parameters.Add("@password", System.Data.DbType.String).Value = this.Password;
                    this.UserID = Convert.ToInt32(cmd.ExecuteScalar());

                }
            }
        }

        // create new user
        private void Update()
        {
            string query = @"UPDATE kk_user SET 
                                username = @username, 
                                first_name = @first_name, last_name = @last_name, 
                                email = @email, is_active = @is_active
                             WHERE user_id = @user_id;";

            // get database connection
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    // load parameters
                    cmd.Parameters.Add("@is_active", System.Data.DbType.Int32).Value = Convert.ToInt16(this.IsActive);
                    cmd.Parameters.Add("@user_id", System.Data.DbType.Int32).Value = this.UserID;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void SetUserActive(string userName, bool isActive)
        {
            string query = @"UPDATE kk_user SET is_active = @is_active WHERE username = @username;";

            // get database connection
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@is_active", System.Data.DbType.Int32).Value = Convert.ToInt32(isActive);
                    cmd.Parameters.Add("@username", System.Data.DbType.String).Value = userName;
                }
            }
        }

        public static void UpdatePassword(string userName, string oldPassword, string newPassword)
        {
            User user = User.GetByUserName(userName);
            if (user.Password != oldPassword) {
                throw new Exception("Old password did not match the new password.");
            }

            string query = @"UPDATE kk_user SET password = @password WHERE username = @username;";

            // get database connection
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@password", System.Data.DbType.String).Value = newPassword;
                    cmd.Parameters.Add("@username", System.Data.DbType.String).Value = userName;
                }
            }
        }

        public void AddUserAccess(int RoleID)
        {
            string query = @"INSERT INTO kk_user_access (user_id,role_id) VALUES (@user_id, @role_id); ";
            // get database connection
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@user_id", System.Data.DbType.Int32).Value = this.UserID;
                    cmd.Parameters.Add("@role_id", System.Data.DbType.Int32).Value = RoleID;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ClearAllUserAccess(int UserID)
        {
            string query = @"DELETE FROM kk_user_access WHERE user_id = @user_id;";
            // get database connection
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@user_id", System.Data.DbType.Int32).Value = this.UserID;
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static void LoadParams(SqliteCommand cmd, User user)
        {
            cmd.Parameters.Add("@username", System.Data.DbType.String).Value = user.UserName;

            cmd.Parameters.Add("@first_name", System.Data.DbType.String).Value = user.FirstName;
            cmd.Parameters.Add("@last_name", System.Data.DbType.String).Value = user.LastName;
            if (user.CreatedByID != null)
            {
                cmd.Parameters.Add("@created_by", System.Data.DbType.Int32).Value = user.CreatedByID;
            }
            else
            {
                cmd.Parameters.Add("@created_by", System.Data.DbType.Int32).Value = DBNull.Value;
            }

            cmd.Parameters.Add("@email", System.Data.DbType.String).Value = user.Email;
        }

        public static void DBFill(SqliteDataReader reader, User user)
        {
            user.UserID = Convert.ToInt32(reader["user_id"]);
            user.UserName = reader["username"].ToString();
            user.Password = reader["password"].ToString();
            user.FirstName = reader["first_name"].ToString();
            user.LastName = reader["last_name"].ToString();
            user.Email = reader["email"].ToString();
            user.IsActive = Convert.ToBoolean(reader["is_active"]);
            user.Roles = Role.GetRolesByUser(user.UserID);
        }
    } 
}