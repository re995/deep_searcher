using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepSearcher.Interfaces;

namespace DeepSearcher.Models
{
    public class FileInfoWrapper : ISearchItem
    {
        private readonly FileInfo _fileInfo;

        public FileInfoWrapper(FileInfo info)
        {
            _fileInfo = info;
        }

        public string FullName
        {
            get
            {
                return _fileInfo.FullName;
            }
        }

        public string Name
        {
            get
            {
                return _fileInfo.Name;
            }
        }

        public string Extension
        {
            get
            {
                return _fileInfo.Extension;
            }
        }

        public long Length
        {
            get
            {
                return _fileInfo.Length;
            }
        }

        public DateTime CreationTime
        {
            get
            {
                return _fileInfo.CreationTime;
            }
        }

        public string DirectoryName
        {
            get
            {
                return _fileInfo.DirectoryName;
            }
        }
    }
}
