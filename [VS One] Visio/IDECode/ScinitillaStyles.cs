using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScintillaNET;

namespace _VS_One__Visio
{
    public class ScinitillaStyles
    {
        public Color Invert(Color color)
        {
            return Color.FromArgb(color.ToArgb() ^ 0xFFFFFF);
        }

        public void scintillaSetInitStyleJson(Scintilla scintilla)
        {
            Color backgroundColor = default;
            //scintilla.StyleResetDefault();
            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 10;
            scintilla.Styles[Style.Default].BackColor = backgroundColor;
            scintilla.Styles[Style.Default].ForeColor = Color.Black;
            scintilla.Styles[Style.LineNumber].ForeColor = Color.FromArgb(36, 162, 131);
            scintilla.Styles[Style.LineNumber].BackColor = SystemColors.Control;
            //scintilla.StyleClearAll();

            scintilla.Margins[0].Width = 50;

            scintilla.Styles[Style.Json.Default].ForeColor = Color.FromArgb(163, 21, 21);//Color.Blue;
            scintilla.Styles[Style.Json.BlockComment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla.Styles[Style.Json.LineComment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla.Styles[Style.Json.Number].ForeColor = Color.Orange;
            scintilla.Styles[Style.Json.PropertyName].ForeColor = Color.FromArgb(163, 21, 21);
            scintilla.Styles[Style.Json.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla.Styles[Style.Json.StringEol].BackColor = Color.Pink;
            scintilla.Styles[Style.Json.Operator].ForeColor = Color.Purple;
            scintilla.Styles[Style.Json.Error].ForeColor = Color.Black;
            scintilla.Styles[Style.Json.Keyword].ForeColor = Color.Blue;

            scintilla.Styles[Style.Json.Default].BackColor = backgroundColor;
            scintilla.Styles[Style.Json.BlockComment].BackColor = backgroundColor;
            scintilla.Styles[Style.Json.LineComment].BackColor = backgroundColor;
            scintilla.Styles[Style.Json.Number].BackColor = backgroundColor;
            scintilla.Styles[Style.Json.PropertyName].BackColor = backgroundColor;
            scintilla.Styles[Style.Json.String].BackColor = backgroundColor;
            scintilla.Styles[Style.Json.Operator].BackColor = backgroundColor;
            scintilla.Styles[Style.Json.Error].BackColor = backgroundColor;
            scintilla.Styles[Style.Json.Keyword].BackColor = backgroundColor;

            //scintilla.SetSelectionBackColor(true, Invert(backgroundColor));

            scintilla.Lexer = Lexer.Json;

            scintilla.SetProperty("fold", "1");
            scintilla.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            scintilla.Margins[2].Type = MarginType.Symbol;
            scintilla.Margins[2].Mask = Marker.MaskFolders;
            scintilla.Margins[2].Sensitive = true;
            scintilla.Margins[2].Width = 10;
            scintilla.Margins[2].BackColor = Color.White;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                scintilla.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                scintilla.Markers[i].SetBackColor(SystemColors.ControlDark);
            }

            // Configure folding markers with respective symbols
            scintilla.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            scintilla.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            scintilla.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            scintilla.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            scintilla.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            scintilla.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            scintilla.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            scintilla.ClearCmdKey(Keys.Control | Keys.S);

            // Enable automatic folding
            scintilla.AutomaticFold = AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change;

            scintilla.IndentationGuides = IndentView.LookBoth;
            scintilla.Styles[Style.BraceLight].BackColor = Color.LightGray;
            scintilla.Styles[Style.BraceLight].ForeColor = Color.BlueViolet;
            scintilla.Styles[Style.BraceBad].ForeColor = Color.Red;

            scintilla.SetKeywords(0, @"end true false null <<<CODE CODE <<<NER NER using return");

            scintilla.MultipleSelection = true;
            scintilla.MouseSelectionRectangularSwitch = true;
            scintilla.AdditionalSelectionTyping = true;
            scintilla.VirtualSpaceOptions = VirtualSpace.RectangularSelection;
        }

        public void resetToDefaultJsonStyle(Scintilla scintilla)
        {
            scintilla.StyleResetDefault();
            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 10;
            scintilla.Styles[Style.Default].BackColor = Color.White;
            scintilla.Styles[Style.Default].ForeColor = Color.Black;
            scintilla.Styles[Style.LineNumber].ForeColor = Color.FromArgb(36, 162, 131);
            scintilla.Styles[Style.LineNumber].BackColor = SystemColors.Control;
            scintilla.StyleClearAll();

            scintilla.Margins[0].Width = 50;

            scintilla.Styles[Style.Json.Default].ForeColor = Color.FromArgb(163, 21, 21);//Color.Blue;
            scintilla.Styles[Style.Json.BlockComment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla.Styles[Style.Json.LineComment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla.Styles[Style.Json.Number].ForeColor = Color.Orange;
            scintilla.Styles[Style.Json.PropertyName].ForeColor = Color.FromArgb(163, 21, 21);
            scintilla.Styles[Style.Json.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla.Styles[Style.Json.StringEol].BackColor = Color.Pink;
            scintilla.Styles[Style.Json.Operator].ForeColor = Color.Purple;
            scintilla.Styles[Style.Json.Error].ForeColor = Color.Black;
            scintilla.Styles[Style.Json.Keyword].ForeColor = Color.Blue;

            scintilla.Lexer = Lexer.Json;

            scintilla.SetProperty("fold", "1");
            scintilla.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            scintilla.Margins[2].Type = MarginType.Symbol;
            scintilla.Margins[2].Mask = Marker.MaskFolders;
            scintilla.Margins[2].Sensitive = true;
            scintilla.Margins[2].Width = 10;
            scintilla.Margins[2].BackColor = Color.White;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                scintilla.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                scintilla.Markers[i].SetBackColor(SystemColors.ControlDark);
            }

            // Configure folding markers with respective symbols
            scintilla.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            scintilla.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            scintilla.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            scintilla.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            scintilla.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            scintilla.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            scintilla.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            scintilla.ClearCmdKey(Keys.Control | Keys.S);

            // Enable automatic folding
            scintilla.AutomaticFold = AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change;

            scintilla.IndentationGuides = IndentView.LookBoth;
            scintilla.Styles[Style.BraceLight].BackColor = Color.LightGray;
            scintilla.Styles[Style.BraceLight].ForeColor = Color.BlueViolet;
            scintilla.Styles[Style.BraceBad].ForeColor = Color.Red;

            scintilla.SetKeywords(0, @"end true false null <<<CODE CODE <<<NER NER using return");

            scintilla.MultipleSelection = true;
            scintilla.MouseSelectionRectangularSwitch = true;
            scintilla.AdditionalSelectionTyping = true;
            scintilla.VirtualSpaceOptions = VirtualSpace.RectangularSelection;
        }

        public void setJsonDarkTheme(Scintilla scintilla)
        {
            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 10;
            scintilla.Styles[Style.Default].BackColor = Color.Black;
            scintilla.Styles[Style.Default].ForeColor = Color.White;
            scintilla.Styles[Style.LineNumber].ForeColor = Color.FromArgb(36, 162, 131);
            scintilla.Styles[Style.LineNumber].BackColor = SystemColors.Control;

            scintilla.SetProperty("fold", "1");
            scintilla.SetProperty("fold.compact", "1");

            scintilla.Margins[0].Width = 50;

            scintilla.Styles[Style.LineNumber].ForeColor = Color.LightBlue;
            scintilla.Styles[Style.LineNumber].BackColor = Color.FromArgb(33, 33, 33);

            scintilla.Styles[Style.Default].ForeColor = Color.White;
            scintilla.Styles[Style.Default].BackColor = Color.FromArgb(33, 33, 33);

            scintilla.Styles[Style.Json.Default].ForeColor = Color.White;
            scintilla.Styles[Style.Json.Default].BackColor = Color.FromArgb(33, 33, 33);

            scintilla.Styles[Style.Json.BlockComment].ForeColor = Color.Green; // Green
            scintilla.Styles[Style.Json.BlockComment].BackColor = Color.FromArgb(33, 33, 33);

            scintilla.Styles[Style.Json.LineComment].ForeColor = Color.Green; // Green
            scintilla.Styles[Style.Json.LineComment].BackColor = Color.FromArgb(33, 33, 33);

            scintilla.Styles[Style.Json.Number].ForeColor = Color.LightGreen;
            scintilla.Styles[Style.Json.Number].BackColor = Color.FromArgb(33, 33, 33);

            scintilla.Styles[Style.Json.PropertyName].ForeColor = Color.LightBlue;
            scintilla.Styles[Style.Json.PropertyName].BackColor = Color.FromArgb(33, 33, 33);

            scintilla.Styles[Style.Json.String].ForeColor = Color.FromArgb(236, 161, 119);
            scintilla.Styles[Style.Json.String].BackColor = Color.FromArgb(33, 33, 33);

            scintilla.Styles[Style.Json.StringEol].ForeColor = Color.Pink;
            scintilla.Styles[Style.Json.StringEol].BackColor = Color.FromArgb(33, 33, 33);

            scintilla.Styles[Style.Json.Operator].ForeColor = Color.White;
            scintilla.Styles[Style.Json.Operator].BackColor = Color.FromArgb(33, 33, 33);

            scintilla.Styles[Style.Json.Error].ForeColor = Color.Gray;
            scintilla.Styles[Style.Json.Error].BackColor = Color.FromArgb(33, 33, 33);

            scintilla.Lexer = Lexer.Json;

            scintilla.Margins[2].Type = MarginType.Symbol;
            scintilla.Margins[2].Mask = Marker.MaskFolders;
            scintilla.Margins[2].BackColor = Color.FromArgb(33, 33, 33);
            scintilla.Margins[2].Sensitive = true;
            scintilla.Margins[2].Width = 10;

            scintilla.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            scintilla.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            scintilla.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            scintilla.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            scintilla.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            scintilla.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            scintilla.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                scintilla.Markers[i].SetForeColor(Color.FromArgb(33, 33, 33));
                scintilla.Markers[i].SetBackColor(Color.White);
            }

            scintilla.ClearCmdKey(Keys.Control | Keys.S);

            // Enable automatic folding
            scintilla.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);

            scintilla.IndentationGuides = IndentView.LookBoth;
            scintilla.Styles[Style.BraceLight].BackColor = Color.LightGray;
            scintilla.Styles[Style.BraceLight].ForeColor = Color.BlueViolet;
            scintilla.Styles[Style.BraceBad].ForeColor = Color.Red;

            scintilla.SetSelectionBackColor(true, Color.FromArgb(165, 183, 186));
            scintilla.SetSelectionForeColor(true, Color.White);

            scintilla.SetKeywords(0, @"end true false null <<<CODE CODE <<<NER NER using return");

            scintilla.MultipleSelection = true;
            scintilla.MouseSelectionRectangularSwitch = true;
            scintilla.AdditionalSelectionTyping = true;
            scintilla.VirtualSpaceOptions = VirtualSpace.RectangularSelection;
        }

        public void scintillaSetInitStyleCSharp(Scintilla scintilla)
        {
            CSharpLexer csLexer = new CSharpLexer();

            scintilla.StyleResetDefault();
            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 10;
            scintilla.Styles[Style.Default].BackColor = Color.White;
            scintilla.Styles[Style.Default].ForeColor = Color.Black;
            scintilla.StyleClearAll();

            scintilla.Margins[0].Width = 50;

            scintilla.CaretStyle = CaretStyle.Line;
            scintilla.Lexer = Lexer.Container;

            scintilla.Styles[CSharpLexer.StyleDefault].ForeColor = Color.Black;
            scintilla.Styles[CSharpLexer.StyleKeyword].ForeColor = Color.DarkSlateBlue;
            scintilla.Styles[CSharpLexer.StyleContainerProcedure].ForeColor = Color.HotPink;
            scintilla.Styles[CSharpLexer.StyleProcedureContainer].ForeColor =
            scintilla.Styles[CSharpLexer.StyleContextual].ForeColor = Color.DarkBlue;
            scintilla.Styles[CSharpLexer.StyleIdentifier].ForeColor = Color.DarkSlateGray;
            scintilla.Styles[CSharpLexer.StyleNumber].ForeColor = Color.OrangeRed;
            scintilla.Styles[CSharpLexer.StyleString].ForeColor = Color.DarkOrange;
            scintilla.Styles[CSharpLexer.StyleComment].ForeColor = Color.Green;
            scintilla.Styles[CSharpLexer.StyleProcedure].ForeColor = Color.Purple;
            scintilla.Styles[CSharpLexer.StyleVerbatim].ForeColor = Color.Purple;
            scintilla.Styles[CSharpLexer.StylePreprocessor].ForeColor = Color.DarkSlateGray;
            scintilla.Styles[CSharpLexer.StyleEscapeSequence].ForeColor = Color.MediumPurple;
            scintilla.Styles[CSharpLexer.StyleOperator].ForeColor = Color.HotPink;
            scintilla.Styles[CSharpLexer.StyleBraces].ForeColor = Color.GreenYellow;
            scintilla.Styles[CSharpLexer.StyleError].ForeColor = Color.DarkRed;
            scintilla.Styles[CSharpLexer.StyleUser].ForeColor = Color.DarkSlateGray;

            CSharpLexer.Init_Lexer(scintilla);
            CSharpLexer.SetKeyWords(
                "abstract add as ascending async await base bool break by byte case catch char checked class const continue decimal default delegate descending do double dynamic else enum equals explicit extern false finally fixed float for foreach from get global global goto goto group if implicit in int interface internal into is join let lock long namespace new null object on operator orderby out override params partial private protected public readonly ref remove return sbyte sealed select set short sizeof stackalloc static string struct switch this throw true try typeof uint ulong unchecked unsafe ushort using value var virtual void volatile where while yield",
                inUserKeywords: "store parameters match static_parameters", AutoFillContextual: true
            );

            scintilla.Margins[2].Type = MarginType.Symbol;
            scintilla.Margins[2].Mask = Marker.MaskFolders;
            scintilla.Margins[2].Sensitive = true;
            scintilla.Margins[2].Width = 10;

            scintilla.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            scintilla.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            scintilla.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            scintilla.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            scintilla.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            scintilla.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            scintilla.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                scintilla.Markers[i].SetForeColor(Color.Black);
                scintilla.Markers[i].SetBackColor(SystemColors.Control);
            }

            scintilla.ClearCmdKey(Keys.Control | Keys.S);

            // Enable automatic folding
            scintilla.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);

            scintilla.IndentationGuides = IndentView.LookBoth;
            scintilla.Styles[Style.BraceLight].BackColor = Color.LightGray;
            scintilla.Styles[Style.BraceLight].ForeColor = Color.BlueViolet;
            scintilla.Styles[Style.BraceBad].ForeColor = Color.Red;
        }

