namespace WordleWinForms;

/// <summary>
/// Represents the Wordle game.
/// </summary>
internal class Game {
    private const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const int COLUMNS = 5;
    private const int ROWS = 6;

    private GameState _state;

    internal Square[][] Grid { get; private set; } = Array.Empty<Square[]>();
    internal Banner Banner { get; private set; } = new();

    private string _word = "";
    internal readonly Dictionary<char, Status> Letters = new();
    private int _activeRow;
    private int _activeColumn;

    /// <summary>
    /// Initializes the game.
    /// </summary>
    public void Initialize() {
        _state = GameState.InGame;

        Grid = Enumerable.Range(0, ROWS)
                          .Select(_ => Enumerable.Range(0, COLUMNS)
                                                 .Select(_ => new Square())
                                                 .ToArray())
                          .ToArray();

        foreach (var square in Grid[0]) {
            square.Status = Status.NotTested;
        }

        NewWord();
        InitializeLetters();
        _activeRow = 0;
        _activeColumn = 0;
        Banner.Caption = "Welcome!";
    }

    private void InitializeLetters() {
        Letters.Clear();
        foreach (char letter in ALPHABET) {
            Letters.Add(letter, Status.NotTested);
        }
    }

    /// <summary>
    /// Selects a new word for the game.
    /// </summary>
    public void NewWord() {
        Random random = new();
        int wordNumber = random.Next(WordList.Words.Count);
        _word = WordList.Words[wordNumber];
    }

    /// <summary>
    /// Checks if the guess is in the dictionary.
    /// </summary>
    /// <param name="guess">The guessed word.</param>
    /// <returns>True if the guess is in the dictionary, otherwise false.</returns>
    public static bool CheckDictionary(string guess) {
        return WordList.Words.Contains(guess.ToLower());
    }

    /// <summary>
    /// Validates the guess.
    /// </summary>
    /// <param name="guess">The guessed word.</param>
    /// <returns>The validation state of the guess.</returns>
    public ValidationState ValidateGuess(string guess) {
        return CheckDictionary(guess) ? ValidationState.Valid : ValidationState.NotInDictionary;
    }

    /// <summary>
    /// Tests the guess and returns the status of each letter and the number of correct letters.
    /// </summary>
    /// <param name="guess">The guessed word.</param>
    /// <returns>A tuple containing the list of statuses and the number of correct letters.</returns>
    public (List<Status>, int) TestGuess(string guess) {
        guess = guess.ToLower();
        List<Status> statusList = Enumerable.Repeat(Status.NotTested, COLUMNS).ToList();
        int correctLetters = 0;
        char[] tempWord = _word.ToCharArray();

        // First pass: Check for correct letters
        for (int i = 0; i < COLUMNS; i++) {
            if (guess[i] == _word[i]) {
                statusList[i] = Status.Correct;
                Letters[guess.ToUpper()[i]] = Status.Correct;
                correctLetters++;
                tempWord[i] = '\0'; // Mark as used
            }
        }

        // Second pass: Check for wrong place and incorrect letters
        for (int i = 0; i < COLUMNS; i++) {
            if (statusList[i] == Status.Correct) continue;

            if (tempWord.Contains(guess[i])) {
                statusList[i] = Status.WrongPlace;
                if (Letters[guess.ToUpper()[i]] != Status.Correct)
                    Letters[guess.ToUpper()[i]] = Status.WrongPlace;
                tempWord[Array.IndexOf(tempWord, guess[i])] = '\0'; // Mark as used
            } else {
                statusList[i] = Status.Incorrect;
                if (Letters[guess.ToUpper()[i]] != Status.Correct && Letters[guess.ToUpper()[i]] != Status.WrongPlace)
                    Letters[guess.ToUpper()[i]] = Status.Incorrect;
            }
        }

        return (statusList, correctLetters);
    }

    /// <summary>
    /// Handles the key event.
    /// </summary>
    /// <param name="e">The key event arguments.</param>
    public void HandleEvent(KeyEventArgs e) {
        if (_state == GameState.OutOfGame) {
            Initialize();
        } else {
            HandleKeyPress(e);
        }
    }

    private void HandleKeyPress(KeyEventArgs e) {
        if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z && Grid[_activeRow][_activeColumn].Letter == ' ') {
            HandleLetterInput(e);
        } else if (e.KeyCode == Keys.Back) {
            HandleBackspace();
        } else if (e.KeyCode == Keys.Enter && Grid[_activeRow][_activeColumn].Letter != ' ') {
            HandleEnter();
        }
    }

    private void HandleLetterInput(KeyEventArgs e) {
        Grid[_activeRow][_activeColumn].Letter = (char)e.KeyData;
        if (_activeColumn <= 3)
            _activeColumn += 1;
        Banner.Caption = "Guess #" + (_activeRow + 1);
    }

    private void HandleBackspace() {
        if (_activeColumn >= 1 && Grid[_activeRow][_activeColumn].Letter == ' ')
            _activeColumn -= 1;
        Grid[_activeRow][_activeColumn].Letter = ' ';
        Banner.Caption = "Guess #" + (_activeRow + 1);
    }

    private void HandleEnter() {
        string guess = new string(Grid[_activeRow].Select(square => square.Letter).ToArray());
        ValidationState validationState = ValidateGuess(guess);
        if (validationState == ValidationState.Valid) {
            ProcessValidGuess(guess);
        } else if (validationState == ValidationState.NotInDictionary) {
            ResetCurrentRow();
            Banner.Caption = "Word not in dictionary. Try again";
        }
    }

    private void ProcessValidGuess(string guess) {
        (List<Status> statuses, int correctLetters) = TestGuess(guess);
        for (int i = 0; i < Grid[_activeRow].Length; i++) {
            Grid[_activeRow][i].Status = statuses[i];
        }
        if (correctLetters == COLUMNS) {
            Banner.Caption = "Correct! You win!";
            _state = GameState.OutOfGame;
        } else if (_activeRow < ROWS - 1) {
            _activeColumn = 0;
            _activeRow++;
            foreach (Square square in Grid[_activeRow]) {
                square.Status = Status.NotTested;
            }
            Banner.Caption = "Guess #" + (_activeRow + 1);
        } else {
            Banner.Caption = "You lost! The word was: " + _word.ToUpper();
            _state = GameState.OutOfGame;
        }
    }

    private void ResetCurrentRow() {
        foreach (Square square in Grid[_activeRow]) {
            square.Letter = ' ';
            square.Status = Status.NotTested;
        }
        _activeColumn = 0;
    }
}
