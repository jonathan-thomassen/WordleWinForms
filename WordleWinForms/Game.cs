using static System.Windows.Forms.DataFormats;
using System.Data.Common;
using System.Drawing.Printing;
using System.Drawing;

namespace WordleWinForms;

/// <summary>
/// Represents the Wordle game.
/// </summary>
internal class Game
{
    private const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const int COLUMNS = 5;
    private const int ROWS = 6;

    private TableLayoutPanel _squarePanel;
    private TableLayoutPanel _keysPanel;

    internal GameState State { get; set; }
    internal Square[][] Grid { get; set; } = [new Square[5], new Square[5], new Square[5], new Square[5], new Square[5], new Square[5]];
    internal Banner Banner { get; private set; } = new();
    internal string Word { get; set; } = String.Empty;
    internal int ActiveRow { get; set; }
    internal int ActiveColumn { get; set; }
    internal readonly Dictionary<char, Status> Letters = new();

    public Game(TableLayoutPanel squarePanel, TableLayoutPanel keysPanel)
    {
        _squarePanel = squarePanel;
        _keysPanel = keysPanel;
        Initialize();
    }

    /// <summary>
    /// Initializes the game.
    /// </summary>
    internal void Initialize()
    {
        State = GameState.InGame;

        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < COLUMNS; j++)
            {
                Square square = new Square();
                _squarePanel.Controls.Add(square.Label);
                Grid[i][j] = square;
            }
        }

        foreach (var square in Grid[0])
        {
            square.Status = Status.NotTested;
        }

        NewWord();
        InitializeLetters();
        ActiveRow = 0;
        ActiveColumn = 0;
        Banner.Caption = "Welcome!";
    }

    internal void InitializeLetters()
    {
        Letters.Clear();
        foreach (char letter in ALPHABET)
        {
            KeySquare kSquare = new KeySquare();
            kSquare.Letter = letter;
            _keysPanel.Controls.Add(kSquare.Label);
            Letters.Add(letter, Status.NotTested);
        }
    }

    /// <summary>
    /// Selects a new word for the game.
    /// </summary>
    internal void NewWord()
    {
        Random random = new();
        int wordNumber = random.Next(WordList.Words.Count);
        Word = WordList.Words[wordNumber];
    }

    /// <summary>
    /// Checks if the guess is in the dictionary.
    /// </summary>
    /// <param name="guess">The guessed word.</param>
    /// <returns>True if the guess is in the dictionary, otherwise false.</returns>
    internal static bool CheckDictionary(string guess)
    {
        return WordList.Words.Contains(guess.ToLower());
    }

    /// <summary>
    /// Validates the guess.
    /// </summary>
    /// <param name="guess">The guessed word.</param>
    /// <returns>The validation state of the guess.</returns>
    internal static ValidationState ValidateGuess(string guess)
    {
        return CheckDictionary(guess) ? ValidationState.Valid : ValidationState.NotInDictionary;
    }

    /// <summary>
    /// Tests the guess and returns the status of each letter and the number of correct letters.
    /// </summary>
    /// <param name="guess">The guessed word.</param>
    /// <returns>A tuple containing the list of statuses and the number of correct letters.</returns>
    internal (List<Status>, int) TestGuess(string guess)
    {
        guess = guess.ToLower();
        List<Status> statusList = Enumerable.Repeat(Status.NotTested, COLUMNS).ToList();
        int correctLetters = 0;
        char[] tempWord = Word.ToCharArray();

        correctLetters = CheckCorrectLetters(guess, statusList, tempWord);
        CheckWrongPlaceAndIncorrectLetters(guess, statusList, tempWord);

        return (statusList, correctLetters);
    }

    internal int CheckCorrectLetters(string guess, List<Status> statusList, char[] tempWord)
    {
        int correctLetters = 0;
        for (int i = 0; i < COLUMNS; i++)
        {
            if (guess[i] == Word[i])
            {
                statusList[i] = Status.Correct;
                Letters[guess.ToUpper()[i]] = Status.Correct;
                correctLetters++;
                tempWord[i] = '\0'; // Mark as used
            }
        }
        return correctLetters;
    }

    internal void CheckWrongPlaceAndIncorrectLetters(string guess, List<Status> statusList, char[] tempWord)
    {
        for (int i = 0; i < COLUMNS; i++)
        {
            if (statusList[i] == Status.Correct) continue;

            if (tempWord.Contains(guess[i]))
            {
                statusList[i] = Status.WrongPlace;
                if (Letters[guess.ToUpper()[i]] != Status.Correct)
                    Letters[guess.ToUpper()[i]] = Status.WrongPlace;
                tempWord[Array.IndexOf(tempWord, guess[i])] = '\0'; // Mark as used
            } else
            {
                statusList[i] = Status.Incorrect;
                if (Letters[guess.ToUpper()[i]] != Status.Correct && Letters[guess.ToUpper()[i]] != Status.WrongPlace)
                    Letters[guess.ToUpper()[i]] = Status.Incorrect;
            }
        }
    }

    /// <summary>
    /// Handles the key event.
    /// </summary>
    /// <param name="e">The key event arguments.</param>
    internal void HandleEvent(KeyEventArgs e)
    {
        if (State == GameState.OutOfGame)
        {
            Initialize();
        } else
        {
            HandleKeyPress(e);
        }
    }

    internal void HandleKeyPress(KeyEventArgs e)
    {
        if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z && Grid[ActiveRow][ActiveColumn].Letter == ' ')
        {
            HandleLetterInput(e);
        } else if (e.KeyCode == Keys.Back)
        {
            HandleBackspace();
        } else if (e.KeyCode == Keys.Enter && Grid[ActiveRow][ActiveColumn].Letter != ' ')
        {
            HandleEnter();
        }
    }

    internal void HandleLetterInput(KeyEventArgs e)
    {
        Grid[ActiveRow][ActiveColumn].Letter = (char)e.KeyData;
        if (ActiveColumn <= 3)
            ActiveColumn += 1;
        Banner.Caption = "Guess #" + (ActiveRow + 1);
    }

    internal void HandleBackspace()
    {
        if (ActiveColumn >= 1 && Grid[ActiveRow][ActiveColumn].Letter == ' ')
            ActiveColumn -= 1;
        Grid[ActiveRow][ActiveColumn].Letter = ' ';
        Banner.Caption = "Guess #" + (ActiveRow + 1);
    }

    internal void HandleEnter()
    {
        string guess = new string(Grid[ActiveRow].Select(square => square.Letter).ToArray());
        ValidationState validationState = ValidateGuess(guess);
        if (validationState == ValidationState.Valid)
        {
            ProcessValidGuess(guess);
        } else if (validationState == ValidationState.NotInDictionary)
        {
            ResetCurrentRow();
            Banner.Caption = "Word not in dictionary. Try again";
        }
    }

    internal void ProcessValidGuess(string guess)
    {
        (List<Status> statuses, int correctLetters) = TestGuess(guess);
        UpdateGridWithStatuses(statuses);
        UpdateGameState(correctLetters);
    }

    internal void UpdateGridWithStatuses(List<Status> statuses)
    {
        for (int i = 0; i < Grid[ActiveRow].Length; i++)
        {
            Grid[ActiveRow][i].Status = statuses[i];
        }
    }

    internal void UpdateGameState(int correctLetters)
    {
        if (correctLetters == COLUMNS)
        {
            Banner.Caption = "Correct! You win!";
            State = GameState.OutOfGame;
        } else if (ActiveRow < ROWS - 1)
        {
            MoveToNextRow();
        } else
        {
            Banner.Caption = "You lost! The word was: " + Word.ToUpper();
            State = GameState.OutOfGame;
        }
    }

    internal void MoveToNextRow()
    {
        ActiveColumn = 0;
        ActiveRow++;
        foreach (Square square in Grid[ActiveRow])
        {
            square.Status = Status.NotTested;
        }
        Banner.Caption = "Guess #" + (ActiveRow + 1);
    }

    internal void ResetCurrentRow()
    {
        foreach (Square square in Grid[ActiveRow])
        {
            square.Letter = ' ';
            square.Status = Status.NotTested;
        }
        ActiveColumn = 0;
    }
}
