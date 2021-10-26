namespace UnicodeSrl.ScciManagement
{
    partial class frmZoomSchede
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
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            this.UltraGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.frmZoom_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.UltraToolbarsManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._frmZoom_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._frmZoom_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._frmZoom_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._frmZoom_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.UltraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.ubConferma = new Infragistics.Win.Misc.UltraButton();
            this.ubAnnulla = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.UltraGrid)).BeginInit();
            this.frmZoom_Fill_Panel.ClientArea.SuspendLayout();
            this.frmZoom_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UltraToolbarsManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBox)).BeginInit();
            this.UltraGroupBox.SuspendLayout();
            this.SuspendLayout();
                                                this.UltraGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.UltraGrid.DisplayLayout.Appearance = appearance1;
            this.UltraGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.UltraGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance2.BorderColor = System.Drawing.SystemColors.Window;
            this.UltraGrid.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.UltraGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.UltraGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.UltraGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.UltraGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.UltraGrid.DisplayLayout.MaxRowScrollRegions = 1;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            appearance5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.UltraGrid.DisplayLayout.Override.ActiveCellAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.UltraGrid.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.UltraGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.UltraGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            this.UltraGrid.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BorderColor = System.Drawing.Color.Silver;
            appearance8.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.UltraGrid.DisplayLayout.Override.CellAppearance = appearance8;
            this.UltraGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.UltraGrid.DisplayLayout.Override.CellPadding = 0;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance9.BorderColor = System.Drawing.SystemColors.Window;
            this.UltraGrid.DisplayLayout.Override.GroupByRowAppearance = appearance9;
            appearance10.TextHAlignAsString = "Left";
            this.UltraGrid.DisplayLayout.Override.HeaderAppearance = appearance10;
            this.UltraGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.UltraGrid.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.Color.Silver;
            this.UltraGrid.DisplayLayout.Override.RowAppearance = appearance11;
            this.UltraGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance12.BackColor = System.Drawing.SystemColors.ControlLight;
            this.UltraGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance12;
            this.UltraGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.UltraGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.UltraGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.UltraGrid.Location = new System.Drawing.Point(12, 12);
            this.UltraGrid.Name = "UltraGrid";
            this.UltraGrid.Size = new System.Drawing.Size(760, 508);
            this.UltraGrid.TabIndex = 0;
            this.UltraGrid.Text = "ultraGrid1";
            this.UltraGrid.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.UltraGrid_InitializeLayout);
                                                                                    this.frmZoom_Fill_Panel.ClientArea.Controls.Add(this.UltraGroupBox);
            this.frmZoom_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmZoom_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmZoom_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.frmZoom_Fill_Panel.Name = "frmZoom_Fill_Panel";
            this.frmZoom_Fill_Panel.Size = new System.Drawing.Size(784, 561);
            this.frmZoom_Fill_Panel.TabIndex = 5;
                                                this.UltraToolbarsManager.DesignerFlags = 1;
            this.UltraToolbarsManager.DockWithinContainer = this;
            this.UltraToolbarsManager.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
                                                this._frmZoom_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmZoom_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._frmZoom_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._frmZoom_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmZoom_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmZoom_Toolbars_Dock_Area_Top.Name = "_frmZoom_Toolbars_Dock_Area_Top";
            this._frmZoom_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(784, 0);
            this._frmZoom_Toolbars_Dock_Area_Top.ToolbarsManager = this.UltraToolbarsManager;
                                                this._frmZoom_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmZoom_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._frmZoom_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._frmZoom_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmZoom_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 561);
            this._frmZoom_Toolbars_Dock_Area_Bottom.Name = "_frmZoom_Toolbars_Dock_Area_Bottom";
            this._frmZoom_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(784, 0);
            this._frmZoom_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.UltraToolbarsManager;
                                                this._frmZoom_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmZoom_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._frmZoom_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._frmZoom_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmZoom_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._frmZoom_Toolbars_Dock_Area_Left.Name = "_frmZoom_Toolbars_Dock_Area_Left";
            this._frmZoom_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 561);
            this._frmZoom_Toolbars_Dock_Area_Left.ToolbarsManager = this.UltraToolbarsManager;
                                                this._frmZoom_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmZoom_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._frmZoom_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._frmZoom_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmZoom_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(784, 0);
            this._frmZoom_Toolbars_Dock_Area_Right.Name = "_frmZoom_Toolbars_Dock_Area_Right";
            this._frmZoom_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 561);
            this._frmZoom_Toolbars_Dock_Area_Right.ToolbarsManager = this.UltraToolbarsManager;
                                                this.UltraGroupBox.Controls.Add(this.ubConferma);
            this.UltraGroupBox.Controls.Add(this.ubAnnulla);
            this.UltraGroupBox.Controls.Add(this.UltraGrid);
            this.UltraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UltraGroupBox.Location = new System.Drawing.Point(0, 0);
            this.UltraGroupBox.Name = "UltraGroupBox";
            this.UltraGroupBox.Size = new System.Drawing.Size(784, 561);
            this.UltraGroupBox.TabIndex = 0;
            this.UltraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.ubConferma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ubConferma.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.ubConferma.Location = new System.Drawing.Point(697, 526);
            this.ubConferma.Name = "ubConferma";
            this.ubConferma.Size = new System.Drawing.Size(75, 23);
            this.ubConferma.TabIndex = 2;
            this.ubConferma.Text = "&Conferma";
            this.ubConferma.Click += new System.EventHandler(this.ubConferma_Click);
                                                this.ubAnnulla.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ubAnnulla.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.ubAnnulla.Location = new System.Drawing.Point(12, 526);
            this.ubAnnulla.Name = "ubAnnulla";
            this.ubAnnulla.Size = new System.Drawing.Size(75, 23);
            this.ubAnnulla.TabIndex = 1;
            this.ubAnnulla.Text = "&Annulla";
            this.ubAnnulla.Click += new System.EventHandler(this.ubAnnulla_Click);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.frmZoom_Fill_Panel);
            this.Controls.Add(this._frmZoom_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._frmZoom_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._frmZoom_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this._frmZoom_Toolbars_Dock_Area_Top);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimizeBox = false;
            this.Name = "frmZoomSchede";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmZoomSchede";
            ((System.ComponentModel.ISupportInitialize)(this.UltraGrid)).EndInit();
            this.frmZoom_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmZoom_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UltraToolbarsManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBox)).EndInit();
            this.UltraGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid UltraGrid;
        private Infragistics.Win.Misc.UltraPanel frmZoom_Fill_Panel;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager UltraToolbarsManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmZoom_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmZoom_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmZoom_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmZoom_Toolbars_Dock_Area_Right;
        private Infragistics.Win.Misc.UltraGroupBox UltraGroupBox;
        private Infragistics.Win.Misc.UltraButton ubConferma;
        private Infragistics.Win.Misc.UltraButton ubAnnulla;
    }
}