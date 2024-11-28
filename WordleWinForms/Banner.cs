namespace WordleWinForms;

/// <summary>
/// Represents a banner with a caption.
/// </summary>
internal class Banner {
    /// <summary>
    /// Gets or sets the caption of the banner.
    /// </summary>
    public string Caption { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Banner"/> class with a default caption.
    /// </summary>
    public Banner() {
        Caption = "Welcome";
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Banner"/> class.
    /// </summary>
    /// <param name="caption">The caption of the banner.</param>
    public Banner(string caption) {
        Caption = caption;
    }

    /// <summary>
    /// Returns a string that represents the current banner.
    /// </summary>
    /// <returns>A string that represents the current banner.</returns>
    public override string ToString() {
        return $"Banner: {Caption}";
    }
}
