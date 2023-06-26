using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _VS_One__Visio
{
    public partial class BlockCodeForm : Form
    {
        private Dictionary<string, string> blocks = new Dictionary<string, string>();

        public BlockCodeForm()
        {
            InitializeComponent();
            treeView1.MouseClick += treeView1_MouseClick;
            treeView1.NodeMouseClick += treeView1_NodeMouseClick;
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level != 0 && e.Button == MouseButtons.Left)
            {
                NewTextEditorForm editorForm = new NewTextEditorForm();
                editorForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                editorForm.Text = e.Node.Text;
                editorForm.scintilla1.Text = blocks[e.Node.Text];
                editorForm.ShowDialog();
                blocks[e.Node.Text] = editorForm.scintilla1.Text;
            }
        }

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip menu = new ContextMenuStrip();

                ToolStripMenuItem addNode = new ToolStripMenuItem("Добавить");
                addNode.Click += addNodeClick;

                ToolStripMenuItem deleteNode = new ToolStripMenuItem("Удалить");
                deleteNode.Click += deleteNodeClick;

                menu.Items.AddRange(new ToolStripItem[] { addNode, deleteNode});

                treeView1.ContextMenuStrip = menu;
            }
        }

        private void addNodeClick(object sender, EventArgs e)
        {
            string counter = (treeView1.Nodes[0].Nodes.Count > 0) ? $"{treeView1.Nodes[0].Nodes.Count}" : "";
            string blocId = $"Новый блок{counter}";
            treeView1.Nodes[0].Nodes.Add(new TreeNode(blocId));
            blocks.Add(blocId, Properties.Settings.Default.IncludeStartText);
            treeView1.ExpandAll();
        }

        private void deleteNodeClick(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Level != 0)
            {
                blocks.Remove(treeView1.SelectedNode.Text);
                treeView1.SelectedNode.Remove();
            }
        }
    }
}
