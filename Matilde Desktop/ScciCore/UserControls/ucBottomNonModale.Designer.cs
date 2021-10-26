namespace UnicodeSrl.ScciCore
{
    partial class ucBottomNonModale
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucBottomNonModale));
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ubIndietro = new UnicodeSrl.ScciCore.ucEasyButton();
            this.ubAvanti = new UnicodeSrl.ScciCore.ucEasyButton();
            this.TableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
                                                this.TableLayoutPanel.ColumnCount = 7;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.TableLayoutPanel.Controls.Add(this.ubIndietro, 1, 1);
            this.TableLayoutPanel.Controls.Add(this.ubAvanti, 5, 1);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.TableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 3;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 86F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.TableLayoutPanel.Size = new System.Drawing.Size(715, 100);
            this.TableLayoutPanel.TabIndex = 1;
                                                appearance1.FontData.SizeInPoints = 18.31978F;
            appearance1.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance1.ImageBackground")));
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance1.TextHAlignAsString = "Center";
            appearance1.TextVAlignAsString = "Middle";
            this.ubIndietro.Appearance = appearance1;
            this.ubIndietro.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ubIndietro.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ubIndietro.Location = new System.Drawing.Point(14, 7);
            this.ubIndietro.Margin = new System.Windows.Forms.Padding(0);
            this.ubIndietro.Name = "ubIndietro";
            this.ubIndietro.PercImageFill = 0.75F;
            this.ubIndietro.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ubIndietro.ShortcutKey = System.Windows.Forms.Keys.F1;
            this.ubIndietro.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ubIndietro.Size = new System.Drawing.Size(178, 86);
            this.ubIndietro.TabIndex = 1;
            this.ubIndietro.Text = "INDIETRO";
            this.ubIndietro.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ubIndietro.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ubIndietro.Click += new System.EventHandler(this.ubIndietro_Click);
                                                appearance2.FontData.SizeInPoints = 18.31978F;
            appearance2.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance2.ImageBackground")));
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance2.TextHAlignAsString = "Center";
            appearance2.TextVAlignAsString = "Middle";
            this.ubAvanti.Appearance = appearance2;
            this.ubAvanti.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ubAvanti.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ubAvanti.Location = new System.Drawing.Point(519, 7);
            this.ubAvanti.Margin = new System.Windows.Forms.Padding(0);
            this.ubAvanti.Name = "ubAvanti";
            this.ubAvanti.PercImageFill = 0.75F;
            this.ubAvanti.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ubAvanti.ShortcutKey = System.Windows.Forms.Keys.F12;
            this.ubAvanti.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ubAvanti.Size = new System.Drawing.Size(178, 86);
            this.ubAvanti.TabIndex = 0;
            this.ubAvanti.Text = "AVANTI";
            this.ubAvanti.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ubAvanti.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ubAvanti.Click += new System.EventHandler(this.ubAvanti_Click);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Violet;
            this.Controls.Add(this.TableLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ucBottomNonModale";
            this.Size = new System.Drawing.Size(715, 100);
            this.TableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        public ucEasyButton ubIndietro;
        public ucEasyButton ubAvanti;
    }
}
