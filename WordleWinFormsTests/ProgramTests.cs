using Moq;
using SystemWrapper.Forms;
using Xunit;
using WordleWinForms;

namespace WordleWinFormsTests
{
    public class ProgramTests
    {
        [Fact]
        public void Main_ShouldInitializeApplicationConfiguration()
        {
            // Arrange
            var applicationMock = new Mock<IApplication>();
            ApplicationWrapper.SetApplication(applicationMock.Object);

            // Act
            Program.Main();

            // Assert
            applicationMock.Verify(app => app.Initialize(), Times.Once);
        }

        [Fact]
        public void Main_ShouldRunMainForm()
        {
            // Arrange
            var applicationMock = new Mock<IApplication>();
            ApplicationWrapper.SetApplication(applicationMock.Object);

            // Act
            Program.Main();

            // Assert
            applicationMock.Verify(app => app.Run(It.IsAny<MainForm>()), Times.Once);
        }
    }
}
