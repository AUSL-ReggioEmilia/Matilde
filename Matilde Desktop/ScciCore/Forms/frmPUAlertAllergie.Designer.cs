namespace UnicodeSrl.ScciCore
{
    partial class frmPUAlertAllergie
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
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            this.ultraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.lblZoomTipoAlertAllergiaAnamnesi = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.udteDataEvento = new UnicodeSrl.ScciCore.ucEasyDateTimeEditor();
            this.lblDataEvento = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.ubZoomTipo = new UnicodeSrl.ScciCore.ucEasyButton();
            this.ehViewer = new System.Windows.Forms.Integration.ElementHost();
            this.ucDcViewer = new UnicodeSrl.WpfControls40.ucDcViewer();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).BeginInit();
            this.ultraGroupBox.SuspendLayout();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udteDataEvento)).BeginInit();
            this.SuspendLayout();
                                                this.ultraGroupBox.Controls.Add(this.ucEasyTableLayoutPanel);
            this.ultraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox.Location = new System.Drawing.Point(0, 24);
            this.ultraGroupBox.Name = "ultraGroupBox";
            this.ultraGroupBox.Size = new System.Drawing.Size(784, 514);
            this.ultraGroupBox.TabIndex = 9;
            this.ultraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.ucEasyTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyTableLayoutPanel.ColumnCount = 6;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.lblZoomTipoAlertAllergiaAnamnesi, 3, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.udteDataEvento, 1, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.lblDataEvento, 1, 0);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ubZoomTipo, 4, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ehViewer, 1, 3);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 5;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 84F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(778, 508);
            this.ucEasyTableLayoutPanel.TabIndex = 0;
                                                appearance1.FontData.SizeInPoints = 20F;
            appearance1.TextVAlignAsString = "Middle";
            this.lblZoomTipoAlertAllergiaAnamnesi.Appearance = appearance1;
            this.lblZoomTipoAlertAllergiaAnamnesi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblZoomTipoAlertAllergiaAnamnesi.Location = new System.Drawing.Point(266, 28);
            this.lblZoomTipoAlertAllergiaAnamnesi.Name = "lblZoomTipoAlertAllergiaAnamnesi";
            this.lblZoomTipoAlertAllergiaAnamnesi.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.lblZoomTipoAlertAllergiaAnamnesi.ShortcutKey = System.Windows.Forms.Keys.None;
            this.lblZoomTipoAlertAllergiaAnamnesi.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.lblZoomTipoAlertAllergiaAnamnesi.Size = new System.Drawing.Size(460, 29);
            this.lblZoomTipoAlertAllergiaAnamnesi.TabIndex = 4;
            this.lblZoomTipoAlertAllergiaAnamnesi.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Large;
                                                appearance2.FontData.SizeInPoints = 16.25F;
            this.udteDataEvento.Appearance = appearance2;
            this.udteDataEvento.AutoSize = false;
            this.udteDataEvento.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2007;
            this.udteDataEvento.Dock = System.Windows.Forms.DockStyle.Fill;
            appearance3.FontData.SizeInPoints = 16.25F;
            this.udteDataEvento.DropDownAppearance = appearance3;
            this.udteDataEvento.FormatString = "dd/MM/yyyy HH:mm";
            this.udteDataEvento.Location = new System.Drawing.Point(18, 28);
            this.udteDataEvento.MaskInput = "{LOC}dd/mm/yyyy hh:mm";
            this.udteDataEvento.Name = "udteDataEvento";
            this.udteDataEvento.Size = new System.Drawing.Size(227, 29);
            this.udteDataEvento.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.udteDataEvento.TabIndex = 2;
            this.udteDataEvento.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.udteDataEvento.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
                                                appearance4.FontData.SizeInPoints = 16.25F;
            appearance4.TextVAlignAsString = "Middle";
            this.lblDataEvento.Appearance = appearance4;
            this.lblDataEvento.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataEvento.Location = new System.Drawing.Point(18, 3);
            this.lblDataEvento.Name = "lblDataEvento";
            this.lblDataEvento.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.lblDataEvento.ShortcutKey = System.Windows.Forms.Keys.None;
            this.lblDataEvento.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.lblDataEvento.Size = new System.Drawing.Size(227, 19);
            this.lblDataEvento.TabIndex = 0;
            this.lblDataEvento.Text = "Data Rilevazione:";
            this.lblDataEvento.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
                                                appearance5.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance5.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance5.TextHAlignAsString = "Center";
            appearance5.TextVAlignAsString = "Bottom";
            this.ubZoomTipo.Appearance = appearance5;
            this.ubZoomTipo.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ubZoomTipo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ubZoomTipo.Location = new System.Drawing.Point(732, 28);
            this.ubZoomTipo.Name = "ubZoomTipo";
            this.ubZoomTipo.PercImageFill = 0.75F;
            this.ubZoomTipo.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ubZoomTipo.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ubZoomTipo.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ubZoomTipo.Size = new System.Drawing.Size(25, 29);
            this.ubZoomTipo.TabIndex = 5;
            this.ubZoomTipo.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions._undefined;
            this.ubZoomTipo.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ubZoomTipo.Click += new System.EventHandler(this.ubZoomTipo_Click);
                                                this.ucEasyTableLayoutPanel.SetColumnSpan(this.ehViewer, 4);
            this.ehViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ehViewer.Location = new System.Drawing.Point(18, 73);
            this.ehViewer.Name = "ehViewer";
            this.ehViewer.Size = new System.Drawing.Size(739, 420);
            this.ehViewer.TabIndex = 6;
            this.ehViewer.Text = "elementHost1";
            this.ehViewer.Child = this.ucDcViewer;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.ultraGroupBox);
            this.Name = "frmPUAlertAllergie";
            this.PulsanteAvantiVisibile = true;
            this.PulsanteIndietroVisibile = true;
            this.Text = "frmPUAlertAllergie";
            this.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmPUAlertAllergie_PulsanteIndietroClick);
            this.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmPUAlertAllergie_PulsanteAvantiClick);
            this.Shown += new System.EventHandler(this.frmPUAlertAllergie_Shown);
            this.Controls.SetChildIndex(this.ultraGroupBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).EndInit();
            this.ultraGroupBox.ResumeLayout(false);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.udteDataEvento)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox;
        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        private ucEasyLabel lblZoomTipoAlertAllergiaAnamnesi;
        private ucEasyDateTimeEditor udteDataEvento;
        private ucEasyLabel lblDataEvento;
        private ucEasyButton ubZoomTipo;
        private System.Windows.Forms.Integration.ElementHost ehViewer;
        private UnicodeSrl.WpfControls40.ucDcViewer ucDcViewer;
    }
}