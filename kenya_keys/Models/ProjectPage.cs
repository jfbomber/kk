using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Mono.Data.Sqlite;
using System.Diagnostics;

namespace KK.Models
{

	public class ProjectPage
    {
        public static String connectionString = connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["KKDB"].ToString();
        /*
        DROP TABLE IF EXISTS kk_project_page;
		CREATE TABLE kk_project_page (
			page_id integer primary key,
			project_id integer not null,
			page_html text not null,
			page_index integer not null
		);
         */
        
		public int? PageID { get; set; }

		public int ProjectID { get; set; }

		public int PageIndex { get; set; }

		public int? PrevPage { get; set; }
		public int? NextPage { get; set; }
        
		public string PageTitle { get; set; } 
		public string PageHtml { get; set; }
        
		private static string SelectQuery = @"SELECT page_id, project_id, page_html, page_index, page_title, 
			(SELECT page_id FROM kk_project_page pp WHERE pp.project_id = kk_project_page.project_id AND pp.page_index = kk_project_page.page_index - 1) as prev_page,
			(SELECT page_id FROM kk_project_page pp WHERE pp.project_id = kk_project_page.project_id AND pp.page_index = kk_project_page.page_index + 1) as next_page 
			FROM kk_project_page ";

		public KK.Models.Project GetProject() {
			return KK.Models.Project.GetById(this.ProjectID);
		}



        /// <summary>
        /// Gets as single project by id
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
		public static List<ProjectPage> GetByAllByProjectId(int projectId)
        {
			List<ProjectPage> pages = new List<ProjectPage>();
			string query = SelectQuery + " WHERE project_id = @project_id ORDER BY page_index ASC ";
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
						while (reader.Read ()) {
							ProjectPage page = new ProjectPage ();
							ProjectPage.DBFill(reader, page);
							pages.Add (page);
						}

                    }

                }
            }
			return pages;
        }



		public static ProjectPage GetById(int pageId)
		{
			ProjectPage page = new ProjectPage ();
			string query = 	SelectQuery + " WHERE page_id = @page_id";
			Debug.WriteLine (query);
			// get database connection
			using (SqliteConnection conn = new SqliteConnection(connectionString))
			{
				conn.Open();
				// execute cmd
				using (SqliteCommand cmd = new SqliteCommand(query, conn))
				{
					cmd.Parameters.Add("@page_id", System.Data.DbType.Int32).Value = pageId;
					// user reader to fill data
					using (SqliteDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read ()) {

							ProjectPage.DBFill(reader, page);

						}

					}

				}
			}
			return page;
		}


        /// <summary>
        /// Saves the Project
        /// </summary>
        public void Save()
        {
            bool isInsert = false;
            string query = string.Empty;
			if (this.PageID == null)
            {
				int count = GetByAllByProjectId (this.ProjectID).Count;
				this.PageIndex = count + 1;
                isInsert = true;
				query = @"INSERT INTO kk_project_page (project_id, page_html, page_index, page_title)
						  VALUES (@project_id, @page_html, @page_index, @page_title);
                          SELECT last_insert_rowid() FROM kk_project_page; ";
            } else {
				query = @"UPDATE kk_project_page SET project_id = @project_id, page_html = @page_html,
                          page_index = @page_index, page_title = @page_title
                          WHERE page_id = @page_id;";
            }
            
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
					cmd.Parameters.Add("@page_html", System.Data.DbType.String).Value = this.PageHtml;
					cmd.Parameters.Add("@project_id", System.Data.DbType.Int32).Value = this.ProjectID;
					cmd.Parameters.Add("@page_index", System.Data.DbType.Int32).Value = this.PageIndex; 
					cmd.Parameters.Add("@page_title", System.Data.DbType.String).Value = this.PageTitle; 
					if (!isInsert) {
						cmd.Parameters.Add ("@page_id", System.Data.DbType.Int32).Value = this.PageID;
						this.PageID = Convert.ToInt32 (cmd.ExecuteScalar ());
					} else {
						cmd.ExecuteNonQuery ();
					}
                }
            }
        }



        /// <summary>
        /// Saves the Project
        /// </summary>
		public static void Delete(int pageId)
        {

			string query = @"DELETE FROM  kk_project_page WHERE page_id = @page_id;";
            
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
					cmd.Parameters.Add("@page_id", System.Data.DbType.Int32).Value = pageId;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="project"></param>
		public static void DBFill(SqliteDataReader reader, ProjectPage page)
        {
			page.PageID = Convert.ToInt32 (reader ["page_id"]);
			page.ProjectID = Convert.ToInt32 (reader ["project_id"]);
			page.PageIndex = Convert.ToInt32 (reader ["page_index"]);

			if (reader["page_title"] != DBNull.Value) {
				page.PageTitle = reader ["page_title"].ToString ();
			}


			if (reader["prev_page"] != DBNull.Value) {
				page.PrevPage = Convert.ToInt32 (reader ["prev_page"]);
			} else {
				page.PrevPage = null;
			}

			if (reader["next_page"] != DBNull.Value) {
				page.NextPage = Convert.ToInt32 (reader ["next_page"]);
			} else {
				page.NextPage = null;
			}

			page.PageHtml = reader ["page_html"].ToString();
        }
    }
}