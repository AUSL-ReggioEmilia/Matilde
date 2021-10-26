namespace UnicodeSrl.ScciManagement
{
    partial class ucMultiSelectPlus
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
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.ucMultiSelect = new UnicodeSrl.ScciCore.ucMultiSelect();
            this.UltraGridMaster = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UltraGridMaster)).BeginInit();
            this.SuspendLayout();
                                                this.ucEasyTableLayoutPanel.ColumnCount = 2;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucMultiSelect, 1, 0);
            this.ucEasyTableLayoutPanel.Controls.Add(this.UltraGridMaster, 0, 0);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 1;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(600, 450);
            this.ucEasyTableLayoutPanel.TabIndex = 0;
                                                this.ucMultiSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucMultiSelect.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ucMultiSelect.GridDXCaption = "";
            this.ucMultiSelect.GridDXCaptionColumnKey = "";
            this.ucMultiSelect.GridSXCaption = "";
            this.ucMultiSelect.GridSXCaptionColumnKey = "";
            this.ucMultiSelect.Location = new System.Drawing.Point(123, 3);
            this.ucMultiSelect.Name = "ucMultiSelect";
            this.ucMultiSelect.Size = new System.Drawing.Size(474, 444);
            this.ucMultiSelect.TabIndex = 0;
            this.ucMultiSelect.ViewDataSetDX = null;
            this.ucMultiSelect.ViewDataSetSX = null;
            this.ucMultiSelect.ViewShowAll = true;
            this.ucMultiSelect.ViewShowFind = true;
            this.ucMultiSelect.GridSXInitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ucMultiSelect_GridSXInitializeLayout);
            this.ucMultiSelect.GridSXInitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.ucMultiSelect_GridSXInitializeRow);
            this.ucMultiSelect.GridDXInitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ucMultiSelect_GridDXInitializeLayout);
            this.ucMultiSelect.GridDXInitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.ucMultiSelect_GridDXInitializeRow);
            this.ucMultiSelect.GridChange += new UnicodeSrl.ScciCore.ChangeEventHandler(this.ucMultiSelect_GridChange);
                                                appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.UltraGridMaster.DisplayLayout.Appearance = appearance1;
            this.UltraGridMaster.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.UltraGridMaster.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.UltraGridMaster.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.UltraGridMaster.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.UltraGridMaster.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.UltraGridMaster.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.UltraGridMaster.DisplayLayout.MaxColScrollRegions = 1;
            this.UltraGridMaster.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.UltraGridMaster.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.UltraGridMaster.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.UltraGridMaster.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.UltraGridMaster.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.UltraGridMaster.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.UltraGridMaster.DisplayLayout.Override.CellAppearance = appearance8;
            this.UltraGridMaster.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.UltraGridMaster.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.UltraGridMaster.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.UltraGridMaster.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.UltraGridMaster.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.UltraGridMaster.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.UltraGridMaster.DisplayLayout.Override.RowAppearance = appearance11;
            this.UltraGridMaster.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.UltraGridMaster.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.UltraGridMaster.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.UltraGridMaster.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.UltraGridMaster.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.UltraGridMaster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UltraGridMaster.Location = new System.Drawing.Point(3, 6);
            this.UltraGridMaster.Margin = new System.Windows.Forms.Padding(3, 6, 3, 8);
            this.UltraGridMaster.Name = "UltraGridMaster";
            this.UltraGridMaster.Size = new System.Drawing.Size(114, 436);
            this.UltraGridMaster.TabIndex = 1;
            this.UltraGridMaster.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ucEasyGridMaster_InitializeLayout);
            this.UltraGridMaster.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.ucEasyGridMaster_InitializeRow);
            this.UltraGridMaster.AfterRowActivate += new System.EventHandler(this.UltraGridMaster_AfterRowActivate);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ucEasyTableLayoutPanel);
            this.Name = "ucMultiSelectPlus";
            this.Size = new System.Drawing.Size(600, 450);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UltraGridMaster)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ScciCore.ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        public ScciCore.ucMultiSelect ucMultiSelect;
        private Infragistics.Win.UltraWinGrid.UltraGrid UltraGridMaster;
    }
}
