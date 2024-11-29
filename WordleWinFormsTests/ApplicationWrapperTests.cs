using Moq;
using System.Windows.Forms;
using SystemWrapper.Forms;
using Xunit;

namespace WordleWinFormsTests
{
    public class ApplicationWrapperTests
    {
        [Fact]
        public void SetApplication_ShouldSetApplicationInstance()
        {
            // Arrange
            var applicationMock = new Mock<IApplication>();

            // Act
            ApplicationWrapper.SetApplication(applicationMock.Object);

            // Assert
            Assert.NotNull(ApplicationWrapper.GetApplication());
        }

        [Fact]
        public void Initialize_ShouldCallInitializeOnApplication()
        {
            // Arrange
            var applicationMock = new Mock<IApplication>();
            ApplicationWrapper.SetApplication(applicationMock.Object);

            // Act
            ApplicationWrapper.Initialize();

            // Assert
            applicationMock.Verify(app => app.Initialize(), Times.Once);
        }

        [Fact]
        public void Run_ShouldCallRunOnApplication()
        {
            // Arrange
            var applicationMock = new Mock<IApplication>();
            var formMock = new Mock<Form>();
            ApplicationWrapper.SetApplication(applicationMock.Object);

            // Act
            ApplicationWrapper.Run(formMock.Object);

            // Assert
            applicationMock.Verify(app => app.Run(formMock.Object), Times.Once);
        }
    }
}
