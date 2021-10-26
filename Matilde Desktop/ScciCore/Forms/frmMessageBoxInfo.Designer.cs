namespace UnicodeSrl.ScciCore
{
    partial class frmMessageBoxInfo
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
            this.ucEasyPictureBox = new UnicodeSrl.ScciCore.ucEasyPictureBox();
            this.ucEasyLabelTitolo = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.ucEasyTextBoxMessaggio = new UnicodeSrl.ScciCore.ucEasyTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).BeginInit();
            this.ultraGroupBox.SuspendLayout();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ucEasyPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ucEasyTextBoxMessaggio)).BeginInit();
            this.SuspendLayout();
                                                this.ucBottomModale.Size = new System.Drawing.Size(784, 24);
                                                this.ultraGroupBox.Controls.Add(this.ucEasyTableLayoutPanel);
            this.ultraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox.Location = new System.Drawing.Point(0, 24);
            this.ultraGroupBox.Name = "ultraGroupBox";
            this.ultraGroupBox.Size = new System.Drawing.Size(784, 514);
            this.ultraGroupBox.TabIndex = 11;
            this.ultraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.ucEasyTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyTableLayoutPanel.ColumnCount = 4;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 79F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyPictureBox, 0, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyLabelTitolo, 2, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyTextBoxMessaggio, 2, 2);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 4;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 86F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(778, 508);
            this.ucEasyTableLayoutPanel.TabIndex = 9;
                                                this.ucEasyPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ucEasyPictureBox.Location = new System.Drawing.Point(26, 31);
            this.ucEasyPictureBox.Margin = new System.Windows.Forms.Padding(26);
            this.ucEasyPictureBox.Name = "ucEasyPictureBox";
            this.ucEasyTableLayoutPanel.SetRowSpan(this.ucEasyPictureBox, 2);
            this.ucEasyPictureBox.ShortcutColor = System.Drawing.Color.Black;
            this.ucEasyPictureBox.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ucEasyPictureBox.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ucEasyPictureBox.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ucEasyPictureBox.Size = new System.Drawing.Size(88, 444);
            this.ucEasyPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ucEasyPictureBox.TabIndex = 0;
            this.ucEasyPictureBox.TabStop = false;
                                                appearance1.FontData.SizeInPoints = 41.91045F;
            appearance1.TextHAlignAsString = "Left";
            appearance1.TextVAlignAsString = "Middle";
            this.ucEasyLabelTitolo.Appearance = appearance1;
            this.ucEasyLabelTitolo.AutoEllipsis = false;
            this.ucEasyLabelTitolo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyLabelTitolo.Location = new System.Drawing.Point(152, 10);
            this.ucEasyLabelTitolo.Margin = new System.Windows.Forms.Padding(5);
            this.ucEasyLabelTitolo.Name = "ucEasyLabelTitolo";
            this.ucEasyLabelTitolo.ShortcutColor = System.Drawing.Color.Black;
            this.ucEasyLabelTitolo.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ucEasyLabelTitolo.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ucEasyLabelTitolo.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ucEasyLabelTitolo.Size = new System.Drawing.Size(604, 50);
            this.ucEasyLabelTitolo.TabIndex = 1;
            this.ucEasyLabelTitolo.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.XLarge;
                                                appearance2.FontData.SizeInPoints = 22.70149F;
            this.ucEasyTextBoxMessaggio.Appearance = appearance2;
            this.ucEasyTextBoxMessaggio.AutoSize = false;
            this.ucEasyTextBoxMessaggio.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2007;
            this.ucEasyTextBoxMessaggio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTextBoxMessaggio.Location = new System.Drawing.Point(152, 70);
            this.ucEasyTextBoxMessaggio.Margin = new System.Windows.Forms.Padding(5);
            this.ucEasyTextBoxMessaggio.Multiline = true;
            this.ucEasyTextBoxMessaggio.Name = "ucEasyTextBoxMessaggio";
            this.ucEasyTextBoxMessaggio.ReadOnly = true;
            this.ucEasyTextBoxMessaggio.Scrollbars = System.Windows.Forms.ScrollBars.Both;
            this.ucEasyTextBoxMessaggio.Size = new System.Drawing.Size(604, 426);
            this.ucEasyTextBoxMessaggio.TabIndex = 2;
            this.ucEasyTextBoxMessaggio.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ucEasyTextBoxMessaggio.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.ultraGroupBox);
            this.Name = "frmMessageBoxInfo";
            this.PulsanteAvantiVisibile = true;
            this.PulsanteIndietroVisibile = true;
            this.Text = "frmMessageBoxInfo";
            this.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmMessageBox_PulsanteIndietroClick);
            this.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmMessageBox_PulsanteAvantiClick);
            this.Shown += new System.EventHandler(this.frmMessageBoxInfo_Shown);
            this.Controls.SetChildIndex(this.ultraGroupBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).EndInit();
            this.ultraGroupBox.ResumeLayout(false);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ucEasyPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ucEasyTextBoxMessaggio)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox;
        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        private ucEasyPictureBox ucEasyPictureBox;
        private ucEasyLabel ucEasyLabelTitolo;
        private ucEasyTextBox ucEasyTextBoxMessaggio;
    }
}