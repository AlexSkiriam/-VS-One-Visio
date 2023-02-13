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

namespace _VS_One__Visio
{
    public partial class NewResultBuilderForm : Form
    {
		TextAnalysis textAnalysis = new TextAnalysis();
		UsingText usingText = new UsingText();
		PhrasesFunctions phrasesFunctions = new PhrasesFunctions();

		public Dictionary<string, Visio.Shape> dictShapeA;

		public NewResultBuilderForm()
        {
            InitializeComponent();

			listView2.FullRowSelect = true;
			listView2.MouseUp += new MouseEventHandler(listView2_MouseClick);
			setResultList();
		}

		public string getPhraseA()
		{
			return (listView2.SelectedItems.Count == 1) ? listView2.SelectedItems[0].SubItems[1].Text : "";
		}

		private void listView2_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				if (listView2.FocusedItem != null && listView2.FocusedItem.Bounds.Contains(e.Location) == true)
				{
					ContextMenu menu = new ContextMenu();

					MenuItem toRes = new MenuItem("Поместить в display_name");
					toRes.Click += delegate (object sender2, EventArgs e2) { copyToRes(); };
					menu.MenuItems.Add(toRes);

					MenuItem copy = new MenuItem("Скопировать текст");
					copy.Click += delegate (object sender2, EventArgs e2) { Clipboard.SetText(getPhraseA()); };
					menu.MenuItems.Add(copy);

					menu.Show(listView2, new Point(e.X, e.Y));
				}
			}
		}

		private void copyToRes()
		{
			textBox1.Text = getPhraseA();
		}

		public void setResultList()
		{
			listView2.Items.Clear();
			dictShapeA = new Dictionary<string, Visio.Shape>();

			Visio.Shapes shapes = Globals.ThisAddIn.Application.ActivePage.Shapes;
			List<Visio.Shape> list = getAllResults(shapes);

			int i = 1;
			foreach (Visio.Shape shp in list)
			{
				if (!dictShapeA.ContainsKey(shp.Text))
				{
					ListViewItem item = new ListViewItem(i.ToString());
					item.SubItems.Add(shp.Text);
					item.SubItems.Add(checkedDuples(list, shp.Text).ToString());
					listView2.Items.Add(item);

					dictShapeA.Add(shp.Text, shp);

					i++;
				}
			}
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

		public void setResultText()
		{
			int tabsCount = trackBar1.Value;
			string tabs = usingText.tabsToReturn(tabsCount);
			string firstTab = checkBox3.Checked ? "" : tabs;

			string stateText = usingText.resultStateText(textBox1.Text, textBox2.Text, textBox5.Text, firstTab, tabs);

			Clipboard.SetText(stateText);
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
						string text = phrasesFunctions.cleanSurplusSpaces(shp.Text);
						Match match = Regex.Match(text, @"(\w\s*\w+\s*(\w+\.?\s*)?№*\s*(\d\.*)+|^\w+\s*\w+\Z)");

						if (!match.Success) list.Add(shp);
					}
				}
			}
			return list;
		}

        private void button2_Click(object sender, EventArgs e)
        {
			setResultText();
		}
    }

}
