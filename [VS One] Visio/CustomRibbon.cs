using Microsoft.Office.Tools.Ribbon;
using Office = Microsoft.Office;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using Visio = Microsoft.Office.Interop.Visio;
using Word = Microsoft.Office.Interop.Word;

namespace _VS_One__Visio
{
    public partial class CustomRibbon
    {
        private Visio.Document doc;
        private Visio.Application application;
        private PhrasesFunctions functions;

        private void CustomRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            
        }

        private void getAllPhrases_Click(object sender, RibbonControlEventArgs e)
        {
            application = Globals.ThisAddIn.Application;

            if (application.ActivePage != null)
            {
                Form frm = Application.OpenForms["Phrases"];

                if (frm == null)
                {
                    frm = new Phrases(false);
                    frm.Visible = true;
                    frm.Show();
                    doc = application.ActiveDocument;
                }
                else if (doc == application.ActiveDocument)
                {
                    frm.Focus();
                }
            }
            else
            {
                MessageBox.Show("Нет открытых страниц!");
            }
        }

        private void assFileOpen_Click(object sender, RibbonControlEventArgs e)
        {
            PhrasesFromAss fromAss = new PhrasesFromAss();
            fromAss.Visible = true;
            fromAss.Show();
            fromAss.Focus();
        }

        private void getResult_Click(object sender, RibbonControlEventArgs e)
        {
            application = Globals.ThisAddIn.Application;

            if (application.ActivePage != null)
            {
                Form frm = Application.OpenForms["DiagramResultsForm"];

                if (frm == null)
                {
                    frm = new DiagramResultsForm();
                    frm.Visible = true;
                    frm.Show();
                    doc = application.ActiveDocument;
                }
                else if (doc == application.ActiveDocument)
                {
                    frm.Focus();
                }
            }
            else
            {
                MessageBox.Show("Нет открытых страниц!");
            }
        }

        private void button2_Click(object sender, RibbonControlEventArgs e)
        {
            application = Globals.ThisAddIn.Application;

            if (application.ActivePage != null)
            {
                Form frm = Application.OpenForms["BulderForm"];

                if (frm == null)
                {
                    frm = new BulderForm();
                    frm.Visible = true;
                    frm.Show();
                    doc = application.ActiveDocument;
                }
                else if (doc == application.ActiveDocument)
                {
                    frm.Focus();
                }
            }
            else
            {
                MessageBox.Show("Нет открытых страниц!");
            }
        }

        private void aboutBtn_Click(object sender, RibbonControlEventArgs e)
        {
            AboutVSOne about = new AboutVSOne();
            about.ShowDialog();
        }

        private void graphMnBtn_Click(object sender, RibbonControlEventArgs e)
        {
            string directory = @"\\abc.orl\Shared\Дирекция по информационным технологиям\ОНиСАА\Инструкции\";
            string fileName = "Руководство по оформлению БС";
            openWordDocument(fileName, directory);
        }

        private void addinMnBtn_Click(object sender, RibbonControlEventArgs e)
        {
            string directory = @"\\abc.orl\Shared\Дирекция по информационным технологиям\ОНиСАА\Инструкции\";
            string fileName = "Руководство по надстройке Visio";
            openWordDocument(fileName, directory);
        }

        public void openWordDocument(string partialName, string directoryToSearchIn)
        {
            try
            {
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(directoryToSearchIn);
                System.IO.FileInfo[] filesInDir = directoryInfo.GetFiles("*" + partialName + "*.*");

                if (filesInDir.Length == 1)
                {
                    var wrd = new Word.Application();

                    wrd.Documents.Open(filesInDir[0].FullName);
                    wrd.Visible = true;
                }
                else
                {
                    MessageBox.Show("Не удалось открыть файл!!!");
                }
            }
            catch
            {
                MessageBox.Show("Не удалось открыть файл!!!");
            }
        }

        private void getIntents_Click(object sender, RibbonControlEventArgs e)
        {
            application = Globals.ThisAddIn.Application;

            if (application.ActivePage != null)
            {
                try
                {
                    Visio.Shapes shapes = application.ActivePage.Shapes;

                    var ex = new Excel.Application();

                    ex.Workbooks.Add();
                    Excel._Worksheet worksheet = (Excel.Worksheet)ex.ActiveSheet;

                    worksheet.Cells[1, "A"] = "source_state";
                    worksheet.Cells[1, "B"] = "selected_hypothesis";
                    worksheet.Cells[1, "C"] = "target_state";

                    List<string> output = new List<string>();

                    foreach (Visio.Shape shp in shapes)
                    {
                        if (shp.NameU.IndexOf("Process") > -1)
                        {
                            string intent = functions.getIntentsFromShapes(shp, shapes);

                            string[] intentArr = functions.splitByUpperCase(intent);

                            for (int i = 0; i < intentArr.Length; i++)
                            {
                                if (intentArr[i] != "" & intentArr[i] != " "
                                    & intentArr[i] != "\n"
                                    & intentArr[i] != "\\" & intentArr[i].IndexOf("<") == -1)
                                {
                                    output.Add(intentArr[i]);
                                }
                            }
                        }
                    }

                    var newOutput = functions.removeDuplicates(output);

                    var index = 2;

                    foreach (string str in newOutput)
                    {
                        if (str != string.Empty)
                        {
                            worksheet.Cells[index, "A"] = "state";
                            worksheet.Cells[index, "B"] = str;
                            worksheet.Cells[index, "C"] = "null";

                            index++;
                        }
                    }

                    ex.Visible = true;
                }
                catch
                {
                    MessageBox.Show("Не удалось открыть Excel!\nВозможно данная программа не установлена на Вашем компьютере.");
                }
            }
            else
            {
                MessageBox.Show("Нет открытых страниц!");
            }
        }

