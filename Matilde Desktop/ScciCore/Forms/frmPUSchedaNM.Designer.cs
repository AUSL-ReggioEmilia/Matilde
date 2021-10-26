namespace UnicodeSrl.ScciCore
{
    partial class frmPUSchedaNM
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
            this.ultraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.ucEasyTableLayoutPanelDC = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.ucAnteprimaRtf = new UnicodeSrl.ScciCore.ucAnteprimaRtf();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).BeginInit();
            this.ultraGroupBox.SuspendLayout();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            this.ucEasyTableLayoutPanelDC.SuspendLayout();
            this.SuspendLayout();
                                                this.ucTopNonModale.Size = new System.Drawing.Size(784, 24);
                                                this.ucBottomNonModale.Size = new System.Drawing.Size(784, 24);
                                                this.ultraGroupBox.Controls.Add(this.ucEasyTableLayoutPanel);
            this.ultraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox.Location = new System.Drawing.Point(0, 24);
            this.ultraGroupBox.Name = "ultraGroupBox";
            this.ultraGroupBox.Size = new System.Drawing.Size(784, 513);
            this.ultraGroupBox.TabIndex = 6;
            this.ultraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.ucEasyTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyTableLayoutPanel.ColumnCount = 3;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 96F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyTableLayoutPanelDC, 1, 1);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 3;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 96F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(778, 507);
            this.ucEasyTableLayoutPanel.TabIndex = 0;
                                                this.ucEasyTableLayoutPanelDC.ColumnCount = 1;
            this.ucEasyTableLayoutPanelDC.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ucEasyTableLayoutPanelDC.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanelDC.Controls.Add(this.ucAnteprimaRtf, 0, 0);
            this.ucEasyTableLayoutPanelDC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanelDC.Location = new System.Drawing.Point(18, 13);
            this.ucEasyTableLayoutPanelDC.Name = "ucEasyTableLayoutPanelDC";
            this.ucEasyTableLayoutPanelDC.RowCount = 1;
            this.ucEasyTableLayoutPanelDC.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ucEasyTableLayoutPanelDC.Size = new System.Drawing.Size(740, 480);
            this.ucEasyTableLayoutPanelDC.TabIndex = 19;
                                                this.ucAnteprimaRtf.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucAnteprimaRtf.Location = new System.Drawing.Point(3, 3);
            this.ucAnteprimaRtf.MovScheda = null;
            this.ucAnteprimaRtf.Name = "ucAnteprimaRtf";
            this.ucAnteprimaRtf.Size = new System.Drawing.Size(734, 474);
            this.ucAnteprimaRtf.TabIndex = 0;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.ultraGroupBox);
            this.Name = "frmPUSchedaNM";
            this.Text = "frmPUSchedaNM";
            this.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmPUSchedaNM_PulsanteIndietroClick);
            this.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmPUSchedaNM_PulsanteAvantiClick);
            this.Shown += new System.EventHandler(this.frmPUSchedaNM_Shown);
            this.Controls.SetChildIndex(this.ultraGroupBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).EndInit();
            this.ultraGroupBox.ResumeLayout(false);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            this.ucEasyTableLayoutPanelDC.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox;
        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        private ucEasyTableLayoutPanel ucEasyTableLayoutPanelDC;
        private ucAnteprimaRtf ucAnteprimaRtf;
    }
}