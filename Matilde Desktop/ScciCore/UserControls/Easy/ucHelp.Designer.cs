namespace UnicodeSrl.ScciCore
{
    partial class ucHelp
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
            this.ultraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.ubChiudi = new UnicodeSrl.ScciCore.ucEasyButton();
            this.ucEasyTableLayoutPanelHelp = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.wbDescrizioneManuale = new System.Windows.Forms.WebBrowser();
            this.wbDescrizioneProdotto = new System.Windows.Forms.WebBrowser();
            this.pbLogoManuale = new System.Windows.Forms.PictureBox();
            this.pbLogoProdotto = new System.Windows.Forms.PictureBox();
            this.pbLogoFabbricatore = new System.Windows.Forms.PictureBox();
            this.wbDescrizioneFabbricatore = new System.Windows.Forms.WebBrowser();
            this.ucEasyLabelHeader1 = new UnicodeSrl.ScciCore.ucEasyLabelHeader();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).BeginInit();
            this.ultraGroupBox.SuspendLayout();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            this.ucEasyTableLayoutPanelHelp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogoManuale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogoProdotto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogoFabbricatore)).BeginInit();
            this.SuspendLayout();
                                                this.ultraGroupBox.Controls.Add(this.ucEasyTableLayoutPanel);
            this.ultraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox.Location = new System.Drawing.Point(0, 0);
            this.ultraGroupBox.Margin = new System.Windows.Forms.Padding(0);
            this.ultraGroupBox.Name = "ultraGroupBox";
            this.ultraGroupBox.Size = new System.Drawing.Size(589, 432);
            this.ultraGroupBox.TabIndex = 1;
            this.ultraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.ucEasyTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyTableLayoutPanel.ColumnCount = 2;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 128F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.ubChiudi, 1, 2);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyTableLayoutPanelHelp, 0, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyLabelHeader1, 0, 0);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.ucEasyTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 3;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(583, 426);
            this.ucEasyTableLayoutPanel.TabIndex = 1;
                                                appearance1.FontData.SizeInPoints = 13.56716F;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance1.TextHAlignAsString = "Center";
            appearance1.TextVAlignAsString = "Middle";
            this.ubChiudi.Appearance = appearance1;
            this.ubChiudi.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ubChiudi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ubChiudi.Location = new System.Drawing.Point(458, 381);
            this.ubChiudi.Name = "ubChiudi";
            this.ubChiudi.PercImageFill = 0.75F;
            this.ubChiudi.ShortcutColor = System.Drawing.Color.Black;
            this.ubChiudi.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.XSmall;
            this.ubChiudi.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ubChiudi.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ubChiudi.Size = new System.Drawing.Size(122, 42);
            this.ubChiudi.TabIndex = 2;
            this.ubChiudi.Text = "CHIUDI";
            this.ubChiudi.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ubChiudi.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ubChiudi.Click += new System.EventHandler(this.ubChiudi_Click);
                                                this.ucEasyTableLayoutPanelHelp.ColumnCount = 2;
            this.ucEasyTableLayoutPanel.SetColumnSpan(this.ucEasyTableLayoutPanelHelp, 2);
            this.ucEasyTableLayoutPanelHelp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.ucEasyTableLayoutPanelHelp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.ucEasyTableLayoutPanelHelp.Controls.Add(this.wbDescrizioneManuale, 1, 2);
            this.ucEasyTableLayoutPanelHelp.Controls.Add(this.wbDescrizioneProdotto, 1, 1);
            this.ucEasyTableLayoutPanelHelp.Controls.Add(this.pbLogoManuale, 0, 2);
            this.ucEasyTableLayoutPanelHelp.Controls.Add(this.pbLogoProdotto, 0, 1);
            this.ucEasyTableLayoutPanelHelp.Controls.Add(this.pbLogoFabbricatore, 0, 0);
            this.ucEasyTableLayoutPanelHelp.Controls.Add(this.wbDescrizioneFabbricatore, 1, 0);
            this.ucEasyTableLayoutPanelHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanelHelp.Location = new System.Drawing.Point(3, 51);
            this.ucEasyTableLayoutPanelHelp.Name = "ucEasyTableLayoutPanelHelp";
            this.ucEasyTableLayoutPanelHelp.RowCount = 3;
            this.ucEasyTableLayoutPanelHelp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.ucEasyTableLayoutPanelHelp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.ucEasyTableLayoutPanelHelp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.ucEasyTableLayoutPanelHelp.Size = new System.Drawing.Size(577, 324);
            this.ucEasyTableLayoutPanelHelp.TabIndex = 3;
                                                this.wbDescrizioneManuale.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbDescrizioneManuale.Location = new System.Drawing.Point(176, 217);
            this.wbDescrizioneManuale.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbDescrizioneManuale.Name = "wbDescrizioneManuale";
            this.wbDescrizioneManuale.Size = new System.Drawing.Size(398, 104);
            this.wbDescrizioneManuale.TabIndex = 5;
                                                this.wbDescrizioneProdotto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbDescrizioneProdotto.Location = new System.Drawing.Point(176, 110);
            this.wbDescrizioneProdotto.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbDescrizioneProdotto.Name = "wbDescrizioneProdotto";
            this.wbDescrizioneProdotto.Size = new System.Drawing.Size(398, 101);
            this.wbDescrizioneProdotto.TabIndex = 4;
                                                this.pbLogoManuale.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbLogoManuale.Location = new System.Drawing.Point(3, 217);
            this.pbLogoManuale.Name = "pbLogoManuale";
            this.pbLogoManuale.Size = new System.Drawing.Size(167, 104);
            this.pbLogoManuale.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbLogoManuale.TabIndex = 3;
            this.pbLogoManuale.TabStop = false;
                                                this.pbLogoProdotto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbLogoProdotto.Location = new System.Drawing.Point(3, 110);
            this.pbLogoProdotto.Name = "pbLogoProdotto";
            this.pbLogoProdotto.Size = new System.Drawing.Size(167, 101);
            this.pbLogoProdotto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbLogoProdotto.TabIndex = 2;
            this.pbLogoProdotto.TabStop = false;
                                                this.pbLogoFabbricatore.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbLogoFabbricatore.Location = new System.Drawing.Point(3, 3);
            this.pbLogoFabbricatore.Name = "pbLogoFabbricatore";
            this.pbLogoFabbricatore.Size = new System.Drawing.Size(167, 101);
            this.pbLogoFabbricatore.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbLogoFabbricatore.TabIndex = 0;
            this.pbLogoFabbricatore.TabStop = false;
                                                this.wbDescrizioneFabbricatore.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbDescrizioneFabbricatore.Location = new System.Drawing.Point(176, 3);
            this.wbDescrizioneFabbricatore.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbDescrizioneFabbricatore.Name = "wbDescrizioneFabbricatore";
            this.wbDescrizioneFabbricatore.Size = new System.Drawing.Size(398, 101);
            this.wbDescrizioneFabbricatore.TabIndex = 1;
                                                appearance2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(217)))), ((int)(((byte)(240)))));
            appearance2.FontData.SizeInPoints = 19.59702F;
            appearance2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(65)))), ((int)(((byte)(158)))));
            appearance2.TextVAlignAsString = "Middle";
            this.ucEasyLabelHeader1.Appearance = appearance2;
            this.ucEasyTableLayoutPanel.SetColumnSpan(this.ucEasyLabelHeader1, 2);
            this.ucEasyLabelHeader1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyLabelHeader1.Location = new System.Drawing.Point(3, 3);
            this.ucEasyLabelHeader1.Name = "ucEasyLabelHeader1";
            this.ucEasyLabelHeader1.ShortcutColor = System.Drawing.Color.Black;
            this.ucEasyLabelHeader1.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ucEasyLabelHeader1.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ucEasyLabelHeader1.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ucEasyLabelHeader1.Size = new System.Drawing.Size(577, 42);
            this.ucEasyLabelHeader1.TabIndex = 4;
            this.ucEasyLabelHeader1.Text = "Informazioni su Matilde";
            this.ucEasyLabelHeader1.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraGroupBox);
            this.Name = "ucHelp";
            this.Size = new System.Drawing.Size(589, 432);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).EndInit();
            this.ultraGroupBox.ResumeLayout(false);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            this.ucEasyTableLayoutPanelHelp.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbLogoManuale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogoProdotto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogoFabbricatore)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox;
        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        private ucEasyButton ubChiudi;
        private ucEasyTableLayoutPanel ucEasyTableLayoutPanelHelp;
        private System.Windows.Forms.PictureBox pbLogoFabbricatore;
        private System.Windows.Forms.WebBrowser wbDescrizioneFabbricatore;
        private System.Windows.Forms.PictureBox pbLogoManuale;
        private System.Windows.Forms.PictureBox pbLogoProdotto;
        private System.Windows.Forms.WebBrowser wbDescrizioneManuale;
        private System.Windows.Forms.WebBrowser wbDescrizioneProdotto;
        private ucEasyLabelHeader ucEasyLabelHeader1;
    }
}
