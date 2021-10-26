namespace UnicodeSrl.ScciCore
{
    partial class ucListboxSearch
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
            this.tlp = new System.Windows.Forms.TableLayoutPanel();
            this.ListBox = new UnicodeSrl.ScciCore.ucEasyListBox();
            this.txtSearchListBox = new UnicodeSrl.ScciCore.ucEasyTextBox();
            this.ultraToolTipManager = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.tlp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ListBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchListBox)).BeginInit();
            this.SuspendLayout();
                                                this.tlp.ColumnCount = 1;
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp.Controls.Add(this.ListBox, 0, 1);
            this.tlp.Controls.Add(this.txtSearchListBox, 0, 0);
            this.tlp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp.Location = new System.Drawing.Point(0, 0);
            this.tlp.Name = "tlp";
            this.tlp.RowCount = 2;
            this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp.Size = new System.Drawing.Size(365, 358);
            this.tlp.TabIndex = 11;
                                                appearance1.FontData.SizeInPoints = 20.43361F;
            this.ListBox.Appearance = appearance1;
            this.ListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListBox.ItemSettings.SelectionType = Infragistics.Win.UltraWinListView.SelectionType.Single;
            this.ListBox.Location = new System.Drawing.Point(3, 45);
            this.ListBox.Name = "ListBox";
            this.ListBox.Size = new System.Drawing.Size(359, 310);
            this.ListBox.TabIndex = 12;
            this.ListBox.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ListBox.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ListBox.View = Infragistics.Win.UltraWinListView.UltraListViewStyle.List;
            this.ListBox.ViewSettingsDetails.AllowColumnMoving = false;
            this.ListBox.ViewSettingsDetails.AllowColumnSizing = false;
            this.ListBox.ViewSettingsDetails.AllowColumnSorting = false;
            this.ListBox.ViewSettingsDetails.AutoFitColumns = Infragistics.Win.UltraWinListView.AutoFitColumns.ResizeAllColumns;
            this.ListBox.ViewSettingsDetails.ColumnAutoSizeMode = ((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode)((Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.Header | Infragistics.Win.UltraWinListView.ColumnAutoSizeMode.AllItems)));
            this.ListBox.ViewSettingsDetails.FullRowSelect = true;
            this.ListBox.ViewSettingsDetails.ImageSize = new System.Drawing.Size(0, 0);
            this.ListBox.ViewSettingsList.ImageSize = new System.Drawing.Size(0, 0);
            this.ListBox.ViewSettingsList.MultiColumn = false;
            this.ListBox.ToolTipDisplaying += new Infragistics.Win.UltraWinListView.ToolTipDisplayingEventHandler(this.ListBox_ToolTipDisplaying);
            this.ListBox.MouseEnterElement += new Infragistics.Win.UIElementEventHandler(this.ListBox_MouseEnterElement);
            this.ListBox.MouseLeaveElement += new Infragistics.Win.UIElementEventHandler(this.ListBox_MouseLeaveElement);
            this.ListBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ListBox_MouseClick);
                                                appearance2.BackColor = System.Drawing.Color.Yellow;
            appearance2.FontData.SizeInPoints = 20.43361F;
            appearance2.TextHAlignAsString = "Left";
            appearance2.TextVAlignAsString = "Top";
            this.txtSearchListBox.Appearance = appearance2;
            this.txtSearchListBox.AutoSize = false;
            this.txtSearchListBox.BackColor = System.Drawing.Color.Yellow;
            this.txtSearchListBox.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2007;
            this.txtSearchListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearchListBox.Location = new System.Drawing.Point(3, 3);
            this.txtSearchListBox.Multiline = true;
            this.txtSearchListBox.Name = "txtSearchListBox";
            this.txtSearchListBox.Size = new System.Drawing.Size(359, 36);
            this.txtSearchListBox.TabIndex = 11;
            this.txtSearchListBox.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.txtSearchListBox.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.txtSearchListBox.ValueChanged += new System.EventHandler(this.txtSearchMultiBox_ValueChanged);
                                                this.ultraToolTipManager.ContainingControl = this;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlp);
            this.Name = "ucListboxSearch";
            this.Size = new System.Drawing.Size(365, 358);
            this.VisibleChanged += new System.EventHandler(this.ucListboxSearch_VisibleChanged);
            this.tlp.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ListBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchListBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlp;
        private ucEasyListBox ListBox;
        private ucEasyTextBox txtSearchListBox;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager;

    }
}
