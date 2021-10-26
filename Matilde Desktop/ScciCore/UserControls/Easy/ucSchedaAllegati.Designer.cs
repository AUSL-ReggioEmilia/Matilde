namespace UnicodeSrl.ScciCore
{
    partial class ucSchedaAllegati
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
            this.ehImageViewer = new System.Windows.Forms.Integration.ElementHost();
            this.ucMultiImageViewer = new UnicodeSrl.WpfControls.ucMultiImageViewer();
            this.SuspendLayout();
                                                this.ehImageViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ehImageViewer.Location = new System.Drawing.Point(0, 0);
            this.ehImageViewer.Name = "ehImageViewer";
            this.ehImageViewer.Size = new System.Drawing.Size(526, 411);
            this.ehImageViewer.TabIndex = 0;
            this.ehImageViewer.Text = "elementHost1";
            this.ehImageViewer.Child = this.ucMultiImageViewer;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.ehImageViewer);
            this.Name = "ucSchedaAllegati";
            this.Size = new System.Drawing.Size(526, 411);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost ehImageViewer;
        private WpfControls.ucMultiImageViewer ucMultiImageViewer;
    }
}
