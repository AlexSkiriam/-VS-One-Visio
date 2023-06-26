using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.CodeDom.Compiler;
using System.IO;
using System.CodeDom;

namespace _VS_One__Visio
{
    class JsonParse
    {
        public string parseConverting(string jsonValue)
        {
            string newText = Regex.Replace(jsonValue, @"<<<CODE(.(?!CODE))+.(?=CODE)CODE",
                elem => 
                {
                    string e = elem.Value.Replace("CODE", "").Replace("<", "");
                    return "{ \"code\": " + JsonConvert.ToString(e) + " }"; 
                },
                RegexOptions.Singleline);
            newText = Regex.Replace(newText, @"<<<NER(.(?!NER))+.(?=NER)NER",
                elem =>
                {
                    string e = elem.Value.Replace("NER", "").Replace("<", "");
                    return "{ \"code\": " + JsonConvert.ToString(e) + " }";
                },
                RegexOptions.Singleline);
            newText = Regex.Replace(newText, @"/\*(.(?!\*/))+.(?=\*/)\*/", "", RegexOptions.Singleline);
            return newText;
        }

        public string getNewLines(int newLine)
        {
            string returnString = "";
            int i = 1;
            while(i < newLine)
            {
                returnString += "\n";
                i++;
            }
            return returnString;
        }

        public List<ownErrorLog> parseScript(string jsonValue, ScintillaNET.Scintilla scintilla, bool needShowSuccess = false)
        {
            List<ownErrorLog> logs = new List<ownErrorLog>();
            logs = parseJson(jsonValue, scintilla, needShowSuccess);
            //logs.AddRange(parseCSharp(jsonValue, scintilla, needShowSuccess));
            return logs;
        }

        public List<ownErrorLog> parseJson(string jsonValue, ScintillaNET.Scintilla scintilla, bool needShowSuccess = false)
        {
            List<ownErrorLog> logs = new List<ownErrorLog>();

            string newJsonValue = parseConverting(jsonValue);

            try
            {
                if (Properties.Settings.Default.NeedValidateStates)
                {
                    List<string> states = new List<string>();

                    var json = JToken.Parse(newJsonValue);

                    List<string> basicStates = new List<string>() { "end", "pop" };

                    Regex.Matches(newJsonValue, @"(?<=""name""\s*\:\s*"")[^""]+(?="")").Cast<Match>().ToList().ForEach(x => 
                    {
                        if (!states.Contains(x.Value)) states.Add(x.Value);
                        else throw new Exception($"Стейт уже существует в скрипте: {x.Value}<{x.Index}>");
                    });

                    Regex.Matches(newJsonValue, @"(?<=""\w*_state""\s*\:\s*"")[^""]+(?="")").Cast<Match>().ToList().ForEach(x =>
                    {
                        if (!states.Contains(x.Value) && !basicStates.Contains(x.Value) && !x.Value.Contains("."))
                            throw new Exception($"Стейт: {x.Value} отсутствует в скрипте, но на него есть ссылка<{x.Index}>");
                    });

                    Regex.Matches(newJsonValue, @"(?<=""\w*dialog\.current_state""\s*\:\s*"")[^""]+(?="")").Cast<Match>().ToList().ForEach(x =>
                    {
                        if (!states.Contains(x.Value) && !basicStates.Contains(x.Value))
                            throw new Exception($"Стейт: {x.Value} отсутствует в скрипте, но на него есть ссылка<{x.Index}>");
                    });

                    if (needShowSuccess) MessageBox.Show("Ошибок не найдено");
                }
            }
            catch (Exception e)
            {
                int start = 0;
                int end = 0;

                string errorText = e.Message;

                var linesOnScreen = scintilla.LinesOnScreen - 2;

                int position = 0;

                if (e.GetType().Name == "JsonReaderException")
                {
                    position = ((JsonReaderException)e).LineNumber;
                }
                else
                {
                    errorText = Regex.Replace(errorText, @"<(\d+)>$", match => {
                        position = Convert.ToInt32(match.Groups[1].Value);
                        return "";
                    });
                }

                start = scintilla.Lines[position - (linesOnScreen / 2)].Position;
                end = scintilla.Lines[position + (linesOnScreen / 2)].Position;
                scintilla.ScrollRange(start, end);

                errorText += "\n\n" + e.StackTrace;
                MessageBox.Show(errorText);

                logs.Add(new ownErrorLog()
                {
                    message = errorText,
                    start = start,
                    end = end
                });
            }

            return logs;
        }

