namespace UnicodeSrl.ScciManagement
{
    partial class frmPUDizionarioWiz
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPUDizionarioWiz));
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.uteVoci = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.lblHelpQuick = new System.Windows.Forms.Label();
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.picView = new System.Windows.Forms.PictureBox();
            this.pnlCopiaScheda = new System.Windows.Forms.Panel();
            this.utcWiz = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.uteDescrizione = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.uteCodice = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.lblDescrizione = new System.Windows.Forms.Label();
            this.lblCodice = new System.Windows.Forms.Label();
            this.ubConferma = new Infragistics.Win.Misc.UltraButton();
            this.ubAnnulla = new Infragistics.Win.Misc.UltraButton();
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraToolbarsManagerForm = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uteVoci)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).BeginInit();
            this.ultraGroupBox.SuspendLayout();
            this.TableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picView)).BeginInit();
            this.pnlCopiaScheda.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utcWiz)).BeginInit();
            this.utcWiz.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uteDescrizione)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteCodice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManagerForm)).BeginInit();
            this.SuspendLayout();
                                                this.ultraTabPageControl1.Controls.Add(this.uteVoci);
            this.ultraTabPageControl1.Controls.Add(this.lblHelpQuick);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 22);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(455, 267);
                                                this.uteVoci.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uteVoci.Location = new System.Drawing.Point(3, 90);
            this.uteVoci.Multiline = true;
            this.uteVoci.Name = "uteVoci";
            this.uteVoci.Scrollbars = System.Windows.Forms.ScrollBars.Both;
            this.uteVoci.Size = new System.Drawing.Size(449, 174);
            this.uteVoci.TabIndex = 1;
            this.uteVoci.WordWrap = false;
                                                this.lblHelpQuick.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHelpQuick.BackColor = System.Drawing.Color.Transparent;
            this.lblHelpQuick.Location = new System.Drawing.Point(3, 3);
            this.lblHelpQuick.Name = "lblHelpQuick";
            this.lblHelpQuick.Size = new System.Drawing.Size(449, 84);
            this.lblHelpQuick.TabIndex = 14;
            this.lblHelpQuick.Text = resources.GetString("lblHelpQuick.Text");
                                                this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(455, 267);
                                                this.ultraGroupBox.Controls.Add(this.TableLayoutPanel);
            this.ultraGroupBox.Controls.Add(this.ubConferma);
            this.ultraGroupBox.Controls.Add(this.ubAnnulla);
            this.ultraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox.Location = new System.Drawing.Point(8, 32);
            this.ultraGroupBox.Name = "ultraGroupBox";
            this.ultraGroupBox.Size = new System.Drawing.Size(644, 440);
            this.ultraGroupBox.TabIndex = 0;
            this.ultraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.TableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.TableLayoutPanel.ColumnCount = 2;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.Controls.Add(this.picView, 0, 1);
            this.TableLayoutPanel.Controls.Add(this.pnlCopiaScheda, 1, 0);
            this.TableLayoutPanel.Location = new System.Drawing.Point(6, 12);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 3;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 128F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel.Size = new System.Drawing.Size(630, 393);
            this.TableLayoutPanel.TabIndex = 6;
                                                this.picView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picView.Location = new System.Drawing.Point(3, 135);
            this.picView.Name = "picView";
            this.picView.Size = new System.Drawing.Size(134, 122);
            this.picView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picView.TabIndex = 1;
            this.picView.TabStop = false;
                                                this.pnlCopiaScheda.Controls.Add(this.utcWiz);
            this.pnlCopiaScheda.Controls.Add(this.uteDescrizione);
            this.pnlCopiaScheda.Controls.Add(this.uteCodice);
            this.pnlCopiaScheda.Controls.Add(this.lblDescrizione);
            this.pnlCopiaScheda.Controls.Add(this.lblCodice);
            this.pnlCopiaScheda.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCopiaScheda.Location = new System.Drawing.Point(143, 3);
            this.pnlCopiaScheda.Name = "pnlCopiaScheda";
            this.TableLayoutPanel.SetRowSpan(this.pnlCopiaScheda, 3);
            this.pnlCopiaScheda.Size = new System.Drawing.Size(484, 387);
            this.pnlCopiaScheda.TabIndex = 0;
                                                this.utcWiz.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.utcWiz.Controls.Add(this.ultraTabSharedControlsPage1);
            this.utcWiz.Controls.Add(this.ultraTabPageControl1);
            this.utcWiz.Controls.Add(this.ultraTabPageControl2);
            this.utcWiz.Location = new System.Drawing.Point(14, 82);
            this.utcWiz.Name = "utcWiz";
            this.utcWiz.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.utcWiz.Size = new System.Drawing.Size(457, 290);
            this.utcWiz.TabIndex = 2;
            ultraTab1.Key = "tab0";
            ultraTab1.TabPage = this.ultraTabPageControl1;
            ultraTab1.Text = "Elenco Voci";
            ultraTab2.Key = "tab1";
            ultraTab2.TabPage = this.ultraTabPageControl2;
            ultraTab2.Text = "File CSV";
            this.utcWiz.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2});
            this.utcWiz.ViewStyle = Infragistics.Win.UltraWinTabControl.ViewStyle.Office2007;
                                                this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(455, 267);
                                                this.uteDescrizione.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uteDescrizione.Location = new System.Drawing.Point(87, 42);
            this.uteDescrizione.Name = "uteDescrizione";
            this.uteDescrizione.Size = new System.Drawing.Size(384, 22);
            this.uteDescrizione.TabIndex = 1;
                                                this.uteCodice.Location = new System.Drawing.Point(87, 14);
            this.uteCodice.Name = "uteCodice";
            this.uteCodice.Size = new System.Drawing.Size(101, 22);
            this.uteCodice.TabIndex = 0;
                                                this.lblDescrizione.BackColor = System.Drawing.Color.Transparent;
            this.lblDescrizione.Location = new System.Drawing.Point(11, 46);
            this.lblDescrizione.Name = "lblDescrizione";
            this.lblDescrizione.Size = new System.Drawing.Size(70, 20);
            this.lblDescrizione.TabIndex = 14;
            this.lblDescrizione.Text = "Descrizione";
                                                this.lblCodice.BackColor = System.Drawing.Color.Transparent;
            this.lblCodice.Location = new System.Drawing.Point(11, 18);
            this.lblCodice.Name = "lblCodice";
            this.lblCodice.Size = new System.Drawing.Size(70, 20);
            this.lblCodice.TabIndex = 13;
            this.lblCodice.Text = "Codice";
                                                this.ubConferma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ubConferma.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.ubConferma.Location = new System.Drawing.Point(563, 411);
            this.ubConferma.Name = "ubConferma";
            this.ubConferma.Size = new System.Drawing.Size(75, 23);
            this.ubConferma.TabIndex = 4;
            this.ubConferma.Text = "&Conferma";
            this.ubConferma.Click += new System.EventHandler(this.ubConferma_Click);
                                                this.ubAnnulla.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ubAnnulla.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.ubAnnulla.Location = new System.Drawing.Point(6, 411);
            this.ubAnnulla.Name = "ubAnnulla";
            this.ubAnnulla.Size = new System.Drawing.Size(75, 23);
            this.ubAnnulla.TabIndex = 5;
            this.ubAnnulla.Text = "&Annulla";
            this.ubAnnulla.Click += new System.EventHandler(this.ubAnnulla_Click);
                                                this._frmPUDizionarioWiz_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 32);
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Left.Name = "_frmPUDizionarioWiz_Toolbars_Dock_Area_Left";
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(8, 440);
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Left.ToolbarsManager = this.ultraToolbarsManagerForm;
                                                this.ultraToolbarsManagerForm.DesignerFlags = 0;
            this.ultraToolbarsManagerForm.DockWithinContainer = this;
            this.ultraToolbarsManagerForm.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
            this.ultraToolbarsManagerForm.FormDisplayStyle = Infragistics.Win.UltraWinToolbars.FormDisplayStyle.RoundedSizable;
            this.ultraToolbarsManagerForm.RuntimeCustomizationOptions = Infragistics.Win.UltraWinToolbars.RuntimeCustomizationOptions.None;
            this.ultraToolbarsManagerForm.Style = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2007;
                                                this._frmPUDizionarioWiz_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(652, 32);
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Right.Name = "_frmPUDizionarioWiz_Toolbars_Dock_Area_Right";
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(8, 440);
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Right.ToolbarsManager = this.ultraToolbarsManagerForm;
                                                this._frmPUDizionarioWiz_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Top.Name = "_frmPUDizionarioWiz_Toolbars_Dock_Area_Top";
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(660, 32);
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Top.ToolbarsManager = this.ultraToolbarsManagerForm;
                                                this._frmPUDizionarioWiz_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 472);
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Bottom.Name = "_frmPUDizionarioWiz_Toolbars_Dock_Area_Bottom";
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(660, 8);
            this._frmPUDizionarioWiz_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.ultraToolbarsManagerForm;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 480);
            this.Controls.Add(this.ultraGroupBox);
            this.Controls.Add(this._frmPUDizionarioWiz_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._frmPUDizionarioWiz_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._frmPUDizionarioWiz_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this._frmPUDizionarioWiz_Toolbars_Dock_Area_Top);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmPUDizionarioWiz";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmPUDizionarioWiz";
            this.Shown += new System.EventHandler(this.frmPUDizionarioWiz_Shown);
            this.ultraTabPageControl1.ResumeLayout(false);
            this.ultraTabPageControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uteVoci)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).EndInit();
            this.ultraGroupBox.ResumeLayout(false);
            this.TableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picView)).EndInit();
            this.pnlCopiaScheda.ResumeLayout(false);
            this.pnlCopiaScheda.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utcWiz)).EndInit();
            this.utcWiz.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uteDescrizione)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteCodice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManagerForm)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox;
        private Infragistics.Win.Misc.UltraButton ubConferma;
        private Infragistics.Win.Misc.UltraButton ubAnnulla;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager ultraToolbarsManagerForm;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmPUDizionarioWiz_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmPUDizionarioWiz_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmPUDizionarioWiz_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmPUDizionarioWiz_Toolbars_Dock_Area_Top;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        internal System.Windows.Forms.PictureBox picView;
        private System.Windows.Forms.Panel pnlCopiaScheda;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor uteDescrizione;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor uteCodice;
        private System.Windows.Forms.Label lblDescrizione;
        private System.Windows.Forms.Label lblCodice;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl utcWiz;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor uteVoci;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        private System.Windows.Forms.Label lblHelpQuick;
    }
}