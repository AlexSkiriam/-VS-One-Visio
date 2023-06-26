using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Visio = Microsoft.Office.Interop.Visio;

namespace _VS_One__Visio
{
    public partial class NewBuilderForm : Form
    {
        TextAnalysis textAnalysis = new TextAnalysis();
        UsingText usingText = new UsingText();
        PhrasesFunctions phrasesFunctions = new PhrasesFunctions();
        LanguageClass language = new LanguageClass();

        public Dictionary<string, Visio.Shape> dictShape;
        public Dictionary<string, Visio.Shape> dictShapeA;

        public NewBuilderForm()
        {
            InitializeComponent();

            this.Load += NewBuilderForm_Load;
            textBox1.KeyDown += TextBox_KeyDown;
            textBox2.KeyDown += TextBox2_KeyDown;
            textBox3.KeyDown += TextBox3_KeyDown;
            textBox4.KeyDown += TextBox4_KeyDown;
            textBox5.KeyDown += TextBox5_KeyDown;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switchlanguage(textBox1, e);
        }

        private void TextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            switchlanguage(textBox2, e);
        }

        private void TextBox3_KeyDown(object sender, KeyEventArgs e)
        {
            switchlanguage(textBox3, e);
        }

        private void TextBox4_KeyDown(object sender, KeyEventArgs e)
        {
            switchlanguage(textBox4, e);
        }

        private void TextBox5_KeyDown(object sender, KeyEventArgs e)
        {
            switchlanguage(textBox5, e);
        }

        public void switchlanguage(TextBox textBox, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.P) language.SelectedTextLanguageSwitcher(textBox);
        }

        private void NewBuilderForm_Load(object sender, EventArgs e)
        {
            listView1.FullRowSelect = true;
            listView1.MouseUp += new MouseEventHandler(listView1_MouseClick);
            setListElements();
        }

        public string getPhrase()
        {
            return (listView1.SelectedItems.Count == 1) ? listView1.SelectedItems[0].SubItems[2].Text : "";
        }

        public void setListElements()
        {
            listView1.Items.Clear();

            Visio.Shapes shapes = Globals.ThisAddIn.Application.ActivePage.Shapes;

            List<PhrasesArray> listOfPhrases = phrasesFunctions.getAllPhrases(shapes);

            dictShape = new Dictionary<string, Visio.Shape>();

            foreach (PhrasesArray phrase in listOfPhrases)
            {
                if (!dictShape.ContainsKey(phrase.phrase))
                {
                    ListViewItem item = new ListViewItem(phrase.phraseNumber.ToString());
                    item.SubItems.Add(phrase.phraseIntent);
                    item.SubItems.Add(phrase.phrase);
                    listView1.Items.Add(item);

                    dictShape.Add(phrase.phrase, phrase.phraseShape);
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

                    MenuItem toOF = new MenuItem("Поместить фразу в ОФ");
                    toOF.Click += delegate (object sender2, EventArgs e2) { copyToOF(); };
                    menu.MenuItems.Add(toOF);

                    MenuItem toFDP = new MenuItem("Поместить фразу в ФДП");
                    toFDP.Click += delegate (object sender2, EventArgs e2) { copyToFDP(); };
                    menu.MenuItems.Add(toFDP);

                    MenuItem copy = new MenuItem("Скопировать текст");
                    copy.Click += delegate (object sender2, EventArgs e2) { Clipboard.SetText(getPhrase()); };
                    menu.MenuItems.Add(copy);

                    MenuItem setClipboradTextSplited = new MenuItem("Создать текст для редактора (разделение параметрами)");
                    setClipboradTextSplited.Click += delegate (object sender2, EventArgs e2) { ToClipBoardSplited(); };
                    menu.MenuItems.Add(setClipboradTextSplited);

                    menu.Show(listView1, new Point(e.X, e.Y));
                }
            }
        }

        private void ToClipBoardSplited()
        {
            string splitedTExt = usingText.splittedText(getPhrase(), "");
            Clipboard.SetText(splitedTExt);
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                Form frm = Application.OpenForms["SearchForm"];

                if (frm == null)
                {
                    frm = new SearchForm(this, listView1, 2);
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

        private void button2_Click(object sender, EventArgs e)
        {
            setStateText();
        }

        private void copyToOF()
        {
            textBox3.Text = getPhrase();
        }

        private void copyToFDP()
        {
            textBox4.Text = getPhrase();
        }

        public void setStateText()
        {
            int tabsCount = trackBar1.Value;
            string tabs = usingText.tabsToReturn(tabsCount);
            string firstTab = checkBox3.Checked ? "" : tabs;

            string cmbTxt = (!String.IsNullOrEmpty(comboBox1.Text)) ? comboBox1.Text + "_for_" : "";
            string stateText = "";
            stateText = usingText.defaultStateText(cmbTxt, textBox1.Text, textBox2.Text, textBox5.Text, textBox3.Text, textBox4.Text, firstTab, tabs);
            Clipboard.SetText(stateText);
        }
    }
}
