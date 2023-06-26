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
using keyw;
using System.Runtime.InteropServices;

namespace _VS_One__Visio
{
    public partial class BlockCodeTextEditor : Form
    {
        int lastCaretPos = 0;

        public string formResult = "";

        public bool setAllObjectTreeViewMode = true;
        public Scintilla otherScintilla = null;
        public Scintilla additionalScintilla = null;

        JsonParse parser = new JsonParse();
        LanguageClass language = new LanguageClass();

        public string storeStringClass = "";
        public string typesStringClass = "";
        ScintillaFunc scinitillaFunc = new ScintillaFunc();
        public List<string> autoCompliteStore = new List<string>();
        public List<string> autoCompliteTypes = new List<string>();
        ScinitillaStyles scinitillaStyles = new ScinitillaStyles();

        Stack<int> lines = new Stack<int>();

        public BlockCodeTextEditor()
        {
            InitializeComponent();

            lines.Push(1);

            scintilla1.Select();
            scinitillaStyles.scintillaSetInitStyleJson(scintilla1);
            scinitillaStyles.setLinks(scintilla1);
            //scinitillaStyles.setJsonDarkTheme(scintilla1);
            autocompleteMenu1.TargetControlWrapper = new ScintillaWrapper(scintilla1);
            BuildAutocompleteMenu();

            scinitillaStyles.scintillaSetInitStyleJson(scintilla2);
            //scinitillaStyles.setJsonDarkTheme(scintilla1);
            autocompleteMenu1.TargetControlWrapper = new ScintillaWrapper(scintilla2);
            BuildAutocompleteMenu();

            scintilla1.MouseDown += Scintilla1_MouseDown;
            scintilla1.InsertCheck += Scintilla1_InsertCheck;
            scintilla1.CharAdded += Scintilla1_CharAdded;
            scintilla1.KeyPress += Scintilla1_KeyPress;
            scintilla1.UpdateUI += Scintilla1_UpdateUI;
            scintilla1.DwellStart += scintilla1_DwellStart;
            scintilla1.DwellEnd += scintilla1_DwellEnd;

            scintilla1.MouseDwellTime = Properties.Settings.Default.ToolTipTime;

            scintilla1.MouseEnter += Scintilla1_MouseEnter;
            scintilla1.MouseLeave += Scintilla1_MouseLeave;

            panel2.Visible = false;
            panel2.MouseDown += panel2_MouseDown;
            panel2.MouseMove += panel2_MouseMove;

            scintilla1.HotspotClick += Scintilla1_HotspotClick;
            scintilla1.AnnotationVisible = Annotation.Boxed;

            scintilla2.HotspotClick += Scintilla2_HotspotClick;

            scintilla1.MouseDwellTime = Properties.Settings.Default.ToolTipTime;
        }