        public List<ownErrorLog> parseCSharp(string jsonValue, ScintillaNET.Scintilla scintilla, bool needShowSuccess = false)
        {
            ScintillaFunc scinitillaFunc = new ScintillaFunc();

            List<ownErrorLog> logs = new List<ownErrorLog>();

            List<string> autoCompliteStore = new List<string>();
            List<string> autoCompliteTypes = new List<string>();

            string storeStringClass = scinitillaFunc.storeBuilder(scintilla, autoCompliteStore);
            string typesStringClass = scinitillaFunc.userTypeBuilder(scintilla, autoCompliteTypes);

            foreach (Match match in Regex.Matches(jsonValue, @"""([^""]+)""\s*:\s*(<<<CODE(.(?!CODE))+.(?=CODE)CODE)", RegexOptions.Singleline))
            {
                string identy = match.Groups[1].Value;
                if (replacements.Contains(identy))
                {
                    string pureCode = match.Groups[2].Value.Replace("<<<CODE", "").Replace("CODE", "");
                    ownErrorLog log = compileCode(match.Index, pureCode, storeStringClass, typesStringClass);
                    if (log != null) logs.Add(log);
                }
            }

            return logs;
        }

        private List<string> replacements = new List<string>()
        {
            "on_enter", "on_match", "on_end", /*"condition",*/
        };

        public ownErrorLog compileCode(int errorIndex, string text, string storeElements, string typesElements)
        {
            ownErrorLog log = null;

            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

            CompilerParameters parameters = new CompilerParameters();
            parameters.ReferencedAssemblies.Add("System.dll");
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, codeToCompile(text, storeElements, typesElements));

            if (results.Errors.Count > 0)
            {
                string errorText = "";
                foreach (CompilerError CompErr in results.Errors)
                {
                    errorText = CompErr.ErrorText + ";";
                    log = new ownErrorLog()
                    {
                        message = errorText,
                        start = errorIndex,
                        end = errorIndex
                    };
                }
            }
            return log;
        }

        private string codeToCompile(string userCode, string store, string userTypes)
        {
            return @"using System;
                using System.Text;
                using System.Collections.Generic;
                using System.Text.RegularExpressions;

                namespace VoiceRobot
                    {
                        public class Program
                        {
                            public static void Main(string[] args)
                            {
                                Store store = new Store();
                                " + userCode +
                            @"
                            }
                        }"
                            + userTypes +
                        "public class Store\n{"
                            + store +
                        @"}
                    }";
        }

        public void jsonRegexFormValidate(string source)
        {
            try { JToken.Parse(source).SelectToken("phrases").ToList().ForEach(elem => { ruleRegexTokenization(((JValue)elem).Value.ToString()); }); }
            catch (Exception e) { }
        }

        public JToken ruleRegexTokenization(string tokenText)
        {
            tokenText = ReplaceTokens(tokenText);
            tokenText = Regex.Replace(tokenText, @"<[^>]+>", "some_ner");
            foreach (Match match in Regex.Matches(tokenText, @"""word_array"": \[([^\]]+)\]"))
            {
                foreach(Match innerMatch in Regex.Matches(match.Groups[1].Value, @"(\@|\~)?\w+~?"))
                {
                    if(innerMatch.Groups[1].Value != null && innerMatch.Groups[1].Value == "@")
                        tokenText = Regex.Replace(tokenText, innerMatch.Value + @"\b", $"\"{innerMatch.Value}\",");
                    else
                        tokenText = Regex.Replace(tokenText, @"\b" + innerMatch.Value + @"\b", $"\"{innerMatch.Value}\",");
                }
            }
            foreach (Match match in Regex.Matches(tokenText, @"""range"": \{([^\}]+)\}"))
            {
                foreach (Match innerMatch in Regex.Matches(match.Groups[1].Value, @"\d+"))
                {
                    tokenText = Regex.Replace(tokenText, @"\b" + innerMatch.Value + @"\b", "\"digit\": \"" + innerMatch +"\",");
                }
            }
            foreach (Match match in Regex.Matches(tokenText, @"""word_group"": \{([^\}]+)\}"))
            {
                foreach (Match innerMatch in Regex.Matches(match.Groups[1].Value, @"((?<=\|)[^\|]+|(?<=\|)[^\|]+(?=\|)|[^\|]+(?=\|))"))
                {
                    foreach(Match newInnerMatch in Regex.Matches(innerMatch.Value, @"(?<!"")(\@|\~)\w+~(?!"")|(?<!"")\b\w+~(?!"")|(?<!"")(\@|\~)\w+\b(?!"")|(?<!"")\b\w+\b(?!"")"))
                    {
                        if (newInnerMatch.Groups[2].Value != null && newInnerMatch.Groups[2].Value == "@")
                            tokenText = Regex.Replace(tokenText, newInnerMatch.Value + @"\b", $"\"item\": \"{newInnerMatch.Value}\",");
                        else
                            tokenText = Regex.Replace(tokenText, @"\b" + newInnerMatch.Value + @"\b", $"\"item\": \"{newInnerMatch.Value}\",");
                    }
                }
            }
            foreach (Match match in Regex.Matches(tokenText, @"(?<!"")(\@|\~)\w+~(?!"")|(?<!"")\b\w+~(?!"")|(?<!"")(\@|\~)\w+\b(?!"")|(?<!"")\b\w+\b(?!"")"))
            {
                if (match.Groups[2].Value != null && match.Groups[2].Value == "@")
                    tokenText = Regex.Replace(tokenText, match.Value + @"\b", $"\"word\": \"{match.Value}\",");
                else
                    tokenText = Regex.Replace(tokenText, @"\b" + match.Value + @"\b", $"\"word\": \"{match.Value}\",");
            }
            tokenText = tokenText.Replace("|", " ").Replace("?", " ").Replace("+", " ").Replace("*", " ");
            tokenText = "{\"rule\": {" + tokenText + "},}";

            tokensList.ForEach(x => { tokenText = replaceTokinizer(tokenText, x); });
            
            try
            {
                return JToken.Parse(tokenText);
            }
            catch (Exception e)
            {
                return null;
                //MessageBox.Show("Ошибка составления правила!!!");
            }
        }

        private string replaceTokinizer(string sourceString, string tokenText)
        {
            int i = 0;
            sourceString = Regex.Replace(sourceString, @"\b" + tokenText + @"\b", match => { i++; return $"{tokenText}{i}"; });
            return sourceString;
        }

        public string basicPhraseBuldier(string tokenText)
        {
            string basicPharse = "";
            
            var rule = ruleRegexTokenization(tokenText);
            if(rule != null)
            {
                foreach (var r in rule.SelectToken("rule"))
                {
                    if (Regex.IsMatch(((JProperty)r).Name.ToString(), @"word_array"))
                    {
                        basicPharse += (((JContainer)r).First.Count() > 0) ? ((JValue)((JContainer)r).First[0]).Value.ToString() : "";
                        basicPharse += " ";
                    }
                    else if (Regex.IsMatch(((JProperty)r).Name.ToString(), @"word_group"))
                    {
                        basicPharse += " ";
                    }
                    else if (Regex.IsMatch(((JProperty)r).Name.ToString(), @"^word"))
                    {
                        basicPharse += ((JProperty)r).Value.ToString() + " ";
                    }
                    basicPharse = Regex.Replace(basicPharse, @"\s+$", "");
                }
            }

            return basicPharse;
        }

        private List<string> tokensList = new List<string>()
        {
            "word_array", "range", "word_group", "item", "word", "digit", "any_word"
        };

        private string ReplaceTokens(string sourceString)
        {
            sourceString = sourceString.Replace(",", " ");
            sourceString = sourceString.Replace("{", "\"range\": {");
            sourceString = sourceString.Replace("}", "},");
            sourceString = sourceString.Replace("[", "\"word_array\": [");
            sourceString = sourceString.Replace("]", "],");
            sourceString = sourceString.Replace("(", "\"word_group\": {");
            sourceString = sourceString.Replace(")", "},");
            sourceString = sourceString.Replace(".", "\"any_word\": {},");
            return sourceString;
        }

        public class ownErrorLog
        {
            public string message { get; set; }
            public int start { get; set; }
            public int end { get; set; }
        }
    }
}
