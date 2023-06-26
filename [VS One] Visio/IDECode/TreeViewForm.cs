using Newtonsoft.Json.Linq;
using ScintillaNET;
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
    public partial class TreeViewForm : Form
    {
        JsonParse parser = new JsonParse();
        LanguageClass language = new LanguageClass();

        public Scintilla mainScintilla = null;
        public Scintilla otherScintilla = null;
        public Scintilla additionalScintilla = null;

        public TreeViewForm()
        {
            InitializeComponent();

            textBox1.TextChanged += textBox1_TextChanged;

            treeView1.AfterSelect += treeView1_AfterSelect;
            treeView1.NodeMouseClick += treeView1_NodeMouseClick;
            treeView1.MouseClick += new MouseEventHandler(treeView1_MouseClick);
        }

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip menu = new ContextMenuStrip();

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

                menu.Items.AddRange(new ToolStripItem[] { copyText, copyListOfElements, compare, createBasic });

                treeView1.ContextMenuStrip = menu;
            }
        }

        private void setBasicPhrase()
        {
            try
            {
                var json = JToken.Parse(parser.parseConverting(mainScintilla.Text));
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

                var json = JToken.Parse(parser.parseConverting(mainScintilla.Text));
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
            Scintilla scin = null;
            if (Control.ModifierKeys == Keys.Alt) scin = additionalScintilla;
            else if (Control.ModifierKeys == Keys.Control) scin = otherScintilla;
            else scin = mainScintilla;
            //selectingNode(scin);
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            Scintilla scin = null;
            if (Control.ModifierKeys == Keys.Alt) scin = additionalScintilla;
            else if (Control.ModifierKeys == Keys.Control) scin = otherScintilla;
            else scin = mainScintilla;
            selectingNode(scin, e.Node);
        }

        private void selectingNode(Scintilla scintilla, TreeNode node)
        {
            try
            {
                if (node != null && !cancel)
                {
                    var json = JToken.Parse(parser.parseConverting(mainScintilla.Text));

                    List<string> listTokens = new List<string>()
                    {
                        "objects", "enums", "types", "objects",
                        "production", "tts_phrases", "tts_phrases_runtime", "expressions", "phrases", "external_actions"
                    };

                    List<string> listTokensStates = new List<string>()
                    {
                        "states", "global_listening_states", "special_states", "on_timer_states",
                    };

                    if (json.Root.SelectToken(node.Text) != null)
                    {
                        int secondPosition = node.Text.Length + 3;
                        string nodeText = json.Root.SelectToken(node.Text).Parent.ToString().Substring(0, secondPosition);
                        goToPosition(scintilla, nodeText);
                    }
                    else
                    {
                        listTokens.Where(element =>
                                json.Root.SelectToken(element) != null &&
                                json.Root.SelectToken(element).SelectToken(node.Text) != null)
                        .ToList()
                        .ForEach(elem =>
                        {
                            int secondPosition = node.Text.Length + 3;
                            string nodeText = json.Root.SelectToken(elem).SelectToken(node.Text).Parent.ToString().Substring(0, secondPosition);
                            goToPosition(scintilla, nodeText);
                        });
                        listTokensStates.ForEach(element => { selectingToken(element, json, scintilla, node.Text); });
                    }
                }
                cancel = false;
            }
            catch
            {

            }
        }

        private void selectingToken(string findToken, JToken json, Scintilla scintilla, string textToFind)
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

        private void goToPosition(Scintilla scintilla, string nodeText)
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
    }
}
