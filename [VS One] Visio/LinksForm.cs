using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Visio = Microsoft.Office.Interop.Visio;
using Excel = Microsoft.Office.Interop.Excel;

namespace _VS_One__Visio
{
    public partial class LinksForm : Form
    {
        private Dictionary<string, Visio.Shape> hyperLinkShapes;
        private Visio.Shapes shapes;
        private string textShape;

        public LinksForm(Visio.Shapes shp)
        {
            InitializeComponent();
            shapes = shp;
        }

        private void LinksForm_Load(object sender, EventArgs e)
        {
            listView1.FullRowSelect = true;
            listView1.MouseClick += new MouseEventHandler(listView1_MouseClick);
            setListElements();
        }

        private void setListElements()
        {
            List<string> linkedBlocks = new List<string>();

            foreach (Visio.Shape shape in shapes)
            {
                Match match = Regex.Match(shape.Text, @"\w\s(\w+\.*\s)*(\w+\s+№*((\d+\.?)+))");

                if ((shape.NameU.IndexOf("Start/End") > -1 || shape.NameU.IndexOf("On-page reference") > -1) 
                    && match.Success && !linkedBlocks.Contains(shape.Text)) linkedBlocks.Add(shape.Text);
            }

            foreach (string str in linkedBlocks)
            {
                listView1.Items.Add(str);
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            textShape = "";

            hyperLinkShapes = new Dictionary<string, Visio.Shape>();

            comboBox1.Text = "";
            comboBox1.Items.Clear();

            ListViewHitTestInfo info = listView1.HitTest(e.X, e.Y);
            ListViewItem item = info.Item;

            if (item != null)
            {
                textShape = item.Text;

                foreach (Visio.Shape shape in shapes)
                {
                    if (shape.NameU.IndexOf("Start/End") == -1 || shape.NameU.IndexOf("On-page reference") == -1)
                    {
                        string element = Regex.Replace(item.Text, @"^\w\s", "").ToLower();

                        Match match = Regex.Match(shape.Text.ToLower(), @"^\s*" + element + @"(\s*[^ \. \d]|\z)");

                        if (match.Success)
                        {
                            if (comboBox1.Items.Count == 0)
                            {
                                if (!hyperLinkShapes.ContainsKey(shape.Text))
                                {
                                    comboBox1.Text = shape.Text;
                                    comboBox1.Items.Add(shape.Text);
                                    hyperLinkShapes.Add(shape.Text, shape);
                                }
                            }
                            else
                            {
                                if (!hyperLinkShapes.ContainsKey(shape.Text))
                                {
                                    comboBox1.Items.Add(shape.Text);
                                    hyperLinkShapes.Add(shape.Text, shape);
                                }
                            }
                        } 
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int counter = 0;

            if (hyperLinkShapes != null && !String.IsNullOrEmpty(textShape))
            {
                Visio.Shape hyperLink = hyperLinkShapes[comboBox1.Text];

                foreach (Visio.Shape shape in shapes)
                {
                    if ((shape.NameU.IndexOf("Start/End") > -1 || shape.NameU.IndexOf("On-page reference") > -1) && shape.Text == textShape)
                    {
                        shape.AddHyperlink();
                        shape.Hyperlink.SubAddress = String.Format("../{0}", hyperLink.NameU);

                        counter++;
                    }
                }
            }
            
            if (counter > 0) MessageBox.Show(String.Format("Ссылки были проставлены. Количество блоков: {0}", counter.ToString()));
        }
    }
}
