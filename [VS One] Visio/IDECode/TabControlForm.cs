using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScintillaNET;
using System.IO;

namespace _VS_One__Visio
{
    public partial class TabControlForm : Form
    {
        TextEditorForm firstContainer;
        TextEditorForm secondContainer;
        NewBuilderForm thirdContainer;
        AllShapesForm fourthContainer;
        NewResultBuilderForm fiveContainer;
        VisioViewerForm sixContainer;

        ScintillaFunc scintillaFunc = new ScintillaFunc();

        public TabControlForm()
        {
            InitializeComponent();

            firstContainer = getForm();
            secondContainer = getForm();
            sixContainer = getVisioViewer();

            tabPage7.Controls.Add(firstContainer);
            firstContainer.Show();
            tabPage5.Controls.Add(secondContainer);
            secondContainer.Show();

            tabPage6.Controls.Add(sixContainer);
            sixContainer.Show();

            if (Globals.ThisAddIn.Application.ActivePage != null)
            {
                thirdContainer = getBulder();
                fourthContainer = getAllBlocks();
                fiveContainer = getAllResult();

                tabPage2.Controls.Add(thirdContainer);
                thirdContainer.Show();
                tabPage3.Controls.Add(fourthContainer);
                fourthContainer.Show();
                tabPage4.Controls.Add(fiveContainer);
                fiveContainer.Show();
            }
            else
            {
                tabPage2.Parent = null;
                tabPage3.Parent = null;
                tabPage4.Parent = null;
            }

            firstContainer.scintilla1.KeyDown += Scintilla1_KeyDown;
            firstContainer.splitContainer1.SplitterMoved += firstContainerScriptObjects_SplitterMoved;

            secondContainer.scintilla1.KeyDown += Scintilla1_KeyDown1;
            secondContainer.splitContainer1.SplitterMoved += secondContainerScriptObjects_SplitterMoved;

            secondContainer.scintilla1.Document = firstContainer.scintilla1.Document;

            sixContainer.splitContainer1.SplitterMoved += sixContainerGraph_SplitterMoved;
            sixContainer.splitContainer2.SplitterDistance = firstContainer.splitContainer1.SplitterDistance;

            listView2.MouseDoubleClick += listView2_MouseDoubleClick;
        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView2.FocusedItem != null && listView2.FocusedItem.Bounds.Contains(e.Location) == true)
            {
                MessageBox.Show(listView2.FocusedItem.SubItems[2].Text);
                string[] positions = listView2.FocusedItem.SubItems[1].Text.Split('_');
                firstContainer.scintilla1.ScrollRange(Convert.ToInt32(positions[0]), Convert.ToInt32(positions[0]));
            }
        }

