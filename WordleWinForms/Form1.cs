namespace WordleWinForms;

public partial class Form1 : Form {
    const double BASE_SCREEN_H = 1080.0;
    const int BASE_WIN_W = 514;
    const int BASE_WIN_H = 818;

    private readonly double _scale = (double)(Screen.PrimaryScreen?.WorkingArea.Height ?? BASE_SCREEN_H) / (double)BASE_SCREEN_H;
    private Game _game;
    private Surface _surface;

    public Form1() {
        InitializeComponent();
        BackColor = Color.Black;
        Size = new Size((int)(BASE_WIN_W * _scale), (int)(BASE_WIN_H * _scale));
        FormBorderStyle = FormBorderStyle.FixedSingle;
        DoubleBuffered = true;
        ShowIcon = false;
        Text = "Wordle";

        _surface = new(_scale, (int)(BASE_WIN_W * _scale));
        _game = new();
    }

    protected override void OnShown(EventArgs e) {
        base.OnShown(e);

        _game.Initialize();
    }

    protected override void OnKeyDown(KeyEventArgs e) {
        base.OnKeyDown(e);
        _game.HandleEvent(e);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e) {
        // Call the OnPaint method of the base class.
        base.OnPaint(e);

        _surface.DrawGame(e.Graphics, _game);
    }

    private void Form1_Load(object sender, EventArgs e) {

    }
}
