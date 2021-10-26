namespace UnicodeSrl.ScciCore
{
    partial class frmFotoPaziente
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFotoPaziente));
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.ultraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.ucPictureSelect = new UnicodeSrl.ScciCore.ucPictureSelect();
            this.ubCaricaDaFile = new UnicodeSrl.ScciCore.ucEasyButton();
            this.ubAcquisisci = new UnicodeSrl.ScciCore.ucEasyButton();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).BeginInit();
            this.ultraGroupBox.SuspendLayout();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
                                                this.ultraGroupBox.Controls.Add(this.ucEasyTableLayoutPanel);
            this.ultraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox.Location = new System.Drawing.Point(0, 30);
            this.ultraGroupBox.Name = "ultraGroupBox";
            this.ultraGroupBox.Size = new System.Drawing.Size(784, 472);
            this.ultraGroupBox.TabIndex = 9;
            this.ultraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.ucEasyTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyTableLayoutPanel.ColumnCount = 6;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucPictureSelect, 1, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ubCaricaDaFile, 3, 2);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ubAcquisisci, 3, 4);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 7;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(778, 466);
            this.ucEasyTableLayoutPanel.TabIndex = 0;
                                                this.ucPictureSelect.BackColor = System.Drawing.Color.Transparent;
            this.ucPictureSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucPictureSelect.Location = new System.Drawing.Point(10, 7);
            this.ucPictureSelect.Name = "ucPictureSelect";
            this.ucEasyTableLayoutPanel.SetRowSpan(this.ucPictureSelect, 5);
            this.ucPictureSelect.Size = new System.Drawing.Size(538, 449);
            this.ucPictureSelect.TabIndex = 0;
            this.ucPictureSelect.ViewCenterImage = false;
            this.ucPictureSelect.ViewCheckSquareImage = true;
            this.ucPictureSelect.ViewImage = null;
            this.ucPictureSelect.ViewOpenFileDialogFilter = "png files (*.png)|*.png";
            this.ucPictureSelect.ViewSaveFileDialogFilter = "png files (*.png)|*.png|jpg files (*.jpg)|*.jpg|bmp files (*.bmp)|*.bmp";
            this.ucPictureSelect.ViewShowAddImage = true;
            this.ucPictureSelect.ViewShowRemoveImage = true;
            this.ucPictureSelect.ViewShowSaveImage = true;
            this.ucPictureSelect.ViewShowToolbar = true;
            this.ucPictureSelect.ViewToolbarStyle = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2007;
            this.ucPictureSelect.ViewUseLargeImages = false;
            this.ucPictureSelect.ViewZoomFactor = UnicodeSrl.Scci.Enums.enumZoomfactor.zf100;
            this.ucPictureSelect.PictureChange += new UnicodeSrl.ScciCore.ChangeEventHandler(this.ucPictureSelect_PictureChange);
                                                appearance1.FontData.SizeInPoints = 18F;
            appearance1.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance1.ImageBackground")));
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance1.TextHAlignAsString = "Center";
            appearance1.TextVAlignAsString = "Middle";
            this.ubCaricaDaFile.Appearance = appearance1;
            this.ubCaricaDaFile.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ubCaricaDaFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ubCaricaDaFile.Location = new System.Drawing.Point(569, 100);
            this.ubCaricaDaFile.Name = "ubCaricaDaFile";
            this.ubCaricaDaFile.PercImageFill = 0.75F;
            this.ubCaricaDaFile.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ubCaricaDaFile.ShortcutKey = System.Windows.Forms.Keys.F;
            this.ubCaricaDaFile.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ubCaricaDaFile.Size = new System.Drawing.Size(180, 40);
            this.ubCaricaDaFile.TabIndex = 1;
            this.ubCaricaDaFile.Text = "Carica da File";
            this.ubCaricaDaFile.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ubCaricaDaFile.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ubCaricaDaFile.Click += new System.EventHandler(this.ubCaricaDaFile_Click);
                                                appearance2.FontData.SizeInPoints = 18F;
            appearance2.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance2.ImageBackground")));
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance2.TextHAlignAsString = "Center";
            appearance2.TextVAlignAsString = "Middle";
            this.ubAcquisisci.Appearance = appearance2;
            this.ubAcquisisci.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ubAcquisisci.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ubAcquisisci.Location = new System.Drawing.Point(569, 239);
            this.ubAcquisisci.Name = "ubAcquisisci";
            this.ubAcquisisci.PercImageFill = 0.75F;
            this.ubAcquisisci.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ubAcquisisci.ShortcutKey = System.Windows.Forms.Keys.A;
            this.ubAcquisisci.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ubAcquisisci.Size = new System.Drawing.Size(180, 40);
            this.ubAcquisisci.TabIndex = 2;
            this.ubAcquisisci.Text = "Acquisisci da Webcam";
            this.ubAcquisisci.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ubAcquisisci.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ubAcquisisci.Click += new System.EventHandler(this.ubAcquisisci_Click);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.ultraGroupBox);
            this.Name = "frmFotoPaziente";
            this.PulsanteAvantiVisibile = true;
            this.PulsanteIndietroVisibile = true;
            this.Text = "frmFotoPaziente";
            this.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmFotoPaziente_PulsanteIndietroClick);
            this.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmFotoPaziente_PulsanteAvantiClick);
            this.Shown += new System.EventHandler(this.frmFotoPaziente_Shown);
            this.Controls.SetChildIndex(this.ultraGroupBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).EndInit();
            this.ultraGroupBox.ResumeLayout(false);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox;
        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        private ucPictureSelect ucPictureSelect;
        private ucEasyButton ubCaricaDaFile;
        private ucEasyButton ubAcquisisci;
    }
}