        private void update_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                string directoryToSearchIn = @"\\abc.orl\Shared\Дирекция по информационным технологиям\ОНиСАА\Инструкции\Надстройка MS Visio\[VS One] Visio";
                string partialName = "[VS One] Visio.vsto";

                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(directoryToSearchIn);
                System.IO.FileInfo[] filesInDir = directoryInfo.GetFiles("*" + partialName + "*.*");

                var fileDate = filesInDir[0].CreationTime;

                Process.Start(filesInDir[0].FullName);
            }
            catch
            {
                MessageBox.Show("Невозможно обновить надстройку");
            }
        }

        private void getOnlyElaboratedPhrases_Click(object sender, RibbonControlEventArgs e)
        {
            application = Globals.ThisAddIn.Application;

            if (application.ActivePage != null)
            {
                Form frm = Application.OpenForms["Phrases"];

                if (frm == null)
                {
                    frm = new Phrases(true);
                    frm.Visible = true;
                    frm.Show();
                    doc = application.ActiveDocument;
                }
                else if (doc == application.ActiveDocument)
                {
                    frm.Focus();
                }
            }
            else
            {
                MessageBox.Show("Нет открытых страниц!");
            }
        }

        private void formSet_Click(object sender, RibbonControlEventArgs e)
        {
            application = Globals.ThisAddIn.Application;

            if (application.ActivePage != null)
            {
                Form frm = Application.OpenForms["LinksForm"];

                if (frm == null)
                {
                    frm = new LinksForm(application.ActivePage.Shapes);
                    frm.Visible = true;
                    frm.Show();
                    doc = application.ActiveDocument;
                }
                else if (doc == application.ActiveDocument)
                {
                    frm.Focus();
                }
            }
            else
            {
                MessageBox.Show("Нет открытых страниц!");
            }
        }

        private void getParametrsFromText_Click(object sender, RibbonControlEventArgs e)
        {
            application = Globals.ThisAddIn.Application;

            if (application.ActivePage != null)
            {
                Visio.Shapes shapes = application.ActivePage.Shapes;

                Form frm = Application.OpenForms["VariablesFromDiagram"];

                if (frm == null)
                {
                    frm = new VariablesFromDiagram(application.ActivePage.Shapes);
                    frm.Visible = true;
                    frm.Show();
                    doc = application.ActiveDocument;
                }
                else if (doc == application.ActiveDocument)
                {
                    frm.Focus();
                }
            }
            else
            {
                MessageBox.Show("Нет открытых страниц!");
            }
        }

        private void button3_Click(object sender, RibbonControlEventArgs e)
        {
            
        }

        private void button4_Click(object sender, RibbonControlEventArgs e)
        {
            application = Globals.ThisAddIn.Application;

            Visio.Shapes shapes = (application.ActivePage != null)? application.ActivePage.Shapes : null;

            Form frm = Application.OpenForms["MainScriptBuilder"];

            if (frm == null)
            {
                frm = new MainScriptBuilder(shapes);
                frm.Visible = true;
                frm.Show();
                doc = application.ActiveDocument;
            }
            else if (doc == application.ActiveDocument)
            {
                frm.Focus();
            }
            
        }

        private void button3_Click_1(object sender, RibbonControlEventArgs e)
        {
            application = Globals.ThisAddIn.Application;

            if (application.ActiveWindow != null && application.ActiveWindow.Selection.Count > 0)
            {
                MoveForm move = new MoveForm(application.ActiveWindow.Selection);

                move.ShowDialog();
            }
        }

        private void button2_Click_1(object sender, RibbonControlEventArgs e)
        {
            application = Globals.ThisAddIn.Application;

            if (application.ActivePage != null)
            {
                Visio.Shapes shapes = application.ActivePage.Shapes;

                foreach (Visio.Shape shp in shapes) if (shp.Hyperlinks.Count > 0) foreach(Visio.Hyperlink link in shp.Hyperlinks) link.Delete();
            }
            else
            {
                MessageBox.Show("Нет открытых страниц!");
            }
        }
    }
}
