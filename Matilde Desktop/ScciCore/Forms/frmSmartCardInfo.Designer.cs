namespace UnicodeSrl.ScciCore
{
    partial class frmSmartCardInfo
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.PictureBox = new System.Windows.Forms.PictureBox();
            this.TableLayoutPanelInfo = new System.Windows.Forms.TableLayoutPanel();
            this.MyProgressBar = new System.Windows.Forms.ProgressBar();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnAggiorna = new System.Windows.Forms.Button();
            this.utxtLog = new UnicodeSrl.ScciCore.ucEasyTextBox();
            this.UltraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.TableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
            this.TableLayoutPanelInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utxtLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBox)).BeginInit();
            this.UltraGroupBox.SuspendLayout();
            this.SuspendLayout();
                                                this.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCancel.Location = new System.Drawing.Point(3, 359);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(451, 37);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Annulla";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
                                                this.TableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.TableLayoutPanel.ColumnCount = 2;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.TableLayoutPanel.Controls.Add(this.PictureBox, 0, 0);
            this.TableLayoutPanel.Controls.Add(this.TableLayoutPanelInfo, 1, 0);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 1;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 405F));
            this.TableLayoutPanel.Size = new System.Drawing.Size(578, 405);
            this.TableLayoutPanel.TabIndex = 0;
                                                this.PictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PictureBox.Location = new System.Drawing.Point(25, 170);
            this.PictureBox.Name = "PictureBox";
            this.PictureBox.Size = new System.Drawing.Size(64, 64);
            this.PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBox.TabIndex = 0;
            this.PictureBox.TabStop = false;
                                                this.TableLayoutPanelInfo.ColumnCount = 1;
            this.TableLayoutPanelInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanelInfo.Controls.Add(this.btnCancel, 0, 4);
            this.TableLayoutPanelInfo.Controls.Add(this.MyProgressBar, 0, 3);
            this.TableLayoutPanelInfo.Controls.Add(this.lblInfo, 0, 1);
            this.TableLayoutPanelInfo.Controls.Add(this.btnAggiorna, 0, 0);
            this.TableLayoutPanelInfo.Controls.Add(this.utxtLog, 0, 2);
            this.TableLayoutPanelInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanelInfo.Location = new System.Drawing.Point(118, 3);
            this.TableLayoutPanelInfo.Name = "TableLayoutPanelInfo";
            this.TableLayoutPanelInfo.RowCount = 5;
            this.TableLayoutPanelInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.TableLayoutPanelInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.TableLayoutPanelInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.TableLayoutPanelInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TableLayoutPanelInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.TableLayoutPanelInfo.Size = new System.Drawing.Size(457, 399);
            this.TableLayoutPanelInfo.TabIndex = 1;
                                                this.MyProgressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MyProgressBar.Location = new System.Drawing.Point(3, 280);
            this.MyProgressBar.Name = "MyProgressBar";
            this.MyProgressBar.Size = new System.Drawing.Size(451, 73);
            this.MyProgressBar.TabIndex = 1;
                                                this.lblInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInfo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.Location = new System.Drawing.Point(3, 39);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(451, 99);
            this.lblInfo.TabIndex = 2;
                                                this.btnAggiorna.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAggiorna.Location = new System.Drawing.Point(3, 3);
            this.btnAggiorna.Name = "btnAggiorna";
            this.btnAggiorna.Size = new System.Drawing.Size(451, 33);
            this.btnAggiorna.TabIndex = 1;
            this.btnAggiorna.Text = "Aggiorna Stato Tessera";
            this.btnAggiorna.UseVisualStyleBackColor = true;
            this.btnAggiorna.Click += new System.EventHandler(this.btnAggiorna_Click);
                                                appearance1.FontData.SizeInPoints = 11.09756F;
            this.utxtLog.Appearance = appearance1;
            this.utxtLog.AutoSize = false;
            this.utxtLog.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2007;
            this.utxtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utxtLog.Location = new System.Drawing.Point(3, 141);
            this.utxtLog.Multiline = true;
            this.utxtLog.Name = "utxtLog";
            this.utxtLog.ReadOnly = true;
            this.utxtLog.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.utxtLog.Size = new System.Drawing.Size(451, 133);
            this.utxtLog.TabIndex = 3;
            this.utxtLog.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.XSmall;
            this.utxtLog.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
                                                this.UltraGroupBox.Controls.Add(this.TableLayoutPanel);
            this.UltraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UltraGroupBox.Location = new System.Drawing.Point(0, 0);
            this.UltraGroupBox.Name = "UltraGroupBox";
            this.UltraGroupBox.Size = new System.Drawing.Size(584, 411);
            this.UltraGroupBox.TabIndex = 2;
            this.UltraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 411);
            this.ControlBox = false;
            this.Controls.Add(this.UltraGroupBox);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmSmartCardInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmSmartCardInfo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSmartCardInfo_FormClosing);
            this.Load += new System.EventHandler(this.frmSmartCardInfo_Load);
            this.TableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
            this.TableLayoutPanelInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.utxtLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBox)).EndInit();
            this.UltraGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        private System.Windows.Forms.PictureBox PictureBox;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanelInfo;
        private System.Windows.Forms.ProgressBar MyProgressBar;
        private Infragistics.Win.Misc.UltraGroupBox UltraGroupBox;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnAggiorna;
        private ucEasyTextBox utxtLog;
    }
}