using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Visio = Microsoft.Office.Interop.Visio;
using Excel = Microsoft.Office.Interop.Excel;

namespace _VS_One__Visio
{
    public partial class EditForm : Form
    {
        public Visio.Shape shape;
        public Phrases form;

        public EditForm(Visio.Shape sourceShape, Phrases phrases)
        {
            InitializeComponent();
            shape = sourceShape;
            form = phrases;
        }

        private void EditForm_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = shape.Text;
            richTextBox2.Text = "Блок №\nОсновная фраза:\n\nФраза для повтора:";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            shape.Text = richTextBox2.Text;
            form.editResult = true;
            this.Close();
        }
    }
}
