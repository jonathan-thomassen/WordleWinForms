using WordleWinForms;
using Xunit;

namespace WordleWinFormsTests
{
    public class BannerTests
    {
        [Fact]
        public void DefaultConstructor_ShouldSetDefaultCaption()
        {
            // Arrange & Act
            var banner = new Banner();

            // Assert
            Assert.Equal("Welcome", banner.Caption);
        }

        [Fact]
        public void ParameterizedConstructor_ShouldSetCaption()
        {
            // Arrange
            string expectedCaption = "Test Caption";

            // Act
            var banner = new Banner(expectedCaption);

            // Assert
            Assert.Equal(expectedCaption, banner.Caption);
        }

        [Fact]
        public void SetCaption_ShouldUpdateCaption()
        {
            // Arrange
            var banner = new Banner();
            string newCaption = "New Caption";

            // Act
            banner.Caption = newCaption;

            // Assert
            Assert.Equal(newCaption, banner.Caption);
        }

        [Fact]
        public void ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var banner = new Banner("Test Caption");

            // Act
            string result = banner.ToString();

            // Assert
            Assert.Equal("Banner: Test Caption", result);
        }
    }
}
