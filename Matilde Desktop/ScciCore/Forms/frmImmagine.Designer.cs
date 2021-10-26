namespace UnicodeSrl.ScciCore
{
    partial class frmImmagine
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
            this.ultraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.ucPictureSelect = new UnicodeSrl.ScciCore.ucPictureSelect();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).BeginInit();
            this.ultraGroupBox.SuspendLayout();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
                                                this.ultraGroupBox.Controls.Add(this.ucEasyTableLayoutPanel);
            this.ultraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox.Location = new System.Drawing.Point(0, 23);
            this.ultraGroupBox.Name = "ultraGroupBox";
            this.ultraGroupBox.Size = new System.Drawing.Size(659, 493);
            this.ultraGroupBox.TabIndex = 10;
            this.ultraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.ucEasyTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyTableLayoutPanel.ColumnCount = 3;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 98F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucPictureSelect, 1, 1);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 3;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 98F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(653, 487);
            this.ucEasyTableLayoutPanel.TabIndex = 0;
                                                this.ucPictureSelect.BackColor = System.Drawing.Color.Transparent;
            this.ucPictureSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucPictureSelect.Location = new System.Drawing.Point(9, 7);
            this.ucPictureSelect.Name = "ucPictureSelect";
            this.ucPictureSelect.Size = new System.Drawing.Size(633, 471);
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
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 539);
            this.Controls.Add(this.ultraGroupBox);
            this.Name = "frmImmagine";
            this.PulsanteAvantiVisibile = true;
            this.PulsanteIndietroVisibile = true;
            this.Text = "frmImmagine";
            this.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmImmagine_PulsanteIndietroClick);
            this.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmImmagine_PulsanteAvantiClick);
            this.Shown += new System.EventHandler(this.frmImmagine_Shown);
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
    }
}