namespace UnicodeSrl.ScciManagement
{
    partial class frmPUScreenCopia
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
            this.ubAnnulla = new Infragistics.Win.Misc.UltraButton();
            this.uteDescrizione = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.uteCodice = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.lblDescrizione = new System.Windows.Forms.Label();
            this.lblCodice = new System.Windows.Forms.Label();
            this.ubConferma = new Infragistics.Win.Misc.UltraButton();
            this.chkCopiaScreenTile = new System.Windows.Forms.CheckBox();
            this.picView = new System.Windows.Forms.PictureBox();
            this.pnlCopiaScheda = new System.Windows.Forms.Panel();
            this.ugbCopiaRelazioni = new Infragistics.Win.Misc.UltraGroupBox();
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ultraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ultraToolbarsManagerForm = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this.chkCopiaRuoli = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.uteDescrizione)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteCodice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picView)).BeginInit();
            this.pnlCopiaScheda.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugbCopiaRelazioni)).BeginInit();
            this.ugbCopiaRelazioni.SuspendLayout();
            this.TableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).BeginInit();
            this.ultraGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManagerForm)).BeginInit();
            this.SuspendLayout();
                                                this.ubAnnulla.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ubAnnulla.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.ubAnnulla.Location = new System.Drawing.Point(6, 331);
            this.ubAnnulla.Name = "ubAnnulla";
            this.ubAnnulla.Size = new System.Drawing.Size(75, 23);
            this.ubAnnulla.TabIndex = 1;
            this.ubAnnulla.Text = "&Annulla";
            this.ubAnnulla.Click += new System.EventHandler(this.ubAnnulla_Click);
                                                this.uteDescrizione.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uteDescrizione.Location = new System.Drawing.Point(87, 42);
            this.uteDescrizione.Name = "uteDescrizione";
            this.uteDescrizione.Size = new System.Drawing.Size(524, 22);
            this.uteDescrizione.TabIndex = 1;
                                                this.uteCodice.Location = new System.Drawing.Point(87, 14);
            this.uteCodice.Name = "uteCodice";
            this.uteCodice.Size = new System.Drawing.Size(101, 22);
            this.uteCodice.TabIndex = 0;
                                                this.lblDescrizione.BackColor = System.Drawing.Color.Transparent;
            this.lblDescrizione.Location = new System.Drawing.Point(11, 46);
            this.lblDescrizione.Name = "lblDescrizione";
            this.lblDescrizione.Size = new System.Drawing.Size(87, 20);
            this.lblDescrizione.TabIndex = 14;
            this.lblDescrizione.Text = "Descrizione";
                                                this.lblCodice.BackColor = System.Drawing.Color.Transparent;
            this.lblCodice.Location = new System.Drawing.Point(11, 18);
            this.lblCodice.Name = "lblCodice";
            this.lblCodice.Size = new System.Drawing.Size(87, 20);
            this.lblCodice.TabIndex = 13;
            this.lblCodice.Text = "Codice";
                                                this.ubConferma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ubConferma.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.ubConferma.Location = new System.Drawing.Point(703, 331);
            this.ubConferma.Name = "ubConferma";
            this.ubConferma.Size = new System.Drawing.Size(75, 23);
            this.ubConferma.TabIndex = 2;
            this.ubConferma.Text = "&Conferma";
            this.ubConferma.Click += new System.EventHandler(this.ubConferma_Click);
                                                this.chkCopiaScreenTile.BackColor = System.Drawing.Color.Transparent;
            this.chkCopiaScreenTile.Location = new System.Drawing.Point(13, 33);
            this.chkCopiaScreenTile.Name = "chkCopiaScreenTile";
            this.chkCopiaScreenTile.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkCopiaScreenTile.Size = new System.Drawing.Size(203, 19);
            this.chkCopiaScreenTile.TabIndex = 0;
            this.chkCopiaScreenTile.Text = "Copia Screen Tile";
            this.chkCopiaScreenTile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkCopiaScreenTile.UseVisualStyleBackColor = false;
                                                this.picView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picView.Location = new System.Drawing.Point(3, 95);
            this.picView.Name = "picView";
            this.picView.Size = new System.Drawing.Size(134, 122);
            this.picView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picView.TabIndex = 1;
            this.picView.TabStop = false;
                                                this.pnlCopiaScheda.Controls.Add(this.ugbCopiaRelazioni);
            this.pnlCopiaScheda.Controls.Add(this.uteDescrizione);
            this.pnlCopiaScheda.Controls.Add(this.uteCodice);
            this.pnlCopiaScheda.Controls.Add(this.lblDescrizione);
            this.pnlCopiaScheda.Controls.Add(this.lblCodice);
            this.pnlCopiaScheda.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCopiaScheda.Location = new System.Drawing.Point(143, 3);
            this.pnlCopiaScheda.Name = "pnlCopiaScheda";
            this.TableLayoutPanel.SetRowSpan(this.pnlCopiaScheda, 3);
            this.pnlCopiaScheda.Size = new System.Drawing.Size(624, 307);
            this.pnlCopiaScheda.TabIndex = 0;
                                                this.ugbCopiaRelazioni.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ugbCopiaRelazioni.Controls.Add(this.chkCopiaRuoli);
            this.ugbCopiaRelazioni.Controls.Add(this.chkCopiaScreenTile);
            appearance1.FontData.BoldAsString = "True";
            this.ugbCopiaRelazioni.HeaderAppearance = appearance1;
            this.ugbCopiaRelazioni.Location = new System.Drawing.Point(14, 69);
            this.ugbCopiaRelazioni.Name = "ugbCopiaRelazioni";
            this.ugbCopiaRelazioni.Size = new System.Drawing.Size(597, 235);
            this.ugbCopiaRelazioni.TabIndex = 2;
            this.ugbCopiaRelazioni.Text = "Copia Associazioni";
            this.ugbCopiaRelazioni.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
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
            this.TableLayoutPanel.Size = new System.Drawing.Size(770, 313);
            this.TableLayoutPanel.TabIndex = 0;
                                                this.ultraGroupBox.Controls.Add(this.TableLayoutPanel);
            this.ultraGroupBox.Controls.Add(this.ubConferma);
            this.ultraGroupBox.Controls.Add(this.ubAnnulla);
            this.ultraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox.Location = new System.Drawing.Point(8, 32);
            this.ultraGroupBox.Name = "ultraGroupBox";
            this.ultraGroupBox.Size = new System.Drawing.Size(784, 360);
            this.ultraGroupBox.TabIndex = 0;
            this.ultraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this._frmPUSchedeCopia_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Top.Name = "_frmPUSchedeCopia_Toolbars_Dock_Area_Top";
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(800, 32);
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Top.ToolbarsManager = this.ultraToolbarsManagerForm;
                                                this._frmPUSchedeCopia_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 392);
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Bottom.Name = "_frmPUSchedeCopia_Toolbars_Dock_Area_Bottom";
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(800, 8);
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.ultraToolbarsManagerForm;
                                                this._frmPUSchedeCopia_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 32);
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Left.Name = "_frmPUSchedeCopia_Toolbars_Dock_Area_Left";
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(8, 360);
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Left.ToolbarsManager = this.ultraToolbarsManagerForm;
                                                this._frmPUSchedeCopia_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(792, 32);
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Right.Name = "_frmPUSchedeCopia_Toolbars_Dock_Area_Right";
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(8, 360);
            this._frmPUSchedeCopia_Toolbars_Dock_Area_Right.ToolbarsManager = this.ultraToolbarsManagerForm;
                                                this.ultraToolbarsManagerForm.DesignerFlags = 0;
            this.ultraToolbarsManagerForm.DockWithinContainer = this;
            this.ultraToolbarsManagerForm.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
            this.ultraToolbarsManagerForm.FormDisplayStyle = Infragistics.Win.UltraWinToolbars.FormDisplayStyle.RoundedSizable;
            this.ultraToolbarsManagerForm.RuntimeCustomizationOptions = Infragistics.Win.UltraWinToolbars.RuntimeCustomizationOptions.None;
            this.ultraToolbarsManagerForm.Style = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2007;
                                                this.chkCopiaRuoli.BackColor = System.Drawing.Color.Transparent;
            this.chkCopiaRuoli.Location = new System.Drawing.Point(13, 58);
            this.chkCopiaRuoli.Name = "chkCopiaRuoli";
            this.chkCopiaRuoli.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkCopiaRuoli.Size = new System.Drawing.Size(203, 19);
            this.chkCopiaRuoli.TabIndex = 1;
            this.chkCopiaRuoli.Text = "Copia Ruoli";
            this.chkCopiaRuoli.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkCopiaRuoli.UseVisualStyleBackColor = false;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 400);
            this.Controls.Add(this.ultraGroupBox);
            this.Controls.Add(this._frmPUSchedeCopia_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._frmPUSchedeCopia_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._frmPUSchedeCopia_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this._frmPUSchedeCopia_Toolbars_Dock_Area_Top);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmPUScreenCopia";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "";
            this.Text = "frmPUScreenCopia";
            ((System.ComponentModel.ISupportInitialize)(this.uteDescrizione)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteCodice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picView)).EndInit();
            this.pnlCopiaScheda.ResumeLayout(false);
            this.pnlCopiaScheda.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugbCopiaRelazioni)).EndInit();
            this.ugbCopiaRelazioni.ResumeLayout(false);
            this.TableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).EndInit();
            this.ultraGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraToolbarsManagerForm)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton ubAnnulla;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor uteDescrizione;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor uteCodice;
        private System.Windows.Forms.Label lblDescrizione;
        private System.Windows.Forms.Label lblCodice;
        private Infragistics.Win.Misc.UltraButton ubConferma;
        private System.Windows.Forms.CheckBox chkCopiaScreenTile;
        internal System.Windows.Forms.PictureBox picView;
        private System.Windows.Forms.Panel pnlCopiaScheda;
        private Infragistics.Win.Misc.UltraGroupBox ugbCopiaRelazioni;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager ultraToolbarsManagerForm;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmPUSchedeCopia_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmPUSchedeCopia_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmPUSchedeCopia_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmPUSchedeCopia_Toolbars_Dock_Area_Top;
        private System.Windows.Forms.CheckBox chkCopiaRuoli;
    }
}