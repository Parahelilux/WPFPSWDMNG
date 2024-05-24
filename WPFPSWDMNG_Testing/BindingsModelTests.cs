using System.IO;
using Xunit;
using WPFPSWDMNG;

namespace WPFPSWDMNG_Testing
{
    public class BindingsModelTests
    {
        private BindingsModel _model;

        public BindingsModelTests()
        {
            _model = new BindingsModel();
        }

        [Fact]
        public void GeneratePassword_ShouldGeneratePasswordWithCorrectLength()
        {
            // Arrange
            _model.PasswordLength = 10;

            // Act
            _model.GenerateCommand.Execute(null);

            // Assert
            Assert.Equal(10, _model.Password.Length);
        }

        [Fact]
        public void SavePassword_ShouldAddPasswordEntryToCollection()
        {
            // Arrange
            _model.Website = "https://example.com";
            _model.Username = "testuser";
            _model.Password = "Password123!";

            // Act
            _model.SaveCommand.Execute(null);

            // Assert
            Assert.Single(_model.PasswordEntries);
            Assert.Equal("https://example.com", _model.PasswordEntries[0].Website);
            Assert.Equal("testuser", _model.PasswordEntries[0].Username);
            Assert.Equal("Password123!", _model.PasswordEntries[0].Password);
        }

        [Fact]
        public void DeletePassword_ShouldRemovePasswordEntryFromCollection()
        {
            // Arrange
            PasswordEntry entry = new PasswordEntry
            {
                Website = "https://example.com",
                Username = "testuser",
                Password = "Password123!"
            };
            _model.PasswordEntries.Add(entry);

            // Act
            _model.DeleteCommand.Execute(entry);

            // Assert
            Assert.Empty(_model.PasswordEntries);
        }

        [Fact]
        public void PurgeAllPasswords_ShouldClearPasswordEntries()
        {
            // Arrange
            _model.PasswordEntries.Add(new PasswordEntry());

            // Act
            _model.PurgeAllCommand.Execute(null);

            // Assert
            Assert.Empty(_model.PasswordEntries);
        }

        [Fact]
        public void ChangeFilePath_ShouldUpdateFilePath()
        {
            // Arrange
            string newFilePath = Path.Combine(Path.GetTempPath(), "test.txt");

            // Act
            _model.ChangeFilePathCommand.Execute(null);
            // Simulate user input by setting the FilePath property
            _model.FilePath = newFilePath;

            // Assert
            Assert.Equal(newFilePath, _model.FilePath);
        }

        [Fact]
        public void LoadFromFile_ShouldLoadPasswordEntriesFromFile()
        {
            // Arrange
            string tempFilePath = Path.GetTempFileName();
            File.WriteAllText(tempFilePath, "https://example.com,testuser,encryptedPassword");

            // Act
            _model.LoadFromFileCommand.Execute(null);
            // Simulate user input by setting the FilePath property
            _model.FilePath = tempFilePath;

            // Assert
            Assert.Single(_model.PasswordEntries);
            Assert.Equal("https://example.com", _model.PasswordEntries[0].Website);
            Assert.Equal("testuser", _model.PasswordEntries[0].Username);
            Assert.NotNull(_model.PasswordEntries[0].Password); // Decrypted password should not be null

            // Clean up
            File.Delete(tempFilePath);
        }
    }
}