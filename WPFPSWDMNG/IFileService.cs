using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPSWDMNG
{
    public interface IFileService
    {
        void Save(string filePath, IEnumerable<string> lines);
        IEnumerable<string> Load(string filePath);
    }
}

