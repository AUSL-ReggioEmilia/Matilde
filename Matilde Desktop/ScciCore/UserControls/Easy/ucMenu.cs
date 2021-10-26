using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.ScciCore.WebSvc;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.ScciResource;

using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.PluginClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Infragistics.Win.UltraWinToolbars;

namespace UnicodeSrl.ScciCore
{
    public partial class ucMenu : UserControl, Interfacce.IViewUserControlMiddle
    {

        private UserControl _ucc = null;

        private const string C_NEW_BUTTON_TEXT = @"§°§";

        const int INTERNET_OPTION_END_BROWSER_SESSION = 42;

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(
            IntPtr hInternet,
            int dwOption,
            IntPtr lpBuffer,
            int lpdwBufferLength);

        public ucMenu()
        {
            InitializeComponent();

            _ucc = (UserControl)this;
            this.MouseWheel += new MouseEventHandler(ucEasyFlowLayoutPanelButtons_MouseWheel);

        }

        #region Interface

        public void Aggiorna()
        {

            try
            {
                this.Inizializza();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        public void Carica()
        {

            try
            {
                this.InizializzaLogo();

                this.ucNewsMenu.Carica();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        public void Ferma()
        {

            try
            {

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        public void Disegna()
        {
            this.Inizializza();
        }

        #endregion

        #region SubRoutine

        private void InizializzaLogo()
        {
            try
            {

                this.ubCambiaRuolo.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_RUOLI_256);
                this.ubCambiaUtente.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_LOGIN_256);

                this.picLogoBig.Image = UnicodeSrl.Scci.Statics.Database.GetConfigTableImage(EnumConfigTable.LogoEasy);

                this.picLogoS.Image = UnicodeSrl.Scci.Statics.Database.GetConfigTableImage(EnumConfigTable.LogoFornitore);

            }
            catch (Exception)
            {
            }
        }

        private void Inizializza()
        {

            try
            {
                this.ucEasyFlowLayoutPanelButtons.SuspendLayout();
                this.ucNewsMenu.Aggiorna();
                this.ucEasyTextBoxNosologico.Visible = false;
                this.ucEasyTextBoxNosologico.Text = "";
                this.ubCambiaRuolo.PercImageFill = 0.75F;
                this.ubCambiaUtente.PercImageFill = 0.75F;

                this.elblNEWS.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;

                Ruoli r = CoreStatics.CoreApplication.Sessione.Utente.Ruoli;

                if (r.Elementi != null)
                    this.ubCambiaRuolo.Visible = (r.Elementi.Count > 1);

                if (r.RuoloSelezionato != null)
                {

                    if (r.RuoloSelezionato.Esiste(EnumModules.Pazienti_Menu))
                    {
                        this.ucEasyTextBoxNosologico.Visible = true;
                        this.ucEasyTextBoxNosologico.Focus();


                        ucEasyButton btn = getMenuButton(EnumModules.Pazienti_Menu, Keys.F1);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Pazienti";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_PAZIENTI_256);
                            btn.Click += new System.EventHandler(this.ubPazienti_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                    {
                        this.ucEasyTextBoxNosologico.Visible = false;
                        removeButtonMenu(EnumModules.Pazienti_Menu);
                    }

                    if (r.RuoloSelezionato.Esiste(EnumModules.ParamV_Home))
                    {

                        ucEasyButton btn = getMenuButton(EnumModules.ParamV_Home, Keys.F2);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Parametri Vitali";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_PARAMETRIVITALI_256);
                            btn.Click += new System.EventHandler(this.ubParametriVitali_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.ParamV_Home);

                    if (r.RuoloSelezionato.Esiste(EnumModules.WorkL_Home))
                    {

                        ucEasyButton btn = getMenuButton(EnumModules.WorkL_Home, Keys.F3);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Worklist";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_WORKLIST_256);
                            btn.Click += new System.EventHandler(this.ubWorklist_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.WorkL_Home);

                    if (r.RuoloSelezionato.Esiste(EnumModules.Agende_Home))
                    {

                        ucEasyButton btn = getMenuButton(EnumModules.Agende_Home, Keys.F4);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Agende";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_AGENDA_256);
                            btn.Click += new System.EventHandler(this.ubAgende_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.Agende_Home);

                    if (r.RuoloSelezionato.Esiste(EnumModules.Consulenza_Menu))
                    {

                        ucEasyButton btn = getMenuButton(EnumModules.Consulenza_Menu, Keys.F5);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Consulenza";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_CONSULENZA_256);
                            btn.Click += new System.EventHandler(this.ubConsulenza_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.Consulenza_Menu);

                    if (r.RuoloSelezionato.Esiste(EnumModules.CartellaAmbulatoriale_Menu))
                    {

                        ucEasyButton btn = getMenuButton(EnumModules.CartellaAmbulatoriale_Menu, Keys.F6);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Cartella Ambulatoriale";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_CARTELLACLINICA_256);
                            btn.Click += new System.EventHandler(this.ubCatellaAmbulatoriale_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.CartellaAmbulatoriale_Menu);

                    if (r.RuoloSelezionato.Esiste(EnumModules.PercorsoAmb_Menu))
                    {

                        ucEasyButton btn = getMenuButton(EnumModules.PercorsoAmb_Menu, Keys.Alt | Keys.A);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Percorso Ambulatoriale";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_PERCORSOCARTELLAAMBULATORIALE_256);
                            btn.Click += new System.EventHandler(this.ubPercorsoAmbulatoriale_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.PercorsoAmb_Menu);

                    if (r.RuoloSelezionato.Esiste(EnumModules.ChiusuraCartella_Menu))
                    {

                        ucEasyButton btn = getMenuButton(EnumModules.ChiusuraCartella_Menu, Keys.F7);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Chiusura Cartelle";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_CHIUSURACARTELLA_256);
                            btn.Click += new System.EventHandler(this.ubChiusuraCatella_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.ChiusuraCartella_Menu);

                    if (r.RuoloSelezionato.Esiste(EnumModules.EvidenzaC_Home))
                    {

                        ucEasyButton btn = getMenuButton(EnumModules.EvidenzaC_Home, Keys.F8);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Evidenza Clinica";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_EVIDENZACLINICA_256);
                            btn.Click += new System.EventHandler(this.ubEvidenzaClinica_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.EvidenzaC_Home);

                    if (r.RuoloSelezionato.Esiste(EnumModules.StampaCartelleChiuse_Menu))
                    {

                        ucEasyButton btn = getMenuButton(EnumModules.StampaCartelleChiuse_Menu, Keys.F9);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Stampa Cartelle";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_STAMPACARTELLAAPERTACHIUSA_256);
                            btn.Click += new System.EventHandler(this.ubStampaCartelleChiuse_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.StampaCartelleChiuse_Menu);

                    if (r.RuoloSelezionato.Esiste(EnumModules.PreTrasferimento_Menu))
                    {

                        ucEasyButton btn = getMenuButton(EnumModules.PreTrasferimento_Menu, Keys.F10);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Pre-Trasferimento";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_PRETRASFERIMENTO_256);
                            btn.Click += new System.EventHandler(this.PreTrasferimento_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.PreTrasferimento_Menu);

                    if (r.RuoloSelezionato.Esiste(EnumModules.Ordini_Home))
                    {
                        ucEasyButton btn = getMenuButton(EnumModules.Ordini_Home, Keys.F11);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Ordini";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_ORDINE_256);
                            btn.Click += new System.EventHandler(this.OrderEntryTrasversale_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.Ordini_Home);

                    if (r.RuoloSelezionato.Esiste(EnumModules.CartelleIV_Menu))
                    {
                        ucEasyButton btn = getMenuButton(EnumModules.CartelleIV_Menu, Keys.Alt | Keys.V);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Cartelle in Visione";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_CARTELLEINVISIONE_256);
                            btn.Click += new System.EventHandler(this.CartelleIV_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.CartelleIV_Menu);

                    if (r.RuoloSelezionato.Esiste(EnumModules.Consulenze_Home))
                    {
                        ucEasyButton btn = getMenuButton(EnumModules.Consulenze_Home, Keys.F12);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Consulenze";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_CONSULENZE_256);
                            btn.Click += new System.EventHandler(this.Consulenze_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.Consulenze_Home);

                    if (r.RuoloSelezionato.Esiste(EnumModules.CartelleIV_Menu))
                    {
                        ucEasyButton btn = getMenuButton(EnumModules.CartelleIV_Menu, Keys.Alt | Keys.V);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Cartelle in Visione";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_CARTELLEINVISIONE_256);
                            btn.Click += new System.EventHandler(this.CartelleIV_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.CartelleIV_Menu);

                    if (r.RuoloSelezionato.Esiste(EnumModules.FirmaCartelleAperte_Menu))
                    {

                        ucEasyButton btn = getMenuButton(EnumModules.FirmaCartelleAperte_Menu, Keys.Alt | Keys.F);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Firma Cartelle Aperte";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_FIRMACARTELLEAPERTE_256);
                            btn.Click += new System.EventHandler(this.ubFirmaCartelleAperte_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.FirmaCartelleAperte_Menu);

                    if (r.RuoloSelezionato.CDSSClient.Elementi.Count() != 0)
                    {
                        ucEasyButton btn = getMenuButton(EnumModules.Menu_Principale_CDSS, Keys.Alt | Keys.Z);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Altre Funzioni";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_CDSSPLUGIN_256);
                            btn.Click += new System.EventHandler(this.Cdss_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                    {
                        removeButtonMenu(EnumModules.Menu_Principale_CDSS);
                    }

                    if (r.RuoloSelezionato.Esiste(EnumModules.MatHome_Menu))
                    {

                        ucEasyButton btn = getMenuButton(EnumModules.MatHome_Menu, Keys.Alt | Keys.H);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Matilde Home";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_MATHOME_256);
                            btn.Click += new System.EventHandler(this.ubMatHome_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.MatHome_Menu);

                    if (r.RuoloSelezionato.Esiste(EnumModules.Consegne_Menu))
                    {
                        ucEasyButton btn = getMenuButton(EnumModules.Consegne_Menu, Keys.Alt | Keys.N);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Consegne di Reparto";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_CONSEGNE_256);
                            btn.Click += new System.EventHandler(this.ubConsegne_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.Consegne_Menu);

                    if (r.RuoloSelezionato.Esiste(EnumModules.ConsegneP_Menu))
                    {
                        ucEasyButton btn = getMenuButton(EnumModules.ConsegneP_Menu, Keys.Alt | Keys.O);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Consegne Paziente";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_CONSEGNEPAZIENTE_256);
                            btn.Click += new System.EventHandler(this.ubConsegneP_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.ConsegneP_Menu);


                    if (r.RuoloSelezionato.Esiste(EnumModules.OrdiniMonitor_Home))
                    {
                        ucEasyButton btn = getMenuButton(EnumModules.OrdiniMonitor_Home, Keys.Alt | Keys.M);
                        if (btn.Text == C_NEW_BUTTON_TEXT)
                        {
                            btn.Text = "Monitor Ordini";
                            btn.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_ORDINEMONITOR_256);
                            btn.Click += new System.EventHandler(this.OrderEntryMonitorTrasversale_Menu_Click);
                        }
                        this.ucEasyFlowLayoutPanelButtons.Controls.Add(setMenuButton(btn));
                    }
                    else
                        removeButtonMenu(EnumModules.OrdiniMonitor_Home);

                }
                this.ucEasyFlowLayoutPanelButtons.ResumeLayout();
            }
            catch (Exception ex)
            {
                this.ucEasyFlowLayoutPanelButtons.ResumeLayout();
                throw new Exception(ex.Message, ex);
            }



        }


        private void removeButtonMenu(EnumModules module)
        {
            try
            {
                Control[] ctrls = this.Controls.Find("ub" + module.ToString(), true);
                if (ctrls.Length == 1)
                {
                    ctrls[0].Parent.Controls.Remove(ctrls[0]);
                    ctrls[0].Dispose();
                    ctrls[0] = null;
                }
            }
            catch (Exception)
            {
            }
        }

        private ucEasyButton getMenuButton(EnumModules module, Keys shortcutKey)
        {
            ucEasyButton btnReturn = null;
            Control[] ctrls = this.Controls.Find("ub" + module.ToString(), true);
            if (ctrls.Length < 1)
            {
                btnReturn = new ucEasyButton();
                btnReturn.Location = new System.Drawing.Point(0, 0);
                btnReturn.Name = "ub" + module.ToString();
                btnReturn.Size = new System.Drawing.Size(106, 101);

                btnReturn.TabIndex = 0;

                btnReturn.Dock = System.Windows.Forms.DockStyle.Fill;

                btnReturn.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
                btnReturn.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2010Button;

                btnReturn.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Medium;
                btnReturn.ShortcutKey = shortcutKey;
                btnReturn.ShortcutPosition = UnicodeSrl.ScciCore.easyStatics.easyShortcutPosition.top_right;

                switch (module)
                {
                    case EnumModules.FirmaCartelleAperte_Menu:
                    case EnumModules.CartellaAmbulatoriale_Menu:
                    case EnumModules.Consegne_Menu:
                        btnReturn.PercImageFill = 0.62F;
                        break;
                    default:
                        btnReturn.PercImageFill = 0.75F;
                        break;
                }

                btnReturn.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Medium;

                btnReturn.Text = C_NEW_BUTTON_TEXT;

                btnReturn.Margin = new System.Windows.Forms.Padding((int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small));
            }
            else
            {
                btnReturn = ctrls[0] as ucEasyButton;
            }

            return btnReturn;
        }

        private ucEasyButton setMenuButton(ucEasyButton ubn)
        {

            try
            {


                int nWidth = Convert.ToInt32(this.ucEasyFlowLayoutPanelButtons.Width / 3.6);
                int nHeight = Convert.ToInt32(this.ucEasyFlowLayoutPanelButtons.Height / 3.4);

                ubn.Size = new Size(nWidth, nHeight); ubn.Dock = System.Windows.Forms.DockStyle.None;

            }
            catch (Exception)
            {

            }

            return ubn;

        }

        private void RunAs()
        {

            try
            {

                if (CoreStatics.CheckEsci() == true)
                {
                    InternetSetOption(IntPtr.Zero, INTERNET_OPTION_END_BROWSER_SESSION, IntPtr.Zero, 0);
                    CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.CambiaUtente);
                }

            }
            catch (Exception)
            {
                easyStatics.EasyMessageBox("Utente, password o dominio NON corretti!" + Environment.NewLine + "Contattare l'amministratore del sistema per la soluzione del problema ...", "Lancia Matilde come...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void RicercaPaziente()
        {
            try
            {
                if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato != null && CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Pazienti_Menu))
                {

                    if (this.ucEasyTextBoxNosologico.Text != null && this.ucEasyTextBoxNosologico.Text != string.Empty && this.ucEasyTextBoxNosologico.Text.Trim() != "")
                    {
                        CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                        Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);

                        string filtrogenerico = string.Empty;
                        string[] ricerche = this.ucEasyTextBoxNosologico.Text.Split(' ');
                        foreach (string ricerca in ricerche)
                        {

                            string format = "dd/MM/yyyy";
                            DateTime dateTime;
                            if (DateTime.TryParseExact(ricerca, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                            {
                                op.Parametro.Add("DataNascita", ricerca);
                            }
                            else
                            {
                                filtrogenerico += ricerca + " ";
                            }

                        }
                        op.Parametro.Add("FiltroGenerico", filtrogenerico);

                        op.Parametro.Add("Ordinamento", "P.Cognome, P.Nome");

                        op.Parametro.Add("CodStatoCartella", "AP");

                        SqlParameterExt[] spcoll = new SqlParameterExt[1];

                        string xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        DataTable dt = Database.GetDataTableStoredProc("MSP_CercaEpisodio", spcoll);

                        CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);

                        if (dt != null)
                        {

                            switch (dt.Rows.Count)
                            {
                                case 0:
                                    easyStatics.EasyMessageBox("La ricerca non ha restituito alcun risultato.", "Ricerca Paziente", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
                                    break;

                                case 1:

                                    bool clearSelection = true;
                                    DataRow oUgr = dt.Rows[0];

                                    CoreStatics.CoreApplication.Paziente = new Paziente("", oUgr["IDEpisodio"].ToString());
                                    CoreStatics.CoreApplication.Episodio = new Episodio(oUgr["IDEpisodio"].ToString());
                                    CoreStatics.CoreApplication.Trasferimento = new Trasferimento(oUgr["IDTrasferimento"].ToString(), CoreStatics.CoreApplication.Ambiente);
                                    if (!oUgr.IsNull("IDCartella") && oUgr["IDCartella"].ToString() != "")
                                    {
                                        CoreStatics.CoreApplication.Cartella = new Cartella(oUgr["IDCartella"].ToString(), oUgr["NumeroCartella"].ToString(), CoreStatics.CoreApplication.Ambiente);
                                    }

                                    clearSelection = !CoreStatics.CaricaCartellaPaziente(oUgr, EnumMaschere.MenuPrincipale);

                                    if (clearSelection)
                                    {
                                        CoreStatics.CoreApplication.Paziente = null;
                                        CoreStatics.CoreApplication.Episodio = null;
                                        CoreStatics.CoreApplication.Trasferimento = null;
                                        CoreStatics.CoreApplication.Cartella = null;
                                    }

                                    break;

                                default:
                                    CoreStatics.CoreApplication.Sessione.RicercaPazienti = this.ucEasyTextBoxNosologico.Text;
                                    CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.RicercaPazienti);
                                    CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
                                    break;
                            }

                            dt.Dispose();

                        }
                    }


                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "RicercaPaziente", this.Name);
            }

        }

        private void SetMenuButtonRight()
        {

            try
            {

                this.UltraToolbarsManager.Tools.Clear();
                PopupMenuTool oMr = new PopupMenuTool("MenuRight");
                this.UltraToolbarsManager.Tools.Add(oMr);

                var utbm = (PopupMenuTool)this.UltraToolbarsManager.Tools["MenuRight"];

                utbm.Settings.UseLargeImages = Infragistics.Win.DefaultableBoolean.True;
                ucEasyButton btn = getMenuButton(EnumModules.Menu_Principale_CDSS, Keys.F12);
                utbm.Settings.Appearance.FontData.SizeInPoints = btn.Appearance.FontData.SizeInPoints;
                utbm.Tools.Clear();

                foreach (Scci.PluginClient.Plugin oPlugin in CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.CDSSClient.Elementi)
                {

                    if (this.UltraToolbarsManager.Tools.Exists(oPlugin.Codice) == false)
                    {

                        ButtonTool oBt = new ButtonTool(oPlugin.Codice);
                        oBt.SharedProps.Caption = oPlugin.Descrizione;
                        oBt.SharedProps.AppearancesLarge.Appearance.BackColor = Color.WhiteSmoke;
                        oBt.SharedProps.AppearancesLarge.Appearance.Image = DrawingProcs.GetImageFromByte(oPlugin.Icona);
                        this.UltraToolbarsManager.Tools.Add(oBt);

                    }

                    utbm.Tools.AddTool(this.UltraToolbarsManager.Tools[oPlugin.Codice].Key);
                    utbm.Tools[this.UltraToolbarsManager.Tools[oPlugin.Codice].Key].InstanceProps.Tag = oPlugin;

                }

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Events

        private void ubCambiaRuolo_Click(object sender, EventArgs e)
        {


            try
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
                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezionaRuolo) == DialogResult.OK)
                    {

                        bool bAvanti = true;
                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.RichiediPassword == true)
                        {
                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.RichiediPassword) != DialogResult.OK)
                            {
                                bAvanti = false;
                            }
                        }

                        if (bAvanti == true)
                        {
                            CoreStatics.CoreApplication.Navigazione.Maschere.CloseAllForm();

                            CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.CaricaReports(CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice, "");
                            PluginClientStatics.Pcm = PluginClientStatics.SetPluginClientManager(CoreStatics.CoreApplication.Ambiente,
                                                                                                    CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice,
                                                                                                    CoreStatics.CoreApplication.Sessione.Computer.SessioneRemota,
                                                                                                    CoreStatics.CoreApplication.Sessione.Computer.IsOSServer);
                        }
                        else
                        {
                            System.Environment.Exit(0);
                        }


                    }

                }

                Interlocked.Decrement(ref frmMain.CounterNav);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void ubPazienti_Menu_Click(object sender, EventArgs e)
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
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.RicercaPazienti);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void ubParametriVitali_Menu_Click(object sender, EventArgs e)
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
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.ParametriVitaliTrasversali);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void ubWorklist_Menu_Click(object sender, EventArgs e)
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
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.WorklistInfermieristicaTrasversale);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void ubAgende_Menu_Click(object sender, EventArgs e)
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
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.AgendeTrasversali);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void ubConsulenza_Menu_Click(object sender, EventArgs e)
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
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Consulenze_RicercaPaziente);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void ubCatellaAmbulatoriale_Menu_Click(object sender, EventArgs e)
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
                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Ambulatoriale_SelezioneUA) == DialogResult.OK)
                {
                    CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Ambulatoriale_RicercaPaziente);
                }
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void ubPercorsoAmbulatoriale_Menu_Click(object sender, EventArgs e)
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
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.PercorsoAmbulatoriale_RicercaPaziente);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);


        }
        private void ubChiusuraCatella_Menu_Click(object sender, EventArgs e)
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
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.ChiusuraCartelle);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void ubFirmaCartelleAperte_Menu_Click(object sender, EventArgs e)
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
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.FirmaCartelleAperte);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void ubConsegne_Menu_Click(object sender, EventArgs e)
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
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Consegne);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void ubConsegneP_Menu_Click(object sender, EventArgs e)
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
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.ConsegnePaziente);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void ubEvidenzaClinica_Menu_Click(object sender, EventArgs e)
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
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EvidenzaClinicaTrasversale);
                try
                {
                    ((ucEvidenzaClinicaTrasversale)CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ControlMiddle).setFocusDefault();
                }
                catch
                {
                }
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void ubStampaCartelleChiuse_Menu_Click(object sender, EventArgs e)
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
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.StampaCartelleChiuse);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void ubCambiaUtente_Click(object sender, EventArgs e)
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
                this.RunAs();
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void PreTrasferimento_Menu_Click(object sender, EventArgs e)
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
                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.PreTrasferimento_SelezioneUAUO) == DialogResult.OK)
                {
                    CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.PreTrasferimento_RicercaPaziente);
                }
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void OrderEntryTrasversale_Menu_Click(object sender, EventArgs e)
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
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.OrderEntryTrasversale);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);
        }

