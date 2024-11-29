using System.Windows.Forms;
using WordleWinForms;
using WordleWinForms.Enums;

namespace WordleWinFormsTests;

public class GameTests
{
    private readonly Game _game;

    public GameTests()
    {
        _game = new Game();
    }

    [Fact]
    public void Initialize_ShouldSetInitialState()
    {
        _game.Initialize();
        Assert.Equal(GameState.InGame, _game.State);
        Assert.Equal(6, _game.Grid.Length);
        Assert.Equal(5, _game.Grid[0].Length);
        Assert.Equal("Welcome!", _game.Banner.Caption);
        Assert.Equal(0, _game.ActiveRow);
        Assert.Equal(0, _game.ActiveColumn);
        Assert.NotEmpty(_game.Word);
        Assert.All(_game.Grid[0], square => Assert.Equal(Status.NotTested, square.Status));
        Assert.All(_game.Grid.Skip(1), row => Assert.All(row, square => Assert.Equal(Status.Inactive, square.Status)));
    }

    [Fact]
    public void NewWord_ShouldSetNewWord()
    {
        string initialWord = _game.Word;
        _game.NewWord();
        Assert.NotEqual(initialWord, _game.Word);
    }

    [Fact]
    public void CheckDictionary_ShouldReturnCorrectValues()
    {
        Assert.True(Game.CheckDictionary("apple"));
        Assert.False(Game.CheckDictionary("zzzzz"));
    }

    [Fact]
    public void ValidateGuess_ShouldReturnCorrectValues()
    {
        Assert.Equal(ValidationState.Valid, Game.ValidateGuess("apple"));
        Assert.Equal(ValidationState.NotInDictionary, Game.ValidateGuess("zzzzz"));
    }

    [Fact]
    public void TestGuess_ShouldReturnCorrectStatuses()
    {
        _game.Word = "apple";
        var (statuses, correctLetters) = _game.TestGuess("apple");
        Assert.Equal(5, correctLetters);
        Assert.All(statuses, status => Assert.Equal(Status.Correct, status));
    }

    [Fact]
    public void HandleEvent_ShouldInitializeGameOnOutOfGameState()
    {
        _game.State = GameState.OutOfGame;
        _game.HandleEvent(new KeyEventArgs(Keys.Enter));
        Assert.Equal(GameState.InGame, _game.State);
    }

    [Fact]
    public void ProcessValidGuess_ShouldUpdateGridAndBanner()
    {
        _game.Word = "apple";
        _game.ProcessValidGuess("apple");
        Assert.Equal("Correct! You win!", _game.Banner.Caption);
        Assert.Equal(GameState.OutOfGame, _game.State);
    }

    [Fact]
    public void ResetCurrentRow_ShouldClearCurrentRow()
    {
        _game.HandleKeyPress(new KeyEventArgs(Keys.A));
        _game.ResetCurrentRow();
        Assert.All(_game.Grid[0], square =>
        {
            Assert.Equal(' ', square.Letter);
            Assert.Equal(Status.NotTested, square.Status);
        });
    }

    [Fact]
    public void InitializeLetters_ShouldSetAllLettersToNotTested()
    {
        _game.InitializeLetters();
        Assert.All(_game.Letters.Values, status => Assert.Equal(Status.NotTested, status));
    }

    [Fact]
    public void CheckCorrectLetters_ShouldReturnCorrectCount()
    {
        string guess = "apple";
        List<Status> statusList = Enumerable.Repeat(Status.NotTested, 5).ToList();
        char[] tempWord = _game.Word.ToCharArray();
        int correctLetters = _game.CheckCorrectLetters(guess, statusList, tempWord);
        Assert.Equal(guess.Where((c, i) => _game.Word[i] == c).Count(), correctLetters);
    }

    [Fact]
    public void CheckWrongPlaceAndIncorrectLetters_ShouldUpdateStatuses()
    {
        string guess = "apple";
        List<Status> statusList = Enumerable.Repeat(Status.NotTested, 5).ToList();
        char[] tempWord = _game.Word.ToCharArray();
        _game.CheckWrongPlaceAndIncorrectLetters(guess, statusList, tempWord);
        Assert.All(statusList, status => Assert.Contains(status, new[] { Status.Correct, Status.WrongPlace, Status.Incorrect }));
    }

    [Fact]
    public void UpdateGridWithStatuses_ShouldUpdateGridStatuses()
    {
        List<Status> statuses = new() { Status.Correct, Status.Incorrect, Status.WrongPlace, Status.NotTested, Status.Correct };
        _game.UpdateGridWithStatuses(statuses);
        for (int i = 0; i < statuses.Count; i++)
        {
            Assert.Equal(statuses[i], _game.Grid[_game.ActiveRow][i].Status);
        }
    }

