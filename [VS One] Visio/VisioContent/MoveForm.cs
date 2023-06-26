using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Visio = Microsoft.Office.Interop.Visio;

namespace _VS_One__Visio
{
    public partial class MoveForm : Form
    {
        Visio.Selection allShapes;

        public MoveForm(Visio.Selection shapes)
        {
            InitializeComponent();

            allShapes = shapes;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Visio.Shape shp in allShapes)
            {
                if (!Regex.IsMatch(shp.NameU, @"[D d]ynamic"))
                {
                    Visio.Cell cell = (checkBox1.Checked) ? shp.Cells["PinY"] : shp.Cells["PinX"];

                    string prev = cell.FormulaU;

                    prev = Regex.Replace(prev, @"(\d+)(\.\d+)*", m =>
                    {
                        decimal d = Convert.ToDecimal(m.Groups[1].Value.Replace(".", ","));
                        d += Convert.ToDecimal(numericUpDown1.Value);
                        return d.ToString();
                    });

                    cell.FormulaU = prev.Replace(",", ".");
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
