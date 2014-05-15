using System;
using System.Collections.Generic;
using System.ComponentModel;
using Mono.Data.Sqlite;
using System.Linq;
using System.Web;

namespace KK.Models
{

	public class Blog : KKBaseModel
    {


        public int? BlogID { get; set; }
        [DisplayName("Blog Description")]
        public string BlogDescription { get; set; }
        [DisplayName("Blog Title")]
        public string BlogTitle { get; set; }
        [DisplayName("Blog HTML")]
        public string BlogHtml { get; set; }
        [DisplayName("Blog Posted On")]
        public DateTime BlogPostDate { get; set; }
        [DisplayName("Blog Start Date")]
        public DateTime? BlogStartDate { get; set; }
        [DisplayName("Blog End Date")]
        public DateTime? BlogEndDate { get; set; }
        [DisplayName("Blog Author")]
        public string BlogAuthor { get; set; }

		public KK.User BlogAuthorData { get; set; } 

        [DisplayName("Blog Hidden")]
        public bool BlogHidden { get; set; } 

		public string BlogStartDateString { 
			get { 
				return this.BlogStartDate != DateTime.MinValue ? string.Format("{0:MM/dd/yyyy}", this.BlogStartDate) : ""; 
			}
		}

		public string BlogEndDateString { 
			get { 
				return this.BlogEndDate != DateTime.MinValue ? string.Format("{0:MM/dd/yyyy}", this.BlogEndDate) : ""; 
			}
		}

		public string BlogPostDateString { 
			get { 
				return this.BlogPostDate != DateTime.MinValue ? string.Format("{0:MM/dd/yyyy h:mm tt}", this.BlogPostDate) : ""; 
			}
		}



        /// <summary>
        /// Gets as single Blob by id
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static Blog Get(int blogId)
        {
            Blog blog = new Blog();
			string query = "SELECT * FROM kk_blog WHERE blog_id = @blog_id;";
            // get database connection
			using (SqliteConnection conn = new SqliteConnection(ConnString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@blog_id", System.Data.DbType.Int32).Value = blogId;
                    // user reader to fill data
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Blog.DBFill(reader, blog);
                    }

                }
            }
            return blog;
        }

		/// <summary>
		/// Gets as single Blob by id
		/// </summary>
		/// <param name="projectId"></param>
		/// <returns></returns>
		public static int GetTotalCount()
		{
			int count = 0;
			string query = "SELECT COUNT(*) FROM kk_blog;";
			// get database connection
			using (SqliteConnection conn = new SqliteConnection(ConnString))
			{
				conn.Open();
				// execute cmd
				using (SqliteCommand cmd = new SqliteCommand(query, conn))
				{
					count = Convert.ToInt32(cmd.ExecuteScalar ());
				}
			}
			return count;
		}

		/// <summary>
		/// Deletes the Blog
		/// </summary>
		/// <param name="projectId"></param>
		/// <returns></returns>
		public static Blog Delete(int blogId)
		{
			Blog blog = Blog.Get (blogId);
			string query = "DELETE FROM kk_blog WHERE blog_id = @blog_id;";
			// get database connection
			try
			{
			using (SqliteConnection conn = new SqliteConnection(ConnString))
			{
				conn.Open();
				// execute cmd
				using (SqliteCommand cmd = new SqliteCommand(query, conn))
				{
					cmd.Parameters.Add("@blog_id", System.Data.DbType.Int32).Value = blogId;
					cmd.ExecuteNonQuery ();

				}
			}
			} catch (Exception e) {
				blog.ErrorMessage = e.Message;
			}
			return blog;
		}

