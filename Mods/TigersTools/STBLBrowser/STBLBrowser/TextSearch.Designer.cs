namespace STBLBrowser
{
    partial class TextSearch
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.searchKeys = new System.Windows.Forms.RadioButton();
            this.searchStrings = new System.Windows.Forms.RadioButton();
            this.checkMatchCase = new System.Windows.Forms.CheckBox();
            this.checkMatchWholeWord = new System.Windows.Forms.CheckBox();
            this.checkUseRegex = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Look for:";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 29);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(265, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.resetSearch);
            // 
            // searchKeys
            // 
            this.searchKeys.AutoSize = true;
            this.searchKeys.Location = new System.Drawing.Point(118, 55);
            this.searchKeys.Name = "searchKeys";
            this.searchKeys.Size = new System.Drawing.Size(74, 17);
            this.searchKeys.TabIndex = 2;
            this.searchKeys.Text = "Key Name";
            this.searchKeys.UseVisualStyleBackColor = true;
            this.searchKeys.CheckedChanged += new System.EventHandler(this.resetSearch);
            // 
            // searchStrings
            // 
            this.searchStrings.AutoSize = true;
            this.searchStrings.Checked = true;
            this.searchStrings.Location = new System.Drawing.Point(12, 55);
            this.searchStrings.Name = "searchStrings";
            this.searchStrings.Size = new System.Drawing.Size(100, 17);
            this.searchStrings.TabIndex = 3;
            this.searchStrings.TabStop = true;
            this.searchStrings.Text = "Localized String";
            this.searchStrings.UseVisualStyleBackColor = true;
            this.searchStrings.CheckedChanged += new System.EventHandler(this.resetSearch);
            // 
            // checkMatchCase
            // 
            this.checkMatchCase.AutoSize = true;
            this.checkMatchCase.Location = new System.Drawing.Point(12, 78);
            this.checkMatchCase.Name = "checkMatchCase";
            this.checkMatchCase.Size = new System.Drawing.Size(82, 17);
            this.checkMatchCase.TabIndex = 4;
            this.checkMatchCase.Text = "Match case";
            this.checkMatchCase.UseVisualStyleBackColor = true;
            this.checkMatchCase.CheckedChanged += new System.EventHandler(this.resetSearch);
            // 
            // checkMatchWholeWord
            // 
            this.checkMatchWholeWord.AutoSize = true;
            this.checkMatchWholeWord.Location = new System.Drawing.Point(12, 102);
            this.checkMatchWholeWord.Name = "checkMatchWholeWord";
            this.checkMatchWholeWord.Size = new System.Drawing.Size(113, 17);
            this.checkMatchWholeWord.TabIndex = 5;
            this.checkMatchWholeWord.Text = "Match whole word";
            this.checkMatchWholeWord.UseVisualStyleBackColor = true;
            this.checkMatchWholeWord.CheckedChanged += new System.EventHandler(this.resetSearch);
            // 
            // checkUseRegex
            // 
            this.checkUseRegex.AutoSize = true;
            this.checkUseRegex.Location = new System.Drawing.Point(12, 126);
            this.checkUseRegex.Name = "checkUseRegex";
            this.checkUseRegex.Size = new System.Drawing.Size(138, 17);
            this.checkUseRegex.TabIndex = 6;
            this.checkUseRegex.Text = "Use regular expressions";
            this.checkUseRegex.UseVisualStyleBackColor = true;
            this.checkUseRegex.CheckedChanged += new System.EventHandler(this.resetSearch);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(202, 122);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Find Next";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TextSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 157);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkUseRegex);
            this.Controls.Add(this.checkMatchWholeWord);
            this.Controls.Add(this.checkMatchCase);
            this.Controls.Add(this.searchStrings);
            this.Controls.Add(this.searchKeys);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Name = "TextSearch";
            this.Text = "Find String";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RadioButton searchKeys;
        private System.Windows.Forms.RadioButton searchStrings;
        private System.Windows.Forms.CheckBox checkMatchCase;
        private System.Windows.Forms.CheckBox checkMatchWholeWord;
        private System.Windows.Forms.CheckBox checkUseRegex;
        private System.Windows.Forms.Button button1;
    }
}