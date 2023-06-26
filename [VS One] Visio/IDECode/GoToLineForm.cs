using ScintillaNET;
using System;
using System.Windows.Forms;

namespace _VS_One__Visio
{
    public partial class GoToLineForm : Form
    {
        Scintilla scintilla;

        public GoToLineForm(Scintilla source)
        {
            InitializeComponent();

            scintilla = source;
            label1.Text = $"Количество строк: {scintilla.Lines.Count}";
            this.ActiveControl = numericUpDown1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            goTo();
        }

        public void goTo()
        {
            scintilla.Lines[Convert.ToInt32(numericUpDown1.Value)].Goto();
        }

        private void GoToLineForm_Load(object sender, EventArgs e)
        {
            numericUpDown1.Focus();
        }

        private void GoToLineForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) goTo();
        }
    }
}
