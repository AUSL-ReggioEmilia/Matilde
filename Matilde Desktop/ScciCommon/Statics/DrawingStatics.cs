using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace UnicodeSrl.Scci.Statics
{
    public static class DrawingProcs
    {
        public static string getStringFromFont(Font font)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
            return converter.ConvertToString(font);

        }

        public static Font getFontFromString(string fontString)
        {

            string[] fontDivided = fontString.Split(';');

            fontDivided[1] = fontDivided[1].Remove(fontDivided[1].Length - 2).Replace(',', '.');

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));

            System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";

            return new System.Drawing.Font(new FontFamily(fontDivided[0]), float.Parse(fontDivided[1], provider));

        }

        public static byte[] GetByteFromImage(Image image)
        {

            try
            {

                if (image != null)
                {
                    MemoryStream stream = new MemoryStream();
                    image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    byte[] pic = stream.ToArray();
                    return pic;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception)
            {
                return null;
            }

        }
        public static Image GetImageFromByte(object dc)
        {

            try
            {

                if (dc == System.DBNull.Value) return null;

                Byte[] data = new Byte[0];
                data = (Byte[])(dc);
                MemoryStream mem = new MemoryStream(data);
                return Image.FromStream(mem);

            }
            catch (Exception)
            {
                return null;
            }

        }
        public static Image GetImageFromByte(object dc, Size imagesize)
        {
            return GetImageFromByte(dc, imagesize, true);
        }
        public static Image GetImageFromByte(object dc, Size imagesize, bool preserveAspectRatio)
        {
            try
            {

                Image imgRet = GetImageFromByte(dc);
                return ResizeImage(imgRet, imagesize, preserveAspectRatio);

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static BitmapSource GetBitmapSourceFromByte(byte[] image)
        {

            BitmapImage imageSource = new BitmapImage();

            if (image != null)
            {

                imageSource.BeginInit();
                imageSource.StreamSource = new System.IO.MemoryStream(image);
                imageSource.EndInit();

            }

            return imageSource;

        }

        public static Image ResizeImage(Image image, Size size, bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;
            if (image != null)
            {
                if (preserveAspectRatio)
                {
                    int originalWidth = image.Width;
                    int originalHeight = image.Height;
                    float percentWidth = (float)size.Width / (float)originalWidth;
                    float percentHeight = (float)size.Height / (float)originalHeight;
                    float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                    newWidth = (int)(originalWidth * percent);
                    newHeight = (int)(originalHeight * percent);
                }
                else
                {
                    newWidth = size.Width;
                    newHeight = size.Height;
                }
                Image newImage = new Bitmap(newWidth, newHeight);
                using (Graphics graphicsHandle = Graphics.FromImage(newImage))
                {
                    graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
                }
                return newImage;
            }
            else
            {
                return null;
            }
        }

    }
}
