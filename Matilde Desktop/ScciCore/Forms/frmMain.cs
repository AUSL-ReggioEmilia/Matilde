using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.Scci;
using UnicodeSrl.ScciResource;
using Infragistics.Win.Misc;
using UnicodeSrl.Scci.Enums;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Scci.PluginClient;
using UnicodeSrl.Scci.Statics;

using System.Threading;

namespace UnicodeSrl.ScciCore
{
    public partial class frmMain : Form, Interfacce.IViewFormMain
    {

        #region Declare

        private Form _frmc = null;

        private int _topheight = -1;
        private int _bottomheight = -1;

                private ucSegnalibri _ucSegnalibri = null;

                private ucCartelleInVisione _ucCartelleInVisione = null;

                private ucPazientiInVisione _ucPazientiInVisione = null;

                private ucPazientiSeguiti _ucPazientiSeguiti = null;

                private ucHelp _ucHelp = null;

        private System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();

                public static long CounterNav;

        #endregion

        public frmMain()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);

            _frmc = (Form)this;

        }

        private Maschera Maschera
        {
            get
            {
                return CoreStatics.CoreApplication.Navigazione.Maschere.Elementi.FirstOrDefault(Maschera => Maschera.CodMaschera == this.ucTop.CodiceMaschera);
            }
        }

        #region Interface

        string Interfacce.IViewFormMain.CodiceMaschera
        {
            get { return this.ucTop.CodiceMaschera; }
            set { this.ucTop.CodiceMaschera = value; }
        }

        private bool m_ControlloCentraleMassimizzato;

        public bool ControlloCentraleMassimizzato
        {
            get { return m_ControlloCentraleMassimizzato; }
            set
            {
                m_ControlloCentraleMassimizzato = value;

                this.ResizeMain();
            }
        }

        int Interfacce.IViewFormMain.ControlloCentraleTimerRefresh
        {
            get
            {
                return _timer.Interval;
            }
            set
            {
                if (value == 0)
                {
                    _timer.Enabled = false;
                }
                else
                {
                    _timer.Interval = value;
                    _timer.Enabled = true;
                }
            }
        }

        #endregion

        #region Subroutine

        private void InitializeMain()
        {
#if NET472
            this.Text = string.Format("Matilde (Versione : {0}) (net 4.7.2)", Application.ProductVersion);
#elif NET40
            this.Text = string.Format("Matilde (Versione : {0}) (net 4.0)", Application.ProductVersion);
#endif
            this.Icon = Risorse.GetIconFromResource(Risorse.GC_SCCIEASY);

                        string fontFamily = null;
            fontFamily = CoreStatics.getFontPredefinitoForm();
            if (fontFamily != null && fontFamily != "") this.Font = new Font(fontFamily, this.Font.Size, this.Font.Style);

                        Form frm = this;
            if (CoreStatics.CoreApplication.Sessione.Sala == true)
            {
                easyStatics.maximizeForm(ref frm, System.Windows.Forms.FormBorderStyle.None);
            }
            else
            {
                easyStatics.maximizeForm(ref frm, System.Windows.Forms.FormBorderStyle.Sizable);
            }

        }

        private void InitializeTabControl()
        {

            try
            {

                this.UltraTabControlMenu.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard;
                this.UltraTabControlMenu.ViewStyle = Infragistics.Win.UltraWinTabControl.ViewStyle.Office2007;

            }
            catch (Exception)
            {

            }

        }

                                private FormWindowState PrevWindowState { get; set; }

        private void ResizeMain()
        {
            try
            {
                this.SuspendLayout();

                if (this.WindowState != FormWindowState.Minimized)
                {
                                        if (this.PrevWindowState == FormWindowState.Minimized)
                        return;

                    if (this.ControlloCentraleMassimizzato)
                    {
                                                if (_topheight < 0 && this.UltraPanelTop.Height > 0) _topheight = this.UltraPanelTop.Height;
                        if (_bottomheight < 0 && this.UltraPanelBottom.Height > 0) _bottomheight = this.UltraPanelBottom.Height;

                        this.UltraPanelTop.Height = 0;
                        this.UltraPanelBottom.Height = 0;

                        this.ucTop.Visible = false;
                        this.ucBottom.Visible = false;
                    }
                    else
                    {
                        bool bForzaRefresh = false;
                                                bForzaRefresh = !this.ucBottom.Visible;

                                                if (_topheight > 0) this.UltraPanelTop.Height = _topheight;
                        if (_bottomheight > 0) this.UltraPanelBottom.Height = _bottomheight;

                        this.UltraPanelTop.Size = new Size(this.UltraPanelTop.Size.Width, (this.Height / 100 * 15));
                        this.UltraPanelBottom.Size = new Size(this.UltraPanelBottom.Size.Width, (this.Height / 100 * 10));

                        this.ucTop.Visible = true;
                        this.ucBottom.Visible = true;
                        if (bForzaRefresh) this.ucBottom.Refresh();
                    }

                    CoreStatics.CoreApplication.Schermo = Screen.FromControl(this);
                }


            }
            catch (Exception)
            {

            }
            finally
            {
                this.PrevWindowState = this.WindowState;
                this.ResumeLayout();
            }


        }

        #endregion

        #region Events Form

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (CoreStatics.CoreApplication.Sessione.Computer.IsOSServer == true && CoreStatics.CoreApplication.Sessione.Computer.SessioneRemota == true)
            {
                this.MinimizeBox = false;
            }

            CoreStatics.impostaCursore(ref _frmc, Scci.Enums.enum_app_cursors.WaitCursor);

            this.InitializeMain();
            this.InitializeTabControl();
            this.ResizeMain();

            _timer.Enabled = false;
            _timer.Tick += new EventHandler(timerRefresh_Tick);

                                    var ucB = ((frmMain)CoreStatics.CoreApplicationContext.MainForm).ucBottom;
            ucB.Refresh();
            
            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;
            else
            {
                TopMost = true;
                Focus();
                BringToFront();
                TopMost = false;
                Activate();
            }

            CoreStatics.CoreApplication.Navigazione.Maschere.TracciaNavigazione("Programma Avviato");
            CoreStatics.impostaCursore(ref _frmc, Scci.Enums.enum_app_cursors.DefaultCursor);

                        Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing && CoreStatics.CheckEsci() == true)
            {
                _timer.Enabled = false;
                _timer.Tick -= new EventHandler(timerRefresh_Tick);
                CoreStatics.CoreApplicationContext.Exit();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            this.ResizeMain();
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (Interlocked.Equals(Maschere._navigare, 0))
            {
                return;
            }
            {
                                ScciCore.easyStatics.checkShortcutKeyDown_MultiKey(e, this.Controls);

                                if (!ScciCore.easyStatics.checkShortcutKeyDown(e.KeyCode, this.UltraTabControlMenu.ActiveTab.TabControl.Controls, false, e.Modifiers))
                {
                    ScciCore.easyStatics.checkShortcutKeyDown(e.KeyCode, this.ucBottom.Controls, !this.UltraPanelBottom.Visible, e.Modifiers);
                }
            }
        }

        private void frmMain_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle != null && CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle.GetType() == typeof(ucMenu))
                {
                    ((ucMenu)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle).Disegna();
                }
            }
            catch
            {

            }

        }
        #endregion

        #region Events

        private void timerRefresh_Tick(object sender, EventArgs e)
        {

            try
            {
                UnicodeSrl.ScciCore.Interfacce.IViewUserControlMiddle ctl = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle;

                if (ctl != null)
                    ctl.Aggiorna();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region Events ucTop

        private void ucTop_ImmagineClick(object sender, ImmagineTopClickEventArgs e)
        {

            if (frmMain.CounterNav != 0)
                return;

            Interlocked.Increment(ref frmMain.CounterNav);

            switch (e.Tipo)
            {

                case EnumImmagineTop.Utente:
                    CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsanteUtenteTop);
                    break;

                case EnumImmagineTop.VociDiarioClinico:
                    CoreStatics.CoreApplication.ListaIDMovDiarioClinicoSelezionati.Clear();
                    CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsanteVociDiarioClinicoTop);

                    if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.DiarioClinico ||
                        CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.FoglioUnico)
                    {
                        CoreStatics.impostaCursore(ref _frmc, Scci.Enums.enum_app_cursors.WaitCursor);
                        CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle.Aggiorna();
                        CoreStatics.impostaCursore(ref _frmc, Scci.Enums.enum_app_cursors.DefaultCursor);
                    }
                    break;

                case EnumImmagineTop.Paziente:
                    if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Pazienti_Modifica_Foto) || CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Pazienti_Visualizza_Foto))
                    {
                        CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsantePazienteTop);
                    }
                    break;

                case EnumImmagineTop.Consensi:
                    CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsanteConsensiTop);
                    bool bWarningElaborati = false;

                                        string idEpisodio = string.Empty;

                    if (CoreStatics.CoreApplication.Episodio != null)
                    {
                        idEpisodio = CoreStatics.CoreApplication.Episodio.ID;
                    }

                    if (CoreStatics.CoreApplication.Paziente != null)
                    {
                        bWarningElaborati = CoreStatics.CoreApplication.Paziente.WarningElaborati;

                        CoreStatics.CoreApplication.Paziente = new Paziente(CoreStatics.CoreApplication.Paziente.ID, idEpisodio);
                        CoreStatics.CoreApplication.Paziente.WarningElaborati = bWarningElaborati;
                        UnicodeSrl.Scci.Statics.CommonStatics.UpdateConsensiDaSAC(CoreStatics.CoreApplication.Paziente.IDPazienteFuso, CoreStatics.CoreApplication.Paziente.PazienteSacDatiAggiuntivi.Consensi, CoreStatics.CoreApplication.Ambiente);
                        CoreStatics.CoreApplication.Paziente = new Paziente(CoreStatics.CoreApplication.Paziente.ID, idEpisodio);
                        CoreStatics.CoreApplication.Paziente.WarningElaborati = bWarningElaborati;
                                                                                                CoreStatics.impostaCursore(ref _frmc, Scci.Enums.enum_app_cursors.WaitCursor);
                        if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata != null)
                        {
                            CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle.Aggiorna();
                        }
                        CoreStatics.impostaCursore(ref _frmc, Scci.Enums.enum_app_cursors.DefaultCursor);
                    }

                    break;

                case EnumImmagineTop.Allergie:
                                        CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsanteAllergieTop);
                    if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.FoglioUnico)
                    {
                        CoreStatics.impostaCursore(ref _frmc, Scci.Enums.enum_app_cursors.WaitCursor);
                        CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle.Aggiorna();
                        CoreStatics.impostaCursore(ref _frmc, Scci.Enums.enum_app_cursors.DefaultCursor);
                    }
                    break;

                case EnumImmagineTop.Alert:
                                        CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsanteAlertTop);
                    if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID == EnumMaschere.FoglioUnico)
                    {
                        CoreStatics.impostaCursore(ref _frmc, Scci.Enums.enum_app_cursors.WaitCursor);
                        CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle.Aggiorna();
                        CoreStatics.impostaCursore(ref _frmc, Scci.Enums.enum_app_cursors.DefaultCursor);
                    }
                    break;

                case EnumImmagineTop.EvidenzaClinica:
                                        CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsanteEvidenzaClinicaTop);
                    break;

                case EnumImmagineTop.Connettivita:
                    CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsanteConnettivitaTop);
                    break;

                case EnumImmagineTop.InfoPaziente:
                    CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsanteInfoPazienteTop);
                    break;

                case EnumImmagineTop.InfoEpisodio:
                    CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsanteInfoEpisodioTop);
                    break;

                case EnumImmagineTop.Refresh:
                    CoreStatics.impostaCursore(ref _frmc, Scci.Enums.enum_app_cursors.WaitCursor);
                    if (CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata != null)
                    {
                        CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle.Aggiorna();
                    }
                    CoreStatics.impostaCursore(ref _frmc, Scci.Enums.enum_app_cursors.DefaultCursor);
                    break;

                case EnumImmagineTop.Segnalibri:
                    if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato != null)
                    {
                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Bookmarks > 0)
                        {
                            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerSegnalibri);
                            _ucSegnalibri = new ucSegnalibri();
                            int iWidth = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large)) * 40;
                            int iHeight = Convert.ToInt32((double)iWidth / 1.52D);
                            _ucSegnalibri.Size = new Size(iWidth, iHeight);
                            _ucSegnalibri.ViewInit();
                            this.UltraPopupControlContainerSegnalibri.Show();
                        }
                    }
                    break;

                case EnumImmagineTop.CartelleInVisione:
                    if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato != null)
                    {
                        if (CoreStatics.CoreApplication.Cartella != null)
                        {
                            if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.CartelleInVisione > 0)
                            {
                                CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerCartelleInVisione);
                                _ucCartelleInVisione = new ucCartelleInVisione();
                                int iWidth = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large)) * 50;
                                int iHeight = Convert.ToInt32((double)iWidth / 1.52D);
                                _ucCartelleInVisione.Size = new Size(iWidth, iHeight);
                                _ucCartelleInVisione.ViewInit();
                                this.UltraPopupControlContainerCartelleInVisione.Show();
                            }
                            else
                            {
                                this.UltraPopupControlContainerCartelleInVisione_Click(sender, new CartelleInVisioneClickEventArgs(EnumPulsanteCartelleInVisione.Nuovo, ""));
                            }
                        }
                        else
                        {
                            if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.PazientiInVisione > 0)
                            {
                                CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerPazientiInVisione);
                                _ucPazientiInVisione = new ucPazientiInVisione();
                                int iWidth = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large)) * 50;
                                int iHeight = Convert.ToInt32((double)iWidth / 1.52D);
                                _ucPazientiInVisione.Size = new Size(iWidth, iHeight);
                                _ucPazientiInVisione.ViewInit();
                                this.UltraPopupControlContainerPazientiInVisione.Show();
                            }
                            else
                            {
                                this.UltraPopupControlContainerPazientiInVisione_Click(sender, new PazientiInVisioneClickEventArgs(EnumPulsanteCartelleInVisione.Nuovo, ""));
                            }
                        }
                    }
                    break;

                case EnumImmagineTop.PazientiSeguiti:
                    if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato != null)
                    {
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerPazientiSeguiti);
                        _ucPazientiSeguiti = new ucPazientiSeguiti();
                        int iWidth = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large)) * 50;
                        int iHeight = Convert.ToInt32((double)iWidth / 1.52D);
                        _ucPazientiSeguiti.Size = new Size(iWidth, iHeight);
                        _ucPazientiSeguiti.ViewInit();
                        this.UltraPopupControlContainerPazientiSeguiti.Show();
                    }
                    break;

                case EnumImmagineTop.Consegne:
                    CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsanteConsegneTop);
                    break;

                case EnumImmagineTop.Help:
                    CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerHelp);
                    _ucHelp = new ucHelp();
                    int iWidthH = Screen.PrimaryScreen.WorkingArea.Width;
                    int iHeightH = Screen.PrimaryScreen.WorkingArea.Height;
                    _ucHelp.Size = new Size(iWidthH, iHeightH);
                    _ucHelp.ViewInit();
                    this.UltraPopupControlContainerHelp.Show();
                    break;

            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        #endregion

        #region Events ucBottom

        private void ucBottom_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {

            if (frmMain.CounterNav != 0)
                return;

            Interlocked.Increment(ref frmMain.CounterNav);

                        if (Interlocked.Equals(Maschere._navigare, 0))
            {
                return;
            }
            {
                CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsanteIndietroBottom);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void ucBottom_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {

            if (frmMain.CounterNav != 0)
                return;

            Interlocked.Increment(ref frmMain.CounterNav);

                        if (Interlocked.Equals(Maschere._navigare, 0))
            {
                return;
            }
            else
            {
                CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsanteAvantiBottom);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void ucBottom_PulsanteAvantiToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {

            if (frmMain.CounterNav != 0)
                return;

            Interlocked.Increment(ref frmMain.CounterNav);

                        if (Interlocked.Equals(Maschere._navigare, 0))
            {
                return;
            }
            else
            {
                if (e.Tool.InstanceProps != null && e.Tool.InstanceProps.Tag != null && e.Tool.InstanceProps.Tag.GetType() == typeof(Plugin))
                {
                                                                                Risposta oRispostaMenuEsegui = PluginClientStatics.PluginClientMenuEsegui((Plugin)e.Tool.InstanceProps.Tag, new object[1] { new object() });
                    if (oRispostaMenuEsegui.Successo == true)
                    {

                    }
                }
                else
                {
                                                                                CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsanteAvantiMenuBottom, (EnumMaschere)Enum.Parse(typeof(EnumMaschere), e.Tool.Key));
                }
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void ucBottom_ImmagineClick(object sender, ImmagineBottomClickEventArgs e)
        {

            if (frmMain.CounterNav != 0)
                return;

            Interlocked.Increment(ref frmMain.CounterNav);

                        if (Interlocked.Equals(Maschere._navigare, 0))
            {
                return;
            }
            else
            {
                switch (e.Tipo)
                {

                    case EnumImmagineBottom.Home:
                        CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsanteHomeBottom);
                        break;

                    case EnumImmagineBottom.ElencoPazienti:
                                                                                                                                                                        CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsanteElencoPazientiBottom);
                                                break;

                    case EnumImmagineBottom.CartellaPaziente:
                        CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsanteCartellaPazienteBottom);
                        break;

                    case EnumImmagineBottom.Stampe:
                        CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsanteStampeBottom);
                        break;

                    case EnumImmagineBottom.CartelleChiuse:
                        CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), this.UltraTabControlMenu.ActiveTab.Key), EnumPulsante.PulsanteChiusuraCartelleBottom);
                                                break;
                }
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        #endregion

        #region UltraPopupControlContainerSegnalibri

        private void UltraPopupControlContainerSegnalibri_Closed(object sender, EventArgs e)
        {
            _ucSegnalibri.SegnalibriClick -= UltraPopupControlContainerSegnalibri_ModificaClick;
        }

        private void UltraPopupControlContainerSegnalibri_Opened(object sender, EventArgs e)
        {
            _ucSegnalibri.SegnalibriClick += UltraPopupControlContainerSegnalibri_ModificaClick;
            _ucSegnalibri.Focus();
        }

        private void UltraPopupControlContainerSegnalibri_Opening(object sender, CancelEventArgs e)
        {
            UltraPopupControlContainer popup = sender as UltraPopupControlContainer;
            popup.PopupControl = _ucSegnalibri;
        }

        private void UltraPopupControlContainerSegnalibri_ModificaClick(object sender, SegnalibriClickEventArgs e)
        {

            try
            {

                switch (e.Pulsante)
                {

                    case EnumPulsanteSegnalibri.Modifica:
                        this.UltraPopupControlContainerSegnalibri.Close();
                        CoreStatics.CaricaPopup(EnumMaschere.Scheda, EnumEntita.SCH, e.ID);
                        break;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

        #region UltraPopupControlContainerCartelleInVisione

        private void UltraPopupControlContainerCartelleInVisione_Closed(object sender, EventArgs e)
        {
            _ucCartelleInVisione.CartelleInVisioneClick -= UltraPopupControlContainerCartelleInVisione_Click;
        }

        private void UltraPopupControlContainerCartelleInVisione_Opened(object sender, EventArgs e)
        {
            _ucCartelleInVisione.CartelleInVisioneClick += UltraPopupControlContainerCartelleInVisione_Click;
            _ucCartelleInVisione.Focus();
        }

        private void UltraPopupControlContainerCartelleInVisione_Opening(object sender, CancelEventArgs e)
        {
            UltraPopupControlContainer popup = sender as UltraPopupControlContainer;
            popup.PopupControl = _ucCartelleInVisione;
        }

        private void UltraPopupControlContainerCartelleInVisione_Click(object sender, CartelleInVisioneClickEventArgs e)
        {

            try
            {

                switch (e.Pulsante)
                {

                    case EnumPulsanteCartelleInVisione.Nuovo:
                        this.UltraPopupControlContainerCartelleInVisione.Close();
                        CoreStatics.CoreApplication.MovCartellaInVisioneSelezionata = new MovCartellaInVisione();
                        CoreStatics.CoreApplication.MovCartellaInVisioneSelezionata.IDCartella = CoreStatics.CoreApplication.Cartella.ID;
                        CoreStatics.CoreApplication.MovCartellaInVisioneSelezionata.DataInizio = DateTime.Now;
                        CoreStatics.CoreApplication.MovCartellaInVisioneSelezionata.DataFine = CoreStatics.CoreApplication.MovCartellaInVisioneSelezionata.DataInizio.AddDays(1);
                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.CartellaInVisione) == DialogResult.OK)
                        {

                        }
                        break;

                    case EnumPulsanteCartelleInVisione.Modifica:
                        this.UltraPopupControlContainerCartelleInVisione.Close();
                        CoreStatics.CoreApplication.MovCartellaInVisioneSelezionata = new MovCartellaInVisione(e.ID);
                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.CartellaInVisione) == DialogResult.OK)
                        {

                        }
                        break;

                    case EnumPulsanteCartelleInVisione.Cancella:
                        CoreStatics.CoreApplication.MovCartellaInVisioneSelezionata = new MovCartellaInVisione(e.ID);
                        CoreStatics.CoreApplication.MovCartellaInVisioneSelezionata.Azione = EnumAzioni.CAN;
                        CoreStatics.CoreApplication.MovCartellaInVisioneSelezionata.Salva();
                        break;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

        #region UltraPopupControlContainerPazientiInVisione

        private void UltraPopupControlContainerPazientiInVisione_Closed(object sender, EventArgs e)
        {
            _ucPazientiInVisione.PazientiInVisioneClick -= UltraPopupControlContainerPazientiInVisione_Click;
        }

        private void UltraPopupControlContainerPazientiInVisione_Opened(object sender, EventArgs e)
        {
            _ucPazientiInVisione.PazientiInVisioneClick += UltraPopupControlContainerPazientiInVisione_Click;
            _ucPazientiInVisione.Focus();
        }

        private void UltraPopupControlContainerPazientiInVisione_Opening(object sender, CancelEventArgs e)
        {
            UltraPopupControlContainer popup = sender as UltraPopupControlContainer;
            popup.PopupControl = _ucPazientiInVisione;
        }

        private void UltraPopupControlContainerPazientiInVisione_Click(object sender, PazientiInVisioneClickEventArgs e)
        {

            try
            {

                switch (e.Pulsante)
                {

                    case EnumPulsanteCartelleInVisione.Nuovo:
                        this.UltraPopupControlContainerPazientiInVisione.Close();
                        CoreStatics.CoreApplication.MovPazienteInVisioneSelezionato = new MovPazienteInVisione();
                        CoreStatics.CoreApplication.MovPazienteInVisioneSelezionato.IDPaziente = CoreStatics.CoreApplication.Paziente.ID;
                        CoreStatics.CoreApplication.MovPazienteInVisioneSelezionato.DataInizio = DateTime.Now;
                        CoreStatics.CoreApplication.MovPazienteInVisioneSelezionato.DataFine = CoreStatics.CoreApplication.MovPazienteInVisioneSelezionato.DataInizio.AddDays(1);
                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.PazienteInVisione) == DialogResult.OK)
                        {

                        }
                        break;

                    case EnumPulsanteCartelleInVisione.Modifica:
                        this.UltraPopupControlContainerPazientiInVisione.Close();
                        CoreStatics.CoreApplication.MovPazienteInVisioneSelezionato = new MovPazienteInVisione(e.ID);
                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.PazienteInVisione) == DialogResult.OK)
                        {

                        }
                        break;

                    case EnumPulsanteCartelleInVisione.Cancella:
                        CoreStatics.CoreApplication.MovPazienteInVisioneSelezionato = new MovPazienteInVisione(e.ID);
                        CoreStatics.CoreApplication.MovPazienteInVisioneSelezionato.Azione = EnumAzioni.CAN;
                        CoreStatics.CoreApplication.MovPazienteInVisioneSelezionato.Salva();
                        break;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

        #region UltraPopupControlContainerPazientiSeguiti

        private void UltraPopupControlContainerPazientiSeguiti_Closed(object sender, EventArgs e)
        {
            _ucPazientiSeguiti.PazientiSeguitiClick -= UltraPopupControlContainerPazientiSeguiti_Click;
        }

        private void UltraPopupControlContainerPazientiSeguiti_Opened(object sender, EventArgs e)
        {
            _ucPazientiSeguiti.PazientiSeguitiClick += UltraPopupControlContainerPazientiSeguiti_Click;
            _ucPazientiSeguiti.Focus();
        }

        private void UltraPopupControlContainerPazientiSeguiti_Opening(object sender, CancelEventArgs e)
        {
            UltraPopupControlContainer popup = sender as UltraPopupControlContainer;
            popup.PopupControl = _ucPazientiSeguiti;
        }

        private void UltraPopupControlContainerPazientiSeguiti_Click(object sender, PazientiSeguitiClickEventArgs e)
        {

            try
            {

                switch (e.Pulsante)
                {

                    case EnumPulsantePazientiSeguiti.Nuovo:
                        this.UltraPopupControlContainerPazientiSeguiti.Close();
                        CoreStatics.CoreApplication.MovPazienteSeguitoSelezionato = new MovPazienteSeguito();
                        CoreStatics.CoreApplication.MovPazienteSeguitoSelezionato.IDPaziente = CoreStatics.CoreApplication.Paziente.ID;
                        CoreStatics.CoreApplication.MovPazienteSeguitoSelezionato.CodUtente = CoreStatics.CoreApplication.Sessione.Utente.Codice;
                        CoreStatics.CoreApplication.MovPazienteSeguitoSelezionato.CodRuolo = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice;
                        CoreStatics.CoreApplication.MovPazienteSeguitoSelezionato.Salva();
                        break;

                    case EnumPulsantePazientiSeguiti.Cancella:
                        CoreStatics.CoreApplication.MovPazienteSeguitoSelezionato = new MovPazienteSeguito(e.ID);
                        CoreStatics.CoreApplication.MovPazienteSeguitoSelezionato.Azione = EnumAzioni.CAN;
                        CoreStatics.CoreApplication.MovPazienteSeguitoSelezionato.Salva();
                        break;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

        #region UltraPopupControlContainerHelp

        private void UltraPopupControlContainerHelp_Closed(object sender, EventArgs e)
        {
            _ucHelp.ChiudiClick -= UltraPopupControlContainerHelp_Click;
        }

        private void UltraPopupControlContainerHelp_Opened(object sender, EventArgs e)
        {
            _ucHelp.ChiudiClick += UltraPopupControlContainerHelp_Click;
            _ucHelp.Focus();
        }

        private void UltraPopupControlContainerHelp_Opening(object sender, CancelEventArgs e)
        {
            UltraPopupControlContainer popup = sender as UltraPopupControlContainer;
            popup.PopupControl = _ucHelp;
        }

        private void UltraPopupControlContainerHelp_Click(object sender, EventArgs e)
        {
            this.UltraPopupControlContainerHelp.Close();
        }

        #endregion

    }
}
