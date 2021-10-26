namespace UnicodeSrl.ScciCore
{
    partial class ucParametriVitaliTrasversali
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
            this.ultraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.ucMultiSelectUA = new UnicodeSrl.ScciCore.ucMultiSelect();
            this.ucMultiSelectPV = new UnicodeSrl.ScciCore.ucMultiSelect();
            this.lblSelUA = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.lblSelPV = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).BeginInit();
            this.ultraGroupBox.SuspendLayout();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
                                                this.ultraGroupBox.Controls.Add(this.ucEasyTableLayoutPanel);
            this.ultraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox.Name = "ultraGroupBox";
            this.ultraGroupBox.Size = new System.Drawing.Size(662, 578);
            this.ultraGroupBox.TabIndex = 1;
            this.ultraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.ucEasyTableLayoutPanel.ColumnCount = 3;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 98F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucMultiSelectUA, 1, 2);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucMultiSelectPV, 1, 5);
            this.ucEasyTableLayoutPanel.Controls.Add(this.lblSelUA, 1, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.lblSelPV, 1, 4);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 7;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 43F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 43F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(656, 572);
            this.ucEasyTableLayoutPanel.TabIndex = 0;
                                                this.ucMultiSelectUA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucMultiSelectUA.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ucMultiSelectUA.GridDXCaption = "";
            this.ucMultiSelectUA.GridDXCaptionColumnKey = "";
            this.ucMultiSelectUA.GridSXCaption = "";
            this.ucMultiSelectUA.GridSXCaptionColumnKey = "";
            this.ucMultiSelectUA.Location = new System.Drawing.Point(9, 36);
            this.ucMultiSelectUA.Name = "ucMultiSelectUA";
            this.ucMultiSelectUA.Size = new System.Drawing.Size(636, 239);
            this.ucMultiSelectUA.TabIndex = 0;
            this.ucMultiSelectUA.ViewDataSetDX = null;
            this.ucMultiSelectUA.ViewDataSetSX = null;
            this.ucMultiSelectUA.ViewShowAll = true;
            this.ucMultiSelectUA.ViewShowFind = true;
            this.ucMultiSelectUA.GridSXInitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ucMultiSelect_GridDXInitializeLayout);
            this.ucMultiSelectUA.GridDXInitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ucMultiSelect_GridDXInitializeLayout);
            this.ucMultiSelectUA.GridChange += new UnicodeSrl.ScciCore.ChangeEventHandler(this.ucMultiSelectUA_GridChange);
                                                this.ucMultiSelectPV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucMultiSelectPV.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ucMultiSelectPV.GridDXCaption = "";
            this.ucMultiSelectPV.GridDXCaptionColumnKey = "";
            this.ucMultiSelectPV.GridSXCaption = "";
            this.ucMultiSelectPV.GridSXCaptionColumnKey = "";
            this.ucMultiSelectPV.Location = new System.Drawing.Point(9, 320);
            this.ucMultiSelectPV.Name = "ucMultiSelectPV";
            this.ucMultiSelectPV.Size = new System.Drawing.Size(636, 239);
            this.ucMultiSelectPV.TabIndex = 1;
            this.ucMultiSelectPV.ViewDataSetDX = null;
            this.ucMultiSelectPV.ViewDataSetSX = null;
            this.ucMultiSelectPV.ViewShowAll = true;
            this.ucMultiSelectPV.ViewShowFind = true;
            this.ucMultiSelectPV.GridSXInitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ucMultiSelect_GridDXInitializeLayout);
            this.ucMultiSelectPV.GridDXInitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.ucMultiSelect_GridDXInitializeLayout);
                                                appearance1.FontData.SizeInPoints = 20.43361F;
            this.lblSelUA.Appearance = appearance1;
            this.lblSelUA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSelUA.Location = new System.Drawing.Point(9, 8);
            this.lblSelUA.Name = "lblSelUA";
            this.lblSelUA.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.lblSelUA.ShortcutKey = System.Windows.Forms.Keys.None;
            this.lblSelUA.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.lblSelUA.Size = new System.Drawing.Size(636, 22);
            this.lblSelUA.TabIndex = 2;
            this.lblSelUA.Text = "Seleziona Unità:";
            this.lblSelUA.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
                                                appearance2.FontData.SizeInPoints = 20.43361F;
            this.lblSelPV.Appearance = appearance2;
            this.lblSelPV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSelPV.Location = new System.Drawing.Point(9, 292);
            this.lblSelPV.Name = "lblSelPV";
            this.lblSelPV.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.lblSelPV.ShortcutKey = System.Windows.Forms.Keys.None;
            this.lblSelPV.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.lblSelPV.Size = new System.Drawing.Size(636, 22);
            this.lblSelPV.TabIndex = 3;
            this.lblSelPV.Text = "Seleziona Parametri Vitali da rilevare:";
            this.lblSelPV.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
                                                this.ultraToolTipManager1.ContainingControl = this;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.ultraGroupBox);
            this.Name = "ucParametriVitaliTrasversali";
            this.Size = new System.Drawing.Size(662, 578);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).EndInit();
            this.ultraGroupBox.ResumeLayout(false);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        private ucMultiSelect ucMultiSelectUA;
        private ucMultiSelect ucMultiSelectPV;
        private ucEasyLabel lblSelUA;
        private ucEasyLabel lblSelPV;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
    }
}
