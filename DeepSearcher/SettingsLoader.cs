using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSearcher
{
    internal static class SettingsLoader
    {
        private static IList<PathItem> _paths;

        internal static IList<PathItem> Paths
        {
            get
            {
                if (_paths == null)
                    Load();
                return _paths;
            }
            private set
            {
                _paths = value;
            }
        }

        public static void Load()
        {
            SerializableDictionary<string, bool> setting = Properties.Settings.Default.Paths;
            Paths = new List<PathItem>();
            if (setting == null)
                return;
            foreach (var item in setting)
            {
                Paths.Add(new PathItem(item.Key, Convert.ToBoolean(item.Value)));
            }
        }

        public static void Save(IList<PathItem> dictionary)
        {
            SerializableDictionary<string, bool> setting = Properties.Settings.Default.Paths;
            if (setting == null)
                setting = Properties.Settings.Default.Paths = new SerializableDictionary<string, bool>();
            setting.Clear();
            foreach (PathItem pathItem in dictionary)
            {
                setting.Add(pathItem.Path, pathItem.Checked);
            }
            Properties.Settings.Default.Save();

        }
    }
}
