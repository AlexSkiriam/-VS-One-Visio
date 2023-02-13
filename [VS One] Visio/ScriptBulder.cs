using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Visio = Microsoft.Office.Interop.Visio;
using System.Text.RegularExpressions;
using AutocompleteMenuNS;
using System.Collections;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace _VS_One__Visio
{
    public partial class ScriptBulder : Form
    {
        int lastCaretPos = 0;
        private Visio.Shapes allShapes;
        private Dictionary<string, Visio.Shape> dictOfShape = new Dictionary<string, Visio.Shape>();

        public ScriptBulder(Visio.Shapes shapes)
        {
            InitializeComponent();

            allShapes = shapes;
        }

        private void ScriptBulder_Load(object sender, EventArgs e)
        {
            scintilla1.MouseDown += new MouseEventHandler(RichTextBox_MouseClick);
            scintilla1.Select();

            scintilla1.Margins[0].Width = 50;

            scintilla1.Styles[ScintillaNET.Style.Json.Default].ForeColor = Color.Blue;
            scintilla1.Styles[ScintillaNET.Style.Json.BlockComment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla1.Styles[ScintillaNET.Style.Json.LineComment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla1.Styles[ScintillaNET.Style.Json.Number].ForeColor = Color.Orange;
            scintilla1.Styles[ScintillaNET.Style.Json.PropertyName].ForeColor = Color.FromArgb(163, 21, 21);
            scintilla1.Styles[ScintillaNET.Style.Json.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla1.Styles[ScintillaNET.Style.Json.StringEol].BackColor = Color.Pink;
            scintilla1.Styles[ScintillaNET.Style.Json.Operator].ForeColor = Color.Purple;

            scintilla1.Lexer = ScintillaNET.Lexer.Json;

            // Instruct the lexer to calculate folding
            scintilla1.SetProperty("fold", "1");
            scintilla1.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            scintilla1.Margins[2].Type = ScintillaNET.MarginType.Symbol;
            scintilla1.Margins[2].Mask = ScintillaNET.Marker.MaskFolders;
            scintilla1.Margins[2].Sensitive = true;
            scintilla1.Margins[2].Width = 10;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                scintilla1.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                scintilla1.Markers[i].SetBackColor(SystemColors.ControlDark);
            }

            // Configure folding markers with respective symbols
            scintilla1.Markers[ScintillaNET.Marker.Folder].Symbol = ScintillaNET.MarkerSymbol.BoxPlus;
            scintilla1.Markers[ScintillaNET.Marker.FolderOpen].Symbol = ScintillaNET.MarkerSymbol.BoxMinus;
            scintilla1.Markers[ScintillaNET.Marker.FolderEnd].Symbol = ScintillaNET.MarkerSymbol.BoxPlusConnected;
            scintilla1.Markers[ScintillaNET.Marker.FolderMidTail].Symbol = ScintillaNET.MarkerSymbol.TCorner;
            scintilla1.Markers[ScintillaNET.Marker.FolderOpenMid].Symbol = ScintillaNET.MarkerSymbol.BoxMinusConnected;
            scintilla1.Markers[ScintillaNET.Marker.FolderSub].Symbol = ScintillaNET.MarkerSymbol.VLine;
            scintilla1.Markers[ScintillaNET.Marker.FolderTail].Symbol = ScintillaNET.MarkerSymbol.LCorner;

            scintilla1.ClearCmdKey(Keys.Control | Keys.S);

            // Enable automatic folding
            scintilla1.AutomaticFold = (ScintillaNET.AutomaticFold.Show | ScintillaNET.AutomaticFold.Click | ScintillaNET.AutomaticFold.Change);

            scintilla1.IndentationGuides = ScintillaNET.IndentView.LookBoth;
            scintilla1.Styles[ScintillaNET.Style.BraceLight].BackColor = Color.LightGray;
            scintilla1.Styles[ScintillaNET.Style.BraceLight].ForeColor = Color.BlueViolet;
            scintilla1.Styles[ScintillaNET.Style.BraceBad].ForeColor = Color.Red;

            autocompleteMenu1.TargetControlWrapper = new ScintillaWrapper(scintilla1);

            scintilla2.Margins[0].Width = 50;

            scintilla2.Styles[ScintillaNET.Style.Json.Default].ForeColor = Color.Blue;
            scintilla2.Styles[ScintillaNET.Style.Json.BlockComment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla2.Styles[ScintillaNET.Style.Json.LineComment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla2.Styles[ScintillaNET.Style.Json.Number].ForeColor = Color.Orange;
            scintilla2.Styles[ScintillaNET.Style.Json.PropertyName].ForeColor = Color.FromArgb(163, 21, 21);
            scintilla2.Styles[ScintillaNET.Style.Json.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla2.Styles[ScintillaNET.Style.Json.StringEol].BackColor = Color.Pink;
            scintilla2.Styles[ScintillaNET.Style.Json.Operator].ForeColor = Color.Purple;

            scintilla2.Lexer = ScintillaNET.Lexer.Json;

            // Instruct the lexer to calculate folding
            scintilla2.SetProperty("fold", "1");
            scintilla2.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            scintilla2.Margins[2].Type = ScintillaNET.MarginType.Symbol;
            scintilla2.Margins[2].Mask = ScintillaNET.Marker.MaskFolders;
            scintilla2.Margins[2].Sensitive = true;
            scintilla2.Margins[2].Width = 10;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                scintilla2.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                scintilla2.Markers[i].SetBackColor(SystemColors.ControlDark);
            }

            // Configure folding markers with respective symbols
            scintilla2.Markers[ScintillaNET.Marker.Folder].Symbol = ScintillaNET.MarkerSymbol.BoxPlus;
            scintilla2.Markers[ScintillaNET.Marker.FolderOpen].Symbol = ScintillaNET.MarkerSymbol.BoxMinus;
            scintilla2.Markers[ScintillaNET.Marker.FolderEnd].Symbol = ScintillaNET.MarkerSymbol.BoxPlusConnected;
            scintilla2.Markers[ScintillaNET.Marker.FolderMidTail].Symbol = ScintillaNET.MarkerSymbol.TCorner;
            scintilla2.Markers[ScintillaNET.Marker.FolderOpenMid].Symbol = ScintillaNET.MarkerSymbol.BoxMinusConnected;
            scintilla2.Markers[ScintillaNET.Marker.FolderSub].Symbol = ScintillaNET.MarkerSymbol.VLine;
            scintilla2.Markers[ScintillaNET.Marker.FolderTail].Symbol = ScintillaNET.MarkerSymbol.LCorner;

            scintilla2.ClearCmdKey(Keys.Control | Keys.S);

            // Enable automatic folding
            scintilla2.AutomaticFold = (ScintillaNET.AutomaticFold.Show | ScintillaNET.AutomaticFold.Click | ScintillaNET.AutomaticFold.Change);

            scintilla2.IndentationGuides = ScintillaNET.IndentView.LookBoth;
            scintilla2.Styles[ScintillaNET.Style.BraceLight].BackColor = Color.LightGray;
            scintilla2.Styles[ScintillaNET.Style.BraceLight].ForeColor = Color.BlueViolet;
            scintilla2.Styles[ScintillaNET.Style.BraceBad].ForeColor = Color.Red;

            autocompleteMenu1.TargetControlWrapper = new ScintillaWrapper(scintilla2);

            listView1.FullRowSelect = true;
            listView1.MouseUp += new MouseEventHandler(listView1_MouseClick);

            setListOfElement();
            BuildAutocompleteMenu();
        }

        private static bool IsBrace(int c)
        {
            switch (c)
            {
                case '(':
                case ')':
                case '[':
                case ']':
                case '{':
                case '}':
                case '<':
                case '>':
                    return true;
            }

            return false;
        }

        private void uiUpdate(object sender, ScintillaNET.UpdateUIEventArgs e)
        {
            ScintillaNET.Scintilla scintilla = (ScintillaNET.Scintilla)sender;

            // Has the caret changed position?
            var caretPos = scintilla.CurrentPosition;
            if (lastCaretPos != caretPos)
            {
                lastCaretPos = caretPos;
                var bracePos1 = -1;
                var bracePos2 = -1;

                // Is there a brace to the left or right?
                if (caretPos > 0 && IsBrace(scintilla.GetCharAt(caretPos - 1)))
                    bracePos1 = (caretPos - 1);
                else if (IsBrace(scintilla.GetCharAt(caretPos)))
                    bracePos1 = caretPos;

                if (bracePos1 >= 0)
                {
                    // Find the matching brace
                    bracePos2 = scintilla.BraceMatch(bracePos1);
                    if (bracePos2 == ScintillaNET.Scintilla.InvalidPosition)
                    {
                        scintilla.BraceBadLight(bracePos1);
                        scintilla.HighlightGuide = 0;
                    }
                    else
                    {
                        scintilla.BraceHighlight(bracePos1, bracePos2);
                        scintilla.HighlightGuide = scintilla.GetColumn(bracePos1);
                    }
                }
                else
                {
                    // Turn off brace matching
                    scintilla.BraceHighlight(ScintillaNET.Scintilla.InvalidPosition, ScintillaNET.Scintilla.InvalidPosition);
                    scintilla.HighlightGuide = 0;
                }
            }
        }

        private void scintilla1_UpdateUI(object sender, ScintillaNET.UpdateUIEventArgs e)
        {
            uiUpdate(sender, e);
            //scintilla2.Text = scintilla1.Text;
        }

        private void scintilla2_UpdateUI(object sender, ScintillaNET.UpdateUIEventArgs e)
        {
            uiUpdate(sender, e);
            //scintilla1.Text = scintilla2.Text;
        }

        private void RichTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            UsingText usingText = new UsingText();

            if (e.Button == MouseButtons.Right)
            {
                ContextMenu menu = new ContextMenu();

                MenuItem speech = new MenuItem("Спич");
                speech.Click += delegate (object sender2, EventArgs e2)
                {
                    string text = "{\n\t\"name\": \"_speech\",\n\t\"speech\": [\n\t\t{\n\t\t\t\"text\": \"\",\n\t\t\t\"namespace\": [],\n\t\t}\n\t],\n\t\"next_state\": \"end\"\n},\n";

                    if (scintilla1.CurrentPosition != -1) scintilla1.InsertText(scintilla1.CurrentPosition, text);
                    else scintilla1.Text += text;
                };
                menu.MenuItems.Add(speech);

                MenuItem condition = new MenuItem("Условие");
                condition.Click += delegate (object sender2, EventArgs e2)
                {
                    string text = "{\n\t\"name\": \"_condition\",\n\t\"conditions\": [\n\t\t{\n\t\t\t\"always_true\": {},\n\t\t\t\"next_state\": \"end\",\n\t\t}\n\t],\n},\n";

                    if (scintilla1.CurrentPosition != -1) scintilla1.InsertText(scintilla1.CurrentPosition, text);
                    else scintilla1.Text += text;
                };
                menu.MenuItems.Add(condition);

                MenuItem conditionWithSpeech = new MenuItem("Условие со спичем");
                conditionWithSpeech.Click += delegate (object sender2, EventArgs e2)
                {
                    string text = usingText.defaultStateText("", "", "", "", "", "", "", "");

                    if (scintilla1.CurrentPosition != -1) scintilla1.InsertText(scintilla1.CurrentPosition, text);
                    else scintilla1.Text += text;
                };
                menu.MenuItems.Add(conditionWithSpeech);

                MenuItem resultWithSpeech = new MenuItem("Результат со спичем");
                resultWithSpeech.Click += delegate (object sender2, EventArgs e2)
                {
                    string text = usingText.resultStateText("", "", "", "", "");

                    if (scintilla1.CurrentPosition != -1) scintilla1.InsertText(scintilla1.CurrentPosition, text);
                    else scintilla1.Text += text;
                };
                menu.MenuItems.Add(resultWithSpeech);

                ((ScintillaNET.Scintilla)sender).ContextMenu = menu;
                menu.Show(scintilla1, e.Location);
            }

        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem != null && listView1.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    ContextMenu menu = new ContextMenu();

                    MenuItem toOF = new MenuItem("Создать спич");
                    toOF.Click += delegate (object sender2, EventArgs e2) { parseSpeech(listView1.FocusedItem.SubItems[1].Text); };
                    menu.MenuItems.Add(toOF);

                    MenuItem toFDP = new MenuItem("Создать слушалку");
                    toFDP.Click += delegate (object sender2, EventArgs e2)
                    {
                        parseListening(dictOfShape[listView1.FocusedItem.SubItems[1].Text]);
                    };
                    menu.MenuItems.Add(toFDP);

                    menu.Show(listView1, new Point(e.X, e.Y));
                }
            }
        }

        private void setListOfElement()
        {
            if (allShapes != null)
            {
                for (int i = 1; i < allShapes.Count; i++)
                {
                    if (Regex.IsMatch(allShapes[i].NameU, @"[P p]rocess"))
                    {
                        if (!dictOfShape.ContainsKey(allShapes[i].Text)) dictOfShape.Add(allShapes[i].Text, allShapes[i]);

                        ListViewItem item = new ListViewItem(i.ToString());
                        item.SubItems.Add(allShapes[i].Text);
                        listView1.Items.Add(item);
                    }
                }
            }
        }

        private void parseSpeech(string sourceText)
        {
            string text = Regex.Replace(sourceText, @"^[^\n]+\n", "");

            string newText = "{\n\t\"name\": \"_speech\",\n\t\"speech\": [\n\t\t{\n\t\t\t\"text\": \""
                + text + "\",\n\t\t\t\"namespace\": [],\n\t\t}\n\t],\n\t\"next_state\": \"end\"\n},\n";

            if (scintilla1.CurrentPosition != -1) scintilla1.InsertText(scintilla1.CurrentPosition, newText);
            else scintilla1.Text += newText;
        }

        private void parseListening(Visio.Shape shape)
        {
            string text = "{" +
                "\n\t\"name\": \"_listening\"," +
                "\n\t\"display_name\": \"Слушаем ответ абонента\"," +
                "\n\t\"settings\": {" +
                "\n\t\t\"max_hypotheses_to_check\": 3," +
                "\n\t\t\"incomprehensible_state\": \"end\"," +
                "\n\t\t\"silence_length_seconds\": 6.5," +
                "\n\t\t\"noise_length_seconds\": 15," +
                "\n\t\t\"long_silence_state\": \"end\"," +
                "\n\t\t\"long_noise_state\": \"end\"" +
                "\n\t}," +
                "\n\t\"phrases_conditions\": [";

            Array shpIds = shape.ConnectedShapes(Visio.VisConnectedShapesFlags.visConnectedShapesOutgoingNodes, "");

            Parallel.For(0, shpIds.Length, i =>
            {
                var element = Convert.ToInt32(shpIds.GetValue(i));
                Visio.Shape newShape = allShapes.ItemFromID[element];

                var newShpIds = shape.GluedShapes(Visio.VisGluedShapesFlags.visGluedShapesOutgoing1D, "", newShape);

                var subElement = Convert.ToInt32(newShpIds.GetValue(0));
                var connector = allShapes.ItemFromID[subElement];

                text += "\n\t\t{" +
                    "\n\t\t\t\"phrases_exact\": []," +
                    "\n\t\t\t\"phrases_parts\": []," +
                    "\n\t\t\t\"phrases_parts_exclude\": []," +
                    "\n\t\t\t\"basic_phrase\": \"" + connector.Text + "\"," +
                    "\n\t\t\t\"next_state\": \"end\"" +
                    "\n\t\t},";
            });

            text += "\n\t]\n},\n";

            if (scintilla1.CurrentPosition != -1) scintilla1.InsertText(scintilla1.CurrentPosition, text);
            else scintilla1.Text += text;
        }

        /* Keywords */
        private const string defaultState = "name next_state display_name require_push";
        private const string speechState = "speech text namespace";
        private const string listeningState = "settings max_hypotheses_to_check incomprehensible_state silence_length_seconds noise_length_seconds long_silence_state long_noise_state phrases_conditions basic_phrase phrases_parts phrases_exact phrases_parts_exclude";
        private const string conditionsState = "conditions condition always_true on_enter on_match";

        private const string cSharp = "";
        private const string typesCSharp = "string int double List<> Dictionary<> DateTime() IsNullOrEmpty()";

        /* Special NULL value styling */
        private const string sp_null = "null";
        /* Special NULL Operator styling */
        private const string sp_operator = "is not";

        string[] snippets = { "snippet_speech", "snippet_condition" };

        private void BuildAutocompleteMenu()
        {
            var items = new List<AutocompleteItem>();

            List<string> list = BuildAutoShowList();

            foreach (var item in snippets)
                items.Add(new Snippet(item) { ImageIndex = 1 });

            //set as autocomplete source
            autocompleteMenu1.SetAutocompleteItems(items);

            autocompleteMenu1.SetAutocompleteItems(new DynamicCollection(scintilla1));

            autocompleteMenu1.AllowsTabKey = true;
        }
        /// <summary>
        /// Inerts line break after '}'
        /// </summary>
        class Snippet : AutocompleteItem
        {
            public Snippet(string snippet)
                : base(snippet)
            {
            }

            public override string GetTextForReplace()
            {
                switch (Parent.SelectedItemIndex)
                {
                    case 0:
                        Parent.Fragment.Text =
                            "{\n\t\"name\": \"_speech\",\n\t\"speech\": [\n\t\t{\n\t\t\t\"text\": \"\",\n\t\t\t\"namespace\": [],\n\t\t}\n\t],\n\t\"next_state\": \"end\"\n},\n";
                        break;

                    case 1:
                        Parent.Fragment.Text =
                            "{\n\t\"name\": \"_condition\",\n\t\"conditions\": [\n\t\t{\n\t\t\t\"always_true\": {},\n\t\t\t\"next_state\": \"end\",\n\t\t}\n\t],\n},\n";
                        break;
                }
                return Parent.Fragment.Text;
            }

            public override string ToolTipTitle
            {
                get
                {
                    return "Вставляет выбранный код";
                }
            }
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

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
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

        private void scintilla1_CharAdded(object sender, ScintillaNET.CharAddedEventArgs e)
        {
            InsertMatchedChars(scintilla1, e);

            int position = scintilla2.CurrentPosition;
            scintilla2.Text = scintilla1.Text;
            scintilla2.GotoPosition(position);
            scintilla2.ScrollCaret();
        }

        private void scintilla2_CharAdded(object sender, ScintillaNET.CharAddedEventArgs e)
        {
            InsertMatchedChars(scintilla2, e);

            int position = scintilla1.CurrentPosition;
            scintilla1.Text = scintilla2.Text;
            scintilla1.GotoPosition(position);
            scintilla1.ScrollCaret();
        }

        private void scintilla2_InsertCheck(object sender, ScintillaNET.InsertCheckEventArgs e)
        {
            if ((e.Text.EndsWith("\r") || e.Text.EndsWith("\n")))
            {
                var curLine = scintilla1.LineFromPosition(e.Position);
                var curLineText = scintilla1.Lines[curLine].Text;

                var indent = Regex.Match(curLineText, @"^[ \t]*");
                e.Text += indent.Value; // Add indent following "\r\n"

                // Current line end with bracket?
                if (Regex.IsMatch(curLineText, @"[\{ \[]\s*$"))
                    e.Text += '\t'; // Add tab
            }
        }

        private void scintilla1_InsertCheck(object sender, ScintillaNET.InsertCheckEventArgs e)
        {
            if ((e.Text.EndsWith("\r") || e.Text.EndsWith("\n")))
            {
                var curLine = scintilla1.LineFromPosition(e.Position);
                var curLineText = scintilla1.Lines[curLine].Text;

                var indent = Regex.Match(curLineText, @"^[ \t]*");
                e.Text += indent.Value; // Add indent following "\r\n"

                // Current line end with bracket?
                if (Regex.IsMatch(curLineText, @"[\{ \[]\s*$"))
                    e.Text += '\t'; // Add tab
            }
        }

        private void InsertMatchedChars(ScintillaNET.Scintilla scintilla, ScintillaNET.CharAddedEventArgs e)
        {
            var caretPos = scintilla.CurrentPosition;
            var docStart = caretPos == 1;
            var docEnd = caretPos == scintilla.Text.Length;

            var charPrev = docStart ? scintilla.GetCharAt(caretPos) : scintilla.GetCharAt(caretPos - 2);
            var charNext = scintilla.GetCharAt(caretPos);

            var isCharPrevBlank = charPrev == ' ' || charPrev == '\t' ||
                                  charPrev == '\n' || charPrev == '\r';

            var isCharNextBlank = charNext == ' ' || charNext == '\t' ||
                                  charNext == '\n' || charNext == '\r' ||
                                  docEnd;

            var isEnclosed = (charPrev == '(' && charNext == ')') ||
                                  (charPrev == '{' && charNext == '}') ||
                                  (charPrev == '[' && charNext == ']');

            var isSpaceEnclosed = (charPrev == '(' && isCharNextBlank) || (isCharPrevBlank && charNext == ')') ||
                                  (charPrev == '{' && isCharNextBlank) || (isCharPrevBlank && charNext == '}') ||
                                  (charPrev == '[' && isCharNextBlank) || (isCharPrevBlank && charNext == ']');

            var isCharOrString = (isCharPrevBlank && isCharNextBlank) || isEnclosed || isSpaceEnclosed;

            var charNextIsCharOrString = charNext == '"' || charNext == '\'';

            switch (e.Char)
            {
                case '(':
                    if (charNextIsCharOrString) return;
                    scintilla.InsertText(caretPos, ")");
                    break;
                case '{':
                    if (charNextIsCharOrString) return;
                    scintilla.InsertText(caretPos, "}");
                    break;
                case '[':
                    if (charNextIsCharOrString) return;
                    scintilla.InsertText(caretPos, "]");
                    break;
                case '"':
                    // 0x22 = "
                    if (charPrev == 0x22 && charNext == 0x22)
                    {
                        scintilla.DeleteRange(caretPos, 1);
                        scintilla.GotoPosition(caretPos);
                        return;
                    }

                    if (isCharOrString)
                        scintilla.InsertText(caretPos, "\"");
                    break;
                case '\'':
                    // 0x27 = '
                    if (charPrev == 0x27 && charNext == 0x27)
                    {
                        scintilla.DeleteRange(caretPos, 1);
                        scintilla.GotoPosition(caretPos);
                        return;
                    }

                    if (isCharOrString)
                        scintilla.InsertText(caretPos, "'");
                    break;
            }
        }

        JsonParse jsonParse = new JsonParse();

        private void scintilla1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                var log = jsonParse.parseJson(scintilla1.Text, scintilla2);
                if (log == null)
                {
                    listView2.Items.Clear();
                    treeView1.SetObjectAsJson(JToken.Parse(jsonParse.parseConverting(scintilla2.Text)));
                }
                else
                {
                    ListViewItem item = new ListViewItem("0-80X08e");
                    item.SubItems.Add(log.start + "_" + log.end);
                    item.SubItems.Add(log.message);
                    item.BackColor = Color.Red;
                    listView2.Items.Add(item);
                }
            }
            if (e.Control && e.Shift && e.KeyCode == Keys.S)
            {
                savingEvent();
            }
            if (e.Control && e.KeyCode == Keys.Z)
            {
                int position = scintilla2.CurrentPosition;
                scintilla2.Text = scintilla1.Text;
                scintilla2.GotoPosition(position);
                scintilla2.ScrollCaret();
            }
            if (e.Control && e.KeyCode == Keys.V)
            {
                int position = scintilla2.CurrentPosition;
                scintilla2.Text = scintilla1.Text;
                scintilla2.GotoPosition(position);
                scintilla2.ScrollCaret();
            }
        }

        private void scintilla2_KeyDown(object sender, KeyEventArgs e)
        {
            JsonParse jsonParse = new JsonParse();

            if (e.Control && e.KeyCode == Keys.S)
            {
                var log = jsonParse.parseJson(scintilla2.Text, scintilla2);
                if (log == null)
                {
                    listView2.Items.Clear();
                    treeView1.SetObjectAsJson(JToken.Parse(jsonParse.parseConverting(scintilla2.Text)));
                }
                else
                {
                    ListViewItem item = new ListViewItem("0-80X08e");
                    item.SubItems.Add(log.start + "_" + log.end);
                    item.SubItems.Add(log.message);
                    item.BackColor = Color.Red;
                    listView2.Items.Add(item);
                }
            }
            if (e.Control && e.Shift && e.KeyCode == Keys.S)
            {
                savingEvent();
            }
            if (e.Control && e.KeyCode == Keys.Z)
            {
                int position = scintilla1.CurrentPosition;
                scintilla1.Text = scintilla2.Text;
                scintilla1.GotoPosition(position);
                scintilla1.ScrollCaret();
            }
            if (e.Control && e.KeyCode == Keys.V)
            {
                int position = scintilla1.CurrentPosition;
                scintilla1.Text = scintilla2.Text;
                scintilla1.GotoPosition(position);
                scintilla1.ScrollCaret();
            }
        }

        private void scintilla1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar < 32)
            {
                e.Handled = true;
                return;
            }
        }

        private void scintilla2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar < 32)
            {
                e.Handled = true;
                return;
            }
        }

        private void scintilla1_Delete(object sender, ScintillaNET.ModificationEventArgs e)
        {
            if (!scintilla2.Focused) scrollToPosition(scintilla2);
        }

        private void scintilla2_Delete(object sender, ScintillaNET.ModificationEventArgs e)
        {
            if (!scintilla1.Focused) scrollToPosition(scintilla1);
        }

        private void scintilla1_Insert(object sender, ScintillaNET.ModificationEventArgs e)
        {
            if (!scintilla2.Focused) scrollToPosition(scintilla2);
        }

        private void scintilla2_Insert(object sender, ScintillaNET.ModificationEventArgs e)
        {
            if (!scintilla1.Focused) scrollToPosition(scintilla1);
        }

        private void scrollToPosition(ScintillaNET.Scintilla scintilla)
        {
            var linesOnScreen = scintilla.LinesOnScreen - 2;

            int lineFromPosition = scintilla1.LineFromPosition(scintilla1.CurrentPosition);
            scintilla1.Text = scintilla2.Text;

            var start = scintilla.Lines[lineFromPosition - (linesOnScreen / 2)].Position;
            var end = scintilla.Lines[lineFromPosition + (linesOnScreen / 2)].Position;
            scintilla.ScrollRange(start, end);
        }

        private Keys keyPress = Keys.None;

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control) keyPress = Keys.Control;
        }

        private void treeView1_KeyUp(object sender, KeyEventArgs e)
        {
            keyPress = Keys.None;
        }

        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ScintillaNET.Scintilla scintilla = (keyPress == Keys.Control) ? scintilla2 : scintilla1;
            keyPress = Keys.None;

            if (treeView1.SelectedNode != null)
            {
                var json = JToken.Parse(jsonParse.parseConverting(scintilla.Text));

                List<string> listTokens = new List<string>()
                {
                    "objects", "production", "tts_phrases", "tts_phrases_runtime",
                    "expressions", "phrases", "special_states", "global_listening_states", "on_timer_states", "states"
                };

                try
                {
                    if (json.Root.SelectToken(treeView1.SelectedNode.Text) != null)
                    {
                        goToPosition(scintilla, json.Root.SelectToken(treeView1.SelectedNode.Text).Parent.ToString().Substring(0, treeView1.SelectedNode.Text.Length + 3));
                    }
                    else if (json.Root.SelectToken("states") != null)
                    {
                        selectingToken("states", json, scintilla);
                    }
                }
                catch
                {

                }
            }
        }

        private void selectingToken(string findToken, JToken json, ScintillaNET.Scintilla scintilla)
        {
            for (int i = 0; i <= json.Root.SelectToken(findToken).Count(); i++)
            {
                if ((json.Root.SelectToken(findToken)[i].SelectToken("name") != null && json.Root.SelectToken(findToken)[i].SelectToken("name").ToString() == treeView1.SelectedNode.Text))
                    goToPosition(scintilla,
                        json.Root.SelectToken(findToken)[i].ToString().Substring(4,
                        (json.Root.SelectToken(findToken)[i].ToString().Length >= treeView1.SelectedNode.Text.Length + 10)
                        ? treeView1.SelectedNode.Text.Length + 10 : json.Root.SelectToken(findToken)[i].ToString().Length));
            }
        }

        private void goToPosition(ScintillaNET.Scintilla scintilla, string nodeText)
        {
            int position = scintilla.Text.IndexOf(nodeText);
            if (position > 0)
            {
                scintilla.GotoPosition(position);
                scintilla.SetSelection(position, position + nodeText.Length);
                scintilla.ScrollCaret();

            }
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.AutoUpgradeEnabled = true;
            dialog.Filter = "Текстовые файлы (*.txt)|*.txt";
            DialogResult result = dialog.ShowDialog();

            string userPathValue = (result == DialogResult.OK) ? dialog.FileName : "";

            if (!String.IsNullOrEmpty(userPathValue) && File.Exists(userPathValue))
            {
                string text = File.ReadAllText(userPathValue);

                scintilla1.Text = text;
                scintilla2.Text = text;
            }

            JsonParse jsonParse = new JsonParse();

            var log = jsonParse.parseJson(scintilla1.Text, scintilla2);
            if (log == null)
            {
                listView2.Items.Clear();
                treeView1.SetObjectAsJson(JToken.Parse(jsonParse.parseConverting(scintilla2.Text)));
            }
            else
            {
                ListViewItem item = new ListViewItem("0-80X08e");
                item.SubItems.Add(log.start + "_" + log.end);
                item.SubItems.Add(log.message);
                item.BackColor = Color.Red;
                listView2.Items.Add(item);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            savingEvent();
        }

        private void savingEvent()
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "Текстовые файлы (*.txt)|*.txt";
            dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() == DialogResult.Cancel) return;
            string filename = dialog.FileName;

            File.WriteAllText(filename, scintilla1.Text);
            MessageBox.Show("Файл сохранен");
        }

        private void listView2_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView2.FocusedItem != null && listView2.FocusedItem.Bounds.Contains(e.Location) == true)
            {
                MessageBox.Show(listView2.FocusedItem.SubItems[2].Text);
                string[] positions = listView2.FocusedItem.SubItems[1].Text.Split('_');
                scintilla1.ScrollRange(Convert.ToInt32(positions[0]), Convert.ToInt32(positions[0]));
            }
        }

        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Black, e.Bounds);
            e.DrawText();
        }

        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }
    }
}
