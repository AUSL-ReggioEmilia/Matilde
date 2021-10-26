namespace UnicodeSrl.ScciCore
{
    partial class frmReport
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
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab5 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.reportLinkViewer = new ReportManager.ReportHandler.ReportLinkViewer();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ucEasyDOCXViewer = new UnicodeSrl.ScciCore.ucEasyDOCXViewer();
            this.ultraTabPageControl3 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabPageControl4 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabPageControl5 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ucEasyDevExpressViewer = new UnicodeSrl.ScciCore.ucEasyDevExpressViewer();
            this.utcReport = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraTabPageControl1.SuspendLayout();
            this.ultraTabPageControl2.SuspendLayout();
            this.ultraTabPageControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utcReport)).BeginInit();
            this.utcReport.SuspendLayout();
            this.SuspendLayout();
                                                this.ucBottomModale.Size = new System.Drawing.Size(784, 24);
                                                this.ultraTabPageControl1.Controls.Add(this.reportLinkViewer);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(784, 514);
                                                this.reportLinkViewer.AllowLinkTargetBlank = false;
            this.reportLinkViewer.BackColor = System.Drawing.Color.Transparent;
            this.reportLinkViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportLinkViewer.Location = new System.Drawing.Point(0, 0);
            this.reportLinkViewer.Name = "reportLinkViewer";
            this.reportLinkViewer.PreventToolbarOnTargetBlank = false;
            this.reportLinkViewer.Size = new System.Drawing.Size(784, 514);
            this.reportLinkViewer.TabIndex = 0;
            this.reportLinkViewer.ToolbarHidden = true;
            this.reportLinkViewer.ToolBarSize = ReportManager.ReportHandler.ReportLinkViewer.enumToolbarSize.size48x48;
            this.reportLinkViewer.ToolbarSizeOnTargetBlank = ReportManager.ReportHandler.ReportLinkViewer.enumToolbarSize.size32x32;
                                                this.ultraTabPageControl2.Controls.Add(this.ucEasyDOCXViewer);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(784, 514);
                                                this.ucEasyDOCXViewer.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyDOCXViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyDOCXViewer.DOCXFileFullPath = "";
            this.ucEasyDOCXViewer.Location = new System.Drawing.Point(0, 0);
            this.ucEasyDOCXViewer.Name = "ucEasyDOCXViewer";
            this.ucEasyDOCXViewer.ShowPrint = false;
            this.ucEasyDOCXViewer.Size = new System.Drawing.Size(784, 514);
            this.ucEasyDOCXViewer.TabIndex = 0;
            this.ucEasyDOCXViewer.DocumentOpenedOnWord += new System.EventHandler(this.ucEasyDOCXViewer_DocumentOpenedOnWord);
                                                this.ultraTabPageControl3.Location = new System.Drawing.Point(0, 0);
            this.ultraTabPageControl3.Name = "ultraTabPageControl3";
            this.ultraTabPageControl3.Size = new System.Drawing.Size(784, 514);
                                                this.ultraTabPageControl4.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl4.Name = "ultraTabPageControl4";
            this.ultraTabPageControl4.Size = new System.Drawing.Size(784, 514);
                                                this.ultraTabPageControl5.Controls.Add(this.ucEasyDevExpressViewer);
            this.ultraTabPageControl5.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl5.Name = "ultraTabPageControl5";
            this.ultraTabPageControl5.Size = new System.Drawing.Size(784, 514);
                                                this.ucEasyDevExpressViewer.ActiveReport = null;
            this.ucEasyDevExpressViewer.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyDevExpressViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyDevExpressViewer.Location = new System.Drawing.Point(0, 0);
            this.ucEasyDevExpressViewer.Name = "ucEasyDevExpressViewer";
            this.ucEasyDevExpressViewer.PrintButtonDefaultModality = UnicodeSrl.ScciCore.ucEasyDevExpressViewer.enumPrintModality.useActiveReportPrintDialog;
            this.ucEasyDevExpressViewer.ShowPrint = true;
            this.ucEasyDevExpressViewer.Size = new System.Drawing.Size(784, 514);
            this.ucEasyDevExpressViewer.TabIndex = 0;
                                                this.utcReport.Controls.Add(this.ultraTabSharedControlsPage1);
            this.utcReport.Controls.Add(this.ultraTabPageControl1);
            this.utcReport.Controls.Add(this.ultraTabPageControl2);
            this.utcReport.Controls.Add(this.ultraTabPageControl3);
            this.utcReport.Controls.Add(this.ultraTabPageControl4);
            this.utcReport.Controls.Add(this.ultraTabPageControl5);
            this.utcReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utcReport.Location = new System.Drawing.Point(0, 24);
            this.utcReport.Name = "utcReport";
            this.utcReport.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.utcReport.Size = new System.Drawing.Size(784, 514);
            this.utcReport.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard;
            this.utcReport.TabIndex = 9;
            ultraTab1.Key = "tabREM";
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "";
            ultraTab2.Key = "tabWORD";
            ultraTab2.TabPage = this.ultraTabPageControl2;
            ultraTab2.Text = "";
            ultraTab3.Key = "tabCAB";
            ultraTab3.TabPage = this.ultraTabPageControl3;
            ultraTab3.Text = "";
            ultraTab4.Key = "tabPDF";
            ultraTab4.TabPage = this.ultraTabPageControl4;
            ultraTab4.Text = "";
            ultraTab5.Key = "tabDE";
            ultraTab5.TabPage = this.ultraTabPageControl5;
            ultraTab5.Text = "";
            this.utcReport.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2,
            ultraTab3,
            ultraTab4,
            ultraTab5});
            this.utcReport.ViewStyle = Infragistics.Win.UltraWinTabControl.ViewStyle.Office2007;
                                                this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(784, 514);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.utcReport);
            this.Name = "frmReport";
            this.PulsanteAvantiVisibile = true;
            this.PulsanteIndietroVisibile = true;
            this.Text = "frmReport";
            this.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmReport_PulsanteIndietroClick);
            this.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmReport_PulsanteAvantiClick);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmReport_FormClosing);
            this.Shown += new System.EventHandler(this.frmReport_Shown);
            this.Controls.SetChildIndex(this.utcReport, 0);
            this.ultraTabPageControl1.ResumeLayout(false);
            this.ultraTabPageControl2.ResumeLayout(false);
            this.ultraTabPageControl5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.utcReport)).EndInit();
            this.utcReport.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinTabControl.UltraTabControl utcReport;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl3;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl4;
        private ReportManager.ReportHandler.ReportLinkViewer reportLinkViewer;
        private ucEasyDOCXViewer ucEasyDOCXViewer;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl5;
        private ucEasyDevExpressViewer ucEasyDevExpressViewer;
    }
}