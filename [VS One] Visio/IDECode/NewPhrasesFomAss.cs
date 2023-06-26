using ActiveBC.Tools.Speech;
using System;
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
    public partial class NewPhrasesFomAss : Form
    {
        public VisioViewerForm text;
        Dictionary<string, int> textMeta = new Dictionary<string, int>();
        public List<string> phrasesAndNamespaces;

        public NewPhrasesFomAss(VisioViewerForm form)
        {
            InitializeComponent();
            text = form;

            listView1.FullRowSelect = true;
            listView1.MouseUp += new MouseEventHandler(listView1_MouseClick);

            this.KeyDown += NewPhrasesFomAss_KeyDown;
            this.FormClosed += NewPhrasesFomAss_FormClosed;
        }

        private void NewPhrasesFomAss_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form frm = Application.OpenForms["SearchForm"];

            if (frm != null)
            {
                frm.Close();
            }
        }

        private void NewPhrasesFomAss_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                switch(e.KeyCode)
                {
                    case Keys.F:
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
                        break;
                }
                
            }
        }

        public string getPhrase()
        {
            return (listView1.SelectedItems.Count == 1) ? listView1.SelectedItems[0].SubItems[0].Text : "";
        }

        public string getNameSpace()
        {
            return (listView1.SelectedItems.Count == 1) ? listView1.SelectedItems[0].SubItems[1].Text : "";
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

                    MenuItem find = new MenuItem("Найти в скрипте");
                    find.Click += delegate (object sender2, EventArgs e2) { tryGoToText(); };
                    if(text.textEditor != null) menu.MenuItems.Add(find);

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

        private void tryGoToText()
        {
            findAllTextFildInScript();
            LevinsteinDistanceClass levinstein = new LevinsteinDistanceClass();
            int currentPosition = levinstein.findWithLevinstainInTextMeta(getPhrase(), textMeta);
            if (currentPosition > 0)
            {
                goToScintillaLine(currentPosition);
            }
        }

        private void findAllTextFildInScript()
        {
            textMeta.Clear();
            foreach (Match match in Regex.Matches(text.textEditor.scintilla1.Text, @"""text""\s*:\s*""([^""]+)"""))
            {
                if (!textMeta.ContainsKey(match.Groups[1].Value)) textMeta.Add(match.Groups[1].Value, match.Index);
            }
        }

        private void goToScintillaLine(int position)
        {
            var linesOnScreen = text.textEditor.scintilla1.LinesOnScreen - 2;

            var currentLine = text.textEditor.scintilla1.LineFromPosition(position);

            var start = text.textEditor.scintilla1.Lines[currentLine - (linesOnScreen / 2)].Position;
            var end = text.textEditor.scintilla1.Lines[currentLine + (linesOnScreen / 2)].Position;
            text.textEditor.scintilla1.ScrollRange(start, end);
        }

        public List<string> assFileRead(string filePath)
        {
            List<string> linesText = new List<string>();
            try
            {
                using (AssReader ass = new AssReader(filePath))
                {
                    foreach (AssReader.Line line in ass.ReadLines())
                    {
                        linesText.Add(line.text);
                    }
                }
            }
            catch
            {

            }
            return linesText;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                phrasesAndNamespaces = assFileRead(openFileDialog.FileName);

                foreach (string str in phrasesAndNamespaces)
                {
                    ListViewItem item = new ListViewItem(str);
                    listView1.Items.Add(item);
                }
            }
        }
    }
}
