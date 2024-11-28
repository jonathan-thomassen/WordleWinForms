namespace WordleWinForms;

/// <summary>
/// Represents a square in the Wordle game grid.
/// </summary>
internal class Square {
    /// <summary>
    /// Gets or sets the status of the square.
    /// </summary>
    public Status Status { get; set; } = Status.Inactive;

    /// <summary>
    /// Gets or sets the letter in the square.
    /// </summary>
    public char Letter { get; set; } = ' ';

    /// <summary>
    /// Initializes a new instance of the <see cref="Square"/> class.
    /// </summary>
    public Square() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Square"/> class with the specified status and letter.
    /// </summary>
    /// <param name="status">The status of the square.</param>
    /// <param name="letter">The letter in the square.</param>
    public Square(Status status, char letter) {
        Status = status;
        Letter = letter;
    }

    /// <summary>
    /// Returns a string that represents the current square.
    /// </summary>
    /// <returns>A string that represents the current square.</returns>
    public override string ToString() {
        return $"Square: {Letter}, Status: {Status}";
    }
}
