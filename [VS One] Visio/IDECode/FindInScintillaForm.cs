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
using ScintillaNET;

namespace _VS_One__Visio
{
    public partial class FindInScintillaForm : Form
    {
        Scintilla scintilla;
        int prevPosition = 0;
        LanguageClass language = new LanguageClass();
        ScintillaFunc scintillaFunc = new ScintillaFunc();

        public FindInScintillaForm(Scintilla source)
        {
            InitializeComponent();

            scintilla = source;
            if (!String.IsNullOrEmpty(scintilla.SelectedText))
            {
                textBox1.Text = scintilla.SelectedText;
                prevPosition = scintilla.CurrentPosition;
            }
            AutoCompleteStringCollection autoComplete = new AutoCompleteStringCollection();
            autoComplete.AddRange(scintillaFunc.objectsInSpript(scintilla).ToArray());
            textBox1.AutoCompleteMode = AutoCompleteMode.Suggest;
            textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBox1.AutoCompleteCustomSource = autoComplete;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            findInText(scintilla);
        }

        private void findInText(Scintilla scintilla)
        {
            int currentPosition = 0;

            string text = (checkBox1.Checked) ? scintilla.Text : scintilla.Text.ToLower();
            string findText = (checkBox1.Checked) ? textBox1.Text : textBox1.Text.ToLower();

            if(checkBox2.Checked)
            {
                MatchCollection matches = Regex.Matches(text, @"\b" + findText + @"\b");
                foreach (Match match in matches)
                {
                    if (match.Index > prevPosition)
                    {
                        currentPosition = match.Index;
                        break;
                    }
                }
            }
            else if (checkBox3.Checked)
            {
                try
                {
                    MatchCollection matches = Regex.Matches(scintilla.Text, @textBox1.Text);
                    foreach (Match match in matches)
                    {
                        if (match.Index > prevPosition)
                        {
                            currentPosition = match.Index;
                            break;
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    MessageBox.Show("Ошибка в написании регулярного выражения!!!\n" + e.Message);
                }
            }
            else
            {
                currentPosition = text.IndexOf(findText, prevPosition);
            }
            prevPosition = currentPosition + textBox1.Text.Length;

            if (currentPosition > 0)
            {
                scintilla.GotoPosition(currentPosition);
                scintilla.SetSelection(currentPosition, currentPosition + textBox1.Text.Length);
                scintilla.Show();
            }
            else
            {
                MessageBox.Show("Найден последний элемент!");
            }
        }

        private void FindInScintillaForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) findInText(scintilla);
            if (e.KeyCode == Keys.Escape) Close();
            if (e.Control && e.KeyCode == Keys.P) language.SelectedTextLanguageSwitcher(textBox1);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox3.Checked) checkBox3.Checked = false;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) checkBox1.Checked = false;
        }
    }
}
