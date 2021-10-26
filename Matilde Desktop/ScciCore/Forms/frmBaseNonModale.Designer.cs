namespace UnicodeSrl.ScciCore
{
    partial class frmBaseNonModale
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
            this.UltraPanelTop = new Infragistics.Win.Misc.UltraPanel();
            this.ucTopNonModale = new UnicodeSrl.ScciCore.ucTopNonModale();
            this.UltraPanelBottom = new Infragistics.Win.Misc.UltraPanel();
            this.ucBottomNonModale = new UnicodeSrl.ScciCore.ucBottomNonModale();
            this.UltraPanelTop.ClientArea.SuspendLayout();
            this.UltraPanelTop.SuspendLayout();
            this.UltraPanelBottom.ClientArea.SuspendLayout();
            this.UltraPanelBottom.SuspendLayout();
            this.SuspendLayout();
                                                this.UltraPanelTop.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
                                                this.UltraPanelTop.ClientArea.Controls.Add(this.ucTopNonModale);
            this.UltraPanelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.UltraPanelTop.Location = new System.Drawing.Point(0, 0);
            this.UltraPanelTop.Margin = new System.Windows.Forms.Padding(0);
            this.UltraPanelTop.Name = "UltraPanelTop";
            this.UltraPanelTop.Size = new System.Drawing.Size(784, 100);
            this.UltraPanelTop.TabIndex = 3;
            this.UltraPanelTop.TabStop = false;
                                                this.ucTopNonModale.BackColor = System.Drawing.Color.DeepPink;
            this.ucTopNonModale.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucTopNonModale.Location = new System.Drawing.Point(0, 0);
            this.ucTopNonModale.Margin = new System.Windows.Forms.Padding(0);
            this.ucTopNonModale.Maschera = null;
            this.ucTopNonModale.Name = "ucTopNonModale";
            this.ucTopNonModale.Size = new System.Drawing.Size(784, 100);
            this.ucTopNonModale.TabIndex = 0;
            this.ucTopNonModale.TabStop = false;
            this.ucTopNonModale.ViewController = null;
                                                this.UltraPanelBottom.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
                                                this.UltraPanelBottom.ClientArea.Controls.Add(this.ucBottomNonModale);
            this.UltraPanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.UltraPanelBottom.Location = new System.Drawing.Point(0, 501);
            this.UltraPanelBottom.Margin = new System.Windows.Forms.Padding(0);
            this.UltraPanelBottom.Name = "UltraPanelBottom";
            this.UltraPanelBottom.Size = new System.Drawing.Size(784, 60);
            this.UltraPanelBottom.TabIndex = 4;
            this.UltraPanelBottom.TabStop = false;
                                                this.ucBottomNonModale.BackColor = System.Drawing.Color.DeepPink;
            this.ucBottomNonModale.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucBottomNonModale.Location = new System.Drawing.Point(0, 0);
            this.ucBottomNonModale.Margin = new System.Windows.Forms.Padding(0);
            this.ucBottomNonModale.Maschera = null;
            this.ucBottomNonModale.Name = "ucBottomNonModale";
            this.ucBottomNonModale.Size = new System.Drawing.Size(784, 60);
            this.ucBottomNonModale.TabIndex = 0;
            this.ucBottomNonModale.TabStop = false;
            this.ucBottomNonModale.ViewController = null;
            this.ucBottomNonModale.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.ucBottomModale_PulsanteIndietroClick);
            this.ucBottomNonModale.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.ucBottomModale_PulsanteAvantiClick);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.UltraPanelTop);
            this.Controls.Add(this.UltraPanelBottom);
            this.KeyPreview = true;
            this.Name = "frmBaseNonModale";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmBaseNonModale";
            this.Load += new System.EventHandler(this.frmBaseNonModale_Load);
            this.TextChanged += new System.EventHandler(this.frmBaseNonModale_TextChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmBaseNonModale_KeyDown);
            this.Resize += new System.EventHandler(this.frmBaseNonModale_Resize);
            this.UltraPanelTop.ClientArea.ResumeLayout(false);
            this.UltraPanelTop.ResumeLayout(false);
            this.UltraPanelBottom.ClientArea.ResumeLayout(false);
            this.UltraPanelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraPanel UltraPanelTop;
        private Infragistics.Win.Misc.UltraPanel UltraPanelBottom;
        public ucTopNonModale ucTopNonModale;
        public ucBottomNonModale ucBottomNonModale;
    }
}