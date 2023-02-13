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

namespace _VS_One__Visio
{
    public partial class SearchForm : Form
    {
        public ListView listView;
        private int counter;
        private int subItemIndex;
        private Form frm;
        private string prevPattern;

        public SearchForm(Form form, ListView list, int subIndex)
        {
            InitializeComponent();
            frm = form;
            listView = list;
            subItemIndex = subIndex;

            listView.FullRowSelect = true;
        }

        private void SearchForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    findText();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Escape:
                    Close();
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            findText();
        }

        private void findText()
        {
            string pattern = textBox1.Text;
            pattern = (checkBox2.Checked && !checkBox1.Checked) ? pattern : pattern.ToLower();

            counter = (pattern == prevPattern) ? counter : 0;
            prevPattern = pattern;

            bool breakWas = false;

            for (int i = counter; i < listView.Items.Count; i++)
            {
                string textInCell = listView.Items[i].SubItems[subItemIndex].Text;
                textInCell = (checkBox2.Checked && !checkBox1.Checked) ? textInCell : textInCell.ToLower();

                if (checkBox1.Checked)
                {
                    try
                    {
                        Match match = Regex.Match(textInCell, @pattern);
                        if (match.Success)
                        {
                            selectListViewItem(i);
                            breakWas = true;
                            break;
                        }
                    }
                    catch (ArgumentException e)
                    {
                        MessageBox.Show("Ошибка в написании регулярного выражения!!!");
                        counter = 0;
                        breakWas = true;
                        break;
                    }
                }
                else
                {
                    if (textInCell.IndexOf(pattern) > -1)
                    {
                        selectListViewItem(i);
                        breakWas = true;
                        break;
                    }
                }
            }
            if (!breakWas)
            {
                MessageBox.Show("Найден последний элемент!");
                counter = 0;
            }
        }

        private void selectListViewItem(int index)
        {
            clearSelection(listView);

            listView.Select();

            listView.Items[index].Focused = true;
            listView.Items[index].Selected = true;
            listView.EnsureVisible(index);

            counter = index + 1;

            Focus();
        }

        private void clearSelection(ListView list)
        {
            ListView.SelectedListViewItemCollection items = list.SelectedItems;

            foreach (ListViewItem item in items)
            {
                item.Selected = false;
            }
        }
    }
}
