namespace WordleWinForms
{
    public partial class Form1 : Form
    {
        private Game _game;
        private Graphics _graphics;

        public Form1()
        {
            InitializeComponent();
            BackColor = Color.Black;
            Size = new Size(516, 820);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            _game = new(new Surface(_graphics), new Banner());
        }

        protected override void OnShown(EventArgs e)
        {
            _game.Initialize();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            _graphics.Clear(BackColor);
            _game.HandleEvent(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
