namespace UnicodeSrl.ScciCore
{
    partial class ucAnteprimaRtf
    {
                                private System.ComponentModel.IContainer components = null;

                                        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

                                        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rtbRichTextBox = new ExtendedRichTextBox.RichTextBoxPrintCtrl();
            this.SuspendLayout();
                                                this.rtbRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbRichTextBox.HideSelection = false;
            this.rtbRichTextBox.Location = new System.Drawing.Point(0, 0);
            this.rtbRichTextBox.Name = "rtbRichTextBox";
            this.rtbRichTextBox.Size = new System.Drawing.Size(315, 272);
            this.rtbRichTextBox.TabIndex = 1;
            this.rtbRichTextBox.Text = "";
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtbRichTextBox);
            this.Name = "ucAnteprimaRtf";
            this.Size = new System.Drawing.Size(315, 272);
            this.ResumeLayout(false);

        }

        #endregion

        public ExtendedRichTextBox.RichTextBoxPrintCtrl rtbRichTextBox;


    }
}
