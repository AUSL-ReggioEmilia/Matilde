namespace UnicodeSrl.ScciCore
{
    partial class ucPictureSelect
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
            Infragistics.Win.UltraWinToolbars.UltraToolbar ultraToolbar1 = new Infragistics.Win.UltraWinToolbars.UltraToolbar("UltraToolbarMain");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Aggiungi");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Rimuovi");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Salva");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Zoom Più");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Zoom Meno");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Adatta");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Dimensioni Reali");
            Infragistics.Win.UltraWinToolbars.LabelTool labelTool2 = new Infragistics.Win.UltraWinToolbars.LabelTool("InfoImage");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Aggiungi");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool31 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Rimuovi");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool33 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Salva");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool39 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Zoom Più");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool40 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Zoom Meno");
            Infragistics.Win.UltraWinToolbars.LabelTool labelTool1 = new Infragistics.Win.UltraWinToolbars.LabelTool("InfoImage");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Adatta");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Dimensioni Reali");
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.PanelPicture = new System.Windows.Forms.Panel();
            this.pbPic = new System.Windows.Forms.PictureBox();
            this.utbZoom = new Infragistics.Win.UltraWinEditors.UltraTrackBar();
            this._ucPictureSelect_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.UltraToolbarsManager = new Infragistics.Win.UltraWinToolbars.UltraToolbarsManager(this.components);
            this._ucPictureSelect_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ucPictureSelect_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ucPictureSelect_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.TableLayoutPanel.SuspendLayout();
            this.PanelPicture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.utbZoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UltraToolbarsManager)).BeginInit();
            this.SuspendLayout();
                                                this.TableLayoutPanel.ColumnCount = 1;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.Controls.Add(this.PanelPicture, 0, 0);
            this.TableLayoutPanel.Controls.Add(this.utbZoom, 0, 1);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel.Location = new System.Drawing.Point(0, 27);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 2;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanel.Size = new System.Drawing.Size(306, 255);
            this.TableLayoutPanel.TabIndex = 0;
                                                this.PanelPicture.AutoScroll = true;
            this.PanelPicture.Controls.Add(this.pbPic);
            this.PanelPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelPicture.Location = new System.Drawing.Point(3, 3);
            this.PanelPicture.Name = "PanelPicture";
            this.PanelPicture.Size = new System.Drawing.Size(300, 219);
            this.PanelPicture.TabIndex = 0;
            this.PanelPicture.MouseHover += new System.EventHandler(this.panelPicture_MouseHover);
            this.PanelPicture.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.panelPicture_MouseWheel);
                                                this.pbPic.Location = new System.Drawing.Point(0, 0);
            this.pbPic.Name = "pbPic";
            this.pbPic.Size = new System.Drawing.Size(140, 136);
            this.pbPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbPic.TabIndex = 51;
            this.pbPic.TabStop = false;
            this.pbPic.MouseHover += new System.EventHandler(this.panelPicture_MouseHover);
            this.pbPic.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.panelPicture_MouseWheel);
                                                this.utbZoom.ButtonSettings.ShowIncrementButtons = Infragistics.Win.DefaultableBoolean.False;
            this.utbZoom.ButtonSettings.ShowMinMaxButtons = Infragistics.Win.DefaultableBoolean.False;
            this.utbZoom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utbZoom.Location = new System.Drawing.Point(3, 228);
            this.utbZoom.Name = "utbZoom";
            this.utbZoom.Size = new System.Drawing.Size(300, 24);
            this.utbZoom.TabIndex = 6;
            this.utbZoom.ValueChanged += new System.EventHandler(this.utbZoom_ValueChanged);
                                                this._ucPictureSelect_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ucPictureSelect_Toolbars_Dock_Area_Left.BackColor = System.Drawing.Color.Transparent;
            this._ucPictureSelect_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._ucPictureSelect_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ucPictureSelect_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 27);
            this._ucPictureSelect_Toolbars_Dock_Area_Left.Name = "_ucPictureSelect_Toolbars_Dock_Area_Left";
            this._ucPictureSelect_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 255);
            this._ucPictureSelect_Toolbars_Dock_Area_Left.ToolbarsManager = this.UltraToolbarsManager;
                                                this.UltraToolbarsManager.DesignerFlags = 1;
            this.UltraToolbarsManager.DockWithinContainer = this;
            this.UltraToolbarsManager.ShowFullMenusDelay = 500;
            ultraToolbar1.DockedColumn = 0;
            ultraToolbar1.DockedRow = 0;
            buttonTool8.InstanceProps.IsFirstInGroup = true;
            labelTool2.InstanceProps.IsFirstInGroup = true;
            ultraToolbar1.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool2,
            buttonTool3,
            buttonTool8,
            buttonTool9,
            buttonTool6,
            buttonTool7,
            labelTool2});
            ultraToolbar1.Settings.AllowCustomize = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowDockBottom = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowDockLeft = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowDockRight = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowDockTop = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowFloating = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.AllowHiding = Infragistics.Win.DefaultableBoolean.False;
            ultraToolbar1.Settings.FillEntireRow = Infragistics.Win.DefaultableBoolean.True;
            ultraToolbar1.Text = "UltraToolbarMain";
            this.UltraToolbarsManager.Toolbars.AddRange(new Infragistics.Win.UltraWinToolbars.UltraToolbar[] {
            ultraToolbar1});
            buttonTool30.SharedPropsInternal.Caption = "Aggiungi Immagine";
            buttonTool31.SharedPropsInternal.Caption = "Rimuovi Immagine";
            buttonTool33.SharedPropsInternal.Caption = "Salva Immagine";
            buttonTool39.SharedPropsInternal.Caption = "Zoom Più";
            buttonTool40.SharedPropsInternal.Caption = "Zoom Meno";
            buttonTool4.SharedPropsInternal.Caption = "Adatta";
            buttonTool5.SharedPropsInternal.Caption = "Dimensioni Reali";
            this.UltraToolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool30,
            buttonTool31,
            buttonTool33,
            buttonTool39,
            buttonTool40,
            labelTool1,
            buttonTool4,
            buttonTool5});
            this.UltraToolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.UltraToolbarsManager_ToolClick);
                                                this._ucPictureSelect_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ucPictureSelect_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.Transparent;
            this._ucPictureSelect_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._ucPictureSelect_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ucPictureSelect_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(306, 27);
            this._ucPictureSelect_Toolbars_Dock_Area_Right.Name = "_ucPictureSelect_Toolbars_Dock_Area_Right";
            this._ucPictureSelect_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 255);
            this._ucPictureSelect_Toolbars_Dock_Area_Right.ToolbarsManager = this.UltraToolbarsManager;
                                                this._ucPictureSelect_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ucPictureSelect_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.Transparent;
            this._ucPictureSelect_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._ucPictureSelect_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ucPictureSelect_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._ucPictureSelect_Toolbars_Dock_Area_Top.Name = "_ucPictureSelect_Toolbars_Dock_Area_Top";
            this._ucPictureSelect_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(306, 27);
            this._ucPictureSelect_Toolbars_Dock_Area_Top.ToolbarsManager = this.UltraToolbarsManager;
                                                this._ucPictureSelect_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ucPictureSelect_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.Transparent;
            this._ucPictureSelect_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._ucPictureSelect_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ucPictureSelect_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 282);
            this._ucPictureSelect_Toolbars_Dock_Area_Bottom.Name = "_ucPictureSelect_Toolbars_Dock_Area_Bottom";
            this._ucPictureSelect_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(306, 0);
            this._ucPictureSelect_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.UltraToolbarsManager;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.TableLayoutPanel);
            this.Controls.Add(this._ucPictureSelect_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._ucPictureSelect_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._ucPictureSelect_Toolbars_Dock_Area_Bottom);
            this.Controls.Add(this._ucPictureSelect_Toolbars_Dock_Area_Top);
            this.Name = "ucPictureSelect";
            this.Size = new System.Drawing.Size(306, 282);
            this.Load += new System.EventHandler(this.ucPictureSelect_Load);
            this.TableLayoutPanel.ResumeLayout(false);
            this.TableLayoutPanel.PerformLayout();
            this.PanelPicture.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.utbZoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UltraToolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        private System.Windows.Forms.Panel PanelPicture;
        private System.Windows.Forms.PictureBox pbPic;
        private Infragistics.Win.UltraWinEditors.UltraTrackBar utbZoom;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsManager UltraToolbarsManager;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ucPictureSelect_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ucPictureSelect_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ucPictureSelect_Toolbars_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ucPictureSelect_Toolbars_Dock_Area_Top;
        private System.Windows.Forms.OpenFileDialog OpenFileDialog;
    }
}
