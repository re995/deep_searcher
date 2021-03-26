using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepSearcher.Models;

namespace DeepSearcher.Helpers
{
    internal static class FileInfoWrapperExtentions
    {
        internal static IEnumerable<FileInfoWrapper> Wrap(this IEnumerable<FileInfo> infos)
        {
            foreach (var fileInfo in infos)
            {
                yield return new FileInfoWrapper(fileInfo);
            }
        }
    }
}
