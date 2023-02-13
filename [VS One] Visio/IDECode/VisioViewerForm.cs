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
using Microsoft.Office.Interop;
using Visio = Microsoft.Office.Interop.Visio;

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

        Aspose.Diagram.Diagram currentDiagram;

        public static string selectedPhrase { get; set; }
        public bool editResult = false;

        private bool resizing = false;

        public TextEditorForm textEditor = null;

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
            scintilla1.UpdateUI += Scintilla1_UpdateUI;
        }

        private ContextMenuStrip getContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();

            menu.Items.Add("Создать стейты с условием");
            menu.Items[0].Click += delegate (object sender2, EventArgs e2) { createCondition(); };

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
            if(axViewer1.SelectedShapeIndex > 0 && axViewer1.Focused)
            {
                int shapeID = axViewer1.get_ShapeIndexToID(axViewer1.SelectedShapeIndex);
                Aspose.Diagram.Shape curentShape = getShapeFormId(shapeID);
                tryAddInMeta(textWithoutTag(curentShape.Text.Value.Text));
                if (meta.ContainsKey(textWithoutTag(curentShape.Text.Value.Text)) && textEditor != null)
                    textEditor.scintilla1.Lines[meta[textWithoutTag(curentShape.Text.Value.Text)]].Goto();
                axViewer1.ContextMenuStrip = getContextMenu();
                axViewer1.ContextMenuStrip.Show(axViewer1.PointToClient(Cursor.Position));
            }
        }

        private void createCondition()
        {
            if (axViewer1.SelectedShapeIndex >= 0)
            {
                int shapeID = axViewer1.get_ShapeIndexToID(axViewer1.SelectedShapeIndex);
                Aspose.Diagram.Shape curentShape = getShapeFormId(shapeID);

                if (curentShape != null)
                {
                    string text = textWithoutTag(curentShape.Text.Value.Text);
                    if (meta.ContainsKey(text) && textEditor != null) textEditor.scintilla1.Lines[meta[text]].Goto();
                    richTextBox1.Text = text;
                    List<string> list = phrasesFunctions.additionalPharsesList(text);
                    if (list.Count > 0) scintilla1.Text = usingText.defaultStateText("", "", "", "", list[0], list[1], "", "");
                    else scintilla1.Text = "";
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

            foreach(var elem in dict)
            {
                newList.Add(elem.Key);
            }

            newList.Sort();

            for(int i = 0; i <= newList.Count; i++)
            {
                treeView1.Nodes[0].Nodes.Add(newList[i]);
                foreach (string str in dict[newList[i]])
                {
                    treeView1.Nodes[0].Nodes[i].Nodes.Add(str);
                }
            }

            treeView1.Sort();
            treeView1.Nodes[0].Expand();
        }

        private void tryAddInMeta(string shapetext)
        {
            meta.Clear();
            string findText = Regex.Match(shapetext, @".+\r?\n(Основная\s+фраза\s*:)*([^\n]+)\r?\n*").Groups[2].Value;
            findText = findText.Replace("\r", "");
            findText = Regex.Replace(findText, @"^\s+", "");
            if (textEditor != null)
            {
                int currentPosition = findWithLevinstain(findText);
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
                if(!textMeta.ContainsKey(match.Groups[1].Value)) textMeta.Add(match.Groups[1].Value, match.Index);
            }
        }

        private int findWithLevinstain(string source)
        {
            Dictionary<int, int> postions = new Dictionary<int, int>();
            foreach(KeyValuePair<string, int> keyValue in textMeta)
            {
                if(!postions.ContainsKey(keyValue.Value)) postions.Add(keyValue.Value, LevenshteinDistance(source, keyValue.Key));
            }
            var keyOfMinValue = postions.Aggregate((x, y) => x.Value < y.Value ? x : y).Key;
            return keyOfMinValue;
        }

        public int LevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                if (string.IsNullOrEmpty(target)) return 0;
                return target.Length;
            }
            if (string.IsNullOrEmpty(target)) return source.Length;

            if (source.Length > target.Length)
            {
                var temp = target;
                target = source;
                source = temp;
            }

            var m = target.Length;
            var n = source.Length;
            var distance = new int[2, m + 1];
            for (var j = 1; j <= m; j++) distance[0, j] = j;

            var currentRow = 0;
            for (var i = 1; i <= n; ++i)
            {
                currentRow = i & 1;
                distance[currentRow, 0] = i;
                var previousRow = currentRow ^ 1;
                for (var j = 1; j <= m; j++)
                {
                    var cost = (target[j - 1] == source[i - 1] ? 0 : 1);
                    distance[currentRow, j] = Math.Min(Math.Min(
                                distance[previousRow, j] + 1,
                                distance[currentRow, j - 1] + 1),
                                distance[previousRow, j - 1] + cost);
                }
            }
            return distance[currentRow, m];
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
                        if (meta.ContainsKey(treeView1.SelectedNode.Text) && textEditor != null)
                            textEditor.scintilla1.Lines[meta[treeView1.SelectedNode.Text]].Goto();
                        break;
                    }
                }
            }   
        }
    }
}
