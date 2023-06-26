using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace _VS_One__Visio
{
    public partial class VisioViewerForm : Form
    {
        TextAnalysis textAnalysis = new TextAnalysis();
        UsingText usingText = new UsingText();
        PhrasesFunctions phrasesFunctions = new PhrasesFunctions();

        ScintillaFunc scinitillaFunc = new ScintillaFunc();
        ScinitillaStyles scinitillaStyles = new ScinitillaStyles();

        int lastCaretPos = 0;
        public int subIndex;

        Dictionary<string, int> meta = new Dictionary<string, int>();
        Dictionary<string, int> textMeta = new Dictionary<string, int>();

        private List<string> _statesInScript = new List<string>();
        public List<string> statesInScript
        {
            get
            {
                return _statesInScript;
            }
            set
            {
                _statesInScript = value;
                BuildAutocompleteMenu();
            }
        }

        Aspose.Diagram.Diagram currentDiagram;

        public static string selectedPhrase { get; set; }
        public bool editResult = false;

        private bool resizing = false;

        public TextEditorForm textEditor = null;
        NewPhrasesFomAss phrasesContainer;

        public VisioViewerForm(string path)
        {
            InitializeComponent();
            axViewer1.BackColor = Color.White;
            this.Load += VisioViewerForm_Load;
            axViewer1.OnSelectionChanged += AxViewer1_OnSelectionChanged;
            axViewer1.ContextMenuEnabled = false;

            if (!String.IsNullOrEmpty(path)) openEvent(path);

            scintilla1.Select();
            scinitillaStyles.scintillaSetInitStyleJson(scintilla1);

            scintilla1.InsertCheck += Scintilla1_InsertCheck;
            scintilla1.CharAdded += Scintilla1_CharAdded;
            scintilla1.KeyPress += Scintilla1_KeyPress;
            scintilla1.KeyDown += Scintilla1_KeyDown;
            scintilla1.UpdateUI += Scintilla1_UpdateUI;

            textBox1.TextChanged += textBox1_TextChanged;

            phrasesContainer = getPhrasesFromAss();

            tabPage2.Controls.Add(phrasesContainer);
            phrasesContainer.Show();

            autocompleteMenu1.TargetControlWrapper = new ScintillaWrapper(scintilla1);
            BuildAutocompleteMenu();
        }

        private void BuildAutocompleteMenu()
        {
            autocompleteMenu1.SetAutocompleteItems(statesInScript);
            autocompleteMenu1.AllowsTabKey = true;
        }

        private void Scintilla1_KeyDown(object sender, KeyEventArgs e)
        {
            LanguageClass language = new LanguageClass();
            if (e.Control && e.KeyCode == Keys.P) language.SelectedTextLanguageSwitcher(scintilla1);
        }

        private ContextMenuStrip getContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            menu.Items.Add("Создать стейты с условием");
            menu.Items[0].Click += delegate (object sender2, EventArgs e2) { menuFunc(MenuMode.CreateCond); };

            menu.Items.Add("Перейти к стейту");
            menu.Items[1].Click += delegate (object sender2, EventArgs e2) { menuFunc(MenuMode.GoTo); };

            return menu;
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

        private void AxViewer1_OnSelectionChanged(object sender, AxVisioViewer._IViewerEvents_OnSelectionChangedEvent e)
        {
            if (axViewer1.SelectedShapeIndex > 0 && axViewer1.Focused)
            {
                int shapeID = axViewer1.get_ShapeIndexToID(axViewer1.SelectedShapeIndex);
                Aspose.Diagram.Shape curentShape = getShapeFormId(shapeID);
                string text = textWithoutTag(curentShape.Text.Value.Text);
                tryAddInMeta(text);
                richTextBox1.Text = text;
                axViewer1.ContextMenuStrip = getContextMenu();
                axViewer1.ContextMenuStrip.Show(axViewer1.PointToClient(Cursor.Position));
            }
        }

        private void goToScintillaLine(string sourceString)
        {
            if (meta.ContainsKey(sourceString) && textEditor != null)
            {
                var linesOnScreen = textEditor.scintilla1.LinesOnScreen - 2;

                var start = textEditor.scintilla1.Lines[meta[sourceString] - (linesOnScreen / 2)].Position;
                var end = textEditor.scintilla1.Lines[meta[sourceString] + (linesOnScreen / 2)].Position;
                textEditor.scintilla1.ScrollRange(start, end);
            }
        }

        private void menuFunc(MenuMode mode)
        {
            if (axViewer1.SelectedShapeIndex >= 0)
            {
                int shapeID = axViewer1.get_ShapeIndexToID(axViewer1.SelectedShapeIndex);
                Aspose.Diagram.Shape curentShape = getShapeFormId(shapeID);

                if (curentShape != null)
                {
                    string text = textWithoutTag(curentShape.Text.Value.Text);
                    switch (mode)
                    {
                        case MenuMode.GoTo:
                            goToScintillaLine(text);
                            break;

                        case MenuMode.CreateCond:
                            List<string> list = phrasesFunctions.additionalPharsesList(text);
                            if (list.Count > 0) scintilla1.Text = usingText.defaultStateText("", "", "", "", list[0], list[1], "", "");
                            else scintilla1.Text = "";
                            break;
                    }
                }
            }
        }

        private Aspose.Diagram.Shape getShapeFormId(int shapeID)
        {
            Aspose.Diagram.Shape curentShape = null;
            foreach (Aspose.Diagram.Shape shp in currentDiagram.Pages[0].Shapes)
            {
                if (shp.ID == shapeID)
                {
                    curentShape = shp;
                    break;
                }
            }

            return curentShape;
        }

        private string textWithoutTag(string sourceString)
        {
            return Regex.Replace(sourceString, @"<(cp|pp|tp) IX=\'\d+\'/>", "");
        }

        private void VisioViewerForm_Load(object sender, EventArgs e)
        {
            this.UpdateSize(this, null);
        }

        public void UpdateSize(object obj, EventArgs ea)
        {
            this.AutoSize = true;
            this.axViewer1.ClientSize = new Size(this.ClientSize.Width, this.ClientSize.Height);
            this.axViewer1.Zoom = -1;
            this.axViewer1.Refresh();
        }

        public void openEvent(string path = "")
        {
            string userPathValue = "";

            if (String.IsNullOrEmpty(path))
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.AutoUpgradeEnabled = true;
                DialogResult result = dialog.ShowDialog();

                userPathValue = (result == DialogResult.OK) ? dialog.FileName : "";
            }
            else
            {
                userPathValue = path;
            }

            try
            {
                if (!String.IsNullOrEmpty(userPathValue) && File.Exists(userPathValue))
                {
                    axViewer1.Load(userPathValue);
                    axViewer1.Refresh();
                    currentDiagram = new Aspose.Diagram.Diagram(userPathValue);
                    setTreeView();
                }
            }
            catch
            {

            }
        }

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip menu = new ContextMenuStrip();

                ToolStripMenuItem addMeta = new ToolStripMenuItem("Добавить позицию в мету");
                addMeta.Click += addMetaClick;

                ToolStripMenuItem copyText = new ToolStripMenuItem("Скопировать фразу");
                copyText.Click += CopyText_Click;

                menu.Items.AddRange(new ToolStripItem[] { addMeta, copyText });

                treeView1.ContextMenuStrip = menu;
            }
        }

        private void CopyText_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null) Clipboard.SetText(treeView1.SelectedNode.Text.Replace(Environment.NewLine, " "));
        }

        private void addMetaClick(object sender, EventArgs e)
        {

            if (treeView1.SelectedNode != null)
            {
                int number = 0;

                using (var form = new MetaForm())
                {
                    if (meta.ContainsKey(treeView1.SelectedNode.Text)) form.numericUpDown1.Value = meta[treeView1.SelectedNode.Text];
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK) number = form.lineNumber;
                }

                if (meta.ContainsKey(treeView1.SelectedNode.Text)) meta[treeView1.SelectedNode.Text] = number;
                else meta.Add(treeView1.SelectedNode.Text, number);
            }
        }

        private TreeView sourceTree = new TreeView();

        private void setTreeView()
        {
            Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
            treeView1.Nodes[0].Nodes.Clear();

            foreach (Aspose.Diagram.Shape shp in currentDiagram.Pages[0].Shapes)
            {
                if (Regex.IsMatch(shp.NameU, @"(Process|Процесс)"))
                {
                    string text = textWithoutTag(shp.Text.Value.Text);
                    string group = Regex.Match(text, @".*№\d+").Value;
                    group = phrasesFunctions.cleanSurplusSpaces(group);
                    if (!String.IsNullOrEmpty(group))
                    {
                        if (!dict.ContainsKey(group))
                        {
                            List<string> list = new List<string>();
                            list.Add(text);
                            dict.Add(group, list);
                        }
                        else dict[group].Add(text);
                    }
                }
            }

            List<string> newList = new List<string>();

            foreach (var elem in dict)
            {
                newList.Add(elem.Key);
            }

            newList.Sort();

            for (int i = 0; i < newList.Count; i++)
            {
                treeView1.Nodes[0].Nodes.Add(newList[i]);
                foreach (string str in dict[newList[i]])
                {
                    treeView1.Nodes[0].Nodes[i].Nodes.Add(str);
                }
            }

            treeView1.Sort();
            treeView1.Nodes[0].Expand();
            sourceTree = treeView1;
        }

        private void tryAddInMeta(string shapetext)
        {
            meta.Clear();
            string findText = Regex.Match(shapetext, @".+\r?\n(Основная\s+фраза\s*:)*([^\n]+)\r?\n*").Groups[2].Value;
            findText = findText.Replace("\r", "");
            findText = Regex.Replace(findText, @"^\s+", "");
            if (textEditor != null)
            {
                LevinsteinDistanceClass levinstein = new LevinsteinDistanceClass();
                int currentPosition = levinstein.findWithLevinstainInTextMeta(findText, textMeta);
                if (currentPosition > 0)
                {
                    int line = textEditor.scintilla1.LineFromPosition(currentPosition);
                    if (!meta.ContainsKey(shapetext)) meta.Add(shapetext, line);
                    else meta[shapetext] = line;
                }
            }
        }

        private void findAllTextFildInScript()
        {
            textMeta.Clear();
            foreach (Match match in Regex.Matches(textEditor.scintilla1.Text, @"""text""\s*:\s*""([^""]+)"""))
            {
                if (!textMeta.ContainsKey(match.Groups[1].Value)) textMeta.Add(match.Groups[1].Value, match.Index);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openEvent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void VisioViewerForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.O)
            {
                if (textEditor != null) findAllTextFildInScript();
                openEvent();
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                foreach (Aspose.Diagram.Shape shp in currentDiagram.Pages[0].Shapes)
                {
                    if (treeView1.SelectedNode.Text == textWithoutTag(shp.Text.Value.Text))
                    {
                        axViewer1.SelectShape(axViewer1.get_ShapeIDToIndex(Convert.ToInt32(shp.ID)));
                        tryAddInMeta(treeView1.SelectedNode.Text);
                        goToScintillaLine(treeView1.SelectedNode.Text);
                        break;
                    }
                }
            }
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
                if (tn.Text.ToLower().Contains(searchString.ToLower()) && (!found))
                {
                    found = true;
                    tn.BackColor = Color.LightGreen;
                    treeView1.SelectedNode = tn;
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

        private NewPhrasesFomAss getPhrasesFromAss()
        {
            NewPhrasesFomAss script = new NewPhrasesFomAss(this);

            script.TopLevel = false;
            script.FormBorderStyle = FormBorderStyle.None;
            script.AutoScaleMode = AutoScaleMode.Dpi;
            script.Dock = DockStyle.Fill;

            return script;
        }
    }

    public enum MenuMode
    {
        GoTo,
        CreateCond
    }
}
