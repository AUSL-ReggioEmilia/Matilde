using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinSchedule;
using Infragistics.Win.UltraWinGrid;
using System.Reflection;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciCore.Common.Extensions;
using System.IO;
using System.Drawing.Imaging;
using UnicodeSrl.Scci.Model;

namespace UnicodeSrl.ScciCore
{
    public partial class ucAgendeTrasversale : UserControl, Interfacce.IViewUserControlMiddle
    {

        public ucAgendeTrasversale()
        {
            InitializeComponent();

            this.ucEasyTableLayoutPanelCalendari.Visible = false;

            _ucc = (UserControl)this;
        }

        #region Declare

        private UserControl _ucc = null;
        private Dictionary<string, Image> oIcone = new Dictionary<string, Image>();
        private Dictionary<string, MassimaliAgenda> dict_MassimaliAgenda = new Dictionary<string, MassimaliAgenda>();

        private enum enumTipoPopup
        {
            Nessuno = 0,
            Tipo1 = 1,
            Tipo2 = 2
        }
        private enumTipoPopup _enumTipoPopup = enumTipoPopup.Nessuno;
        private ucEasyGrid _ucEasyGrid = null;
        private ucRichTextBox _ucRichTextBox = null;

        Appointment _appselezionato = null;
        Owner _ownerselezionato = null;
        Infragistics.Win.UltraWinSchedule.Day _dayselezionato = null;
        SelectedTimeSlotRange _selectedtimeslotrange = null;
        TimeSlotInterval _timeSlotInterval = TimeSlotInterval.TenMinutes;

        bool _bInserisci = false;
        bool _bModifica = false;
        bool _bCancella = false;
        bool _bAnnulla = false;

        bool _bChangeStatoAppuntamento = true;

        #endregion

        #region Interface

        public void Aggiorna()
        {

            try
            {

                this.SuspendLayout();

                if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato != null)
                {

                    Calendario oCalendario = new Calendario();
                    oCalendario.TipoCalendario = this.ucCalendario1.Calendario.TipoCalendario;
                    oCalendario.ActiveDay = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio.Date;

                    foreach (MovAppuntamentoAgende oMaa in CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Elementi)
                    {
                        if (oMaa.Selezionata == true)
                        {
                            oCalendario.CodAgenda = oMaa.CodAgenda;
                            break;
                        }
                    }

                    this.ucCalendario1.Calendario = oCalendario;
                    StateButtonTool oSbt = (StateButtonTool)this.UltraToolbarsManager.Tools["1"];
                    oSbt.Checked = true;
                    this.ucCalendario1.RefreshData(this.ucEasyTextBoxFiltra.Text);
                    this.ucCalendario1.UltraCalendarInfo.SelectedAppointments.Clear();
                    Appointment oApp = this.ucCalendario1.GetAppointments(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDAppuntamento);
                    if (oApp != null)
                    {
                        this.ucCalendario1.UltraCalendarInfo.SelectedAppointments.Add(oApp);
                        this.ucCalendario1.UltraDayView.EnsureTimeSlotVisible(oApp.StartDateTime, true);

                    }

                }
                setPulsanteMax();

                this.ResumeLayout();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Aggiorna", this.Name);
            }

        }

        public void Carica()
        {

            try
            {

                this.SuspendLayout();

                this.ucCalendario1.Visible = false;
                this.ucCalendario2.Visible = false;
                this.ucCalendario3.Visible = false;
                this.ucCalendario4.Visible = false;

                Ruoli r = CoreStatics.CoreApplication.Sessione.Utente.Ruoli;
                _bInserisci = (r.RuoloSelezionato.Esiste(EnumModules.Agende_Inserisci));
                _bModifica = (r.RuoloSelezionato.Esiste(EnumModules.Agende_Modifica));
                _bCancella = (r.RuoloSelezionato.Esiste(EnumModules.Agende_Cancella));
                _bAnnulla = (r.RuoloSelezionato.Esiste(EnumModules.Agende_Annulla));

                InizializzaControlli();

                this.ucEasyTableLayoutPanelCalendari.Visible = true;

                this.ResumeLayout();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }

        }

