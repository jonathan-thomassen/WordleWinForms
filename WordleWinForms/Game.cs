using WordleWinForms.Enums;

namespace WordleWinForms;

internal class Game(Surface surface, Banner banner)
{
    const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const int Columns = 5;
    const int Rows = 6;

    private GameState _state;
    private Square[][] _grid = [];
    private string _word = "";
    private readonly Dictionary<char, Status> _letters = [];
    private int _activeRow;
    private int _activeColumn;

    public void Initialize()
    {
        _state = GameState.InGame;

        _grid = Enumerable.Range(0, Rows)
                          .Select(_ => Enumerable.Range(0, Columns)
                                                 .Select(_ => new Square())
                                                 .ToArray())
                          .ToArray();

        foreach (var square in _grid[0])
        {
            square.Status = Status.NotTested;
        }

        NewWord();
        InitializeLetters();
        _activeRow = 0;
        _activeColumn = 0;
        banner.Caption = "Welcome. Start typing to begin";

        surface.DrawSurface(_grid, banner, _letters);
    }

    private void InitializeLetters()
    {
        _letters.Clear();
        foreach (char letter in Alphabet)
        {
            _letters.Add(letter, Status.NotTested);
        }
    }

    public void NewWord()
    {
        Random random = new();
        int wordNumber = random.Next(WordList.Words.Count - 1);
        _word = WordList.Words[wordNumber];
    }

    public bool CheckDictionary(string guess)
    {
        return WordList.Words.Contains(guess.ToLower());
    }

    public ValidationState ValidateGuess(string guess)
    {
        return CheckDictionary(guess) ? ValidationState.Valid : ValidationState.NotInDictionary;
    }

    public (List<Status>, int) TestGuess(string guess)
    {
        guess = guess.ToLower();
        List<Status> statusList = Enumerable.Repeat(Status.NotTested, 5).ToList();
        int correctLetters = 0;
        string tempWord = _word;

        // First pass: Check for correct letters
        for (int i = 0; i < 5; i++)
        {
            if (guess[i] == _word[i])
            {
                statusList[i] = Status.Correct;
                _letters[guess.ToUpper()[i]] = Status.Correct;
                correctLetters++;
                tempWord = tempWord.Remove(tempWord.IndexOf(_word[i]), 1);
            }
        }

        // Second pass: Check for wrong place and incorrect letters
        for (int i = 0; i < 5; i++)
        {
            if (statusList[i] == Status.Correct) continue;

            if (tempWord.Contains(guess[i]))
            {
                statusList[i] = Status.WrongPlace;
                if (_letters[guess.ToUpper()[i]] != Status.Correct)
                    _letters[guess.ToUpper()[i]] = Status.WrongPlace;
                tempWord = tempWord.Remove(tempWord.IndexOf(guess[i]), 1);
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

    public void HandleEvent(KeyEventArgs e)
    {
        if (_state == GameState.OutOfGame)
        {
            Initialize();
            surface.DrawSurface(_grid, banner, _letters);
        }
        else
        {
            HandleKeyPress(e);
        }
    }

    private void HandleKeyPress(KeyEventArgs e)
    {
        if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z && _grid[_activeRow][_activeColumn].Letter == ' ')
        {
            _grid[_activeRow][_activeColumn].Letter = (char)e.KeyData;
            if (_activeColumn <= 3)
                _activeColumn += 1;
            banner.Caption = "Guess #" + (_activeRow + 1);
            surface.DrawSurface(_grid, banner, _letters);
        }
        else if (e.KeyCode == Keys.Back)
        {
            HandleBackspace();
        }
        else if (e.KeyCode == Keys.Enter && _grid[_activeRow][_activeColumn].Letter != ' ')
        {
            HandleEnter();
        }
    }

    private void HandleBackspace()
    {
        if (_activeColumn >= 1 && _grid[_activeRow][_activeColumn].Letter == ' ')
            _activeColumn -= 1;
        _grid[_activeRow][_activeColumn].Letter = ' ';
        banner.Caption = "Guess #" + (_activeRow + 1);
        surface.DrawSurface(_grid, banner, _letters);
    }

    private void HandleEnter()
    {
        string guess = new string(_grid[_activeRow].Select(square => square.Letter).ToArray());
        ValidationState validationState = ValidateGuess(guess);
        if (validationState == ValidationState.Valid)
        {
            ProcessValidGuess(guess);
        }
        else if (validationState == ValidationState.NotInDictionary)
        {
            ResetCurrentRow();
            banner.Caption = "Word not in dictionary. Try again";
        }
        surface.DrawSurface(_grid, banner, _letters);
    }

    private void ProcessValidGuess(string guess)
    {
        (List<Status> statuses, int correctLetters) = TestGuess(guess);
        for (int i = 0; i < _grid[_activeRow].Length; i++)
        {
            _grid[_activeRow][i].Status = statuses[i];
        }
        if (correctLetters == 5)
        {
            banner.Caption = "Correct! You win!";
            _state = GameState.OutOfGame;
        }
        else if (_activeRow <= 4)
        {
            _activeColumn = 0;
            _activeRow++;
            foreach (Square square in _grid[_activeRow])
            {
                square.Status = Status.NotTested;
            }
            banner.Caption = "Guess #" + (_activeRow + 1);
        }
        else
        {
            banner.Caption = "You lost! The word was: " + _word.ToUpper();
            _state = GameState.OutOfGame;
        }
    }

    private void ResetCurrentRow()
    {
        foreach (Square square in _grid[_activeRow])
        {
            square.Letter = ' ';
            square.Status = Status.NotTested;
        }
        _activeColumn = 0;
    }
}
