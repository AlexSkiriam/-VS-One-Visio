
namespace _VS_One__Visio
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.buttonStyles1 = new _VS_One__Visio.ButtonStyles();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.checkBox1.FlatAppearance.CheckedBackColor = System.Drawing.SystemColors.Control;
            this.checkBox1.ForeColor = System.Drawing.Color.White;
            this.checkBox1.Location = new System.Drawing.Point(13, 13);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.checkBox1.Size = new System.Drawing.Size(93, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Темная тема";
            this.checkBox1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // buttonStyles1
            // 
            this.buttonStyles1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.buttonStyles1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.buttonStyles1.BorderColor = System.Drawing.Color.White;
            this.buttonStyles1.BorderRadius = 10;
            this.buttonStyles1.BorderSize = 1;
            this.buttonStyles1.FlatAppearance.BorderSize = 0;
            this.buttonStyles1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStyles1.ForeColor = System.Drawing.Color.White;
            this.buttonStyles1.Location = new System.Drawing.Point(262, 213);
            this.buttonStyles1.Name = "buttonStyles1";
            this.buttonStyles1.Size = new System.Drawing.Size(84, 29);
            this.buttonStyles1.TabIndex = 1;
            this.buttonStyles1.Text = "Сохранить";
            this.buttonStyles1.TextColor = System.Drawing.Color.White;
            this.buttonStyles1.UseVisualStyleBackColor = false;
            this.buttonStyles1.Click += new System.EventHandler(this.buttonStyles1_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.ClientSize = new System.Drawing.Size(358, 254);
            this.Controls.Add(this.buttonStyles1);
            this.Controls.Add(this.checkBox1);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox1;
        private ButtonStyles buttonStyles1;
    }
}