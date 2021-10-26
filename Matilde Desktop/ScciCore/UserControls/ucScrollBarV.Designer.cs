namespace UnicodeSrl.ScciCore
{
    partial class ucScrollBarV
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
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ubSu = new Infragistics.Win.Misc.UltraButton();
            this.ubGiu = new Infragistics.Win.Misc.UltraButton();
            this.TableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
                                                this.TableLayoutPanel.ColumnCount = 1;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanel.Controls.Add(this.ubSu, 0, 0);
            this.TableLayoutPanel.Controls.Add(this.ubGiu, 0, 2);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 3;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.TableLayoutPanel.Size = new System.Drawing.Size(80, 530);
            this.TableLayoutPanel.TabIndex = 1;
            this.TableLayoutPanel.Resize += new System.EventHandler(this.TableLayoutPanel_Resize);
                                                this.ubSu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ubSu.Location = new System.Drawing.Point(3, 3);
            this.ubSu.Name = "ubSu";
            this.ubSu.Size = new System.Drawing.Size(74, 44);
            this.ubSu.TabIndex = 0;
            this.ubSu.Click += new System.EventHandler(this.UltraButton_Click);
                                                this.ubGiu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ubGiu.Location = new System.Drawing.Point(3, 483);
            this.ubGiu.Name = "ubGiu";
            this.ubGiu.Size = new System.Drawing.Size(74, 44);
            this.ubGiu.TabIndex = 1;
            this.ubGiu.Click += new System.EventHandler(this.UltraButton_Click);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.TableLayoutPanel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ucScrollBarV";
            this.Size = new System.Drawing.Size(80, 530);
            this.TableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        internal Infragistics.Win.Misc.UltraButton ubSu;
        internal Infragistics.Win.Misc.UltraButton ubGiu;
    }
}
