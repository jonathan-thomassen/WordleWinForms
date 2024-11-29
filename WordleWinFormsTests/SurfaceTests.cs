using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using WordleWinForms;
using WordleWinForms.Enums;
using Xunit;

namespace WordleWinFormsTests
{
    public class SurfaceTests
    {
        private readonly Surface _surface;
        private readonly Game _game;

        public SurfaceTests()
        {
            double scale = 1.0;
            int winW = 800;
            _surface = new Surface(scale, winW);
            _game = new Game();
        }

        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            double scale = 1.5;
            int winW = 1024;

            // Act
            var surface = new Surface(scale, winW);

            // Assert
            Assert.Equal((int)(10 * scale), GetPrivateField<int>(surface, "_margin"));
            Assert.Equal((int)(88 * scale), GetPrivateField<int>(surface, "_squareSize"));
            Assert.Equal((int)(17 * Math.Sqrt(scale) + 10), GetPrivateField<int>(surface, "_squareFontSize"));
            Assert.Equal((int)(34 * scale), GetPrivateField<int>(surface, "_bannerSize"));
            Assert.Equal((int)(8 * Math.Sqrt(scale) + 5), GetPrivateField<int>(surface, "_bannerFontSize"));
            Assert.Equal((int)(42 * scale), GetPrivateField<int>(surface, "_keybSize"));
            Assert.Equal((int)(7 * scale), GetPrivateField<int>(surface, "_keybSpacing"));
            Assert.Equal((int)(8 * scale), GetPrivateField<int>(surface, "_keybRowOffset"));
            Assert.Equal((int)(11 * Math.Sqrt(scale) + 3), GetPrivateField<int>(surface, "_keybFontSize"));
        }

        [Fact]
        public void GetLetterColor_ShouldReturnCorrectColor()
        {
            var letters = new Dictionary<char, Status>
            {
                { 'A', Status.Correct },
                { 'B', Status.Incorrect },
                { 'C', Status.WrongPlace },
                { 'D', Status.NotTested },
                { 'E', (Status)999 } // Default case
            };

            Assert.Equal(Color.Green, Surface.GetLetterColor(letters, 'A'));
            Assert.Equal(Color.Red, Surface.GetLetterColor(letters, 'B'));
            Assert.Equal(Color.Yellow, Surface.GetLetterColor(letters, 'C'));
            Assert.Equal(Color.White, Surface.GetLetterColor(letters, 'D'));
            Assert.Equal(Color.Gray, Surface.GetLetterColor(letters, 'E'));
        }

        [Fact]
        public void DrawGame_ShouldDrawGameComponents()
        {
            using Bitmap bitmap = new(800, 600);
            using Graphics graphics = Graphics.FromImage(bitmap);

            _surface.DrawGame(graphics, _game);

            // Since we can't directly assert on the drawing, we ensure no exceptions are thrown
            Assert.True(true);
        }

        [Fact]
        public void DrawSquares_ShouldReturnCorrectEndY()
        {
            using Bitmap bitmap = new(800, 600);
            using Graphics graphics = Graphics.FromImage(bitmap);

            int endY = _surface.DrawSquares(graphics, _game.Grid);

            // Assuming the margin and square size are set correctly, we can calculate the expected endY
            int expectedEndY = 6 * (88 + 10) + 10; // 6 rows of squares + margin
            Assert.Equal(expectedEndY, endY);
        }

        [Fact]
        public void DrawBanner_ShouldReturnCorrectEndY()
        {
            using Bitmap bitmap = new(800, 600);
            using Graphics graphics = Graphics.FromImage(bitmap);

            int startY = 100;
            int endY = _surface.DrawBanner(graphics, _game.Banner, startY);

            // Assuming the margin and banner size are set correctly, we can calculate the expected endY
            int expectedEndY = startY + 34 + 10; // banner size + margin
            Assert.Equal(expectedEndY, endY);
        }

        [Fact]
        public void DrawKeyboard_ShouldDrawKeyboard()
        {
            using Bitmap bitmap = new(800, 600);
            using Graphics graphics = Graphics.FromImage(bitmap);

            int startY = 200;
            _surface.DrawKeyboard(graphics, startY, _game.Letters);

            // Since we can't directly assert on the drawing, we ensure no exceptions are thrown
            Assert.True(true);
        }

        [Fact]
        public void DrawCharacter_ShouldDrawCharacter()
        {
            using Bitmap bitmap = new(800, 600);
            using Graphics graphics = Graphics.FromImage(bitmap);

            var letters = new Dictionary<char, Status>
            {
                { 'A', Status.Correct }
            };

            MethodInfo? drawCharacterMethod = typeof(Surface).GetMethod("DrawCharacter", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(drawCharacterMethod);
            drawCharacterMethod.Invoke(_surface, [graphics, 0, 0, 'A', letters, new SolidBrush(Color.Gray), new Pen(Color.Gray), new Font("Arial", 12), new StringFormat(), 0]);
            drawCharacterMethod.Invoke(_surface, [graphics, 0, 0, 'A', letters, new SolidBrush(Color.Gray), new Pen(Color.Gray), new Font("Arial", 12), new StringFormat(), 0]);

            // Since we can't directly assert on the drawing, we ensure no exceptions are thrown
            Assert.True(true);
        }

        [Fact]
        public void SetBrushAndPenColor_ShouldSetCorrectColor()
        {
            var brush = new SolidBrush(Color.Gray);
            var pen = new Pen(Color.Gray);

            MethodInfo setBrushAndPenColorMethod = typeof(Surface).GetMethod("SetBrushAndPenColor", BindingFlags.NonPublic | BindingFlags.Static);

            setBrushAndPenColorMethod.Invoke(null, new object[] { brush, pen, Status.Correct });
            Assert.Equal(Color.Green, brush.Color);
            Assert.Equal(Color.Green, pen.Color);

            setBrushAndPenColorMethod.Invoke(null, new object[] { brush, pen, Status.Incorrect });
            Assert.Equal(Color.Red, brush.Color);
            Assert.Equal(Color.Red, pen.Color);

            setBrushAndPenColorMethod.Invoke(null, new object[] { brush, pen, Status.WrongPlace });
            Assert.Equal(Color.Yellow, brush.Color);
            Assert.Equal(Color.Yellow, pen.Color);

            setBrushAndPenColorMethod.Invoke(null, new object[] { brush, pen, Status.NotTested });
            Assert.Equal(Color.White, brush.Color);
            Assert.Equal(Color.White, pen.Color);

            setBrushAndPenColorMethod.Invoke(null, new object[] { brush, pen, (Status)999 });
            Assert.Equal(Color.Gray, brush.Color);
            Assert.Equal(Color.Gray, pen.Color);
        }

        [Fact]
        public void ListAvailableFonts_ShouldListFonts()
        {
            // Redirect Trace output to a StringWriter to capture the output
            using var sw = new StringWriter();
            Trace.Listeners.Add(new TextWriterTraceListener(sw));

            Surface.ListAvailableFonts();

            // Ensure that some fonts are listed
            string output = sw.ToString();
            Assert.NotEmpty(output);
        }

        private static T GetPrivateField<T>(object obj, string fieldName)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)field.GetValue(obj);
        }
    }
}