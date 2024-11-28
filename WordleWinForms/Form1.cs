using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordleWinForms
{
    public partial class Form1 : Form
    {
        private Game _game;

        public Form1()
        {
            InitializeComponent();
            Size = new Size(516, 820);
            Graphics g = CreateGraphics();
            _game = new(new Screen(g), new Banner());
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            _game.Update();
        }

        protected override void OnShown(EventArgs e)
        {
            _game.Initialize();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
