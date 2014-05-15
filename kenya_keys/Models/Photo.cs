using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using Mono.Data.Sqlite;

namespace KK.Models
{
    public class PhotoDropDown
    {
        public List<Photo> Photos { get; set; }
        public string PhotoViewID { get; set; } 
        public string OnChange { get; set; }
        public int? SelectedPhotoID { get; set; } 
        public PhotoDropDown()
        {
            this.Photos = Photo.GetAll();
            this.PhotoViewID = "Default-Photo-Viewer";
            this.OnChange = "alert('Set event in Photo.cs -> PhotoDropDown');";
        }
    }

    public class Photo
    {
        public static String connectionString = connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["KKDB"].ToString();
       /*************
		* CONSTANTS *
		*************/
        private const int LARGE = 600;
        private const int MEDIUM = 300;
        private const int SMALL = 150;
        private const int THUMB = 76;

		public enum PhotoSize
		{
			Full = 'f',
			Large = 'l',
			Medium = 'm',
			Small = 's',
			Thumb = 't'
		};

	   /**************
		* PROPERTIES *
		**************/
        public int? PhotoID { get; set; }
        public string PhotoName { get; set; } 
        public byte[] PhotoRawData { get; set; } 
        public byte[] PhotoLargeData { get; set; }
        public byte[] PhotoMediumData { get; set; }
        public byte[] PhotoSmallData { get; set; }
        public byte[] PhotoThumbData { get; set; }
        public int PhotoWidth { get; set; }
        public int PhotoHeight { get; set; }

       	// This creates the url for each photo type
        public string FullUrl { get { return string.Format("/Photo/GetPhoto?photoId={0}&size=f", this.PhotoID); } }
        public string LargeUrl { get { return string.Format("/Photo/GetPhoto?photoId={0}&size=l", this.PhotoID); } }
        public string MediumUrl { get { return string.Format("/Photo/GetPhoto?photoId={0}&size=m", this.PhotoID); } }
        public string SmallUrl { get { return string.Format("/Photo/GetPhoto?photoId={0}&size=s", this.PhotoID); } }
        public string ThumbUrl { get { return string.Format("/Photo/GetPhoto?photoId={0}&size=t", this.PhotoID); } }

		public string GetUrlBySize(PhotoSize photoSize) {
			string url = "";
			switch (photoSize) {
			case PhotoSize.Full:
				{
					url = this.FullUrl;
					break;
				}
			case PhotoSize.Large:
				{
					url = this.LargeUrl;
					break;
				}
			case PhotoSize.Medium:
				{
					url = this.MediumUrl;
					break;
				}
			case PhotoSize.Small:
				{
					url = this.SmallUrl;
					break;
				}
			case PhotoSize.Thumb:
			default :
				{
					url = this.ThumbUrl;
					break;
				}

			}
			return url;
		}

	   /***********
		* METHODS *
		***********/
       	/// <summary>
       	/// Converts a posted http image to a C# System.Drawing.Image
       	/// </summary>
       	/// <param name="postedImage">HttpPostedFileBase</param>
       	/// <returns>System.Drawing.Image</returns>
        public Photo() { }
        
       	/// <summary>
       	/// Converts a posted http image to a C# System.Drawing.Image
       	/// </summary>
       	/// <param name="postedImage">HttpPostedFileBase</param>
       	/// <returns>System.Drawing.Image</returns>
        public Photo(HttpPostedFileBase image, bool storeFullSize)
        {
            this.SetPostedPhoto(image, storeFullSize);
        }
       	/// <summary>
       	/// Converts a posted http image to a C# System.Drawing.Image
       	/// </summary>
       	/// <param name="postedImage">HttpPostedFileBase</param>
       	/// <returns>System.Drawing.Image</returns>
        public void SetPostedPhoto(HttpPostedFileBase imageFile, bool storeFullSize)
        {


            Bitmap bmp = new Bitmap(imageFile.InputStream);
            ImageConverter converter = new ImageConverter();
            byte[] rawData = (byte[])converter.ConvertTo(bmp, typeof(byte[]));
            this.PhotoName = imageFile.FileName;
            if (bmp.Width > LARGE)
            {
                this.PhotoLargeData = ConvertImageToByteArray(ResizeImageToWidth(bmp, LARGE));
            }
            else
            {
                storeFullSize = true;
            }
            

            if (storeFullSize)
            {
                this.PhotoRawData = rawData;
                this.PhotoWidth = bmp.Width;
                this.PhotoHeight = bmp.Height;
            }
            else
            {
                Image lgImage = ConvertByteArrayToImage(this.PhotoLargeData);
                this.PhotoWidth = lgImage.Width;
                this.PhotoHeight = lgImage.Height;
            }

            if (bmp.Width > MEDIUM) {
                this.PhotoMediumData = ConvertImageToByteArray(ResizeImageToWidth(bmp, MEDIUM));
                rawData = this.PhotoMediumData;
            }

            if (bmp.Width > SMALL)
            {
                this.PhotoSmallData = ConvertImageToByteArray(ResizeImageToWidth(bmp, SMALL));
                rawData = this.PhotoSmallData;
            }

            this.PhotoThumbData = ConvertImageToByteArray(ResizeImageToWidth(bmp, THUMB));
            this.Save();
        }

