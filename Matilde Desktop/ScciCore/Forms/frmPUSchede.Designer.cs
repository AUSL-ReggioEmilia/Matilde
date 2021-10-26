namespace UnicodeSrl.ScciCore
{
    partial class frmPUSchede
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
            this.schede = new UnicodeSrl.ScciCore.ucSchede();
            this.SuspendLayout();
                                                this.ucBottomModale.Size = new System.Drawing.Size(784, 24);
                                                this.schede.Dock = System.Windows.Forms.DockStyle.Fill;
            this.schede.Location = new System.Drawing.Point(0, 24);
            this.schede.Name = "schede";
            this.schede.Size = new System.Drawing.Size(784, 514);
            this.schede.TabIndex = 3;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.schede);
            this.Name = "frmPUSchede";
            this.PulsanteAvantiVisibile = true;
            this.PulsanteIndietroVisibile = true;
            this.Text = "frmPUSchede";
            this.Controls.SetChildIndex(this.schede, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private ucSchede schede;
    }
}