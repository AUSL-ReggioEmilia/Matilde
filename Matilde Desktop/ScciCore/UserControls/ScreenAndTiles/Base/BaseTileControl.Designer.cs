using UnicodeSrl.ScciCore;

namespace UnicodeSrl.CdssScreenTiles
{
    partial class BaseTileControl
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.content = new System.Windows.Forms.Panel();
            this.tlpTile = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.tlpTileTitle = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.pbTile = new System.Windows.Forms.PictureBox();
            this.lblTitleTile = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.cmdWnd = new UnicodeSrl.ScciCore.ucEasyButton();
            this.tlpTile.SuspendLayout();
            this.tlpTileTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTile)).BeginInit();
            this.SuspendLayout();
                                                this.content.BackColor = System.Drawing.Color.Transparent;
            this.content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content.Location = new System.Drawing.Point(5, 40);
            this.content.Margin = new System.Windows.Forms.Padding(0);
            this.content.Name = "content";
            this.content.Size = new System.Drawing.Size(640, 393);
            this.content.TabIndex = 2;
                                                this.tlpTile.ColumnCount = 1;
            this.tlpTile.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTile.Controls.Add(this.tlpTileTitle, 0, 0);
            this.tlpTile.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpTile.Location = new System.Drawing.Point(5, 5);
            this.tlpTile.Name = "tlpTile";
            this.tlpTile.RowCount = 1;
            this.tlpTile.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlpTile.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlpTile.Size = new System.Drawing.Size(640, 35);
            this.tlpTile.TabIndex = 1;
                                                this.tlpTileTitle.BackColor = System.Drawing.Color.LightSteelBlue;
            this.tlpTileTitle.ColumnCount = 3;
            this.tlpTileTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tlpTileTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTileTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tlpTileTitle.Controls.Add(this.pbTile, 0, 0);
            this.tlpTileTitle.Controls.Add(this.lblTitleTile, 1, 0);
            this.tlpTileTitle.Controls.Add(this.cmdWnd, 2, 0);
            this.tlpTileTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpTileTitle.Location = new System.Drawing.Point(0, 0);
            this.tlpTileTitle.Margin = new System.Windows.Forms.Padding(0);
            this.tlpTileTitle.Name = "tlpTileTitle";
            this.tlpTileTitle.RowCount = 1;
            this.tlpTileTitle.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpTileTitle.Size = new System.Drawing.Size(640, 35);
            this.tlpTileTitle.TabIndex = 3;
                                                this.pbTile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbTile.Image = global::UnicodeSrl.ScciCore.Properties.Resources.AlertAllergia_32;
            this.pbTile.Location = new System.Drawing.Point(1, 1);
            this.pbTile.Margin = new System.Windows.Forms.Padding(1);
            this.pbTile.Name = "pbTile";
            this.pbTile.Size = new System.Drawing.Size(48, 33);
            this.pbTile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbTile.TabIndex = 0;
            this.pbTile.TabStop = false;
                                                appearance1.FontData.SizeInPoints = 13.97015F;
            appearance1.ForeColor = System.Drawing.Color.Black;
            appearance1.TextVAlignAsString = "Middle";
            this.lblTitleTile.Appearance = appearance1;
            this.lblTitleTile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitleTile.Location = new System.Drawing.Point(53, 3);
            this.lblTitleTile.Name = "lblTitleTile";
            this.lblTitleTile.ShortcutColor = System.Drawing.Color.Black;
            this.lblTitleTile.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.lblTitleTile.ShortcutKey = System.Windows.Forms.Keys.None;
            this.lblTitleTile.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.lblTitleTile.Size = new System.Drawing.Size(549, 29);
            this.lblTitleTile.TabIndex = 1;
            this.lblTitleTile.Text = "<TITOLO>";
            this.lblTitleTile.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
                                                appearance2.Image = global::UnicodeSrl.ScciCore.Properties.Resources.Tile_Expand;
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance2.TextHAlignAsString = "Center";
            appearance2.TextVAlignAsString = "Bottom";
            this.cmdWnd.Appearance = appearance2;
            this.cmdWnd.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.cmdWnd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdWnd.ImageSize = new System.Drawing.Size(21, 21);
            this.cmdWnd.Location = new System.Drawing.Point(608, 3);
            this.cmdWnd.Name = "cmdWnd";
            this.cmdWnd.PercImageFill = 0.75F;
            this.cmdWnd.ShortcutColor = System.Drawing.Color.Black;
            this.cmdWnd.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.cmdWnd.ShortcutKey = System.Windows.Forms.Keys.None;
            this.cmdWnd.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.cmdWnd.Size = new System.Drawing.Size(29, 29);
            this.cmdWnd.TabIndex = 2;
            this.cmdWnd.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions._undefined;
            this.cmdWnd.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            this.cmdWnd.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.cmdWnd.Click += new System.EventHandler(this.cmdWnd_Click);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.content);
            this.Controls.Add(this.tlpTile);
            this.Name = "BaseTileControl";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(650, 438);
            this.tlpTile.ResumeLayout(false);
            this.tlpTileTitle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbTile)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private ucEasyTableLayoutPanel tlpTile;
        private ucEasyTableLayoutPanel tlpTileTitle;
        private System.Windows.Forms.PictureBox pbTile;
        private ucEasyLabel lblTitleTile;
        private ucEasyButton cmdWnd;
        protected System.Windows.Forms.Panel content;
    }
}
