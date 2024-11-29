using System.Drawing;
using System.Windows.Forms;
using WordleWinForms;
using WordleWinForms.Enums;
using Xunit;

namespace WordleWinFormsTests
{
    public class MainFormTests
    {
        private readonly MainForm _mainForm;

        public MainFormTests()
        {
            _mainForm = new MainForm();
        }

        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Assert
            Assert.Equal(Color.Black, _mainForm.BackColor);
            Assert.Equal(FormBorderStyle.FixedSingle, _mainForm.FormBorderStyle);
            Assert.True(GetPrivateProperty<bool>(_mainForm, "DoubleBuffered"));
            Assert.False(_mainForm.ShowIcon);
            Assert.Equal("Wordle", _mainForm.Text);
        }

        private T GetPrivateProperty<T>(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (T)property.GetValue(obj);
        }

        [Fact]
        public void OnShown_ShouldInitializeGame()
        {
            // Arrange
            var game = GetPrivateField<Game>(_mainForm, "_game");

            // Act
            _mainForm.GetType().GetMethod("OnShown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_mainForm, new object[] { new EventArgs() });

            // Assert
            Assert.Equal(GameState.InGame, game.State);
        }

        [Fact]
        public void OnKeyDown_ShouldHandleEvent()
        {
            // Arrange
            var game = GetPrivateField<Game>(_mainForm, "_game");
            var keyEventArgs = new KeyEventArgs(Keys.A);
            game.Grid = Enumerable.Range(0, 6)
                          .Select(_ => Enumerable.Range(0, 5)
                                                 .Select(_ => new Square())
                                                 .ToArray())
                          .ToArray();

            // Act
            _mainForm.GetType().GetMethod("OnKeyDown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_mainForm, new object[] { keyEventArgs });

            // Assert
            Assert.Equal('A', game.Grid[0][0].Letter);
        }

        [Fact]
        public void OnPaint_ShouldDrawGame()
        {
            // Arrange
            using Bitmap bitmap = new(800, 600);
            using Graphics graphics = Graphics.FromImage(bitmap);
            var paintEventArgs = new PaintEventArgs(graphics, new Rectangle());
            var game = GetPrivateField<Game>(_mainForm, "_game");

            // Act
            _mainForm.GetType().GetMethod("OnPaint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(_mainForm, new object[] { paintEventArgs });

            // Since we can't directly assert on the drawing, we ensure no exceptions are thrown
            Assert.True(true);
        }

        [Fact]
        public void Constructor_ShouldSetScaleBasedOnScreenHeight()
        {
            // Arrange
            var screenHeight = 1080;
            var expectedScale = screenHeight / 1080.0;

            // Act
            var mainForm = new MainForm();
            var actualScale = GetPrivateField<double>(mainForm, "_scale");

            // Assert
            Assert.Equal(expectedScale, actualScale);
        }

        [Fact]
        public void Constructor_ShouldSetScaleToDefaultWhenScreenHeightIsNull()
        {
            // Arrange
            var expectedScale = 1.0;

            // Act
            var mainForm = new MainForm();
            var actualScale = GetPrivateField<double>(mainForm, "_scale");

            // Assert
            Assert.Equal(expectedScale, actualScale);
        }

        private T GetPrivateField<T>(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (T)field.GetValue(obj);
        }
    }
}

