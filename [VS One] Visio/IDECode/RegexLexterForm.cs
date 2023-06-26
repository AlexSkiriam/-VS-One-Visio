using AutocompleteMenuNS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _VS_One__Visio
{
    public partial class RegexLexterForm : Form
    {
        TextEditorForm editor;
        int lastCaretPos = 0;

        JsonParse parser = new JsonParse();

        ScintillaFunc scinitillaFunc = new ScintillaFunc();
        ScinitillaStyles scinitillaStyles = new ScinitillaStyles();

        public RegexLexterForm(TextEditorForm source)
        {
            InitializeComponent();

            scintilla1.Select();
            scinitillaStyles.scintillaSetInitStyleJson(scintilla1);
            autocompleteMenu1.TargetControlWrapper = new ScintillaWrapper(scintilla1);
            BuildAutocompleteMenu();

            scintilla1.MouseDown += Scintilla1_MouseDown;
            scintilla1.InsertCheck += Scintilla1_InsertCheck;
            scintilla1.CharAdded += Scintilla1_CharAdded;
            scintilla1.KeyPress += Scintilla1_KeyPress;
            scintilla1.UpdateUI += Scintilla1_UpdateUI;
            scintilla1.KeyDown += scintilla1_KeyDown;
        }

        private void Scintilla1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu menu = new ContextMenu();

                MenuItem anyWord = new MenuItem("Любое слово .");
                anyWord.Click += delegate { scintilla1.InsertText(scintilla1.CurrentPosition, "."); };
                menu.MenuItems.Add(anyWord);

                MenuItem array = new MenuItem("Одно из слов []");
                array.Click += delegate { scintilla1.InsertText(scintilla1.CurrentPosition, "[]"); };
                menu.MenuItems.Add(array);

                MenuItem group = new MenuItem("Все слова внутри ()");
                group.Click += delegate { scintilla1.InsertText(scintilla1.CurrentPosition, "()"); };
                menu.MenuItems.Add(group);

                MenuItem oneOf = new MenuItem("Одно из выражений |");
                oneOf.Click += delegate { scintilla1.InsertText(scintilla1.CurrentPosition, "|"); };
                menu.MenuItems.Add(oneOf);

                MenuItem zeroAll = new MenuItem("Ноль или более вхождений *");
                zeroAll.Click += delegate { scintilla1.InsertText(scintilla1.CurrentPosition, "*"); };
                menu.MenuItems.Add(zeroAll);

                MenuItem oneAll = new MenuItem("Одно или более вхождений +");
                oneAll.Click += delegate { scintilla1.InsertText(scintilla1.CurrentPosition, "+"); };
                menu.MenuItems.Add(oneAll);

                MenuItem zeroOne = new MenuItem("Ноль или одно вхождений ?");
                zeroOne.Click += delegate { scintilla1.InsertText(scintilla1.CurrentPosition, "?"); };
                menu.MenuItems.Add(zeroOne);

                MenuItem some = new MenuItem("От n до m слов {n,m}");
                some.Click += delegate { scintilla1.InsertText(scintilla1.CurrentPosition, "{0,3}"); };
                menu.MenuItems.Add(some);

                MenuItem wordPart = new MenuItem("Произвольная часть слова ~");
                wordPart.Click += delegate { scintilla1.InsertText(scintilla1.CurrentPosition, "~"); };
                menu.MenuItems.Add(wordPart);

                MenuItem wordForm = new MenuItem("Все словоформы слова @");
                wordForm.Click += delegate { scintilla1.InsertText(scintilla1.CurrentPosition, "@"); };
                menu.MenuItems.Add(wordForm);

                ((ScintillaNET.Scintilla)sender).ContextMenu = menu;
                menu.Show(scintilla1, e.Location);
            }
        }

        private void Scintilla1_InsertCheck(object sender, ScintillaNET.InsertCheckEventArgs e)
        {
            scinitillaFunc.insertCheck(sender, e, scintilla1);
        }

        private void Scintilla1_CharAdded(object sender, ScintillaNET.CharAddedEventArgs e)
        {
            scinitillaFunc.InsertMatchedChars(scintilla1, e);
        }

        private void Scintilla1_KeyPress(object sender, KeyPressEventArgs e)
        {
            scinitillaFunc.keyPress(sender, e);
        }
        private void Scintilla1_UpdateUI(object sender, ScintillaNET.UpdateUIEventArgs e)
        {
            scinitillaFunc.uiUpdate(sender, e, lastCaretPos);
        }

        private void scintilla1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                string text = "{\"phrases\":[" + scintilla1.Text + "]}";
                parser.jsonRegexFormValidate(text);
            }
        }

        private void BuildAutocompleteMenu()
        {
            autocompleteMenu1.SetAutocompleteItems(new DynamicCollection(scintilla1));

            autocompleteMenu1.AllowsTabKey = true;
        }
        private static List<string> BuildAutoShowList()
        {
            List<string> list = new List<string>();
            return list.OrderBy(m => m).ToList();
        }

        internal class DynamicCollection : IEnumerable<AutocompleteItem>
        {
            private ScintillaNET.Scintilla tb;

            public DynamicCollection(ScintillaNET.Scintilla tb)
            {
                this.tb = tb;
            }

            public IEnumerator<AutocompleteItem> GetEnumerator()
            {
                return BuildList().GetEnumerator();
            }

            private IEnumerable<AutocompleteItem> BuildList()
            {
                var words = new Dictionary<string, string>();
                foreach (Match m in Regex.Matches(tb.Text, @"\b\w+\b"))
                    words[m.Value] = m.Value;

                foreach (var item in BuildAutoShowList())
                    words[item] = item;

                //return autocomplete items
                foreach (var word in words.Keys)
                    yield return new AutocompleteItem(word);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
