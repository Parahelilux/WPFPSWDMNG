using WPFPSWDMNG;
using Xunit;

namespace WPFPSWDMNG.Tests
{
    public class PasswordGeneratorTests
    {
        private readonly PasswordGenerator _passwordGenerator;

        public PasswordGeneratorTests()
        {
            _passwordGenerator = new PasswordGenerator();
        }

        [Theory]
        [InlineData(8)]
        [InlineData(12)]
        [InlineData(16)]
        [InlineData(20)]
        [InlineData(32)]
        [InlineData(64)]

        public void Generate_ShouldReturnPasswordOfSpecifiedLength(int length)
        {
            // Act
            string password = _passwordGenerator.Generate(length);

            // Assert
            Assert.Equal(length, password.Length);
        }

        [Fact]
        public void Generate_ShouldContainDifferentCharacterTypes()
        {
            // Arrange
            int length = 12;

            // Act
            string password = _passwordGenerator.Generate(length);

            // Assert
            Assert.Contains(password, c => char.IsLower(c));
            Assert.Contains(password, c => char.IsUpper(c));
            Assert.Contains(password, c => char.IsDigit(c));
            Assert.Contains(password, c => "!@#$%^&*()_+-=[]{}|;:,.<>?/~`".Contains(c));
        }
    }
}