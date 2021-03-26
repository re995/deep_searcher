using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace DeepSearcher.Helpers
{
    public static class ExeBitmapExtractor
    {
        private static bool _notifiedIndexNeeded;

        public static Bitmap ExtractBitmap(string path)
        {
            var converter = new IconConverter();
            if (converter.CanConvertTo(typeof (Bitmap)) && File.Exists(path))
            {
                try
                {
                    using (var icon = Icon.ExtractAssociatedIcon(path))
                    {
                        if (icon != null)
                        {
                            var bitmap = converter.ConvertTo(icon, typeof (Bitmap)) as Bitmap;
                            return bitmap;
                        }
                    }
                }
                catch (ArgumentException)
                {
                    return null;
                }
            }

            return null;
        }
    }
}
