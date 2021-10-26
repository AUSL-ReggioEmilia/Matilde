namespace UnicodeSrl.ScciCore
{
    partial class frmTestiTipo
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
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTestiTipo));
            this.ultraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.utvTesti = new UnicodeSrl.ScciCore.ucEasyTreeView();
            this.lblAnteprima = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.lblTesto = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.ubIncolla = new UnicodeSrl.ScciCore.ucEasyButton();
            this.rtfAnteprima = new UnicodeSrl.ScciCore.ucRichTextBox();
            this.rtfTesto = new UnicodeSrl.ScciCore.ucRichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).BeginInit();
            this.ultraGroupBox.SuspendLayout();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utvTesti)).BeginInit();
            this.SuspendLayout();
                                                this.ultraGroupBox.Controls.Add(this.ucEasyTableLayoutPanel);
            this.ultraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox.Location = new System.Drawing.Point(0, 30);
            this.ultraGroupBox.Name = "ultraGroupBox";
            this.ultraGroupBox.Size = new System.Drawing.Size(784, 472);
            this.ultraGroupBox.TabIndex = 9;
            this.ultraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.ucEasyTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyTableLayoutPanel.ColumnCount = 5;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.utvTesti, 1, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.lblAnteprima, 3, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.lblTesto, 1, 4);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ubIncolla, 3, 3);
            this.ucEasyTableLayoutPanel.Controls.Add(this.rtfAnteprima, 3, 2);
            this.ucEasyTableLayoutPanel.Controls.Add(this.rtfTesto, 1, 5);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 7;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(778, 466);
            this.ucEasyTableLayoutPanel.TabIndex = 0;
            this.ucEasyTableLayoutPanel.TabStop = true;
                                                this.utvTesti.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.utvTesti.DisplayStyle = Infragistics.Win.UltraWinTree.UltraTreeDisplayStyle.WindowsVista;
            this.utvTesti.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utvTesti.HideSelection = false;
            this.utvTesti.Location = new System.Drawing.Point(10, 7);
            this.utvTesti.Name = "utvTesti";
            this.ucEasyTableLayoutPanel.SetRowSpan(this.utvTesti, 3);
            this.utvTesti.Size = new System.Drawing.Size(367, 193);
            this.utvTesti.TabIndex = 0;
            this.utvTesti.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions._undefined;
            this.utvTesti.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.utvTesti.AfterActivate += new Infragistics.Win.UltraWinTree.AfterNodeChangedEventHandler(this.utvTesti_AfterActivate);
                                                appearance1.FontData.SizeInPoints = 18F;
            appearance1.TextVAlignAsString = "Bottom";
            this.lblAnteprima.Appearance = appearance1;
            this.lblAnteprima.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAnteprima.Location = new System.Drawing.Point(398, 7);
            this.lblAnteprima.Name = "lblAnteprima";
            this.lblAnteprima.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.lblAnteprima.ShortcutKey = System.Windows.Forms.Keys.None;
            this.lblAnteprima.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.lblAnteprima.Size = new System.Drawing.Size(367, 17);
            this.lblAnteprima.TabIndex = 1;
            this.lblAnteprima.Text = "Anteprima:";
            this.lblAnteprima.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
                                                appearance2.FontData.SizeInPoints = 18F;
            appearance2.TextVAlignAsString = "Bottom";
            this.lblTesto.Appearance = appearance2;
            this.lblTesto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTesto.Location = new System.Drawing.Point(10, 206);
            this.lblTesto.Name = "lblTesto";
            this.lblTesto.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.lblTesto.ShortcutKey = System.Windows.Forms.Keys.None;
            this.lblTesto.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.lblTesto.Size = new System.Drawing.Size(367, 17);
            this.lblTesto.TabIndex = 2;
            this.lblTesto.Text = "Testo:";
            this.lblTesto.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
                                                appearance3.FontData.SizeInPoints = 24F;
            appearance3.ImageBackground = ((System.Drawing.Image)(resources.GetObject("appearance3.ImageBackground")));
            appearance3.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance3.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance3.TextHAlignAsString = "Center";
            appearance3.TextVAlignAsString = "Middle";
            this.ubIncolla.Appearance = appearance3;
            this.ubIncolla.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ubIncolla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ubIncolla.Location = new System.Drawing.Point(398, 169);
            this.ubIncolla.Name = "ubIncolla";
            this.ubIncolla.PercImageFill = 0.75F;
            this.ubIncolla.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ubIncolla.ShortcutKey = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.ubIncolla.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ubIncolla.Size = new System.Drawing.Size(367, 31);
            this.ubIncolla.TabIndex = 1;
            this.ubIncolla.Text = "INCOLLA";
            this.ubIncolla.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Large;
            this.ubIncolla.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ubIncolla.Click += new System.EventHandler(this.ubIncolla_Click);
                                                this.rtfAnteprima.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtfAnteprima.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtfAnteprima.Location = new System.Drawing.Point(398, 30);
            this.rtfAnteprima.Name = "rtfAnteprima";
            this.rtfAnteprima.Size = new System.Drawing.Size(367, 133);
            this.rtfAnteprima.TabIndex = 3;
            this.rtfAnteprima.ViewFont = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtfAnteprima.ViewReadOnly = true;
            this.rtfAnteprima.ViewRtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1040{\\fonttbl{\\f0\\fnil\\fcharset0 Tahoma;}}\r\n" +
    "\\viewkind4\\uc1\\pard\\f0\\fs17\\par\r\n}\r\n";
            this.rtfAnteprima.ViewShowInsertImage = true;
            this.rtfAnteprima.ViewShowToolbar = false;
            this.rtfAnteprima.ViewText = "";
            this.rtfAnteprima.ViewToolbarStyle = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2007;
            this.rtfAnteprima.ViewUseLargeImages = false;
                                                this.ucEasyTableLayoutPanel.SetColumnSpan(this.rtfTesto, 3);
            this.rtfTesto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtfTesto.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtfTesto.Location = new System.Drawing.Point(10, 229);
            this.rtfTesto.Name = "rtfTesto";
            this.rtfTesto.Size = new System.Drawing.Size(755, 227);
            this.rtfTesto.TabIndex = 2;
            this.rtfTesto.ViewFont = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtfTesto.ViewReadOnly = false;
            this.rtfTesto.ViewRtf = "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1040{\\fonttbl{\\f0\\fnil\\fcharset0 Tahoma;}}\r\n" +
    "\\viewkind4\\uc1\\pard\\f0\\fs17\\par\r\n}\r\n";
            this.rtfTesto.ViewShowInsertImage = true;
            this.rtfTesto.ViewShowToolbar = true;
            this.rtfTesto.ViewText = "";
            this.rtfTesto.ViewToolbarStyle = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2007;
            this.rtfTesto.ViewUseLargeImages = false;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.ultraGroupBox);
            this.Name = "frmTestiTipo";
            this.PulsanteAvantiVisibile = true;
            this.PulsanteIndietroVisibile = true;
            this.Text = "frmTestiTipo";
            this.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmTestiTipo_PulsanteIndietroClick);
            this.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmTestiTipo_PulsanteAvantiClick);
            this.Controls.SetChildIndex(this.ultraGroupBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).EndInit();
            this.ultraGroupBox.ResumeLayout(false);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.utvTesti)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox;
        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        private ucEasyTreeView utvTesti;
        private ucEasyLabel lblAnteprima;
        private ucEasyLabel lblTesto;
        private ucEasyButton ubIncolla;
        private ucRichTextBox rtfAnteprima;
        private ucRichTextBox rtfTesto;
    }
}