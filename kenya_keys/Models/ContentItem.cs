using System;
using System.Collections.Generic;
using System.ComponentModel;
using Mono.Data.Sqlite;
using System.Linq;
using System.Web;

namespace KK.Models
{

    public class ContentItem
    {
        public static String connectionString = connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["KKDB"].ToString();

        public int? ItemID { get; set; }
        public Content Content { get; set; } 
        public int ContentID { get; set; } 
        public string ItemTitle { get; set; } 
        public string ItemHtml { get; set; } 
        public DateTime ItemAdded { get; set; } 
        public string ItemAddedBy { get; set; }
        public DateTime ItemLastUpdated { get; set; }
        public string ItemLastUpdatedBy { get; set; } 
        public int? ItemPhotoID { get; set; } 
        public bool ItemHidden { get; set; }
        public bool IsAdmin { get; set; }

        public string ItemElementID { get { return "ContentItemDisplay-" + (this.ItemID == null ? "0" : this.ItemID.ToString()); } }
        public string ItemFormID { get { return "ContentItemDisplay-" + (this.ItemID == null ? "0" : this.ItemID.ToString()) + "-Form"; } }

        /// <summary>
        /// Gets as single project by id
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static ContentItem Get(int itemId)
        {
            ContentItem contentItem = new ContentItem();
            string query = "SELECT * FROM kk_content_item WHERE content_item_id = @content_item_id;";
            // get database connection
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@content_item_id", System.Data.DbType.Int32).Value = itemId;
                    // user reader to fill data
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        ContentItem.DBFill(reader, contentItem);
                    }

                }
            }
            return contentItem;
        }

        /// <summary>
        /// Gets all project
        /// </summary>
        /// <returns>Array of projects</returns>
        public static List<ContentItem> GetAll(int contentID)
        {
            List<ContentItem> contentItems = new List<ContentItem>();
            string query = "SELECT * FROM kk_content_item WHERE content_id = @content_id AND content_item_hidden = 0;";
            // get database connection
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@content_id", System.Data.DbType.Int32).Value = contentID;
                    // user reader to fill data
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ContentItem contentItem = new ContentItem();
                            ContentItem.DBFill(reader, contentItem);
                            contentItems.Add(contentItem);
                        }
                    }
                }
            }
            return contentItems;
        }

        /// <summary>
        /// Saves the ContentItem (
        /// </summary>
        public void Save()
        {
            bool isInsert = false;
            string query = string.Empty;
            if (this.ItemID == null)
            {
                isInsert = true;
                query = @"INSERT INTO kk_content_item (content_id, content_item_title, content_item_html, content_item_added_by, content_item_photo_id, content_item_updated_by) 
                          VALUES (@content_id, @content_item_title, @content_item_html, @content_item_added_by, @content_item_photo_id, @content_item_updated_by);
                          SELECT last_insert_rowid() FROM kk_content_item; ";
            } else {
                query = @"UPDATE kk_content_item SET 
                            content_id = @content_id, content_item_title = @content_item_title, content_item_html = @content_item_html, 
                            content_item_photo_id = @content_item_photo_id, content_item_updated_by = @content_item_updated_by,
                            content_item_updated = current_timestamp, content_item_hidden = @content_item_hidden
                          WHERE content_item_id = @content_item_id;";
            }
            
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("content_id", System.Data.DbType.Int32).Value = this.ContentID;
                    cmd.Parameters.Add("content_item_title", System.Data.DbType.String).Value = this.ItemTitle;
                    cmd.Parameters.Add("content_item_html", System.Data.DbType.String).Value = this.ItemHtml;
                    
                    if (this.ItemPhotoID != null) {
                        cmd.Parameters.Add("content_item_photo_id", System.Data.DbType.String).Value = this.ItemPhotoID;
                    } else {
                        cmd.Parameters.Add("content_item_photo_id", System.Data.DbType.String).Value = DBNull.Value;
                    }

                    if (this.ItemLastUpdatedBy != null) {
                        cmd.Parameters.Add("content_item_updated_by", System.Data.DbType.String).Value = this.ItemLastUpdatedBy;
                    } else {
                        cmd.Parameters.Add("content_item_updated_by", System.Data.DbType.String).Value = DBNull.Value;
                    }

                    if (!isInsert)
                    {
                        cmd.Parameters.Add("@content_item_hidden", System.Data.DbType.Int16).Value = this.ItemHidden;
                        cmd.Parameters.Add("@content_item_id", System.Data.DbType.Int32).Value = this.ItemID;
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        if (this.ItemAddedBy != null) {
                            cmd.Parameters.Add("content_item_added_by", System.Data.DbType.String).Value = this.ItemAddedBy;
                        } else {
                            cmd.Parameters.Add("content_item_added_by", System.Data.DbType.String).Value = DBNull.Value;
                        }

                        this.ItemID = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                }
            }
        }

        /// <summary>
        /// Saves the Project
        /// </summary>
        public static void Delete(int itemID)
        {

            string query = @"DELETE FROM kk_content_item WHERE content_item_id = @content_item_id;";
            
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@content_item_id", System.Data.DbType.Int32).Value = itemID;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Insert
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="project"></param>
        public static void DBFill(SqliteDataReader reader, ContentItem contentItem)
        {
            contentItem.ItemID = Convert.ToInt32(reader["content_item_id"]);
            contentItem.ContentID = Convert.ToInt32(reader["content_id"]);
            contentItem.ItemTitle = reader["content_item_title"].ToString();
            contentItem.ItemHtml = HttpUtility.UrlDecode(reader["content_item_html"].ToString(), System.Text.Encoding.Default);
            contentItem.ItemAddedBy = reader["content_item_added_by"].ToString();
            contentItem.ItemLastUpdatedBy = reader["content_item_updated_by"].ToString();
            if (reader["content_item_updated"] != DBNull.Value) { contentItem.ItemLastUpdated = Convert.ToDateTime(reader["content_item_updated"]); }
            if (reader["content_item_added"] != DBNull.Value) { contentItem.ItemAdded = Convert.ToDateTime(reader["content_item_added"]); }
            if (reader["content_item_photo_id"] != DBNull.Value) { contentItem.ItemPhotoID = Convert.ToInt32(reader["content_item_photo_id"]); }
            contentItem.ItemHidden = Convert.ToBoolean(reader["content_item_hidden"]);
        }


        
    }
}