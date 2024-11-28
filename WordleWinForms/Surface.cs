using System.Diagnostics;
using System.Drawing.Text;

namespace WordleWinForms;

internal class Surface {
    private readonly double _scale;
    private readonly int _winW;

    public Surface(double scale, int winW) {
        _scale = scale;
        _winW = winW;
    }

    /// <summary>
    /// Gets the color for a letter based on its status.
    /// </summary>
    /// <param name="letters">The dictionary of letters and their statuses.</param>
    /// <param name="letter">The letter to get the color for.</param>
    /// <returns>The color corresponding to the letter's status.</returns>
    public static Color GetLetterColor(Dictionary<char, Status> letters, char letter) {
        return letters[letter] switch {
            Status.Correct => Color.Green,
            Status.Incorrect => Color.Red,
            Status.WrongPlace => Color.Yellow,
            Status.NotTested => Color.White,
            _ => Color.Gray,
        };
    }

    /// <summary>
    /// Draws the game components on the surface.
    /// </summary>
    /// <param name="graphics">The graphics object to draw on.</param>
    /// <param name="game">The game object containing the game state.</param>
    public void DrawGame(Graphics graphics, Game game) {
        int bannerStart = DrawSquares(graphics, game.Grid);
        DrawBanner(graphics, game.Banner, bannerStart);
        DrawKeyboard(graphics, bannerStart + (int)(24 * _scale), game.Letters);
    }

    /// <summary>
    /// Draws the squares of the game grid.
    /// </summary>
    /// <param name="graphics">The graphics object to draw on.</param>
    /// <param name="grid">The game grid.</param>
    /// <returns>The y-coordinate of the bottom of the last row of squares.</returns>
    public int DrawSquares(Graphics graphics, Square[][] grid) {
        int size = (int)(88 * _scale);
        int screenEdge = (int)(10 * _scale);
        int hMargin = (int)(10 * _scale);
        int vMargin = (int)(10 * _scale);
        int fontSize = (int)(17 * _scale);

        int end_y = 0;

        //ListAvailableFonts();

        using Font font = new("Trebuchet MS", fontSize);
        using SolidBrush brush = new(Color.Gray);
        using Pen pen = new(brush);
        using StringFormat format = new() {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        for (int row = 0; row < 6; row++) {
            for (int column = 0; column < 5; column++) {
                Square square = grid[row][column];
                SetBrushAndPenColor(brush, pen, square.Status);

                Rectangle rect = new(column * (size + hMargin) + screenEdge, row * (size + vMargin) + screenEdge, size, size);
                graphics.DrawRectangle(pen, rect);
                graphics.DrawString(square.Letter.ToString(), font, brush, rect.Left + rect.Width / 2, rect.Top + rect.Height / 2, format);

                end_y = rect.Bottom + vMargin;
            }
        }

        return end_y;
    }

    /// <summary>
    /// Draws the banner.
    /// </summary>
    /// <param name="graphics">The graphics object to draw on.</param>
    /// <param name="banner">The banner object containing the caption.</param>
    /// <param name="startY">The y-coordinate to start drawing the banner.</param>
    public void DrawBanner(Graphics graphics, Banner banner, int startY) {
        using Font font = new("Trebuchet MS", (int)(8 * _scale));
        using SolidBrush brush = new(Color.White);
        using StringFormat format = new() {
            Alignment = StringAlignment.Center
        };
        graphics.DrawString(banner.Caption, font, brush, _winW / 2, startY, format);
    }

    /// <summary>
    /// Draws the keyboard.
    /// </summary>
    /// <param name="graphics">The graphics object to draw on.</param>
    /// <param name="startY">The y-coordinate to start drawing the keyboard.</param>
    /// <param name="letters">The dictionary of letters and their statuses.</param>
    public void DrawKeyboard(Graphics graphics, int startY, Dictionary<char, Status> letters) {
        int size = (int)(42 * _scale);
        int fontSize = (int)(10 * _scale);
        int hScreenEdge = (int)(8 * _scale);
        int hMargin = (int)(7 * _scale);
        int vMargin = (int)(7 * _scale);
        int rowMargin = (int)(8 * _scale);

        startY += vMargin;
        List<string> rows = new() { "QWERTYUIOP", "ASDFGHJKL", "ZXCVBNM" };

        using Font font = new("Trebuchet MS", fontSize);
        using SolidBrush brush = new(Color.Gray);
        using Pen pen = new(brush);
        using StringFormat format = new() {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        for (int row = 0; row < 3; row++) {
            int column = 0;
            foreach (char letter in rows[row]) {
                DrawCharacter(graphics, row, column++, letter);
            }
        }

        void DrawCharacter(Graphics graphics, int row, int column, char letter) {
            brush.Color = GetLetterColor(letters, letter);
            pen.Color = brush.Color;
            Rectangle rect = new(hScreenEdge + column * (size + hMargin) + row * rowMargin, startY + row * (size + vMargin), size, size);
            graphics.DrawRectangle(pen, rect);
            graphics.DrawString(letter.ToString(), font, brush, rect.Left + rect.Width / 2, rect.Top + rect.Height / 2, format);
        }
    }

    /// <summary>
    /// Sets the color of the brush and pen based on the status.
    /// </summary>
    /// <param name="brush">The brush to set the color for.</param>
    /// <param name="pen">The pen to set the color for.</param>
    /// <param name="status">The status to determine the color.</param>
    private static void SetBrushAndPenColor(SolidBrush brush, Pen pen, Status status) {
        brush.Color = status switch {
            Status.Correct => Color.Green,
            Status.Incorrect => Color.Red,
            Status.WrongPlace => Color.Yellow,
            Status.NotTested => Color.White,
            _ => Color.Gray,
        };
        pen.Color = brush.Color;
    }

    public static void ListAvailableFonts() {
        using InstalledFontCollection fontsCollection = new();
        FontFamily[] fontFamilies = fontsCollection.Families;
        foreach (FontFamily font in fontFamilies) {
            Debug.WriteLine(font.Name);
        }
    }
}
