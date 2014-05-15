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
    public class Gallery
    {
        public static String connectionString = connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["KKDB"].ToString();

        public int? GalleryID { get; set; }
        public int GalleryIndex { get; set; } 
        // 700 x 350
        public byte[] GalleryPhotoData { get; set; } 
        public string GalleryDescription { get; set; }

        public Gallery(HttpPostedFileBase image, int index, string description) {
            this.SetPostedPhoto(image);
            this.GalleryIndex = index;
            this.GalleryDescription = description;
        }
        
        public void SetPostedPhoto(HttpPostedFileBase imageFile)
        {
			// Bitmap bmp = new Bitmap(imageFile.InputStream);
			// ImageConverter converter = new ImageConverter();
			// byte[] bitmapData = (byte[])converter.ConvertTo(bmp, typeof(byte[]));
            // this.GalleryPhotoData = (byte[])converter.ConvertTo(ResizeImage(bmp, 76), typeof(byte[]));
        }



    }
}