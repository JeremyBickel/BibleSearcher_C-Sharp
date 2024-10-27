namespace KJVStrongs
{
    partial class BibleLinks
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tbxBibleText = new System.Windows.Forms.TextBox();
            this.cboBook = new System.Windows.Forms.ComboBox();
            this.cboChapter = new System.Windows.Forms.ComboBox();
            this.cboVerse = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1068, 811);
            this.splitContainer1.SplitterDistance = 520;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.cboBook);
            this.splitContainer2.Panel1.Controls.Add(this.cboChapter);
            this.splitContainer2.Panel1.Controls.Add(this.cboVerse);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tbxBibleText);
            this.splitContainer2.Size = new System.Drawing.Size(1068, 520);
            this.splitContainer2.SplitterDistance = 60;
            this.splitContainer2.TabIndex = 0;
            // 
            // tbxBibleText
            // 
            this.tbxBibleText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbxBibleText.Location = new System.Drawing.Point(0, 0);
            this.tbxBibleText.Multiline = true;
            this.tbxBibleText.Name = "tbxBibleText";
            this.tbxBibleText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbxBibleText.Size = new System.Drawing.Size(1068, 456);
            this.tbxBibleText.TabIndex = 0;
            // 
            // cboBook
            // 
            this.cboBook.FormattingEnabled = true;
            this.cboBook.Location = new System.Drawing.Point(12, 12);
            this.cboBook.Name = "cboBook";
            this.cboBook.Size = new System.Drawing.Size(172, 23);
            this.cboBook.TabIndex = 1;
            this.cboBook.SelectedIndexChanged += new System.EventHandler(this.cboBook_SelectedIndexChanged);
            // 
            // cboChapter
            // 
            this.cboChapter.FormattingEnabled = true;
            this.cboChapter.Location = new System.Drawing.Point(190, 12);
            this.cboChapter.Name = "cboChapter";
            this.cboChapter.Size = new System.Drawing.Size(66, 23);
            this.cboChapter.TabIndex = 2;
            this.cboChapter.SelectedIndexChanged += new System.EventHandler(this.cboChapter_SelectedIndexChanged);
            // 
            // cboVerse
            // 
            this.cboVerse.FormattingEnabled = true;
            this.cboVerse.Location = new System.Drawing.Point(262, 12);
            this.cboVerse.Name = "cboVerse";
            this.cboVerse.Size = new System.Drawing.Size(66, 23);
            this.cboVerse.TabIndex = 3;
            this.cboVerse.SelectedIndexChanged += new System.EventHandler(this.cboVerse_SelectedIndexChanged);
            // 
            // BibleLinks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1068, 811);
            this.Controls.Add(this.splitContainer1);
            this.Name = "BibleLinks";
            this.Text = "BibleLinks";
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private ComboBox cboBook;
        private ComboBox cboChapter;
        private ComboBox cboVerse;
        private TextBox tbxBibleText;
    }
}