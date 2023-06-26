using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public partial class WorkWithPhrases : Form
    {
        string text;
        string textWithParse;
        ScinitillaStyles scinitillaStyles = new ScinitillaStyles();

        public WorkWithPhrases(string source)
        {
            InitializeComponent();
            text = source;
            textWithParse = new JsonParse().parseConverting(source);
            scinitillaStyles.scintillaSetInitStyleJson(scintilla1);
        }

        private List<string> getListTokensFromUser()
        {
            string textWithout = Regex.Replace(richTextBox1.Text, @"<[^>]+>", "", RegexOptions.Singleline);
            textWithout = Regex.Replace(textWithout, @"\[[^\]]+\]", "", RegexOptions.Singleline);
            textWithout = Regex.Replace(textWithout, @"\.\{[^\}]+\}", "", RegexOptions.Singleline);
            textWithout = Regex.Replace(textWithout, @"\@\w+\b|~\w+\b|\b\w+~|~\w+~|\b\w+\@", "", RegexOptions.Singleline);
            List<string> tokensFromUser = Regex.Split(textWithout, @"\n|\r\n|\s|,").Where(x => !String.IsNullOrWhiteSpace(x)).ToList();
            tokensFromUser.Sort();
            return tokensFromUser;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string phrases = "";
            List<string> expressions = Regex.Matches(richTextBox1.Text, @"<([^>]+)>").Cast<Match>().Where(x => !x.Value.Contains("=")).Select(match => match.Groups[1].Value).ToList();
            expressions.Sort();

            List<string> tokensFromUser = getListTokensFromUser();

            try
            {
                var expressionsTokens = JToken.Parse(textWithParse).SelectToken("expressions");
                if (expressionsTokens != null)
                {
                    expressions.ForEach(x =>
                    {
                        if (expressionsTokens.SelectToken(x) != null) phrases += $"{expressionsTokens.SelectToken(x).Parent.ToString().Replace("  ", "\t")},\n";
                    });
                }
                var phrasesTokens = JToken.Parse(textWithParse).SelectToken("phrases");
                if (phrasesTokens != null)
                {
                    tokensFromUser.ForEach(x =>
                    {
                        if (phrasesTokens.SelectToken(x) != null) phrases += $"{phrasesTokens.SelectToken(x).Parent.ToString().Replace("  ", "\t")},\n";
                    });
                }
            }
            catch
            {

            }
            scintilla1.Text = phrases;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string newText = text;
            List<string> tokensFromUser = getListTokensFromUser();
            if(!String.IsNullOrEmpty(textBox1.Text)) 
                tokensFromUser.ForEach(x => { newText = Regex.Replace(newText, @"(?<!\.)\b" + x + @"\b", $"{textBox1.Text}.{x}", RegexOptions.Singleline); });
            scintilla1.Text = newText;
        }
    }
}
