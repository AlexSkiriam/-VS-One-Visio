using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Visio = Microsoft.Office.Interop.Visio;
using Excel = Microsoft.Office.Interop.Excel;

namespace _VS_One__Visio
{
    public partial class Phrases : Form
    {
        public PhrasesMode m;

        public Phrases(PhrasesMode oep)
        {
            InitializeComponent();
            m = oep;
        }

        TextAnalysis textAnalysis = new TextAnalysis();
        UsingText usingText = new UsingText();
        PhrasesFunctions phrasesFunctions = new PhrasesFunctions();

        public int subIndex;

        public static string selectedPhrase { get; set; }
        public bool editResult = false;

        public Dictionary<string, Visio.Shape> dictShape;
        public Dictionary<string, Visio.Shape> dictShapeA;

        private bool resizing = false;

        public string getPhrase()
        {
            return (listView1.SelectedItems.Count == 1) ? listView1.SelectedItems[0].SubItems[2].Text : "";
        }

        public string getPhraseA()
        {
            return (listView2.SelectedItems.Count == 1) ? listView2.SelectedItems[0].SubItems[1].Text : "";
        }

        private void ListView_SizeChanged(object sender, EventArgs e)
        {
            if (!resizing)
            {
                resizing = true;

                ListView listView = sender as ListView;
                if (listView != null)
                {
                    float totalColumnWidth = 0;

                    for (int i = 0; i < listView.Columns.Count; i++)
                    {
                        totalColumnWidth += Convert.ToInt32(listView.Columns[i].Tag);
                    }

                    for (int i = 0; i < listView.Columns.Count; i++)
                    {
                        float colPercentage = (Convert.ToInt32(listView.Columns[i].Tag) / totalColumnWidth);
                        listView.Columns[i].Width = (int)(colPercentage * listView.ClientRectangle.Width);
                    }
                }
            }

            resizing = false;
        }

        private void Phrases_Load(object sender, EventArgs e)
        {
            listView1.FullRowSelect = true;
            listView1.MouseUp += new MouseEventHandler(listView1_MouseClick);
            setListElements();

            listView2.FullRowSelect = true;
            listView2.MouseUp += new MouseEventHandler(listView2_MouseClick);
            listView2.MouseDoubleClick += new MouseEventHandler(listView2_MouseDoubleClick);
            checkBlocks();

            label6.Visible = false;
            label7.Visible = false;
            checkBox2.Visible = false;
            checkBox3.Visible = false;
            checkBox4.Visible = false;
            comboBox1.Visible = false;

            /*listView1.SizeChanged += new EventHandler(ListView_SizeChanged);
            listView2.SizeChanged += new EventHandler(ListView_SizeChanged);*/
        }

        private void Phrases_Closed(object sender, FormClosedEventArgs e)
        {
            Form frm = Application.OpenForms["SearchForm"];

            if (frm != null) frm.Close();
        }

        private void Phrases_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                Form frm = Application.OpenForms["SearchForm"];

