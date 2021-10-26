namespace UnicodeSrl.ScciCore
{
    partial class frmSplash
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
            this.ucEasyTableLayoutPanelSplash = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.Version = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.Copyright = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.ucEasyTableLayoutPanelSplash.SuspendLayout();
            this.SuspendLayout();
                                                this.ucEasyTableLayoutPanelSplash.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyTableLayoutPanelSplash.ColumnCount = 2;
            this.ucEasyTableLayoutPanelSplash.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 88F));
            this.ucEasyTableLayoutPanelSplash.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.ucEasyTableLayoutPanelSplash.Controls.Add(this.Version, 1, 2);
            this.ucEasyTableLayoutPanelSplash.Controls.Add(this.Copyright, 1, 1);
            this.ucEasyTableLayoutPanelSplash.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanelSplash.Location = new System.Drawing.Point(0, 0);
            this.ucEasyTableLayoutPanelSplash.Margin = new System.Windows.Forms.Padding(0);
            this.ucEasyTableLayoutPanelSplash.Name = "ucEasyTableLayoutPanelSplash";
            this.ucEasyTableLayoutPanelSplash.RowCount = 3;
            this.ucEasyTableLayoutPanelSplash.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88F));
            this.ucEasyTableLayoutPanelSplash.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.ucEasyTableLayoutPanelSplash.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.ucEasyTableLayoutPanelSplash.Size = new System.Drawing.Size(1198, 265);
            this.ucEasyTableLayoutPanelSplash.TabIndex = 5;
                                                appearance1.FontData.BoldAsString = "True";
            appearance1.FontData.SizeInPoints = 12.1194F;
            appearance1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(75)))), ((int)(((byte)(131)))));
            appearance1.TextVAlignAsString = "Middle";
            this.Version.Appearance = appearance1;
            this.Version.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Version.Location = new System.Drawing.Point(1054, 248);
            this.Version.Margin = new System.Windows.Forms.Padding(0);
            this.Version.Name = "Version";
            this.Version.ShortcutColor = System.Drawing.Color.Black;
            this.Version.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.Version.ShortcutKey = System.Windows.Forms.Keys.None;
            this.Version.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.Version.Size = new System.Drawing.Size(144, 17);
            this.Version.TabIndex = 0;
            this.Version.Text = "Version {0}.{1:00}.{2:00}.{3:00}";
            this.Version.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.XSmall;
                                                appearance2.FontData.BoldAsString = "True";
            appearance2.FontData.SizeInPoints = 9.522388F;
            appearance2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(75)))), ((int)(((byte)(131)))));
            appearance2.TextVAlignAsString = "Middle";
            this.Copyright.Appearance = appearance2;
            this.Copyright.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Copyright.Location = new System.Drawing.Point(1054, 233);
            this.Copyright.Margin = new System.Windows.Forms.Padding(0);
            this.Copyright.Name = "Copyright";
            this.Copyright.ShortcutColor = System.Drawing.Color.Black;
            this.Copyright.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.Copyright.ShortcutKey = System.Windows.Forms.Keys.None;
            this.Copyright.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.Copyright.Size = new System.Drawing.Size(144, 15);
            this.Copyright.TabIndex = 0;
            this.Copyright.Text = "copyright nascosto..";
            this.Copyright.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.XXSmall;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::UnicodeSrl.ScciCore.Properties.Resources.SplashMatilde;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(1198, 265);
            this.ControlBox = false;
            this.Controls.Add(this.ucEasyTableLayoutPanelSplash);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSplash";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmSplash_Load);
            this.ucEasyTableLayoutPanelSplash.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ucEasyTableLayoutPanel ucEasyTableLayoutPanelSplash;
        private ucEasyLabel Version;
        private ucEasyLabel Copyright;

    }
}