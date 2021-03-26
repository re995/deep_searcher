using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DeepSearcher.Indexing
{
    public class IndexFileWriter : IDisposable
    {
        private StreamWriter _writer;

        public IndexFileWriter(string path)
        {
            _writer = new StreamWriter(path);
        }

        public void Write(IEnumerable<IndexedItem> infos)
        {
            foreach (var indexFile in infos)
            {
                _writer.WriteLine(indexFile);
            }
            
        }

        public void Clear()
        {
            _writer.BaseStream.SetLength(0);
            _writer.Close();
        }

        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}
