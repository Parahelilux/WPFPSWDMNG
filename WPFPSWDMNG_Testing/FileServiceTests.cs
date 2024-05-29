using System.IO;
using WPFPSWDMNG;
using Xunit;

namespace WPFPSWDMNG.Tests
{
    public class FileServiceTests
    {
        private readonly FileService _fileService;
        private readonly string _testFilePath;

        public FileServiceTests()
        {
            _fileService = new FileService();
            _testFilePath = Path.GetTempFileName();
        }

        [Fact]
        public void Save_ShouldWriteLinesToFile()
        {
            // Arrange
            var lines = new[] { "line1", "line2", "line3" };

            // Act
            _fileService.Save(_testFilePath, lines);

            // Assert
            var savedLines = File.ReadAllLines(_testFilePath);
            Assert.Equal(lines, savedLines);
        }

        [Fact]
        public void Load_ShouldReadLinesFromFile()
        {
            // Arrange
            var lines = new[] { "line1", "line2", "line3" };
            File.WriteAllLines(_testFilePath, lines);

            // Act
            var loadedLines = _fileService.Load(_testFilePath);

            // Assert
            Assert.Equal(lines, loadedLines);
        }
    }
}
