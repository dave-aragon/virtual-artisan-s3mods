namespace STBLBrowser.ImportExport
{
    partial class ExportCSV
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.quoteStrings = new System.Windows.Forms.CheckBox();
            this.quoteAll = new System.Windows.Forms.CheckBox();
            this.hexNums = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.idNumber = new System.Windows.Forms.RadioButton();
            this.idDetect = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabDelim = new System.Windows.Forms.RadioButton();
            this.otherDelim = new System.Windows.Forms.RadioButton();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(182, 106);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(188, 21);
            this.comboBox1.TabIndex = 30;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(121, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Language";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.quoteStrings);
            this.groupBox3.Controls.Add(this.quoteAll);
            this.groupBox3.Controls.Add(this.hexNums);
            this.groupBox3.Location = new System.Drawing.Point(182, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(188, 90);
            this.groupBox3.TabIndex = 28;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Format Options";
            // 
            // quoteStrings
            // 
            this.quoteStrings.AutoSize = true;
            this.quoteStrings.Checked = true;
            this.quoteStrings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.quoteStrings.Location = new System.Drawing.Point(6, 21);
            this.quoteStrings.Name = "quoteStrings";
            this.quoteStrings.Size = new System.Drawing.Size(150, 17);
            this.quoteStrings.TabIndex = 3;
            this.quoteStrings.Text = "Strings enclosed in quotes";
            this.quoteStrings.UseVisualStyleBackColor = true;
            // 
            // quoteAll
            // 
            this.quoteAll.AutoSize = true;
            this.quoteAll.Location = new System.Drawing.Point(6, 44);
            this.quoteAll.Name = "quoteAll";
            this.quoteAll.Size = new System.Drawing.Size(163, 17);
            this.quoteAll.TabIndex = 9;
            this.quoteAll.Text = "All values enclosed in quotes";
            this.quoteAll.UseVisualStyleBackColor = true;
            // 
            // hexNums
            // 
            this.hexNums.AutoSize = true;
            this.hexNums.Location = new System.Drawing.Point(6, 67);
            this.hexNums.Name = "hexNums";
            this.hexNums.Size = new System.Drawing.Size(168, 17);
            this.hexNums.TabIndex = 4;
            this.hexNums.Text = "Do not add 0x to hex numbers";
            this.hexNums.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Location = new System.Drawing.Point(388, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(150, 90);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "String ID Form";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.idNumber);
            this.panel1.Controls.Add(this.idDetect);
            this.panel1.Location = new System.Drawing.Point(6, 17);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(117, 66);
            this.panel1.TabIndex = 13;
            // 
            // idNumber
            // 
            this.idNumber.AutoSize = true;
            this.idNumber.Location = new System.Drawing.Point(6, 3);
            this.idNumber.Name = "idNumber";
            this.idNumber.Size = new System.Drawing.Size(76, 17);
            this.idNumber.TabIndex = 10;
            this.idNumber.Text = "ID Number";
            this.idNumber.UseVisualStyleBackColor = true;
            // 
            // idDetect
            // 
            this.idDetect.AutoSize = true;
            this.idDetect.Checked = true;
            this.idDetect.Location = new System.Drawing.Point(6, 49);
            this.idDetect.Name = "idDetect";
            this.idDetect.Size = new System.Drawing.Size(77, 17);
            this.idDetect.TabIndex = 12;
            this.idDetect.TabStop = true;
            this.idDetect.Text = "ID or name";
            this.idDetect.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(150, 90);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ID/name separator";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabDelim);
            this.panel2.Controls.Add(this.otherDelim);
            this.panel2.Controls.Add(this.textBox2);
            this.panel2.Location = new System.Drawing.Point(6, 17);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(140, 71);
            this.panel2.TabIndex = 14;
            // 
            // tabDelim
            // 
            this.tabDelim.AutoSize = true;
            this.tabDelim.Location = new System.Drawing.Point(3, 3);
            this.tabDelim.Name = "tabDelim";
            this.tabDelim.Size = new System.Drawing.Size(44, 17);
            this.tabDelim.TabIndex = 5;
            this.tabDelim.Text = "Tab";
            this.tabDelim.UseVisualStyleBackColor = true;
            // 
            // otherDelim
            // 
            this.otherDelim.AutoSize = true;
            this.otherDelim.Checked = true;
            this.otherDelim.Location = new System.Drawing.Point(3, 49);
            this.otherDelim.Name = "otherDelim";
            this.otherDelim.Size = new System.Drawing.Size(51, 17);
            this.otherDelim.TabIndex = 7;
            this.otherDelim.TabStop = true;
            this.otherDelim.Text = "Other";
            this.otherDelim.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(92, 48);
            this.textBox2.MaxLength = 1;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(25, 20);
            this.textBox2.TabIndex = 8;
            this.textBox2.Text = ",";
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(382, 137);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 25;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.Location = new System.Drawing.Point(463, 137);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 24;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(376, 108);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(175, 17);
            this.checkBox1.TabIndex = 31;
            this.checkBox1.Text = "Append locale suffix to filename";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // ExportCSV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 173);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Name = "ExportCSV";
            this.Text = "String Table Export Text/CSV";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox quoteStrings;
        private System.Windows.Forms.CheckBox quoteAll;
        private System.Windows.Forms.CheckBox hexNums;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton idNumber;
        private System.Windows.Forms.RadioButton idDetect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton tabDelim;
        private System.Windows.Forms.RadioButton otherDelim;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}