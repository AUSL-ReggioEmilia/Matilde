namespace UnicodeSrl.ScciCore
{
    partial class ucEasyPopUpFolder
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTree.Override _override1 = new Infragistics.Win.UltraWinTree.Override();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            this.ultraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.uceCodEntita = new UnicodeSrl.ScciCore.ucEasyComboEditor();
            this.ucEasyLabel = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.ucEasyButtonConferma = new UnicodeSrl.ScciCore.ucEasyButton();
            this.ucEasyButtonCancel = new UnicodeSrl.ScciCore.ucEasyButton();
            this.tvFolder = new UnicodeSrl.ScciCore.ucEasyTreeView();
            this.txtFolder = new UnicodeSrl.ScciCore.ucEasyTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).BeginInit();
            this.ultraGroupBox.SuspendLayout();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uceCodEntita)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvFolder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFolder)).BeginInit();
            this.SuspendLayout();
                                                this.ultraGroupBox.Controls.Add(this.ucEasyTableLayoutPanel);
            this.ultraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox.Name = "ultraGroupBox";
            this.ultraGroupBox.Size = new System.Drawing.Size(400, 500);
            this.ultraGroupBox.TabIndex = 2;
            this.ultraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.ucEasyTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyTableLayoutPanel.ColumnCount = 3;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.uceCodEntita, 0, 3);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyLabel, 0, 0);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyButtonConferma, 2, 4);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyButtonCancel, 0, 4);
            this.ucEasyTableLayoutPanel.Controls.Add(this.tvFolder, 0, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.txtFolder, 0, 2);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 5;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(394, 494);
            this.ucEasyTableLayoutPanel.TabIndex = 0;
                                                appearance1.FontData.SizeInPoints = 22.50746F;
            this.uceCodEntita.Appearance = appearance1;
            this.uceCodEntita.AutoSize = false;
            this.ucEasyTableLayoutPanel.SetColumnSpan(this.uceCodEntita, 3);
            this.uceCodEntita.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2007;
            this.uceCodEntita.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uceCodEntita.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.uceCodEntita.Location = new System.Drawing.Point(3, 369);
            this.uceCodEntita.Name = "uceCodEntita";
            this.uceCodEntita.Size = new System.Drawing.Size(388, 58);
            this.uceCodEntita.TabIndex = 16;
            this.uceCodEntita.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.uceCodEntita.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
                                                appearance2.FontData.SizeInPoints = 22.50746F;
            appearance2.TextVAlignAsString = "Middle";
            this.ucEasyLabel.Appearance = appearance2;
            this.ucEasyTableLayoutPanel.SetColumnSpan(this.ucEasyLabel, 3);
            this.ucEasyLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyLabel.Location = new System.Drawing.Point(3, 3);
            this.ucEasyLabel.Name = "ucEasyLabel";
            this.ucEasyLabel.ShortcutColor = System.Drawing.Color.Black;
            this.ucEasyLabel.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ucEasyLabel.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ucEasyLabel.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ucEasyLabel.Size = new System.Drawing.Size(388, 58);
            this.ucEasyLabel.TabIndex = 4;
            this.ucEasyLabel.Text = "Elenco Folder";
            this.ucEasyLabel.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
                                                appearance3.FontData.SizeInPoints = 22.50746F;
            appearance3.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance3.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance3.TextHAlignAsString = "Center";
            appearance3.TextVAlignAsString = "Middle";
            this.ucEasyButtonConferma.Appearance = appearance3;
            this.ucEasyButtonConferma.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ucEasyButtonConferma.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyButtonConferma.Location = new System.Drawing.Point(238, 433);
            this.ucEasyButtonConferma.Name = "ucEasyButtonConferma";
            this.ucEasyButtonConferma.PercImageFill = 0.75F;
            this.ucEasyButtonConferma.ShortcutColor = System.Drawing.Color.Black;
            this.ucEasyButtonConferma.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.XSmall;
            this.ucEasyButtonConferma.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ucEasyButtonConferma.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ucEasyButtonConferma.Size = new System.Drawing.Size(153, 58);
            this.ucEasyButtonConferma.TabIndex = 2;
            this.ucEasyButtonConferma.Text = "Conferma";
            this.ucEasyButtonConferma.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ucEasyButtonConferma.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
                                                appearance4.FontData.SizeInPoints = 22.50746F;
            appearance4.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance4.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance4.TextHAlignAsString = "Center";
            appearance4.TextVAlignAsString = "Middle";
            this.ucEasyButtonCancel.Appearance = appearance4;
            this.ucEasyButtonCancel.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ucEasyButtonCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyButtonCancel.Location = new System.Drawing.Point(3, 433);
            this.ucEasyButtonCancel.Name = "ucEasyButtonCancel";
            this.ucEasyButtonCancel.PercImageFill = 0.75F;
            this.ucEasyButtonCancel.ShortcutColor = System.Drawing.Color.Black;
            this.ucEasyButtonCancel.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.XSmall;
            this.ucEasyButtonCancel.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ucEasyButtonCancel.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ucEasyButtonCancel.Size = new System.Drawing.Size(151, 58);
            this.ucEasyButtonCancel.TabIndex = 1;
            this.ucEasyButtonCancel.Text = "Annulla";
            this.ucEasyButtonCancel.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ucEasyButtonCancel.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
                                                appearance5.FontData.SizeInPoints = 22.50746F;
            this.tvFolder.Appearance = appearance5;
            this.tvFolder.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.ucEasyTableLayoutPanel.SetColumnSpan(this.tvFolder, 3);
            this.tvFolder.DisplayStyle = Infragistics.Win.UltraWinTree.UltraTreeDisplayStyle.WindowsVista;
            this.tvFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFolder.HideSelection = false;
            this.tvFolder.LeftImagesSize = new System.Drawing.Size(23, 23);
            this.tvFolder.Location = new System.Drawing.Point(3, 67);
            this.tvFolder.Name = "tvFolder";
            appearance6.BackColor = System.Drawing.Color.LightYellow;
            appearance6.BackColor2 = System.Drawing.Color.Orange;
            _override1.ActiveNodeAppearance = appearance6;
            _override1.SelectedNodeAppearance = appearance6;
            this.tvFolder.Override = _override1;
            this.tvFolder.RightImagesSize = new System.Drawing.Size(23, 23);
            this.tvFolder.Size = new System.Drawing.Size(388, 232);
            this.tvFolder.TabIndex = 5;
            this.tvFolder.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.tvFolder.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.tvFolder.AfterActivate += new Infragistics.Win.UltraWinTree.AfterNodeChangedEventHandler(this.tvFolder_AfterActivate);
                                                appearance7.FontData.SizeInPoints = 22.50746F;
            this.txtFolder.Appearance = appearance7;
            this.txtFolder.AutoSize = false;
            this.ucEasyTableLayoutPanel.SetColumnSpan(this.txtFolder, 3);
            this.txtFolder.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2007;
            this.txtFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFolder.Location = new System.Drawing.Point(3, 305);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(388, 58);
            this.txtFolder.TabIndex = 6;
            this.txtFolder.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.txtFolder.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox);
            this.Name = "ucEasyPopUpFolder";
            this.Size = new System.Drawing.Size(400, 500);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).EndInit();
            this.ultraGroupBox.ResumeLayout(false);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uceCodEntita)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvFolder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFolder)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox;
        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        private ucEasyLabel ucEasyLabel;
        public ucEasyTreeView tvFolder;
        public ucEasyButton ucEasyButtonConferma;
        public ucEasyButton ucEasyButtonCancel;
        public ucEasyTextBox txtFolder;
        public ucEasyComboEditor uceCodEntita;
    }
}
