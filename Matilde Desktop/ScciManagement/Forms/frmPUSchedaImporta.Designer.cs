namespace UnicodeSrl.ScciManagement
{
    partial class frmPUSchedaImporta
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
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            this.ultraGroupBoxForm = new Infragistics.Win.Misc.UltraGroupBox();
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.picView = new System.Windows.Forms.PictureBox();
            this.pnlExportScheda = new System.Windows.Forms.Panel();
            this.ubFilePath = new Infragistics.Win.Misc.UltraButton();
            this.uteFilePath = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.lblFilePath = new System.Windows.Forms.Label();
            this.ugbEsito = new Infragistics.Win.Misc.UltraGroupBox();
            this.tlpEsito = new System.Windows.Forms.TableLayoutPanel();
            this.picEsito = new System.Windows.Forms.PictureBox();
            this.uteEsito = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ubConferma = new Infragistics.Win.Misc.UltraButton();
            this.ubAnnulla = new Infragistics.Win.Misc.UltraButton();
            this.UltraToolbarsManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._frmPUView_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._frmPUView_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._frmPUView_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._frmPUView_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.chkVersione = new System.Windows.Forms.CheckBox();
            this.uceVersioni = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBoxForm)).BeginInit();
            this.ultraGroupBoxForm.SuspendLayout();
            this.TableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picView)).BeginInit();
            this.pnlExportScheda.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uteFilePath)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ugbEsito)).BeginInit();
            this.ugbEsito.SuspendLayout();
            this.tlpEsito.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picEsito)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteEsito)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UltraToolbarsManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uceVersioni)).BeginInit();
            this.SuspendLayout();
                                                this.ultraGroupBoxForm.Controls.Add(this.TableLayoutPanel);
            this.ultraGroupBoxForm.Controls.Add(this.ubConferma);
            this.ultraGroupBoxForm.Controls.Add(this.ubAnnulla);
            this.ultraGroupBoxForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBoxForm.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBoxForm.Name = "ultraGroupBoxForm";
            this.ultraGroupBoxForm.Size = new System.Drawing.Size(814, 464);
            this.ultraGroupBoxForm.TabIndex = 0;
            this.ultraGroupBoxForm.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.TableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.TableLayoutPanel.ColumnCount = 2;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.Controls.Add(this.picView, 0, 1);
            this.TableLayoutPanel.Controls.Add(this.pnlExportScheda, 1, 0);
            this.TableLayoutPanel.Location = new System.Drawing.Point(6, 6);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 3;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 128F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanel.Size = new System.Drawing.Size(802, 423);
            this.TableLayoutPanel.TabIndex = 7;
                                                this.picView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picView.Location = new System.Drawing.Point(3, 150);
            this.picView.Name = "picView";
            this.picView.Size = new System.Drawing.Size(134, 122);
            this.picView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picView.TabIndex = 1;
            this.picView.TabStop = false;
                                                this.pnlExportScheda.Controls.Add(this.uceVersioni);
            this.pnlExportScheda.Controls.Add(this.chkVersione);
            this.pnlExportScheda.Controls.Add(this.ubFilePath);
            this.pnlExportScheda.Controls.Add(this.uteFilePath);
            this.pnlExportScheda.Controls.Add(this.lblFilePath);
            this.pnlExportScheda.Controls.Add(this.ugbEsito);
            this.pnlExportScheda.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlExportScheda.Location = new System.Drawing.Point(143, 3);
            this.pnlExportScheda.Name = "pnlExportScheda";
            this.TableLayoutPanel.SetRowSpan(this.pnlExportScheda, 3);
            this.pnlExportScheda.Size = new System.Drawing.Size(656, 417);
            this.pnlExportScheda.TabIndex = 0;
                                                this.ubFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ubFilePath.Location = new System.Drawing.Point(623, 14);
            this.ubFilePath.Name = "ubFilePath";
            this.ubFilePath.Size = new System.Drawing.Size(30, 21);
            this.ubFilePath.TabIndex = 1;
            this.ubFilePath.Text = "...";
            this.ubFilePath.Click += new System.EventHandler(this.ubFilePath_Click);
                                                this.uteFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uteFilePath.Location = new System.Drawing.Point(87, 14);
            this.uteFilePath.Name = "uteFilePath";
            this.uteFilePath.Size = new System.Drawing.Size(530, 21);
            this.uteFilePath.TabIndex = 0;
                                                this.lblFilePath.BackColor = System.Drawing.Color.Transparent;
            this.lblFilePath.Location = new System.Drawing.Point(10, 18);
            this.lblFilePath.Name = "lblFilePath";
            this.lblFilePath.Size = new System.Drawing.Size(71, 20);
            this.lblFilePath.TabIndex = 22;
            this.lblFilePath.Text = "File Import";
                                                this.ugbEsito.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ugbEsito.Controls.Add(this.tlpEsito);
            appearance3.FontData.BoldAsString = "True";
            this.ugbEsito.HeaderAppearance = appearance3;
            this.ugbEsito.Location = new System.Drawing.Point(13, 68);
            this.ugbEsito.Name = "ugbEsito";
            this.ugbEsito.Size = new System.Drawing.Size(640, 346);
            this.ugbEsito.TabIndex = 21;
            this.ugbEsito.Text = "Esito";
            this.ugbEsito.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.tlpEsito.ColumnCount = 2;
            this.tlpEsito.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tlpEsito.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpEsito.Controls.Add(this.picEsito, 0, 1);
            this.tlpEsito.Controls.Add(this.uteEsito, 1, 0);
            this.tlpEsito.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpEsito.Location = new System.Drawing.Point(3, 16);
            this.tlpEsito.Name = "tlpEsito";
            this.tlpEsito.RowCount = 3;
            this.tlpEsito.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpEsito.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tlpEsito.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpEsito.Size = new System.Drawing.Size(634, 327);
            this.tlpEsito.TabIndex = 25;
                                                this.picEsito.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picEsito.Location = new System.Drawing.Point(3, 121);
            this.picEsito.Name = "picEsito";
            this.picEsito.Size = new System.Drawing.Size(84, 84);
            this.picEsito.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picEsito.TabIndex = 26;
            this.picEsito.TabStop = false;
                                                this.uteEsito.AlwaysInEditMode = true;
            this.uteEsito.AutoSize = false;
            this.uteEsito.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uteEsito.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uteEsito.Location = new System.Drawing.Point(93, 3);
            this.uteEsito.Multiline = true;
            this.uteEsito.Name = "uteEsito";
            this.uteEsito.ReadOnly = true;
            this.tlpEsito.SetRowSpan(this.uteEsito, 3);
            this.uteEsito.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.uteEsito.Size = new System.Drawing.Size(538, 321);
            this.uteEsito.TabIndex = 0;
                                                this.ubConferma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ubConferma.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.ubConferma.Location = new System.Drawing.Point(733, 435);
            this.ubConferma.Name = "ubConferma";
            this.ubConferma.Size = new System.Drawing.Size(75, 23);
            this.ubConferma.TabIndex = 1;
            this.ubConferma.Text = "&Conferma";
            this.ubConferma.Click += new System.EventHandler(this.ubConferma_Click);
                                                this.ubAnnulla.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ubAnnulla.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.ubAnnulla.Location = new System.Drawing.Point(5, 435);
            this.ubAnnulla.Name = "ubAnnulla";
            this.ubAnnulla.Size = new System.Drawing.Size(75, 23);
            this.ubAnnulla.TabIndex = 0;
            this.ubAnnulla.Text = "&Annulla";
            this.ubAnnulla.Click += new System.EventHandler(this.ubAnnulla_Click);
                                                this.UltraToolbarsManager.DesignerFlags = 0;
            this.UltraToolbarsManager.DockWithinContainer = this;
            this.UltraToolbarsManager.DockWithinContainerBaseType = typeof(System.Windows.Forms.Form);
                                                this._frmPUView_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPUView_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._frmPUView_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._frmPUView_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPUView_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmPUView_Toolbars_Dock_Area_Top.Name = "_frmPUView_Toolbars_Dock_Area_Top";
            this._frmPUView_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(814, 0);
            this._frmPUView_Toolbars_Dock_Area_Top.ToolbarsManager = this.UltraToolbarsManager;
                                                this._frmPUView_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPUView_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._frmPUView_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._frmPUView_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPUView_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 464);
            this._frmPUView_Toolbars_Dock_Area_Bottom.Name = "_frmPUView_Toolbars_Dock_Area_Bottom";
            this._frmPUView_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(814, 0);
            this._frmPUView_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.UltraToolbarsManager;
                                                this._frmPUView_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPUView_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._frmPUView_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._frmPUView_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPUView_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._frmPUView_Toolbars_Dock_Area_Left.Name = "_frmPUView_Toolbars_Dock_Area_Left";
            this._frmPUView_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 464);
            this._frmPUView_Toolbars_Dock_Area_Left.ToolbarsManager = this.UltraToolbarsManager;
                                                this._frmPUView_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPUView_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._frmPUView_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._frmPUView_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPUView_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(814, 0);
            this._frmPUView_Toolbars_Dock_Area_Right.Name = "_frmPUView_Toolbars_Dock_Area_Right";
            this._frmPUView_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 464);
            this._frmPUView_Toolbars_Dock_Area_Right.ToolbarsManager = this.UltraToolbarsManager;
                                                this.openFileDialog.FileName = "openFileDialog1";
                                                this.chkVersione.Checked = true;
            this.chkVersione.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVersione.Location = new System.Drawing.Point(87, 41);
            this.chkVersione.Name = "chkVersione";
            this.chkVersione.Size = new System.Drawing.Size(185, 21);
            this.chkVersione.TabIndex = 21;
            this.chkVersione.Text = "Importa solo una Versione";
            this.chkVersione.UseVisualStyleBackColor = true;
            this.chkVersione.CheckedChanged += new System.EventHandler(this.chkVersione_CheckedChanged);
                                                this.uceVersioni.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2007;
            this.uceVersioni.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.uceVersioni.Location = new System.Drawing.Point(290, 41);
            this.uceVersioni.Name = "uceVersioni";
            this.uceVersioni.Size = new System.Drawing.Size(327, 21);
            this.uceVersioni.TabIndex = 2;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 464);
            this.ControlBox = false;
            this.Controls.Add(this.ultraGroupBoxForm);
            this.Controls.Add(this._frmPUView_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._frmPUView_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._frmPUView_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this._frmPUView_Toolbars_Dock_Area_Top);
            this.Name = "frmPUSchedaImporta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmPUSchedaImporta";
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBoxForm)).EndInit();
            this.ultraGroupBoxForm.ResumeLayout(false);
            this.TableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picView)).EndInit();
            this.pnlExportScheda.ResumeLayout(false);
            this.pnlExportScheda.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uteFilePath)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ugbEsito)).EndInit();
            this.ugbEsito.ResumeLayout(false);
            this.tlpEsito.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picEsito)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteEsito)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UltraToolbarsManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uceVersioni)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBoxForm;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        internal System.Windows.Forms.PictureBox picView;
        private System.Windows.Forms.Panel pnlExportScheda;
        private Infragistics.Win.Misc.UltraGroupBox ugbEsito;
        private Infragistics.Win.Misc.UltraButton ubConferma;
        private Infragistics.Win.Misc.UltraButton ubAnnulla;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager UltraToolbarsManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmPUView_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmPUView_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmPUView_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _frmPUView_Toolbars_Dock_Area_Top;
        private Infragistics.Win.Misc.UltraButton ubFilePath;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor uteFilePath;
        private System.Windows.Forms.Label lblFilePath;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TableLayoutPanel tlpEsito;
        internal System.Windows.Forms.PictureBox picEsito;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor uteEsito;
        private System.Windows.Forms.CheckBox chkVersione;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor uceVersioni;
    }
}