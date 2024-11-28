using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WordleWinForms.Enums;

namespace WordleWinForms
{
    internal class Surface
    {
        const int BASE_H = 1080;
        const int SCREEN_WIDTH = 500;
        const int SCREEN_HEIGHT = 782;

        private readonly Graphics _graphics;
        private readonly double _scale = Screen.PrimaryScreen?.WorkingArea.Height / BASE_H ?? 1.0;

        public Surface(Graphics graphics)
        {
            _graphics = graphics;
        }

        public static Color GetLetterColor(Dictionary<char, Status> letters, char letter)
        {
            if (letters[letter] == Status.Correct)
                return Color.Green;
            if (letters[letter] == Status.Incorrect)
                return Color.Red;
            if (letters[letter] == Status.WrongPlace)
                return Color.Yellow;
            if (letters[letter] == Status.NotTested)
                return Color.White;
            else
                return Color.Gray;
        }

        public void DrawSurface(Square[][] grid, Banner banner, Dictionary<char, Status> letters)
        {
            int bannerStart = DrawSquares(grid);
            DrawBanner(banner, bannerStart);
            DrawKeyboard(bannerStart + (int)(24 * _scale), letters);
        }

        public int DrawSquares(Square[][] grid)
        {
            int size = (int)(88 * _scale);
            int screenEdge = (int)(10 * _scale);
            int hMargin = (int)(10 * _scale);
            int vMargin = (int)(10 * _scale);
            int fontSize = (int)(36 * _scale);

            int end_y = 0;

            Font font = new("Trebuchet MS", fontSize);
            SolidBrush brush = new(Color.Gray);
            Pen pen = new(brush);
            StringFormat format = new();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;

            for (int row = 0; row < 6; row++)
            {
                for (int column = 0; column < 5; column++)
                {
                    Square square = grid[row][column];

                    if (square.Status == Status.NotTested)
                    {
                        brush.Color = Color.White;
                        pen.Color = Color.White;
                    }
                    else if (square.Status == Status.Incorrect)
                    {
                        brush.Color = Color.Red;
                        pen.Color = Color.Red;
                    }
                    else if (square.Status == Status.Correct)
                    {
                        brush.Color = Color.Green;
                        pen.Color = Color.Green;
                    }
                    else if (square.Status == Status.WrongPlace)
                    {
                        brush.Color = Color.Yellow;
                        pen.Color = Color.Yellow;
                    }
                    else
                    {
                        brush.Color = Color.Gray;
                        pen.Color = Color.Gray;
                    }
                    
                    Rectangle rect = new(column * (size + hMargin) + screenEdge, row * (size + vMargin) + screenEdge, size, size);
                    _graphics.DrawRectangle(pen, rect);
                    _graphics.DrawString(square.Letter.ToString(), font, brush, rect.Left + rect.Width / 2, rect.Top + rect.Height / 2, format);

                    end_y = rect.Bottom + vMargin;
                }
            }

            format.Dispose();
            pen.Dispose();
            brush.Dispose();
            font.Dispose();

            return end_y;
        }

        public void DrawBanner(Banner banner, int startY)
        {
            Font font = new("Lucida Console", (int)(14 * _scale));
            SolidBrush brush = new(Color.White);
            StringFormat format = new()
            {
                Alignment = StringAlignment.Center
            };
            _graphics.DrawString(banner.Caption, font, brush, SCREEN_WIDTH / 2, startY, format);
            font.Dispose();
            brush.Dispose();
        }

        public void DrawKeyboard(int startY, Dictionary<char, Status> letters)
        {
            int size = (int)(42 * _scale);
            int fontSize = (int)(16 * _scale);
            int hScreenEdge = (int)(8 * _scale);
            int hMargin = (int)(7 * _scale);
            int vMargin = (int)(7 * _scale);
            int rowMargin = (int)(8 * _scale);

            startY += vMargin;
            List<string> rows = new() { "QWERTYUIOP", "ASDFGHJKL", "ZXCVBNM" };

            Font font = new("Trebuchet MS", fontSize);
            SolidBrush brush = new(Color.Gray);
            Pen pen = new(brush);
            StringFormat format = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            for (int row = 0; row < 3; row++)
            {
                int column = 0;
                foreach (char letter in rows[row])
                {
                    DrawCharacter(row, column++, letter);
                }
            }

            void DrawCharacter(int row, int column, char letter)
            {
                brush.Color = GetLetterColor(letters, letter);
                pen.Color = brush.Color;
                Rectangle rect = new(hScreenEdge + column * (size + hMargin) + row * rowMargin, startY + row * (size + vMargin), size, size);
                _graphics.DrawRectangle(pen, rect);
                _graphics.DrawString(letter.ToString(), font, brush, rect.Left + rect.Width / 2, rect.Top + rect.Height / 2, format);
            }

            format.Dispose();
            pen.Dispose();
            brush.Dispose();
            font.Dispose();
        }
    }
}
