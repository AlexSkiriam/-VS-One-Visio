using AutocompleteMenuNS;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScintillaNET;
using System.Reflection;
using Microsoft.CSharp;

namespace _VS_One__Visio
{
    public partial class CSharpCodeForm : Form
    {
        LanguageClass language = new LanguageClass();
        int lastCaretPos = 0;
        NewTextEditorForm editor;

        string storeElements = "";
        string typesElements = "";

        ScintillaFunc scinitillaFunc = new ScintillaFunc();
        ScinitillaStyles scinitillaStyles = new ScinitillaStyles();

        public static List<string> autoCompleteStore = new List<string>();
        public static List<string> autoCompleteTypes = new List<string>();

        public CSharpCodeForm(NewTextEditorForm source, string classValue, string typesValue, List<string> listStore, List<string> listTypes)
        {
            InitializeComponent();

            editor = source;
            storeElements = classValue;
            typesElements = typesValue;
            autoCompleteStore = listStore;
            autoCompleteTypes = listTypes;

            scinitillaStyles.scintillaSetInitStyleCSharp(scintilla1);
            autocompleteMenu1.TargetControlWrapper = new ScintillaWrapper(scintilla1);
            BuildAutocompleteMenu();

            scintilla1.InsertCheck += Scintilla1_InsertCheck;
            scintilla1.CharAdded += Scintilla1_CharAdded;
            scintilla1.KeyPress += Scintilla1_KeyPress;
            scintilla1.UpdateUI += Scintilla1_UpdateUI;
            scintilla1.KeyDown += scintilla1_KeyDown;
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
            if ((e.Change & UpdateChange.Selection) > 0)
            {
                var currentPos = scintilla1.CurrentPosition;
                var currentLinePos = scintilla1.LineFromPosition(currentPos) + 1;
                var linePosition = currentPos - scintilla1.Lines[scintilla1.LineFromPosition(currentPos)].Position;
                toolStripStatusLabel1.Text = "Строка: " + currentLinePos + " Позиция: " + (linePosition + 1);
            }
        }

        private void scintilla1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S) compileCode();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            compileCode();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            compileCode(true);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            editor.formResult = scintilla1.Text;
            this.Close();
        }

        public void compileCode(bool needRunCode = false)
        {
            richTextBox1.Text = "";
            string mainText = "Сборка:\n";
            string Output = "Out.exe";

            var CSHarpProvider = CSharpCodeProvider.CreateProvider("CSharp");
            CompilerParameters compilerParams = new CompilerParameters()
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
            };
            compilerParams.ReferencedAssemblies.AddRange(new string[]
            {
                "System.dll",
                "System.Core.dll",
                "System.Net.dll",
            });
            CompilerResults compilerResult = CSHarpProvider.CompileAssemblyFromSource(compilerParams, sourceCode(scintilla1.Text));

            if (compilerResult.Errors.Count == 0)
            {
                richTextBox1.Text += mainText;
                try
                {
                    string r = compilerResult.CompiledAssembly.GetType("VoiceRobot.Program").GetMethod("Main").Invoke(null, null).ToString();
                    richTextBox1.Text += "========== Успешно ==========\n";
                    richTextBox1.Text += r;
                }
                catch (Exception e)
                {
                    richTextBox1.Text += e.InnerException.Message + "rn" + e.InnerException.StackTrace;
                }
            }
            else
            {
                foreach (var oline in compilerResult.Output)
                    richTextBox1.Text += oline;
            }
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
                                Store store = new Store();"
                                + userCode +
                            @"}
                        }"
                            + userTypes +
                        "public class Store\n{"
                            + store +
                        @"}
                    }";
        }

        private string sourceCode(string usercode)
        {
            return @"using System;
                using System.IO;
                using System.Net;
                using System.Threading;
                using System.Linq;
                using System.Text.RegularExpressions;
                using System.Collections.Generic;

                namespace VoiceRobot
                {
                    public class Program
                    {
                        public static void Main()
                        {
                            "
                            + usercode + @"
                        }
                    }
                }";
        }

        /* Keywords */
        private const string cSharp = "";
        private const string typesCSharp = @"string int double List<> Dictionary<> DateTime() String.IsNullOrEmpty() for() if() else() foreach()
            Console Console.WriteLine(); Console.ReadLine(); decimal class const int namespace partial public static using void";

        private void BuildAutocompleteMenu()
        {
            autocompleteMenu1.SetAutocompleteItems(new DynamicCollection(scintilla1));
            autocompleteMenu1.AllowsTabKey = true;
        }
        private static List<string> BuildAutoShowList()
        {
            List<string> list = new List<string>();

            list.AddRange(cSharp.Split(' '));
            list.AddRange(typesCSharp.Split(' '));
            list.AddRange(autoCompleteStore);

            return list.OrderBy(m => m).ToList();
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

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        private void scintilla1_KeyDown_1(object sender, KeyEventArgs e)
        {
            if(e.Control && e.KeyCode == Keys.P) language.SelectedTextLanguageSwitcher(scintilla1);
        }
    }
}
