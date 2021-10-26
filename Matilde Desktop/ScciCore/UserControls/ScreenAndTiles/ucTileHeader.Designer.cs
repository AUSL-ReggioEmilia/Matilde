namespace UnicodeSrl.ScciCore.UserControls.ScreenAndTiles
{
    partial class ucTileHeader
    {
                                private System.ComponentModel.IContainer components = null;



        #region Component Designer generated code

                                        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.pb = new System.Windows.Forms.PictureBox();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.lblCaption = new UnicodeSrl.ScciCore.ucEasyLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pb)).BeginInit();
            this.SuspendLayout();
                                                this.pb.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pb.Dock = System.Windows.Forms.DockStyle.Right;
            this.pb.Image = global::UnicodeSrl.ScciCore.Properties.Resources.FrecciaSxN_256;
            this.pb.Location = new System.Drawing.Point(166, 0);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(40, 15);
            this.pb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb.TabIndex = 0;
            this.pb.TabStop = false;
            this.pb.Click += new System.EventHandler(this.pb_Click);
                                                this.tooltip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
                                                appearance1.FontData.SizeInPoints = 12.1194F;
            appearance1.TextVAlignAsString = "Middle";
            this.lblCaption.Appearance = appearance1;
            this.lblCaption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCaption.Location = new System.Drawing.Point(0, 0);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.ShortcutColor = System.Drawing.Color.Black;
            this.lblCaption.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.lblCaption.ShortcutKey = System.Windows.Forms.Keys.None;
            this.lblCaption.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.lblCaption.Size = new System.Drawing.Size(166, 15);
            this.lblCaption.TabIndex = 1;
            this.lblCaption.Text = "-";
            this.lblCaption.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.XSmall;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblCaption);
            this.Controls.Add(this.pb);
            this.Name = "ucTileHeader";
            this.Size = new System.Drawing.Size(206, 15);
            ((System.ComponentModel.ISupportInitialize)(this.pb)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pb;
        private System.Windows.Forms.ToolTip tooltip;
        private ucEasyLabel lblCaption;
    }
}
