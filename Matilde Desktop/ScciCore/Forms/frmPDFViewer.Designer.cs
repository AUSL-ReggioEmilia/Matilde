namespace UnicodeSrl.ScciCore
{
    partial class frmPDFViewer
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
            this.ucEasyO2SPDFView = new UnicodeSrl.ScciCore.ucEasyO2SPDFView();
            this.SuspendLayout();
                                                this.ucBottomModale.Size = new System.Drawing.Size(784, 24);
                                                this.ucEasyO2SPDFView.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyO2SPDFView.BookmarksVisible = UnicodeSrl.ScciCore.easyStatics.DocumentBookmarkVisibility.auto;
            this.ucEasyO2SPDFView.CustomActionEnabled = true;
            this.ucEasyO2SPDFView.CustomActionImage = null;
            this.ucEasyO2SPDFView.CustomActionShortcutKey = System.Windows.Forms.Keys.None;
            this.ucEasyO2SPDFView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyO2SPDFView.Location = new System.Drawing.Point(0, 24);
            this.ucEasyO2SPDFView.Name = "ucEasyO2SPDFView";
            this.ucEasyO2SPDFView.PDFFileFullPath = "";
            this.ucEasyO2SPDFView.ShowCustomAction = false;
            this.ucEasyO2SPDFView.ShowPrint = false;
            this.ucEasyO2SPDFView.Size = new System.Drawing.Size(784, 514);
            this.ucEasyO2SPDFView.TabIndex = 3;
            this.ucEasyO2SPDFView.CustomActionClick += new System.EventHandler(this.ucEasyO2SPDFView_CustomActionClick);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.ucEasyO2SPDFView);
            this.Name = "frmPDFViewer";
            this.PulsanteAvantiTesto = "STAMPA";
            this.PulsanteAvantiVisibile = true;
            this.PulsanteIndietroTesto = "CHIUDI";
            this.PulsanteIndietroVisibile = true;
            this.Text = "frmPDFViewer";
            this.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmPDFViewer_PulsanteIndietroClick);
            this.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmPDFViewer_PulsanteAvantiClick);
            this.Controls.SetChildIndex(this.ucEasyO2SPDFView, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private ucEasyO2SPDFView ucEasyO2SPDFView;

    }
}