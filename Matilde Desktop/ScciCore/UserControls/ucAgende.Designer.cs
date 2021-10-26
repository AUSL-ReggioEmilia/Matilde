namespace UnicodeSrl.ScciCore
{
    partial class ucAgende
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
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinScrollBar.ScrollBarLook scrollBarLook1 = new Infragistics.Win.UltraWinScrollBar.ScrollBarLook();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.UltraGridAgende = new UnicodeSrl.ScciCore.ucEasyGrid();
            this.ubSeleziona = new UnicodeSrl.ScciCore.ucEasyButton();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UltraGridAgende)).BeginInit();
            this.SuspendLayout();
                                                this.ucEasyTableLayoutPanel.ColumnCount = 1;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.UltraGridAgende, 0, 0);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ubSeleziona, 0, 1);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 2;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(340, 340);
            this.ucEasyTableLayoutPanel.TabIndex = 0;
                                                this.UltraGridAgende.ColonnaRTFResize = "";
            this.UltraGridAgende.DataRowFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.UltraGridAgende.DisplayLayout.Appearance = appearance1;
            this.UltraGridAgende.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            this.UltraGridAgende.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance2.FontData.BoldAsString = "True";
            appearance2.FontData.SizeInPoints = 16F;
            this.UltraGridAgende.DisplayLayout.CaptionAppearance = appearance2;
            this.UltraGridAgende.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance3.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance3.BorderColor = System.Drawing.SystemColors.Window;
            this.UltraGridAgende.DisplayLayout.GroupByBox.Appearance = appearance3;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.UltraGridAgende.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.UltraGridAgende.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.UltraGridAgende.DisplayLayout.GroupByBox.Hidden = true;
            this.UltraGridAgende.DisplayLayout.GroupByBox.Prompt = "Trascina un\'intestazione della colonna qui per raggrupparla.";
            appearance5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance5.BackColor2 = System.Drawing.SystemColors.Control;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.UltraGridAgende.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
            this.UltraGridAgende.DisplayLayout.MaxColScrollRegions = 1;
            this.UltraGridAgende.DisplayLayout.MaxRowScrollRegions = 1;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            appearance6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.UltraGridAgende.DisplayLayout.Override.ActiveCellAppearance = appearance6;
            appearance7.BackColor = System.Drawing.SystemColors.Highlight;
            appearance7.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.UltraGridAgende.DisplayLayout.Override.ActiveRowAppearance = appearance7;
            this.UltraGridAgende.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.UltraGridAgende.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this.UltraGridAgende.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.UltraGridAgende.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.UltraGridAgende.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.UltraGridAgende.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.UltraGridAgende.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance8.BackColor = System.Drawing.SystemColors.Window;
            this.UltraGridAgende.DisplayLayout.Override.CardAreaAppearance = appearance8;
            appearance9.BorderColor = System.Drawing.Color.Silver;
            appearance9.FontData.SizeInPoints = 16F;
            appearance9.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.UltraGridAgende.DisplayLayout.Override.CellAppearance = appearance9;
            this.UltraGridAgende.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.UltraGridAgende.DisplayLayout.Override.CellPadding = 0;
            this.UltraGridAgende.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.SiblingRowsOnly;
            appearance10.FontData.SizeInPoints = 16F;
            this.UltraGridAgende.DisplayLayout.Override.FilterRowAppearance = appearance10;
            appearance11.BackColor = System.Drawing.SystemColors.Control;
            appearance11.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance11.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance11.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance11.BorderColor = System.Drawing.SystemColors.Window;
            this.UltraGridAgende.DisplayLayout.Override.GroupByRowAppearance = appearance11;
            appearance12.FontData.BoldAsString = "True";
            appearance12.FontData.SizeInPoints = 16F;
            appearance12.TextHAlignAsString = "Center";
            this.UltraGridAgende.DisplayLayout.Override.HeaderAppearance = appearance12;
            this.UltraGridAgende.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortSingle;
            this.UltraGridAgende.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsVista;
            appearance13.BackColor = System.Drawing.Color.WhiteSmoke;
            appearance13.ForeColor = System.Drawing.Color.DarkBlue;
            this.UltraGridAgende.DisplayLayout.Override.RowAlternateAppearance = appearance13;
            appearance14.BackColor = System.Drawing.SystemColors.Window;
            appearance14.BorderColor = System.Drawing.Color.Silver;
            this.UltraGridAgende.DisplayLayout.Override.RowAppearance = appearance14;
            this.UltraGridAgende.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.UltraGridAgende.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFree;
            this.UltraGridAgende.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.UltraGridAgende.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance15.BackColor = System.Drawing.SystemColors.ControlLight;
            this.UltraGridAgende.DisplayLayout.Override.TemplateAddRowAppearance = appearance15;
            scrollBarLook1.ViewStyle = Infragistics.Win.UltraWinScrollBar.ScrollBarViewStyle.Office2010;
            this.UltraGridAgende.DisplayLayout.ScrollBarLook = scrollBarLook1;
            this.UltraGridAgende.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.UltraGridAgende.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.UltraGridAgende.DisplayLayout.TabNavigation = Infragistics.Win.UltraWinGrid.TabNavigation.NextControl;
            this.UltraGridAgende.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.UltraGridAgende.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UltraGridAgende.FattoreRidimensionamentoRTF = 21;
            this.UltraGridAgende.FilterRowFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.UltraGridAgende.GridCaptionFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.UltraGridAgende.HeaderFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.UltraGridAgende.Location = new System.Drawing.Point(3, 3);
            this.UltraGridAgende.Name = "UltraGridAgende";
            this.UltraGridAgende.ShowFilterRow = false;
            this.UltraGridAgende.ShowGroupByBox = false;
            this.UltraGridAgende.Size = new System.Drawing.Size(334, 266);
            this.UltraGridAgende.TabIndex = 0;
            this.UltraGridAgende.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            this.UltraGridAgende.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.UltraGridAgende.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.UltraGridAgende_InitializeLayout);
            this.UltraGridAgende.AfterRowActivate += new System.EventHandler(this.UltraGridAgende_AfterRowActivate);
                                                appearance16.FontData.SizeInPoints = 20F;
            appearance16.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance16.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance16.TextHAlignAsString = "Center";
            this.ubSeleziona.Appearance = appearance16;
            this.ubSeleziona.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ubSeleziona.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ubSeleziona.Location = new System.Drawing.Point(3, 275);
            this.ubSeleziona.Name = "ubSeleziona";
            this.ubSeleziona.PercImageFill = 0.75F;
            this.ubSeleziona.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ubSeleziona.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ubSeleziona.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ubSeleziona.Size = new System.Drawing.Size(334, 62);
            this.ubSeleziona.TabIndex = 1;
            this.ubSeleziona.Text = "SELEZIONA >>";
            this.ubSeleziona.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Large;
            this.ubSeleziona.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ubSeleziona.Click += new System.EventHandler(this.ubSeleziona_Click);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.ucEasyTableLayoutPanel);
            this.Name = "ucAgende";
            this.Size = new System.Drawing.Size(340, 340);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UltraGridAgende)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        private ucEasyGrid UltraGridAgende;
        private ucEasyButton ubSeleziona;
    }
}
