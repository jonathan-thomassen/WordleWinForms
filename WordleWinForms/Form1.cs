namespace WordleWinForms;
public partial class Form1 : Form
{
    const double BASE_SCREEN_H = 1080.0;
    const int BASE_CLIENT_WIDTH = 496;
    const int BASE_CLIENT_HEIGHT = 792;

    private readonly double _scale;
    private readonly Game _game;
    private readonly Surface _surface;

    public Form1()
    {
        InitializeComponent();

        _scale = (double)(Screen.PrimaryScreen?.WorkingArea.Height ?? BASE_SCREEN_H) / BASE_SCREEN_H;

        //BackColor = Color.Black;
        //ClientSize = new Size((int)(BASE_CLIENT_WIDTH * _scale), (int)(BASE_CLIENT_HEIGHT * _scale));
        //FormBorderStyle = FormBorderStyle.FixedSingle;
        //DoubleBuffered = true;
        //ShowIcon = false;
        //Text = "Wordle";

        _surface = new(_scale, ClientSize.Width);
        _game = new(tableLayoutPanel1, tableLayoutPanel2);
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        _game.HandleEvent(e);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        //_surface.DrawGame(e.Graphics, _game);
    }

    private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
    {

    }

    private void Form1_Load(object sender, EventArgs e)
    {

    }
}
