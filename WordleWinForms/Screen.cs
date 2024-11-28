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
using static System.Windows.Forms.AxHost;

namespace WordleWinForms
{
    internal class Screen
    {
        private int _desktopH;
        private double _scale = 1.0;
        private nint _window;
        private nint _renderer;
        private Graphics _g;

        const int SCREEN_WIDTH = 500;
        const int SCREEN_HEIGHT = 782;
        const string BG_COLOR = "black";
        const string INACTIVE_COLOR = "grey";
        const string NOT_TESTED_COLOR = "white";
        const string INCORRECT_COLOR = "red";
        const string WRONG_PLACE_COLOR = "yellow";
        const string CORRECT_COLOR = "green";


        public Screen(Graphics g)
        {
            _g = g;
        }


        public string GetLetterColor(Dictionary<char, Status> letters, char letter)
        {
            if (letters[letter] == Status.Correct)
                return CORRECT_COLOR;
            if (letters[letter] == Status.Incorrect)
                return INCORRECT_COLOR;
            if (letters[letter] == Status.WrongPlace)
                return WRONG_PLACE_COLOR;
            if (letters[letter] == Status.NotTested)
                return NOT_TESTED_COLOR;
            else
                return INACTIVE_COLOR;
        }

        public void DrawScreen(List<List<Square>> grid, Banner banner, Dictionary<char, Status> letters)
        {
            int exitCode;

            //Fill the surface black
            SolidBrush myBrush = new(Color.Black);
            _g.FillRectangle(myBrush, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT));
            myBrush.Dispose();

            int bannerStart = DrawSquares(grid);
            DrawBanner(banner, bannerStart);
            DrawKeyboard(bannerStart + (int)(24 * _scale), letters);
        }

        //public bool InitSdl()
        //{
        //    //Initialization flag
        //    bool success = true;

        //    //Initialize SDL
        //    if (SDL_Init(SDL_INIT_VIDEO) < 0)
        //    {
        //        SDL_Log($"SDL could not initialize! SDL error: {SDL_GetError()}");
        //        success = false;
        //    }
        //    else
        //    {
        //        //Create window
        //        _window = SDL_CreateWindow("SDL2 Window", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, K_SCREEN_WIDTH, K_SCREEN_HEIGHT, SDL_WindowFlags.SDL_WINDOW_SHOWN);
        //        if (_window == nint.Zero)
        //        {
        //            SDL_Log($"Window could not be created! SDL error: {SDL_GetError()}");
        //            success = false;
        //        }
        //        else
        //        {
        //            //Get window surface
        //            _renderer = SDL_CreateRenderer(_window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
        //            if (_renderer == nint.Zero)
        //            {
        //                SDL_Log($"Rendering surface could not be created! SDL error: {SDL_GetError()}");
        //                success = false;
        //            }
        //        }
        //    }
        //    return success;
        //}

        public int DrawSquares(List<List<Square>> grid)
        {
            int size = (int)(88 * _scale);
            int screenEdge = (int)(10 * _scale);
            int hMargin = (int)(10 * _scale);
            int vMargin = (int)(10 * _scale);
            int fontSize = (int)(36 * _scale);

            int end_y = 0;

            for (int row = 0; row < 6; row++)
            {
                for (int column = 0; column < 5; column++)
                {
                    Square square = grid[row][column];
                    string textColor = INACTIVE_COLOR;

                    if (square.Status == Status.NotTested)
                        textColor = NOT_TESTED_COLOR;
                    else if (square.Status == Status.Incorrect)
                        textColor = INCORRECT_COLOR;
                    else if (square.Status == Status.Correct)
                        textColor = CORRECT_COLOR;
                    else if (square.Status == Status.WrongPlace)
                        textColor = WRONG_PLACE_COLOR;

                    Font font = new("OCR-A Extended", fontSize);
                    SolidBrush brush = new(Color.White);
                    Pen pen = new(brush);
                    StringFormat format = new();
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    Rectangle rect = new(column * (size + hMargin) + screenEdge, row * (size + vMargin) + screenEdge, size, size);
                    _g.DrawRectangle(pen, rect);
                    _g.DrawString(square.Letter.ToString(), font, brush, rect.Left + rect.Width / 2, rect.Top + rect.Height / 2, format);

                    format.Dispose();
                    pen.Dispose();
                    brush.Dispose();
                    font.Dispose();

                    end_y = rect.Bottom + vMargin;
                }
            }

            return end_y;
        }

        public void DrawBanner(Banner banner, int startY)
        {
            Font font = new("Lucida Console", (int)(14 * _scale));
            SolidBrush brush = new(Color.White);
            StringFormat format = new();
            format.Alignment = StringAlignment.Center;
            _g.DrawString(banner.Caption, font, brush, SCREEN_WIDTH / 2, startY, format);
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
            List<string> rows = ["QWERTYUIOP", "ASDFGHJKL", "ZXCVBNM"];

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
                string fgColor = GetLetterColor(letters, letter);
                Font font = new("Trebuchet MS", fontSize);
                SolidBrush brush = new(Color.White);
                Pen pen = new(brush);
                StringFormat format = new();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                Rectangle rect = new(hScreenEdge + column * (size + hMargin) + row * rowMargin, startY + row * (size + vMargin), size, size);
                _g.DrawRectangle(pen, rect);
                _g.DrawString(letter.ToString(), font, brush, rect.Left + rect.Width / 2, rect.Top + rect.Height / 2, format);

                format.Dispose();
                pen.Dispose();
                brush.Dispose();
                font.Dispose();
            }
        }
    }
}
