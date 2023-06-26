using System;
using System.Windows.Forms;
using Visio = Microsoft.Office.Interop.Visio;

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
