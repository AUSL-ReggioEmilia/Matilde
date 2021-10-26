namespace UnicodeSrl.ScciCore
{
    partial class frmWebBrowser
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
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            this.UltraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.ubStop = new UnicodeSrl.ScciCore.ucEasyButton();
            this.ubRefresh = new UnicodeSrl.ScciCore.ucEasyButton();
            this.ubForward = new UnicodeSrl.ScciCore.ucEasyButton();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.ubBackward = new UnicodeSrl.ScciCore.ucEasyButton();
            this.pbAnim = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBox)).BeginInit();
            this.UltraGroupBox.SuspendLayout();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAnim)).BeginInit();
            this.SuspendLayout();
                                                this.UltraGroupBox.Controls.Add(this.ucEasyTableLayoutPanel);
            this.UltraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UltraGroupBox.Location = new System.Drawing.Point(0, 24);
            this.UltraGroupBox.Margin = new System.Windows.Forms.Padding(0);
            this.UltraGroupBox.Name = "UltraGroupBox";
            this.UltraGroupBox.Size = new System.Drawing.Size(784, 514);
            this.UltraGroupBox.TabIndex = 11;
            this.UltraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.ucEasyTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyTableLayoutPanel.ColumnCount = 6;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.ubStop, 3, 0);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ubRefresh, 2, 0);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ubForward, 1, 0);
            this.ucEasyTableLayoutPanel.Controls.Add(this.webBrowser, 0, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ubBackward, 0, 0);
            this.ucEasyTableLayoutPanel.Controls.Add(this.pbAnim, 5, 0);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 2;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(778, 508);
            this.ucEasyTableLayoutPanel.TabIndex = 0;
                                                appearance5.FontData.SizeInPoints = 20.43361F;
            appearance5.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance5.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance5.TextHAlignAsString = "Center";
            appearance5.TextVAlignAsString = "Bottom";
            this.ubStop.Appearance = appearance5;
            this.ubStop.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ubStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ubStop.Location = new System.Drawing.Point(303, 3);
            this.ubStop.Name = "ubStop";
            this.ubStop.PercImageFill = 0.75F;
            this.ubStop.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ubStop.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ubStop.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ubStop.Size = new System.Drawing.Size(94, 94);
            this.ubStop.TabIndex = 4;
            this.ubStop.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ubStop.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ubStop.Click += new System.EventHandler(this.ubStop_Click);
                                                appearance6.FontData.SizeInPoints = 20.43361F;
            appearance6.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance6.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance6.TextHAlignAsString = "Center";
            appearance6.TextVAlignAsString = "Bottom";
            this.ubRefresh.Appearance = appearance6;
            this.ubRefresh.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ubRefresh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ubRefresh.Location = new System.Drawing.Point(203, 3);
            this.ubRefresh.Name = "ubRefresh";
            this.ubRefresh.PercImageFill = 0.75F;
            this.ubRefresh.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ubRefresh.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ubRefresh.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ubRefresh.Size = new System.Drawing.Size(94, 94);
            this.ubRefresh.TabIndex = 3;
            this.ubRefresh.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ubRefresh.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ubRefresh.Click += new System.EventHandler(this.ubRefresh_Click);
                                                appearance7.FontData.SizeInPoints = 20.43361F;
            appearance7.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance7.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance7.TextHAlignAsString = "Center";
            appearance7.TextVAlignAsString = "Bottom";
            this.ubForward.Appearance = appearance7;
            this.ubForward.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ubForward.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ubForward.Location = new System.Drawing.Point(103, 3);
            this.ubForward.Name = "ubForward";
            this.ubForward.PercImageFill = 0.75F;
            this.ubForward.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ubForward.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ubForward.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ubForward.Size = new System.Drawing.Size(94, 94);
            this.ubForward.TabIndex = 2;
            this.ubForward.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ubForward.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ubForward.Click += new System.EventHandler(this.ubForward_Click);
                                                this.ucEasyTableLayoutPanel.SetColumnSpan(this.webBrowser, 6);
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(3, 103);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.ScriptErrorsSuppressed = true;
            this.webBrowser.Size = new System.Drawing.Size(772, 402);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.WebBrowserShortcutsEnabled = false;
            this.webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
            this.webBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowser_Navigating);
            this.webBrowser.NewWindow += new System.ComponentModel.CancelEventHandler(this.webBrowser_NewWindow);
                                                appearance8.FontData.SizeInPoints = 20.43361F;
            appearance8.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance8.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance8.TextHAlignAsString = "Center";
            appearance8.TextVAlignAsString = "Bottom";
            this.ubBackward.Appearance = appearance8;
            this.ubBackward.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ubBackward.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ubBackward.Location = new System.Drawing.Point(3, 3);
            this.ubBackward.Name = "ubBackward";
            this.ubBackward.PercImageFill = 0.75F;
            this.ubBackward.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ubBackward.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ubBackward.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ubBackward.Size = new System.Drawing.Size(94, 94);
            this.ubBackward.TabIndex = 1;
            this.ubBackward.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ubBackward.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ubBackward.Click += new System.EventHandler(this.ubBackward_Click);
                                                this.pbAnim.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbAnim.Image = global::UnicodeSrl.ScciCore.Properties.Resources.LoadingAnim;
            this.pbAnim.Location = new System.Drawing.Point(681, 3);
            this.pbAnim.Name = "pbAnim";
            this.pbAnim.Size = new System.Drawing.Size(94, 94);
            this.pbAnim.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbAnim.TabIndex = 5;
            this.pbAnim.TabStop = false;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.UltraGroupBox);
            this.Name = "frmWebBrowser";
            this.PulsanteAvantiVisibile = true;
            this.PulsanteIndietroVisibile = true;
            this.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmWebBrowser_PulsanteAvantiClick);
            this.Load += new System.EventHandler(this.frmWebBrowser_Load);
            this.Controls.SetChildIndex(this.UltraGroupBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBox)).EndInit();
            this.UltraGroupBox.ResumeLayout(false);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbAnim)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox UltraGroupBox;
        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        private System.Windows.Forms.WebBrowser webBrowser;
        private ucEasyButton ubForward;
        private ucEasyButton ubBackward;
        private ucEasyButton ubStop;
        private ucEasyButton ubRefresh;
        private System.Windows.Forms.PictureBox pbAnim;
    }
}
