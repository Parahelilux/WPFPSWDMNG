using Moq;
using System.Windows;
using WPFPSWDMNG;
using Xunit;

namespace WPFPSWDMNG.Tests
{
    public class PasswordManagerViewModelTests
    {
        private readonly Mock<IPasswordGenerator> _passwordGeneratorMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly Mock<IPasswordProtector> _passwordProtectorMock;
        private readonly Mock<IMessageBoxService> _messageBoxServiceMock;
        private readonly PasswordManagerViewModel _viewModel;

        public PasswordManagerViewModelTests()
        {
            _passwordGeneratorMock = new Mock<IPasswordGenerator>();
            _fileServiceMock = new Mock<IFileService>();
            _passwordProtectorMock = new Mock<IPasswordProtector>();
            _messageBoxServiceMock = new Mock<IMessageBoxService>();
            _viewModel = new PasswordManagerViewModel(
                _passwordGeneratorMock.Object,
                _fileServiceMock.Object,
                _passwordProtectorMock.Object,
                _messageBoxServiceMock.Object);
        }

        [Fact]
        public void GeneratePassword_Command_ShouldGeneratePassword()
        {
            // Arrange
            string generatedPassword = "TestPassword123!";
            _passwordGeneratorMock.Setup(pg => pg.Generate(It.IsAny<int>())).Returns(generatedPassword);

            // Act
            _viewModel.PasswordLength = 12;
            _viewModel.GenerateCommand.Execute(null);

            // Assert
            Assert.Equal(generatedPassword, _viewModel.Password);
        }

        [Fact]
        public void SavePassword_Command_ShouldSavePassword()
        {
            // Arrange
            _viewModel.Website = "example.com";
            _viewModel.Username = "user";
            _viewModel.Password = "TestPassword123!";
            _passwordProtectorMock.Setup(pp => pp.EncryptPassword(It.IsAny<string>())).Returns("EncryptedPassword");

            // Act
            _viewModel.SaveCommand.Execute(null);

            // Assert
            _fileServiceMock.Verify(fs => fs.Save(It.IsAny<string>(), It.IsAny<string[]>()), Times.Once);
            Assert.Contains(_viewModel.PasswordEntries, pe => pe.Website == "example.com" && pe.Username == "user" && pe.Password == "TestPassword123!");
        }

        [Fact]
        public void DeletePassword_Command_ShouldRemovePassword()
        {
            // Arrange
            var passwordEntry = new PasswordEntry { Website = "example.com", Username = "user", Password = "TestPassword123!" };
            _viewModel.PasswordEntries.Add(passwordEntry);
            _viewModel.SelectedPasswordEntry = passwordEntry;

            _messageBoxServiceMock
                .Setup(mb => mb.Show("Are you sure you want to delete the selected password entry?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question))
                .Returns(MessageBoxResult.Yes);

            // Act
            _viewModel.DeleteCommand.Execute(passwordEntry);

            // Assert
            Assert.DoesNotContain(passwordEntry, _viewModel.PasswordEntries);
        }

        [Fact]
        public void PurgeAllPasswords_Command_ShouldClearAllPasswords()
        {
            // Arrange
            _viewModel.PasswordEntries.Add(new PasswordEntry { Website = "example.com", Username = "user1", Password = "TestPassword123!" });
            _viewModel.PasswordEntries.Add(new PasswordEntry { Website = "example2.com", Username = "user2", Password = "TestPassword456!" });

            _messageBoxServiceMock.Setup(mb => mb.Show(It.IsAny<string>(), It.IsAny<string>(), MessageBoxButton.YesNo, MessageBoxImage.Warning)).Returns(MessageBoxResult.Yes);

            // Act
            _viewModel.PurgeAllCommand.Execute(null);

            // Assert
            Assert.Empty(_viewModel.PasswordEntries);
        }
    }
}
