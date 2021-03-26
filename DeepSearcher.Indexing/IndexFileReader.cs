using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DeepSearcher.Indexing
{
    public class IndexFileReader : IDisposable
    {
        private StreamReader _reader;

        public IndexFileReader(string path)
        {
            _reader = new StreamReader(path);
        }

        public IEnumerable<IndexedItem> ReadAll()
        {
            string line;
            while ((line = _reader.ReadLine()) != null)
            {
                yield return IndexedItem.Parse(line);
            }
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
