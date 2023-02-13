using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScintillaNET;

namespace _VS_One__Visio
{
    class CSharpLexer
    {
        private static List<char> NumberTypes = new List<char>
        {
            '0', '1', '2', '3', '4', '5', '6','7', '8', '9',
            'a', 'b', 'c', 'd', 'e', 'f', 'x',
            'A', 'B', 'C', 'D', 'E', 'F', '-', '.'
        };

        private static List<char> EscapeSequences = new List<char>
        {
            '\'', '"', '\\', '0', 'a', 'b', 'f',
            'n', 'r', 't', 'v'
        };

        private static List<string> Operators = new List<string>
        {
            "<<", ">>", "<=", ">=", "+=", "-=", "*=", "&=",
            "|=", "!=", "^=", "->", "??", "=>", "++", "--",
            "==", "&&", "||", "+", "-", "*", "&", "!", "|",
            "^", "~", "=", "<", ">"
        };

        private static List<char> OperatorStragglers = new List<char>
        {
            '*', '&', '?', '-', '!'
        };

        private static List<char> IdentifierMarkers = new List<char>
        {
            '<', '[', '.'
        };

        private static List<string> FullDocument = new List<string>
        {
            "*", "/", "{", "}"
        };

        //Few of these might need renamed
        public const int StyleDefault = 0,
                         StyleKeyword = 1,
                      StyleIdentifier = 2,
                          StyleNumber = 3,
                          StyleString = 4,
                         StyleComment = 5,
                       StyleProcedure = 6,
                      StyleContextual = 7,
                        StyleVerbatim = 8,
                    StylePreprocessor = 9,
                  StyleEscapeSequence = 10,
                        StyleOperator = 11,
                          StyleBraces = 12,
                           StyleError = 13,
                            StyleUser = 14,
              StyleProcedureContainer = 15,
              StyleContainerProcedure = 16;

        private const int STATE_UNKNOWN = 0,
                       STATE_IDENTIFIER = 1,
                           STATE_NUMBER = 2,
                           STATE_STRING = 3,
                STATE_MULTILINE_COMMENT = 4,
                     STATE_PREPROCESSOR = 5,
                         STATE_OPERATOR = 6;

        public static List<string> KEYWORDS, //Primary keywords
                        CONTEXTUAL_KEYWORDS, //Secondary keywords
                              USER_KEYWORDS; //User-defined keywords

        private static bool IMPORTANT_KEY_DELETED = false;

        public static void SetKeyWords(string inKeywords = "", string inContextualKeywords = "", string inUserKeywords = "", bool AutoFillContextual = false)
        {
            IEnumerable<string> AssemblyTypes()
            {
                return typeof(string).Assembly.GetTypes()
                                         .Where(t => t.IsPublic && t.IsVisible)
                                         .Select(t => new { t.Name, Length = t.Name.IndexOf('`') })
                                         .Select(x => x.Length == -1 ? x.Name : x.Name.Substring(0, x.Length))
                                         .Distinct();
            }

            //Wasn't going to do it this way.  But, I guess, this is more "Flexible".
            CONTEXTUAL_KEYWORDS = new List<string>();

            if (inKeywords != "") { KEYWORDS = new List<string>(inKeywords.Split(' ')); }
            if (inContextualKeywords != "") { CONTEXTUAL_KEYWORDS.AddRange(inContextualKeywords.Split(' ').ToList()); }
            if (inUserKeywords != "") { USER_KEYWORDS = new List<string>(inUserKeywords.Split(' ')); }

            if (AutoFillContextual) { CONTEXTUAL_KEYWORDS.AddRange(AssemblyTypes()); }
        }

