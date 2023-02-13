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
using Visio = Microsoft.Office.Interop.Visio;
using ScintillaNET;

namespace _VS_One__Visio
{
    public partial class AllShapesForm : Form
    {
        Scintilla scintilla;
        Visio.Shapes shapes = Globals.ThisAddIn.Application.ActivePage.Shapes;
        private Dictionary<string, Visio.Shape> dictOfShape = new Dictionary<string, Visio.Shape>();

        public AllShapesForm(Scintilla sourse)
        {
            InitializeComponent();
            scintilla = sourse;
            listView1.MouseUp += new MouseEventHandler(listView1_MouseClick);
            setListOfElement();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem != null && listView1.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    ContextMenu menu = new ContextMenu();

                    MenuItem toOF = new MenuItem("Создать спич");
                    toOF.Click += delegate (object sender2, EventArgs e2) { parseSpeech(listView1.FocusedItem.SubItems[1].Text); };
                    menu.MenuItems.Add(toOF);

                    MenuItem toFDP = new MenuItem("Создать слушалку");
                    toFDP.Click += delegate (object sender2, EventArgs e2)
                    {
                        parseListening(dictOfShape[listView1.FocusedItem.SubItems[1].Text]);
                    };
                    menu.MenuItems.Add(toFDP);

                    menu.Show(listView1, new Point(e.X, e.Y));
                }
            }
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                Form frm = Application.OpenForms["SearchForm"];

                if (frm == null)
                {
                    frm = new SearchForm(this, listView1, 1);
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

        private void setListOfElement()
        {
            if (shapes != null)
            {
                for (int i = 1; i < shapes.Count; i++)
                {
                    if (Regex.IsMatch(shapes[i].NameU, @"[P p]rocess"))
                    {
                        if (!dictOfShape.ContainsKey(shapes[i].Text)) dictOfShape.Add(shapes[i].Text, shapes[i]);

                        ListViewItem item = new ListViewItem(i.ToString());
                        item.SubItems.Add(shapes[i].Text);
                        listView1.Items.Add(item);
                    }
                }
            }
        }

        private void parseSpeech(string sourceText)
        {
            string text = Regex.Replace(sourceText, @"^[^\n]+\n", "");

            string newText = "{\n\t\"name\": \"_speech\",\n\t\"speech\": [\n\t\t{\n\t\t\t\"text\": \""
                + text + "\",\n\t\t\t\"namespace\": [],\n\t\t}\n\t],\n\t\"next_state\": \"end\"\n},\n";

            /*if (scintilla.CurrentPosition != -1) scintilla.InsertText(scintilla.CurrentPosition, newText);
            else scintilla.Text += newText;*/
            Clipboard.SetText(newText);
        }

        private void parseListening(Visio.Shape shape)
        {
            string text = "{" +
                "\n\t\"name\": \"_listening\"," +
                "\n\t\"display_name\": \"Слушаем ответ абонента\"," +
                "\n\t\"settings\": {" +
                "\n\t\t\"max_hypotheses_to_check\": 3," +
                "\n\t\t\"incomprehensible_state\": \"end\"," +
                "\n\t\t\"silence_length_seconds\": 6.5," +
                "\n\t\t\"noise_length_seconds\": 15," +
                "\n\t\t\"long_silence_state\": \"end\"," +
                "\n\t\t\"long_noise_state\": \"end\"" +
                "\n\t}," +
                "\n\t\"phrases_conditions\": [";

            Array shpIds = shape.ConnectedShapes(Visio.VisConnectedShapesFlags.visConnectedShapesOutgoingNodes, "");

            Parallel.For(0, shpIds.Length, i =>
            {
                var element = Convert.ToInt32(shpIds.GetValue(i));
                Visio.Shape newShape = shapes.ItemFromID[element];

                var newShpIds = shape.GluedShapes(Visio.VisGluedShapesFlags.visGluedShapesOutgoing1D, "", newShape);

                var subElement = Convert.ToInt32(newShpIds.GetValue(0));
                var connector = shapes.ItemFromID[subElement];

                text += "\n\t\t{" +
                    "\n\t\t\t\"phrases_exact\": []," +
                    "\n\t\t\t\"phrases_parts\": []," +
                    "\n\t\t\t\"phrases_parts_exclude\": []," +
                    "\n\t\t\t\"basic_phrase\": \"" + connector.Text + "\"," +
                    "\n\t\t\t\"next_state\": \"end\"" +
                    "\n\t\t},";
            });

            text += "\n\t]\n},\n";

            /*if (scintilla.CurrentPosition != -1) scintilla.InsertText(scintilla.CurrentPosition, text);
            else scintilla.Text += text;*/
            Clipboard.SetText(text);
        }
    }
}
