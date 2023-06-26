using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Visio = Microsoft.Office.Interop.Visio;

namespace _VS_One__Visio
{
    public partial class MainScriptBuilder : Form
    {
        Visio.Shapes allshapes;
        TextBox renameBox;

        public MainScriptBuilder(Visio.Shapes shapes)
        {
            InitializeComponent();
            allshapes = shapes;

            this.tabControl1.Padding = new Point(12, 4);
            this.tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.tabControl1.DrawItem += tabControl1_DrawItem;
            this.tabControl1.MouseDown += tabControl1_MouseDown;
            this.tabControl1.Selecting += tabControl1_Selecting;
            this.tabControl1.HandleCreated += tabControl1_HandleCreated;
        }

        private void MainScriptBuilder_Load(object sender, EventArgs e)
        {
            
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        private const int TCM_SETMINTABWIDTH = 0x1300 + 49;
        private void tabControl1_HandleCreated(object sender, EventArgs e)
        {
            SendMessage(this.tabControl1.Handle, TCM_SETMINTABWIDTH, IntPtr.Zero, (IntPtr)16);
        }

        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            var lastIndex = this.tabControl1.TabCount - 1;
            if (this.tabControl1.GetTabRect(lastIndex).Contains(e.Location))
            {
                this.tabControl1.TabPages.Insert(lastIndex, createNewTab(Convert.ToString(lastIndex)));
                this.tabControl1.SelectedIndex = lastIndex;
            }
            else
            {
                for (var i = 0; i < this.tabControl1.TabPages.Count; i++)
                {
                    var tabRect = this.tabControl1.GetTabRect(i);
                    tabRect.Inflate(-2, -2);
                    var closeImage = Properties.Resources.DeleteButton_Image;
                    var imageRect = new Rectangle(
                        (tabRect.Right - closeImage.Width),
                        tabRect.Top + (tabRect.Height - closeImage.Height) / 2,
                        closeImage.Width,
                        closeImage.Height);
                    if (imageRect.Contains(e.Location))
                    {
                        this.tabControl1.TabPages.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == this.tabControl1.TabCount - 1)
                e.Cancel = true;
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabPage = this.tabControl1.TabPages[e.Index];
            var tabRect = this.tabControl1.GetTabRect(e.Index);
            tabRect.Inflate(-2, -2);
            if (e.Index == this.tabControl1.TabCount - 1)
            {
                var addImage = Properties.Resources.AddButton_Image;
                e.Graphics.DrawImage(addImage,
                    tabRect.Left + (tabRect.Width - addImage.Width) / 2,
                    tabRect.Top + (tabRect.Height - addImage.Height) / 2);
            }
            else
            {
                var closeImage = Properties.Resources.DeleteButton_Image;
                e.Graphics.DrawImage(closeImage,
                    (tabRect.Right - closeImage.Width),
                    tabRect.Top + (tabRect.Height - closeImage.Height) / 2);
                TextRenderer.DrawText(e.Graphics, tabPage.Text, tabPage.Font,
                    tabRect, tabPage.ForeColor, TextFormatFlags.Left);
            }
        }

        private void tabControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            AddTextBoxToTab(tabControl1);
        }

        private TextBox AddTextBoxToTab(TabControl tabctl, int index = -1)
        {
            if (index < 0) index = tabctl.SelectedIndex;
            var rc = tabctl.GetTabRect(index);
            rc = tabctl.RectangleToScreen(rc);
            rc = tabctl.Parent.RectangleToClient(rc);
            var box = new TextBox();
            box.Font = tabctl.Font;
            box.Text = tabctl.SelectedTab.Text;
            box.Leave += delegate { box.Dispose(); };
            box.LostFocus += delegate {
                tabctl.SelectedTab.Text = box.Text;
                box.Dispose();
            };
            box.SetBounds(rc.Left, rc.Top, rc.Width, rc.Height);
            tabctl.Parent.Controls.Add(box);
            box.BringToFront();
            box.Focus();
            return box;
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        public TabPage createNewTab(string index)
        {
            //TabControlForm script = new TabControlForm();
            NewTabBuilder newTabBuilder = new NewTabBuilder();

            newTabBuilder.TopLevel = false;
            newTabBuilder.FormBorderStyle = FormBorderStyle.None;
            newTabBuilder.AutoScaleMode = AutoScaleMode.Dpi;

            string newIndex = (index == "0") ? "" : "_" + index;

            TabPage tab = new TabPage();
            tab.Text = "Новый файл" + newIndex;

            tab.Controls.Add(newTabBuilder);
            newTabBuilder.Dock = DockStyle.Fill;

            newTabBuilder.Show();
            Refresh();

            return tab;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SettingsForm form = new SettingsForm();
            form.ShowDialog();
        }
    }
}
