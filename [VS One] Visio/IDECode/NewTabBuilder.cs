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
    public partial class NewTabBuilder : Form
    {
        public NewTextEditorForm mainView;
        public NewTextEditorForm additionalView;
        public NewTextEditorForm someAdditionalView;
        public TreeViewForm treeView;
        public BlockCodeForm blockTreeView;

        ScintillaFunc scintillaFunc = new ScintillaFunc();

        public NewTabBuilder()
        {
            InitializeComponent();

            mainView = getScriptForm();
            additionalView = getScriptForm();
            someAdditionalView = getScriptForm();
            treeView = getTreeviewForm();
            blockTreeView = getBlockCodeForm();

            splitContainer3.Panel2.Controls.Add(mainView);
            mainView.Show();
            splitContainer5.Panel1.Controls.Add(additionalView);
            additionalView.Show();
            splitContainer5.Panel2.Controls.Add(someAdditionalView);
            someAdditionalView.Show();
            splitContainer4.Panel1.Controls.Add(blockTreeView);
            blockTreeView.Show();
            splitContainer4.Panel2.Controls.Add(treeView);
            treeView.Show();

            splitContainer5.Panel2.Hide();
            splitContainer5.Panel2Collapsed = true;

            treeView.mainScintilla = mainView.scintilla1;
            treeView.otherScintilla = additionalView.scintilla1;
            treeView.additionalScintilla = someAdditionalView.scintilla1;

            mainView.otherScintilla = additionalView.scintilla1;
            mainView.additionalScintilla = someAdditionalView.scintilla1;
            additionalView.otherScintilla = mainView.scintilla1;
            additionalView.additionalScintilla = someAdditionalView.scintilla1;
            someAdditionalView.otherScintilla = additionalView.scintilla1;
            someAdditionalView.additionalScintilla = mainView.scintilla1;

            mainView.toolStripLabel2.Text = "Главное окно";
            additionalView.toolStripLabel2.Text = "Дополнительное окно";
            someAdditionalView.toolStripLabel2.Text = "Третье окно";

            additionalView.scintilla1.Document = mainView.scintilla1.Document;
            mainView.scintilla1.Document = additionalView.scintilla1.Document;
            someAdditionalView.scintilla1.Document = mainView.scintilla1.Document;

            label1.Text = "";
            label1.BackColor = Color.White;
            tableLayoutPanel2.BackColor = Color.White;

            mainView.scintilla1.KeyDown += mainView_KeyDown;
            additionalView.scintilla1.KeyDown += additionalView_KeyDown;
            someAdditionalView.scintilla1.KeyDown += someAdditionalView_KeyDown;
            this.Deactivate += NewTabBuilder_Deactivate;
            listView2.MouseDoubleClick += listView2_MouseDoubleClick;

            toolStripButton4.Image = (Properties.Settings.Default.NeedValidateStates) ? Properties.Resources.validate : Properties.Resources.unvalidate;
        }

        private void NewTabBuilder_Deactivate(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UseHighLight)
            {
                if (mainView.scintilla1.Focused) mainView.tableLayoutPanel2.BackColor = Properties.Settings.Default.HightLightColor;
                if (additionalView.scintilla1.Focused) additionalView.tableLayoutPanel2.BackColor = Properties.Settings.Default.HightLightColor;
                if (someAdditionalView.scintilla1.Focused) someAdditionalView.tableLayoutPanel2.BackColor = Properties.Settings.Default.HightLightColor;
            }
        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView2.FocusedItem != null && listView2.FocusedItem.Bounds.Contains(e.Location) == true)
            {
                //MessageBox.Show(listView2.FocusedItem.SubItems[2].Text);
                string[] positions = listView2.FocusedItem.SubItems[1].Text.Split('_');
                mainView.scintilla1.ScrollRange(Convert.ToInt32(positions[0]), Convert.ToInt32(positions[0]));
                BasicPhraseBuldier basic = new BasicPhraseBuldier(listView2.FocusedItem.SubItems[2].Text);
                basic.ShowDialog();
            }
        }

        private void mainView_KeyDown(object sender, KeyEventArgs e)
        {
            beforeKeyEvents(mainView, e);
        }

        private void additionalView_KeyDown(object sender, KeyEventArgs e)
        {
            beforeKeyEvents(additionalView, e);
        }

        private void someAdditionalView_KeyDown(object sender, KeyEventArgs e)
        {
            beforeKeyEvents(someAdditionalView, e);
        }

        private void beforeKeyEvents(NewTextEditorForm container, KeyEventArgs e)
        {
            label1.Text = "";
            label1.BackColor = Color.White;
            keysEvents(e, container, container.scintilla1);
        }

        public string documentPath = "";

        private void keysEvents(KeyEventArgs e, NewTextEditorForm form, Scintilla scintilla)
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
            if (e.Control && e.KeyCode == Keys.N)
            {
                form.removeNewLinesAction(form.scintilla1);
            }
            if (e.KeyCode == Keys.F12)
            {
                if (e.Alt) form.findStore(form.otherScintilla);
                else form.findStore(form.scintilla1);
            }

        }

        JsonParse parser = new JsonParse();
        ScinitillaStyles scinitillaStyles = new ScinitillaStyles();

        public void validateEvent()
        {
            listView2.Items.Clear();
            var logs = parser.parseScript(mainView.scintilla1.Text, additionalView.scintilla1);
            if (logs.Count() == 0)
            {
                listView2.Items.Clear();
                label1.Text = "Сохранено локально";
                label1.BackColor = Color.Green;

                afterValidate(mainView);
                afterValidate(additionalView);

                List<string> states = new List<string>();

                foreach (Match match in Regex.Matches(mainView.scintilla1.Text, @"(?<=""name""\s*\:\s*"")[^""]+(?="")"))
                {
                    if (!states.Contains(match.Value)) states.Add(match.Value);
                }
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

        private void afterValidate(NewTextEditorForm form)
        {
            ScinitillaStyles scinitillaStyles = new ScinitillaStyles();

            treeView.treeView1.SetObjectAsJson(JToken.Parse(parser.parseConverting(form.scintilla1.Text)));
            treeView.treeView1.Nodes[0].Expand();
            form.storeStringClass = scintillaFunc.storeBuilder(form.scintilla1, form.autoCompliteStore);
            form.typesStringClass = scintillaFunc.userTypeBuilder(form.scintilla1, form.autoCompliteTypes);
            scinitillaStyles.setLinks(form.scintilla1);
        }

        private NewTextEditorForm getScriptForm()
        {
            NewTextEditorForm script = new NewTextEditorForm();

            script.TopLevel = false;
            script.FormBorderStyle = FormBorderStyle.None;
            script.AutoScaleMode = AutoScaleMode.Dpi;
            script.Dock = DockStyle.Fill;

            return script;
        }

        private TreeViewForm getTreeviewForm()
        {
            TreeViewForm tree = new TreeViewForm();

            tree.TopLevel = false;
            tree.FormBorderStyle = FormBorderStyle.None;
            tree.AutoScaleMode = AutoScaleMode.Dpi;
            tree.Dock = DockStyle.Fill;

            return tree;
        }

        private BlockCodeForm getBlockCodeForm()
        {
            BlockCodeForm tree = new BlockCodeForm();

            tree.TopLevel = false;
            tree.FormBorderStyle = FormBorderStyle.None;
            tree.AutoScaleMode = AutoScaleMode.Dpi;
            tree.Dock = DockStyle.Fill;

            return tree;
        }

        private int counter = 1;

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            counter++;

            Orientation orientation = (counter % 2 == 0) ? Orientation.Vertical : Orientation.Horizontal;
            int size = (counter % 2 == 0) ? splitContainer3.Width : splitContainer3.Height;
            splitContainer3.Orientation = orientation;
            splitContainer3.SplitterDistance = size / 2;

            Orientation addOrientation = (counter % 2 == 0) ? Orientation.Horizontal : Orientation.Vertical;
            int addSize = (counter % 2 == 0) ? splitContainer5.Height : splitContainer5.Width;
            splitContainer5.Orientation = addOrientation;
            splitContainer5.SplitterDistance = addSize / 2;
        }

        private int windowCounter = 1;
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            windowCounter++;
            bool condition = (windowCounter % 2 == 0);
            if (condition)
            {
                toolStripButton2.Image = Properties.Resources.windows_2;
                splitContainer5.Panel2.Show();
            }
            else
            {
                toolStripButton2.Image = Properties.Resources.windows_3;
                splitContainer5.Panel2.Hide();
            }
            splitContainer5.Panel2Collapsed = !condition;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            scinitillaStyles.resetToDefaultJsonStyle(mainView.scintilla1);
            scinitillaStyles.resetToDefaultJsonStyle(additionalView.scintilla1);
            scinitillaStyles.resetToDefaultJsonStyle(someAdditionalView.scintilla1);
        }

        int validateCounter = (Properties.Settings.Default.NeedValidateStates)? 0 : 1;

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            validateCounter++;
            Properties.Settings.Default.NeedValidateStates = (validateCounter % 2 == 0);
            toolStripButton4.Image = (Properties.Settings.Default.NeedValidateStates) ? Properties.Resources.validate : Properties.Resources.unvalidate;
        }
    }
}
