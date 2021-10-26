namespace UnicodeSrl.ScciCore
{
    partial class ucUltraMonthViewMulti
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
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.ubSeleziona = new UnicodeSrl.ScciCore.ucEasyButton();
            this.UltraMonthViewMulti = new Infragistics.Win.UltraWinSchedule.UltraMonthViewMulti();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UltraMonthViewMulti)).BeginInit();
            this.SuspendLayout();
                                                this.ucEasyTableLayoutPanel.ColumnCount = 1;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.ubSeleziona, 0, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.UltraMonthViewMulti, 0, 0);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 2;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(346, 387);
            this.ucEasyTableLayoutPanel.TabIndex = 0;
                                                appearance1.FontData.SizeInPoints = 20F;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Top;
            appearance1.TextHAlignAsString = "Center";
            this.ubSeleziona.Appearance = appearance1;
            this.ubSeleziona.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ubSeleziona.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ubSeleziona.Location = new System.Drawing.Point(3, 312);
            this.ubSeleziona.Name = "ubSeleziona";
            this.ubSeleziona.PercImageFill = 0.75F;
            this.ubSeleziona.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ubSeleziona.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ubSeleziona.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ubSeleziona.Size = new System.Drawing.Size(340, 72);
            this.ubSeleziona.TabIndex = 1;
            this.ubSeleziona.Text = "CHIUDI";
            this.ubSeleziona.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Large;
            this.ubSeleziona.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ubSeleziona.Click += new System.EventHandler(this.ubSeleziona_Click);
                                                appearance2.FontData.BoldAsString = "True";
            appearance2.FontData.SizeInPoints = 20F;
            this.UltraMonthViewMulti.Appearance = appearance2;
            this.UltraMonthViewMulti.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UltraMonthViewMulti.Location = new System.Drawing.Point(3, 3);
            this.UltraMonthViewMulti.Name = "UltraMonthViewMulti";
            this.UltraMonthViewMulti.ResizeMode = Infragistics.Win.UltraWinSchedule.ResizeMode.UseMonthDimensions;
            this.UltraMonthViewMulti.Size = new System.Drawing.Size(340, 303);
            this.UltraMonthViewMulti.TabIndex = 0;
            this.UltraMonthViewMulti.BeforeMonthScroll += new Infragistics.Win.UltraWinSchedule.BeforeMonthScrollEventHandler(this.UltraMonthViewMulti_BeforeMonthScroll);
            this.UltraMonthViewMulti.VisibleMonthsChanged += new System.EventHandler(this.UltraMonthViewMulti_VisibleMonthsChanged);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.ucEasyTableLayoutPanel);
            this.Name = "ucUltraMonthViewMulti";
            this.Size = new System.Drawing.Size(346, 387);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UltraMonthViewMulti)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        private ucEasyButton ubSeleziona;
        public Infragistics.Win.UltraWinSchedule.UltraMonthViewMulti UltraMonthViewMulti;
    }
}
