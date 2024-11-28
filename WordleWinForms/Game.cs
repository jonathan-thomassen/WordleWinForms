using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using WordleWinForms.Enums;

namespace WordleWinForms
{
    internal class Game
    {
        const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private GameState _state;
        private List<List<Square>> _grid;
        private List<string> _wordList;
        private string _word;
        private Dictionary<char, Status> _letters;
        private int _activeRow;
        private int _activeColumn;
        private Banner _banner;
        private Screen _screen;

        public Screen screen
        {
            get { return _screen; }
            set { _screen = value; }
        }

        public Banner banner
        {
            get { return _banner; }
            set { _banner = value; }
        }

        public Game(Screen screen, Banner banner)
        {
            _screen = screen;
            _banner = banner;
        }

        public void Update()
        {
            _screen.DrawScreen(_grid, _banner, _letters);

            //bool running = true;
            //while (running)
            //{
            //    //while (SDL_PollEvent(out SDL_Event e) == 1)
            //    //{
            //    //    switch (e.type)
            //    //    {
            //    //        case SDL_EventType.SDL_QUIT:
            //    //            running = false;
            //    //            break;
            //    //        case SDL_EventType.SDL_KEYDOWN:
            //    //            HandleEvent(e);
            //    //            break;
            //    //    }
            //    //}
            //}

            //clock.tick(60);
        }

        public void Initialize()
        {
            _state = GameState.InGame;
            _grid = Enumerable.Repeat(Enumerable.Repeat(new Square(), 5).ToList(), 6).ToList();
            _wordList = WordList.Words;
            for (int i = 0; i < _grid[0].Count; i++)
            {
                _grid[0][i].Status = Status.NotTested;
            }
            NewWord();
            _letters = [];
            foreach (char letter in ALPHABET)
            {
                _letters.Add(letter, Status.NotTested);
            }
            _activeRow = 0;
            _activeColumn = 0;
            _banner.Caption = "Welcome. Start typing to begin";

            _screen.DrawScreen(_grid, _banner, _letters);
        }

        public void NewWord()
        {
            Random random = new();
            int wordNumber = random.Next(WordList.Words.Count - 1);
            _word = WordList.Words[wordNumber];
        }

        public bool CheckDictionary(string guess)
        {
            if (_wordList.Contains(guess.ToLower()))
                return true;
            return false;
        }

        public ValidationState ValidateGuess(string guess)
        {
            if (!CheckDictionary(guess))
                return ValidationState.NotInDictionary;
            return ValidationState.Valid;
        }

        public (List<Status>, int) TestGuess(string guess)
        {
            guess = guess.ToLower();
            List<Status> statusList = Enumerable.Repeat(Status.NotTested, 5).ToList();
            int correctLetters = 0;
            string tempWord = _word;

            for (int i = 0; i < 5; i++)
            {
                if (guess[i] == _word[i])
                {
                    statusList[i] = Status.Correct;
                    _letters[guess.ToUpper()[i]] = Status.Correct;
                    correctLetters++;
                    Regex regex = new(Regex.Escape(_word[i].ToString()));
                    tempWord = regex.Replace(tempWord, "", 1);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                if (_word.Count(l => l == guess[i]) > 0)
                {
                    if (statusList[i] != Status.Correct)
                    {
                        if (tempWord.Contains(guess[i]))
                        {
                            statusList[i] = Status.WrongPlace;
                            if (_letters[guess.ToUpper()[i]] != Status.Correct)
                                _letters[guess.ToUpper()[i]] = Status.WrongPlace;
                            Regex regex = new(Regex.Escape(_word[i].ToString()));
                            tempWord = regex.Replace(tempWord, "", 1);
                        }
                        else
                        {
                            statusList[i] = Status.Incorrect;
                            if (_letters[guess.ToUpper()[i]] != Status.Correct && _letters[guess.ToUpper()[i]] != Status.WrongPlace)
                                _letters[guess.ToUpper()[i]] = Status.Incorrect;
                        }
                    }
                }
                else
                {
                    statusList[i] = Status.Incorrect;
                    if (_letters[guess.ToUpper()[i]] != Status.Correct && _letters[guess.ToUpper()[i]] != Status.WrongPlace)
                        _letters[guess.ToUpper()[i]] = Status.Incorrect;
                }
            }

            return (statusList, correctLetters);
        }

        //public void HandleEvent(SDL_Event e)
        //{
        //    if (_state == GameState.OutOfGame)
        //    {
        //        Initialize();
        //        _screen.DrawScreen(_grid, _banner, _letters);
        //    }
        //    else
        //    {
        //        //Check if key pressed is in English alphabet:
        //        if ((int)e.key.keysym.scancode >= 4 && (int)e.key.keysym.scancode <= 29 && _grid[_activeRow][_activeColumn].Letter == ' ')
        //        {
        //            _grid[_activeRow][_activeColumn].Letter = ALPHABET[(int)e.key.keysym.scancode - 4];
        //            if (_activeColumn <= 3)
        //                _activeColumn += 1;
        //            _banner.Caption = "Guess #" + (_activeRow + 1);
        //            _screen.DrawScreen(_grid, _banner, _letters);
        //        }
        //        // Check for backspace:
        //        else if (e.key.keysym.sym == SDL_Keycode.SDLK_BACKSPACE)
        //        {
        //            if (_activeColumn >= 1 && _grid[_activeRow][_activeColumn].Letter == ' ')
        //                _activeColumn -= 1;
        //            _grid[_activeRow][_activeColumn].Letter = ' ';
        //            _banner.Caption = "Guess #" + (_activeRow + 1);
        //            _screen.DrawScreen(_grid, _banner, _letters);
        //        }
        //        // Check for enter:
        //        else if (e.key.keysym.sym == SDL_Keycode.SDLK_RETURN && _grid[_activeRow][_activeColumn].Letter != ' ')
        //        {
        //            string guess = "";
        //            foreach (Square square in _grid[_activeRow])
        //            {
        //                guess += square.Letter;
        //            }
        //            ValidationState validationState = ValidateGuess(guess);
        //            if (validationState == ValidationState.Valid)
        //            {
        //                (List<Status> statuses, int correctLetters) = TestGuess(guess);
        //                int i = 0;
        //                foreach (Square square in _grid[_activeRow])
        //                {
        //                    square.Status = statuses[i];
        //                    i++;
        //                }
        //                if (correctLetters == 5)
        //                {
        //                    _banner.Caption = "Correct! You win!";
        //                    _state = GameState.OutOfGame;
        //                }
        //                else if (_activeRow <= 4)
        //                {
        //                    _activeColumn = 0;
        //                    _activeRow++;
        //                    foreach (Square square in _grid[_activeRow])
        //                    {
        //                        square.Status = Status.NotTested;
        //                    }
        //                    _banner.Caption = "Guess #" + (_activeRow + 1);
        //                }
        //                else
        //                {
        //                    _banner.Caption = "You lost! The word was: " + _word.ToUpper();
        //                    _state = GameState.OutOfGame;
        //                }
        //            }
        //            else if (validationState == ValidationState.NotInDictionary)
        //            {
        //                foreach (Square square in _grid[_activeRow])
        //                {
        //                    square.Letter = ' ';
        //                    square.Status = Status.NotTested;
        //                }

        //                _activeColumn = 0;

        //                _banner.Caption = "Word not in dictionary. Try again";
        //            }
        //            _screen.DrawScreen(_grid, _banner, _letters);
        //        }
        //    }
        //}
    }
}
