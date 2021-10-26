namespace UnicodeSrl.ScciCore
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.UltraPanelTop = new Infragistics.Win.Misc.UltraPanel();
            this.ucTop = new UnicodeSrl.ScciCore.ucTop();
            this.UltraPanelBottom = new Infragistics.Win.Misc.UltraPanel();
            this.ucBottom = new UnicodeSrl.ScciCore.ucBottom();
            this.UltraTabControlMenu = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.UltraPopupControlContainerSegnalibri = new Infragistics.Win.Misc.UltraPopupControlContainer(this.components);
            this.UltraPopupControlContainerCartelleInVisione = new Infragistics.Win.Misc.UltraPopupControlContainer(this.components);
            this.UltraPopupControlContainerPazientiSeguiti = new Infragistics.Win.Misc.UltraPopupControlContainer(this.components);
            this.UltraPopupControlContainerPazientiInVisione = new Infragistics.Win.Misc.UltraPopupControlContainer(this.components);
            this.UltraPopupControlContainerHelp = new Infragistics.Win.Misc.UltraPopupControlContainer(this.components);
            this.UltraPanelTop.ClientArea.SuspendLayout();
            this.UltraPanelTop.SuspendLayout();
            this.UltraPanelBottom.ClientArea.SuspendLayout();
            this.UltraPanelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UltraTabControlMenu)).BeginInit();
            this.UltraTabControlMenu.SuspendLayout();
            this.SuspendLayout();
                                                this.UltraPanelTop.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
                                                this.UltraPanelTop.ClientArea.Controls.Add(this.ucTop);
            this.UltraPanelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.UltraPanelTop.Location = new System.Drawing.Point(0, 0);
            this.UltraPanelTop.Margin = new System.Windows.Forms.Padding(0);
            this.UltraPanelTop.Name = "UltraPanelTop";
            this.UltraPanelTop.Size = new System.Drawing.Size(1008, 100);
            this.UltraPanelTop.TabIndex = 5;
                                                this.ucTop.BackColor = System.Drawing.Color.SteelBlue;
            this.ucTop.CodiceMaschera = "";
            this.ucTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucTop.Location = new System.Drawing.Point(0, 0);
            this.ucTop.Margin = new System.Windows.Forms.Padding(0);
            this.ucTop.Name = "ucTop";
            this.ucTop.Size = new System.Drawing.Size(1008, 100);
            this.ucTop.TabIndex = 0;
            this.ucTop.ImmagineClick += new UnicodeSrl.ScciCore.ImmagineTopClickHandler(this.ucTop_ImmagineClick);
                                                this.UltraPanelBottom.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
                                                this.UltraPanelBottom.ClientArea.Controls.Add(this.ucBottom);
            this.UltraPanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.UltraPanelBottom.Location = new System.Drawing.Point(0, 622);
            this.UltraPanelBottom.Margin = new System.Windows.Forms.Padding(0);
            this.UltraPanelBottom.Name = "UltraPanelBottom";
            this.UltraPanelBottom.Size = new System.Drawing.Size(1008, 60);
            this.UltraPanelBottom.TabIndex = 6;
                                                this.ucBottom.BackColor = System.Drawing.Color.SteelBlue;
            this.ucBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucBottom.Location = new System.Drawing.Point(0, 0);
            this.ucBottom.Margin = new System.Windows.Forms.Padding(0);
            this.ucBottom.MenuPulsanteAvanti = ((System.Collections.Generic.IDictionary<string, object>)(resources.GetObject("ucBottom.MenuPulsanteAvanti")));
            this.ucBottom.MenuPulsanteAvantiCDSS = ((System.Collections.Generic.IDictionary<string, object>)(resources.GetObject("ucBottom.MenuPulsanteAvantiCDSS")));
            this.ucBottom.MenuPulsanteAvantiEnabled = true;
            this.ucBottom.Name = "ucBottom";
            this.ucBottom.Size = new System.Drawing.Size(1008, 60);
            this.ucBottom.TabIndex = 0;
            this.ucBottom.PulsanteIndietroClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.ucBottom_PulsanteIndietroClick);
            this.ucBottom.PulsanteAvantiClick += new UnicodeSrl.ScciCore.PulsanteBottomClickHandler(this.ucBottom_PulsanteAvantiClick);
            this.ucBottom.PulsanteAvantiToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.ucBottom_PulsanteAvantiToolClick);
            this.ucBottom.ImmagineClick += new UnicodeSrl.ScciCore.ImmagineBottomClickHandler(this.ucBottom_ImmagineClick);
                                                this.UltraTabControlMenu.Controls.Add(this.ultraTabSharedControlsPage1);
            this.UltraTabControlMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UltraTabControlMenu.Location = new System.Drawing.Point(0, 100);
            this.UltraTabControlMenu.Name = "UltraTabControlMenu";
            this.UltraTabControlMenu.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.UltraTabControlMenu.Size = new System.Drawing.Size(1008, 522);
            this.UltraTabControlMenu.TabIndex = 8;
                                                this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(1, 20);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(1004, 499);
                                                this.UltraPopupControlContainerSegnalibri.Opening += new System.ComponentModel.CancelEventHandler(this.UltraPopupControlContainerSegnalibri_Opening);
            this.UltraPopupControlContainerSegnalibri.Opened += new System.EventHandler(this.UltraPopupControlContainerSegnalibri_Opened);
            this.UltraPopupControlContainerSegnalibri.Closed += new System.EventHandler(this.UltraPopupControlContainerSegnalibri_Closed);
                                                this.UltraPopupControlContainerCartelleInVisione.Opening += new System.ComponentModel.CancelEventHandler(this.UltraPopupControlContainerCartelleInVisione_Opening);
            this.UltraPopupControlContainerCartelleInVisione.Opened += new System.EventHandler(this.UltraPopupControlContainerCartelleInVisione_Opened);
            this.UltraPopupControlContainerCartelleInVisione.Closed += new System.EventHandler(this.UltraPopupControlContainerCartelleInVisione_Closed);
                                                this.UltraPopupControlContainerPazientiSeguiti.Opening += new System.ComponentModel.CancelEventHandler(this.UltraPopupControlContainerPazientiSeguiti_Opening);
            this.UltraPopupControlContainerPazientiSeguiti.Opened += new System.EventHandler(this.UltraPopupControlContainerPazientiSeguiti_Opened);
            this.UltraPopupControlContainerPazientiSeguiti.Closed += new System.EventHandler(this.UltraPopupControlContainerPazientiSeguiti_Closed);
                                                this.UltraPopupControlContainerPazientiInVisione.Opening += new System.ComponentModel.CancelEventHandler(this.UltraPopupControlContainerPazientiInVisione_Opening);
            this.UltraPopupControlContainerPazientiInVisione.Opened += new System.EventHandler(this.UltraPopupControlContainerPazientiInVisione_Opened);
            this.UltraPopupControlContainerPazientiInVisione.Closed += new System.EventHandler(this.UltraPopupControlContainerPazientiInVisione_Closed);
                                                this.UltraPopupControlContainerHelp.Opening += new System.ComponentModel.CancelEventHandler(this.UltraPopupControlContainerHelp_Opening);
            this.UltraPopupControlContainerHelp.Opened += new System.EventHandler(this.UltraPopupControlContainerHelp_Opened);
            this.UltraPopupControlContainerHelp.Closed += new System.EventHandler(this.UltraPopupControlContainerHelp_Closed);
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 682);
            this.Controls.Add(this.UltraTabControlMenu);
            this.Controls.Add(this.UltraPanelTop);
            this.Controls.Add(this.UltraPanelBottom);
            this.KeyPreview = true;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.VisibleChanged += new System.EventHandler(this.frmMain_VisibleChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.UltraPanelTop.ClientArea.ResumeLayout(false);
            this.UltraPanelTop.ResumeLayout(false);
            this.UltraPanelBottom.ClientArea.ResumeLayout(false);
            this.UltraPanelBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UltraTabControlMenu)).EndInit();
            this.UltraTabControlMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraPanel UltraPanelTop;
        private Infragistics.Win.Misc.UltraPanel UltraPanelBottom;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        public Infragistics.Win.UltraWinTabControl.UltraTabControl UltraTabControlMenu;
        internal ucTop ucTop;
        internal ucBottom ucBottom;
        private Infragistics.Win.Misc.UltraPopupControlContainer UltraPopupControlContainerSegnalibri;
        private Infragistics.Win.Misc.UltraPopupControlContainer UltraPopupControlContainerCartelleInVisione;
        private Infragistics.Win.Misc.UltraPopupControlContainer UltraPopupControlContainerPazientiSeguiti;
        private Infragistics.Win.Misc.UltraPopupControlContainer UltraPopupControlContainerPazientiInVisione;
        private Infragistics.Win.Misc.UltraPopupControlContainer UltraPopupControlContainerHelp;
    }
}