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
using System.Text.RegularExpressions;

namespace _VS_One__Visio
{
    public partial class TabControlForm : Form
    {
        TextEditorForm firstContainer;
        TextEditorForm secondContainer;
        TextEditorForm rOContainer;
        TextEditorForm rSContainer;
        TextEditorForm rOContainer1;
        TextEditorForm rSContainer1;
        NewBuilderForm thirdContainer;
        AllShapesForm fourthContainer;
        NewResultBuilderForm fiveContainer;
        VisioViewerForm sixContainer;

        private bool UseDocument = Properties.Settings.Default.UseDocument;

        ScintillaFunc scintillaFunc = new ScintillaFunc();

        public TabControlForm()
        {
            InitializeComponent();

            firstContainer = getForm();
            secondContainer = getForm();
            //
            rOContainer = getForm();
            rSContainer = getForm();
            rOContainer1 = getForm();
            rSContainer1 = getForm();
            //
            sixContainer = getVisioViewer();

            tabPage7.Controls.Add(firstContainer);
            firstContainer.Show();
            tabPage5.Controls.Add(secondContainer);
            secondContainer.Show();
            //
            splitContainer3.Panel1.Controls.Add(rOContainer);
            rOContainer.Show();
            splitContainer3.Panel2.Controls.Add(rSContainer);
            rSContainer.Show();
            splitContainer4.Panel1.Controls.Add(rOContainer1);
            rOContainer1.Show();
            splitContainer4.Panel2.Controls.Add(rSContainer1);
            rSContainer1.Show();
            //

            //rOContainer.splitContainer1.Panel1Collapsed = true;
            rSContainer.splitContainer1.Panel1Collapsed = true;
            rSContainer1.splitContainer1.Panel1Collapsed = true;

            firstContainer.otherScintilla = secondContainer.scintilla1;
            secondContainer.otherScintilla = firstContainer.scintilla1;

            tabPage6.Controls.Add(sixContainer);
            sixContainer.Show();

            label1.Text = "";
            label1.BackColor = Color.White;
            tableLayoutPanel2.BackColor = Color.White;

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

            rOContainer.scintilla1.KeyDown += Scintilla1_KeyDown2;
            rOContainer.splitContainer1.SplitterMoved += rSO_SplitterMoved;
            rSContainer.scintilla1.KeyDown += Scintilla1_KeyDown3;
            rSContainer.splitContainer1.SplitterMoved += rSC_SplitterMoved;
            splitContainer3.SplitterMoved += SplitterMovedForF;

            rOContainer1.scintilla1.KeyDown += Scintilla1_KeyDown4;
            rOContainer1.splitContainer1.SplitterMoved += rSO1_SplitterMoved;
            rSContainer1.scintilla1.KeyDown += Scintilla1_KeyDown5;
            rSContainer1.splitContainer1.SplitterMoved += rSC1_SplitterMoved;
            splitContainer4.SplitterMoved += SplitterMovedForS;

            secondContainer.scintilla1.Document = firstContainer.scintilla1.Document;

            if(UseDocument)
            {
                rOContainer.scintilla1.Document = firstContainer.scintilla1.Document;
                rSContainer.scintilla1.Document = firstContainer.scintilla1.Document;
                rOContainer1.scintilla1.Document = firstContainer.scintilla1.Document;
                rSContainer1.scintilla1.Document = firstContainer.scintilla1.Document;

                rOContainer.otherScintilla = rSContainer.scintilla1;
                rOContainer1.otherScintilla = rSContainer1.scintilla1;
            }

            sixContainer.splitContainer1.SplitterMoved += sixContainerGraph_SplitterMoved;
            sixContainer.splitContainer2.SplitterDistance = firstContainer.splitContainer1.SplitterDistance;

            listView2.MouseDoubleClick += listView2_MouseDoubleClick;
        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView2.FocusedItem != null && listView2.FocusedItem.Bounds.Contains(e.Location) == true)
            {
                //MessageBox.Show(listView2.FocusedItem.SubItems[2].Text);
                string[] positions = listView2.FocusedItem.SubItems[1].Text.Split('_');
                firstContainer.scintilla1.ScrollRange(Convert.ToInt32(positions[0]), Convert.ToInt32(positions[0]));
                BasicPhraseBuldier basic = new BasicPhraseBuldier(listView2.FocusedItem.SubItems[2].Text);
                basic.ShowDialog();
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

        private void SplitterMovedForF(object sender, SplitterEventArgs e)
        {
            if (!splitContainer4.Focused) splitContainer4.SplitterDistance = splitContainer3.SplitterDistance;
        }

        private void rSO_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!rSContainer1.Focused) rOContainer1.splitContainer1.SplitterDistance = rOContainer.splitContainer1.SplitterDistance;
        }

