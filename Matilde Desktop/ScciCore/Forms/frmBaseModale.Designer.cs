namespace UnicodeSrl.ScciCore
{
    partial class frmBaseModale
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
            this.ucTopModale = new UnicodeSrl.ScciCore.ucTopModale();
            this.UltraPanelBottom = new Infragistics.Win.Misc.UltraPanel();
            this.ucBottomModale = new UnicodeSrl.ScciCore.ucBottomModale();
            this.UltraPanelTop.ClientArea.SuspendLayout();
            this.UltraPanelTop.SuspendLayout();
            this.UltraPanelBottom.ClientArea.SuspendLayout();
            this.UltraPanelBottom.SuspendLayout();
            this.SuspendLayout();
                                                this.UltraPanelTop.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
                                                this.UltraPanelTop.ClientArea.Controls.Add(this.ucTopModale);
            this.UltraPanelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.UltraPanelTop.Location = new System.Drawing.Point(0, 0);
            this.UltraPanelTop.Margin = new System.Windows.Forms.Padding(0);
            this.UltraPanelTop.Name = "UltraPanelTop";
            this.UltraPanelTop.Size = new System.Drawing.Size(784, 100);
            this.UltraPanelTop.TabIndex = 1;
            this.UltraPanelTop.TabStop = false;
                                                this.ucTopModale.BackColor = System.Drawing.Color.Orange;
            this.ucTopModale.CodiceMaschera = "";
            this.ucTopModale.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucTopModale.Location = new System.Drawing.Point(0, 0);
            this.ucTopModale.Margin = new System.Windows.Forms.Padding(0);
            this.ucTopModale.Name = "ucTopModale";
            this.ucTopModale.PazienteVisibile = true;
            this.ucTopModale.Size = new System.Drawing.Size(784, 100);
            this.ucTopModale.TabIndex = 0;
            this.ucTopModale.TabStop = false;
            this.ucTopModale.Titolo = "";
            this.ucTopModale.ImmagineClick += new UnicodeSrl.ScciCore.ImmagineTopClickHandler(this.ucTopModale_ImmagineClick);
                                                this.UltraPanelBottom.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
                                                this.UltraPanelBottom.ClientArea.Controls.Add(this.ucBottomModale);
            this.UltraPanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.UltraPanelBottom.Location = new System.Drawing.Point(0, 502);
            this.UltraPanelBottom.Margin = new System.Windows.Forms.Padding(0);
            this.UltraPanelBottom.Name = "UltraPanelBottom";
            this.UltraPanelBottom.Size = new System.Drawing.Size(784, 60);
            this.UltraPanelBottom.TabIndex = 2;
            this.UltraPanelBottom.TabStop = false;
                                                this.ucBottomModale.BackColor = System.Drawing.Color.Orange;
            this.ucBottomModale.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucBottomModale.Location = new System.Drawing.Point(0, 0);
            this.ucBottomModale.Margin = new System.Windows.Forms.Padding(0);
            this.ucBottomModale.Name = "ucBottomModale";
            this.ucBottomModale.Size = new System.Drawing.Size(784, 60);
            this.ucBottomModale.TabIndex = 0;
            this.ucBottomModale.TabStop = false;
            this.ucBottomModale.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.ucBottomModale_PulsanteIndietroClick);
            this.ucBottomModale.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.ucBottomModale_PulsanteAvantiClick);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.ControlBox = false;
            this.Controls.Add(this.UltraPanelTop);
            this.Controls.Add(this.UltraPanelBottom);
            this.KeyPreview = true;
            this.Name = "frmBaseModale";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmBaseModale";
            this.Load += new System.EventHandler(this.frmBaseModale_Load);
            this.TextChanged += new System.EventHandler(this.frmBaseModale_TextChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmBaseModale_KeyDown);
            this.Resize += new System.EventHandler(this.frmBaseModale_Resize);
            this.UltraPanelTop.ClientArea.ResumeLayout(false);
            this.UltraPanelTop.ResumeLayout(false);
            this.UltraPanelBottom.ClientArea.ResumeLayout(false);
            this.UltraPanelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraPanel UltraPanelTop;
        private Infragistics.Win.Misc.UltraPanel UltraPanelBottom;
        internal ucTopModale ucTopModale;
        public ucBottomModale ucBottomModale;


    }
}