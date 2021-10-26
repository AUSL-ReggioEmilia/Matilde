namespace UnicodeSrl.ScciCore
{
    partial class frmMessageBox
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
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.ucEasyLabelErrore = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.ucEasyPictureBox = new UnicodeSrl.ScciCore.ucEasyPictureBox();
            this.ucEasyLabel = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.ultraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ucEasyPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).BeginInit();
            this.ultraGroupBox.SuspendLayout();
            this.SuspendLayout();
                                                this.ucEasyTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyTableLayoutPanel.ColumnCount = 4;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 76F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyLabelErrore, 2, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyPictureBox, 0, 0);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyLabel, 2, 0);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 2;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(778, 508);
            this.ucEasyTableLayoutPanel.TabIndex = 9;
                                                appearance1.FontData.SizeInPoints = 11.25F;
            appearance1.TextHAlignAsString = "Left";
            appearance1.TextVAlignAsString = "Top";
            this.ucEasyLabelErrore.Appearance = appearance1;
            this.ucEasyLabelErrore.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyLabelErrore.Location = new System.Drawing.Point(180, 365);
            this.ucEasyLabelErrore.Margin = new System.Windows.Forms.Padding(10);
            this.ucEasyLabelErrore.Name = "ucEasyLabelErrore";
            this.ucEasyLabelErrore.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ucEasyLabelErrore.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ucEasyLabelErrore.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ucEasyLabelErrore.Size = new System.Drawing.Size(571, 133);
            this.ucEasyLabelErrore.TabIndex = 2;
            this.ucEasyLabelErrore.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
                                                this.ucEasyPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ucEasyPictureBox.Location = new System.Drawing.Point(30, 30);
            this.ucEasyPictureBox.Margin = new System.Windows.Forms.Padding(30);
            this.ucEasyPictureBox.Name = "ucEasyPictureBox";
            this.ucEasyTableLayoutPanel.SetRowSpan(this.ucEasyPictureBox, 2);
            this.ucEasyPictureBox.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ucEasyPictureBox.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ucEasyPictureBox.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ucEasyPictureBox.Size = new System.Drawing.Size(95, 448);
            this.ucEasyPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ucEasyPictureBox.TabIndex = 0;
            this.ucEasyPictureBox.TabStop = false;
                                                appearance2.FontData.SizeInPoints = 30F;
            appearance2.TextHAlignAsString = "Left";
            appearance2.TextVAlignAsString = "Middle";
            this.ucEasyLabel.Appearance = appearance2;
            this.ucEasyLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyLabel.Location = new System.Drawing.Point(180, 10);
            this.ucEasyLabel.Margin = new System.Windows.Forms.Padding(10);
            this.ucEasyLabel.Name = "ucEasyLabel";
            this.ucEasyLabel.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ucEasyLabel.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ucEasyLabel.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ucEasyLabel.Size = new System.Drawing.Size(571, 335);
            this.ucEasyLabel.TabIndex = 1;
            this.ucEasyLabel.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.XLarge;
                                                this.ultraGroupBox.Controls.Add(this.ucEasyTableLayoutPanel);
            this.ultraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox.Location = new System.Drawing.Point(0, 24);
            this.ultraGroupBox.Name = "ultraGroupBox";
            this.ultraGroupBox.Size = new System.Drawing.Size(784, 514);
            this.ultraGroupBox.TabIndex = 10;
            this.ultraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.ultraGroupBox);
            this.Name = "frmMessageBox";
            this.PulsanteAvantiVisibile = true;
            this.PulsanteIndietroVisibile = true;
            this.Text = "frmMessageBox";
            this.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmMessageBox_PulsanteIndietroClick);
            this.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmMessageBox_PulsanteAvantiClick);
            this.Controls.SetChildIndex(this.ultraGroupBox, 0);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ucEasyPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).EndInit();
            this.ultraGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox;
        private ucEasyPictureBox ucEasyPictureBox;
        private ucEasyLabel ucEasyLabel;
        private ucEasyLabel ucEasyLabelErrore;
    }
}