        /// <summary>
        /// Gets a list if blogs
        /// </summary>
        /// <returns>List of blogs</returns>
		public static List<Blog> GetAll(int? offset, int? limit)
        {
            List<Blog> blogs = new List<Blog>();

			string query = String.Format("SELECT * FROM kk_blog ORDER BY blog_id DESC ");
			if (limit != null) {
				query += String.Format ("LIMIT {0} ", limit);
			}

			if (offset != null) {
				query += String.Format ("OFFSET {0} ", offset);
			}

            // get database connection
			using (SqliteConnection conn = new SqliteConnection (ConnString))
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
                            Blog blog = new Blog();
                            Blog.DBFill(reader, blog);
                            blogs.Add(blog);
                        }
                    }
                }
            }
            return blogs;
        }


        public void HideBlog()
        {
            this.BlogHidden = true;
            this.Save();
        }

        /// <summary>
        /// Saves the Blog
        /// </summary>
        public void Save()
        {
            bool isInsert = false;
            string query = string.Empty;
            if (this.BlogID == null)
            {
                isInsert = true;
                query = @"INSERT INTO kk_blog (blog_description, blog_title, blog_html, blog_hidden, blog_start_date, blog_end_date, blog_author) 
                          VALUES (@blog_description, @blog_title, @blog_html, @blog_hidden, @blog_start_date, @blog_end_date, @blog_author) ;
                          SELECT last_insert_rowid() FROM kk_blog; ";
            }
            else
            {
                query = @"UPDATE kk_blog SET 
	                        blog_description = @blog_description, blog_title = @blog_title,
	                        blog_html = @blog_html, blog_hidden = @blog_hidden, blog_start_date = @blog_start_date,
	                        blog_end_date = @blog_end_date, blog_author = @blog_author
                        WHERE blog_id = @blog_id;";
            }

			try
			{
				using (SqliteConnection conn = new SqliteConnection(ConnString))
	            {
	                conn.Open();
	                // execute cmd
	                using (SqliteCommand cmd = new SqliteCommand(query, conn))
	                {
	                    LoadParameters(cmd, this);
	                    if (isInsert)
	                    {
	                        this.BlogID = Convert.ToInt32(cmd.ExecuteScalar());
	                    }
	                    else
	                    {
	                        cmd.ExecuteNonQuery();
	                    }
	                }
	            }
			} catch (Exception e) {
				this.ErrorMessage = e.Message;
			}
        }
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="project"></param>
        public static void DBFill(SqliteDataReader reader, Blog blog)
        {
            blog.BlogID = Convert.ToInt32(reader["blog_id"]);
            blog.BlogTitle = reader["blog_title"].ToString();
            blog.BlogDescription = reader["blog_description"].ToString();
            blog.BlogHtml = reader["blog_html"].ToString();
            blog.BlogPostDate = Convert.ToDateTime(reader["blog_post_date"]);

            if (reader["blog_start_date"] != DBNull.Value) { blog.BlogStartDate = Convert.ToDateTime(reader["blog_start_date"]); }
            if (reader["blog_end_date"] != DBNull.Value) { blog.BlogEndDate = Convert.ToDateTime(reader["blog_end_date"]); }
            if (reader["blog_author"] != DBNull.Value) { 
				blog.BlogAuthor = reader["blog_author"].ToString(); 
				blog.BlogAuthorData = KK.User.GetByUserName (blog.BlogAuthor);
			}

        }



		public static void LoadParameters(SqliteCommand cmd, Blog blog)
		{
			string query = cmd.CommandText;

			if (query.IndexOf("@blog_id") != -1) { cmd.Parameters.Add("@blog_id", System.Data.DbType.String).Value = Convert.ToInt32(blog.BlogID); }

			cmd.Parameters.Add("@blog_html", System.Data.DbType.String).Value = blog.BlogHtml;
			cmd.Parameters.Add("@blog_hidden", System.Data.DbType.Int32).Value = Convert.ToInt32(blog.BlogHidden);

			if (blog.BlogDescription != null) { cmd.Parameters.Add("@blog_description", System.Data.DbType.String).Value = blog.BlogDescription; }
			else { cmd.Parameters.Add("@blog_description", System.Data.DbType.String).Value = DBNull.Value;  }

			if (blog.BlogTitle != null) { cmd.Parameters.Add("@blog_title", System.Data.DbType.String).Value = blog.BlogTitle; }
			else { cmd.Parameters.Add("@blog_title", System.Data.DbType.String).Value = DBNull.Value; }

			if (blog.BlogStartDate != null) { cmd.Parameters.Add("@blog_start_date", System.Data.DbType.DateTime).Value = blog.BlogStartDate; }
			else { cmd.Parameters.Add("@blog_start_date", System.Data.DbType.DateTime).Value = DBNull.Value; }

			if (blog.BlogEndDate != null) { cmd.Parameters.Add("@blog_end_date", System.Data.DbType.DateTime).Value = blog.BlogEndDate; }
			else { cmd.Parameters.Add("@blog_end_date", System.Data.DbType.DateTime).Value = DBNull.Value; }

			if (blog.BlogAuthor != null) { cmd.Parameters.Add("@blog_author", System.Data.DbType.String).Value = blog.BlogAuthor; }
			else { cmd.Parameters.Add("@blog_author", System.Data.DbType.String).Value = DBNull.Value; }

		}


    }
}