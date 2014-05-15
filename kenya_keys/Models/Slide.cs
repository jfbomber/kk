using System;
using System.Collections.Generic;
using System.ComponentModel;
using Mono.Data.Sqlite;
using System.Linq;
using System.Web;

namespace KK.Models
{

    public class Slide
    {
        public static String connectionString = connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["KKDB"].ToString();
        [DisplayName("Project ID")]
        public int? SlideID { get; set; }
        [DisplayName("Slide Text")]
        public string SlideText { get; set; }
        [DisplayName("Slide Index")]
        public int SlideIndex { get; set; } 
        [DisplayName("Photo ID")]
        public int PhotoID { get; set; }
		[DisplayName("Photo")]
		public Photo Photo { get; set; } 
		

	
        /// <summary>
        /// Gets all project
        /// </summary>
        /// <returns>List of slides</returns>
        public static List<Slide> GetAll()
        {
            List<Slide> slides = new List<Slide>();
            string query = "SELECT * FROM kk_slide ORDER BY slide_index;";
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
                            Slide slide = new Slide();
                            Slide.DBFill(reader, slide);
                            slides.Add(slide);
                        }
                    }
                }
            }
            return slides;
        }

        /// <summary>
        /// Saves the slide
        /// </summary>
        public void Save()
        {
            bool isInsert = false;
            string query = string.Empty;
            if (this.SlideID == null)
            {
                isInsert = true;
                query = @"INSERT INTO kk_slide (slide_text, slide_index, photo_id) 
                          VALUES kk_slide (@slide_text, @slide_index, @photo_id); 
                          SELECT last_insert_rowid() FROM kk_slide;";
            } else {
                query = @"UPDATE kk_slide SET slide_text = @slide_text, 
                			slide_index = @slide_index,
                          	photo_id = @photo_id 
                          WHERE slide_id = @slide_id;";
            }
            
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@slide_text", System.Data.DbType.String).Value = this.SlideText;
                    cmd.Parameters.Add("@slide_index", System.Data.DbType.Int32).Value = this.SlideIndex;
                    cmd.Parameters.Add("@photo_id", System.Data.DbType.Int32).Value = this.PhotoID; 

                    if (isInsert)
                    {
                        this.SlideID = Convert.ToInt32(cmd.ExecuteScalar());
                        
                    }
                    else
                    {
                       cmd.Parameters.Add("@slide_id", System.Data.DbType.Int32).Value = this.SlideID;
                       cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Deletes a slide
        /// </summary>
        public static void Delete(int slideId)
        {

            string query = @"DELETE FROM kk_slide WHERE slide_id = @slide_id;";
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@slide_id", System.Data.DbType.Int32).Value = slideId;
                    cmd.ExecuteNonQuery();
                }
            }
        }
        
        /// <summary>
        /// DBFill
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="slide"></param>
		public static void DBFill(SqliteDataReader reader, Slide slide)
        {
            slide.SlideID = Convert.ToInt32(reader["slide_id"]);
			slide.SlideText = HttpUtility.UrlDecode(reader["slide_text"].ToString(), System.Text.Encoding.Default);
            slide.SlideIndex = Convert.ToInt32(reader["slide_index"]);
            slide.PhotoID = Convert.ToInt32(reader["photo_id"]);
            slide.Photo = Photo.Get(slide.PhotoID);
        }


        
    }
}