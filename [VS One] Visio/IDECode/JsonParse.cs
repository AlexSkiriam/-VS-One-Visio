using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace _VS_One__Visio
{
    class JsonParse
    {
        public string parseConverting(string jsonValue)
        {
            string newText = Regex.Replace(jsonValue.Replace(Environment.NewLine, "___this_is_new_line___"), @"<<<CODE((?!CODE).)*CODE", m => {
                string value = m.Groups[0].Value.Replace("\"", "\\\"");
                return "{ \"code\": \"" + value + "\" }";
            });
            newText = Regex.Replace(newText, @"<<<NER((?!NER).)*NER", m => {
                string value = m.Groups[0].Value.Replace("\"", "\\\"");
                return "{ \"ner\": \"" + value + "\" }";
            });
            return newText.Replace("___this_is_new_line___", Environment.NewLine);
        }

        public ownErrorLog parseJson(string jsonValue, ScintillaNET.Scintilla scintilla)
        {
            ownErrorLog log = null;

            string newJsonValue = parseConverting(jsonValue);

            try
            {
                List<string> states = new List<string>();

                var json = JToken.Parse(newJsonValue);

                if (json.SelectToken("states") != null)
                {
                    foreach (var tokens in json.SelectToken("states"))
                    {
                        if (!states.Contains(tokens.SelectToken("name").ToString())) states.Add(tokens.SelectToken("name").ToString());
                        else throw new Exception($"Стейт уже существует в скрипте: {tokens.SelectToken("name").ToString()}");
                    }
                    foreach (var tokens in json.SelectToken("special_states"))
                    {
                        if (!states.Contains(tokens.SelectToken("name").ToString())) states.Add(tokens.SelectToken("name").ToString());
                        else throw new Exception($"Стейт уже существует в скрипте: {tokens.SelectToken("name").ToString()}");
                    }
                    foreach (var tokens in json.SelectToken("global_listening_states"))
                    {
                        if (!states.Contains(tokens.SelectToken("name").ToString())) states.Add(tokens.SelectToken("name").ToString());
                        else throw new Exception($"Стейт уже существует в скрипте: {tokens.SelectToken("name").ToString()}");
                    }
                    foreach (var tokens in json.SelectToken("on_timer_states"))
                    {
                        if (!states.Contains(tokens.SelectToken("name").ToString())) states.Add(tokens.SelectToken("name").ToString());
                        else throw new Exception($"Стейт уже существует в скрипте: {tokens.SelectToken("name").ToString()}");
                    }
                }
                MessageBox.Show("Ошибок не найдено");
            }
            catch (Exception e)
            {
                string errorText = e.Message + "\n\n" + e.StackTrace;
                MessageBox.Show(errorText);

                int start = 0;
                int end = 0;

                if (e.GetType().Name == "JsonReaderException")
                {
                    var linesOnScreen = scintilla.LinesOnScreen - 2;

                    start = scintilla.Lines[((JsonReaderException)e).LineNumber - (linesOnScreen / 2)].Position;
                    end = scintilla.Lines[((JsonReaderException)e).LineNumber + (linesOnScreen / 2)].Position;
                    scintilla.ScrollRange(start, end);
                }

                log = new ownErrorLog()
                {
                    message = errorText,
                    start = start,
                    end = end
                };
            }

            return log;
        }

        public class ownErrorLog
        {
            public string message { get; set; }
            public int start { get; set; }
            public int end { get; set; }
        }
    }
}
