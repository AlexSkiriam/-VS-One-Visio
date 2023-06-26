using System.Windows.Forms;

namespace _VS_One__Visio
{
    public partial class BasicPhraseBuldier : Form
    {
        public BasicPhraseBuldier(string text)
        {
            InitializeComponent();
            richTextBox1.Text = text;
        }
    }
}
