namespace UnicodeSrl.ScciCore
{
    partial class frmInfo
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
            this.BackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.btnCancel = new System.Windows.Forms.Button();
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.PictureBox = new System.Windows.Forms.PictureBox();
            this.TableLayoutPanelInfo = new System.Windows.Forms.TableLayoutPanel();
            this.MyProgressBar = new System.Windows.Forms.ProgressBar();
            this.lblInfo = new System.Windows.Forms.Label();
            this.UltraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnAggiorna = new System.Windows.Forms.Button();
            this.TableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
            this.TableLayoutPanelInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBox)).BeginInit();
            this.UltraGroupBox.SuspendLayout();
            this.SuspendLayout();
                                                this.btnCancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCancel.Location = new System.Drawing.Point(3, 360);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(451, 36);
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
            this.TableLayoutPanelInfo.Controls.Add(this.btnCancel, 0, 3);
            this.TableLayoutPanelInfo.Controls.Add(this.MyProgressBar, 0, 2);
            this.TableLayoutPanelInfo.Controls.Add(this.lblInfo, 0, 1);
            this.TableLayoutPanelInfo.Controls.Add(this.btnAggiorna, 0, 0);
            this.TableLayoutPanelInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanelInfo.Location = new System.Drawing.Point(118, 3);
            this.TableLayoutPanelInfo.Name = "TableLayoutPanelInfo";
            this.TableLayoutPanelInfo.RowCount = 4;
            this.TableLayoutPanelInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.TableLayoutPanelInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.TableLayoutPanelInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.TableLayoutPanelInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.TableLayoutPanelInfo.Size = new System.Drawing.Size(457, 399);
            this.TableLayoutPanelInfo.TabIndex = 1;
                                                this.MyProgressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MyProgressBar.Location = new System.Drawing.Point(3, 261);
            this.MyProgressBar.Name = "MyProgressBar";
            this.MyProgressBar.Size = new System.Drawing.Size(451, 93);
            this.MyProgressBar.TabIndex = 1;
                                                this.lblInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInfo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.Location = new System.Drawing.Point(3, 39);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(451, 219);
            this.lblInfo.TabIndex = 2;
                                                this.UltraGroupBox.Controls.Add(this.TableLayoutPanel);
            this.UltraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UltraGroupBox.Location = new System.Drawing.Point(0, 0);
            this.UltraGroupBox.Name = "UltraGroupBox";
            this.UltraGroupBox.Size = new System.Drawing.Size(584, 411);
            this.UltraGroupBox.TabIndex = 2;
            this.UltraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.btnAggiorna.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAggiorna.Location = new System.Drawing.Point(3, 3);
            this.btnAggiorna.Name = "btnAggiorna";
            this.btnAggiorna.Size = new System.Drawing.Size(451, 33);
            this.btnAggiorna.TabIndex = 1;
            this.btnAggiorna.Text = "Aggiorna Stato Tessera";
            this.btnAggiorna.UseVisualStyleBackColor = true;
            this.btnAggiorna.Click += new System.EventHandler(this.btnAggiorna_Click);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 411);
            this.ControlBox = false;
            this.Controls.Add(this.UltraGroupBox);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmInfo";
            this.Load += new System.EventHandler(this.frmInfo_Load);
            this.TableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
            this.TableLayoutPanelInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBox)).EndInit();
            this.UltraGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker BackgroundWorker;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        private System.Windows.Forms.PictureBox PictureBox;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanelInfo;
        private System.Windows.Forms.ProgressBar MyProgressBar;
        private Infragistics.Win.Misc.UltraGroupBox UltraGroupBox;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnAggiorna;
    }
}