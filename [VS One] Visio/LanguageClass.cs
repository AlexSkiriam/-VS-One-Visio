using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScintillaNET;
using keyw;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace _VS_One__Visio
{
    class LanguageClass
    {
        private string fromRuToEng(string source)
        {
            ruseng switcherRuEng = new ruseng();
            return switcherRuEng.layout(source);
        }

        private string fromEngToRu(string source)
        {
            engrus switcherEngRu = new engrus();
            return switcherEngRu.layout(source);
        }

        ///<summary>
        ///Switching selected text language
        ///</summary>
        public void SelectedTextLanguageSwitcher(Scintilla scintilla)
        {
            if (scintilla.SelectedText != null)
            {
                if (Regex.IsMatch(scintilla.SelectedText, @"[А-я]+"))
                {
                    string rueng = fromRuToEng(scintilla.SelectedText);
                    scintilla.ReplaceSelection(rueng);
                }

                if (Regex.IsMatch(scintilla.SelectedText, @"[A-z]+"))
                {
                    string engru = fromEngToRu(scintilla.SelectedText);
                    scintilla.ReplaceSelection(engru);
                }
            }
        }

        ///<summary>
        ///Switching selected text language
        ///</summary>
        public void SelectedTextLanguageSwitcher(TextBox textBox)
        {
            if (textBox.SelectedText != null)
            {
                if (Regex.IsMatch(textBox.SelectedText, @"[А-я]+"))
                {
                    textBox.SelectedText = fromRuToEng(textBox.SelectedText);
                }

                if (Regex.IsMatch(textBox.SelectedText, @"[A-z]+"))
                {
                    textBox.SelectedText = fromEngToRu(textBox.SelectedText);
                }
            }
        }
    }
}
