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
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void buttonStyles1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DarkTheme = checkBox1.Checked;
        }
    }
}