    	/// <summary>
       	/// Converts a posted http image to a C# System.Drawing.Image
       	/// </summary>
       	/// <param name="postedImage">HttpPostedFileBase</param>
       	/// <returns>System.Drawing.Image</returns>
        public void Save()
        {
            string query = string.Empty;
            bool isUpdate = false;
            if (this.PhotoID != null)
            {
                query = @"UPDATE kk_photo
                          SET photo_name = @photo_name, photo_raw = @photo_raw, photo_large = @photo_large, 
                              photo_medium =  @photo_medium, photo_small =  @photo_small, photo_thumb =  @photo_thumb, 
                              photo_width = @photo_width, photo_height = @photo_height
                          WHERE photo_id = @photo_id; ";
                isUpdate = true;
            }
            else
            {
                query = @"INSERT INTO kk_photo (photo_name, photo_raw, photo_large, photo_medium, photo_small, photo_thumb, photo_width, photo_height)
                          VALUES (@photo_name, @photo_raw, @photo_large, @photo_medium, @photo_small, @photo_thumb, @photo_width, @photo_height); 
                          SELECT last_insert_rowid() FROM kk_photo; ";
            }
            
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@photo_name", System.Data.DbType.String).Value = this.PhotoName;
                    cmd.Parameters.Add("@photo_raw", System.Data.DbType.Binary).Value = this.PhotoRawData;
                    cmd.Parameters.Add("@photo_large", System.Data.DbType.Binary).Value = this.PhotoLargeData;
                    cmd.Parameters.Add("@photo_medium", System.Data.DbType.Binary).Value = this.PhotoMediumData;
                    cmd.Parameters.Add("@photo_small", System.Data.DbType.Binary).Value = this.PhotoSmallData;
                    cmd.Parameters.Add("@photo_thumb", System.Data.DbType.Binary).Value = this.PhotoThumbData;
                    cmd.Parameters.Add("@photo_width", System.Data.DbType.Int32).Value = this.PhotoWidth;
                    cmd.Parameters.Add("@photo_height", System.Data.DbType.Int32).Value = this.PhotoHeight;

                    if (isUpdate)
                    {
                        cmd.Parameters.Add("@photo_id", System.Data.DbType.Int32).Value = this.PhotoID;
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                       this.PhotoID = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
        }
       /// <summary>
       /// Converts a posted http image to a C# System.Drawing.Image
       /// </summary>
       /// <param name="postedImage">HttpPostedFileBase</param>
       /// <returns>System.Drawing.Image</returns>
        public static List<Photo> GetAll()
        {
            List<Photo> photos = new List<Photo>();
            string query = @"SELECT photo_id, photo_name, photo_width, photo_height 
                             FROM kk_photo
                             ORDER BY photo_id;";
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
                            Photo photo = new Photo();
                            DBFill(reader, photo);
                            photos.Add(photo);
                        }
                    }

                }
            }
            return photos;
        }
       	
       	/// <summary>
       	/// Converts a posted http image to a C# System.Drawing.Image
       	/// </summary>
       	/// <param name="postedImage">HttpPostedFileBase</param>
       	/// <returns>System.Drawing.Image</returns>
        public static byte[] GetPhotoData(int photoId, string size = "t")
        {
            string column = "photo_thumb";
            switch (size)
            {
                case "f" : 
                case "full" : 
                case "l" : 
                case "lg" : 
                case "large" :
                    column = "photo_large";
                    break;
                case "med" :
                case "m" :
                case "medium" :
                    column = "photo_medium";
                    break;
                case "small":
                case "sm":
                case "s":
                    column = "photo_small";
                    break;
                case "t" :
                case "thumb" :
                    column = "photo_thumb";
                    break;
                default :
                    column = "photo_raw";
                 break;
            }
            byte[] data = null;
            string query = @"SELECT "+column+@" 
                             FROM kk_photo 
                             WHERE photo_id = @photo_id;";
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@photo_id", System.Data.DbType.Int32).Value = photoId;
                    // user reader to fill data
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        if (reader.HasRows)
                        {
                            data = (byte[])reader[column];
                        }   
                    }
                }
            }
            
            return data;
        }
       	
       	/// <summary>
       	/// Gets a photo by id
       	/// </summary>
       	/// <param name="S">Integer</param>
       	/// <returns>System.Drawing.Image</returns>
        public static Photo Get(int photoId, string[] columns = null)
        {
            if (columns == null || columns.Length == 0) { 
            	columns = new string[] { "photo_thumb" };
            }
            Photo photo = new Photo();
            string query = "SELECT photo_id, photo_name, photo_width, photo_height, " + string.Join(",", columns) + @"
                            FROM kk_photo 
                            WHERE photo_id = @photo_id;";
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@photo_id", System.Data.DbType.Int32).Value = photoId;
                    // user reader to fill data
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            Photo.DBFill(reader, photo, columns);
                        }

                    }

                }
            }
            return photo;
        }

       	/// <summary>
       	/// Converts a posted http image to a C# System.Drawing.Image
       	/// </summary>
       	/// <param name="postedImage">HttpPostedFileBase</param>
       	/// <returns>System.Drawing.Image</returns>
        public static void Delete(int photoId)
        {
            string query = "DELETE FROM kk_photo WHERE photo_id = @photo_id;";
            using (SqliteConnection conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                // execute cmd
                using (SqliteCommand cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.Add("@photo_id", System.Data.DbType.Int32).Value = photoId;
                    // user reader to fill data
                    cmd.ExecuteNonQuery();

                }
            }
        }
       	
       	/// <summary>
       	/// Converts a posted http image to a C# System.Drawing.Image
       	/// </summary>
       	/// <param name="postedImage">HttpPostedFileBase</param>
       	/// <returns>System.Drawing.Image</returns>
        public static void DBFill(SqliteDataReader reader, Photo photo, string[] columns = null)
        {
            photo.PhotoID = Convert.ToInt32(reader["photo_id"]);
            photo.PhotoName = reader["photo_name"].ToString();
            photo.PhotoWidth = Convert.ToInt32(reader["photo_width"]);
            photo.PhotoHeight = Convert.ToInt32(reader["photo_height"]);
            if (columns != null)
            {
                for (int i = 0; i < columns.Length; i++)
                {
                    string column = columns[i] as string;
                    switch (column)
                    {
                        case "photo_raw": photo.PhotoRawData = (byte[])reader["photo_raw"]; break;
                        case "photo_large": photo.PhotoRawData = (byte[])reader["photo_large"]; break;
                        case "photo_medium": photo.PhotoRawData = (byte[])reader["photo_medium"]; break;
                        case "photo_small": photo.PhotoRawData = (byte[])reader["photo_small"]; break;
                        case "photo_thumb": photo.PhotoRawData = (byte[])reader["photo_thumb"]; break;
                        default: break;
                    }
                }
            }
        }

		/// <summary>
		/// Converts a posted http image to a C# System.Drawing.Image
		/// </summary>
		/// <param name="postedImage">HttpPostedFileBase</param>
		/// <returns>System.Drawing.Image</returns>
        public static System.Drawing.Bitmap ConvertHttpPostedFileBaseToImage(HttpPostedFileBase postedImage) {
            try {
                using (var image = Image.FromStream(postedImage.InputStream, true, true)) {
                    return (System.Drawing.Bitmap)image;
                }
            } catch(Exception e) {
                throw e; 
            }
        }
		
		/// <summary>
		/// Resizes a byte array image
		/// </summary>
		/// <param name="photoData">byte[]</param>
		/// <param name="width">int</param>
		/// <returns>System.Byte[]</returns>
		public static byte[] ResizeImageAsByteArray(byte[] photoData, int width) {
            System.IO.MemoryStream myMemStream = new System.IO.MemoryStream(photoData);
            System.Drawing.Image fullsizeImage = System.Drawing.Image.FromStream(myMemStream);
            System.Drawing.Image newImage = ResizeImage((System.Drawing.Bitmap)fullsizeImage, width);
            return ConvertImageToByteArray(newImage);
        }
         
        /// <summary>
        /// Converts an image to a byte array
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public static byte[] ConvertImageToByteArray(System.Drawing.Image imageIn) {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }

        /// <summary>
        /// Converts a byte array to an image
        /// </summary>
        /// <param name="byteArrayIn">byte[]</param>
        /// <returns>System.Drawing.Image</returns>
        public static System.Drawing.Image ConvertByteArrayToImage(byte[] byteArrayIn) {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }

        /// <summary>
        /// Resizes the image
        /// </summary>
        /// <param name="imgPhoto">System.Drawing.Image</param>
        /// <param name="size">Int</param>
        /// <returns>System.Drawing.Image</returns>
        public static System.Drawing.Image ResizeImage(System.Drawing.Bitmap imgPhoto, int size) {
            var logoSize = size;
            float sourceWidth = imgPhoto.Width;
            float sourceHeight = imgPhoto.Height;
            float destHeight;
            float destWidth;

            const int sourceX = 0;
            const int sourceY = 0;
            const int destX = 0;
            const int destY = 0;

            // Resize Image to have the height = logoSize/2 or width = logoSize.
            // Height is greater than width, set Height = logoSize and resize width accordingly
            if (sourceWidth > (2 * sourceHeight))
            {
                destWidth = logoSize;
                destHeight = sourceHeight * logoSize / sourceWidth;
            }
            else
            {
                int h = logoSize / 2;
                destHeight = h;
                destWidth = sourceWidth * h / sourceHeight;
            }
            // Width is greater than height, set Width = logoSize and resize height accordingly

            var bmPhoto = new Bitmap((int)destWidth, (int)destHeight, PixelFormat.Format32bppPArgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            using (Graphics grPhoto = Graphics.FromImage(bmPhoto))
            {
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                grPhoto.DrawImage(imgPhoto,
                        new Rectangle(destX, destY, (int)destWidth, (int)destHeight),
                        new Rectangle(sourceX, sourceY, (int)sourceWidth, (int)sourceHeight),
                        GraphicsUnit.Pixel);
                grPhoto.Dispose();
            }
            return bmPhoto;
        }

        /// <summary>
        /// Resizes the image to a specific width
        /// </summary>
        /// <param name="imgPhoto">System.Drawing.Image</param>
        /// <param name="size">Int</param>
        /// <returns></returns>
        public static System.Drawing.Image ResizeImageToWidth(System.Drawing.Bitmap imgPhoto, int width)
        {
            float sourceWidth = imgPhoto.Width;
            float sourceHeight = imgPhoto.Height;
            float destHeight = 0.0F;
            float destWidth = 0.0F;

            const int sourceX = 0;
            const int sourceY = 0;
            const int destX = 0;
            const int destY = 0;

            if (imgPhoto.Width > width)
            {
                decimal pct = (Convert.ToDecimal(width) / Convert.ToDecimal(imgPhoto.Width));
                destWidth = (float)Convert.ToDecimal(width);
                destHeight = (float)(Convert.ToDecimal(imgPhoto.Height) * pct);
            }
            else
            {
                return (System.Drawing.Image)imgPhoto;
            }
             
            var bmPhoto = new Bitmap((int)destWidth, (int)destHeight, PixelFormat.Format32bppPArgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            using (Graphics grPhoto = Graphics.FromImage(bmPhoto))
            {
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                grPhoto.DrawImage(imgPhoto,
                        new Rectangle(destX, destY, (int)destWidth, (int)destHeight),
                        new Rectangle(sourceX, sourceY, (int)sourceWidth, (int)sourceHeight),
                        GraphicsUnit.Pixel);
                grPhoto.Dispose();
            }
            return bmPhoto;
        }
        
               	
       	/// <summary>
       	/// Crops the Image
       	/// NOTE : This has not yet been implemeneted
       	/// </summary>
       	/// <param name="img">System.Drawing.Image</param>
       	/// <param name="cropArea">System.Drawing.Rectangle</param>
       	/// <returns>System.Drawing.Image</returns>
        private static Image Crop(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            return (Image)(bmpCrop);
        }
    }
}