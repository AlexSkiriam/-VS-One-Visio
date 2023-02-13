using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScintillaNET;

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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            scintilla.Lines[Convert.ToInt32(numericUpDown1.Value)].Goto();
        }
    }
}