        private void Scintilla1_MouseEnter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UseHighLight) tableLayoutPanel2.BackColor = Properties.Settings.Default.HightLightColor;
        }

        private void Scintilla1_MouseLeave(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UseHighLight) tableLayoutPanel2.BackColor = SystemColors.Control;
        }

        private void Scintilla2_HotspotClick(object sender, HotspotClickEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                hotSpotClick(scintilla2, e);
            }
        }

        private void Scintilla1_HotspotClick(object sender, HotspotClickEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                hotSpotClick(scintilla1, e);
            }
            else if (otherScintilla != null && e.Modifiers == Keys.Shift)
            {
                hotSpotClick(otherScintilla, e);
            }
            else if (additionalScintilla != null && e.Modifiers == Keys.Control && e.Modifiers == Keys.Shift)
            {
                hotSpotClick(additionalScintilla, e);
            }
            else if (e.Modifiers == Keys.Alt)
            {
                int pos = scintilla1.LineFromPosition(e.Position);
                defenition(pos, DefenitionMode.HotSpots);
            }
        }

        private void hotSpotClick(Scintilla scintilla, HotspotClickEventArgs e)
        {
            List<string> listTokensStates = new List<string>() { "states", "global_listening_states", "special_states", "on_timer_states", };

            var json = JToken.Parse(parser.parseConverting(scintilla1.Text));
            int line = scintilla.LineFromPosition(e.Position);
            string lineText = scintilla.Lines[line].Text;
            lineText = Regex.Match(lineText, @"(?<=""\w*_state""\s*\:\s*"")[^""]+(?="")").Value;

            listTokensStates.ForEach(element => { selectingToken(element, json, scintilla1, lineText); });
            scintilla.ClearSelections();
        }

        private void TextEditorForm_Load(object sender, EventArgs e)
        {

        }

        private void scintilla1_DwellStart(object sender, DwellEventArgs e)
        {
            Point point = MousePosition;
            var cor = scintilla1.PointToClient(point);
            var pos = scintilla1.CharPositionFromPoint(cor.X, cor.Y);
            string word = (pos > -1) ? scintilla1.GetWordFromPosition(pos) : "";
            if (!String.IsNullOrEmpty(word))
            {
                try
                {
                    var json = JToken.Parse(parser.parseConverting(scintilla1.Text));
                    List<string> list = new List<string>();

                    var global_listening_states = json.SelectToken("global_listening_states");
                    foreach (var state in global_listening_states) list.AddRange(findUsage(state, word));

                    var special_states = json.SelectToken("special_states");
                    foreach (var state in special_states) list.AddRange(findUsage(state, word));

                    var states = json.SelectToken("states");
                    foreach (var state in states) list.AddRange(findUsage(state, word));

                    list = list.Distinct().ToList();

                    if (list.Count > 0)
                    {
                        string str = "Используется в следующих стейтах:\n\n";
                        list.ForEach(elem => { str += $"{elem}\n"; });
                        scintilla1.CallTipShow(e.Position, str);
                    }
                }
                catch
                {

                }
            }
        }

        private void scintilla1_DwellEnd(object sender, DwellEventArgs e)
        {
            scintilla1.CallTipCancel();
        }

        Point PanelMouseDownLocation;

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left) PanelMouseDownLocation = e.Location;
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                panel2.Left += e.X - PanelMouseDownLocation.X;
                panel2.Top += e.Y - PanelMouseDownLocation.Y;
            }

        }

        private List<string> findUsage(JToken token, string word)
        {
            List<string> list = new List<string>();
            List<string> patterns = new List<string>()
            {
                @"(?<=""\w*_state""\s*\:\s*"")[^""]+(?="")", @"(?<=""runtime""\s*\:\s*"")[^""]+(?="")",
                @"(?<=store\.)\b\w+\b", @"(?<=parameters\.)\b\w+\b", @"(?<=output\.)\b\w+\b", @"(?<=static_parameters\.)\b\w+\b"
            };

            patterns.ForEach(elem => { list.AddRange(returnFindUsageList(list, word, elem, token)); });

            return list;
        }

        private List<string> returnFindUsageList(List<string> sourceList, string word, string pattern, JToken token)
        {
            List<string> returnList = new List<string>();
            foreach (Match match in Regex.Matches(token.ToString(), @pattern))
            {
                if (match.Value == word && !sourceList.Contains(token.SelectToken("name").ToString())) returnList.Add(token.SelectToken("name").ToString());
            }
            return returnList;
        }

        private void findUsageForMenuItem(string word)
        {
            try
            {
                var json = JToken.Parse(parser.parseConverting(scintilla1.Text));
                List<string> list = new List<string>();

                var global_listening_states = json.SelectToken("global_listening_states");
                foreach (var state in global_listening_states) list.AddRange(findUsage(state, word));

                var special_states = json.SelectToken("special_states");
                foreach (var state in special_states) list.AddRange(findUsage(state, word));

                var states = json.SelectToken("states");
                foreach (var state in states) list.AddRange(findUsage(state, word));

                list = list.Distinct().ToList();

                if (list.Count > 0)
                {
                    string str = $"\"{word}\" используется в следующих стейтах:\n\n";
                    list.ForEach(elem => { str += $"\"next_state\": \"{elem}\"\n"; });
                    str += "\"next_state\": \"end\"(Костыль, а как без них :D)\n";
                    setAdditionalWindowStyle(AdditionalWindowMode.Show, str);
                    scinitillaStyles.setLinks(scintilla2);
                }
            }
            catch
            {

            }
        }

        private void Scintilla1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu menu = new ContextMenu();

                MenuItem cSharpInsert = new MenuItem("C# Вставка");
                cSharpInsert.Click += delegate (object sender2, EventArgs e2)
                {
                    CSharpCodeForm form = new CSharpCodeForm(this, storeStringClass, typesStringClass, autoCompliteStore, autoCompliteTypes);
                    form.ShowDialog();
                    int pos = scintilla1.LineFromPosition(scintilla1.CurrentPosition);
                    string tabs = scintilla1.Lines[pos].Text;
                    tabs = scinitillaFunc.tabBuilder(Regex.Match(tabs, @"^\t+").Captures.Count);
                    scintilla1.InsertText(scintilla1.CurrentPosition, scinitillaFunc.withInsertTabs(formResult, tabs));
                };
                menu.MenuItems.Add(cSharpInsert);

                MenuItem makeCompact = new MenuItem("Убрать лишние переносы строки в коде");
                makeCompact.Click += delegate (object sender2, EventArgs e2) { makeCSharpCodeCompact(scintilla1); };
                menu.MenuItems.Add(makeCompact);

                if (!String.IsNullOrEmpty(scintilla1.SelectedText))
                {
                    MenuItem findUsageItem = new MenuItem("Найти ссылки");
                    findUsageItem.Click += delegate (object sender2, EventArgs e2) { findUsageForMenuItem(scintilla1.SelectedText); };
                    menu.MenuItems.Add(findUsageItem);

                    MenuItem showDefenition = new MenuItem("Показать определение");
                    showDefenition.Click += delegate (object sender2, EventArgs e2) { defenition(scintilla1.LineFromPosition(scintilla1.CurrentPosition), DefenitionMode.Other); };
                    menu.MenuItems.Add(showDefenition);

                    MenuItem goToDefenition = new MenuItem("Перейти к определению");
                    goToDefenition.Click += delegate (object sender2, EventArgs e2) { findStore(scintilla1); };
                    menu.MenuItems.Add(goToDefenition);

                    MenuItem goToDefenitionOther = new MenuItem("Перейти к определению (в другом окне)");
                    goToDefenitionOther.Click += delegate (object sender2, EventArgs e2) { findStore(otherScintilla); };
                    menu.MenuItems.Add(goToDefenitionOther);

                    MenuItem changelayout = new MenuItem("Изменить раскладку");
                    changelayout.Click += delegate (object sender2, EventArgs e2) { changeLanguage(); };
                    menu.MenuItems.Add(changelayout);

                    MenuItem removeNewLines = new MenuItem("Склеить строки");
                    removeNewLines.Click += delegate (object sender2, EventArgs e2) { removeNewLinesAction(scintilla1); };
                    menu.MenuItems.Add(removeNewLines);
                }

                ((ScintillaNET.Scintilla)sender).ContextMenu = menu;
                menu.Show(scintilla1, e.Location);
            }
        }

        public void removeNewLinesAction(Scintilla scintilla)
        {
            string afterReplacementText = removeNewLineInString(scintilla.SelectedText);
            scintilla.ReplaceSelection(afterReplacementText);
        }

        public string removeNewLineInString(string source)
        {
            string newString = source.Replace("\r\n", " ").Replace("\t", "");
            newString = new PhrasesFunctions().cleanSurplusSpaces(newString);
            return newString;
        }

        public void makeCSharpCodeCompact(Scintilla scintilla)
        {
            string newText = Regex.Replace(scintilla.Text, @"<<<CODE(.(?!CODE))+.(?=CODE)CODE",
            elem => {
                return (Regex.Split(elem.Value, @"\r\n").Count() == 3) ? removeNewLineInString(elem.Value) : elem.Value;
            }, RegexOptions.Singleline);
            scintilla.Text = newText;
        }

        public void changeLanguage()
        {
            language.SelectedTextLanguageSwitcher(scintilla1);
        }

        public enum DefenitionMode
        {
            HotSpots,
            Other
        }

        public void defenition(int position, DefenitionMode m)
        {
            try
            {
                if (String.IsNullOrEmpty(scintilla1.Lines[position].AnnotationText) || scintilla1.Lines[position].AnnotationText == "-")
                {
                    string text = "";
                    var json = JToken.Parse(parser.parseConverting(scintilla1.Text));
                    string lineText = (m == DefenitionMode.HotSpots) ? scintilla1.Lines[position].Text : getTextForSearch();
                    if (m == DefenitionMode.HotSpots)
                    {
                        List<string> listTokensStates = new List<string>() { "states", "global_listening_states", "special_states", "on_timer_states", };
                        lineText = Regex.Match(lineText, @"(?<=""\w*_state""\s*\:\s*"")[^""]+(?="")").Value;

                        listTokensStates.ForEach(element =>
                        {
                            var tokens = json.Root.SelectToken(element).ToList();
                            tokens.Where(innerElement => innerElement.SelectToken("name") != null && innerElement.SelectToken("name").ToString() == lineText)
                                .ToList()
                                .ForEach(elem => { text += elem.ToString(); });
                        });
                    }
                    else
                    {
                        text = (!String.IsNullOrEmpty(lineText)) ? findObjects(lineText, json) : "";
                    }
                    setAdditionalWindowStyle(AdditionalWindowMode.Show, text);
                    //setAnnotationStyle(text, position);
                }
                else
                {
                    setAdditionalWindowStyle(AdditionalWindowMode.Hide);
                    //setAnnotationStyle("-", position);
                }

                //scintilla1.Lines[position].AnnotationStyle = 50;
            }
            catch
            {

            }
        }

        private string findObjects(string text, JToken json)
        {
            string returntext = "";
            var rootTypes = json.Root.SelectToken("types");
            if (rootTypes != null && rootTypes.SelectToken(text) != null) returntext = rootTypes.SelectToken(text).ToString();
            var rootObject = json.Root.SelectToken("objects");
            List<string> listTokens = new List<string>() { "store", "parameters", "output", "static_parameters" };
            listTokens.ForEach(elem =>
            {
                if (rootObject.SelectToken(elem) != null && rootObject.SelectToken(elem).SelectToken(text) != null)
                    returntext = rootObject.SelectToken(elem).SelectToken(text).ToString();
            });
            return returntext;
        }

        public enum AdditionalWindowMode
        {
            Show,
            Hide
        }

        private void setAdditionalWindowStyle(AdditionalWindowMode mode, string text = "")
        {
            switch(mode)
            {
                case AdditionalWindowMode.Show:
                    panel2.Visible = true;
                    scintilla2.Text = text;
                    panel2.Focus();
                    break;

                case AdditionalWindowMode.Hide:
                    panel2.Visible = false;
                    scintilla2.Text = "";
                    break;
            }
        }

        private void setAnnotationStyle(string text, int pos)
        {
            scintilla1.Lines[pos].AnnotationText = text;
            scintilla1.Styles[50].ForeColor = Color.DarkGreen;
            scintilla1.Styles[50].BackColor = Color.White;
        }

        public void findStore(Scintilla scintilla)
        {
            try
            {
                int pos = scintilla1.LineFromPosition(scintilla1.CurrentPosition);
                string lineText = getTextForSearch();

                var json = JToken.Parse(parser.parseConverting(scintilla1.Text));
                var rootObject = json.Root.SelectToken("objects");
                var rootTypes = json.Root.SelectToken("types");

                if (rootTypes.SelectToken(lineText) != null ||
                    rootObject.SelectToken("store").SelectToken(lineText) != null ||
                    rootObject.SelectToken("parameters").SelectToken(lineText) != null ||
                    rootObject.SelectToken("output").SelectToken(lineText) != null ||
                    rootObject.SelectToken("static_parameters").SelectToken(lineText) != null)
                {
                    goToPosition(scintilla, $"\"{lineText}");
                }
            }
            catch
            {

            }
        }

        public string getTextForSearch()
        {
            string lineText = "";
            if (String.IsNullOrEmpty(scintilla1.SelectedText))
            {
                int pos = scintilla1.LineFromPosition(scintilla1.CurrentPosition);
                lineText = scintilla1.Lines[pos].Text;
                lineText = Regex.Match(lineText, @"(?<=store\.)\w+").Value;
            }
            else
            {
                lineText = scintilla1.SelectedText.Replace("store.", "");
            }

            return lineText;
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
            //Строка: 0 Позиция: 0
            if ((e.Change & UpdateChange.Selection) > 0)
            {
                var currentPos = scintilla1.CurrentPosition;
                lines.Push(scintilla1.LineFromPosition(currentPos));
                var currentLinePos = scintilla1.LineFromPosition(currentPos) + 1;
                var linePosition = currentPos - scintilla1.Lines[scintilla1.LineFromPosition(currentPos)].Position;
                toolStripLabel1.Text = "Строка: " + currentLinePos + " Позиция: " + (linePosition + 1);
            }
        }

        private void selectingToken(string findToken, JToken json, ScintillaNET.Scintilla scintilla, string textToFind)
        {
            var tokens = json.Root.SelectToken(findToken).ToList();
            tokens.Where(element =>
                element.SelectToken("name") != null &&
                element.SelectToken("name").ToString() == textToFind)
            .ToList()
            .ForEach(elem =>
            {
                int sceondIndex = (elem.ToString().Length >= textToFind.Length + 10) ? textToFind.Length + 10 : elem.ToString().Length;
                string nodeText = elem.ToString().Substring(5, sceondIndex);
                goToPosition(scintilla, nodeText);
            });
        }

        private void goToPosition(ScintillaNET.Scintilla scintilla, string nodeText)
        {
            int position = scintilla.Text.IndexOf(nodeText);
            if (position > 0)
            {
                var linesOnScreen = scintilla.LinesOnScreen - 2;

                var start = scintilla.Lines[scintilla.LineFromPosition(position) - (linesOnScreen / 2)].Position;
                var end = scintilla.Lines[scintilla.LineFromPosition(position) + (linesOnScreen / 2)].Position;
                scintilla.ScrollRange(start, end);
                scintilla.SetSelection(position, position + nodeText.Length);
            }
        }

        public string openEvent()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.AutoUpgradeEnabled = true;
            dialog.Filter = "Текстовые файлы (*.txt)|*.txt";
            DialogResult result = dialog.ShowDialog();

            string userPathValue = (result == DialogResult.OK) ? dialog.FileName : "";

            if (!String.IsNullOrEmpty(userPathValue) && File.Exists(userPathValue))
            {
                scintilla1.Text = File.ReadAllText(userPathValue);
            }
            return userPathValue;
        }

        public void savingEvent(string str = "")
        {
            string filename = "";
            if (String.IsNullOrEmpty(str))
            {
                SaveFileDialog dialog = new SaveFileDialog();

                dialog.Filter = "Текстовые файлы (*.txt)|*.txt";
                dialog.FilterIndex = 2;
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.Cancel) return;
                filename = dialog.FileName;
            }
            else filename = str;

            File.WriteAllText(filename, scintilla1.Text);
        }

        /* Keywords */
        private const string defaultState =
            "name next_state display_name require_push";
        private const string speechState =
            "speech text namespace";
        private const string listeningState =
            "settings max_hypotheses_to_check incomprehensible_state silence_length_seconds noise_length_seconds"
            + " long_silence_state long_noise_state phrases_conditions basic_phrase phrases_parts phrases_exact phrases_parts_exclude";
        private const string conditionsState =
            "conditions condition always_true on_enter on_match";

        private const string cSharp = "";
        private const string typesCSharp = "string int double List<> Dictionary<> DateTime() String.IsNullOrEmpty()";

        private void BuildAutocompleteMenu()
        {
            autocompleteMenu1.SetAutocompleteItems(new DynamicCollection(scintilla1));

            autocompleteMenu1.AllowsTabKey = true;
        }
        private static List<string> BuildAutoShowList()
        {
            List<string> list = new List<string>();

            list.AddRange(defaultState.Split(' '));
            list.AddRange(speechState.Split(' '));
            list.AddRange(listeningState.Split(' '));
            list.AddRange(conditionsState.Split(' '));
            list.AddRange(cSharp.Split(' '));
            list.AddRange(typesCSharp.Split(' '));

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

                foreach (Match m in Regex.Matches(tb.Text, @"store\.\w+"))
                    words[m.Value] = m.Value;

                BuildAutoShowList().ForEach(elem => { words[elem] = elem; });

                //return autocomplete items
                foreach (var word in words.Keys)
                    yield return new AutocompleteItem(word);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        private void TextEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form frm = Application.OpenForms["FindInScintillaForm"];

            if (frm == null) frm.Close();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (lines.Count > 0)
            {
                int posP = lines.Pop();
                scintilla1.Lines[lines.Peek()].Goto();
                lines.Push(posP);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            scintilla1.Lines[lines.Peek()].Goto();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }

        private int documentCounter = 1;

        private void button2_Click(object sender, EventArgs e)
        {
            documentCounter++;
            scintilla2.Text = (documentCounter % 2 == 0) ? scintilla1.Text : "";
            string mode = (documentCounter % 2 == 0) ? "Весь документ (Только для чтения)" : "Переход к определению";
            scintilla2.ReadOnly = (documentCounter % 2 == 0);
            label1.Text = $"Режим:{mode}";
        }

        int prevPosition = 0;

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                prevPosition = scinitillaFunc.findTextInScintilla(scintilla2, textBox1.Text, prevPosition);
            }
        }
    }
}
