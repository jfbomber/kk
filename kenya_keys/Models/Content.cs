using System;
using System.Collections.Generic;
using System.ComponentModel;
using Mono.Data.Sqlite;
using System.Linq;
using System.Web;

namespace KK.Models
{

    public class Content
    {
        public static String connectionString = connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["KKDB"].ToString();
        
        public int? ContentID { get; set; }
        public string ContentController { get; set; }
        public string ContentAction { get; set; }
        public string ContentTitle { get; set; } 
        public string ContentHtml { get; set; }
        public string ContentAddedBy { get; set; }
        public DateTime ContentAdded { get; set; }
        public DateTime ContentUpdated { get; set; }
        public string ContentUpdatedBy { get; set; }
        public int? ContentPhotoID { get; set; } 
        public List<ContentItem> ContentItems { get; set; }

        /// <summary>
        /// Stored in case of a querystring
        /// </summary>
        public ContentItem ContentItem { get; set; } 
        // helper properties
        public Boolean IsAdmin { get; set; } 
        public string ContentElementID { get { return "ContentDisplay-" + (this.ContentID == null ? "0" : this.ContentID.ToString()); } }
        public string ContentFormID { get { return "ContentDisplay-" + (this.ContentID == null ? "0" : this.ContentID.ToString()) + "-Form"; } }
        public string ElementDisplayID { get { return "ContentDisplay-" + (this.ContentID == null ? "0" : this.ContentID.ToString()) + "-Display"; } }
        public string ElementPhotoViewerID { get { return "ContentDisplay-" + (this.ContentID == null ? "0" : this.ContentID.ToString()) + "-PhotoViewer"; } }
        public string ElementTitleID { get { return "ContentDisplay-" + (this.ContentID == null ? "0" : this.ContentID.ToString()) + "-Title"; } }
        public string ElementHtmlID { get { return "ContentDisplay-" + (this.ContentID == null ? "0" : this.ContentID.ToString()) + "-Html"; } }
        
        
        /// <summary>
        /// Gets as single project by id
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static Content Get(string controller, string action, string title)
        {
            Content content = new Content();
            string query = @"SELECT * FROM kk_content 
                            WHERE content_action = @action 
                            AND content_controller = @controller AND content_title = @title";
            // get database connection
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@action", System.Data.DbType.String).Value = action;
                    cmd.Parameters.Add("@controller", System.Data.DbType.String).Value = controller;
                    cmd.Parameters.Add("@title", System.Data.DbType.String).Value = title;
                    
                    // user reader to fill data
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Content.DBFill(reader, content);
                    }

                }
            }
            return content;
        }

        public static Content Get(int contentId)
        {
            Content content = new Content();
            string query = "SELECT * FROM kk_content WHERE content_id = @id;";
            // get database connection
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@id", System.Data.DbType.Int32).Value = contentId;
                    // user reader to fill data
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Content.DBFill(reader, content);
                    }

                }
            }
            return content;
        }

        /// <summary>
        /// Saves the Project
        /// </summary>
        public void Save()
        {
            bool isInsert = false;
            string query = string.Empty;
            if (this.ContentID == null)
            {
                isInsert = true;
                query = @"INSERT INTO kk_content (content_controller, content_action, content_title, content_html, content_added_by, content_updated_by, content_photo_id)
                          VALUES (@content_controller, @content_action, @content_title, @content_html, @content_added_by, @content_updated_by, @content_photo_id)
                          SELECT last_insert_rowid() FROM kk_content; ";
            } else {
                query = @"UPDATE kk_content 
                          SET content_controller = @content_controller, content_action = @content_action, content_title = @content_title, content_photo_id = @content_photo_id, 
                              content_html = @content_html, content_added_by = @content_added_by, content_updated_by = @content_updated_by, content_updated_dt = current_timestamp
                          WHERE content_id = @content_id;";
            }
            
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@content_controller", System.Data.DbType.String).Value = this.ContentController;
                    cmd.Parameters.Add("@content_action", System.Data.DbType.String).Value = this.ContentAction;
                    cmd.Parameters.Add("@content_title", System.Data.DbType.String).Value = this.ContentTitle;
                    cmd.Parameters.Add("@content_html", System.Data.DbType.String).Value = this.ContentHtml;
                    
                    if (this.ContentAddedBy != string.Empty)
                        cmd.Parameters.Add("@content_added_by", System.Data.DbType.String).Value = this.ContentAddedBy;
                    else
                        cmd.Parameters.Add("@content_added_by", System.Data.DbType.String).Value = DBNull.Value;

                    if (this.ContentPhotoID != null)
                        cmd.Parameters.Add("@content_photo_id", System.Data.DbType.Int32).Value = this.ContentPhotoID;
                    else
                        cmd.Parameters.Add("@content_photo_id", System.Data.DbType.Int32).Value = DBNull.Value;

                    if (this.ContentUpdatedBy != string.Empty)
                        cmd.Parameters.Add("@content_updated_by", System.Data.DbType.String).Value = this.ContentUpdatedBy;
                    else
                        cmd.Parameters.Add("@content_updated_by", System.Data.DbType.String).Value = DBNull.Value;

                    if (isInsert)
                    {
                        this.ContentID = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                       cmd.Parameters.Add("@content_id", System.Data.DbType.Int32).Value = this.ContentID;
                       cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="project"></param>
        public static void DBFill(SqliteDataReader reader, Content content)
        {
            content.ContentID = Convert.ToInt32(reader["content_id"]);
            content.ContentAction = reader["content_action"].ToString();
            content.ContentController = reader["content_controller"].ToString();

            if (reader["content_title"] != DBNull.Value) { content.ContentTitle = reader["content_title"].ToString(); }
            if (reader["content_photo_id"] != DBNull.Value) { content.ContentPhotoID = Convert.ToInt32(reader["content_photo_id"]); }

            content.ContentHtml = HttpUtility.UrlDecode(reader["content_html"].ToString(), System.Text.Encoding.Default);
            content.ContentUpdatedBy = reader["content_updated_by"].ToString();
            content.ContentUpdated = Convert.ToDateTime(reader["content_updated_dt"]);
            content.ContentAddedBy = reader["content_added_by"].ToString();
            content.ContentAdded = Convert.ToDateTime(reader["content_added_dt"]);
            content.ContentItems = ContentItem.GetAll(Convert.ToInt32(content.ContentID));
        }


        
    }
}