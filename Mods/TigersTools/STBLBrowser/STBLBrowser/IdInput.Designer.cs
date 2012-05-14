namespace STBLBrowser
{
    partial class IdInput
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.byName = new System.Windows.Forms.RadioButton();
            this.byID = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(76, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(76, 29);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 1;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox2_KeyPress);
            // 
            // byName
            // 
            this.byName.AutoSize = true;
            this.byName.Location = new System.Drawing.Point(3, 4);
            this.byName.Name = "byName";
            this.byName.Size = new System.Drawing.Size(68, 17);
            this.byName.TabIndex = 2;
            this.byName.TabStop = true;
            this.byName.Text = "By Name";
            this.byName.UseVisualStyleBackColor = true;
            this.byName.CheckedChanged += new System.EventHandler(this.byName_CheckedChanged);
            // 
            // byID
            // 
            this.byID.AutoSize = true;
            this.byID.Location = new System.Drawing.Point(3, 30);
            this.byID.Name = "byID";
            this.byID.Size = new System.Drawing.Size(51, 17);
            this.byID.TabIndex = 3;
            this.byID.TabStop = true;
            this.byID.Text = "By ID";
            this.byID.UseVisualStyleBackColor = true;
            this.byID.CheckedChanged += new System.EventHandler(this.byName_CheckedChanged);
            // 
            // IdInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.byID);
            this.Controls.Add(this.byName);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Name = "IdInput";
            this.Size = new System.Drawing.Size(180, 52);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.RadioButton byName;
        private System.Windows.Forms.RadioButton byID;
    }
}
