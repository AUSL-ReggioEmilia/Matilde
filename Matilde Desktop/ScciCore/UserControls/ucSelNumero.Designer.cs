namespace UnicodeSrl.ScciCore
{
    partial class ucSelNumero
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
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            this.UltraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.ucEasyButtonDown = new UnicodeSrl.ScciCore.ucEasyButton();
            this.ucEasyNumericEditor = new UnicodeSrl.ScciCore.ucEasyNumericEditor();
            this.ucEasyButtonUp = new UnicodeSrl.ScciCore.ucEasyButton();
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBox)).BeginInit();
            this.UltraGroupBox.SuspendLayout();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ucEasyNumericEditor)).BeginInit();
            this.SuspendLayout();
                                                this.UltraGroupBox.Controls.Add(this.ucEasyTableLayoutPanel);
            this.UltraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UltraGroupBox.Location = new System.Drawing.Point(0, 0);
            this.UltraGroupBox.Name = "UltraGroupBox";
            this.UltraGroupBox.Size = new System.Drawing.Size(170, 50);
            this.UltraGroupBox.TabIndex = 0;
            this.UltraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.ucEasyTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyTableLayoutPanel.ColumnCount = 2;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyButtonDown, 1, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyNumericEditor, 0, 0);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ucEasyButtonUp, 1, 0);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 2;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(164, 44);
            this.ucEasyTableLayoutPanel.TabIndex = 0;
                                                appearance1.Image = global::UnicodeSrl.ScciCore.Properties.Resources.arrow_down;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance1.TextHAlignAsString = "Center";
            appearance1.TextVAlignAsString = "Bottom";
            this.ucEasyButtonDown.Appearance = appearance1;
            this.ucEasyButtonDown.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ucEasyButtonDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyButtonDown.Location = new System.Drawing.Point(114, 22);
            this.ucEasyButtonDown.Margin = new System.Windows.Forms.Padding(0);
            this.ucEasyButtonDown.Name = "ucEasyButtonDown";
            this.ucEasyButtonDown.PercImageFill = 0.75F;
            this.ucEasyButtonDown.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ucEasyButtonDown.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ucEasyButtonDown.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ucEasyButtonDown.Size = new System.Drawing.Size(50, 22);
            this.ucEasyButtonDown.TabIndex = 2;
            this.ucEasyButtonDown.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions._undefined;
            this.ucEasyButtonDown.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ucEasyButtonDown.Click += new System.EventHandler(this.ucEasyButtonDown_Click);
                                                appearance2.FontData.SizeInPoints = 16.25F;
            this.ucEasyNumericEditor.Appearance = appearance2;
            this.ucEasyNumericEditor.AutoSize = false;
            this.ucEasyNumericEditor.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.Office2007;
            this.ucEasyNumericEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyNumericEditor.Location = new System.Drawing.Point(0, 0);
            this.ucEasyNumericEditor.Margin = new System.Windows.Forms.Padding(0);
            this.ucEasyNumericEditor.MaskInput = "-nnnnnnnnn";
            this.ucEasyNumericEditor.Name = "ucEasyNumericEditor";
            this.ucEasyNumericEditor.PromptChar = ' ';
            this.ucEasyTableLayoutPanel.SetRowSpan(this.ucEasyNumericEditor, 2);
            this.ucEasyNumericEditor.Size = new System.Drawing.Size(114, 44);
            this.ucEasyNumericEditor.TabIndex = 0;
            this.ucEasyNumericEditor.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Medium;
            this.ucEasyNumericEditor.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ucEasyNumericEditor.ValueChanged += new System.EventHandler(this.ucEasyNumericEditor_ValueChanged);
                                                appearance3.Image = global::UnicodeSrl.ScciCore.Properties.Resources.arrow_up;
            appearance3.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance3.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance3.TextHAlignAsString = "Center";
            appearance3.TextVAlignAsString = "Bottom";
            this.ucEasyButtonUp.Appearance = appearance3;
            this.ucEasyButtonUp.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2003ToolbarButton;
            this.ucEasyButtonUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyButtonUp.Location = new System.Drawing.Point(114, 0);
            this.ucEasyButtonUp.Margin = new System.Windows.Forms.Padding(0);
            this.ucEasyButtonUp.Name = "ucEasyButtonUp";
            this.ucEasyButtonUp.PercImageFill = 0.75F;
            this.ucEasyButtonUp.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.ucEasyButtonUp.ShortcutKey = System.Windows.Forms.Keys.None;
            this.ucEasyButtonUp.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.ucEasyButtonUp.Size = new System.Drawing.Size(50, 22);
            this.ucEasyButtonUp.TabIndex = 1;
            this.ucEasyButtonUp.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions._undefined;
            this.ucEasyButtonUp.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ucEasyButtonUp.Click += new System.EventHandler(this.ucEasyButtonUp_Click);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.UltraGroupBox);
            this.Name = "ucSelNumero";
            this.Size = new System.Drawing.Size(170, 50);
            ((System.ComponentModel.ISupportInitialize)(this.UltraGroupBox)).EndInit();
            this.UltraGroupBox.ResumeLayout(false);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ucEasyNumericEditor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox UltraGroupBox;
        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        private ucEasyButton ucEasyButtonDown;
        private ucEasyNumericEditor ucEasyNumericEditor;
        private ucEasyButton ucEasyButtonUp;

    }
}