    [Fact]
    public void UpdateGameState_ShouldSetOutOfGameOnWin()
    {
        _game.UpdateGameState(5);
        Assert.Equal(GameState.OutOfGame, _game.State);
        Assert.Equal("Correct! You win!", _game.Banner.Caption);
    }

    [Fact]
    public void UpdateGameState_ShouldMoveToNextRowOnIncorrectGuess()
    {
        _game.UpdateGameState(3);
        Assert.Equal(1, _game.ActiveRow);
        Assert.Equal(0, _game.ActiveColumn);
        Assert.All(_game.Grid[_game.ActiveRow], square => Assert.Equal(Status.NotTested, square.Status));
    }

    [Fact]
    public void MoveToNextRow_ShouldResetActiveColumnAndIncrementActiveRow()
    {
        _game.MoveToNextRow();
        Assert.Equal(1, _game.ActiveRow);
        Assert.Equal(0, _game.ActiveColumn);
        Assert.All(_game.Grid[_game.ActiveRow], square => Assert.Equal(Status.NotTested, square.Status));
    }

    [Fact]
    public void HandleKeyPress_ShouldHandleLetterInput()
    {
        var keyEventArgs = new KeyEventArgs(Keys.A);
        _game.HandleKeyPress(keyEventArgs);
        Assert.Equal('A', _game.Grid[0][0].Letter);
        Assert.Equal(1, _game.ActiveColumn);
    }

    [Fact]
    public void HandleKeyPress_ShouldHandleBackspace()
    {
        _game.HandleKeyPress(new KeyEventArgs(Keys.A));
        var backspaceEventArgs = new KeyEventArgs(Keys.Back);
        _game.HandleKeyPress(backspaceEventArgs);
        Assert.Equal(' ', _game.Grid[0][0].Letter);
        Assert.Equal(0, _game.ActiveColumn);
    }

    [Fact]
    public void HandleKeyPress_ShouldHandleEnter()
    {
        _game.HandleKeyPress(new KeyEventArgs((Keys)'A'));
        _game.HandleKeyPress(new KeyEventArgs((Keys)'P'));
        _game.HandleKeyPress(new KeyEventArgs((Keys)'P'));
        _game.HandleKeyPress(new KeyEventArgs((Keys)'L'));
        _game.HandleKeyPress(new KeyEventArgs((Keys)'E'));
        var enterEventArgs = new KeyEventArgs(Keys.Enter);
        _game.HandleKeyPress(enterEventArgs);
        Assert.Equal(1, _game.ActiveRow);
        Assert.Equal(0, _game.ActiveColumn);
    }

    [Fact]
    public void HandleEvent_ShouldHandleKeyPressWhenInGame()
    {
        var keyEventArgs = new KeyEventArgs(Keys.A);
        _game.HandleEvent(keyEventArgs);
        Assert.Equal('A', _game.Grid[0][0].Letter);
    }

    [Fact]
    public void GameLossTest()
    {
        _game.Initialize();
        _game.Word = "apple"; // Set a known word
        for (int i = 0; i < 6; i++)
        {
            _game.Grid[i][0].Letter = 'z'; // Set incorrect letters
            _game.Grid[i][1].Letter = 'e'; // Set incorrect letters
            _game.Grid[i][2].Letter = 'b'; // Set incorrect letters
            _game.Grid[i][3].Letter = 'r'; // Set incorrect letters
            _game.Grid[i][4].Letter = 'a'; // Set incorrect letters
            _game.HandleEnter();
        }
        Assert.Equal(GameState.OutOfGame, _game.State);
        Assert.Equal("You lost! The word was: APPLE", _game.Banner.Caption);
    }

    [Fact]
    public void WordNotInDictionaryTest()
    {
        _game.Initialize();
        for (int i = 0; i < 6; i++)
        {
            _game.Grid[i][0].Letter = 'z'; // Set incorrect letters
            _game.Grid[i][1].Letter = 'z'; // Set incorrect letters
            _game.Grid[i][2].Letter = 'z'; // Set incorrect letters
            _game.Grid[i][3].Letter = 'z'; // Set incorrect letters
            _game.Grid[i][4].Letter = 'z'; // Set incorrect letters
            _game.HandleEnter();
        }
        Assert.Equal("Word not in dictionary. Try again", _game.Banner.Caption);
        Assert.Equal(0, _game.ActiveRow);
        Assert.Equal(0, _game.ActiveColumn);
    }
}