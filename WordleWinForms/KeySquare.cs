namespace WordleWinForms;

/// <summary>
/// Represents a square in the Wordle game grid.
/// </summary>
internal class KeySquare
{
    private Status _status = Status.Inactive;
    private char _letter = ' ';
    private Label _label;
    /// <summary>
    /// Gets or sets the status of the square.
    /// </summary>
    public Status Status
    {
        get => _status;
        set
        {
            _status = value;
            switch (_status)
            {
                case Status.Inactive:
                    _label.BorderStyle = BorderStyle.FixedSingle;
                    _label.ForeColor = Color.Gray;
                    break;
                case Status.NotTested:
                    _label.ForeColor = Color.White;
                    break;
                case Status.Incorrect:
                    _label.ForeColor = Color.Red;
                    break;
                case Status.WrongPlace:
                    _label.ForeColor = Color.Yellow;
                    break;
                case Status.Correct:
                    _label.ForeColor = Color.Green;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }

    /// <summary>
    /// Gets or sets the letter in the square.
    /// </summary>
    public char Letter
    {
        get => _letter;
        set
        {
            _label.Text = value.ToString();
            _letter = value;
        }
    }

    public Label Label
    {
        get => _label;
        set
        {
            _label = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Square"/> class.
    /// </summary>
    public KeySquare()
    {
        _label = new Label()
        {
            Text = _letter.ToString(),
            BackColor = Color.Black,
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Trebuchet MS", (float)16.0)
        };
    }

    /// <summary>
    /// Returns a string that represents the current square.
    /// </summary>
    /// <returns>A string that represents the current square.</returns>
    public override string ToString()
    {
        return $"Square: {Letter}, Status: {Status}";
    }
}
