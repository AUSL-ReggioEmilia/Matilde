namespace UnicodeSrl.ScciCore
{
    partial class frmPUScheda
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.ehViewer = new System.Windows.Forms.Integration.ElementHost();
            this.ucDcViewer = new UnicodeSrl.WpfControls40.ucDcViewer();
            this.UltraPopupControlContainerSegnalibri = new Infragistics.Win.Misc.UltraPopupControlContainer(this.components);
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.ucEasyTableLayoutPanelButton = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.ucSbInEvidenza = new UnicodeSrl.ScciCore.ucEasyStateButton();
            this.ucSbValida = new UnicodeSrl.ScciCore.ucEasyStateButton();
            this.UltraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            this.ucEasyTableLayoutPanelButton.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBox)).BeginInit();
            this.UltraGroupBox.SuspendLayout();
            this.SuspendLayout();
                                                this.ehViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ehViewer.Location = new System.Drawing.Point(0, 0);
            this.ehViewer.Margin = new System.Windows.Forms.Padding(0);
            this.ehViewer.Name = "ehViewer";
            this.ehViewer.Size = new System.Drawing.Size(778, 444);
            this.ehViewer.TabIndex = 0;
            this.ehViewer.Text = "elementHost1";
            this.ehViewer.Child = this.ucDcViewer;
                                                this.UltraPopupControlContainerSegnalibri.Opening += new System.ComponentModel.CancelEventHandler(this.UltraPopupControlContainerSegnalibri_Opening);
            this.UltraPopupControlContainerSegnalibri.Opened += new System.EventHandler(this.UltraPopupControlContainerSegnalibri_Opened);
            this.UltraPopupControlContainerSegnalibri.Closed += new System.EventHandler(this.UltraPopupControlContainerSegnalibri_Closed);
                                                this.ucEasyTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyTableLayoutPanel.ColumnCount = 1;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.ehViewer, 0, 0);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyTableLayoutPanelButton, 0, 1);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 2;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(778, 508);
            this.ucEasyTableLayoutPanel.TabIndex = 3;
                                                this.ucEasyTableLayoutPanelButton.ColumnCount = 3;
            this.ucEasyTableLayoutPanelButton.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ucEasyTableLayoutPanelButton.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.ucEasyTableLayoutPanelButton.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.ucEasyTableLayoutPanelButton.Controls.Add(this.ucSbInEvidenza, 1, 0);
            this.ucEasyTableLayoutPanelButton.Controls.Add(this.ucSbValida, 2, 0);
            this.ucEasyTableLayoutPanelButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanelButton.Location = new System.Drawing.Point(3, 447);
            this.ucEasyTableLayoutPanelButton.Name = "ucEasyTableLayoutPanelButton";
            this.ucEasyTableLayoutPanelButton.RowCount = 1;
            this.ucEasyTableLayoutPanelButton.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ucEasyTableLayoutPanelButton.Size = new System.Drawing.Size(772, 58);
            this.ucEasyTableLayoutPanelButton.TabIndex = 1;
                                                appearance1.FontData.SizeInPoints = 16.25F;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance1.TextHAlignAsString = "Center";
            appearance1.TextVAlignAsString = "Bottom";
            this.ucSbInEvidenza.Appearance = appearance1;
            this.ucSbInEvidenza.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ucSbInEvidenza.Checked = false;
            this.ucSbInEvidenza.CheckedImage = null;
            this.ucSbInEvidenza.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucSbInEvidenza.Location = new System.Drawing.Point(647, 3);
            this.ucSbInEvidenza.Name = "ucSbInEvidenza";
            this.ucSbInEvidenza.PercImageFill = 0.75F;
            this.ucSbInEvidenza.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ucSbInEvidenza.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ucSbInEvidenza.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ucSbInEvidenza.Size = new System.Drawing.Size(58, 52);
            this.ucSbInEvidenza.TabIndex = 5;
            this.ucSbInEvidenza.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ucSbInEvidenza.UNCheckedImage = null;
            this.ucSbInEvidenza.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ucSbInEvidenza.Visible = false;
                                                appearance2.FontData.SizeInPoints = 16.25F;
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance2.TextHAlignAsString = "Center";
            appearance2.TextVAlignAsString = "Bottom";
            this.ucSbValida.Appearance = appearance2;
            this.ucSbValida.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ucSbValida.Checked = false;
            this.ucSbValida.CheckedImage = null;
            this.ucSbValida.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucSbValida.Location = new System.Drawing.Point(711, 3);
            this.ucSbValida.Name = "ucSbValida";
            this.ucSbValida.PercImageFill = 0.75F;
            this.ucSbValida.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ucSbValida.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ucSbValida.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ucSbValida.Size = new System.Drawing.Size(58, 52);
            this.ucSbValida.TabIndex = 4;
            this.ucSbValida.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ucSbValida.UNCheckedImage = null;
            this.ucSbValida.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
                                                this.UltraGroupBox.Controls.Add(this.ucEasyTableLayoutPanel);
            this.UltraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UltraGroupBox.Location = new System.Drawing.Point(0, 24);
            this.UltraGroupBox.Name = "UltraGroupBox";
            this.UltraGroupBox.Size = new System.Drawing.Size(784, 514);
            this.UltraGroupBox.TabIndex = 4;
            this.UltraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.UltraGroupBox);
            this.Name = "frmPUScheda";
            this.PulsanteAvantiVisibile = true;
            this.PulsanteIndietroVisibile = true;
            this.Text = "frmPUScheda";
            this.ImmagineClick += new UnicodeSrl.ScciCore.ImmagineTopClickHandler(this.frmPUScheda_ImmagineClick);
            this.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmPUScheda_PulsanteIndietroClick);
            this.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmPUScheda_PulsanteAvantiClick);
            this.Shown += new System.EventHandler(this.frmPUScheda_Shown);
            this.Controls.SetChildIndex(this.UltraGroupBox, 0);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            this.ucEasyTableLayoutPanelButton.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBox)).EndInit();
            this.UltraGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost ehViewer;
        private UnicodeSrl.WpfControls40.ucDcViewer ucDcViewer;
        private Infragistics.Win.Misc.UltraPopupControlContainer UltraPopupControlContainerSegnalibri;
        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        private Infragistics.Win.Misc.UltraGroupBox UltraGroupBox;
        private ucEasyTableLayoutPanel ucEasyTableLayoutPanelButton;
        private ucEasyStateButton ucSbInEvidenza;
        private ucEasyStateButton ucSbValida;
    }
}