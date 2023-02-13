using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _VS_One__Visio
{
    public partial class MetaForm : Form
    {
        public int lineNumber { get; set; }

        public MetaForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            closing();
        }

        private void closing()
        {
            this.lineNumber = Convert.ToInt32(numericUpDown1.Value);
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void MetaForm_Load(object sender, EventArgs e)
        {

        }

        private void MetaForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape) closing();
        }
    }
}
