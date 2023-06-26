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
    public partial class SettingsForm : Form
    {
        public Color color = Properties.Settings.Default.HightLightColor;

        public SettingsForm()
        {
            InitializeComponent();
            checkBox1.Checked = Properties.Settings.Default.UseHighLight;
            checkBox2.Checked = Properties.Settings.Default.UseRedactorDarkTheme;
            button2.BackColor = color;
            numericUpDown1.Value = Properties.Settings.Default.ToolTipTime;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.UseHighLight = checkBox1.Checked;
            Properties.Settings.Default.UseRedactorDarkTheme = checkBox2.Checked;
            Properties.Settings.Default.HightLightColor = color;
            Properties.Settings.Default.ToolTipTime = (int)numericUpDown1.Value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                color = colorDialog1.Color;
                button2.BackColor = color;
            }
        }
    }
}
