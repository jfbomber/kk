using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;

namespace KK
{
    public class ImageHelper
    {

        public static System.Drawing.Image resizeWidth(System.Drawing.Image Img, int width)
        {
            // Get the dimensions to resize the height
            decimal pct = (decimal)width / (decimal)Img.Width;
            int height = Convert.ToInt32(Img.Height * pct);

            int sourceWidth = Img.Width;
            int sourceHeight = Img.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            // Get new percents to calculate
            nPercentW = ((float)width / (float)sourceWidth);
            nPercentH = ((float)height / (float)sourceHeight);

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((width - (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((height - (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            // Create a new bitmap
            Bitmap bitmapImage = new Bitmap(width, height,
            System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            bitmapImage.SetResolution(Img.HorizontalResolution, Img.VerticalResolution);

            // Convert the bitmap to a Graphic
            Graphics graphicImage = Graphics.FromImage(bitmapImage);
            graphicImage.Clear(Color.White);
            graphicImage.InterpolationMode =
            System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            // Draw the graphic with the new dimensions
            graphicImage.DrawImage(Img,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            graphicImage.Dispose();
            return Img;
        }
        
        // Converts and Image to a Byte Array
        public static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }

        // Converts a byte array to an Image
        public static System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage =
                System.Drawing.Image.FromStream(ms);
            return returnImage;
        }

    }
}