        private void firstContainerScriptObjects_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!secondContainer.Focused) secondContainer.splitContainer1.SplitterDistance = firstContainer.splitContainer1.SplitterDistance;
            if (!sixContainer.Focused) sixContainer.splitContainer2.SplitterDistance = firstContainer.splitContainer1.SplitterDistance;
        }

        private void secondContainerScriptObjects_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!firstContainer.Focused) firstContainer.splitContainer1.SplitterDistance = secondContainer.splitContainer1.SplitterDistance;
            if (!sixContainer.Focused) sixContainer.splitContainer2.SplitterDistance = secondContainer.splitContainer1.SplitterDistance;
        }

        private void sixContainerGraph_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!firstContainer.Focused && !secondContainer.Focused)
            {
                firstContainer.splitContainer1.SplitterDistance = sixContainer.splitContainer2.SplitterDistance;
                secondContainer.splitContainer1.SplitterDistance = firstContainer.splitContainer1.SplitterDistance;
            }
        }

        private void Scintilla1_KeyDown(object sender, KeyEventArgs e)
        {
            keysEvents(e, firstContainer, firstContainer.scintilla1);
        }

        private void Scintilla1_KeyDown1(object sender, KeyEventArgs e)
        {
            keysEvents(e, secondContainer, secondContainer.scintilla1);
        }

        public string documentPath = "";

        private void keysEvents(KeyEventArgs e, TextEditorForm form, Scintilla scintilla)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                validateEvent();
                if (e.Shift) form.savingEvent(documentPath); 
            }
            if (e.Control && e.KeyCode == Keys.O)
            {
                documentPath = form.openEvent();
                validateEvent();
            }
            if (e.Control && e.KeyCode == Keys.F)
            {
                Form frm = Application.OpenForms["FindInScintillaForm"];

                if (frm == null)
                {
                    frm = new FindInScintillaForm(scintilla);
                    frm.Visible = true;
                    frm.Show();
                }
                else
                {
                    frm.Focus();
                }
            }
            if (e.Control && e.KeyCode == Keys.G)
            {
                Form frm = Application.OpenForms["GoToLineForm"];

                if (frm == null)
                {
                    frm = new GoToLineForm(scintilla);
                    frm.Visible = true;
                    frm.Show();
                }
                else
                {
                    frm.Focus();
                }
            }
        }

        JsonParse parser = new JsonParse();

        public void validateEvent()
        {
            sixContainer.textEditor = firstContainer;
            var log = parser.parseJson(firstContainer.scintilla1.Text, firstContainer.scintilla1);
            if (log == null)
            {
                listView2.Items.Clear();
                textBox1.BackColor = SystemColors.Control;
                textBox1.Text = "";
                firstContainer.treeView1.SetObjectAsJson(JToken.Parse(parser.parseConverting(firstContainer.scintilla1.Text)));
                firstContainer.treeView1.Nodes[0].Expand();
                firstContainer.storeStringClass = scintillaFunc.storeBuilder(firstContainer.scintilla1, firstContainer.autoCompliteStore);
                firstContainer.typesStringClass = scintillaFunc.userTypeBuilder(firstContainer.scintilla1, firstContainer.autoCompliteTypes);

                //secondContainer.treeView1.SetObjectAsJson(JToken.Parse(scintillaFunc.getOnlyPhraseObjects(parser.parseConverting(firstContainer.scintilla1.Text))));
                secondContainer.treeView1.SetObjectAsJson(JToken.Parse(parser.parseConverting(secondContainer.scintilla1.Text)));
                secondContainer.treeView1.Nodes[0].Expand();
                secondContainer.storeStringClass = scintillaFunc.storeBuilder(secondContainer.scintilla1, secondContainer.autoCompliteStore);
                secondContainer.typesStringClass = scintillaFunc.userTypeBuilder(secondContainer.scintilla1, secondContainer.autoCompliteTypes);
            }
            else
            {
                ListViewItem item = new ListViewItem("0-80X08e");
                item.SubItems.Add(log.start + "_" + log.end);
                item.SubItems.Add(log.message);
                listView2.Items.Add(item);
                textBox1.Text = "В скрипте имеются ошибки";
                textBox1.BackColor = Color.Red;
            }
        }

        private TextEditorForm getForm()
        {
            TextEditorForm script = new TextEditorForm();

            script.TopLevel = false;
            script.FormBorderStyle = FormBorderStyle.None;
            script.AutoScaleMode = AutoScaleMode.Dpi;
            script.Dock = DockStyle.Fill;

            return script;
        }

        private NewBuilderForm getBulder()
        {
            NewBuilderForm script = new NewBuilderForm();

            script.TopLevel = false;
            script.FormBorderStyle = FormBorderStyle.None;
            script.AutoScaleMode = AutoScaleMode.Dpi;
            script.Dock = DockStyle.Fill;

            return script;
        }

        private AllShapesForm getAllBlocks()
        {
            AllShapesForm script = new AllShapesForm(firstContainer.scintilla1);

            script.TopLevel = false;
            script.FormBorderStyle = FormBorderStyle.None;
            script.AutoScaleMode = AutoScaleMode.Dpi;
            script.Dock = DockStyle.Fill;

            return script;
        }

        private NewResultBuilderForm getAllResult()
        {
            NewResultBuilderForm script = new NewResultBuilderForm();

            script.TopLevel = false;
            script.FormBorderStyle = FormBorderStyle.None;
            script.AutoScaleMode = AutoScaleMode.Dpi;
            script.Dock = DockStyle.Fill;

            return script;
        }

        private VisioViewerForm getVisioViewer()
        {
            VisioViewerForm script = new VisioViewerForm("");

            script.TopLevel = false;
            script.FormBorderStyle = FormBorderStyle.None;
            script.AutoScaleMode = AutoScaleMode.Dpi;
            script.Dock = DockStyle.Fill;

            return script;
        }
    }
}
