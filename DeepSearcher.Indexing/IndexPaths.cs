using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DeepSearcher.Indexing
{
    public static class IndexPaths
    {
        private static string[] _paths;

        public static IEnumerable<string> Paths
        {
            get
            {
                if (_paths == null)
                    LoadPaths();
                return _paths;
            }
        }

        public static void LoadPaths()
        {
            string[] files = Directory.GetFiles(Directory.GetParent(System.Windows.Forms.Application.UserAppDataPath).ToString(), "*.idx");
            _paths = files;
        }

        public static string GetIdxFilePath(string searchPath)
        {
            return Path.Combine(Directory.GetParent(System.Windows.Forms.Application.UserAppDataPath).ToString(), string.Format("{0}.idx", searchPath.Replace("\\", "_").Replace(":", "")));
        }

        public static bool IdxExists(string searchPath)
        {
            return File.Exists(GetIdxFilePath(searchPath));
        }
    }
}
