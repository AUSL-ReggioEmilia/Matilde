namespace UnicodeSrl.ScciCore
{
    partial class frmSmartCardProgress
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSmartCardProgress));
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            this.UltraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.TableLayoutPanelInfo = new System.Windows.Forms.TableLayoutPanel();
            this.MyProgressBar = new System.Windows.Forms.ProgressBar();
            this.utxtLog = new UnicodeSrl.ScciCore.ucEasyTextBox();
            this.btnCancel = new UnicodeSrl.ScciCore.ucEasyButton();
            this.lblInfo = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.PictureBox = new UnicodeSrl.ScciCore.ucEasyPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBox)).BeginInit();
            this.UltraGroupBox.SuspendLayout();
            this.TableLayoutPanel.SuspendLayout();
            this.TableLayoutPanelInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utxtLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
            this.SuspendLayout();
                                                appearance1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            appearance1.BackColor2 = System.Drawing.Color.Orange;
            appearance1.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.UltraGroupBox.Appearance = appearance1;
            this.UltraGroupBox.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.None;
            this.UltraGroupBox.Controls.Add(this.TableLayoutPanel);
            this.UltraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UltraGroupBox.Location = new System.Drawing.Point(0, 0);
            this.UltraGroupBox.Name = "UltraGroupBox";
            this.UltraGroupBox.Size = new System.Drawing.Size(975, 220);
            this.UltraGroupBox.TabIndex = 3;
                                                this.TableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.TableLayoutPanel.ColumnCount = 2;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85F));
            this.TableLayoutPanel.Controls.Add(this.TableLayoutPanelInfo, 1, 0);
            this.TableLayoutPanel.Controls.Add(this.PictureBox, 0, 0);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 1;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.Size = new System.Drawing.Size(975, 220);
            this.TableLayoutPanel.TabIndex = 0;
                                                this.TableLayoutPanelInfo.ColumnCount = 2;
            this.TableLayoutPanelInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanelInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanelInfo.Controls.Add(this.MyProgressBar, 0, 1);
            this.TableLayoutPanelInfo.Controls.Add(this.utxtLog, 1, 0);
            this.TableLayoutPanelInfo.Controls.Add(this.btnCancel, 0, 2);
            this.TableLayoutPanelInfo.Controls.Add(this.lblInfo, 0, 0);
            this.TableLayoutPanelInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanelInfo.Location = new System.Drawing.Point(149, 3);
            this.TableLayoutPanelInfo.Name = "TableLayoutPanelInfo";
            this.TableLayoutPanelInfo.RowCount = 3;
            this.TableLayoutPanelInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.TableLayoutPanelInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TableLayoutPanelInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.TableLayoutPanelInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanelInfo.Size = new System.Drawing.Size(823, 214);
            this.TableLayoutPanelInfo.TabIndex = 1;
                                                this.TableLayoutPanelInfo.SetColumnSpan(this.MyProgressBar, 2);
            this.MyProgressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MyProgressBar.Location = new System.Drawing.Point(3, 120);
            this.MyProgressBar.Name = "MyProgressBar";
            this.MyProgressBar.Size = new System.Drawing.Size(817, 36);
            this.MyProgressBar.TabIndex = 1;
                                                appearance2.FontData.SizeInPoints = 14.26829F;
            this.utxtLog.Appearance = appearance2;
            this.utxtLog.AutoSize = false;
            this.utxtLog.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2007;
            this.utxtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utxtLog.Location = new System.Drawing.Point(414, 3);
            this.utxtLog.Multiline = true;
            this.utxtLog.Name = "utxtLog";
            this.utxtLog.ReadOnly = true;
            this.utxtLog.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.utxtLog.Size = new System.Drawing.Size(406, 111);
            this.utxtLog.TabIndex = 3;
            this.utxtLog.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.utxtLog.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
                                                appearance3.FontData.SizeInPoints = 20.60976F;
            appearance3.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance3.ImageBackground")));
            appearance3.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance3.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance3.TextHAlignAsString = "Center";
            appearance3.TextVAlignAsString = "Middle";
            this.btnCancel.Appearance = appearance3;
            this.btnCancel.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.TableLayoutPanelInfo.SetColumnSpan(this.btnCancel, 3);
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCancel.Location = new System.Drawing.Point(3, 162);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.PercImageFill = 0.75F;
            this.btnCancel.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.btnCancel.ShortcutKey = System.Windows.Forms.Keys.T;
            this.btnCancel.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.btnCancel.Size = new System.Drawing.Size(817, 49);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Termina";
            this.btnCancel.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.btnCancel.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
                                                appearance4.FontData.SizeInPoints = 25.36585F;
            appearance4.TextHAlignAsString = "Center";
            appearance4.TextVAlignAsString = "Middle";
            this.lblInfo.Appearance = appearance4;
            this.lblInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInfo.Location = new System.Drawing.Point(3, 3);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Large;
            this.lblInfo.ShortcutKey = System.Windows.Forms.Keys.None;
            this.lblInfo.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.lblInfo.Size = new System.Drawing.Size(405, 111);
            this.lblInfo.TabIndex = 5;
            this.lblInfo.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Large;
                                                this.PictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PictureBox.Location = new System.Drawing.Point(3, 3);
            this.PictureBox.Name = "PictureBox";
            this.PictureBox.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.PictureBox.ShortcutKey = System.Windows.Forms.Keys.None;
            this.PictureBox.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.PictureBox.Size = new System.Drawing.Size(140, 214);
            this.PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBox.TabIndex = 2;
            this.PictureBox.TabStop = false;
            this.PictureBox.Click += new System.EventHandler(this.PictureBox_Click);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 220);
            this.Controls.Add(this.UltraGroupBox);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "frmSmartCardProgress";
            this.Text = "Smart Card Progress";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSmartCardProgress_FormClosing);
            this.Load += new System.EventHandler(this.frmSmartCardProgress_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmSmartCardProgress_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBox)).EndInit();
            this.UltraGroupBox.ResumeLayout(false);
            this.TableLayoutPanel.ResumeLayout(false);
            this.TableLayoutPanelInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.utxtLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox UltraGroupBox;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanelInfo;
        private System.Windows.Forms.ProgressBar MyProgressBar;
        private ucEasyTextBox utxtLog;
        private ucEasyButton btnCancel;
        private ucEasyLabel lblInfo;
        private ucEasyPictureBox PictureBox;
    }
}