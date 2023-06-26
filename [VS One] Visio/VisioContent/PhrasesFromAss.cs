using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _VS_One__Visio
{
    public partial class PhrasesFromAss : Form
    {
        public PhrasesFromAss()
        {
            InitializeComponent();
        }

        public List<string> phrasesAndNamespaces;

        public string getPhrase()
        {
            return (listView1.SelectedItems.Count == 1) ? listView1.SelectedItems[0].SubItems[0].Text : "";
        }

        public string getNameSpace()
        {
            return (listView1.SelectedItems.Count == 1) ? listView1.SelectedItems[0].SubItems[1].Text : "";
        }

        UsingText usingText = new UsingText();

        private void PhrasesFromAss_Load(object sender, EventArgs e)
        {
            listView1.FullRowSelect = true;
            listView1.MouseUp += new MouseEventHandler(listView1_MouseClick);

            checkBox2.Checked = true;
            checkBox3.Checked = true;
            trackBar1.Value = 5;
        }

        private void PhrasesFromAss_Closed(object sender, FormClosedEventArgs e)
        {
            Form frm = Application.OpenForms["SearchForm"];

            if (frm != null)
            {
                frm.Close();
            }
        }

        private void PhrasesFromAss_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                Form frm = Application.OpenForms["SearchForm"];

                if (frm == null)
                {
                    frm = new SearchForm(this, listView1, 0);
                    frm.Visible = true;
                    frm.Show();
                    frm.TopMost = true;
                }
                else
                {
                    frm.Focus();
                }
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem != null && listView1.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    ContextMenu menu = new ContextMenu();

                    MenuItem toClipboard = new MenuItem("Скопировать фразу");
                    toClipboard.Click += delegate (object sender2, EventArgs e2) { addPhraseToClipBoard(); };
                    menu.MenuItems.Add(toClipboard);

                    MenuItem toClipboardNamespace = new MenuItem("Скопировать неймспейс");
                    toClipboardNamespace.Click += delegate (object sender2, EventArgs e2) { addNamespaceToClipBoard(); };
                    menu.MenuItems.Add(toClipboardNamespace);

                    menu.Show(listView1, new Point(e.X, e.Y));
                }
            }
        }

        private void addPhraseToClipBoard()
        {
            Clipboard.SetText(getPhrase());
        }

        private void addNamespaceToClipBoard()
        {
            Clipboard.SetText(getNameSpace());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            phrasesAndNamespaces = new List<string>();

            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;

                string[] lines = File.ReadAllLines(fileName);

                foreach (string line in lines)
                {
                    Match match = Regex.Match(line, @"[А-я]+");
                    if (match.Success) phrasesAndNamespaces.Add(line);
                    //if (Array.IndexOf(lines, line) > 17) phrasesAndNamespaces.Add(line);
                }

                foreach (string str in phrasesAndNamespaces)
                {
                    Dictionary<string, string> dict = phraseParser(str);

                    ListViewItem item = new ListViewItem(dict.ElementAt(0).Key);
                    item.SubItems.Add(dict.ElementAt(0).Value);

                    listView1.Items.Add(item);
                }
            }
        }

        public Dictionary<string, string> phraseParser(string sourceString)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string key = Regex.Replace(sourceString, @"\w+: \d+,\d+:\d+:\d+\.\d+,\d+:\d+:\d+\.\d+,[^,]*,[^,]*,[^,]*,\d+,\d+,\d+,,", "");

            string value = Regex.Replace(sourceString, @"\w+: \d+,\d+:\d+:\d+\.\d+,\d+:\d+:\d+\.\d+,[^,]+,[^,]+\s", "");
            value = Regex.Replace(value, @",\d+,\d+,\d+,[^,]*,.+", "");

            dict.Add(key, value);

            return dict;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int tabsCount = trackBar1.Value;
            string tabs = usingText.tabsToReturn(tabsCount);
            string firstTab = checkBox3.Checked ? "" : tabs;

            string returnString = "";

            int i = 0;

            foreach (ListViewItem item in listView1.SelectedItems)
            {
                string firstTabOrNot = (i == 0) ? firstTab : tabs;
                string endOfString = (i == listView1.SelectedItems.Count - 1) ? "" : "\n";

                string aditionalText = (!String.IsNullOrEmpty(comboBox1.Text)) ? String.Format("_{0}", comboBox1.Text) : "";

                returnString += firstTabOrNot + "{\n"
                            + tabs + "\t\"text" + aditionalText + "\": \"" + item.Text + "\",\n"
                            + tabs + "\t\"namespace\": [\"" + item.SubItems[1].Text + "\"]\n"
                            + tabs + "}," + endOfString;

                i++;
            }

            richTextBox1.Text = returnString;
            Clipboard.SetText(returnString);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;
                comboBox1.Items.Clear();
                comboBox1.Items.Add("male");
                comboBox1.Items.Add("female");
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
                comboBox1.Items.Clear();
                comboBox1.Items.Add("Grigoriev");
                comboBox1.Items.Add("Trofimov");
                comboBox1.Items.Add("Vetrova");
                comboBox1.Items.Add("Prudchenko");
            }
        }
    }
}
