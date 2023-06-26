using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Visio = Microsoft.Office.Interop.Visio;

namespace _VS_One__Visio
{
    public partial class VariablesFromDiagram : Form
    {
        Visio.Shapes shapes;
        PhrasesFunctions phr = new PhrasesFunctions();

        public string getPhrase()
        {
            return (listView2.SelectedItems.Count == 1) ? listView2.SelectedItems[0].Text : "";
        }

        public VariablesFromDiagram(Visio.Shapes s)
        {
            InitializeComponent();
            shapes = s;
        }

        private void VariablesFromDiagram_OnLoad(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            listView2.FullRowSelect = true;
            listView2.MouseUp += new MouseEventHandler(listView2_MouseClick);
            setParametersList();

        }

        private void listView2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView2.FocusedItem != null && listView2.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    ContextMenu menu = new ContextMenu();

                    MenuItem toClipboard = new MenuItem("Скопировать фразу");
                    toClipboard.Click += delegate (object sender2, EventArgs e2) { addPhraseToClipBoard(); };
                    menu.MenuItems.Add(toClipboard);

                    menu.Show(listView2, new Point(e.X, e.Y));
                }
            }
        }

        private void addPhraseToClipBoard()
        {
            Clipboard.SetText(getPhrase());
        }

        public void setParametersList()
        {
            List<string> list = phr.variableText(shapes);

            foreach (string str in list)
            {
                ListViewItem item = new ListViewItem(str);

                listView2.Items.Add(item);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            shapes.Cast<Visio.Shape>().ToList().Where(x => x.NameU.IndexOf("Process") > -1 && !String.IsNullOrEmpty(x.Text)).ToList().ForEach(elem =>
            {
                if (!String.IsNullOrEmpty(elem.Text))
                {
                    string pattern = "(";
                    listView2.Items.Cast<ListViewItem>().ToList().ForEach(x => { pattern += $"{x.Text}|"; });
                    pattern = Regex.Replace(pattern, @"\|$", "");
                    pattern = Regex.Replace(pattern, @"(?<!\[)\[", "\\[");
                    pattern = Regex.Replace(pattern, @"(?<!\])\]", "\\]");
                    pattern += ")";

                    Visio.Characters chars = elem.Characters;
                    chars.Text = elem.Text;
                    if (Regex.IsMatch(elem.Text, @pattern))
                    {
                        Regex.Matches(elem.Text, @pattern).Cast<Match>().ToList().ForEach(x =>
                        {
                            int start = x.Index;
                            chars.Begin = start;
                            chars.End = start + x.Length;
                            chars.CharProps[1] = 2;
                        });
                    }
                }
            });
        }
    }
}
