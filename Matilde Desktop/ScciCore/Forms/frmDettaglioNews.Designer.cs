namespace UnicodeSrl.ScciCore
{
    partial class frmDettaglioNews
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
            this.ultraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.ucRichTextBox = new UnicodeSrl.ScciCore.ucRichTextBox();
            this.lblDataOra = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.lblTitoloNews = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.UltraPanelLeft = new Infragistics.Win.Misc.UltraPanel();
            this.UltraGroupBoxLeft = new Infragistics.Win.Misc.UltraGroupBox();
            this.TableLayoutPanelLeft = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.PictureBox = new UnicodeSrl.ScciCore.ucEasyPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).BeginInit();
            this.ultraGroupBox.SuspendLayout();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            this.UltraPanelLeft.ClientArea.SuspendLayout();
            this.UltraPanelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBoxLeft)).BeginInit();
            this.UltraGroupBoxLeft.SuspendLayout();
            this.TableLayoutPanelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
            this.SuspendLayout();
                                                this.ucBottomModale.Size = new System.Drawing.Size(784, 24);
                                                this.ultraGroupBox.Controls.Add(this.ucEasyTableLayoutPanel);
            this.ultraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox.Location = new System.Drawing.Point(131, 24);
            this.ultraGroupBox.Name = "ultraGroupBox";
            this.ultraGroupBox.Size = new System.Drawing.Size(653, 514);
            this.ultraGroupBox.TabIndex = 9;
            this.ultraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.ucEasyTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyTableLayoutPanel.ColumnCount = 4;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 78F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucRichTextBox, 1, 3);
            this.ucEasyTableLayoutPanel.Controls.Add(this.lblDataOra, 1, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.lblTitoloNews, 2, 1);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 5;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 87F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(647, 508);
            this.ucEasyTableLayoutPanel.TabIndex = 0;
                                                this.ucEasyTableLayoutPanel.SetColumnSpan(this.ucRichTextBox, 2);
            this.ucRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucRichTextBox.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ucRichTextBox.Location = new System.Drawing.Point(6, 60);
            this.ucRichTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.ucRichTextBox.Name = "ucRichTextBox";
            this.ucRichTextBox.Size = new System.Drawing.Size(633, 441);
            this.ucRichTextBox.TabIndex = 0;
            this.ucRichTextBox.ViewFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ucRichTextBox.ViewReadOnly = true;
            this.ucRichTextBox.ViewRtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\nouicompat\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 " +
    "Microsoft Sans Serif;}}\r\n{\\*\\generator Riched20 10.0.10586}\\viewkind4\\uc1 \r\n\\par" +
    "d\\f0\\fs17\\par\r\n}\r\n";
            this.ucRichTextBox.ViewShowInsertImage = true;
            this.ucRichTextBox.ViewShowPlainText = true;
            this.ucRichTextBox.ViewShowToolbar = false;
            this.ucRichTextBox.ViewText = "";
            this.ucRichTextBox.ViewToolbarStyle = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2007;
            this.ucRichTextBox.ViewUseLargeImages = false;
            this.ucRichTextBox.RtfLinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.ucRichTextBox_RtfLinkClicked);
                                                appearance1.FontData.SizeInPoints = 20.1791F;
            appearance1.TextHAlignAsString = "Center";
            appearance1.TextVAlignAsString = "Middle";
            this.lblDataOra.Appearance = appearance1;
            this.lblDataOra.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataOra.Location = new System.Drawing.Point(9, 8);
            this.lblDataOra.Name = "lblDataOra";
            this.lblDataOra.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.lblDataOra.ShortcutKey = System.Windows.Forms.Keys.None;
            this.lblDataOra.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.lblDataOra.Size = new System.Drawing.Size(123, 44);
            this.lblDataOra.TabIndex = 1;
            this.lblDataOra.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
                                                appearance2.FontData.SizeInPoints = 37.25373F;
            appearance2.TextHAlignAsString = "Center";
            appearance2.TextVAlignAsString = "Middle";
            this.lblTitoloNews.Appearance = appearance2;
            this.lblTitoloNews.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitoloNews.Location = new System.Drawing.Point(138, 8);
            this.lblTitoloNews.Name = "lblTitoloNews";
            this.lblTitoloNews.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.lblTitoloNews.ShortcutKey = System.Windows.Forms.Keys.None;
            this.lblTitoloNews.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.lblTitoloNews.Size = new System.Drawing.Size(498, 44);
            this.lblTitoloNews.TabIndex = 1;
            this.lblTitoloNews.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.XLarge;
                                                                                    this.UltraPanelLeft.ClientArea.Controls.Add(this.UltraGroupBoxLeft);
            this.UltraPanelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.UltraPanelLeft.Location = new System.Drawing.Point(0, 24);
            this.UltraPanelLeft.Name = "UltraPanelLeft";
            this.UltraPanelLeft.Size = new System.Drawing.Size(131, 514);
            this.UltraPanelLeft.TabIndex = 13;
                                                this.UltraGroupBoxLeft.Controls.Add(this.TableLayoutPanelLeft);
            this.UltraGroupBoxLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UltraGroupBoxLeft.Location = new System.Drawing.Point(0, 0);
            this.UltraGroupBoxLeft.Margin = new System.Windows.Forms.Padding(0);
            this.UltraGroupBoxLeft.Name = "UltraGroupBoxLeft";
            this.UltraGroupBoxLeft.Size = new System.Drawing.Size(131, 514);
            this.UltraGroupBoxLeft.TabIndex = 1;
            this.UltraGroupBoxLeft.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.TableLayoutPanelLeft.BackColor = System.Drawing.Color.Transparent;
            this.TableLayoutPanelLeft.ColumnCount = 1;
            this.TableLayoutPanelLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanelLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanelLeft.Controls.Add(this.PictureBox, 0, 0);
            this.TableLayoutPanelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanelLeft.Location = new System.Drawing.Point(3, 3);
            this.TableLayoutPanelLeft.Name = "TableLayoutPanelLeft";
            this.TableLayoutPanelLeft.RowCount = 1;
            this.TableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 396F));
            this.TableLayoutPanelLeft.Size = new System.Drawing.Size(125, 508);
            this.TableLayoutPanelLeft.TabIndex = 0;
                                                this.PictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PictureBox.Location = new System.Drawing.Point(23, 221);
            this.PictureBox.Name = "PictureBox";
            this.PictureBox.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.PictureBox.ShortcutKey = System.Windows.Forms.Keys.None;
            this.PictureBox.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.PictureBox.Size = new System.Drawing.Size(78, 66);
            this.PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBox.TabIndex = 0;
            this.PictureBox.TabStop = false;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.ultraGroupBox);
            this.Controls.Add(this.UltraPanelLeft);
            this.Name = "frmDettaglioNews";
            this.PulsanteAvantiTesto = "CHIUDI";
            this.PulsanteAvantiVisibile = true;
            this.PulsanteIndietroVisibile = true;
            this.Text = "frmDettaglioNews";
            this.TopMost = true;
            this.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmDettaglioNews_PulsanteIndietroClick);
            this.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmDettaglioNews_PulsanteAvantiClick);
            this.Controls.SetChildIndex(this.UltraPanelLeft, 0);
            this.Controls.SetChildIndex(this.ultraGroupBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).EndInit();
            this.ultraGroupBox.ResumeLayout(false);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            this.UltraPanelLeft.ClientArea.ResumeLayout(false);
            this.UltraPanelLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBoxLeft)).EndInit();
            this.UltraGroupBoxLeft.ResumeLayout(false);
            this.TableLayoutPanelLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox;
        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        private ucRichTextBox ucRichTextBox;
        private ucEasyLabel lblDataOra;
        private ucEasyLabel lblTitoloNews;
        private Infragistics.Win.Misc.UltraPanel UltraPanelLeft;
        private Infragistics.Win.Misc.UltraGroupBox UltraGroupBoxLeft;
        private ucEasyTableLayoutPanel TableLayoutPanelLeft;
        private ucEasyPictureBox PictureBox;
    }
}