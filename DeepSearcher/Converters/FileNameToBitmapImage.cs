using System;
using System.Drawing;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using DeepSearcher.Helpers;

namespace DeepSearcher.Converters
{
    public class FileNameToBitmapImage : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var path = value as string;
            string extention = Path.GetExtension(path);
            if (extention == null)
                return null;
            Image img = null;
            if (extention.ToLower() == ".bmp" || extention.ToLower() == ".png" || extention.ToLower() == ".jpg" || extention.ToLower() == ".gif")
            {
                try
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        using (Image original = Image.FromStream(fs))
                        {
                            img = original.GetThumbnailImage(32, 32, () => false, IntPtr.Zero);
                        }
                    }
                }
                catch (ArgumentException)
                {
                    img = ExeBitmapExtractor.ExtractBitmap(path);
                }
            }
            else
            {
                img = ExeBitmapExtractor.ExtractBitmap(path);
            }
            using (img)
            {
                return TranslateToBitmapImage(img);
            }
        }

        private static BitmapImage TranslateToBitmapImage(Image img)
        {
            using (img)
            {
                if (img == null)
                    return null;
                var ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                var image = new BitmapImage();
                image.BeginInit();
                ms.Seek(0, SeekOrigin.Begin);
                image.StreamSource = ms;
                image.EndInit();

                return image; 
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
