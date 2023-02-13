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

namespace _VS_One__Visio
{
    public partial class CSharpCodeForm : Form
    {
        int lastCaretPos = 0;
        TextEditorForm editor;

        string storeElements = "";
        string typesElements = "";

        ScintillaFunc scinitillaFunc = new ScintillaFunc();
        ScinitillaStyles scinitillaStyles = new ScinitillaStyles();

        public static List<string> autoCompleteStore = new List<string>();
        public static List<string> autoCompleteTypes = new List<string>();

        public CSharpCodeForm(TextEditorForm source, string classValue, string typesValue, List<string> listStore, List<string> listTypes)
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

            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = Output;
            parameters.ReferencedAssemblies.Add("System.dll");
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, codeToCompile(scintilla1.Text, storeElements, typesElements));

            if (results.Errors.Count > 0)
            {
                string errorText = "";
                foreach (CompilerError CompErr in results.Errors)
                {
                    string windowLine = Convert.ToString(Convert.ToInt32(CompErr.Line) - 10);

                    errorText = errorText +
                                "Строка " + windowLine +
                                ", Код ошибки: " + CompErr.ErrorNumber +
                                ", '" + CompErr.ErrorText + "';" +
                                Environment.NewLine + Environment.NewLine;
                }
                //MessageBox.Show(errorText);
                richTextBox1.Text += mainText + "========== С ошибками ==========\n" + errorText;
            }
            else
            {
                //MessageBox.Show("Успешно");
                richTextBox1.Text += mainText + "========== Успешно ==========";
                if (needRunCode) Process.Start(Output);
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
    }
}
