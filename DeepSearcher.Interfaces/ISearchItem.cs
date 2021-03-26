using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSearcher.Interfaces
{
    public interface ISearchItem
    {

        string FullName { get; }
        string Name { get; }

        string Extension { get; }

        long Length { get; }

        DateTime CreationTime { get; }

        string DirectoryName { get; }
    }
}