        private void OrderEntryMonitorTrasversale_Menu_Click(object sender, EventArgs e)
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
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.OrderEntryMonitorTrasversale);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);
        }

        private void Consulenze_Menu_Click(object sender, EventArgs e)
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
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.ConsulenzeTrasversali);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);
        }

        private void CartelleIV_Menu_Click(object sender, EventArgs e)
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
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.CartelleInVisione);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void Cdss_Menu_Click(object sender, EventArgs e)
        {

            this.SetMenuButtonRight();
            Point p = ((ucEasyButton)sender).PointToScreen(((ucEasyButton)sender).Parent.Location);
            if (CoreStatics.CoreApplication.Sessione.Computer.SessioneIpovedente)
            {
                Risorse.GetPlaySoundFromResource(Risorse.GC_WAV_ALTREFUNZIONI);
            }
            ((PopupMenuTool)this.UltraToolbarsManager.Tools["MenuRight"]).ShowPopup();

        }

        private void ubMatHome_Menu_Click(object sender, EventArgs e)
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
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.MatHome_RicercaPaziente);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void UltraToolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
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
                Risposta oRispostaMenuEsegui = PluginClientStatics.PluginClientMenuEsegui((Plugin)e.Tool.InstanceProps.Tag, new object[1] { new object() });
                if (oRispostaMenuEsegui.Successo == true)
                {

                }
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        private void ucEasyTextBoxNosologico_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    RicercaPaziente();
                }
            }
            catch (Exception)
            {
            }
        }

        void mt_LastThreadException(object sender, Exception ex)
        {
            MessageBox.Show(ex.ToString());
            UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);

        }

        void mt_DatatableCompleted(object sender, DataTable dt)
        {
            MessageBox.Show(dt.Rows.Count.ToString());

            UnicodeSrl.ScciCore.Common.MT.ScciWebSvcMT mt = (UnicodeSrl.ScciCore.Common.MT.ScciWebSvcMT)sender;
            mt.DatatableCompleted -= mt_DatatableCompleted;
            mt.LastThreadException -= mt_LastThreadException;

        }

        private void ucEasyFlowLayoutPanelButtons_MouseEnter(object sender, EventArgs e)
        {
        }

        private void ucEasyFlowLayoutPanelButtons_MouseClick(object sender, MouseEventArgs e)
        {
            Region r = new Region();
            System.Drawing.Drawing2D.GraphicsPath P = new System.Drawing.Drawing2D.GraphicsPath(System.Drawing.Drawing2D.FillMode.Winding);
            Rectangle rec = default(Rectangle);
            rec.X = this.ucEasyFlowLayoutPanelButtons.Location.X;
            rec.Y = this.ucEasyFlowLayoutPanelButtons.Location.Y;
            rec.Width = this.ucEasyFlowLayoutPanelButtons.Width;
            rec.Height = this.ucEasyFlowLayoutPanelButtons.Height / 2;
            P.AddRectangle(rec);
            r.MakeEmpty();
            r.Union(P);
            if (r.IsVisible(e.X, e.Y))
            {
                CoreStatics.ScrollUp(this.ucEasyFlowLayoutPanelButtons, true);
            }
            else
            {
                CoreStatics.ScrollDown(this.ucEasyFlowLayoutPanelButtons, true);
            }
        }

        private void ucEasyFlowLayoutPanelButtons_MouseWheel(object sender, MouseEventArgs e)
        {
            this.ucEasyFlowLayoutPanelButtons.Focus();
        }

        #endregion

    }
}
