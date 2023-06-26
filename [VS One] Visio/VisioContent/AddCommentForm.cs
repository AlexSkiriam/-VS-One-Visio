using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Visio = Microsoft.Office.Interop.Visio;

namespace _VS_One__Visio
{
    public partial class AddCommentForm : Form
    {
        Visio.Shape shape;

        public AddCommentForm(Visio.Shape sourceShape)
        {
            InitializeComponent();
            shape = sourceShape;
        }

        private void AddCommentForm_OnLoad(object sender, EventArgs e)
        {
            if (shape.Comments.Count > 0) commentParser(shape.Comments[1].Text);
        }

        private void commentParser(string sourceComment)
        {
            Match match = Regex.Match(sourceComment, @"(^\s*\w+)(\([^\)]*\));(^\s*\w+)(\([^\)]*\));");
            Match spareMatch = Regex.Match(sourceComment, @"(^\s*\w+)(\([^\)]*\));");
            if (match.Success)
            {
                textBox1.Text = match.Groups[1].Value;
                textBox2.Text = match.Groups[2].Value.Replace("(", "").Replace(")", "");
                textBox3.Text = match.Groups[3].Value;
                textBox4.Text = match.Groups[4].Value.Replace("(", "").Replace(")", "");
            }
            else if (spareMatch.Success)
            {
                textBox1.Text = match.Groups[1].Value;
                textBox2.Text = match.Groups[2].Value.Replace("(", "").Replace(")", "");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string firstPart = (!String.IsNullOrEmpty(textBox3.Text)) ? String.Format("{0}({1});", textBox1.Text, textBox2.Text) : "";
            string secondPart = (!String.IsNullOrEmpty(textBox3.Text)) ? String.Format("{0}({1});", textBox3.Text, textBox4.Text) : "";
            string newComment = String.Format("{0}{1}", firstPart, secondPart);
            if (shape.Comments.Count == 0) shape.Comments.Add(newComment);
            else shape.Comments[1].Text = newComment;
            Close();
        }
    }
}
