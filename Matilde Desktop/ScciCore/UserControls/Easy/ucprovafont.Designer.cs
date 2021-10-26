namespace UnicodeSrl.ScciCore
{
    partial class ucprovafont
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

        #region Component Designer generated code

                                        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucprovafont));
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Ricerca", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.ultraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.cmdRicerca = new UnicodeSrl.ScciCore.ucEasyButton();
            this.lblTEST = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.lblfontsize = new UnicodeSrl.ScciCore.ucEasyLabel();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).BeginInit();
            this.ultraGroupBox.SuspendLayout();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
                                                this.ultraToolTipManager1.ContainingControl = this;
                                                this.ultraGroupBox.Controls.Add(this.ucEasyTableLayoutPanel);
            this.ultraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox.Name = "ultraGroupBox";
            this.ultraGroupBox.Size = new System.Drawing.Size(878, 624);
            this.ultraGroupBox.TabIndex = 8;
            this.ultraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.ucEasyTableLayoutPanel.ColumnCount = 6;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.cmdRicerca, 2, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.lblTEST, 1, 3);
            this.ucEasyTableLayoutPanel.Controls.Add(this.lblfontsize, 1, 1);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 5;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 3F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(872, 618);
            this.ucEasyTableLayoutPanel.TabIndex = 0;
                                                appearance1.FontData.SizeInPoints = 16.25F;
            appearance1.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance1.ImageBackground")));
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance1.TextHAlignAsString = "Center";
            appearance1.TextVAlignAsString = "Middle";
            this.cmdRicerca.Appearance = appearance1;
            this.cmdRicerca.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ucEasyTableLayoutPanel.SetColumnSpan(this.cmdRicerca, 3);
            this.cmdRicerca.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdRicerca.Location = new System.Drawing.Point(734, 9);
            this.cmdRicerca.Name = "cmdRicerca";
            this.cmdRicerca.PercImageFill = 0.75F;
            this.cmdRicerca.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.XSmall;
            this.cmdRicerca.ShortcutKey = System.Windows.Forms.Keys.F5;
            this.cmdRicerca.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.cmdRicerca.Size = new System.Drawing.Size(123, 31);
            this.cmdRicerca.TabIndex = 9;
            this.cmdRicerca.Text = "Ricerca";
            this.cmdRicerca.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            ultraToolTipInfo1.ToolTipText = "Ricerca";
            this.ultraToolTipManager1.SetUltraToolTip(this.cmdRicerca, ultraToolTipInfo1);
            this.cmdRicerca.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.cmdRicerca.Click += new System.EventHandler(this.cmdRicerca_Click);
                                                appearance2.TextHAlignAsString = "Center";
            appearance2.TextVAlignAsString = "Middle";
            this.lblTEST.Appearance = appearance2;
            this.lblTEST.AutoEllipsis = false;
            this.lblTEST.BorderStyleOuter = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ucEasyTableLayoutPanel.SetColumnSpan(this.lblTEST, 4);
            this.lblTEST.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTEST.Font = new System.Drawing.Font("Microsoft Sans Serif", 290F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTEST.Location = new System.Drawing.Point(37, 52);
            this.lblTEST.Name = "lblTEST";
            this.lblTEST.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.lblTEST.ShortcutKey = System.Windows.Forms.Keys.None;
            this.lblTEST.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.lblTEST.Size = new System.Drawing.Size(820, 544);
            this.lblTEST.TabIndex = 10;
            this.lblTEST.Text = "999";
            this.lblTEST.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions._undefined;
                                                appearance3.FontData.SizeInPoints = 20F;
            appearance3.TextVAlignAsString = "Middle";
            this.lblfontsize.Appearance = appearance3;
            this.lblfontsize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblfontsize.Location = new System.Drawing.Point(37, 9);
            this.lblfontsize.Name = "lblfontsize";
            this.lblfontsize.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.lblfontsize.ShortcutKey = System.Windows.Forms.Keys.None;
            this.lblfontsize.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.lblfontsize.Size = new System.Drawing.Size(691, 31);
            this.lblfontsize.TabIndex = 10;
            this.lblfontsize.Text = "ucEasyLabel1";
            this.lblfontsize.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Large;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.ultraGroupBox);
            this.Name = "ucprovafont";
            this.Size = new System.Drawing.Size(878, 624);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).EndInit();
            this.ultraGroupBox.ResumeLayout(false);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        private ucEasyButton cmdRicerca;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox;
        private ucEasyLabel lblTEST;
        private ucEasyLabel lblfontsize;
    }
}
