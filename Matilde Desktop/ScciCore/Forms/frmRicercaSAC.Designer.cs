namespace UnicodeSrl.ScciCore
{
    partial class frmRicercaSAC
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
            this.ucRicercaSAC = new UnicodeSrl.ScciCore.ucRicercaSAC();
            this.SuspendLayout();
                                                this.ucRicercaSAC.BackColor = System.Drawing.Color.Transparent;
            this.ucRicercaSAC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucRicercaSAC.GridDataRowFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ucRicercaSAC.GridHeaderFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ucRicercaSAC.LabelsFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ucRicercaSAC.Location = new System.Drawing.Point(0, 30);
            this.ucRicercaSAC.Name = "ucRicercaSAC";
            this.ucRicercaSAC.Size = new System.Drawing.Size(784, 472);
            this.ucRicercaSAC.TabIndex = 9;
            this.ucRicercaSAC.TextBoxFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Large;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.ucRicercaSAC);
            this.Name = "frmRicercaSAC";
            this.PulsanteAvantiVisibile = true;
            this.PulsanteIndietroVisibile = true;
            this.Text = "frmRicercaSAC";
            this.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmRicercaSAC_PulsanteIndietroClick);
            this.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmRicercaSAC_PulsanteAvantiClick);
            this.Controls.SetChildIndex(this.ucRicercaSAC, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private ucRicercaSAC ucRicercaSAC;
    }
}