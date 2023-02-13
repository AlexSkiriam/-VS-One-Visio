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
    public partial class TextEditorForm : Form
    {
        int lastCaretPos = 0;

        public string formResult = "";

        public bool setAllObjectTreeViewMode = true;

        JsonParse parser = new JsonParse();

        public string storeStringClass = "";
        public string typesStringClass = "";
        ScintillaFunc scinitillaFunc = new ScintillaFunc();
        public List<string> autoCompliteStore = new List<string>();
        public List<string> autoCompliteTypes = new List<string>();
        ScinitillaStyles scinitillaStyles = new ScinitillaStyles();

        public TextEditorForm()
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

            treeView1.AfterSelect += treeView1_AfterSelect;
            treeView1.MouseClick += new MouseEventHandler(treeView1_MouseClick);
        }

        private void TextEditorForm_Load(object sender, EventArgs e)
        {

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
                    string tabs = scintilla1.LineFromPosition(scintilla1.CurrentPosition).ToString();
                    tabs = scinitillaFunc.tabBuilder(Regex.Match(tabs, @"^\t+").Captures.Count);
                    scintilla1.InsertText(scintilla1.CurrentPosition, scinitillaFunc.withInsertTabs(formResult, tabs));
                };
                menu.MenuItems.Add(cSharpInsert);

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

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip menu = new ContextMenuStrip();

                ToolStripMenuItem copyText = new ToolStripMenuItem("Копировать текст");
                copyText.Click += delegate { if(treeView1.SelectedNode != null) Clipboard.SetText(treeView1.SelectedNode.Text); };

                ToolStripMenuItem copyListOfElements = new ToolStripMenuItem("Скопировать список вложенных элементов");
                copyListOfElements.Click += delegate 
                {
                    string elements = "";
                    if (treeView1.SelectedNode != null && treeView1.SelectedNode.Nodes.Count > 0)
                    {
                        foreach(TreeNode nodes in treeView1.SelectedNode.Nodes)
                        {
                            elements += $"{nodes.Text}\n";
                        }
                    }
                    Clipboard.SetText(elements);
                };

                menu.Items.AddRange(new ToolStripItem[] { copyText, copyListOfElements });

                treeView1.ContextMenuStrip = menu;
            }
        }

        private bool cancel = false;

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
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
                        goToPosition(scintilla1, json.Root.SelectToken(treeView1.SelectedNode.Text).Parent.ToString().Substring(0, treeView1.SelectedNode.Text.Length + 3));
                    }
                    else
                    {
                        foreach (string str in listTokens)
                        {
                            if (json.Root.SelectToken(str) != null && json.Root.SelectToken(str).SelectToken(treeView1.SelectedNode.Text) != null)
                                goToPosition(scintilla1, json.Root.SelectToken(str).SelectToken(treeView1.SelectedNode.Text).Parent.ToString().Substring(0, treeView1.SelectedNode.Text.Length + 3));
                        }
                        foreach (string str in listTokensStates)
                        {
                            selectingToken(str, json, scintilla1);
                        }
                    }
                }
                cancel = false;
            }
            catch
            {

            }
        }

        private void selectingToken(string findToken, JToken json, ScintillaNET.Scintilla scintilla)
        {
            foreach (var tok in json.Root.SelectToken(findToken))
            {
                if (tok.SelectToken("name") != null  && tok.SelectToken("name").ToString() == treeView1.SelectedNode.Text)
                    goToPosition(scintilla, tok.ToString().Substring(5, (tok.ToString().Length >= treeView1.SelectedNode.Text.Length + 10)? treeView1.SelectedNode.Text.Length + 10 : tok.ToString().Length));
            }
        }

        private void goToPosition(ScintillaNET.Scintilla scintilla, string nodeText)
        {
            int position = scintilla.Text.IndexOf(nodeText);
            if (position > 0)
            {
                var linesOnScreen = scintilla.LinesOnScreen - 2;

                var start = scintilla.Lines[scintilla.LineFromPosition(position) - (linesOnScreen / 2)].Position;
                var  end = scintilla.Lines[scintilla.LineFromPosition(position) + (linesOnScreen / 2)].Position;
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
            MessageBox.Show("Файл сохранен");
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

        private void TextEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form frm = Application.OpenForms["FindInScintillaForm"];

            if (frm == null) frm.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox1.Text))
            {
                FindRecursive(treeView1.Nodes[0], textBox1.Text, false);
            } 
            else
            {
                clearRecursive(treeView1.Nodes[0]);
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
            foreach(TreeNode node in treeNode.Nodes)
            {
                node.Collapse();
                node.BackColor = Color.White;
                clearRecursive(node);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
