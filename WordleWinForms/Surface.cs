using System.Diagnostics;
using System.Drawing.Text;

namespace WordleWinForms;

internal class Surface
{
    private const int MARGIN_BASE = 10;
    private const int SQUARE_SIZE_BASE = 88;
    private const string SQUARE_FONT = "OCR A Extended";
    private const int SQUARE_FONT_SIZE_BASE = 17;
    private const int SQUARE_FONT_SIZE_OFFSET = 10;
    private const int BANNER_SIZE_BASE = 34;
    private const string BANNER_FONT = "Verdana";
    private const int BANNER_FONT_SIZE_BASE = 8;
    private const int BANNER_FONT_SIZE_OFFSET = 5;
    private const string KEYB_ROW_1 = "QWERTYUIOP";
    private const string KEYB_ROW_2 = "ASDFGHJKL";
    private const string KEYB_ROW_3 = "ZXCVBNM";
    private const int KEYB_SIZE_BASE = 42;
    private const int KEYB_SPACING_BASE = 7;
    private const int KEYB_ROW_OFFSET_BASE = 8;
    private const string KEYB_FONT = "Trebuchet MS";
    private const int KEYB_FONT_SIZE_BASE = 11;
    private const int KEYB_FONT_SIZE_OFFSET = 3;

    private readonly int _winW;
    private readonly int _margin;
    private readonly int _squareSize;
    private readonly int _squareFontSize;
    private readonly int _bannerSize;
    private readonly int _bannerFontSize;
    private readonly string[] _keybRows;
    private readonly int _keybSize;
    private readonly int _keybSpacing;
    private readonly int _keybRowOffset;
    private readonly int _keybFontSize;

    private static readonly Color CorrectColor = Color.Green;
    private static readonly Color IncorrectColor = Color.Red;
    private static readonly Color WrongPlaceColor = Color.Yellow;
    private static readonly Color NotTestedColor = Color.White;
    private static readonly Color DefaultColor = Color.Gray;

    public Surface(double scale, int winW)
    {
        _winW = winW;
        _margin = (int)(MARGIN_BASE * scale);
        _squareSize = (int)(SQUARE_SIZE_BASE * scale);
        _squareFontSize = (int)(SQUARE_FONT_SIZE_BASE * Math.Sqrt(scale) + SQUARE_FONT_SIZE_OFFSET);
        _bannerSize = (int)(BANNER_SIZE_BASE * scale);
        _bannerFontSize = (int)(BANNER_FONT_SIZE_BASE * Math.Sqrt(scale) + BANNER_FONT_SIZE_OFFSET);
        _keybRows = [KEYB_ROW_1, KEYB_ROW_2, KEYB_ROW_3];
        _keybSize = (int)(KEYB_SIZE_BASE * scale);
        _keybSpacing = (int)(KEYB_SPACING_BASE * scale);
        _keybRowOffset = (int)(KEYB_ROW_OFFSET_BASE * scale);
        _keybFontSize = (int)(KEYB_FONT_SIZE_BASE * Math.Sqrt(scale) + KEYB_FONT_SIZE_OFFSET);
    }

    /// <summary>
    /// Gets the color for a letter based on its status.
    /// </summary>
    /// <param name="letters">The dictionary of letters and their statuses.</param>
    /// <param name="letter">The letter to get the color for.</param>
    /// <returns>The color corresponding to the letter's status.</returns>
    public Color GetLetterColor(Dictionary<char, Status> letters, char letter)
    {
        return letters[letter] switch
        {
            Status.Correct => CorrectColor,
            Status.Incorrect => IncorrectColor,
            Status.WrongPlace => WrongPlaceColor,
            Status.NotTested => NotTestedColor,
            _ => DefaultColor,
        };
    }

    /// <summary>
    /// Draws the game components on the surface.
    /// </summary>
    /// <param name="graphics">The graphics object to draw on.</param>
    /// <param name="game">The game object containing the game state.</param>
    public void DrawGame(Graphics graphics, Game game)
    {
        int bannerStart = DrawSquares(graphics, game.Grid);
        int keyboardStart = DrawBanner(graphics, game.Banner, bannerStart);
        DrawKeyboard(graphics, keyboardStart, game.Letters);
    }

