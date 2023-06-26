using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Visio = Microsoft.Office.Interop.Visio;
using Excel = Microsoft.Office.Interop.Excel;

namespace _VS_One__Visio
{
    public partial class DiagramResultsForm : Form
    {
        PhrasesFunctions functions = new PhrasesFunctions();
        Dictionary<string, Visio.Shape> dictShape;

        public string getPhrase()
        {
            return (listView1.SelectedItems.Count == 1) ? listView1.SelectedItems[0].SubItems[1].Text : "";
        }

        public DiagramResultsForm()
        {
            InitializeComponent();
        }

        private void DiagramResultsForm_OnLoad(object sender, EventArgs e)
        {
            listView1.FullRowSelect = true;
            listView1.MouseUp += new MouseEventHandler(listView1_MouseClick);
            setResultList();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem != null && listView1.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    ContextMenu menu = new ContextMenu();

                    MenuItem toSelectedShape = new MenuItem("Перейти к фигуре");
                    toSelectedShape.Click += delegate (object sender2, EventArgs e2) { goToSelectedShape(); };
                    menu.MenuItems.Add(toSelectedShape);

                    MenuItem toClipboard = new MenuItem("Скопировать фразу");
                    toClipboard.Click += delegate (object sender2, EventArgs e2) { addPhraseToClipBoard(); };
                    menu.MenuItems.Add(toClipboard);

                    menu.Show(listView1, new Point(e.X, e.Y));
                }
            }
        }

        private void addPhraseToClipBoard()
        {
            Clipboard.SetText(getPhrase());
        }

        private void goToSelectedShape()
        {
            var application = Globals.ThisAddIn.Application;
            var window = application.ActiveWindow;

            if (!String.IsNullOrEmpty(getPhrase()))
            {
                var shp = dictShape[getPhrase()];

                window.DeselectAll();
                application.Settings.CenterSelectionOnZoom = true;
                window.Select(application.ActivePage.Shapes.ItemFromID[shp.ID], (short)Visio.VisSelectArgs.visSelect);
                window.Zoom = 1;

                this.Activate();
            }
        }

        public void setResultList()
        {
            listView1.Items.Clear();
            dictShape = new Dictionary<string, Visio.Shape>();

            Visio.Shapes shapes = Globals.ThisAddIn.Application.ActivePage.Shapes;
            List<Visio.Shape> list = getAllResults(shapes);

            label2.Text = "-";
            label2.Text = list.Count.ToString();

            int i = 1;
            foreach (Visio.Shape shp in list)
            {
                string newText = Regex.Replace(shp.Text, @"^\w+\s*\w+:?\s*", "");
                newText = Regex.Replace(newText, @"(^[^\w]|[^\w]$)", "");

                if (!dictShape.ContainsKey(newText))
                {
                    ListViewItem item = new ListViewItem(i.ToString());

                    item.SubItems.Add(newText);
                    item.SubItems.Add(checkedDuples(list, newText).ToString());
                    listView1.Items.Add(item);

                    dictShape.Add(newText, shp);

                    i++;
                }
            }
        }

        public List<Visio.Shape> getAllResults(Visio.Shapes shapes)
        {
            List<Visio.Shape> list = new List<Visio.Shape>();

            foreach (Visio.Shape shp in shapes)
            {
                if (shp.NameU.IndexOf("Start/End") > -1)
                {
                    if (!String.IsNullOrEmpty(shp.Text))
                    {
                        string text = functions.cleanSurplusSpaces(shp.Text);
                        Match match = Regex.Match(text, @"(\w\s*\w+\s*(\w+\.?\s*)?№*\s*(\d\.*)+|^\w+\s*\w+\Z)");

                        if (!match.Success) list.Add(shp);
                    }
                }
            }
            return list;
        }

        public int checkedDuples(List<Visio.Shape> list, string text)
        {
            int dup = 0;
            foreach (Visio.Shape shape in list)
            {
                if (shape.Text == text) dup++;
            }
            return (dup > 1) ? dup : 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            setResultList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var ex = new Excel.Application();

                ex.Workbooks.Add();
                Excel._Worksheet worksheet = (Excel.Worksheet)ex.ActiveSheet;

                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    string textInCell = listView1.Items[i].SubItems[1].Text;
                    worksheet.Cells[i + 1, "A"] = textInCell;
                    worksheet.Cells[i + 1, "A"].WrapText = false;
                }

                ex.Visible = true;
            }
            catch
            {
                MessageBox.Show("Не удалось открыть Excel!\nВозможно данная программа не установлена на Вашем компьютере.");
            }
        }
    }
}
