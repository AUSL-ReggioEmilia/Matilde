namespace UnicodeSrl.ScciManagement
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
            this.Version = new System.Windows.Forms.Label();
            this.Copyright = new System.Windows.Forms.Label();
            this.SuspendLayout();
                                                this.Version.BackColor = System.Drawing.Color.Transparent;
            this.Version.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Version.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(96)))), ((int)(((byte)(20)))));
            this.Version.Location = new System.Drawing.Point(580, 246);
            this.Version.Name = "Version";
            this.Version.Size = new System.Drawing.Size(380, 32);
            this.Version.TabIndex = 3;
            this.Version.Text = "Version {0}.{1:00}.{2:00}.{3:00}";
            this.Version.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                                                this.Copyright.BackColor = System.Drawing.Color.Transparent;
            this.Copyright.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Copyright.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(96)))), ((int)(((byte)(20)))));
            this.Copyright.Location = new System.Drawing.Point(580, 278);
            this.Copyright.Name = "Copyright";
            this.Copyright.Size = new System.Drawing.Size(380, 32);
            this.Copyright.TabIndex = 4;
            this.Copyright.Text = "Copyright";
            this.Copyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(972, 387);
            this.ControlBox = false;
            this.Controls.Add(this.Version);
            this.Controls.Add(this.Copyright);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSplash";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmSplash_Load);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.Label Version;
        internal System.Windows.Forms.Label Copyright;
    }
}