        public void Ferma()
        {

            try
            {

                oIcone = new Dictionary<string, Image>();
                dict_MassimaliAgenda = new Dictionary<string, MassimaliAgenda>();

                CoreStatics.SetContesto(EnumEntita.APP, null);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region private functions

        private void InizializzaControlli()
        {

            try
            {

                CoreStatics.SetEasyUltraDockManager(ref this.ultraDockManager);
                CoreStatics.SetUltraToolbarsManager(ref this.UltraToolbarsManager);
                this.UltraToolbarsManager.Tools["1"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDE_1);
                this.UltraToolbarsManager.Tools["2"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDE_2);
                this.UltraToolbarsManager.Tools["3"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDE_3);
                this.UltraToolbarsManager.Tools["4"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDE_4);

                this.UltraToolbarsManager.Tools["NuovoAppuntamento"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_APPUNTAMENTOAGGIUNGI);
                this.UltraToolbarsManager.Tools["ModificaAppuntamento"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_APPUNTAMENTOMODIFICA);
                this.UltraToolbarsManager.Tools["CancellaAppuntamento"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_APPUNTAMENTOCANCELLA);
                this.UltraToolbarsManager.Tools["AnnullaAppuntamento"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_APPUNTAMENTOANNULLA);

                this.UltraToolbarsManager.Tools["NuovaNota"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NOTAAGGIUNGI);
                this.UltraToolbarsManager.Tools["ModificaNota"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NOTAMODIFICA);
                this.UltraToolbarsManager.Tools["CancellaNota"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NOTACANCELLA);

                this.UltraToolbarsManager.Tools["Tracker"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_TRACKER_256);
                this.UltraToolbarsManager.Tools["Cartella"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_CARTELLACLINICA_256);
                this.UltraToolbarsManager.Tools["Sincronizza"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGENDESINCRONIZZA_256);

                this.UltraToolbarsManager.Tools["Home"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_HOME_256);

                this.ultraDockManager.ControlPanes["Comandi"].Pinned = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.Calendari.ComandiPin;

                this.UltraToolbarsManager.Tools["Paziente"].SharedProps.Caption = "";
                this.UltraToolbarsManager.Tools["Paziente"].SharedProps.ToolTipText = "Info Paziente";
                this.UltraToolbarsManager.Tools["Paziente"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_INFOPAZ_256);
                this.UltraToolbarsManager.Tools["Paziente"].SharedProps.Visible = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Schede_Dettaglio_Paziente);


                caricaComboStatoAppuntamento("TR");

                this.uchkFiltro.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTRO_256);
                this.uchkFiltro.CheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTROAPPLICATO_256);
                this.uchkFiltro.Checked = false;
                this.uchkFiltro.PercImageFill = 0.75F;
                this.uchkFiltro.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.uchkFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ucCalendario1.Calendario = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.Calendari.Calendario1;
                this.ucCalendario2.Calendario = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.Calendari.Calendario2;
                this.ucCalendario3.Calendario = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.Calendari.Calendario3;
                this.ucCalendario4.Calendario = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.Calendari.Calendario4;

                StateButtonTool oSbt = (StateButtonTool)this.UltraToolbarsManager.Tools[CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.Calendari.Tipo.ToString()];
                oSbt.Checked = true;

                setPulsanteMax();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaControlli", this.Name);
            }

        }

        private void caricaComboStatoAppuntamento(string vsCodStatoAppuntamento)
        {
            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
            op.Parametro.Add("DatiEstesi", "0");
            op.Parametro.Add("CodStato", vsCodStatoAppuntamento);
            op.Parametro.Add("IncludiStato", "1"); SqlParameterExt[] spcoll = new SqlParameterExt[1];
            string xmlParam = XmlProcs.XmlSerializeToString(op);
            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
            DataTable oDt = Database.GetDataTableStoredProc("MSP_SelStatoAppuntamento", spcoll);

            this.uceStatoAppuntamento.Clear();
            this.uceStatoAppuntamento.ValueMember = "Codice";
            this.uceStatoAppuntamento.DisplayMember = "Descrizione";
            this.uceStatoAppuntamento.DataSource = oDt;
            this.uceStatoAppuntamento.Refresh();
        }

        private void ActionToolClick(ToolBase Tool)
        {

            ucCalendario oUcCalendario1 = null;
            ucCalendario oUcCalendario2 = null;

            bool bRefresh = true;

            try
            {

                switch (Tool.Key)
                {

                    case "1":
                    case "2":
                    case "3":
                    case "4":
                        this.SetUcControls(int.Parse(Tool.Key));
                        CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.Calendari.Tipo = int.Parse(Tool.Key);
                        CoreStatics.CoreApplication.Sessione.Utente.SalvaConfigUtente();
                        this.ucCalendario1.Visible = true;
                        this.ucCalendario2.Visible = true;
                        this.ucCalendario3.Visible = true;
                        this.ucCalendario4.Visible = true;
                        break;

                    case "NuovoAppuntamento":

                        this.SvuotaContesto();

                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Ambulatoriale_SelezioneUA, false) == DialogResult.OK)
                        {
                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.RicercaSAC) == DialogResult.OK)
                            {
                                CoreStatics.CoreApplication.MovAppuntamentiGenerati = new List<MovAppuntamento>();
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento("", CoreStatics.CoreApplication.Paziente.ID, "", "");
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodAgendaPartenza = _ownerselezionato.Key;

                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneTipoAppuntamento) == DialogResult.OK)
                                {
                                    if (_selectedtimeslotrange == null)
                                    {
                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio = _dayselezionato.Date;
                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine = _dayselezionato.Date;
                                    }
                                    else
                                    {
                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio = _selectedtimeslotrange.StartDateTime;
                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine = _selectedtimeslotrange.EndDateTime;
                                    }
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA = CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata;
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CaricaAgende();

                                    foreach (MovAppuntamentoAgende oMaa in CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Elementi)
                                    {
                                        if (oMaa.CodAgenda == _ownerselezionato.Key)
                                        {
                                            oMaa.Selezionata = true;
                                            oMaa.Modificata = true;
                                        }
                                    }

                                    while (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneAgendeAppuntamento, false) == DialogResult.OK)
                                    {
                                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneAppuntamento, false) == DialogResult.OK)
                                        {



                                            try
                                            {
                                                DataTable dtCartellePaziente = CoreStatics.CoreApplication.Paziente.CercaCartelle();
                                                if (dtCartellePaziente != null)
                                                {
                                                    if (dtCartellePaziente.Rows.Count > 0)
                                                    {
                                                        string idCartella = "";
                                                        string idTrasferimento = "";
                                                        string idEpisodio = "";
                                                        string numCartella = "";

                                                        if (!dtCartellePaziente.Rows[0].IsNull("IDCartella")) idCartella = dtCartellePaziente.Rows[0]["IDCartella"].ToString();
                                                        if (!dtCartellePaziente.Rows[0].IsNull("IDTrasferimento")) idTrasferimento = dtCartellePaziente.Rows[0]["IDTrasferimento"].ToString();
                                                        if (!dtCartellePaziente.Rows[0].IsNull("IDEpisodio")) idEpisodio = dtCartellePaziente.Rows[0]["IDEpisodio"].ToString();
                                                        if (!dtCartellePaziente.Rows[0].IsNull("NumeroCartella")) numCartella = dtCartellePaziente.Rows[0]["NumeroCartella"].ToString();

                                                        string sMsg = @"Il Paziente:" + Environment.NewLine + CoreStatics.CoreApplication.Paziente.Descrizione + Environment.NewLine;
                                                        sMsg += @"ha già una Cartella aperta: " + numCartella + Environment.NewLine;
                                                        sMsg += @"Vuoi inserire l'Appuntamento corrente nella Cartella?";

                                                        if (easyStatics.EasyMessageBox(sMsg, "Nuovo Appuntamento", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                                        {
                                                            if (CoreStatics.CoreApplication.MovAppuntamentiGenerati != null && CoreStatics.CoreApplication.MovAppuntamentiGenerati.Count > 1)
                                                            {
                                                                MovAppuntamento apporiginale = CoreStatics.CoreApplication.MovAppuntamentoSelezionato;

                                                                for (int i = 0; i < CoreStatics.CoreApplication.MovAppuntamentiGenerati.Count; i++)
                                                                {
                                                                    CoreStatics.CoreApplication.MovAppuntamentiGenerati[i].Azione = EnumAzioni.MOD;
                                                                    CoreStatics.CoreApplication.MovAppuntamentiGenerati[i].IDEpisodio = idEpisodio;
                                                                    CoreStatics.CoreApplication.MovAppuntamentiGenerati[i].IDTrasferimento = idTrasferimento;
                                                                    foreach (MovAppuntamentoAgende maa in CoreStatics.CoreApplication.MovAppuntamentiGenerati[i].Elementi)
                                                                    {
                                                                        if (maa.Selezionata == true && maa.Modificata == true) maa.Modificata = false;
                                                                    }

                                                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato = CoreStatics.CoreApplication.MovAppuntamentiGenerati[i];
                                                                    PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                                                    CoreStatics.CoreApplication.MovAppuntamentiGenerati[i] = CoreStatics.CoreApplication.MovAppuntamentoSelezionato;

                                                                    CoreStatics.CoreApplication.MovAppuntamentiGenerati[i].Salva();
                                                                }

                                                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = apporiginale;
                                                            }
                                                            else
                                                            {
                                                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione = EnumAzioni.MOD;
                                                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDEpisodio = idEpisodio;
                                                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDTrasferimento = idTrasferimento;
                                                                foreach (MovAppuntamentoAgende maa in CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Elementi)
                                                                {
                                                                    if (maa.Selezionata == true && maa.Modificata == true) maa.Modificata = false;
                                                                }

                                                                PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Salva();
                                                            }
                                                        }
                                                    }

                                                    dtCartellePaziente.Dispose();
                                                }
                                            }
                                            catch (Exception exnp)
                                            {
                                                CoreStatics.ExGest(ref exnp, "ActionToolClick", this.Name);
                                            }


                                            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                                            if (CoreStatics.CoreApplication.MovAppuntamentiGenerati != null && CoreStatics.CoreApplication.MovAppuntamentiGenerati.Count > 1)
                                            {
                                                for (int i = 0; i < CoreStatics.CoreApplication.MovAppuntamentiGenerati.Count; i++)
                                                {
                                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato = CoreStatics.CoreApplication.MovAppuntamentiGenerati[i];

                                                    PluginClientStatics.PluginClient(EnumPluginClient.APP_NUOVO_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                                }
                                            }
                                            else
                                            {
                                                PluginClientStatics.PluginClient(EnumPluginClient.APP_NUOVO_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                            }

                                            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);

                                            break;
                                        }
                                    }
                                }
                            }
                            CoreStatics.CoreApplication.MovAppuntamentoSelezionato = null;
                        }
                        break;

                    case "ModificaAppuntamento":
                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento(((System.Data.DataRowView)_appselezionato.BindingListObject)["IDAppuntamento"].ToString(), EnumAzioni.MOD);
                        while (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneAgendeAppuntamento) == DialogResult.OK)
                        {
                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneAppuntamento) == DialogResult.OK)
                            {


                                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);


                                PluginClientStatics.PluginClient(EnumPluginClient.APP_MODIFICA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
                                break;
                            }
                        }
                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato = null;

                        break;

                    case "CancellaAppuntamento":

                        bool bEliminaAppuntamentoSingolo = true;
                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento(((System.Data.DataRowView)_appselezionato.BindingListObject)["IDAppuntamento"].ToString(), EnumAzioni.CAN);

                        if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodSistema == EnumEntita.APP.ToString()
                            && !string.IsNullOrEmpty(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDGruppo))
                        {
                            if (easyStatics.EasyMessageBox("L'appuntamento selezionato fa parte di un gruppo." +
    Environment.NewLine + "Eseguire la cancellazione di tutto il gruppo ?", "Cancellazione Appuntamento",
    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {

                                bEliminaAppuntamentoSingolo = false;

                                if (easyStatics.EasyMessageBox("Confermi la cancellazione di TUTTO IL GRUPPO dell'Appuntamento selezionato ?", "Cancellazione Appuntamenti",
                                                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {

                                    CoreStatics.ImpostaCursoreMainForm(Scci.Enums.enum_app_cursors.WaitCursor);

                                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                                    op.Parametro.Add("IDGruppo", CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDGruppo);
                                    op.Parametro.Add("CodSistema", EnumEntita.APP.ToString());
                                    op.Parametro.Add("DatiEstesi", "0");

                                    op.TimeStamp.CodEntita = EnumEntita.APP.ToString();
                                    op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                                    string xmlParam = XmlProcs.XmlSerializeToString(op);

                                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                    DataSet dsSerieAppuntamenti = Database.GetDatasetStoredProc("MSP_SelMovAppuntamenti", spcoll);

                                    for (int a = dsSerieAppuntamenti.Tables[0].Rows.Count - 1; a >= 0; a--)
                                    {
                                        DataRow drApp = dsSerieAppuntamenti.Tables[0].Rows[a];
                                        if (!drApp.IsNull("PermessoCancella") && drApp["PermessoCancella"].ToString() == "1")
                                        {

                                            CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento(drApp["ID"].ToString(), EnumAzioni.CAN);
                                            CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento = EnumStatoAppuntamento.CA.ToString();

                                            PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                            CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Salva();
                                            PluginClientStatics.PluginClient(EnumPluginClient.APP_CANCELLA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                            CoreStatics.CoreApplication.MovAppuntamentoSelezionato = null;
                                        }
                                    }


                                    CoreStatics.ImpostaCursoreMainForm(Scci.Enums.enum_app_cursors.DefaultCursor);

                                }
                            }
                        }

                        if (bEliminaAppuntamentoSingolo)
                        {
                            if (easyStatics.EasyMessageBox("Confermi la cancellazione dell'appuntamento selezionato?", "Cancellazione appuntamento", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                CoreStatics.ImpostaCursoreMainForm(Scci.Enums.enum_app_cursors.WaitCursor);

                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento = EnumStatoAppuntamento.CA.ToString();

                                PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Salva();

                                PluginClientStatics.PluginClient(EnumPluginClient.APP_CANCELLA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                CoreStatics.ImpostaCursoreMainForm(Scci.Enums.enum_app_cursors.DefaultCursor);

                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = null;

                            }
                        }

                        break;

                    case "AnnullaAppuntamento":

                        bool bAnnullaAppuntamentoSingolo = true;
                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento(((System.Data.DataRowView)_appselezionato.BindingListObject)["IDAppuntamento"].ToString(), EnumAzioni.ANN);
                        #region Annullamento multiplo (disattivato)













                        #endregion

                        if (bAnnullaAppuntamentoSingolo)
                        {
                            if (easyStatics.EasyMessageBox("Confermi l'annullamento dell'appuntamento ?", "Annullamento appuntamento", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento = EnumStatoAppuntamento.AN.ToString();
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione = EnumAzioni.ANN;

                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.AnnullaAppuntamento) == DialogResult.OK)
                                {


                                    PluginClientStatics.PluginClient(EnumPluginClient.APP_ANNULLA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                    CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
                                }
                            }
                        }
                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato = null;


                        break;

                    case "NuovaNota":
                        CoreStatics.CoreApplication.MovNoteAgendeSelezionata = new MovNoteAgende();
                        CoreStatics.CoreApplication.MovNoteAgendeSelezionata.Azione = EnumAzioni.INS;
                        if (_selectedtimeslotrange == null)
                        {
                            CoreStatics.CoreApplication.MovNoteAgendeSelezionata.DataInizio = _dayselezionato.Date;
                            CoreStatics.CoreApplication.MovNoteAgendeSelezionata.DataFine = _dayselezionato.Date;
                        }
                        else
                        {
                            CoreStatics.CoreApplication.MovNoteAgendeSelezionata.DataInizio = _selectedtimeslotrange.StartDateTime;
                            CoreStatics.CoreApplication.MovNoteAgendeSelezionata.DataFine = AddTimeSlotInterval(_selectedtimeslotrange.StartDateTime, _timeSlotInterval);
                        }
                        CoreStatics.CoreApplication.MovNoteAgendeSelezionata.CodStatoNota = EnumStatoNotaAgenda.PR.ToString();
                        CoreStatics.CoreApplication.MovNoteAgendeSelezionata.CodAgenda = _ownerselezionato.Key;
                        CoreStatics.CoreApplication.MovNoteAgendeSelezionata.DescrAgenda = _ownerselezionato.Name;
                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneNota) == DialogResult.OK)
                        {
                        }
                        break;

                    case "ModificaNota":
                        CoreStatics.CoreApplication.MovNoteAgendeSelezionata = new MovNoteAgende(((System.Data.DataRowView)_appselezionato.BindingListObject)["ID"].ToString(), EnumAzioni.MOD);
                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneNota) == DialogResult.OK)
                        {
                        }
                        break;

                    case "CancellaNota":
                        if (easyStatics.EasyMessageBox("Sei sicuro di voler CANCELLARE" + Environment.NewLine +
                                                        "la nota selezionata ?" + Environment.NewLine +
                                                        "'" + _appselezionato.Subject + "'", "Cancellazione Note", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {

                            bool bRicorrenza = true;
                            if (((System.Data.DataRowView)_appselezionato.BindingListObject)["IDGruppo"].ToString() != string.Empty)
                            {
                                if (easyStatics.EasyMessageBox("Questa NOTA fà parte di una ricorrenza." + Environment.NewLine +
                                                                "Vuoi CANCELLARE anche tutte le ricorrenze ?", "Cancellazione Note", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                {
                                    bRicorrenza = false;
                                }
                            }

                            MovNoteAgende oMovNoteAgende = new MovNoteAgende(((System.Data.DataRowView)_appselezionato.BindingListObject)["ID"].ToString(), EnumAzioni.CAN);
                            oMovNoteAgende.CodStatoNota = EnumStatoNotaAgenda.CA.ToString();
                            if (bRicorrenza == false) { oMovNoteAgende.IDGruppo = string.Empty; }
                            oMovNoteAgende.Salva();
                            oMovNoteAgende = null;

                        }
                        break;

                    case "Tracker":
                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato = null;
                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.TrackerPaziente);
                        bRefresh = false;
                        break;

                    case "Cartella":
                        ApriCartella();
                        bRefresh = false;
                        break;

                    case "Sincronizza":
                        if (this.ActiveControl is ucCalendario)
                        {

                            DateTime _activeday = ((ucCalendario)this.ActiveControl).Calendario.ActiveDay;

                            this.ucCalendario1.UltraCalendarInfo.SelectedDateRanges.Clear();
                            this.ucCalendario1.UltraCalendarInfo.SelectedDateRanges.Add(_activeday);
                            this.ucCalendario1.UltraCalendarInfo.ActivateDay(_activeday);
                            this.ucCalendario1.Calendario.ActiveDay = _activeday;

                            this.ucCalendario2.UltraCalendarInfo.SelectedDateRanges.Clear();
                            this.ucCalendario2.UltraCalendarInfo.SelectedDateRanges.Add(_activeday);
                            this.ucCalendario2.UltraCalendarInfo.ActivateDay(_activeday);
                            this.ucCalendario2.Calendario.ActiveDay = _activeday;

                            this.ucCalendario3.UltraCalendarInfo.SelectedDateRanges.Clear();
                            this.ucCalendario3.UltraCalendarInfo.SelectedDateRanges.Add(_activeday);
                            this.ucCalendario3.UltraCalendarInfo.ActivateDay(_activeday);
                            this.ucCalendario3.Calendario.ActiveDay = _activeday;

                            this.ucCalendario4.UltraCalendarInfo.SelectedDateRanges.Clear();
                            this.ucCalendario4.UltraCalendarInfo.SelectedDateRanges.Add(_activeday);
                            this.ucCalendario4.UltraCalendarInfo.ActivateDay(_activeday);
                            this.ucCalendario4.Calendario.ActiveDay = _activeday;

                            bRefresh = true;

                        }
                        break;

                    case "Massimizza":
                        try
                        {
                            CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                            Form frm = this.FindForm();
                            if (frm != null && frm is Interfacce.IViewFormMain)
                            {
                                (frm as Interfacce.IViewFormMain).ControlloCentraleMassimizzato = !(frm as Interfacce.IViewFormMain).ControlloCentraleMassimizzato;

                                setPulsanteMax();
                            }
                        }
                        catch (Exception ex)
                        {
                            CoreStatics.ExGest(ref ex, "ActionToolClick", this.Name);
                        }
                        finally
                        {
                            CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                        }
                        bRefresh = false;
                        break;

                    case "Paziente":
                        if (CoreStatics.CoreApplication.Paziente != null)
                            CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera(CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID, EnumPulsante.PulsanteInfoPazienteTop);

                        bRefresh = false;
                        break;

                    case "Home":
                        minimizza();
                        CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera(CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID, EnumPulsante.PulsanteHomeBottom);

                        bRefresh = false;
                        break;
                }

                if (bRefresh == true)
                {
                    switch (this.UltraToolbarsManager.OptionSets["Tipo"].SelectedTool.Key)
                    {

                        case "1":
                            oUcCalendario1 = (ucCalendario)this.ucEasyTableLayoutPanelCalendari.GetControlFromPosition(0, 0);
                            if (oUcCalendario1.FirstLoad == true) { oUcCalendario1.ViewInit(); }
                            oUcCalendario1.RefreshData(this.ucEasyTextBoxFiltra.Text);
                            break;

                        case "2":
                            oUcCalendario1 = (ucCalendario)this.ucEasyTableLayoutPanelCalendari.GetControlFromPosition(0, 0);
                            if (oUcCalendario1.FirstLoad == true) { oUcCalendario1.ViewInit(); }
                            oUcCalendario1.RefreshData(this.ucEasyTextBoxFiltra.Text);
                            oUcCalendario2 = (ucCalendario)this.ucEasyTableLayoutPanelCalendari.GetControlFromPosition(1, 0);
                            if (oUcCalendario2.FirstLoad == true) { oUcCalendario2.ViewInit(); }
                            oUcCalendario2.RefreshData(this.ucEasyTextBoxFiltra.Text);
                            break;

                        case "3":
                            oUcCalendario1 = (ucCalendario)this.ucEasyTableLayoutPanelCalendari.GetControlFromPosition(0, 0);
                            if (oUcCalendario1.FirstLoad == true) { oUcCalendario1.ViewInit(); }
                            oUcCalendario1.RefreshData(this.ucEasyTextBoxFiltra.Text);
                            oUcCalendario2 = (ucCalendario)this.ucEasyTableLayoutPanelCalendari.GetControlFromPosition(0, 1);
                            if (oUcCalendario2.FirstLoad == true) { oUcCalendario2.ViewInit(); }
                            oUcCalendario2.RefreshData(this.ucEasyTextBoxFiltra.Text);
                            break;

                        case "4":
                            if (this.ucCalendario1.FirstLoad == true) { this.ucCalendario1.ViewInit(); }
                            if (this.ucCalendario2.FirstLoad == true) { this.ucCalendario2.ViewInit(); }
                            if (this.ucCalendario3.FirstLoad == true) { this.ucCalendario3.ViewInit(); }
                            if (this.ucCalendario4.FirstLoad == true) { this.ucCalendario4.ViewInit(); }
                            this.ucCalendario1.RefreshData(this.ucEasyTextBoxFiltra.Text);
                            this.ucCalendario2.RefreshData(this.ucEasyTextBoxFiltra.Text);
                            this.ucCalendario3.RefreshData(this.ucEasyTextBoxFiltra.Text);
                            this.ucCalendario4.RefreshData(this.ucEasyTextBoxFiltra.Text);
                            break;

                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ActionToolClick", this.Name);
                CoreStatics.ImpostaCursoreMainForm(Scci.Enums.enum_app_cursors.DefaultCursor);
            }

            CoreStatics.SetNavigazione(true);

        }

        private DateTime AddTimeSlotInterval(DateTime startDateTime, TimeSlotInterval interval)
        {
            DateTime ret = startDateTime;

            ret = ret.AddMinutes((int)interval);

            return ret;
        }

        private void SetUcControls(int nCount)
        {

            try
            {

                var oTlp = this.ucEasyTableLayoutPanelCalendari;

                oTlp.SuspendLayout();

                switch (nCount)
                {

                    case 1:
                        oTlp.ColumnStyles[0].Width = 100;
                        oTlp.ColumnStyles[1].Width = 0;
                        oTlp.RowStyles[0].Height = 100;
                        oTlp.RowStyles[1].Height = 0;
                        if (this.ActiveControl is ucCalendario)
                        {
                            if (this.ActiveControl.Equals((ucCalendario)oTlp.GetControlFromPosition(0, 0)) == false)
                            {
                                TableLayoutPanelCellPosition oCelPos = oTlp.GetPositionFromControl(this.ActiveControl);
                                ucCalendario oUcCalendarioActive = (ucCalendario)oTlp.GetControlFromPosition(oCelPos.Column, oCelPos.Row);
                                ucCalendario oUcCalendario0 = (ucCalendario)oTlp.GetControlFromPosition(0, 0);
                                oTlp.Controls.Remove(oUcCalendario0);
                                oTlp.Controls.Remove(oUcCalendarioActive);
                                oTlp.Controls.Add(oUcCalendario0, oCelPos.Column, oCelPos.Row);
                                oTlp.Controls.Add(oUcCalendarioActive, 0, 0);
                                oUcCalendarioActive.Focus();
                            }
                        }
                        break;

                    case 2:
                        if (oTlp.ColumnStyles[0].Width == 100 && oTlp.ColumnStyles[1].Width == 0 &&
                            oTlp.RowStyles[0].Height == 50 && oTlp.RowStyles[1].Height == 50)
                        {
                            ucCalendario oUcCalendario2 = (ucCalendario)oTlp.GetControlFromPosition(1, 0);
                            ucCalendario oUcCalendario3 = (ucCalendario)oTlp.GetControlFromPosition(0, 1);
                            oTlp.Controls.Remove(oUcCalendario2);
                            oTlp.Controls.Remove(oUcCalendario3);
                            oTlp.Controls.Add(oUcCalendario2, 0, 1);
                            oTlp.Controls.Add(oUcCalendario3, 1, 0);
                        }
                        oTlp.ColumnStyles[0].Width = 50;
                        oTlp.ColumnStyles[1].Width = 50;
                        oTlp.RowStyles[0].Height = 100;
                        oTlp.RowStyles[1].Height = 0;
                        break;

                    case 3:
                        if (oTlp.ColumnStyles[0].Width == 50 && oTlp.ColumnStyles[1].Width == 50 &&
                            oTlp.RowStyles[0].Height == 100 && oTlp.RowStyles[1].Height == 0)
                        {
                            ucCalendario oUcCalendario2 = (ucCalendario)oTlp.GetControlFromPosition(1, 0);
                            ucCalendario oUcCalendario3 = (ucCalendario)oTlp.GetControlFromPosition(0, 1);
                            oTlp.Controls.Remove(oUcCalendario2);
                            oTlp.Controls.Remove(oUcCalendario3);
                            oTlp.Controls.Add(oUcCalendario2, 0, 1);
                            oTlp.Controls.Add(oUcCalendario3, 1, 0);
                        }
                        oTlp.ColumnStyles[0].Width = 100;
                        oTlp.ColumnStyles[1].Width = 0;
                        oTlp.RowStyles[0].Height = 50;
                        oTlp.RowStyles[1].Height = 50;
                        break;

                    case 4:
                        oTlp.ColumnStyles[0].Width = 50;
                        oTlp.ColumnStyles[1].Width = 50;
                        oTlp.RowStyles[0].Height = 50;
                        oTlp.RowStyles[1].Height = 50;
                        break;

                }

                oTlp.ResumeLayout();

            }
            catch (Exception)
            {

            }

        }

        private void SetUltraToolbars(object sender, CalendarioEventArgs e)
        {

            try
            {

                if (_ownerselezionato != null)
                {

                    if (_appselezionato != null)
                    {

                        if (((System.Data.DataRowView)_appselezionato.BindingListObject)["flagNota"].ToString() == "0")
                        {
                            this.UltraToolbarsManager.Tools["NuovoAppuntamento"].SharedProps.Enabled = _bInserisci;
                            if ((((System.Data.DataRowView)_appselezionato.BindingListObject)["CodStatoAppuntamento"].ToString() == EnumStatoAppuntamento.ER.ToString()) ||
                                (((System.Data.DataRowView)_appselezionato.BindingListObject)["CodStatoAppuntamento"].ToString() == EnumStatoAppuntamento.AN.ToString())
                                )
                            {
                                this.UltraToolbarsManager.Tools["ModificaAppuntamento"].SharedProps.Enabled = false;
                            }
                            else
                            {
                                this.UltraToolbarsManager.Tools["ModificaAppuntamento"].SharedProps.Enabled = _bModifica;
                            }

                            if ((((System.Data.DataRowView)_appselezionato.BindingListObject)["CodStatoAppuntamento"].ToString() == EnumStatoAppuntamento.ER.ToString()) ||
   (((System.Data.DataRowView)_appselezionato.BindingListObject)["CodStatoAppuntamento"].ToString() == EnumStatoAppuntamento.AN.ToString())
   )
                            {
                                this.UltraToolbarsManager.Tools["CancellaAppuntamento"].SharedProps.Enabled = false;
                            }
                            else
                            {
                                this.UltraToolbarsManager.Tools["CancellaAppuntamento"].SharedProps.Enabled = _bCancella;
                            }

                            if ((((System.Data.DataRowView)_appselezionato.BindingListObject)["CodStatoAppuntamento"].ToString() == EnumStatoAppuntamento.ER.ToString()) ||
   (((System.Data.DataRowView)_appselezionato.BindingListObject)["CodStatoAppuntamento"].ToString() == EnumStatoAppuntamento.AN.ToString())
   )
                            {
                                this.UltraToolbarsManager.Tools["AnnullaAppuntamento"].SharedProps.Enabled = false;
                            }
                            else
                            {
                                this.UltraToolbarsManager.Tools["AnnullaAppuntamento"].SharedProps.Enabled = _bAnnulla;
                            }
                            this.UltraToolbarsManager.Tools["NuovaNota"].SharedProps.Enabled = _bInserisci;
                            this.UltraToolbarsManager.Tools["ModificaNota"].SharedProps.Enabled = false;
                            this.UltraToolbarsManager.Tools["CancellaNota"].SharedProps.Enabled = false;
                            this.UltraToolbarsManager.Tools["Tracker"].SharedProps.Enabled = _bModifica;
                            this.UltraToolbarsManager.Tools["Cartella"].SharedProps.Enabled = _bModifica;
                            this.UltraToolbarsManager.Tools["Paziente"].SharedProps.Enabled = true;
                            this.UltraToolbarsManager.Tools["Sincronizza"].SharedProps.Enabled = true;
                        }
                        else
                        {
                            this.UltraToolbarsManager.Tools["NuovoAppuntamento"].SharedProps.Enabled = _bInserisci;
                            this.UltraToolbarsManager.Tools["ModificaAppuntamento"].SharedProps.Enabled = false;
                            this.UltraToolbarsManager.Tools["CancellaAppuntamento"].SharedProps.Enabled = false;
                            this.UltraToolbarsManager.Tools["AnnullaAppuntamento"].SharedProps.Enabled = false;
                            this.UltraToolbarsManager.Tools["NuovaNota"].SharedProps.Enabled = _bInserisci;
                            this.UltraToolbarsManager.Tools["ModificaNota"].SharedProps.Enabled = _bModifica;
                            this.UltraToolbarsManager.Tools["CancellaNota"].SharedProps.Enabled = _bCancella;
                            this.UltraToolbarsManager.Tools["Tracker"].SharedProps.Enabled = false;
                            this.UltraToolbarsManager.Tools["Cartella"].SharedProps.Enabled = false;
                            this.UltraToolbarsManager.Tools["Sincronizza"].SharedProps.Enabled = true;
                            this.UltraToolbarsManager.Tools["Paziente"].SharedProps.Enabled = false;
                        }

                    }
                    else
                    {
                        this.UltraToolbarsManager.Tools["NuovoAppuntamento"].SharedProps.Enabled = _bInserisci;
                        this.UltraToolbarsManager.Tools["ModificaAppuntamento"].SharedProps.Enabled = false;
                        this.UltraToolbarsManager.Tools["CancellaAppuntamento"].SharedProps.Enabled = false;
                        this.UltraToolbarsManager.Tools["AnnullaAppuntamento"].SharedProps.Enabled = false;
                        this.UltraToolbarsManager.Tools["NuovaNota"].SharedProps.Enabled = _bInserisci;
                        this.UltraToolbarsManager.Tools["ModificaNota"].SharedProps.Enabled = false;
                        this.UltraToolbarsManager.Tools["CancellaNota"].SharedProps.Enabled = false;
                        this.UltraToolbarsManager.Tools["Tracker"].SharedProps.Enabled = false;
                        this.UltraToolbarsManager.Tools["Cartella"].SharedProps.Enabled = false;
                        this.UltraToolbarsManager.Tools["Sincronizza"].SharedProps.Enabled = true;
                        this.UltraToolbarsManager.Tools["Paziente"].SharedProps.Enabled = false;
                    }
                }
                else
                {
                    this.UltraToolbarsManager.Tools["NuovoAppuntamento"].SharedProps.Enabled = false;
                    this.UltraToolbarsManager.Tools["ModificaAppuntamento"].SharedProps.Enabled = false;
                    this.UltraToolbarsManager.Tools["CancellaAppuntamento"].SharedProps.Enabled = false;
                    this.UltraToolbarsManager.Tools["AnnullaAppuntamento"].SharedProps.Enabled = false;
                    this.UltraToolbarsManager.Tools["NuovaNota"].SharedProps.Enabled = false;
                    this.UltraToolbarsManager.Tools["ModificaNota"].SharedProps.Enabled = false;
                    this.UltraToolbarsManager.Tools["CancellaNota"].SharedProps.Enabled = false;
                    this.UltraToolbarsManager.Tools["Tracker"].SharedProps.Enabled = false;
                    this.UltraToolbarsManager.Tools["Cartella"].SharedProps.Enabled = false;
                    this.UltraToolbarsManager.Tools["Sincronizza"].SharedProps.Enabled = false;
                    this.UltraToolbarsManager.Tools["Paziente"].SharedProps.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void SetInfo(Object sender, CalendarioEventArgs e)
        {

            _bChangeStatoAppuntamento = false;

            try
            {

                CoreStatics.SetContesto(EnumEntita.APP, null);

                if (_ownerselezionato != null)
                {

                    if (_appselezionato != null)
                    {
                        if (((System.Data.DataRowView)_appselezionato.BindingListObject)["flagNota"].ToString() == "0")
                        {
                            string sIDEpisodio = (((System.Data.DataRowView)_appselezionato.BindingListObject)["IDEpisodio"] == null ? "" : ((System.Data.DataRowView)_appselezionato.BindingListObject)["IDEpisodio"].ToString());
                            string sIDTrasferimento = (((System.Data.DataRowView)_appselezionato.BindingListObject)["IDTrasferimento"] == null ? "" : ((System.Data.DataRowView)_appselezionato.BindingListObject)["IDTrasferimento"].ToString());
                            CoreStatics.CoreApplication.Paziente = new Paziente(((System.Data.DataRowView)_appselezionato.BindingListObject)["IDPaziente"].ToString(),
                                                                                sIDEpisodio);
                            if (sIDEpisodio != string.Empty)
                            {
                                CoreStatics.CoreApplication.Episodio = new Episodio(sIDEpisodio);
                            }
                            else
                            {
                                CoreStatics.CoreApplication.Episodio = null;
                            }
                            if (sIDTrasferimento != string.Empty)
                            {
                                CoreStatics.CoreApplication.Trasferimento = new Trasferimento(sIDTrasferimento, CoreStatics.CoreApplication.Ambiente);
                            }
                            else
                            {
                                CoreStatics.CoreApplication.Trasferimento = null;
                            }
                            if (CoreStatics.CoreApplication.Trasferimento != null && CoreStatics.CoreApplication.Trasferimento.NumeroCartella != "")
                            {
                                CoreStatics.CoreApplication.Cartella = new Cartella(CoreStatics.CoreApplication.Trasferimento.IDCartella, CoreStatics.CoreApplication.Trasferimento.NumeroCartella, CoreStatics.CoreApplication.Ambiente);
                            }
                            else
                            {
                                CoreStatics.CoreApplication.Cartella = null;
                            }

                            caricaComboStatoAppuntamento(((System.Data.DataRowView)_appselezionato.BindingListObject)["CodStatoAppuntamento"].ToString());
                            this.uceStatoAppuntamento.Enabled = _bModifica;
                            this.uceStatoAppuntamento.Value = ((System.Data.DataRowView)_appselezionato.BindingListObject)["CodStatoAppuntamento"].ToString();

                            this.lblInfo.Tag = ((System.Data.DataRowView)_appselezionato.BindingListObject)["IDAppuntamento"].ToString();
                            CoreStatics.SetContesto(EnumEntita.APP, this.lblInfo.Tag);

                        }
                        else
                        {
                            CoreStatics.CoreApplication.Paziente = null;
                            CoreStatics.CoreApplication.Episodio = null;
                            CoreStatics.CoreApplication.Trasferimento = null;
                            CoreStatics.CoreApplication.Cartella = null;
                            this.uceStatoAppuntamento.Enabled = false;
                            this.uceStatoAppuntamento.SelectedIndex = -1;
                            this.lblInfo.Tag = string.Empty;
                        }
                        this.lblInfo.Text = string.Format("Dal : {0} - Al : {1}" + Environment.NewLine + "OGGETTO : {2}", _appselezionato.StartDateTime, _appselezionato.EndDateTime, _appselezionato.Subject);
                    }
                    else
                    {
                        CoreStatics.CoreApplication.Paziente = null;
                        CoreStatics.CoreApplication.Episodio = null;
                        CoreStatics.CoreApplication.Trasferimento = null;
                        CoreStatics.CoreApplication.Cartella = null;
                        this.uceStatoAppuntamento.Enabled = false;
                        this.uceStatoAppuntamento.SelectedIndex = -1;
                        this.lblInfo.Text = "";
                        this.lblInfo.Tag = string.Empty;
                    }

                }
                else
                {
                    CoreStatics.CoreApplication.Paziente = null;
                    CoreStatics.CoreApplication.Episodio = null;
                    CoreStatics.CoreApplication.Trasferimento = null;
                    CoreStatics.CoreApplication.Cartella = null;
                    this.uceStatoAppuntamento.Enabled = false;
                    this.uceStatoAppuntamento.SelectedIndex = -1;
                    this.lblInfo.Text = "";
                    this.lblInfo.Tag = string.Empty;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            _bChangeStatoAppuntamento = true;

        }

        private void setPulsanteMax()
        {
            int iStato = 0; try
            {
                Form frm = this.FindForm();
                if (frm != null)
                {
                    if (frm is Interfacce.IViewFormMain)
                    {
                        if ((frm as Interfacce.IViewFormMain).ControlloCentraleMassimizzato)
                            iStato = 2;
                        else
                            iStato = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            try
            {
                switch (iStato)
                {
                    case 1:
                        this.UltraToolbarsManager.Tools["Massimizza"].SharedProps.Visible = true;
                        this.UltraToolbarsManager.Tools["Massimizza"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_CTRL_INGRANDISCI_256);
                        break;
                    case 2:
                        this.UltraToolbarsManager.Tools["Massimizza"].SharedProps.Visible = true;
                        this.UltraToolbarsManager.Tools["Massimizza"].SharedProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_CTRL_RIDUCI_256);
                        break;
                    default:
                        this.UltraToolbarsManager.Tools["Massimizza"].SharedProps.Visible = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void minimizza()
        {
            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                Form frm = this.FindForm();
                if (frm != null && frm is Interfacce.IViewFormMain)
                {
                    if ((frm as Interfacce.IViewFormMain).ControlloCentraleMassimizzato)
                    {
                        (frm as Interfacce.IViewFormMain).ControlloCentraleMassimizzato = false;

                        setPulsanteMax();

                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "minimizza", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }
        }

        private void ApriCartella()
        {
            try
            {

                if (CoreStatics.CoreApplication.Cartella != null)
                {

                    if (_appselezionato != null && ((System.Data.DataRowView)_appselezionato.BindingListObject)["PermessoCartella"].ToString() != "0")
                    {

                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Pazienti_Menu))
                        {

                            if (CoreStatics.CoreApplication.Cartella.CodStatoCartella == EnumStatoCartella.CH.ToString())
                            {
                                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.CartellaPazienteChiusa);
                            }
                            else
                            {
                                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.CartellaPaziente);
                            }

                            CoreStatics.CoreApplication.Navigazione.Maschere.RimuoviMaschereMassimizzabili();
                        }
                        else
                        {
                            easyStatics.EasyMessageBox("Il ruolo selezionato non ha i diritti per aprire il modulo Cartella Ricoverati", "Apri Cartella", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }
                    else
                    {
                        easyStatics.EasyMessageBox("Il ruolo selezionato non ha i diritti per aprire la Cartella " + CoreStatics.CoreApplication.Cartella.NumeroCartella, "Apri Cartella", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }

                }
                else if (CoreStatics.CoreApplication.Paziente != null)
                {
                    DataTable dtCartelle = CoreStatics.CoreApplication.Paziente.CercaCartelle();
                    if (dtCartelle != null && dtCartelle.Rows.Count > 0)
                    {

                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Pazienti_Menu))
                        {

                            if (!dtCartelle.Rows[0].IsNull("IDEpisodio") && dtCartelle.Rows[0]["IDEpisodio"].ToString() != "")
                            {
                                CoreStatics.CoreApplication.Episodio = new Episodio(dtCartelle.Rows[0]["IDEpisodio"].ToString().Trim());
                            }
                            else
                            {
                                CoreStatics.CoreApplication.Episodio = null;
                            }
                            if (!dtCartelle.Rows[0].IsNull("IDTrasferimento") && dtCartelle.Rows[0]["IDTrasferimento"].ToString() != "")
                            {
                                CoreStatics.CoreApplication.Trasferimento = new Trasferimento(dtCartelle.Rows[0]["IDTrasferimento"].ToString().Trim(), CoreStatics.CoreApplication.Ambiente);
                            }
                            else
                            {
                                CoreStatics.CoreApplication.Trasferimento = null;
                            }
                            CoreStatics.CoreApplication.Cartella = new Cartella(dtCartelle.Rows[0]["IDCartella"].ToString().Trim(), "", CoreStatics.CoreApplication.Ambiente);

                            if (CoreStatics.CoreApplication.Cartella != null)
                            {
                                if (CoreStatics.CoreApplication.Cartella.CodStatoCartella == EnumStatoCartella.CH.ToString())
                                {
                                    CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.CartellaPazienteChiusa);
                                }
                                else
                                {
                                    CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.CartellaPaziente);
                                }
                            }

                        }
                        else
                        {
                            easyStatics.EasyMessageBox("Il ruolo selezionato non ha i diritti per aprire il modulo Cartella Ricoverati", "Apri Cartella", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }


                    }
                    else
                    {
                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.CartellaAmbulatoriale_Menu))
                        {

                            CoreStatics.CoreApplication.Episodio = null;
                            CoreStatics.CoreApplication.Trasferimento = null;
                            CoreStatics.CoreApplication.Cartella = null;

                            CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata = string.Empty;
                            CoreStatics.CoreApplication.AmbulatorialeUADescrizioneSelezionata = string.Empty;


                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Ambulatoriale_SelezioneUA) == DialogResult.OK)
                            {

                                CoreStatics.CoreApplication.Paziente.CodUAAmbulatoriale = CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata;
                                CoreStatics.CoreApplication.Paziente.DescrUAAmbulatoriale = CoreStatics.CoreApplication.AmbulatorialeUADescrizioneSelezionata;



                                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Ambulatoriale_Cartella);

                            }

                        }
                        else
                        {
                            easyStatics.EasyMessageBox("Il ruolo selezionato non ha i diritti per aprire il modulo Cartella Ambulatoriale", "Apri Cartella", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }


                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ApriCartella", this.Name);
            }
        }

        private void SvuotaContesto()
        {

            try
            {

                CoreStatics.CoreApplication.Paziente = null;
                CoreStatics.CoreApplication.Episodio = null;
                CoreStatics.CoreApplication.Trasferimento = null;
                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = null;
            }
            catch (Exception)
            {
            }

        }

        private Image getImageFromStatoAppuntamento(string codstatoappuntamento)
        {

            Image image = null;

            try
            {

                T_StatoAppuntamento _statoAppuntamento = new T_StatoAppuntamento();
                _statoAppuntamento.Codice = codstatoappuntamento;
                _statoAppuntamento.TrySelect();

                if (_statoAppuntamento.Icona.Value != null)
                {
                    image = CoreStatics.resizeImage(CoreStatics.ByteToImage(_statoAppuntamento.Icona.Value), new Size(32, 32));
                }

            }
            catch (Exception)
            {

            }

            return image;

        }

        private bool CheckMassimale(string codagenda, DateTime data)
        {

            bool bRet = true;

            try
            {

                if (!dict_MassimaliAgenda.ContainsKey(codagenda))
                {
                    dict_MassimaliAgenda.Add(codagenda, getMassimaleAgenda(codagenda));
                }

                if (dict_MassimaliAgenda[codagenda] != null)
                {
                    int nMassimale = dict_MassimaliAgenda[codagenda].Massimale[(int)data.DayOfWeek];
                    if (nMassimale != 0)
                    {
                        int nNumAppPren = getRisorseDisponibilita(codagenda, data, nMassimale);
                        if (nNumAppPren >= nMassimale)
                        {
                            bRet = false;
                        }
                    }
                }

            }
            catch (Exception)
            {

            }

            return bRet;

        }

        private MassimaliAgenda getMassimaleAgenda(string codagenda)
        {

            MassimaliAgenda _MassimaliAgenda = null;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodAgenda", codagenda);
                op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                op.Parametro.Add("DatiEstesi", "1");
                op.Parametro.Add("SoloFiltroAgenda", "0");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataSet oDs = Database.GetDatasetStoredProc("MSP_SelAgende", spcoll);

                if (oDs != null && oDs.Tables.Count == 1 && oDs.Tables[0].Rows.Count == 1 && !oDs.Tables[0].Rows[0].IsNull("Risorse"))
                {
                    _MassimaliAgenda = XmlProcs.XmlDeserializeFromString<MassimaliAgenda>(oDs.Tables[0].Rows[0]["Risorse"].ToString());
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "getMassimaleAgenda", "Common");
            }

            return _MassimaliAgenda;

        }

        private int getRisorseDisponibilita(string codagenda, DateTime data, int massimale)
        {

            int nRet = 0;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodAgenda", codagenda);

                DateTime dt = data;
                dt = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
                op.Parametro.Add("DataInizio", Database.dataOra105PerParametri(dt));
                op.Parametro.Add("DataFine", Database.dataOra105PerParametri(dt));
                op.Parametro.Add("MassimaleAgenda", massimale.ToString());

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable odt = Database.GetDataTableStoredProc("MSP_CercaPrimaDisponibilitaGiorno", spcoll);
                if (odt.Rows.Count == 1)
                {
                    nRet = (int)odt.Rows[0]["NumAppPren"];
                }
                else
                {
                    nRet = massimale;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "getRisorseDisponibilita", this.Name);
            }

            return nRet;

        }

        #endregion

        #region Events UserControl

        private void ucAgendeTrasversale_Leave(object sender, EventArgs e)
        {
            CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.Calendari.ComandiPin = this.ultraDockManager.ControlPanes["Comandi"].Pinned;
            CoreStatics.CoreApplication.Sessione.Utente.SalvaConfigUtente();
        }

        private void ucAgendeTrasversale_Resize(object sender, EventArgs e)
        {
            CoreStatics.SetResizeUltraToolbarsManager(ref this.UltraToolbarsManager);
        }

        #endregion

        #region Events

        private void ultraDockManager_AfterSplitterDrag(object sender, Infragistics.Win.UltraWinDock.PanesEventArgs e)
        {
            CoreStatics.SetResizeUltraToolbarsManager(ref this.UltraToolbarsManager);
        }

        private void ultraDockManager_InitializePane(object sender, Infragistics.Win.UltraWinDock.InitializePaneEventArgs e)
        {
            e.Pane.Settings.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            e.Pane.Settings.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FILTRO_32);

            int filtroWidth = 12 * (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            this.ultraDockManager.ControlPanes[0].FlyoutSize = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].FlyoutSize.Height);
            this.ultraDockManager.ControlPanes[0].Size = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].Size.Height);
            this.ultraDockManager.DockAreas[0].Size = new Size(filtroWidth, this.ultraDockManager.DockAreas[0].Size.Height);
            this.pnlFiltro.Width = filtroWidth;
        }

        private void UltraToolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.ActionToolClick(e.Tool);
        }

        private void ubFiltra_Click(object sender, EventArgs e)
        {

            try
            {
                this.uchkFiltro.Checked = true;

                if (this.ultraDockManager.FlyoutPane != null && !this.ultraDockManager.FlyoutPane.Pinned) this.ultraDockManager.FlyIn();
                this.ActionToolClick(this.UltraToolbarsManager.OptionSets["Tipo"].SelectedTool);
                this.ucCalendario1.Focus();

            }
            catch (Exception)
            {

            }

        }

        private void uchkFiltro_Click(object sender, EventArgs e)
        {
            if (!this.uchkFiltro.Checked)
            {
                this.ucEasyTextBoxFiltra.Text = "";
                this.ActionToolClick(this.UltraToolbarsManager.OptionSets["Tipo"].SelectedTool);
            }
            else
            {
                this.uchkFiltro.Checked = !this.uchkFiltro.Checked;
            }
        }

        private void uceStatoAppuntamento_ValueChanged(object sender, EventArgs e)
        {

            if (_bChangeStatoAppuntamento == true)
            {
                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento(((System.Data.DataRowView)_appselezionato.BindingListObject)["IDAppuntamento"].ToString(), EnumAzioni.MOD); ;

                if (this.uceStatoAppuntamento.Value.ToString() == EnumStatoAppuntamento.AN.ToString())
                {
                    if (easyStatics.EasyMessageBox("Confermi l'annullamento dell'appuntamento ?", "Annullamento appuntamento", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione = EnumAzioni.ANN;
                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento = EnumStatoAppuntamento.AN.ToString();

                        CoreStatics.CompletaDatiAppuntamento(CoreStatics.CoreApplication.MovAppuntamentoSelezionato);

                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.AnnullaAppuntamento) == DialogResult.OK)
                        {



                            PluginClientStatics.PluginClient(EnumPluginClient.APP_ANNULLA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                        }

                    }
                }
                else
                {
                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento = this.uceStatoAppuntamento.Value.ToString();

                    if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.ER.ToString())
                    {
                        CoreStatics.CompletaDatiAppuntamento(CoreStatics.CoreApplication.MovAppuntamentoSelezionato);
                    }

                    PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Salva();

                    PluginClientStatics.PluginClient(EnumPluginClient.APP_MODIFICA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                    if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.ER.ToString())
                    {
                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Ripianificazione(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente);
                    }

                }

                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);

                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = null;

                this.ActionToolClick(this.UltraToolbarsManager.OptionSets["Tipo"].SelectedTool);

            }

        }

        private void lblInfo_Click(object sender, EventArgs e)
        {

            if (this.lblInfo.Tag != string.Empty)
            {

                MovAppuntamento oMovAppuntamento = new MovAppuntamento(this.lblInfo.Tag.ToString());

                if (oMovAppuntamento.MovScheda.AnteprimaRTF != string.Empty)
                {
                    _enumTipoPopup = enumTipoPopup.Tipo2;
                    CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                    _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(oMovAppuntamento.MovScheda.AnteprimaRTF);
                    this.UltraPopupControlContainer.Show();
                }

                oMovAppuntamento = null;

            }

        }

        #endregion

        #region Events ucCalendario

        private void ucCalendario_CalendarInfoAppointmentDataInitialized(object sender, Infragistics.Win.UltraWinSchedule.AppointmentDataInitializedEventArgs e)
        {

            try
            {

                try
                {
                    Color Colore = CoreStatics.GetColorFromString(((System.Data.DataRowView)e.Appointment.BindingListObject)["Colore"].ToString());
                    e.Appointment.Appearance.BackColor = Colore;

                    if (((System.Data.DataRowView)e.Appointment.BindingListObject)["VisualizzaIconeAppuntamenti"].ToString() == "1")
                    {
                        if (oIcone.ContainsKey(((System.Data.DataRowView)e.Appointment.BindingListObject)["CodStatoAppuntamento"].ToString()) == false)
                        {
                            oIcone.Add(((System.Data.DataRowView)e.Appointment.BindingListObject)["CodStatoAppuntamento"].ToString(), getImageFromStatoAppuntamento(((System.Data.DataRowView)e.Appointment.BindingListObject)["CodStatoAppuntamento"].ToString()));
                        }
                        e.Appointment.Appearance.Image = oIcone[((System.Data.DataRowView)e.Appointment.BindingListObject)["CodStatoAppuntamento"].ToString()];
                        e.Appointment.Appearance.ImageVAlign = Infragistics.Win.VAlign.Bottom;

                    }
                }
                catch (Exception)
                {

                }

                e.Appointment.Locked = true;

            }
            catch (Exception)
            {

            }

        }

        private void ucCalendario_CalendarInfoOwnerDataInitialized(object sender, Infragistics.Win.UltraWinSchedule.OwnerDataInitializedEventArgs e)
        {

            try
            {

                Color Colore = CoreStatics.GetColorFromString(((System.Data.DataRowView)e.Owner.BindingListObject)["Colore"].ToString());
                e.Owner.DayAppearance.BackColor = Colore;
                e.Owner.WorkingHourTimeSlotAppearance.BackColor = Color.LightGreen;
                e.Owner.HeaderAppearance.BackColor = Colore;

                WorkingHourTime oWht = XmlProcs.XmlDeserializeFromString<WorkingHourTime>(((System.Data.DataRowView)e.Owner.BindingListObject)["OrariLavoro"].ToString());

                e.Owner.ResetDateSettings();

                for (int x = 0; x < oWht.HourI.Length; x++)
                {
                    e.Owner.DayOfWeekSettings[(System.DayOfWeek)x].WorkingHours.Add(
                        new TimeSpan(int.Parse(oWht.HourI[x].ToString().Substring(0, 2)), int.Parse(oWht.HourI[x].ToString().Substring(3, 2)), 0),
                        new TimeSpan(int.Parse(oWht.HourF[x].ToString().Substring(0, 2)), int.Parse(oWht.HourF[x].ToString().Substring(3, 2)), 0));
                }

            }
            catch (Exception)
            {

            }
        }

        private void ucCalendario_CalendarioDragDrop(object sender, DragEventArgs e)
        {

            DateTime oStartDate;
            DateTime oEndDate;
            Infragistics.Win.UltraWinSchedule.Day oDay;
            string sOwnerKey = string.Empty;

            try
            {

                if (e.Data.GetDataPresent(typeof(Appointment)))
                {

                    Appointment oApp = e.Data.GetData(typeof(Appointment)) as Appointment;

                    Type ViewType = sender.GetType();
                    MethodInfo ViewMethod = ViewType.GetMethod("PointToClient");
                    Point oPt = (Point)ViewMethod.Invoke(sender, new object[] { new Point(e.X, e.Y) });
                    ViewMethod = ViewType.GetMethod("GetOwnerFromPoint", new[] { typeof(Point) });
                    sOwnerKey = ((Owner)ViewMethod.Invoke(sender, new object[] { oPt })).Key;


                    if (sender.GetType() == typeof(UltraDayView))
                    {
                        oDay = ((UltraDayView)sender).GetVisibleDayFromPoint(oPt).Day;
                        TimeSpan oTSpan = oApp.EndDateTime.Subtract(oApp.StartDateTime);
                        TimeSlot oTSlot = ((UltraDayView)sender).GetTimeSlotFromPoint(oPt);
                        oStartDate = new DateTime(oDay.Date.Year, oDay.Date.Month, oDay.Date.Day, oTSlot.StartTime.Hour, oTSlot.StartTime.Minute, oTSlot.StartTime.Second);
                        oEndDate = oStartDate.Add(oTSpan);
                    }
                    else
                    {
                        ViewMethod = ViewType.GetMethod("GetDayFromPoint", new[] { typeof(Point) });
                        oDay = (Infragistics.Win.UltraWinSchedule.Day)ViewMethod.Invoke(sender, new object[] { oPt });
                        oStartDate = new DateTime(oDay.Date.Year, oDay.Date.Month, oDay.Date.Day, oApp.StartDateTime.Hour, oApp.StartDateTime.Minute, oApp.StartDateTime.Second);
                        oEndDate = new DateTime(oDay.Date.Year, oDay.Date.Month, oDay.Date.Day, oApp.EndDateTime.Hour, oApp.EndDateTime.Minute, oApp.EndDateTime.Second);
                    }

                    if (CheckMassimale(sOwnerKey, oStartDate))
                    {


                        if (oApp.OwnerKey == sOwnerKey)
                        {
                            if (((System.Data.DataRowView)oApp.BindingListObject)["flagNota"].ToString() == "0")
                            {

                                MovAppuntamento movAppSelOriginale = CoreStatics.CoreApplication.MovAppuntamentoSelezionato;

                                MovAppuntamento oMovAppuntamento = new MovAppuntamento(((System.Data.DataRowView)oApp.BindingListObject)["IDAppuntamento"].ToString(), EnumAzioni.MOD);
                                oMovAppuntamento.DataInizio = oStartDate;
                                oMovAppuntamento.DataFine = oEndDate;

                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = oMovAppuntamento;
                                PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                oMovAppuntamento = CoreStatics.CoreApplication.MovAppuntamentoSelezionato;

                                oMovAppuntamento.Salva();

                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = oMovAppuntamento;
                                PluginClientStatics.PluginClient(EnumPluginClient.APP_MODIFICA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                oMovAppuntamento = null;

                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = movAppSelOriginale;
                            }
                            else
                            {
                                MovNoteAgende oMovNoteAgende = new MovNoteAgende(((System.Data.DataRowView)oApp.BindingListObject)["ID"].ToString(), EnumAzioni.MOD);
                                oMovNoteAgende.DataInizio = oStartDate;
                                oMovNoteAgende.DataFine = oEndDate;
                                oMovNoteAgende.Salva();
                                oMovNoteAgende = null;
                            }
                            this.ActionToolClick(this.UltraToolbarsManager.OptionSets["Tipo"].SelectedTool);
                        }
                        else
                        {
                            if (((System.Data.DataRowView)oApp.BindingListObject)["flagNota"].ToString() == "0")
                            {

                                MovAppuntamento movAppSelOriginale = CoreStatics.CoreApplication.MovAppuntamentoSelezionato;

                                MovAppuntamento oMovAppuntamento = new MovAppuntamento(((System.Data.DataRowView)oApp.BindingListObject)["IDAppuntamento"].ToString(), EnumAzioni.MOD);

                                MovAppuntamentoAgende oMaa = oMovAppuntamento.Elementi.Find(MovAppuntamentoAgende => MovAppuntamentoAgende.CodAgenda == sOwnerKey);
                                if (oMaa == null)
                                {
                                    easyStatics.EasyMessageBox("Appuntamento NON associabile a questa agenda!", "Agende", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                                else
                                {
                                    if (oMaa.Selezionata == true)
                                    {
                                        easyStatics.EasyMessageBox("Appuntamento già presente in questa agenda!", "Agende", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    }
                                    else
                                    {
                                        oMaa.Selezionata = true;
                                        oMaa.Modificata = true;

                                        oMovAppuntamento.ElencoRisorse += @", " + oMaa.Descrizione;

                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato = oMovAppuntamento;
                                        PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                        oMovAppuntamento = CoreStatics.CoreApplication.MovAppuntamentoSelezionato;

                                        oMovAppuntamento.Salva();

                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato = oMovAppuntamento;
                                        PluginClientStatics.PluginClient(EnumPluginClient.APP_MODIFICA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                        this.ActionToolClick(this.UltraToolbarsManager.OptionSets["Tipo"].SelectedTool);
                                    }
                                }
                                oMovAppuntamento = null;

                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = movAppSelOriginale;
                            }
                            else
                            {
                                MovNoteAgende oMovNoteAgende = new MovNoteAgende();
                                oMovNoteAgende.Azione = EnumAzioni.INS;
                                oMovNoteAgende.Oggetto = oApp.Subject;
                                oMovNoteAgende.Descrizione = oApp.Description;
                                oMovNoteAgende.DataInizio = oStartDate;
                                oMovNoteAgende.DataFine = oEndDate;
                                oMovNoteAgende.Colore = oApp.Appearance.BackColor.ToString();
                                oMovNoteAgende.CodStatoNota = EnumStatoNotaAgenda.PR.ToString();
                                oMovNoteAgende.CodAgenda = sOwnerKey;
                                oMovNoteAgende.Salva();
                                oMovNoteAgende = null;
                                this.ActionToolClick(this.UltraToolbarsManager.OptionSets["Tipo"].SelectedTool);
                            }
                        }
                    }
                    else
                    {
                        easyStatics.EasyMessageBox("Massimale Agenda/Giorno RAGGIUNTO!", "Agende", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void ucCalendario_CalendarioDragOver(object sender, DragEventArgs e)
        {

            try
            {

                if (e.Data.GetDataPresent(typeof(Appointment)) && e.AllowedEffect != DragDropEffects.None)
                {
                    e.Effect = DragDropEffects.Copy;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void ucCalendario_CalendarioMouseDown(object sender, MouseEventArgs e)
        {

            try
            {

                UltraCalendarInfo oUci = null;
                Appointment oApp = null;

                if (sender.GetType() == typeof(UltraDayView))
                {
                    oUci = ((UltraDayView)sender).CalendarInfo;
                    oApp = ((UltraDayView)sender).GetAppointmentFromPoint(e.X, e.Y);
                    _selectedtimeslotrange = ((UltraDayView)sender).SelectedTimeSlotRange;
                    _timeSlotInterval = ((UltraDayView)sender).TimeSlotInterval;
                }
                else if (sender.GetType() == typeof(UltraWeekView))
                {
                    oUci = ((UltraWeekView)sender).CalendarInfo;
                    oApp = ((UltraWeekView)sender).GetAppointmentFromPoint(e.X, e.Y);
                    _selectedtimeslotrange = null;
                    _timeSlotInterval = TimeSlotInterval.TenMinutes;
                }
                else if (sender.GetType() == typeof(UltraMonthViewSingle))
                {
                    oUci = ((UltraMonthViewSingle)sender).CalendarInfo;
                    oApp = ((UltraMonthViewSingle)sender).GetAppointmentFromPoint(e.X, e.Y);
                    _selectedtimeslotrange = null;
                    _timeSlotInterval = TimeSlotInterval.TenMinutes;
                }

                if (e.Button == MouseButtons.Left)
                {
                    if (oUci.SelectedAppointments.Count == 0)
                    {
                        if (oApp != null && oApp.Selected == false)
                        {
                            oUci.SelectedAppointments.Clear();
                            oApp.Selected = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void ucCalendario_CalendarioMouseMove(object sender, MouseEventArgs e)
        {

            try
            {

                UltraCalendarInfo oUci = null;

                if (sender.GetType() == typeof(UltraDayView))
                {
                    oUci = ((UltraDayView)sender).CalendarInfo;
                }
                else if (sender.GetType() == typeof(UltraWeekView))
                {
                    oUci = ((UltraWeekView)sender).CalendarInfo;
                }
                else if (sender.GetType() == typeof(UltraMonthViewSingle))
                {
                    oUci = ((UltraMonthViewSingle)sender).CalendarInfo;
                }

                Type ViewType = sender.GetType();
                MethodInfo ViewMethod = ViewType.GetMethod("DoDragDrop");

                if (e.Button == MouseButtons.Left)
                {
                    if (oUci.SelectedAppointments.Count == 1 && _bModifica == true)
                    {
                        ViewMethod.Invoke(sender, new object[] { oUci.SelectedAppointments[0], DragDropEffects.All });
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void ucCalendario_DayNumberClick(object sender, DayNumberClickEventArgs e)
        {

            try
            {

                _enumTipoPopup = enumTipoPopup.Tipo1;

                _ucEasyGrid = null;
                _ucEasyGrid = CoreStatics.getGridAppuntamentixTipo(new DateTime(e.Day.Date.Year, e.Day.Date.Month, e.Day.Date.Day, 0, 0, 0),
                                                                    new DateTime(e.Day.Date.Year, e.Day.Date.Month, e.Day.Date.Day, 23, 59, 59),
                                                                    (e.Owner != null ? e.Owner.Key : ""));

                if (_ucEasyGrid != null && _ucEasyGrid.DataSource != null)
                {

                    _ucEasyGrid.Size = new Size(400, 200);

                    _ucEasyGrid.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                    CoreStatics.SetEasyUltraGridLayout(ref _ucEasyGrid);

                    CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);



                    this.UltraPopupControlContainer.Show();


                    _ucEasyGrid.DisplayLayout.Bands[0].ClearGroupByColumns();

                    _ucEasyGrid.DisplayLayout.Bands[0].Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
                    _ucEasyGrid.Refresh();
                }

            }
            catch (Exception)
            {

            }

        }

        private void ucCalendario_SelectedCalendario(object sender, CalendarioEventArgs e)
        {

            _appselezionato = null;
            _ownerselezionato = null;
            _dayselezionato = null;
            _selectedtimeslotrange = null;

            try
            {

                if (e.Owner != null)
                {

                    UltraCalendarInfo oUci = ((UltraCalendarInfo)sender);

                    _ownerselezionato = e.Owner;

                    if (oUci.SelectedAppointments.Count == 1)
                    {
                        _appselezionato = oUci.SelectedAppointments[0];
                    }

                    _dayselezionato = oUci.ActiveDay;
                }

                this.SetUltraToolbars(sender, e);
                this.SetInfo(sender, e);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

        #region UltraPopupControlContainer

        private void UltraPopupControlContainer_Closed(object sender, EventArgs e)
        {

            switch (_enumTipoPopup)
            {

                case enumTipoPopup.Nessuno:
                    break;

                case enumTipoPopup.Tipo1:
                    _ucEasyGrid.ClickCell -= ucEasyGrid_ClickCell;
                    break;

                case enumTipoPopup.Tipo2:
                    _ucRichTextBox.RtfClick -= ucRichTextBox_Click;
                    break;

            }

        }

        private void UltraPopupControlContainer_Opened(object sender, EventArgs e)
        {

            switch (_enumTipoPopup)
            {

                case enumTipoPopup.Nessuno:
                    break;

                case enumTipoPopup.Tipo1:
                    _ucEasyGrid.ClickCell += ucEasyGrid_ClickCell;
                    _ucEasyGrid.Focus();
                    break;

                case enumTipoPopup.Tipo2:
                    _ucRichTextBox.RtfClick += ucRichTextBox_Click;
                    break;

            }

        }

        private void UltraPopupControlContainer_Opening(object sender, CancelEventArgs e)
        {

            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;

            switch (_enumTipoPopup)
            {

                case enumTipoPopup.Nessuno:
                    break;

                case enumTipoPopup.Tipo1:
                    popup.PopupControl = _ucEasyGrid;
                    break;

                case enumTipoPopup.Tipo2:
                    popup.PopupControl = _ucRichTextBox;
                    break;

            }

        }

        private void ucEasyGrid_ClickCell(object sender, ClickCellEventArgs e)
        {

            switch (_enumTipoPopup)
            {

                case enumTipoPopup.Nessuno:
                    break;

                case enumTipoPopup.Tipo1:
                    this.UltraPopupControlContainer.Close();
                    break;

                case enumTipoPopup.Tipo2:
                    break;

            }

        }

        private void ucRichTextBox_Click(object sender, EventArgs e)
        {

        }

        #endregion

    }
}
