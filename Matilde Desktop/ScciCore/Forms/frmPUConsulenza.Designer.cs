namespace UnicodeSrl.ScciCore
{
    partial class frmPUConsulenza
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
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            this.ultraGroupBox = new Infragistics.Win.Misc.UltraGroupBox();
            this.ucEasyTableLayoutPanel = new UnicodeSrl.ScciCore.ucEasyTableLayoutPanel(this.components);
            this.lblConsulente = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.lblUserName = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.lblTipo = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.lblDataOra = new UnicodeSrl.ScciCore.ucEasyLabel();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.ehViewer = new System.Windows.Forms.Integration.ElementHost();
            this.ucDcViewer = new UnicodeSrl.WpfControls40.ucDcViewer();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).BeginInit();
            this.ultraGroupBox.SuspendLayout();
            this.ucEasyTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
                                                this.ultraGroupBox.Controls.Add(this.ucEasyTableLayoutPanel);
            this.ultraGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGroupBox.Location = new System.Drawing.Point(0, 24);
            this.ultraGroupBox.Name = "ultraGroupBox";
            this.ultraGroupBox.Size = new System.Drawing.Size(784, 514);
            this.ultraGroupBox.TabIndex = 0;
            this.ultraGroupBox.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2007;
                                                this.ucEasyTableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.ucEasyTableLayoutPanel.ColumnCount = 5;
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47F));
            this.ucEasyTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.Controls.Add(this.lblConsulente, 1, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.lblUserName, 1, 2);
            this.ucEasyTableLayoutPanel.Controls.Add(this.lblTipo, 3, 1);
            this.ucEasyTableLayoutPanel.Controls.Add(this.lblDataOra, 3, 2);
            this.ucEasyTableLayoutPanel.Controls.Add(this.ehViewer, 1, 4);
            this.ucEasyTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucEasyTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.ucEasyTableLayoutPanel.Name = "ucEasyTableLayoutPanel";
            this.ucEasyTableLayoutPanel.RowCount = 6;
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 1F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 83F));
            this.ucEasyTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.ucEasyTableLayoutPanel.Size = new System.Drawing.Size(778, 508);
            this.ucEasyTableLayoutPanel.TabIndex = 0;
                                                appearance1.FontData.SizeInPoints = 20F;
            appearance1.TextVAlignAsString = "Middle";
            this.lblConsulente.Appearance = appearance1;
            this.lblConsulente.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblConsulente.Location = new System.Drawing.Point(18, 13);
            this.lblConsulente.Name = "lblConsulente";
            this.lblConsulente.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.lblConsulente.ShortcutKey = System.Windows.Forms.Keys.None;
            this.lblConsulente.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.lblConsulente.Size = new System.Drawing.Size(359, 24);
            this.lblConsulente.TabIndex = 3;
            this.lblConsulente.Text = "Nome Consulente:";
            this.lblConsulente.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Large;
                                                appearance2.FontData.SizeInPoints = 20F;
            appearance2.TextVAlignAsString = "Middle";
            this.lblUserName.Appearance = appearance2;
            this.lblUserName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUserName.Location = new System.Drawing.Point(18, 43);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.lblUserName.ShortcutKey = System.Windows.Forms.Keys.None;
            this.lblUserName.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.lblUserName.Size = new System.Drawing.Size(359, 24);
            this.lblUserName.TabIndex = 3;
            this.lblUserName.Text = "Nome Utente Consulente:";
            this.lblUserName.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Large;
                                                appearance3.FontData.SizeInPoints = 20F;
            appearance3.TextVAlignAsString = "Middle";
            this.lblTipo.Appearance = appearance3;
            this.lblTipo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTipo.Location = new System.Drawing.Point(398, 13);
            this.lblTipo.Name = "lblTipo";
            this.lblTipo.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.lblTipo.ShortcutKey = System.Windows.Forms.Keys.None;
            this.lblTipo.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.lblTipo.Size = new System.Drawing.Size(359, 24);
            this.lblTipo.TabIndex = 3;
            this.lblTipo.Text = "Tipo di Consulenza:";
            this.lblTipo.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Large;
                                                appearance4.FontData.SizeInPoints = 20F;
            appearance4.TextVAlignAsString = "Middle";
            this.lblDataOra.Appearance = appearance4;
            this.lblDataOra.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataOra.Location = new System.Drawing.Point(398, 43);
            this.lblDataOra.Name = "lblDataOra";
            this.lblDataOra.ShortcutFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Small;
            this.lblDataOra.ShortcutKey = System.Windows.Forms.Keys.None;
            this.lblDataOra.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;
            this.lblDataOra.Size = new System.Drawing.Size(359, 24);
            this.lblDataOra.TabIndex = 3;
            this.lblDataOra.Text = "Data/Ora Inserimento Consulenza:";
            this.lblDataOra.TextFontRelativeDimension = UnicodeSrl.ScciCore.easyStatics.easyRelativeDimensions.Large;
                                                this.ultraToolTipManager1.ContainingControl = this;
                                                this.ucEasyTableLayoutPanel.SetColumnSpan(this.ehViewer, 3);
            this.ehViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ehViewer.Location = new System.Drawing.Point(18, 78);
            this.ehViewer.Name = "ehViewer";
            this.ehViewer.Size = new System.Drawing.Size(739, 415);
            this.ehViewer.TabIndex = 4;
            this.ehViewer.Text = "elementHost1";
            this.ehViewer.Child = this.ucDcViewer;
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.ultraGroupBox);
            this.Name = "frmPUConsulenza";
            this.PulsanteAvantiVisibile = true;
            this.PulsanteIndietroVisibile = true;
            this.Text = "frmPUConsulenza";
            this.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmPUConsulenza_PulsanteIndietroClick);
            this.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.frmPUConsulenza_PulsanteAvantiClick);
            this.Shown += new System.EventHandler(this.frmPUConsulenza_Shown);
            this.Controls.SetChildIndex(this.ultraGroupBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox)).EndInit();
            this.ultraGroupBox.ResumeLayout(false);
            this.ucEasyTableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox;
        private ucEasyTableLayoutPanel ucEasyTableLayoutPanel;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private ucEasyLabel lblConsulente;
        private ucEasyLabel lblUserName;
        private ucEasyLabel lblTipo;
        private ucEasyLabel lblDataOra;
        private System.Windows.Forms.Integration.ElementHost ehViewer;
        private UnicodeSrl.WpfControls40.ucDcViewer ucDcViewer;
    }
}