        private void rSC_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!rSContainer1.Focused) rSContainer1.splitContainer1.SplitterDistance = rSContainer.splitContainer1.SplitterDistance;
        }

        private void SplitterMovedForS(object sender, SplitterEventArgs e)
        {
            if (!splitContainer3.Focused) splitContainer3.SplitterDistance = splitContainer4.SplitterDistance;
        }

        private void rSO1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!rSContainer.Focused) rOContainer.splitContainer1.SplitterDistance = rOContainer1.splitContainer1.SplitterDistance;
        }

        private void rSC1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (!rSContainer.Focused) rSContainer.splitContainer1.SplitterDistance = rSContainer1.splitContainer1.SplitterDistance;
        }

        private void Scintilla1_KeyDown(object sender, KeyEventArgs e)
        {
            label1.BackColor = Color.White;
            label1.Text = "";
            keysEvents(e, firstContainer, firstContainer.scintilla1);
        }

        private void Scintilla1_KeyDown1(object sender, KeyEventArgs e)
        {
            label1.BackColor = Color.White;
            label1.Text = "";
            keysEvents(e, secondContainer, secondContainer.scintilla1);
        }

        private void Scintilla1_KeyDown2(object sender, KeyEventArgs e)
        {
            keysEvents(e, rOContainer, rOContainer.scintilla1);
        }

        private void Scintilla1_KeyDown3(object sender, KeyEventArgs e)
        {
            keysEvents(e, rSContainer, rSContainer.scintilla1);
        }

        private void Scintilla1_KeyDown4(object sender, KeyEventArgs e)
        {
            keysEvents(e, rOContainer1, rOContainer1.scintilla1);
        }

        private void Scintilla1_KeyDown5(object sender, KeyEventArgs e)
        {
            keysEvents(e, rSContainer1, rSContainer1.scintilla1);
        }

        public string documentPath = "";

        private void keysEvents(KeyEventArgs e, TextEditorForm form, Scintilla scintilla)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                validateEvent();
                if (e.Shift)
                {
                    form.savingEvent(documentPath);
                    label1.Text = "Сохранено в файл";
                }
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
                    frm.Owner = this;
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
                    frm.Owner = this;
                    frm.Visible = true;
                    frm.Show();
                }
                else
                {
                    frm.Focus();
                }
            }
            if (e.Control && e.KeyCode == Keys.P)
            {
                form.changeLanguage();
            }
            if (e.KeyCode == Keys.F12)
            {
                if (e.Alt) form.findStore(form.otherScintilla);
                else form.findStore(form.scintilla1);
            }
            
        }

        JsonParse parser = new JsonParse();

        public void validateEvent()
        {
            listView2.Items.Clear();
            sixContainer.textEditor = firstContainer;
            var logs = parser.parseScript(firstContainer.scintilla1.Text, firstContainer.scintilla1);
            if (logs.Count() == 0)
            {
                ScinitillaStyles scinitillaStyles = new ScinitillaStyles();

                listView2.Items.Clear();
                label1.Text = "Сохранено локально";
                label1.BackColor = Color.Green;

                afterValidate(firstContainer);
                afterValidate(secondContainer);

                if(UseDocument)
                {
                    afterValidate(rOContainer);
                    afterValidate(rSContainer);
                    afterValidate(rOContainer1);
                    afterValidate(rSContainer1);
                }

                sixContainer.textEditor = firstContainer;

                List<string> states = new List<string>();

                foreach (Match match in Regex.Matches(firstContainer.scintilla1.Text, @"(?<=""name""\s*\:\s*"")[^""]+(?="")"))
                {
                    if (!states.Contains(match.Value)) states.Add(match.Value);
                }

                sixContainer.statesInScript = states;
            }
            else
            {
                logs.ForEach(log =>
                {
                    ListViewItem item = new ListViewItem("0-80X08e");
                    item.SubItems.Add(log.start + "_" + log.end);
                    item.SubItems.Add(log.message);
                    listView2.Items.Add(item);
                });
                label1.Text = "В скрипте имеются ошибки";
                label1.BackColor = Color.Red;
            }
        }

        private void afterValidate(TextEditorForm form)
        {
            ScinitillaStyles scinitillaStyles = new ScinitillaStyles();

            form.treeView1.SetObjectAsJson(JToken.Parse(parser.parseConverting(form.scintilla1.Text)));
            form.treeView1.Nodes[0].Expand();
            form.storeStringClass = scintillaFunc.storeBuilder(form.scintilla1, form.autoCompliteStore);
            form.typesStringClass = scintillaFunc.userTypeBuilder(form.scintilla1, form.autoCompliteTypes);
            scinitillaStyles.setLinks(form.scintilla1);
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                checkBox1.Text = "V";
                splitContainer1.Orientation = Orientation.Vertical;
            }
            else
            {
                checkBox1.Text = "H";
                splitContainer1.Orientation = Orientation.Horizontal;
            }
        }
    }
}