        public static void Init_Lexer(Scintilla inScintilla)
        {
            inScintilla.CharAdded += (s, ae) => { IMPORTANT_KEY_DELETED = FullDocument.Contains(ae.Char.ToString()); };

            //PLEASE NOTE I'M ALLOWING THIS TO BE CALLED MULTIPLE TIMES.  JUST IN CASE, IT NEEDS TO BE USED ON MULTIPLE SCINTILLA CONTROLS.
            inScintilla.Delete += (s, de) => { IMPORTANT_KEY_DELETED = (FullDocument.Contains(de.Text) || de.Text == @""""); };

            inScintilla.StyleNeeded += (s, se) =>
            {
                Style(inScintilla, inScintilla.GetEndStyled(), se.Position, IMPORTANT_KEY_DELETED);

                IMPORTANT_KEY_DELETED = false;
            };
        }

        public static void Style(Scintilla scintilla, int startPos, int endPos, bool fullDoc = true)
        {
            startPos = (fullDoc ? 0 : scintilla.Lines[scintilla.LineFromPosition(startPos)].Position);
            endPos = (fullDoc ? (scintilla.Lines[scintilla.Lines.Count].EndPosition - 1) : endPos);

            int style, length = 0, state = STATE_UNKNOWN;

            bool VERBATIM = false, PARENTHESIS = false;

            char c = '\0', d = '\0';

            bool SINGLE_LINE_COMMENT,
                  MULTI_LINE_COMMENT,
                             DBL_OPR;

            void ClearState() { length = state = STATE_UNKNOWN; }

            void DefaultStyle() => scintilla.SetStyling(1, StyleDefault);

            int StyleUntilEndOfLine(int inPosition, int inStyle)
            {
                int len = (scintilla.Lines[scintilla.LineFromPosition(inPosition)].EndPosition - inPosition);

                scintilla.SetStyling(len, inStyle);

                return --len; //We return the length, cause we'll have to adjust the startPos.
            }

            bool ContainsUsingStatement(int inPosition) => (scintilla.GetTextRange(scintilla.Lines[scintilla.LineFromPosition(inPosition)].Position, 5)).Contains("using");

            SetKeyWords(AutoFillContextual: true);

            scintilla.StartStyling(startPos);
            {
                for (; startPos < endPos; startPos++)
                {
                    //Got rid of half the casts and half of the method calls.
                    c = scintilla.Text[startPos];
                    d = (char)scintilla.GetCharAt(startPos + 1);

                    if (state == STATE_UNKNOWN)
                    {
                        bool bFormattedVerbatim = ((c == '$') && (d == '@')),
                                     bFormatted = ((c == '$') && ((d == '"'))),
                                   bNegativeNum = ((c == '-') && (char.IsDigit(d))),
                                      bFraction = ((c == '.') && (char.IsDigit(d))),
                                        bString = (c == '"');

                        VERBATIM = ((c == '@') && (d == '"'));

                        SINGLE_LINE_COMMENT = ((c == '/') && (d == '/'));
                        MULTI_LINE_COMMENT = ((c == '/') && (d == '*'));

                        //I always want braces to be highlighted 
                        if ((c == '{') || (c == '}'))
                        {
                            scintilla.SetStyling(1, ((scintilla.BraceMatch(startPos) > -1) ? StyleBraces : StyleError)); //Only works if I load my external lexer.
                        }
                        else if (char.IsLetter(c)) //Indentifier - Keywords, procedures, etc ...
                        {
                            state = STATE_IDENTIFIER;
                            continue;
                        }
                        else if (bString || VERBATIM || bFormatted || bFormattedVerbatim) //String
                        {
                            int len = ((VERBATIM || bFormatted || bFormattedVerbatim) ? ((bFormattedVerbatim) ? 3 : 2) : 1);

                            scintilla.SetStyling(len, (!VERBATIM ? StyleString : StyleVerbatim));

                            startPos += (len - 1);

                            state = STATE_STRING;
                            continue;
                        }
                        else if (char.IsDigit(c) || bNegativeNum || bFraction) //Number
                        {
                            state = STATE_NUMBER;
                            continue;
                        }
                        else if (SINGLE_LINE_COMMENT || MULTI_LINE_COMMENT) //Comment
                        {
                            if (SINGLE_LINE_COMMENT)
                            {
                                startPos += StyleUntilEndOfLine(startPos, StyleComment);
                            }
                            else
                            {
                                scintilla.SetStyling(2, StyleComment);

                                startPos += 2;

                                state = STATE_MULTILINE_COMMENT;
                                continue;
                            }
                        }
                        else if (c == '#') //Preprocessor
                        {
                            startPos += StyleUntilEndOfLine(startPos, StylePreprocessor);
                        }
                        else if (
                                    (char.IsSymbol(c) || OperatorStragglers.Contains(c)) && (Operators.Contains($"{c}" +
                                    ((DBL_OPR = (char.IsSymbol(d) || OperatorStragglers.Contains(d))) ? $"{d}" : "")))
                                ) //Operators
                        {
                            scintilla.SetStyling((DBL_OPR ? 2 : 1), StyleOperator);

                            startPos += (DBL_OPR ? 1 : 0);
                            continue;
                        }
                        else { DefaultStyle(); }
                    }

                    switch (state)
                    {
                        case STATE_IDENTIFIER:
                            string identifier = scintilla.GetWordFromPosition(startPos);

                            style = StyleIdentifier;

                            int s = startPos;

                            startPos += (identifier.Length - 2);

                            d = (char)scintilla.GetCharAt(startPos + 1);

                            bool OPEN_PAREN = (d == '(');

                            if (!OPEN_PAREN && KEYWORDS.Contains(identifier)) { style = StyleKeyword; } //Keywords
                            else if (!OPEN_PAREN && CONTEXTUAL_KEYWORDS.Contains(identifier)) { style = StyleContextual; } //Contextual Keywords
                            else if (!OPEN_PAREN && USER_KEYWORDS.Contains(identifier)) { style = StyleUser; } //User Keywords
                            else if (OPEN_PAREN) { style = StyleProcedure; } //Procedures
                            else if (IdentifierMarkers.Contains(d) && !(ContainsUsingStatement(startPos))) { style = StyleProcedureContainer; } //Procedure Containers "classes?"
                            else
                            {
                                if (((char)scintilla.GetCharAt(s - 2) == '.') && !(ContainsUsingStatement(s))) { style = StyleContainerProcedure; } //Container "procedures"
                            }

                            ClearState();

                            scintilla.SetStyling(identifier.Length, style);

                            break;

                        case STATE_NUMBER:
                            length++;

                            if (!NumberTypes.Contains(c))
                            {
                                scintilla.SetStyling(length, StyleNumber);

                                ClearState();

                                startPos--;
                            }

                            break;

                        case STATE_STRING:
                            length++;

                            style = (VERBATIM ? StyleVerbatim : StyleString);

                            if (PARENTHESIS || ((c == '{') || (d == '}'))) //Formatted strings that are using braces
                            {
                                if (c == '{') { PARENTHESIS = true; }
                                if (c == '}') { PARENTHESIS = false; }
                            }
                            else if (VERBATIM && ((c == '"') && (d == '"'))) //Skip over embedded quotation marks 
                            {
                                length++;
                                startPos++;
                            }
                            else if ((c == '"') && (d != '"')) //End of our string?
                            {
                                scintilla.SetStyling(length, style);
                                ClearState();

                                VERBATIM = false;
                            }
                            else
                            {
                                if ((c == '\\') && EscapeSequences.Contains(d)) //Escape Sequences
                                {
                                    length += ((d == '\\') ? 0 : -1);

                                    scintilla.SetStyling(length, style);
                                    {
                                        startPos++; length = 0;
                                    }
                                    scintilla.SetStyling(2, StyleEscapeSequence);
                                }
                            }

                            break;

                        case STATE_MULTILINE_COMMENT:
                            length++;

                            if ((c == '*') && (d == '/'))
                            {
                                length += 2;

                                scintilla.SetStyling(length, StyleComment);

                                ClearState();

                                startPos++;
                            }

                            break;
                    }
                }
            }
        }
    }
}
