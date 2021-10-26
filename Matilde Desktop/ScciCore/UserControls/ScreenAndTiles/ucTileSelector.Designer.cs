namespace UnicodeSrl.ScciCore.UserControls.ScreenAndTiles
{
    partial class ucTileSelector
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
            this.pb = new System.Windows.Forms.PictureBox();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pb)).BeginInit();
            this.SuspendLayout();
                                                this.pb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pb.Location = new System.Drawing.Point(0, 0);
            this.pb.Margin = new System.Windows.Forms.Padding(1);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(50, 100);
            this.pb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb.TabIndex = 0;
            this.pb.TabStop = false;
            this.pb.Click += new System.EventHandler(this.pb_Click);
                                                this.tooltip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pb);
            this.DoubleBuffered = true;
            this.Name = "ucTileSelector";
            this.Size = new System.Drawing.Size(50, 100);
            ((System.ComponentModel.ISupportInitialize)(this.pb)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pb;
        private System.Windows.Forms.ToolTip tooltip;
    }
}
