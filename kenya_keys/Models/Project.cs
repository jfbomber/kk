using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Mono.Data.Sqlite;

namespace KK.Models
{

    public class Project
    {
        public static String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["KKDB"].ToString();
        /*
        DROP TABLE IF EXISTS kk_project;
        CREATE TABLE kk_project (
            project_id integer PRIMARY KEY,

            project_name VARCHAR (255) NOT NULL,
            project_description text null,
            project_html text not null,
            project_added datetime not null default CURRENT_TIMESTAMP,
            project_added_by integer not null, -- constraint to kk_user
            project_photo_id integer null,		-- constraint to kk_photo
            project_hide integer default 0
        );
         */
        
        public int? ProjectID { get; set; }
        [DisplayName("Project Name")]
        public string ProjectName { get; set; }
        [DisplayName("Project Description")]
        public string ProjectDescription { get; set; } 
        [DisplayName("Project HTML")]
        public string ProjectHtml { get; set; }
        [DisplayName("Date Project Was Added")]
        public DateTime ProjectAdded { get; set; }

		public int? ParentID { get; set; } 
        public int ProjectAddedById { get; set; }

		 // links to the 
        public int ProjectImageID { get; set; } 
		public string GetImageUrl(Photo.PhotoSize size) {
			if (this.ProjectImageID == 0) {
				return "";
			}

			Photo photo = Photo.Get (this.ProjectImageID);
			return photo.GetUrlBySize (size);
		}
        
		public bool ProjectHide { get; set; }
		public bool Selected { get; set; } 

		public List<Project> GetChildren()
		{	
			List<SqliteParameter> parameters = new List<SqliteParameter> ();
			SqliteParameter p1 = new SqliteParameter ("@parent_id", System.Data.DbType.Int32);
			p1.Value = this.ProjectID;
			parameters.Add (p1);

			return Project.GetList ("SELECT * FROM kk_project WHERE parent_id = @parent_id", parameters);
		}

        /// <summary>
        /// Gets as single project by id
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
		public static Project GetById(int projectId)
        {
            Project project = new Project();
            string query = "SELECT * FROM kk_project WHERE project_id = @project_id;";
            // get database connection
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@project_id", System.Data.DbType.Int32).Value = projectId;
                    // user reader to fill data
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Project.DBFill(reader, project);
                    }

                }
            }
            return project;
        }

		private static List<Project> GetList(string query, List<SqliteParameter> parameters = null)
		{
			List<Project> projects = new List<Project>();
			// get database connection
			using (SqliteConnection conn = new SqliteConnection(connectionString))
			{
				conn.Open();
				// execute cmd
				using (SqliteCommand cmd = new SqliteCommand(query, conn))
				{
					if (parameters != null) {
						foreach (SqliteParameter param in parameters) {
							cmd.Parameters.Add (param);
						}
					}
					// user reader to fill data
					using (SqliteDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							Project project = new Project();
							Project.DBFill(reader, project);
							projects.Add(project);
						}
					}
				}
			}
			return projects;
		}




        /// <summary>
        /// Gets all project
        /// </summary>
        /// <returns>Array of projects</returns>
        public static List<Project> GetAll()
		{	
			return GetList("SELECT * FROM kk_project WHERE project_hide = 0 AND parent_id IS NULL;");
        }



        /// <summary>
        /// Saves the Project
        /// </summary>
        public void Save()
        {
            bool isInsert = false;
            string query = string.Empty;
            if (this.ProjectID == null)
            {
                isInsert = true;
				query = @"INSERT INTO kk_project (project_name, project_description, project_html, project_added_by, project_photo_id, parent_id) 
                          VALUES (@project_name, @project_description, @project_html, @project_added_by, @project_photo_id, @parent_id); 
                          SELECT last_insert_rowid() FROM kk_photo; ";
            } else {
                query = @"UPDATE kk_project SET project_name = @project_name, project_description = @project_description,
                          project_html = @project_html, project_photo_id = @project_photo_id, parent_id = @parent_id
                          WHERE project_id = @project_id;";
            }
            
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@project_name", System.Data.DbType.String).Value = this.ProjectName;
                    if (this.ProjectDescription != string.Empty)
                        cmd.Parameters.Add("@project_description", System.Data.DbType.String).Value = this.ProjectDescription;
                    else
                        cmd.Parameters.Add("@project_description", System.Data.DbType.String).Value = DBNull.Value;
                    
                    cmd.Parameters.Add("@project_html", System.Data.DbType.String).Value = this.ProjectHtml;
                    cmd.Parameters.Add("@project_photo_id", System.Data.DbType.Int32).Value = this.ProjectImageID; 

					if (this.ParentID != null) {
						cmd.Parameters.Add ("@parent_id", System.Data.DbType.Int32).Value = this.ParentID;
					} else {
						cmd.Parameters.Add ("@parent_id", System.Data.DbType.Int32).Value = DBNull.Value;
					}

                    if (isInsert)
                    {
                        cmd.Parameters.Add("@project_added_by", System.Data.DbType.Int32).Value = this.ProjectAddedById;
                        this.ProjectID = Convert.ToInt32(cmd.ExecuteScalar());
                        
                    }
                    else
                    {
                       cmd.Parameters.Add("@project_id", System.Data.DbType.Int32).Value = this.ProjectID;
                       cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Saves the Project
        /// </summary>
        public static void Delete(int projectId)
        {
            string query = @"DELETE FROM kk_project WHERE project_id = @project_id;";
			try
			{
	            using (SqliteConnection conn = new SqliteConnection(connectionString))
	            {
	                conn.Open();
	                // execute cmd
	                using (SqliteCommand cmd = new SqliteCommand(query, conn))
	                {
	                    cmd.Parameters.Add("@project_id", System.Data.DbType.Int32).Value = projectId;
	                    cmd.ExecuteNonQuery();
	                }
	            }
			} catch (Exception e) {
				System.Diagnostics.Debug.WriteLine (e.Message);
			}
        }
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="project"></param>
        public static void DBFill(SqliteDataReader reader, Project project)
        {
            project.ProjectID = Convert.ToInt32(reader["project_id"]);
            project.ProjectName = reader["project_name"].ToString();
            project.ProjectDescription = reader["project_description"].ToString();
            project.ProjectHtml = reader["project_html"].ToString();
			if (reader ["parent_id"] != DBNull.Value) { project.ParentID = Convert.ToInt32 (reader ["parent_id"]); }
            if (reader["project_added_by"] != null) { project.ProjectAddedById = Convert.ToInt32(reader["project_added_by"]); }
            if (reader["project_photo_id"] != null) { project.ProjectImageID = Convert.ToInt32(reader["project_photo_id"]); }
            project.ProjectHide = Convert.ToBoolean(reader["project_hide"]);
		
        }


        
    }
}