    /// <summary>
    /// Draws the squares of the game grid.
    /// </summary>
    /// <param name="graphics">The graphics object to draw on.</param>
    /// <param name="grid">The game grid.</param>
    /// <returns>The y-coordinate of the bottom of the last row of squares.</returns>
    public int DrawSquares(Graphics graphics, Square[][] grid)
    {
        using Font font = new(SQUARE_FONT, _squareFontSize);
        using SolidBrush brush = new(DefaultColor);
        using Pen pen = new(brush);
        using StringFormat format = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        int end_y = 0;

        for (int row = 0; row < 6; row++)
        {
            for (int column = 0; column < 5; column++)
            {
                Square square = grid[row][column];
                SetBrushAndPenColor(brush, pen, square.Status);

                Rectangle rect = new(column * (_squareSize + _margin) + _margin, row * (_squareSize + _margin) + _margin, _squareSize, _squareSize);
                graphics.DrawRectangle(pen, rect);
                graphics.DrawString(square.Letter.ToString(), font, brush, rect.Left + rect.Width / 2, rect.Top + rect.Height / 2, format);

                end_y = rect.Bottom + _margin;
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
    public int DrawBanner(Graphics graphics, Banner banner, int startY)
    {
        using Font font = new(BANNER_FONT, _bannerFontSize);
        using SolidBrush brush = new(NotTestedColor);
        using Pen pen = new(brush);
        using StringFormat format = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        Rectangle rect = new(_margin, startY, _winW - 2 * _margin, _bannerSize);
        graphics.DrawRectangle(pen, rect);
        graphics.DrawString(banner.Caption, font, brush, rect.Left + rect.Width / 2, rect.Top + rect.Height / 2, format);

        return rect.Bottom + _margin;
    }

    /// <summary>
    /// Draws the keyboard.
    /// </summary>
    /// <param name="graphics">The graphics object to draw on.</param>
    /// <param name="startY">The y-coordinate to start drawing the keyboard.</param>
    /// <param name="letters">The dictionary of letters and their statuses.</param>
    public void DrawKeyboard(Graphics graphics, int startY, Dictionary<char, Status> letters)
    {
        using Font font = new(KEYB_FONT, _keybFontSize);
        using SolidBrush brush = new(DefaultColor);
        using Pen pen = new(brush);
        using StringFormat format = new()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        int rowNo = 0;
        foreach (string row in _keybRows)
        {
            int columnNo = 0;
            foreach (char letter in row)
            {
                DrawCharacter(graphics, rowNo, columnNo++, letter, letters, brush, pen, font, format, startY);
            }
            rowNo++;
        }
    }

    private void DrawCharacter(Graphics graphics, int row, int column, char letter, Dictionary<char, Status> letters, SolidBrush brush, Pen pen, Font font, StringFormat format, int startY)
    {
        brush.Color = GetLetterColor(letters, letter);
        pen.Color = brush.Color;
        Rectangle rect = new(_margin + column * (_keybSize + _keybSpacing) + row * _keybRowOffset, startY + row * (_keybSize + _keybSpacing), _keybSize, _keybSize);
        graphics.DrawRectangle(pen, rect);
        graphics.DrawString(letter.ToString(), font, brush, rect.Left + rect.Width / 2, rect.Top + rect.Height / 2, format);
    }

    /// <summary>
    /// Sets the color of the brush and pen based on the status.
    /// </summary>
    /// <param name="brush">The brush to set the color for.</param>
    /// <param name="pen">The pen to set the color for.</param>
    /// <param name="status">The status to determine the color.</param>
    private static void SetBrushAndPenColor(SolidBrush brush, Pen pen, Status status)
    {
        brush.Color = status switch
        {
            Status.Correct => Color.Green,
            Status.Incorrect => Color.Red,
            Status.WrongPlace => Color.Yellow,
            Status.NotTested => Color.White,
            _ => Color.Gray,
        };
        pen.Color = brush.Color;
    }

    public static void ListAvailableFonts()
    {
        using InstalledFontCollection fontsCollection = new();
        FontFamily[] fontFamilies = fontsCollection.Families;
        foreach (FontFamily font in fontFamilies)
        {
            Debug.WriteLine(font.Name);
        }
    }
}
