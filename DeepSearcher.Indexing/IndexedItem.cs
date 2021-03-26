using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DeepSearcher.Interfaces;

namespace DeepSearcher.Indexing
{
    public class IndexedItem : ISearchItem
    {
        public string FullName { get; set; }

        public long Length { get; set; }

        public DateTime CreationTime { get; set; }

        public string Extension
        {
            get
            {
                return System.IO.Path.GetExtension(FullName);
            }
        }

        public string Name
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(FullName);
            }
        }

        public string DirectoryName
        {
            get
            {
                return Path.GetDirectoryName(FullName);
            }
        }

        public IndexedItem(string fullName, long length, DateTime createdDate)
        {
            FullName = fullName;
            Length = length;
            CreationTime = createdDate;
        }

        public IndexedItem(FileInfo info)
        {
            FullName = info.FullName;
            Length = info.Length;
            CreationTime = info.CreationTime;
        }

        public static IndexedItem Parse(string str)
        {
            string[] values = str.Split('?');
            return new IndexedItem(values[0], Convert.ToInt64(values[1]), Convert.ToDateTime(values[2]));
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(FullName);
            builder.Append("?");
            builder.Append(Length);
            builder.Append("?");
            builder.Append(CreationTime);
            return builder.ToString();
        }
    }
}