                if (frm == null)
                {
                    frm = new SearchForm(this, listView1, 2);
                    frm.Visible = true;
                    frm.Show();
                    frm.TopMost = true;
                }
                else
                {
                    frm.Focus();
                }
            }
        }

        public void setListElements(bool ValidationNeeded = true)
        {
            listView1.Items.Clear();

            Visio.Shapes shapes = Globals.ThisAddIn.Application.ActivePage.Shapes;

            label2.Text = "-";
            label2.Text = textAnalysis.phrasesCount(shapes).ToString();

            try
            {
                List<PhrasesArray> listOfPhrases = phrasesFunctions.getAllPhrases(shapes, ValidationNeeded, m);

                dictShape = new Dictionary<string, Visio.Shape>();

                listOfPhrases.ForEach(element =>
                {
                    if (!dictShape.ContainsKey(element.phrase))
                    {
                        ListViewItem item = new ListViewItem(element.phraseNumber.ToString());
                        item.SubItems.Add(element.phraseIntent);
                        item.SubItems.Add(element.phrase);
                        item.SubItems.Add(element.nameSpace);
                        listView1.Items.Add(item);

                        dictShape.Add(element.phrase, element.phraseShape);
                    }
                });
            }
            catch
            {
                MessageBox.Show("Схема не соответствует требованиям оформления блок-схем!!!");
            }
        }

        private void makeFile_Click(object sender, EventArgs e)
        {
            try
            {
                var ex = new Excel.Application();

                ex.Workbooks.Add();
                Excel._Worksheet worksheet = (Excel.Worksheet)ex.ActiveSheet;

                subIndex = 0;

                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    string intent = listView1.Items[i].SubItems[1].Text;
                    string phrase = listView1.Items[i].SubItems[2].Text;
                    string nameSpace = listView1.Items[i].SubItems[3].Text;

                    if (checkBox1.Checked && checkBox4.Checked)
                        if (listView1.Items[i].Checked)
                            setSplitedTextInExcelCellls(worksheet, i, intent, phrase, nameSpace);
                        else
                            setTextInExcelCells(worksheet, i + subIndex, intent, phrase, nameSpace);
                    else if (checkBox1.Checked)
                        setSplitedTextInExcelCellls(worksheet, i, intent, phrase, nameSpace);
                    else
                        setTextInExcelCells(worksheet, i, intent, phrase, nameSpace);
                }

                ex.Visible = true;
            }
            catch
            {
                MessageBox.Show("Не удалось открыть Excel!\nВозможно данная программа не установлена на Вашем компьютере.");
            }
        }

        private void setSplitedTextInExcelCellls(Excel._Worksheet worksheet, int index, string intent, string mainText, string nameSpace)
        {
            List<string> splitStringList = new List<string>(
                (checkBox2.Checked) ? mainText.Split(comboBox1.Text.ToCharArray()) : Regex.Split(mainText, @comboBox1.Text)
            );

            splitStringList.Where(element => !String.IsNullOrEmpty(element)).ToList().ForEach(elem =>
            {
                setTextInExcelCells(worksheet, index + subIndex, intent, elem, nameSpace);
                subIndex++;
            });
            subIndex--;
        }

        private void setTextInExcelCells(Excel._Worksheet worksheet, int index, string intent, string mainText, string nameSpace)
        {
            string textInCell = $"{intent} {mainText}";
            worksheet.Cells[index + 1, "A"] = textInCell;
            worksheet.Cells[index + 1, "A"].WrapText = false;

            worksheet.Cells[index + 1, "B"] = nameSpace;
            worksheet.Cells[index + 1, "B"].WrapText = false;
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

                    MenuItem removeItem = new MenuItem("Удалить строку");
                    removeItem.Click += delegate (object sender2, EventArgs e2) { listView1.FocusedItem.Remove(); };
                    menu.MenuItems.Add(removeItem);

                    MenuItem toClipboard = new MenuItem("Скопировать фразу");
                    toClipboard.Click += delegate (object sender2, EventArgs e2) { addPhraseToClipBoard(); };
                    menu.MenuItems.Add(toClipboard);

                    MenuItem setClipboradTextSplited = new MenuItem("Создать текст для редактора (разделение параметрами)");
                    setClipboradTextSplited.Click += delegate (object sender2, EventArgs e2) { ToClipBoardSplited(); };
                    menu.MenuItems.Add(setClipboradTextSplited);

                    menu.Show(listView1, new Point(e.X, e.Y));
                }
            }
        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = listView2.HitTest(e.X, e.Y);
            ListViewItem item = info.Item;

            if (item != null)
            {
                var application = Globals.ThisAddIn.Application;
                var window = application.ActiveWindow;

                var shp = dictShapeA[item.SubItems[1].Text];

                window.DeselectAll();
                application.Settings.CenterSelectionOnZoom = true;
                window.Select(application.ActivePage.Shapes.ItemFromID[shp.ID], (short)Visio.VisSelectArgs.visSelect);
                window.Zoom = 1;

                this.Activate();
            }
        }

        private void listView2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView2.FocusedItem != null && listView2.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    ContextMenu menu = new ContextMenu();

                    MenuItem editSelectedShape = new MenuItem("Редактировать блок");
                    editSelectedShape.Click += delegate (object sender2, EventArgs e2) { editSelected(); };
                    menu.MenuItems.Add(editSelectedShape);

                    menu.Show(listView2, new Point(e.X, e.Y));
                }
            }
        }

        private void addPhraseToClipBoard()
        {
            Clipboard.SetText(getPhrase());
        }

        private void editSelected()
        {
            var application = Globals.ThisAddIn.Application;

            if (!String.IsNullOrEmpty(getPhraseA()))
            {
                Visio.Shape shape = dictShapeA[getPhraseA()];

                EditForm edtFrm = new EditForm(shape, this);
                edtFrm.ShowDialog();

                if (editResult)
                {
                    setListElements();
                    checkBlocks();
                }
            }
        }

        private void ToClipBoardSplited()
        {
            string splitedTExt = usingText.splittedText(getPhrase(), "");
            Clipboard.SetText(splitedTExt);
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

        private void button1_Click(object sender, EventArgs e)
        {
            setListElements();
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

        public void checkBlocks()
        {
            label5.Text = "-";
            listView2.Items.Clear();
            dictShapeA = new Dictionary<string, Visio.Shape>();

            List<Visio.Shape> list = textAnalysis.getShapesWithoutNumber(Globals.ThisAddIn.Application.ActivePage.Shapes);
            label5.Text = list.Count.ToString();

            List<PhrasesArray> listOfPhrases = phrasesFunctions.getAllPhrases(Globals.ThisAddIn.Application.ActivePage.Shapes, true, m);

            List<Visio.Shape> validateList = new List<Visio.Shape>();

            foreach (var element in listOfPhrases)
            {
                validateList.Add(element.phraseShape);
            }

            foreach(Visio.Shape shp in Globals.ThisAddIn.Application.ActivePage.Shapes)
            {
                if (shp.NameU.IndexOf("Process") > -1 && !list.Contains(shp) && !validateList.Contains(shp)) 
                    list.Add(shp);
            }

            int i = 1;
            foreach (Visio.Shape shp in list)
            {
                if (!dictShapeA.ContainsKey(shp.Text) && !textAnalysis.containsBlock(shp.Text))
                {
                    ListViewItem item = new ListViewItem(i.ToString());
                    item.SubItems.Add(shp.Text);
                    item.SubItems.Add(checkedDuples(list, shp.Text).ToString());
                    item.SubItems.Add(textAnalysis.getErrorText(shp.Text));
                    listView2.Items.Add(item);

                    dictShapeA.Add(shp.Text, shp);

                    i++;
                }
            }
            if (listView2.Items.Count == 0) label5.Text = "-";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            checkBlocks();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                tableLayoutPanel3.BackColor = Color.White;
                label6.Visible = true;
                label7.Visible = true;
                checkBox2.Visible = true;
                checkBox3.Visible = true;
                checkBox4.Visible = true;
                comboBox1.Visible = true;
            }
            else
            {
                tableLayoutPanel3.BackColor = DefaultBackColor;
                label6.Visible = false;
                label7.Visible = false;
                checkBox2.Visible = false;
                checkBox3.Visible = false;
                checkBox4.Visible = false;
                comboBox1.Visible = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            checkBox3.Checked = !checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            checkBox2.Checked = !checkBox3.Checked;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            listView1.CheckBoxes = checkBox4.Checked;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                setListElements();
                checkBlocks();
            }
            else
            {
                listView2.Items.Clear();
                setListElements(false);
            }

        }
    }
}
