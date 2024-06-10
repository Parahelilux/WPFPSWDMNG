using System.Collections.Generic;
using System.IO;

namespace WPFPSWDMNG
{
    public class FileService : IFileService
    {
        public void Save(string filePath, IEnumerable<string> lines)
        {
            File.WriteAllLines(filePath, lines);
        }
        /// Test 1
        public IEnumerable<string> Load(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllLines(filePath);
            }
            return new List<string>();
        }
    }
}
