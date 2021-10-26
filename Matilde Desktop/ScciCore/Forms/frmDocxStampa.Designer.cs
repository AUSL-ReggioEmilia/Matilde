namespace UnicodeSrl.ScciCore.Forms
{
    partial class frmDocxStampa
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

        #region Windows Form Designer generated code

                                        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dcv = new DevExpress.XtraPdfViewer.PdfViewer();
            this.SuspendLayout();
                                                this.dcv.Location = new System.Drawing.Point(42, 38);
            this.dcv.Name = "dcv";
            this.dcv.Size = new System.Drawing.Size(207, 183);
            this.dcv.TabIndex = 1;
            this.dcv.Load += new System.EventHandler(this.dcv_DocLoaded);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 481);
            this.ControlBox = false;
            this.Controls.Add(this.dcv);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDocxStampa";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "frmDocxStampa";
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraPdfViewer.PdfViewer dcv;
    }
}