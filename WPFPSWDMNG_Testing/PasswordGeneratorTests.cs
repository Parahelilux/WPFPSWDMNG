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
        public void Generate_ShouldNotContainDisallowedCharacters()
        {
            // Arrange
            int length = 12;
            string disallowedChars = "!@#$%^&*()_+-=[]{}|;:,.<>?/~`";

            // Act
            string password = _passwordGenerator.Generate(length);

            // Assert
            Assert.All(password, c => Assert.DoesNotContain(c, disallowedChars));
        }
    }
}
