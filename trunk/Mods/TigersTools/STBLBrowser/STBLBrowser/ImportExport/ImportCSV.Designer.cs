namespace STBLBrowser.ImportExport
{
    partial class ImportCSV
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
            this.quoteStrings = new System.Windows.Forms.CheckBox();
            this.hexNums = new System.Windows.Forms.CheckBox();
            this.tabDelim = new System.Windows.Forms.RadioButton();
            this.whiteSpaceDelim = new System.Windows.Forms.RadioButton();
            this.otherDelim = new System.Windows.Forms.RadioButton();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.quoteAll = new System.Windows.Forms.CheckBox();
            this.idNumber = new System.Windows.Forms.RadioButton();
            this.idKey = new System.Windows.Forms.RadioButton();
            this.idDetect = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
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
            // hexNums
            // 
            this.hexNums.AutoSize = true;
            this.hexNums.Location = new System.Drawing.Point(6, 67);
            this.hexNums.Name = "hexNums";
            this.hexNums.Size = new System.Drawing.Size(126, 17);
            this.hexNums.TabIndex = 4;
            this.hexNums.Text = "Assume hex numbers";
            this.hexNums.UseVisualStyleBackColor = true;
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
            // whiteSpaceDelim
            // 
            this.whiteSpaceDelim.AutoSize = true;
            this.whiteSpaceDelim.Location = new System.Drawing.Point(3, 26);
            this.whiteSpaceDelim.Name = "whiteSpaceDelim";
            this.whiteSpaceDelim.Size = new System.Drawing.Size(85, 17);
            this.whiteSpaceDelim.TabIndex = 6;
            this.whiteSpaceDelim.Text = "White-space";
            this.whiteSpaceDelim.UseVisualStyleBackColor = true;
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
            this.otherDelim.CheckedChanged += new System.EventHandler(this.otherDelim_CheckedChanged);
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
            // idKey
            // 
            this.idKey.AutoSize = true;
            this.idKey.Location = new System.Drawing.Point(6, 26);
            this.idKey.Name = "idKey";
            this.idKey.Size = new System.Drawing.Size(72, 17);
            this.idKey.TabIndex = 11;
            this.idKey.Text = "String key";
            this.idKey.UseVisualStyleBackColor = true;
            this.idKey.CheckedChanged += new System.EventHandler(this.idKey_CheckedChanged);
            // 
            // idDetect
            // 
            this.idDetect.AutoSize = true;
            this.idDetect.Checked = true;
            this.idDetect.Location = new System.Drawing.Point(6, 49);
            this.idDetect.Name = "idDetect";
            this.idDetect.Size = new System.Drawing.Size(52, 17);
            this.idDetect.TabIndex = 12;
            this.idDetect.TabStop = true;
            this.idDetect.Text = "Either";
            this.idDetect.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.idNumber);
            this.panel1.Controls.Add(this.idDetect);
            this.panel1.Controls.Add(this.idKey);
            this.panel1.Location = new System.Drawing.Point(6, 17);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(89, 66);
            this.panel1.TabIndex = 13;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabDelim);
            this.panel2.Controls.Add(this.whiteSpaceDelim);
            this.panel2.Controls.Add(this.otherDelim);
            this.panel2.Controls.Add(this.textBox2);
            this.panel2.Location = new System.Drawing.Point(6, 17);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(140, 71);
            this.panel2.TabIndex = 14;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.Location = new System.Drawing.Point(462, 135);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 17;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(381, 135);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 18;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Location = new System.Drawing.Point(12, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(150, 90);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ID/name separator";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Location = new System.Drawing.Point(387, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(150, 90);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "String ID Form";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.quoteStrings);
            this.groupBox3.Controls.Add(this.quoteAll);
            this.groupBox3.Controls.Add(this.hexNums);
            this.groupBox3.Location = new System.Drawing.Point(181, 10);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(188, 90);
            this.groupBox3.TabIndex = 21;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Parse Options";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(120, 107);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Language";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(181, 104);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(188, 21);
            this.comboBox1.TabIndex = 23;
            // 
            // ImportCSV
            // 
            this.AcceptButton = this.button2;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button3;
            this.ClientSize = new System.Drawing.Size(549, 171);
            this.ControlBox = false;
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "ImportCSV";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "String Table Import Text / CSV";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox quoteStrings;
        private System.Windows.Forms.CheckBox hexNums;
        private System.Windows.Forms.RadioButton tabDelim;
        private System.Windows.Forms.RadioButton whiteSpaceDelim;
        private System.Windows.Forms.RadioButton otherDelim;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.CheckBox quoteAll;
        private System.Windows.Forms.RadioButton idNumber;
        private System.Windows.Forms.RadioButton idKey;
        private System.Windows.Forms.RadioButton idDetect;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}