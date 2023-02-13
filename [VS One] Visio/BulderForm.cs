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
    public partial class BulderForm : Form
    {
		TextAnalysis textAnalysis = new TextAnalysis();
		UsingText usingText = new UsingText();
		PhrasesFunctions phrasesFunctions = new PhrasesFunctions();

		public Dictionary<string, Visio.Shape> dictShape;
		public Dictionary<string, Visio.Shape> dictShapeA;

		public BulderForm()
		{
			InitializeComponent();
		}

		public string getPhrase()
		{
			return (listView1.SelectedItems.Count == 1) ? listView1.SelectedItems[0].SubItems[2].Text : "";
		}

		public string getPhraseA()
		{
			return (listView2.SelectedItems.Count == 1) ? listView2.SelectedItems[0].SubItems[1].Text : "";
		}

		private void BulderForm_Load(object sender, EventArgs e)
		{
			listView1.FullRowSelect = true;
			listView1.MouseUp += new MouseEventHandler(listView1_MouseClick);
			setListElements();


			listView2.FullRowSelect = true;
			listView2.MouseUp += new MouseEventHandler(listView2_MouseClick);
			setResultList();
		}

		public void setListElements()
		{
			listView1.Items.Clear();

			Visio.Shapes shapes = Globals.ThisAddIn.Application.ActivePage.Shapes;

			List<PhrasesArray> listOfPhrases = phrasesFunctions.getAllPhrases(shapes);

			dictShape = new Dictionary<string, Visio.Shape>();

			foreach (PhrasesArray phrase in listOfPhrases)
			{
				if (!dictShape.ContainsKey(phrase.phrase))
				{
					ListViewItem item = new ListViewItem(phrase.phraseNumber.ToString());
					item.SubItems.Add(phrase.phraseIntent);
					item.SubItems.Add(phrase.phrase);
					listView1.Items.Add(item);

					dictShape.Add(phrase.phrase, phrase.phraseShape);
				}
			}
		}

		private void listView1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				if (listView1.FocusedItem != null && listView1.FocusedItem.Bounds.Contains(e.Location) == true)
				{
					ContextMenu menu = new ContextMenu();

					MenuItem toOF = new MenuItem("Поместить фразу в ОФ");
					toOF.Click += delegate (object sender2, EventArgs e2) { copyToOF(); };
					menu.MenuItems.Add(toOF);

					MenuItem toFDP = new MenuItem("Поместить фразу в ФДП");
					toFDP.Click += delegate (object sender2, EventArgs e2) { copyToFDP(); };
					menu.MenuItems.Add(toFDP);

					MenuItem toResultState = new MenuItem("Поместить фразу в Текст результата");
					toResultState.Click += delegate (object sender2, EventArgs e2) { copyToResText(); };
					menu.MenuItems.Add(toResultState);

					MenuItem copy = new MenuItem("Скопировать текст");
					copy.Click += delegate (object sender2, EventArgs e2) { Clipboard.SetText(getPhrase()); };
					menu.MenuItems.Add(copy);

					menu.Show(listView1, new Point(e.X, e.Y));
				}
			}
		}

		private void copyToOF()
        {
			textBox3.Text = getPhrase();
		}

		private void copyToFDP()
		{
			textBox4.Text = getPhrase();
		}

		private void copyToResText()
		{
			textBox8.Text = getPhrase();
		}

		public void setStateText()
		{
			int tabsCount = trackBar1.Value;
			string tabs = usingText.tabsToReturn(tabsCount);
			string firstTab = checkBox3.Checked ? "" : tabs;

			string cmbTxt = (!String.IsNullOrEmpty(comboBox1.Text)) ? comboBox1.Text + "_for_" : "";
			string stateText = "";
			if (checkBox1.Checked)
			{
				stateText = usingText.vsIceStateText(cmbTxt, textBox1.Text, textBox2.Text, textBox5.Text, textBox3.Text, textBox4.Text, firstTab, tabs);
			}
			else if (checkBox2.Checked)
			{
				stateText = usingText.defaultStateText(cmbTxt, textBox1.Text, textBox2.Text, textBox5.Text, textBox3.Text, textBox4.Text, firstTab, tabs);
			}

			richTextBox1.Text = stateText;
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

		public int checkedDuples(List<Visio.Shape> list, string text)
		{
			int dup = 0;
			foreach (Visio.Shape shape in list)
			{
				if (shape.Text == text) dup++;
			}
			return (dup > 1) ? dup : 0;
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
			textBox7.Text = getPhraseA();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			setStateText();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			setStateText();
			Clipboard.SetText(richTextBox1.Text);
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox1.Checked) checkBox2.Checked = false;
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBox2.Checked) checkBox1.Checked = false;
		}

		public void setResultText()
        {
			int tabsCount = trackBar2.Value;
			string tabs = usingText.tabsToReturn(tabsCount);
			string firstTab = checkBox4.Checked ? "" : tabs;

			string stateText = usingText.resultStateText(textBox6.Text, textBox7.Text, textBox8.Text, firstTab, tabs);

			richTextBox2.Text = stateText;
		}

        private void button6_Click(object sender, EventArgs e)
        {
			setResultText();
		}

        private void button3_Click(object sender, EventArgs e)
        {
			setResultText();
			Clipboard.SetText(richTextBox2.Text);
		}

        private void listView1_KeyDown(object sender, KeyEventArgs e)
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
    }
}