        public void setLinks(Scintilla scintilla)
        {
            scintillaSetInitStyleJson(scintilla);

            scintilla.Styles[40].ForeColor = Color.Blue;
            scintilla.Styles[40].BackColor = Color.White;
            scintilla.Styles[40].Bold = false;
            scintilla.Styles[40].Hotspot = true;
            scintilla.Styles[40].Visible = true;
            scintilla.Styles[40].Underline = true;

            scintilla.Styles[41].ForeColor = Color.Blue;
            scintilla.Styles[42].ForeColor = Color.Orange;
            scintilla.Styles[43].ForeColor = Color.Black;
            scintilla.Styles[44].ForeColor = Color.Green;

            scintilla.Styles[45].ForeColor = Color.FromArgb(163, 21, 21);

            List<(int, int, int)> colorIndexList = new List<(int, int, int)>();

            foreach (Match match in Regex.Matches(scintilla.Text, @"(?<=""\w*_state""\s*\:\s*"")[^""]+(?="")"))
                colorIndexList.Add((match.Index, match.Length, 40));

            foreach (Match match in Regex.Matches(scintilla.Text, @"(?<=""text""\s*\:\s*"")[^""]+(?="")"))
                colorIndexList.Add((match.Index, match.Length, 43));

            foreach (Match match in Regex.Matches(scintilla.Text, @"<<<CODE(.(?!CODE))*\W*(?=CODE)CODE", RegexOptions.Singleline))
                colorIndexList.Add((match.Index, match.Length, 41));

            foreach (Match match in Regex.Matches(scintilla.Text, @"<<<NER(.(?!NER))*\W*(?=NER)NER", RegexOptions.Singleline))
                colorIndexList.Add((match.Index, match.Length, 41));

            foreach (Match match in Regex.Matches(scintilla.Text, @"(?<=""\s*:\s*)(true|false|null)", RegexOptions.Singleline))
                colorIndexList.Add((match.Index, match.Length, 41));

            foreach (Match match in Regex.Matches(scintilla.Text, @"(?<=""\s*:\s*)(-)?\d+(\.\d+)*"))
                colorIndexList.Add((match.Index, match.Length, 42));

            foreach (Match match in Regex.Matches(scintilla.Text, @"(//.+|//(?=//))"))
                colorIndexList.Add((match.Index, match.Length, 44));

            foreach (Match match in Regex.Matches(scintilla.Text, @"/\*(.(?!\*/))+.(?=\*/)\*/",  RegexOptions.Singleline))
                colorIndexList.Add((match.Index, match.Length, 44));

            colorIndexList.OrderBy(x => x.Item1).ToList().ForEach(x => 
            {
                scintilla.StartStyling(x.Item1);
                scintilla.SetStyling(x.Item2, x.Item3);
                scintilla.StartStyling(x.Item1 + x.Item2);
                scintilla.SetStyling(0, 45);
            });

            scintillaSetInitStyleJson(scintilla);
        }
    }
}
