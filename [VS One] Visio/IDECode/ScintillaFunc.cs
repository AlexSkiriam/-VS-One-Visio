using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using ScintillaNET;

namespace _VS_One__Visio
{
    class ScintillaFunc
    {
        JsonParse parser = new JsonParse();

        public string storeStringClass = "";
        public string typesStringClass = "";

        public void insertCheck(object sender, InsertCheckEventArgs e, Scintilla scintilla)
        {
            if ((e.Text.EndsWith("\r") || e.Text.EndsWith("\n")))
            {
                var curLine = scintilla.LineFromPosition(e.Position);
                var curLineText = scintilla.Lines[curLine].Text;

                var indent = Regex.Match(curLineText, @"^[ \t]*");
                e.Text += indent.Value; // Add indent following "\r\n"

                // Current line end with bracket?
                if (Regex.IsMatch(curLineText, @"[\{ \[]\s*$"))
                    e.Text += '\t'; // Add tab
            }
        }

        public void keyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar < 32)
            {
                e.Handled = true;
                return;
            }
        }

        public void uiUpdate(object sender, UpdateUIEventArgs e, int lastCaretPos)
        {
            Scintilla scintilla = (Scintilla)sender;

            var caretPos = scintilla.CurrentPosition;
            if (lastCaretPos != caretPos)
            {
                lastCaretPos = caretPos;
                var bracePos1 = -1;
                var bracePos2 = -1;

                if (caretPos > 0 && IsBrace(scintilla.GetCharAt(caretPos - 1)))
                    bracePos1 = (caretPos - 1);
                else if (IsBrace(scintilla.GetCharAt(caretPos)))
                    bracePos1 = caretPos;

                if (bracePos1 >= 0)
                {
                    bracePos2 = scintilla.BraceMatch(bracePos1);
                    if (bracePos2 == Scintilla.InvalidPosition)
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
                    scintilla.BraceHighlight(Scintilla.InvalidPosition, Scintilla.InvalidPosition);
                    scintilla.HighlightGuide = 0;
                }
            }
        }

        public string storeBuilder(Scintilla scintilla, List<string> list)
        {
            string store = "";
            JToken token = JToken.Parse(parser.parseConverting(scintilla.Text));
            if (token.SelectToken("objects") != null && token.SelectToken("objects").SelectToken("store") != null)
            {
                foreach (var tok in token.SelectToken("objects").SelectToken("store"))
                {
                    list.Add($"store.{((JProperty)tok).Name}");

                    if (((JProperty)tok).Value.Type == JTokenType.String)
                    {
                        store += "public " + ((JProperty)tok).Value + " " + ((JProperty)tok).Name + " { get; set; }\n";
                    }
                    if (((JProperty)tok).Value.SelectToken("type") != null)
                    {
                        store += "public " + ((JValue)((JProperty)tok).Value.SelectToken("type")).Value + " " + ((JProperty)tok).Name + " { get; set; }\n";
                    }
                    if (((JProperty)tok).Value.SelectToken("collection_type") != null
                        && ((JValue)((JProperty)tok).Value.SelectToken("element_type") != null
                            || ((JValue)((JProperty)tok).Value.SelectToken("key_type") != null && (JValue)((JProperty)tok).Value.SelectToken("value_type") != null)))
                    {
                        string types =
                            ((JValue)((JProperty)tok).Value.SelectToken("element_type") != null) ?
                                $"<{(JValue)((JProperty)tok).Value.SelectToken("element_type")}>"
                                : $"<{(JValue)((JProperty)tok).Value.SelectToken("key_type")},{(JValue)((JProperty)tok).Value.SelectToken("value_type")}>";
                        store += "public "
                            + ((JValue)((JProperty)tok).Value.SelectToken("collection_type")).Value
                            + types + " "
                            + ((JProperty)tok).Name
                            + " { get; set; }\n";
                    }
                }
            }
            return store;
        }

        public string userTypeBuilder(Scintilla scintilla, List<string> list)
        {
            string userType = "";
            JToken token = JToken.Parse(parser.parseConverting(scintilla.Text));
            if (token.SelectToken("types") != null)
            {
                foreach (var tok in token.SelectToken("types"))
                {
                    list.Add(((JProperty)tok).Name);
                    userType += "public class " + ((JProperty)tok).Name + " {\n";
                    foreach (var subtok in ((JProperty)tok).Value)
                    {
                        if (((JProperty)subtok).Value.Type == JTokenType.String)
                        {
                            userType += "public " + ((JProperty)subtok).Value + " " + ((JProperty)subtok).Name + " { get; set; }\n";
                        }
                        if (((JProperty)tok).Value.SelectToken("type") != null)
                        {
                            userType += "public " + ((JValue)((JProperty)tok).Value.SelectToken("type")).Value + " " + ((JProperty)tok).Name + " { get; set; }\n";
                        }
                        if (((JProperty)tok).Value.SelectToken("collection_type") != null
                            && ((JValue)((JProperty)tok).Value.SelectToken("element_type") != null
                                || ((JValue)((JProperty)tok).Value.SelectToken("key_type") != null && (JValue)((JProperty)tok).Value.SelectToken("value_type") != null)))
                        {
                            string types =
                                ((JValue)((JProperty)tok).Value.SelectToken("element_type") != null) ?
                                    $"<{(JValue)((JProperty)tok).Value.SelectToken("element_type")}>"
                                    : $"<{(JValue)((JProperty)tok).Value.SelectToken("key_type")},{(JValue)((JProperty)tok).Value.SelectToken("value_type")}>";
                            userType += "public "
                                + ((JValue)((JProperty)tok).Value.SelectToken("collection_type")).Value
                                + types + " "
                                + ((JProperty)tok).Name
                                + " { get; set; }\n";
                        }
                    }
                    userType += "}\n";
                }
            }
            return userType;
        }

        public string getOnlyPhraseObjects(string jsonValue)
        {
            JToken tok = JToken.Parse(jsonValue);

            string returnString = "{ ";

            if (tok.SelectToken("expressions") != null) returnString += tok.SelectToken("expressions").Parent.ToString() + ",";
            if (tok.SelectToken("phrases") != null)  returnString += tok.SelectToken("phrases").Parent.ToString() + ",";

            return returnString + " }";
        }

        public string tabBuilder(int tabCount)
        {
            string newString = "";
            for (int i = 1; i <= tabCount; i++) newString += "\t";
            return newString;
        }

        public string withInsertTabs(string source, string tabs)
        {
            string stringToReturn = "";
            string[] list = source.Split('\n');
            foreach (string str in list)
            {
                stringToReturn += tabs + str;
            }
            return stringToReturn;
        }

        public static bool IsBrace(int c)
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

        public void InsertMatchedChars(Scintilla scintilla, CharAddedEventArgs e)
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
    }
}
