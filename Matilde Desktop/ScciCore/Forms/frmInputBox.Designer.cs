namespace UnicodeSrl.ScciCore
{
    partial class frmInputBox
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
            this.ucEasyPictureBox = new UnicodeSrl.ScciCore.ucEasyPictureBox();
            this.ucEasyLabel = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.ucEasyTextBox = new UnicodeSrl.ScciCore.ucEasyTextBox();
            this.ultraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ucEasyPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ucEasyTextBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).BeginInit();
            this.ultraGroupBox.SuspendLayout();
            this.SuspendLayout();
                                                this.ucBottomModale.Size = new System.Drawing.Size(784, 24);
                                                this.ucEasyTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyTableLayoutPanel.ColumnCount = 4;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 84F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyPictureBox, 0, 0);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyLabel, 2, 0);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyTextBox, 2, 1);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 3;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 58F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(778, 508);
            this.ucEasyTableLayoutPanel.TabIndex = 9;
                                                this.ucEasyPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ucEasyPictureBox.Location = new System.Drawing.Point(6, 30);
            this.ucEasyPictureBox.Margin = new System.Windows.Forms.Padding(6, 30, 6, 30);
            this.ucEasyPictureBox.Name = "ucEasyPictureBox";
            this.ucEasyTableLayoutPanel.SetRowSpan(this.ucEasyPictureBox, 3);
            this.ucEasyPictureBox.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ucEasyPictureBox.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ucEasyPictureBox.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ucEasyPictureBox.Size = new System.Drawing.Size(81, 448);
            this.ucEasyPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ucEasyPictureBox.TabIndex = 0;
            this.ucEasyPictureBox.TabStop = false;
                                                appearance1.FontData.SizeInPoints = 41.91045F;
            appearance1.TextHAlignAsString = "Left";
            appearance1.TextVAlignAsString = "Middle";
            this.ucEasyLabel.Appearance = appearance1;
            this.ucEasyLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyLabel.Location = new System.Drawing.Point(118, 10);
            this.ucEasyLabel.Margin = new System.Windows.Forms.Padding(10);
            this.ucEasyLabel.Name = "ucEasyLabel";
            this.ucEasyLabel.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ucEasyLabel.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ucEasyLabel.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ucEasyLabel.Size = new System.Drawing.Size(633, 183);
            this.ucEasyLabel.TabIndex = 1;
            this.ucEasyLabel.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.XLarge;
                                                appearance2.FontData.SizeInPoints = 41.91045F;
            appearance2.TextHAlignAsString = "Left";
            appearance2.TextVAlignAsString = "Top";
            this.ucEasyTextBox.Appearance = appearance2;
            this.ucEasyTextBox.AutoSize = false;
            this.ucEasyTextBox.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2007;
            this.ucEasyTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTextBox.Location = new System.Drawing.Point(111, 206);
            this.ucEasyTextBox.Multiline = true;
            this.ucEasyTextBox.Name = "ucEasyTextBox";
            this.ucEasyTextBox.Scrollbars = System.Windows.Forms.ScrollBars.Both;
            this.ucEasyTextBox.Size = new System.Drawing.Size(647, 288);
            this.ucEasyTextBox.TabIndex = 2;
            this.ucEasyTextBox.Text = "ABC DEF";
            this.ucEasyTextBox.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.XLarge;
            this.ucEasyTextBox.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ucEasyTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ucEasyTextBox_KeyDown);
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
            this.Name = "frmInputBox";
            this.PulsanteAvantiVisibile = true;
            this.PulsanteIndietroVisibile = true;
            this.Text = "frmInputBox";
            this.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmInputBox_PulsanteIndietroClick);
            this.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmInputBox_PulsanteAvantiClick);
            this.Controls.SetChildIndex(this.ultraGroupBox, 0);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ucEasyPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ucEasyTextBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).EndInit();
            this.ultraGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox;
        private ucEasyPictureBox ucEasyPictureBox;
        private ucEasyLabel ucEasyLabel;
        private ucEasyTextBox ucEasyTextBox;
    }
}