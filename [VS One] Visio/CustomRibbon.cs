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
using System.Drawing;
using System.Text.RegularExpressions;

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

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            functions = new PhrasesFunctions();
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
                    List<(string, string, string)> excelElemList = new List<(string, string, string)>();

                    shapes.Cast<Visio.Shape>().ToList().Where(x => x.NameU.IndexOf("Dynamic connector") > -1 && !String.IsNullOrEmpty(x.Text)).ToList().ForEach(elem =>
                    {

                        var rE = elem.GluedShapes(Visio.VisGluedShapesFlags.visGluedShapesAll2D, "", elem);
                        foreach (var re in rE)
                        {
                            int ids = Convert.ToInt32(re);
                            Visio.Shape newShape = shapes.ItemFromID[ids];
                            if (Regex.IsMatch(newShape.Text, @"[С с]лушаем"))
                            {
                                List<string> listText = Regex.Split(elem.Text, @"(\\|\n)").Cast<string>().ToList();
                                listText.ForEach(x => { excelElemList.Add((newShape.Text, x, "null")); });
                            }
                        }
                    });

                    var index = 2;
                    excelElemList = excelElemList.OrderBy(x => x.Item1).ToList();
                    excelElemList.Where(x => !String.IsNullOrWhiteSpace(x.Item2)).ToList().ForEach(x =>
                    {
                        worksheet.Cells[index, "A"] = x.Item1;
                        worksheet.Cells[index, "B"] = x.Item2;
                        worksheet.Cells[index, "C"] = x.Item3;

                        index++;
                    });

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

        private void button6_Click(object sender, RibbonControlEventArgs e)
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

                    worksheet.Cells[1, "A"] = "selected_hypothesis";

                    List<string> output = new List<string>();
                    List<(string, string, string)> excelElemList = new List<(string, string, string)>();

                    shapes.Cast<Visio.Shape>().ToList().Where(x => x.NameU.IndexOf("Dynamic connector") > -1 && !String.IsNullOrEmpty(x.Text)).ToList().ForEach(elem =>
                    {

                        var rE = elem.GluedShapes(Visio.VisGluedShapesFlags.visGluedShapesAll2D, "", elem);
                        foreach (var re in rE)
                        {
                            int ids = Convert.ToInt32(re);
                            Visio.Shape newShape = shapes.ItemFromID[ids];
                            if (Regex.IsMatch(newShape.Text, @"[С с]лушаем"))
                            {
                                List<string> listText = Regex.Split(elem.Text, @"(\\|\n)").Cast<string>().ToList();
                                listText.ForEach(x => { output.Add(x); });
                            }
                        }
                    });

                    var index = 2;
                    output = output.Distinct().ToList();
                    output.Where(x => !String.IsNullOrWhiteSpace(x)).ToList().ForEach(x =>
                    {
                        worksheet.Cells[index, "A"] = x;
                        index++;
                    });

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
            openPharsesForm(PhrasesMode.OnlyElaboratedPhrases);
        }

        private void getAllPhrases_Click(object sender, RibbonControlEventArgs e)
        {
            openPharsesForm(PhrasesMode.MainMode);
        }

        private void button5_Click(object sender, RibbonControlEventArgs e)
        {
            openPharsesForm(PhrasesMode.OnlyNewBlock);
        }

        private void openPharsesForm(PhrasesMode mode)
        {
            application = Globals.ThisAddIn.Application;

            if (application.ActivePage != null)
            {
                Form frm = Application.OpenForms["Phrases"];

                if (frm == null)
                {
                    frm = new Phrases(mode);
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

            MainScriptBuilder form = new MainScriptBuilder(shapes);
            form.Show();

            /*Form frm = Application.OpenForms["MainScriptBuilder"];

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
            }*/
            
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

        private void splitButton1_Click(object sender, RibbonControlEventArgs e)
        {

        }

        private void button7_Click(object sender, RibbonControlEventArgs e)
        {
            application = Globals.ThisAddIn.Application;

            if (application.ActivePage != null)
            {
                Visio.Shapes shapes = application.ActivePage.Shapes;
                shapes.Cast<Visio.Shape>().ToList().Where(x => x.NameU.IndexOf("Process") > -1 && !String.IsNullOrEmpty(x.Text)).ToList().ForEach(elem =>
                {
                    if (Regex.IsMatch(elem.Text.ToLower(), @"(основная фраза:|фраза для повтора:)"))
                    {
                        Visio.Characters chars = elem.Characters;
                        chars.Text = elem.Text;
                        Regex.Matches(elem.Text.ToLower(), @"(основная фраза:|фраза для повтора:)").Cast<Match>().ToList().ForEach(x =>
                        {
                            int start = x.Index;
                            chars.Begin = start;
                            chars.End = start + x.Length;
                            chars.CharProps[2] = (short)Visio.VisCellVals.visBold;
                        });
                    }
                });
            }
        }
    }
}
