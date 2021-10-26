namespace UnicodeSrl.ScciManagement
{
    partial class frmConfigurazione
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
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("Zoom");
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.uteConnectionString = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.lblConnectionString = new System.Windows.Forms.Label();
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.PicView = new System.Windows.Forms.PictureBox();
            this.UltraToolbarsManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this.frmConfigurazione_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.UltraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.UltraTabControl = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ubConferma = new Infragistics.Win.Misc.UltraButton();
            this.ubAnnulla = new Infragistics.Win.Misc.UltraButton();
            this._frmConfigurazione_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._frmConfigurazione_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._frmConfigurazione_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._frmConfigurazione_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uteConnectionString)).BeginInit();
            this.TableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UltraToolbarsManager)).BeginInit();
            this.frmConfigurazione_Fill_Panel.ClientArea.SuspendLayout();
            this.frmConfigurazione_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBox)).BeginInit();
            this.UltraGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UltraTabControl)).BeginInit();
            this.UltraTabControl.SuspendLayout();
            this.ultraTabSharedControlsPage1.SuspendLayout();
            this.SuspendLayout();
                                                this.ultraTabPageControl1.Controls.Add(this.uteConnectionString);
            this.ultraTabPageControl1.Controls.Add(this.lblConnectionString);
            this.ultraTabPageControl1.Controls.Add(this.TableLayoutPanel);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 22);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(758, 486);
                                                this.uteConnectionString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            editorButton1.Key = "Zoom";
            editorButton1.Text = "...";
            this.uteConnectionString.ButtonsRight.Add(editorButton1);
            this.uteConnectionString.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2007;
            this.uteConnectionString.Location = new System.Drawing.Point(314, 8);
            this.uteConnectionString.Multiline = true;
            this.uteConnectionString.Name = "uteConnectionString";
            this.uteConnectionString.PasswordChar = '*';
            this.uteConnectionString.Size = new System.Drawing.Size(430, 99);
            this.uteConnectionString.TabIndex = 0;
            this.uteConnectionString.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.uteConnectionString_EditorButtonClick);
            this.uteConnectionString.KeyDown += new System.Windows.Forms.KeyEventHandler(this.uteConnectionString_KeyDown);
                                                this.lblConnectionString.Location = new System.Drawing.Point(140, 12);
            this.lblConnectionString.Name = "lblConnectionString";
            this.lblConnectionString.Size = new System.Drawing.Size(168, 20);
            this.lblConnectionString.TabIndex = 1;
            this.lblConnectionString.Text = "Stringa di connessione";
                                                this.TableLayoutPanel.ColumnCount = 1;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanel.Controls.Add(this.PicView, 0, 1);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.TableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 3;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 128F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel.Size = new System.Drawing.Size(134, 486);
            this.TableLayoutPanel.TabIndex = 0;
                                                this.PicView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PicView.Location = new System.Drawing.Point(3, 182);
            this.PicView.Name = "PicView";
            this.PicView.Size = new System.Drawing.Size(128, 122);
            this.PicView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PicView.TabIndex = 0;
            this.PicView.TabStop = false;
                                                this.UltraToolbarsManager.DesignerFlags = 1;
            this.UltraToolbarsManager.DockWithinContainer = this;
            this.UltraToolbarsManager.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
                                                                                    this.frmConfigurazione_Fill_Panel.ClientArea.Controls.Add(this.UltraGroupBox);
            this.frmConfigurazione_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmConfigurazione_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmConfigurazione_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.frmConfigurazione_Fill_Panel.Name = "frmConfigurazione_Fill_Panel";
            this.frmConfigurazione_Fill_Panel.Size = new System.Drawing.Size(784, 562);
            this.frmConfigurazione_Fill_Panel.TabIndex = 0;
                                                this.UltraGroupBox.Controls.Add(this.UltraTabControl);
            this.UltraGroupBox.Controls.Add(this.ubConferma);
            this.UltraGroupBox.Controls.Add(this.ubAnnulla);
            this.UltraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UltraGroupBox.Location = new System.Drawing.Point(0, 0);
            this.UltraGroupBox.Name = "UltraGroupBox";
            this.UltraGroupBox.Size = new System.Drawing.Size(784, 562);
            this.UltraGroupBox.TabIndex = 0;
            this.UltraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.UltraTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UltraTabControl.Controls.Add(this.ultraTabSharedControlsPage1);
            this.UltraTabControl.Controls.Add(this.ultraTabPageControl1);
            this.UltraTabControl.Location = new System.Drawing.Point(12, 12);
            this.UltraTabControl.Name = "UltraTabControl";
            this.UltraTabControl.SharedControls.AddRange(new System.Windows.Forms.Control[] {
            this.TableLayoutPanel});
            this.UltraTabControl.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.UltraTabControl.Size = new System.Drawing.Size(760, 509);
            this.UltraTabControl.TabIndex = 0;
            ultraTab1.Key = "tab1";
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "tab1";
            this.UltraTabControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1});
            this.UltraTabControl.TabStop = false;
            this.UltraTabControl.ViewStyle = Infragistics.Win.UltraWinTabControl.ViewStyle.Office2007;
                                                this.ultraTabSharedControlsPage1.Controls.Add(this.TableLayoutPanel);
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(758, 486);
                                                this.ubConferma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ubConferma.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.ubConferma.Location = new System.Drawing.Point(697, 527);
            this.ubConferma.Name = "ubConferma";
            this.ubConferma.Size = new System.Drawing.Size(75, 23);
            this.ubConferma.TabIndex = 2;
            this.ubConferma.Text = "&Conferma";
            this.ubConferma.Click += new System.EventHandler(this.ubConferma_Click);
                                                this.ubAnnulla.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ubAnnulla.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.ubAnnulla.Location = new System.Drawing.Point(12, 527);
            this.ubAnnulla.Name = "ubAnnulla";
            this.ubAnnulla.Size = new System.Drawing.Size(75, 23);
            this.ubAnnulla.TabIndex = 1;
            this.ubAnnulla.Text = "&Annulla";
            this.ubAnnulla.Click += new System.EventHandler(this.ubAnnulla_Click);
                                                this._frmConfigurazione_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmConfigurazione_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._frmConfigurazione_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._frmConfigurazione_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmConfigurazione_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._frmConfigurazione_Toolbars_Dock_Area_Left.Name = "_frmConfigurazione_Toolbars_Dock_Area_Left";
            this._frmConfigurazione_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 562);
            this._frmConfigurazione_Toolbars_Dock_Area_Left.ToolbarsManager = this.UltraToolbarsManager;
                                                this._frmConfigurazione_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmConfigurazione_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._frmConfigurazione_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._frmConfigurazione_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmConfigurazione_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(784, 0);
            this._frmConfigurazione_Toolbars_Dock_Area_Right.Name = "_frmConfigurazione_Toolbars_Dock_Area_Right";
            this._frmConfigurazione_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 562);
            this._frmConfigurazione_Toolbars_Dock_Area_Right.ToolbarsManager = this.UltraToolbarsManager;
                                                this._frmConfigurazione_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmConfigurazione_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._frmConfigurazione_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._frmConfigurazione_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmConfigurazione_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmConfigurazione_Toolbars_Dock_Area_Top.Name = "_frmConfigurazione_Toolbars_Dock_Area_Top";
            this._frmConfigurazione_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(784, 0);
            this._frmConfigurazione_Toolbars_Dock_Area_Top.ToolbarsManager = this.UltraToolbarsManager;
                                                this._frmConfigurazione_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmConfigurazione_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._frmConfigurazione_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._frmConfigurazione_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmConfigurazione_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 562);
            this._frmConfigurazione_Toolbars_Dock_Area_Bottom.Name = "_frmConfigurazione_Toolbars_Dock_Area_Bottom";
            this._frmConfigurazione_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(784, 0);
            this._frmConfigurazione_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.UltraToolbarsManager;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.frmConfigurazione_Fill_Panel);
            this.Controls.Add(this._frmConfigurazione_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._frmConfigurazione_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._frmConfigurazione_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this._frmConfigurazione_Toolbars_Dock_Area_Top);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfigurazione";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmConfigurazione";
            this.ultraTabPageControl1.ResumeLayout(false);
            this.ultraTabPageControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uteConnectionString)).EndInit();
            this.TableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PicView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UltraToolbarsManager)).EndInit();
            this.frmConfigurazione_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmConfigurazione_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBox)).EndInit();
            this.UltraGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UltraTabControl)).EndInit();
            this.UltraTabControl.ResumeLayout(false);
            this.ultraTabSharedControlsPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager UltraToolbarsManager;
        private Infragistics.Win.Misc.UltraPanel frmConfigurazione_Fill_Panel;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmConfigurazione_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmConfigurazione_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmConfigurazione_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmConfigurazione_Toolbars_Dock_Area_Top;
        private Infragistics.Win.Misc.UltraButton ubConferma;
        private Infragistics.Win.Misc.UltraButton ubAnnulla;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl UltraTabControl;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.Misc.UltraGroupBox UltraGroupBox;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        private System.Windows.Forms.Label lblConnectionString;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor uteConnectionString;
        private System.Windows.Forms.PictureBox PicView;
    }
}