namespace UnicodeSrl.ScciCore
{
    partial class frmProgress
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
            this.UltraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.TableLayoutPanelInfo = new System.Windows.Forms.TableLayoutPanel();
            this.MyProgressBar = new System.Windows.Forms.ProgressBar();
            this.lblInfo = new UnicodeSrl.ScciCore.ucEasyLabel();
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBox)).BeginInit();
            this.UltraGroupBox.SuspendLayout();
            this.TableLayoutPanelInfo.SuspendLayout();
            this.SuspendLayout();
                                                appearance1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            appearance1.BackColor2 = System.Drawing.Color.Orange;
            appearance1.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.UltraGroupBox.Appearance = appearance1;
            this.UltraGroupBox.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.None;
            this.UltraGroupBox.Controls.Add(this.TableLayoutPanelInfo);
            this.UltraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UltraGroupBox.Location = new System.Drawing.Point(0, 0);
            this.UltraGroupBox.Name = "UltraGroupBox";
            this.UltraGroupBox.Size = new System.Drawing.Size(766, 100);
            this.UltraGroupBox.TabIndex = 4;
                                                this.TableLayoutPanelInfo.BackColor = System.Drawing.Color.Transparent;
            this.TableLayoutPanelInfo.ColumnCount = 1;
            this.TableLayoutPanelInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanelInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanelInfo.Controls.Add(this.MyProgressBar, 0, 1);
            this.TableLayoutPanelInfo.Controls.Add(this.lblInfo, 0, 0);
            this.TableLayoutPanelInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanelInfo.Location = new System.Drawing.Point(0, 0);
            this.TableLayoutPanelInfo.Name = "TableLayoutPanelInfo";
            this.TableLayoutPanelInfo.RowCount = 2;
            this.TableLayoutPanelInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanelInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanelInfo.Size = new System.Drawing.Size(766, 100);
            this.TableLayoutPanelInfo.TabIndex = 1;
                                                this.MyProgressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MyProgressBar.Location = new System.Drawing.Point(3, 53);
            this.MyProgressBar.Name = "MyProgressBar";
            this.MyProgressBar.Size = new System.Drawing.Size(760, 44);
            this.MyProgressBar.Step = 1;
            this.MyProgressBar.TabIndex = 1;
                                                appearance2.FontData.SizeInPoints = 16.25F;
            appearance2.TextHAlignAsString = "Center";
            appearance2.TextVAlignAsString = "Middle";
            this.lblInfo.Appearance = appearance2;
            this.lblInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInfo.Location = new System.Drawing.Point(3, 3);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.lblInfo.ShortcutKey = System.Windows.Forms.Keys.None;
            this.lblInfo.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.lblInfo.Size = new System.Drawing.Size(760, 44);
            this.lblInfo.TabIndex = 2;
            this.lblInfo.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 100);
            this.Controls.Add(this.UltraGroupBox);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmProgress";
            this.Text = "frmProgress";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBox)).EndInit();
            this.UltraGroupBox.ResumeLayout(false);
            this.TableLayoutPanelInfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox UltraGroupBox;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanelInfo;
        private System.Windows.Forms.ProgressBar MyProgressBar;
        private ucEasyLabel lblInfo;
    }
}