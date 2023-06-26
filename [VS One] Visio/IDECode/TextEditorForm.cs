using AutocompleteMenuNS;
using Newtonsoft.Json.Linq;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace _VS_One__Visio
{
    public partial class TextEditorForm : Form
    {
        int lastCaretPos = 0;

        public string formResult = "";

        public bool setAllObjectTreeViewMode = true;
        public Scintilla otherScintilla = null;

        JsonParse parser = new JsonParse();
        LanguageClass language = new LanguageClass();

        public string storeStringClass = "";
        public string typesStringClass = "";
        ScintillaFunc scinitillaFunc = new ScintillaFunc();
        public List<string> autoCompliteStore = new List<string>();
        public List<string> autoCompliteTypes = new List<string>();
        ScinitillaStyles scinitillaStyles = new ScinitillaStyles();

        Stack<int> lines = new Stack<int>();

        public TextEditorForm()
        {
            InitializeComponent();

            lines.Push(1);

            scintilla1.Select();
            scinitillaStyles.scintillaSetInitStyleJson(scintilla1);
            scinitillaStyles.setLinks(scintilla1);
            autocompleteMenu1.TargetControlWrapper = new ScintillaWrapper(scintilla1);
            BuildAutocompleteMenu();

            scintilla1.MouseDown += Scintilla1_MouseDown;
            scintilla1.InsertCheck += Scintilla1_InsertCheck;
            scintilla1.CharAdded += Scintilla1_CharAdded;
            scintilla1.KeyPress += Scintilla1_KeyPress;
            scintilla1.UpdateUI += Scintilla1_UpdateUI;

            treeView1.AfterSelect += treeView1_AfterSelect;
            treeView1.MouseClick += new MouseEventHandler(treeView1_MouseClick);

            scintilla1.HotspotClick += Scintilla1_HotspotClick;
            scintilla1.AnnotationVisible = Annotation.Boxed;

            scintilla1.MouseDwellTime = Properties.Settings.Default.ToolTipTime;
        }

        private void ShowToolTip(object sender, string message)
        {
            //new ToolTip().Show(message, this, Cursor.Position.X, Cursor.Position.Y, 1000);
        }

        private void Scintilla1_HotspotClick(object sender, HotspotClickEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                List<string> listTokensStates = new List<string>() { "states", "global_listening_states", "special_states", "on_timer_states", };

                var json = JToken.Parse(parser.parseConverting(scintilla1.Text));
                int line = scintilla1.LineFromPosition(e.Position);
                lines.Push(line);
                string lineText = scintilla1.Lines[line].Text;
                lineText = Regex.Match(lineText, @"(?<=""\w*_state""\s*\:\s*"")[^""]+(?="")").Value;

                listTokensStates.ForEach(element => { selectingToken(element, json, scintilla1, lineText); });
                scintilla1.ClearSelections();
            }
            else if (otherScintilla != null && e.Modifiers == Keys.Shift)
            {
                List<string> listTokensStates = new List<string>() { "states", "global_listening_states", "special_states", "on_timer_states", };

                var json = JToken.Parse(parser.parseConverting(otherScintilla.Text));
                int line = otherScintilla.LineFromPosition(e.Position);
                string lineText = otherScintilla.Lines[line].Text;
                lineText = Regex.Match(lineText, @"(?<=""\w*_state""\s*\:\s*"")[^""]+(?="")").Value;

                listTokensStates.ForEach(element => { selectingToken(element, json, otherScintilla, lineText); });
                otherScintilla.ClearSelections();
            }
            else if (e.Modifiers == Keys.Alt)
            {
                int pos = scintilla1.LineFromPosition(e.Position);
                if (String.IsNullOrEmpty(scintilla1.Lines[pos].AnnotationText) || scintilla1.Lines[pos].AnnotationText == "-")
                {
                    List<string> listTokensStates = new List<string>() { "states", "global_listening_states", "special_states", "on_timer_states", };

                    var json = JToken.Parse(parser.parseConverting(scintilla1.Text));
                    string lineText = scintilla1.Lines[pos].Text;
                    lineText = Regex.Match(lineText, @"(?<=""\w*_state""\s*\:\s*"")[^""]+(?="")").Value;

                    listTokensStates.ForEach(element =>
                    {
                        var tokens = json.Root.SelectToken(element).ToList();
                        tokens.Where(innerElement =>
                            innerElement.SelectToken("name") != null &&
                            innerElement.SelectToken("name").ToString() == lineText)
                        .ToList()
                        .ForEach(elem => { scintilla1.Lines[pos].AnnotationText = elem.ToString(); });
                    });
                    scintilla1.Styles[50].ForeColor = Color.DarkGreen;
                    scintilla1.Styles[50].BackColor = Color.White;
                }
                else
                {
                    scintilla1.Lines[pos].AnnotationText = "-";
                    scintilla1.Styles[50].ForeColor = Color.DarkGreen;
                    scintilla1.Styles[50].BackColor = Color.White;
                }

                scintilla1.Lines[pos].AnnotationStyle = 50;
            }
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

        private List<string> findUsage(JToken token, string word)
        {
            List<string> list = new List<string>();

            foreach (Match match in Regex.Matches(token.ToString(), @"(?<=""\w*_state""\s*\:\s*"")[^""]+(?="")"))
            {
                if (match.Value == word && !list.Contains(token.SelectToken("name").ToString())) list.Add(token.SelectToken("name").ToString());
            }
            foreach (Match match in Regex.Matches(token.ToString(), @"(?<=""runtime""\s*\:\s*"")[^""]+(?="")"))
            {
                if (match.Value == word && !list.Contains(token.SelectToken("name").ToString())) list.Add(token.SelectToken("name").ToString());
            }

            return list;
        }

        private void scintilla1_DwellEnd(object sender, DwellEventArgs e)
        {
            scintilla1.CallTipCancel();
        }

        private void Scintilla1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu menu = new ContextMenu();

                /*MenuItem cSharpInsert = new MenuItem("C# Вставка");
                cSharpInsert.Click += delegate (object sender2, EventArgs e2)
                {
                    CSharpCodeForm form = new CSharpCodeForm(this, storeStringClass, typesStringClass, autoCompliteStore, autoCompliteTypes);
                    form.ShowDialog();
                    int pos = scintilla1.LineFromPosition(scintilla1.CurrentPosition);
                    string tabs = scintilla1.Lines[pos].Text;
                    tabs = scinitillaFunc.tabBuilder(Regex.Match(tabs, @"^\t+").Captures.Count);
                    scintilla1.InsertText(scintilla1.CurrentPosition, scinitillaFunc.withInsertTabs(formResult, tabs));
                };
                menu.MenuItems.Add(cSharpInsert);*/

                if (!String.IsNullOrEmpty(scintilla1.SelectedText))
                {
                    MenuItem showDefenition = new MenuItem("Показать определение");
                    showDefenition.Click += delegate (object sender2, EventArgs e2) { defenition(); };
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
                }

                ((ScintillaNET.Scintilla)sender).ContextMenu = menu;
                menu.Show(scintilla1, e.Location);
            }
        }

        public void changeLanguage()
        {
            language.SelectedTextLanguageSwitcher(scintilla1);
        }

        public void defenition()
        {
            try
            {
                int pos = scintilla1.LineFromPosition(scintilla1.CurrentPosition);
                string lineText = getTextForSearch();

                if (String.IsNullOrEmpty(scintilla1.Lines[pos].AnnotationText) || scintilla1.Lines[pos].AnnotationText == "-")
                {
                    var json = JToken.Parse(parser.parseConverting(scintilla1.Text));

                    var listTokensObjects = (!String.IsNullOrEmpty(lineText)) ?
                        json.Root.SelectToken("objects").SelectToken("store").SelectToken(lineText).ToString() : "";

                    setAnnotationStyle(listTokensObjects, pos);
                }
                else
                {
                    setAnnotationStyle("-", pos);
                }

                scintilla1.Lines[pos].AnnotationStyle = 50;
            }
            catch
            {

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

                if (json.Root.SelectToken("objects").SelectToken("store").SelectToken(lineText) != null)
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

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip menu = new ContextMenuStrip();

                ToolStripMenuItem goToOther = new ToolStripMenuItem("Перейти в другом окне");
                goToOther.Click += delegate { if (treeView1.SelectedNode != null) selectingNode(otherScintilla); };

                ToolStripMenuItem copyText = new ToolStripMenuItem("Копировать текст");
                copyText.Click += delegate { if (treeView1.SelectedNode != null) Clipboard.SetText(treeView1.SelectedNode.Text); };

                ToolStripMenuItem copyListOfElements = new ToolStripMenuItem("Скопировать список вложенных элементов");
                copyListOfElements.Click += delegate
                {
                    string elements = "";
                    if (treeView1.SelectedNode != null && treeView1.SelectedNode.Nodes.Count > 0)
                    {
                        foreach (TreeNode nodes in treeView1.SelectedNode.Nodes)
                        {
                            elements += $"{nodes.Text}\n";
                        }
                    }
                    Clipboard.SetText(elements);
                };
                ToolStripMenuItem compare = new ToolStripMenuItem("Сравнить правила");
                compare.Click += delegate { findDupl(); };

                ToolStripMenuItem createBasic = new ToolStripMenuItem("Создать basic_phrase из словарей");
                createBasic.Click += delegate { setBasicPhrase(); };

                menu.Items.AddRange(new ToolStripItem[] { goToOther, copyText, copyListOfElements, compare, createBasic });

                treeView1.ContextMenuStrip = menu;
            }
        }

        private void setBasicPhrase()
        {
            try
            {
                var json = JToken.Parse(parser.parseConverting(scintilla1.Text));
                string elements = "";
                if (treeView1.SelectedNode != null && treeView1.SelectedNode.Nodes.Count > 0 && treeView1.SelectedNode.Text == "phrases")
                {
                    foreach (TreeNode nodes in treeView1.SelectedNode.Nodes)
                    {
                        var rules = json.SelectToken("phrases").SelectToken(nodes.Text);

                        string rule = (rules.Type == JTokenType.Object && rules.SelectToken("display_name") != null) ?
                                ((JValue)rules.SelectToken("display_name")).Value.ToString() : "";

                        elements += "{\r\n\t\"basic_phrase\": \"" + rule +
                            "\",\r\n\t\"phrases_parts\": [ \"" +
                            nodes.Text +
                            "\" ],\r\n\t\"next_state\": \"end\"\r\n},\r\n";
                    }
                }
                BasicPhraseBuldier basic = new BasicPhraseBuldier(elements);
                basic.ShowDialog();
            }
            catch
            {

            }
        }

        private void findDupl()
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Parent.Text == "phrases")
            {
                LevinsteinDistanceClass levinstein = new LevinsteinDistanceClass();

                var json = JToken.Parse(parser.parseConverting(scintilla1.Text));
                var rule = json.SelectToken("phrases").SelectToken(treeView1.SelectedNode.Text);
                List<string> list = new List<string>();

                if (rule.Type == JTokenType.Object)
                    rule.SelectToken("phrases").ToList().ForEach(element => { list.Add(((JValue)element).Value.ToString()); });
                else
                    rule.ToList().ForEach(element => { list.Add(((JValue)element).Value.ToString()); });
                string error = "";

                for (int i = list.Count - 1; i >= 0; i--)
                {
                    string str = list[i];
                    list.RemoveAt(i);
                    error += levinstein.findWithLevinstainInList(str, list) + "\n";
                }

                if (!String.IsNullOrWhiteSpace(error)) MessageBox.Show(error);
                else MessageBox.Show("Нет дублей");
            }
        }

        private bool cancel = false;

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Scintilla scin = (Control.ModifierKeys == Keys.Control) ? otherScintilla : scintilla1;
            selectingNode(scin);
        }

        private void selectingNode(Scintilla scintilla)
        {
            try
            {
                if (treeView1.SelectedNode != null && !cancel)
                {
                    var json = JToken.Parse(parser.parseConverting(scintilla1.Text));

                    List<string> listTokens = new List<string>()
                    {
                        "objects", "enums", "types", "objects",
                        "production", "tts_phrases", "tts_phrases_runtime", "expressions", "phrases", "external_actions"
                    };

                    List<string> listTokensStates = new List<string>()
                    {
                        "states", "global_listening_states", "special_states", "on_timer_states",
                    };

                    if (json.Root.SelectToken(treeView1.SelectedNode.Text) != null)
                    {
                        int secondPosition = treeView1.SelectedNode.Text.Length + 3;
                        string nodeText = json.Root.SelectToken(treeView1.SelectedNode.Text).Parent.ToString().Substring(0, secondPosition);
                        goToPosition(scintilla, nodeText);
                    }
                    else
                    {
                        listTokens.Where(element =>
                                json.Root.SelectToken(element) != null &&
                                json.Root.SelectToken(element).SelectToken(treeView1.SelectedNode.Text) != null)
                        .ToList()
                        .ForEach(elem =>
                        {
                            int secondPosition = treeView1.SelectedNode.Text.Length + 3;
                            string nodeText = json.Root.SelectToken(elem).SelectToken(treeView1.SelectedNode.Text).Parent.ToString().Substring(0, secondPosition);
                            goToPosition(scintilla, nodeText);
                        });
                        listTokensStates.ForEach(element => { selectingToken(element, json, scintilla, treeView1.SelectedNode.Text); });
                    }
                }
                cancel = false;
            }
            catch
            {

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
            //validateEvent();
        }

        public void validateEvent()
        {
            var log = parser.parseJson(scintilla1.Text, scintilla1);
            if (log == null)
            {
                if (setAllObjectTreeViewMode)
                    treeView1.SetObjectAsJson(JToken.Parse(parser.parseConverting(scintilla1.Text)));
                else
                    treeView1.SetObjectAsJson(JToken.Parse(scinitillaFunc.getOnlyPhraseObjects(parser.parseConverting(scintilla1.Text))));
                treeView1.Nodes[0].Expand();
                storeStringClass = scinitillaFunc.storeBuilder(scintilla1, autoCompliteStore);
                typesStringClass = scinitillaFunc.userTypeBuilder(scintilla1, autoCompliteTypes);
            }
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
            //MessageBox.Show("Файл сохранен");
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(textBox1.Text)) FindRecursive(treeView1.Nodes[0], textBox1.Text, false);
                else clearRecursive(treeView1.Nodes[0]);
            }
            catch
            {

            }
        }

        private void FindRecursive(TreeNode treeNode, string searchString, Boolean foundFirst)
        {
            Boolean found = foundFirst;
            foreach (TreeNode tn in treeNode.Nodes)
            {
                tn.BackColor = Color.White;
                if (tn.Text.Contains(searchString) && (!found))
                {
                    found = true;
                    tn.BackColor = Color.LightGreen;
                    if (Regex.IsMatch(treeNode.Text, @"(states|global_listening_states|special_states|on_timer_states)"))
                    {
                        cancel = true;
                        treeView1.SelectedNode = tn;
                    }
                }
                FindRecursive(tn, searchString, found);
            }
        }

        public void clearRecursive(TreeNode treeNode)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Collapse();
                node.BackColor = Color.White;
                clearRecursive(node);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.P) language.SelectedTextLanguageSwitcher(textBox1);
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
    }
}
