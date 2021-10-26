using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinTree;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinSchedule;
using Infragistics.Win.UltraWinSchedule.TimelineView;
using Infragistics.Win.UltraWinToolbars;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using Infragistics.Win.UltraWinEditors;
using UnicodeSrl.ScciCore.WebSvc;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.Scci.PluginClient;
using System.Reflection;
using UnicodeSrl.ScciCore.Common.Extensions;

namespace UnicodeSrl.ScciCore
{
    public partial class ucFoglioUnico : UserControl, Interfacce.IViewUserControlMiddle
    {

        public ucFoglioUnico()
        {
            InitializeComponent();
            _ucc = (UserControl)this;
        }

        #region Declare

        UserControl _ucc = null;

        private ucRichTextBox _ucRichTextBox = null;
        private ucEasyGrid _ucEasyGridOrari = null;
        private ucEasyPopUpOrario _ucEasyPopUpOrario;
        private ucEasyPopUpTerapiaRapida _ucEasyPopUpTerapiaRapida;

        private ucEasyPopUpNota _ucEasyPopUpNota;

        private Gestore oGestore = null;

        private Dictionary<string, byte[]> oIcone = new Dictionary<string, byte[]>();

        private Color _colorecambiogiorno = Color.Empty;

        int timerinterval = 0;

        private bool _locknuovoordine = false;

        #endregion

        #region Interface

        public void Aggiorna()
        {
            this.Aggiorna(string.Empty);
        }
        public void Aggiorna(string ID)
        {

            CoreStatics.SetNavigazione(false);

            try
            {

                this.VerificaSicurezza();
                this.LoadUltraCalendarInfo(ID);
                this.CalcolaDimensioneColonne();
                foreach (Owner o in this.UltraCalendarInfo.Owners)
                {
                    this.SetWorkingHour(o);
                }
                this.UltraTimelineView.Refresh();

                AggiornaNomePaziente();

                setPulsanteMax();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Aggiorna", this.Name);
            }

            CoreStatics.SetNavigazione(true);


        }

        private void AggiornaNomePaziente()
        {
            var oPaziente = CoreStatics.CoreApplication.Paziente;
            var oEpisodio = CoreStatics.CoreApplication.Episodio;
            var oTrasferimento = CoreStatics.CoreApplication.Trasferimento;

            this.lblPaziente.Text = string.Empty;
            if (oPaziente != null && oPaziente.Attivo == true)
            {
                this.lblPaziente.Text = string.Format("{0} {1} - Nato il {2} ", oPaziente.Cognome, oPaziente.Nome, oPaziente.DataNascita.ToShortDateString());
            }
            if (oTrasferimento != null && oTrasferimento.Attivo == true)
            {
                this.lblPaziente.Text += string.Format("- N. Cartella: {0} ", oTrasferimento.NumeroCartella);
            }
            if (oEpisodio != null && oEpisodio.Attivo == true)
            {
                this.lblPaziente.Text += string.Format("- Nosologico: {0}", (oEpisodio.NumeroEpisodio != string.Empty ? oEpisodio.NumeroEpisodio : oEpisodio.NumeroListaAttesa));
            }
        }

        public void Carica()
        {

            CoreStatics.SetNavigazione(false);

            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

            try
            {
                AggiornaNomePaziente();

                _colorecambiogiorno = CoreStatics.GetColorFromString(Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.FUTColoreCambioGiorno));

                this.ubIndietro.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FRECCIASX_256);
                this.ubIndietro.PercImageFill = 0.75F;
                this.ubIndietro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ubAvanti.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FRECCIADX_256);
                this.ubAvanti.PercImageFill = 0.75F;
                this.ubAvanti.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ubProsegui.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_PROSEGUITERAPIA_256);
                this.ubProsegui.PercImageFill = 0.75F;
                this.ubProsegui.ShortcutKey = Keys.P;
                this.ubProsegui.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.ubProsegui.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                CoreStatics.SetEasyUltraDockManager(ref this.UltraDockManager);
                CoreStatics.SetUltraCalendarInfo(this.UltraCalendarInfo);
                CoreStatics.SetUltraCalendarLook(this.UltraCalendarLook);
                this.UltraCalendarLook.ViewStyle = Infragistics.Win.UltraWinSchedule.ViewStyle.Default;

                this.VerificaSicurezza();
                this.InizializzaUltraTimelineView();
                this.InizializzaUltraTreeView();
                this.CaricaUltraTreeView();

                string sValue = UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.TimerFUT);
                if (sValue.Trim() != "")
                {
                    if (!int.TryParse(sValue.Trim(), out timerinterval)) timerinterval = 0;
                }
                if (timerinterval != 0) { this.TimerChange.Interval = timerinterval; }

                this.uceGridStep.SelectedIndex = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.FoglioUnico.Step;
                this.udteFiltroDA.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                this.udteFiltroA.DateTime = (DateTime)this.udteFiltroDA.DateTime.AddDays(1);

                this.uceRange.SelectedIndex = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.FoglioUnico.Range;
                this.chkFuoriOrario.Checked = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.FoglioUnico.MostraNotte;
                this.chkSoloPresenti.Checked = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.FoglioUnico.SoloAttivi;

                CheckFuoriOrario();

                this.chkTipoVisualizzazione.Checked = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.FoglioUnico.TipoVisualizzazione;
                this.SetTipoVisualizzazione();
                setPulsanteMax();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }

            CoreStatics.SetNavigazione(true);

            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);

        }

        public void Ferma()
        {

            try
            {

                oIcone = new Dictionary<string, byte[]>();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region PRIVATE

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
                        this.ubMaximize.Visible = true;
                        this.ubMaximize.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_CTRL_INGRANDISCI_256);
                        this.ubMaximize.PercImageFill = 0.75F;
                        this.lblPaziente.Visible = false;
                        this.ucEasyTableLayoutPanel.RowStyles[0].Height = 0;
                        break;
                    case 2:
                        this.ubMaximize.Visible = true;
                        this.ubMaximize.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_CTRL_RIDUCI_256);
                        this.ubMaximize.PercImageFill = 0.75F;
                        this.lblPaziente.Visible = true;
                        this.ucEasyTableLayoutPanel.RowStyles[0].Height = 20;
                        break;
                    default:
                        this.ubMaximize.Visible = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void AzionePredefinita()
        {

            try
            {

                if (this.UltraCalendarInfo.SelectedAppointments.Count == 1)
                {

                    DataRowView oRowView = (System.Data.DataRowView)this.UltraCalendarInfo.SelectedAppointments[0].BindingListObject;

                    if (oRowView["PermessoModifica"].ToString() == "1" || oRowView["PermessoVisualizza"].ToString() == "1" || oRowView["PermessoEroga"].ToString() == "1")
                    {

                        switch ((EnumEntita)Enum.Parse(typeof(EnumEntita), oRowView["CodEntita"].ToString()))
                        {

                            case EnumEntita.NTG:
                                CoreStatics.CoreApplication.MovNotaSelezionata = new MovNota(oRowView["IDRiferimento"].ToString(), EnumAzioni.MOD);
                                _ucEasyPopUpNota = new ucEasyPopUpNota();
                                _ucEasyPopUpNota.DataOra = CoreStatics.CoreApplication.MovNotaSelezionata.DataInizio;
                                _ucEasyPopUpNota.Nota = CoreStatics.CoreApplication.MovNotaSelezionata.Oggetto;
                                _ucEasyPopUpNota.Cancella = true;
                                this.UltraPopupControlContainerNote.Show();
                                break;

                            case EnumEntita.PVT:
                                CoreStatics.CoreApplication.MovParametroVitaleSelezionato = new MovParametroVitale(oRowView["IDRiferimento"].ToString(), (oRowView["PermessoModifica"].ToString() == "1" ? EnumAzioni.MOD : EnumAzioni.VIS), CoreStatics.CoreApplication.Ambiente);
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingParametriVitali) == DialogResult.OK)
                                {
                                    this.Aggiorna();
                                }
                                break;

                            case EnumEntita.WKI:
                            case EnumEntita.PRF:
                                if (oRowView["PermessoModifica"].ToString() == "0")
                                {
                                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(oRowView["IDRiferimento"].ToString(),
                                                                                                        EnumAzioni.VIS,
                                                                                                        CoreStatics.CoreApplication.Ambiente);
                                    CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.ErogazioneTaskInfermieristici);
                                }
                                else if (oRowView["PermessoModifica"].ToString() == "1")
                                {
                                    this.Eroga(oRowView["IDRiferimento"].ToString());
                                    this.Aggiorna();
                                }
                                break;

                            case EnumEntita.EVC:
                                CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = new MovEvidenzaClinica(oRowView["IDRiferimento"].ToString(), "", EnumAzioni.VIS);
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingEvidenzaClinica) == DialogResult.OK)
                                {
                                    this.Aggiorna();
                                }
                                break;

                            case EnumEntita.APP:
                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento(oRowView["IDRiferimento"].ToString(), (oRowView["PermessoModifica"].ToString() == "1" ? EnumAzioni.MOD : EnumAzioni.VIS));
                                while (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneAgendeAppuntamento) == DialogResult.OK)
                                {
                                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneAppuntamento) == DialogResult.OK)
                                    {



                                        PluginClientStatics.PluginClient(EnumPluginClient.APP_MODIFICA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                        this.Aggiorna();
                                        break;
                                    }
                                }
                                break;

                            case EnumEntita.DCL:
                                CoreStatics.CoreApplication.MovDiarioClinicoSelezionato = new MovDiarioClinico(oRowView["IDRiferimento"].ToString(), (oRowView["PermessoModifica"].ToString() == "1" ? EnumAzioni.VAL : EnumAzioni.VIS), CoreStatics.CoreApplication.Ambiente);
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingVoceDiDiario) == DialogResult.OK)
                                {
                                    this.Aggiorna();
                                }
                                break;

                        }

                    }


                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "AzionePredefinita", "ucFoglioUnico");
            }

        }

        private void AzionePredefinitaRapida()
        {

            try
            {

                if (this.UltraCalendarInfo.SelectedAppointments.Count == 1)
                {

                    DataRowView oRowView = (System.Data.DataRowView)this.UltraCalendarInfo.SelectedAppointments[0].BindingListObject;

                    if (oRowView["PermessoModifica"].ToString() == "1" || oRowView["PermessoVisualizza"].ToString() == "1" || oRowView["PermessoEroga"].ToString() == "1")
                    {

                        switch ((EnumEntita)Enum.Parse(typeof(EnumEntita), oRowView["CodEntita"].ToString()))
                        {

                            case EnumEntita.NTG:
                                break;

                            case EnumEntita.PVT:
                                break;

                            case EnumEntita.WKI:
                            case EnumEntita.PRF:
                                if (oRowView["PermessoModifica"].ToString() == "0")
                                {
                                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(oRowView["IDRiferimento"].ToString(),
                                                                                                        EnumAzioni.VIS,
                                                                                                        CoreStatics.CoreApplication.Ambiente);
                                    CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.ErogazioneTaskInfermieristici);
                                }
                                else if (oRowView["PermessoModifica"].ToString() == "1")
                                {
                                    this.ErogazioneRapida(oRowView["IDRiferimento"].ToString());
                                    this.Aggiorna();
                                }
                                break;

                            case EnumEntita.EVC:
                                break;

                            case EnumEntita.APP:
                                break;

                            case EnumEntita.DCL:
                                break;

                            case EnumEntita.OE:
                                break;

                        }

                    }


                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "AzionePredefinitaRapida", "ucFoglioUnico");
            }

        }

        private void AzioneMenu()
        {

            var btnMenu = (PopupMenuTool)this.UltraToolbarsManager.Tools["Menu"];

            try
            {

                if (this.UltraCalendarInfo.SelectedAppointments.Count == 1)
                {

                    DataRowView oRowView = (System.Data.DataRowView)this.UltraCalendarInfo.SelectedAppointments[0].BindingListObject;

                    if (oRowView["PermessoModifica"].ToString() == "1" || oRowView["PermessoVisualizza"].ToString() == "1" || oRowView["PermessoEroga"].ToString() == "1")
                    {

                        ((ButtonTool)btnMenu.Tools["ErogaRapida"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_EROGAZIONERAPIDA_32);
                        ((ButtonTool)btnMenu.Tools["Eroga"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_SI_32);
                        ((ButtonTool)btnMenu.Tools["Modifica"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICA_32);
                        ((ButtonTool)btnMenu.Tools["Sposta"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICA_ORARIO_32);
                        ((ButtonTool)btnMenu.Tools["Annulla"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NO_32);
                        ((ButtonTool)btnMenu.Tools["Cancella"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_ELIMINA_32);
                        ((ButtonTool)btnMenu.Tools["CambiaStato"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_CONFIGURAZIONE_TABLE_32);
                        ((ButtonTool)btnMenu.Tools["Visualizza"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_VISUALIZZA_32);
                        ((ButtonTool)btnMenu.Tools["Copia"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_COPIA_32);
                        ((ButtonTool)btnMenu.Tools["Inoltra"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FRECCIA_DX_32);

                        switch ((EnumEntita)Enum.Parse(typeof(EnumEntita), oRowView["CodEntita"].ToString()))
                        {

                            case EnumEntita.PVT:
                                ((ButtonTool)btnMenu.Tools["ErogaRapida"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Eroga"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Modifica"]).InstanceProps.Visible = (oRowView["PermessoModifica"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["Sposta"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Annulla"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Cancella"]).InstanceProps.Visible = (oRowView["PermessoCancella"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["CambiaStato"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Visualizza"]).InstanceProps.Visible = (oRowView["PermessoVisualizza"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["Copia"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Inoltra"]).InstanceProps.Visible = DefaultableBoolean.False;
                                if (CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                                {
                                    ((ButtonTool)btnMenu.Tools["ErogaRapida"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Eroga"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Modifica"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Sposta"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Annulla"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Cancella"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["CambiaStato"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Copia"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Inoltra"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    if (oRowView["PermessoVisualizza"].ToString() == "1")
                                    {
                                        btnMenu.ShowPopup();
                                    }
                                }
                                else
                                {
                                    btnMenu.ShowPopup();
                                }
                                break;

                            case EnumEntita.WKI:
                                ((ButtonTool)btnMenu.Tools["ErogaRapida"]).InstanceProps.Visible = (oRowView["PermessoEroga"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["Eroga"]).InstanceProps.Visible = (oRowView["PermessoEroga"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["Modifica"]).InstanceProps.Visible = (oRowView["PermessoModifica"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["Sposta"]).InstanceProps.Visible = (oRowView["PermessoModifica"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["Annulla"]).InstanceProps.Visible = (oRowView["PermessoAnnulla"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["Cancella"]).InstanceProps.Visible = (oRowView["PermessoCancella"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["CambiaStato"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Visualizza"]).InstanceProps.Visible = (oRowView["PermessoVisualizza"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["Copia"]).InstanceProps.Visible = (oRowView["PermessoInserimento"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["Inoltra"]).InstanceProps.Visible = DefaultableBoolean.False;
                                if (CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                                {
                                    ((ButtonTool)btnMenu.Tools["ErogaRapida"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Eroga"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Modifica"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Sposta"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Annulla"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Cancella"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["CambiaStato"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Copia"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Inoltra"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    if (oRowView["PermessoVisualizza"].ToString() == "1")
                                    {
                                        btnMenu.ShowPopup();
                                    }
                                }
                                else
                                {
                                    btnMenu.ShowPopup();
                                }
                                break;

                            case EnumEntita.PRF:
                                ((ButtonTool)btnMenu.Tools["ErogaRapida"]).InstanceProps.Visible = (oRowView["PermessoEroga"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["Eroga"]).InstanceProps.Visible = (oRowView["PermessoEroga"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["Modifica"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Sposta"]).InstanceProps.Visible = (oRowView["PermessoModifica"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["Annulla"]).InstanceProps.Visible = (oRowView["PermessoAnnulla"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["Cancella"]).InstanceProps.Visible = (oRowView["PermessoCancella"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["CambiaStato"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Visualizza"]).InstanceProps.Visible = (oRowView["PermessoVisualizza"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["Copia"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Inoltra"]).InstanceProps.Visible = DefaultableBoolean.False;
                                if (CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                                {
                                    ((ButtonTool)btnMenu.Tools["ErogaRapida"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Eroga"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Modifica"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Sposta"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Annulla"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Cancella"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["CambiaStato"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Copia"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Inoltra"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    if (oRowView["PermessoVisualizza"].ToString() == "1")
                                    {
                                        btnMenu.ShowPopup();
                                    }
                                }
                                else
                                {
                                    btnMenu.ShowPopup();
                                }
                                break;

                            case EnumEntita.APP:
                                ((ButtonTool)btnMenu.Tools["ErogaRapida"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Eroga"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Modifica"]).InstanceProps.Visible = (oRowView["PermessoModifica"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["Sposta"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Annulla"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Cancella"]).InstanceProps.Visible = (oRowView["PermessoCancella"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["CambiaStato"]).InstanceProps.Visible = (oRowView["PermessoCambiaStato"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["Visualizza"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Copia"]).InstanceProps.Visible = (oRowView["PermessoInserimento"].ToString() == "1" ? DefaultableBoolean.True : DefaultableBoolean.False);
                                ((ButtonTool)btnMenu.Tools["Inoltra"]).InstanceProps.Visible = DefaultableBoolean.False;
                                if (CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                                {
                                }
                                else
                                {
                                    btnMenu.ShowPopup();
                                }
                                break;

                            case EnumEntita.OE:

                                bool OEPermessoModifica = false;
                                bool OEPermessoInoltra = false;
                                bool OEPermessoCancella = false;
                                bool OEPermessoVisualizza = false;
                                bool OEPermessoCopia = false;

                                CalcolaPermessiOE(oRowView["Valore"].ToString(), out OEPermessoModifica, out OEPermessoInoltra,
                                                  out OEPermessoCancella, out OEPermessoVisualizza, out OEPermessoCopia);

                                ((ButtonTool)btnMenu.Tools["ErogaRapida"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Eroga"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Modifica"]).InstanceProps.Visible = ConvertBoolToDefaultableBoolean(OEPermessoModifica);
                                ((ButtonTool)btnMenu.Tools["Sposta"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Annulla"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Cancella"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["CambiaStato"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Visualizza"]).InstanceProps.Visible = ConvertBoolToDefaultableBoolean(OEPermessoVisualizza);
                                ((ButtonTool)btnMenu.Tools["Copia"]).InstanceProps.Visible = DefaultableBoolean.False;
                                ((ButtonTool)btnMenu.Tools["Inoltra"]).InstanceProps.Visible = ConvertBoolToDefaultableBoolean(OEPermessoInoltra);

                                if (CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                                {
                                    ((ButtonTool)btnMenu.Tools["Eroga"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Modifica"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Sposta"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Annulla"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Cancella"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["CambiaStato"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Copia"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    ((ButtonTool)btnMenu.Tools["Inoltra"]).InstanceProps.Visible = DefaultableBoolean.False;
                                    if (oRowView["PermessoVisualizza"].ToString() == "1")
                                    {
                                        btnMenu.ShowPopup();
                                    }
                                }
                                else
                                {
                                    btnMenu.ShowPopup();
                                }

                                break;
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "AzioneMenu", "ucFoglioUnico");
            }

        }

        private void AzioneMenuPrescrizione(string codvoce, DateTime dataevento)
        {

            var btnMenu = (PopupMenuTool)this.UltraToolbarsManager.Tools["MenuPrescrizione"];

            btnMenu.Tag = codvoce;

            try
            {

                if (codvoce == string.Empty)
                {
                    ((ButtonTool)btnMenu.Tools["Continua"]).InstanceProps.Visible = DefaultableBoolean.False;
                    ((ButtonTool)btnMenu.Tools["Singola"]).InstanceProps.Visible = DefaultableBoolean.False;
                    ((ButtonTool)btnMenu.Tools["Somministrazione"]).InstanceProps.Visible = DefaultableBoolean.False;
                }
                else
                {
                    ((ButtonTool)btnMenu.Tools["Continua"]).InstanceProps.Visible = DefaultableBoolean.True;
                    ((ButtonTool)btnMenu.Tools["Singola"]).InstanceProps.Visible = DefaultableBoolean.True;
                    ((ButtonTool)btnMenu.Tools["Somministrazione"]).InstanceProps.Visible = DefaultableBoolean.True;
                }
                ((ButtonTool)btnMenu.Tools["Continua"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGGIUNGICONTINUA_256);
                ((ButtonTool)btnMenu.Tools["Continua"]).Tag = dataevento;
                ((ButtonTool)btnMenu.Tools["Singola"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGGIUNGISINGOLA_256);
                ((ButtonTool)btnMenu.Tools["Singola"]).Tag = dataevento;
                ((ButtonTool)btnMenu.Tools["Somministrazione"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGGIUNGISOMMINISTRAZIONE_256);
                ((ButtonTool)btnMenu.Tools["Somministrazione"]).Tag = dataevento;
                ((ButtonTool)btnMenu.Tools["TerapiaRapida"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGGIUNGITERAPIARAPIDA_256);
                ((ButtonTool)btnMenu.Tools["TerapiaRapida"]).Tag = dataevento;
                ((ButtonTool)btnMenu.Tools["Terapia"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_AGGIUNGITERAPIA_256);
                ((ButtonTool)btnMenu.Tools["Terapia"]).Tag = dataevento;
                ((ButtonTool)btnMenu.Tools["ProseguiTerapia"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_PROSEGUITERAPIA_256);
                ((ButtonTool)btnMenu.Tools["ProseguiTerapia"]).Tag = dataevento;
                btnMenu.ShowPopup();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "AzioneMenuPrescrizione", "ucFoglioUnico");
            }

        }

        private string GeneraCodAgenda(string codvoce, string codsezione)
        {

            string sRet = string.Empty;

            try
            {
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodSezione", codsezione);
                op.Parametro.Add("CodVoce", codvoce);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataSet oDs = Database.GetDatasetStoredProc("MSP_SelFUTRiga", spcoll);

                if (oDs != null && oDs.Tables[0].Rows.Count > 0)
                {
                    sRet = oDs.Tables[0].Rows[0]["CodVoce"].ToString();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "GeneraCodAgenda", "ucFoglioUnico");
            }

            return sRet;

        }

        private void VerificaSicurezza()
        {

            try
            {
                if (CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                {
                    this.ubProsegui.Enabled = false;

                }
                else
                {
                    this.ubProsegui.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Prescr_Inserisci);

                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "VerificaSicurezza", this.Name);
            }
        }

        private void CheckFuoriOrario()
        {

            try
            {



                this.SetFuoriOrario();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CheckFuoriOrario", "ucFoglioUnico");
            }

        }

        private void SetFuoriOrario()
        {

            try
            {

                if (this.chkFuoriOrario.Checked == true)
                {
                    this.UltraCalendarInfo.ResetLogicalDayOffset();
                    this.UltraCalendarInfo.ResetLogicalDayDuration();
                }
                else
                {

                    DataSet oDs = LoadFuoriOrario();
                    if (oDs != null && oDs.Tables[0].Rows.Count > 0 && !oDs.Tables[0].Rows[0].IsNull("OraApertura") && !oDs.Tables[0].Rows[0].IsNull("OraChiusura"))
                    {

                        TimeSpan tsOraApertura = TimeSpan.Parse(oDs.Tables[0].Rows[0]["OraApertura"].ToString());
                        TimeSpan tsOraChiusura = TimeSpan.Parse(oDs.Tables[0].Rows[0]["OraChiusura"].ToString());

                        TimeSpan duration = tsOraChiusura.Subtract(tsOraApertura);
                        if (duration.Ticks < 0)
                        {
                            tsOraChiusura = tsOraChiusura.Add(new TimeSpan(24, 0, 0));
                            duration = tsOraChiusura.Subtract(tsOraApertura);
                        }
                        this.UltraCalendarInfo.LogicalDayOffset = tsOraApertura;
                        this.UltraCalendarInfo.LogicalDayDuration = duration;

                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "SetFuoriOrario", "ucFoglioUnico");
            }

        }

        private DataSet LoadFuoriOrario()
        {

            DataSet oDs = null;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodUA", CoreStatics.CoreApplication.Trasferimento.CodUA);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                oDs = Database.GetDatasetStoredProc("MSP_SelUA", spcoll);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "LoadFuoriOrario", "ucFoglioUnico");
            }

            return oDs;

        }

        private void SetTipoVisualizzazione()
        {

            try
            {

                bool bControlloCentraleMassimizzato = true;

                Form frm = this.FindForm();
                if (frm != null && frm is Interfacce.IViewFormMain)
                {
                    bControlloCentraleMassimizzato = (frm as Interfacce.IViewFormMain).ControlloCentraleMassimizzato;
                }

                double dbHeigthN = double.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.PercentualeAltezzaRighaNormaleFUT));
                double dbHeigthC = double.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.PercentualeAltezzaRighaCompattaFUT));
                int nOwner = 0;

                if (this.chkTipoVisualizzazione.Checked == false)
                {
                    this.lblTipoVisualizzazione.Text = "COMPATTA";
                    nOwner = Convert.ToInt16(this.UltraTimelineView.Height / (this.UltraTimelineView.Height * dbHeigthN / 100));
                    this.UltraTimelineView.OwnerHeaderImageSize = new Size(48, 48);
                }
                else
                {
                    this.lblTipoVisualizzazione.Text = "COMPATTA";
                    if (bControlloCentraleMassimizzato == true)
                    {
                        nOwner = Convert.ToInt16(UltraTimelineView.Height / (this.UltraTimelineView.Height * dbHeigthC / 100.0));
                    }
                    else
                    {
                        nOwner = Convert.ToInt16(UltraTimelineView.Height / (this.UltraTimelineView.Height * dbHeigthC * 1.5 / 100.0));
                    }
                    this.UltraTimelineView.OwnerHeaderImageSize = new Size(16, 16);
                }

                this.UltraTimelineView.MaximumOwnersInView = nOwner;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "SetTipoVisualizzazione", "ucFoglioUnico");
            }

        }

        #endregion

        #region     Erogazione Task Inf

        private bool ErogazioneRapida(string id)
        {

            try
            {

                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(id, EnumAzioni.VAL, CoreStatics.CoreApplication.Ambiente);

                bool result = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.ErogaRapida(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente, true);

                return result;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ErogazioneRapida", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
            }

            return false;
        }

        private bool Eroga(string id)
        {
            try
            {

                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(id, EnumAzioni.VAL, CoreStatics.CoreApplication.Ambiente);

                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                bool result = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Eroga(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente, true);

                return result;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Eroga Task", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
            }

            return false;
        }

        #endregion  Erogazione Task Inf

        #region UltraTree

        private void InizializzaUltraTreeView()
        {

            CreationFilterManager oCfm = new CreationFilterManager();
            CreationFilterUltraTreeButton oCfmT = new CreationFilterUltraTreeButton(CoreStatics.CoreApplication.Cartella.CartellaChiusa);
            oCfmT.AddClick += new MyClickEventHandler(OnCustomButtonTreeAddClick);
            oCfm.Add(oCfmT);
            this.ucEasyTreeView.CreationFilter = oCfm;

            this.ucEasyTreeView.Override.Multiline = Infragistics.Win.DefaultableBoolean.True;
            this.ucEasyTreeView.PerformLayout();

            this.ucEasyTreeViewVie.Override.Multiline = Infragistics.Win.DefaultableBoolean.True;
            this.ucEasyTreeViewVie.PerformLayout();

        }

        private async void CaricaUltraTreeView()
        {

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                op.Parametro.Add("IDTrasferimento", CoreStatics.CoreApplication.Trasferimento.ID);
                op.Parametro.Add("Modalita", "0");

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                FwDataParametersList plist = new FwDataParametersList();
                plist.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);

                DataTable tableSezioni = new DataTable();
                DataTable tableVieSomm = new DataTable();

                using (FwDataConnection fdc = new FwDataConnection(Database.ConnectionString))
                {
                    using (FwDataMultiTable multiTable = await fdc.QueryMultiAsync("MSP_SelFUT", plist, CommandType.StoredProcedure))
                    {
                        tableSezioni.Load(multiTable.DataReader);
                        tableVieSomm.Load(multiTable.DataReader);
                    }
                }

                this.ucEasyTreeView.Nodes.Clear();

                this.ucEasyTreeView.EventManager.SetEnabled(TreeEventIds.AfterCheck, false);
                foreach (DataRow oDr in tableSezioni.Rows)
                {
                    UltraTreeNode oNode = new UltraTreeNode(oDr["Codice"].ToString(), oDr["Descrizione"].ToString());
                    oNode.Override.NodeStyle = NodeStyle.CheckBox;
                    if (CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.FoglioUnico.Sezioni.Length == 0)
                    {
                        oNode.CheckedState = CheckState.Checked;
                    }
                    else
                    {
                        bool has = (Array.IndexOf(CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.FoglioUnico.Sezioni, oDr["Codice"].ToString()) >= 0);
                        oNode.CheckedState = (has == true ? CheckState.Checked : CheckState.Unchecked);
                    }

                    if (oDr["Colore"] != null)
                    {
                        oNode.LeftImages.Add(CoreStatics.CreateSolidBitmap(CoreStatics.GetColorFromString(oDr["Colore"].ToString())));
                    }
                    oNode.Tag = (oDr["PermessoInserisci"].ToString() == "1" ? oDr["CodEntita"].ToString() : "");

                    this.ucEasyTreeView.Nodes.Add(oNode);

                }
                this.ucEasyTreeView.EventManager.SetEnabled(TreeEventIds.AfterCheck, true);


                this.ucEasyTreeViewVie.Nodes.Clear();

                UltraTreeNode oNodeRoot = new UltraTreeNode(CoreStatics.GC_TUTTI, "Vie di Somministrazione");
                oNodeRoot.Override.NodeStyle = Infragistics.Win.UltraWinTree.NodeStyle.CheckBox;
                oNodeRoot.CheckedState = CheckState.Checked;
                foreach (DataRow oDr in tableVieSomm.Rows)
                {
                    UltraTreeNode oNode = new UltraTreeNode(oDr["Codice"].ToString(), oDr["Descrizione"].ToString());
                    oNode.Override.NodeStyle = NodeStyle.CheckBox;
                    oNode.CheckedState = CheckState.Checked;

                    if (oDr["Icona"] != null)
                    {
                        oNode.LeftImages.Add(CoreStatics.ByteToImage((byte[])oDr["Icona"]));
                    }
                    oNodeRoot.Nodes.Add(oNode);

                }
                this.ucEasyTreeViewVie.Nodes.Add(oNodeRoot);
                this.ucEasyTreeViewVie.ExpandAll();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaUltraTreeView", "ucFoglioUnico");
            }

        }

        #endregion

        #region Schedule

        private void InizializzaUltraTimelineView()
        {

            try
            {

                this.UltraTimelineView.OwnerHeaderAppearance.FontData.SizeInPoints = 10;
                this.UltraTimelineView.OwnerHeaderTextOrientation = new TextOrientationInfo(0, TextFlowDirection.Horizontal);

                this.UltraTimelineView.OwnerHeaderImageSize = new Size(16, 16);
                this.UltraCalendarInfo.DataBindingsForOwners.BindingContextControl = this;



                this.UltraTimelineView.DrawFilter = new DrawFilterManager() { { new DrawFilterUltraTimeLineViewSplitOwner() },
                                                                                { new DrawFilterUltraTimeLineViewCurrentTime() },
                                                                                { new DrawFilterUltraTimeLineViewMoreApp() },
                                                                                { new DrawFilterUltraTimeLineViewAgendaBlank()}};

                CreationFilterManager oCfm = new CreationFilterManager();
                CreationFilterUltraTimeLineViewButton oCfmB = new CreationFilterUltraTimeLineViewButton(CoreStatics.CoreApplication.Cartella.CartellaChiusa);
                oCfmB.MenuClick += new MyClickEventHandler(OnCustomButtonMenuClick);
                oCfm.Add(oCfmB);
                this.UltraTimelineView.CreationFilter = oCfm;

            }
            catch (Exception)
            {

            }

        }

        private void LoadUltraCalendarInfo(string ID)
        {

            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

            try
            {

                string sKey = ID;

                if (this.UltraTimelineView.ActiveOwner != null && sKey == string.Empty)
                {
                    sKey = this.UltraTimelineView.ActiveOwner.Key;
                }
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                op.Parametro.Add("IDTrasferimento", CoreStatics.CoreApplication.Trasferimento.ID);
                op.Parametro.Add("DataOraInizio", Database.dataOra105PerParametri((DateTime)this.udteFiltroDA.Value));
                op.Parametro.Add("DataOraFine", Database.dataOra105PerParametri((DateTime)this.udteFiltroA.Value));
                op.Parametro.Add("Modalita", "1");
                op.Parametro.Add("Modalita1SoloRange", this.chkSoloPresenti.Checked == false ? "0" : "1");

                Dictionary<string, string> listasezioni = new Dictionary<string, string>();
                foreach (UltraTreeNode oNode in this.ucEasyTreeView.Nodes)
                {
                    if (oNode.CheckedState == CheckState.Checked)
                    {
                        listasezioni.Add(oNode.Key, oNode.Text);
                    }
                }
                string[] codsezioni = listasezioni.Keys.ToArray();
                op.ParametroRipetibile.Add("CodSezione", codsezioni);
                CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.FoglioUnico.Sezioni = listasezioni.Keys.ToArray();

                if (this.ucEasyTreeViewVie.Nodes.Exists(CoreStatics.GC_TUTTI) == true)
                {
                    Dictionary<string, string> listavie = new Dictionary<string, string>();
                    foreach (UltraTreeNode oNode in this.ucEasyTreeViewVie.Nodes[CoreStatics.GC_TUTTI].Nodes)
                    {
                        if (oNode.Override.NodeStyle == NodeStyle.CheckBox && oNode.CheckedState == CheckState.Checked)
                        {
                            listavie.Add(oNode.Key, oNode.Text);
                        }
                    }
                    string[] codvie = listavie.Keys.ToArray();
                    op.ParametroRipetibile.Add("ViaSomministrazione", codvie);
                }


                string xmlParam = XmlProcs.XmlSerializeToString(op);
                FwDataParametersList plist = new FwDataParametersList();
                plist.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);

                DataTable tableCalOwners = new DataTable();

                using (FwDataConnection fdc = new FwDataConnection(Database.ConnectionString))
                {
                    using (FwDataMultiTable multiTable = fdc.QueryMulti("MSP_SelFUT", plist, CommandType.StoredProcedure))
                    {
                        tableCalOwners.Load(multiTable.DataReader);
                    }
                }

                this.UltraCalendarInfo.Owners.Clear();

                if ((tableCalOwners != null) && (tableCalOwners.Rows.Count > 0))
                {
                    this.UltraCalendarInfo.DataBindingsForOwners.SetDataBinding(tableCalOwners, "");
                    this.UltraCalendarInfo.DataBindingsForOwners.BindingContextControl = this;

                    this.UltraCalendarInfo.DataBindingsForOwners.KeyMember = "CodAgenda";
                    this.UltraCalendarInfo.DataBindingsForOwners.NameMember = "DescrVoceCorta";

                    this.UltraCalendarInfo.DataBindingsForOwners.RefreshData();
                }

                op.Parametro.Remove("Modalita");
                op.Parametro.Add("Modalita", "2");

                xmlParam = XmlProcs.XmlSerializeToString(op);
                plist = new FwDataParametersList();
                plist.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);

                DataTable tableCalAppt = new DataTable();
                DataTable tableFuoriOrari = new DataTable();

                using (FwDataConnection fdc = new FwDataConnection(Database.ConnectionString))
                {
                    using (FwDataMultiTable multiTable = fdc.QueryMulti("MSP_SelFUT", plist, CommandType.StoredProcedure))
                    {
                        tableCalAppt.Load(multiTable.DataReader);
                        tableFuoriOrari.Load(multiTable.DataReader);
                    }
                }

                this.UltraCalendarInfo.Appointments.Clear();

                if ((tableCalAppt != null) && (tableCalAppt.Rows.Count > 0))
                {
                    this.UltraCalendarInfo.DataBindingsForAppointments.SetDataBinding(tableCalAppt, "");
                    this.UltraCalendarInfo.DataBindingsForAppointments.BindingContextControl = this;
                }

                this.UltraCalendarInfo.DataBindingsForAppointments.SubjectMember = "Valore";
                this.UltraCalendarInfo.DataBindingsForAppointments.StartDateTimeMember = "DataOraInizio";
                this.UltraCalendarInfo.DataBindingsForAppointments.EndDateTimeMember = "DataOraFine";
                this.UltraCalendarInfo.DataBindingsForAppointments.OwnerKeyMember = "CodAgenda";

                this.UltraCalendarInfo.DataBindingsForAppointments.RefreshData();

                this.UltraCalendarInfo.MinDate = ((DateTime)this.udteFiltroDA.Value).Date;
                this.UltraCalendarInfo.MaxDate = ((DateTime)this.udteFiltroA.Value).Date;

                this.UltraTimelineView.EnsureDateTimeVisible((DateTime)this.udteFiltroDA.Value);

                if (sKey != string.Empty && this.UltraCalendarInfo.Owners.Exists(sKey) == true)
                {
                    this.UltraTimelineView.ActiveOwner = this.UltraCalendarInfo.Owners[sKey];
                    bool isOwnerVisible = this.UltraTimelineView.CalendarInfo.VisibleOwners.Exists(sKey);

                    if (isOwnerVisible)
                        this.UltraTimelineView.EnsureOwnerVisible(this.UltraTimelineView.ActiveOwner, false, true);


                }

                string sFuoriorario = string.Empty;
                if (tableFuoriOrari != null && tableFuoriOrari.Rows.Count == 1 &&
                    !tableFuoriOrari.Rows[0].IsNull("FuoriOrario") && (bool)tableFuoriOrari.Rows[0]["FuoriOrario"] == true && this.chkFuoriOrario.Checked == false)
                {
                    this.lblFuoriOrario.Appearance.BackColor = Color.Red;
                }
                else
                {
                    this.lblFuoriOrario.Appearance.ResetBackColor();
                }


                this.UltraTimelineView.AppointmentDisplaySettings.BarColorIndicatorHeight = 1;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "LoadUltraCalendarInfo", "ucFoglioUnico");
            }

            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);

        }

        private void CalcolaDimensioneColonne()
        {


            try
            {

                TimeSpan oTs = ((DateTime)this.udteFiltroA.Value).Subtract((DateTime)this.udteFiltroDA.Value);



                if (this.UltraCalendarInfo.LogicalDayDuration.TotalHours != 24)
                {

                    DateTime oDtInizio = (DateTime)this.udteFiltroDA.Value;
                    DateTime oDtFine = (DateTime)this.udteFiltroA.Value;
                    TimeSpan oTsTotale = new TimeSpan();

                    while (oDtInizio < oDtFine)
                    {

                        oDtInizio = oDtInizio.AddMinutes(1);

                        DateTime oDtTempI = new DateTime(oDtInizio.Year, oDtInizio.Month, oDtInizio.Day,
                                                            this.UltraCalendarInfo.LogicalDayOffset.Hours, this.UltraCalendarInfo.LogicalDayOffset.Minutes, 0);

                        DateTime oDtTempF = new DateTime(oDtInizio.Year, oDtInizio.Month, oDtInizio.Day,
                                                            this.UltraCalendarInfo.LogicalDayOffset.Hours, this.UltraCalendarInfo.LogicalDayOffset.Minutes, 0);
                        oDtTempF = oDtTempF.Add(this.UltraCalendarInfo.LogicalDayDuration);

                        if (oDtInizio > oDtTempI && oDtInizio < oDtTempF)
                        {
                            oTsTotale = oTsTotale.Add(new TimeSpan(0, 1, 0));
                        }

                    }

                    oTs = oTsTotale;






                }

                if (this.uceGridStep.SelectedItem.DataValue.ToString() == "AUTO")
                {

                    int nIntervallo = (int)oTs.TotalMinutes / 39;

                    if (nIntervallo < int.Parse(this.uceGridStep.Items[1].DataValue.ToString()))
                    {
                        this.UltraTimelineView.PrimaryInterval.Interval = int.Parse(this.uceGridStep.Items[1].DataValue.ToString());
                    }
                    else if (nIntervallo < int.Parse(this.uceGridStep.Items[2].DataValue.ToString()))
                    {
                        this.UltraTimelineView.PrimaryInterval.Interval = int.Parse(this.uceGridStep.Items[2].DataValue.ToString());
                    }
                    else if (nIntervallo < int.Parse(this.uceGridStep.Items[3].DataValue.ToString()))
                    {
                        this.UltraTimelineView.PrimaryInterval.Interval = int.Parse(this.uceGridStep.Items[3].DataValue.ToString());
                    }
                    else if (nIntervallo < int.Parse(this.uceGridStep.Items[4].DataValue.ToString()))
                    {
                        this.UltraTimelineView.PrimaryInterval.Interval = int.Parse(this.uceGridStep.Items[4].DataValue.ToString());
                    }
                    else if (nIntervallo < int.Parse(this.uceGridStep.Items[5].DataValue.ToString()))
                    {
                        this.UltraTimelineView.PrimaryInterval.Interval = int.Parse(this.uceGridStep.Items[5].DataValue.ToString());
                    }
                    else if (nIntervallo < int.Parse(this.uceGridStep.Items[6].DataValue.ToString()))
                    {
                        this.UltraTimelineView.PrimaryInterval.Interval = int.Parse(this.uceGridStep.Items[6].DataValue.ToString());
                    }
                    else
                    {
                        this.UltraTimelineView.PrimaryInterval.Interval = int.Parse(this.uceGridStep.Items[6].DataValue.ToString());
                    }
                }
                else
                {

                    this.UltraTimelineView.PrimaryInterval.Interval = int.Parse(this.uceGridStep.SelectedItem.DataValue.ToString());
                }
                this.UltraTimelineView.AdditionalIntervals[1].Visible = (this.UltraTimelineView.PrimaryInterval.Interval < 60 ? true : false);

                if (this.UltraTimelineView.AdditionalIntervals[1].Visible == true)
                {
                    this.UltraTimelineView.PrimaryInterval.HeaderTextFormat = "mm";
                }
                else
                {
                    this.UltraTimelineView.PrimaryInterval.HeaderTextFormat = "";
                }

                int nSlot = (int)System.Math.Ceiling(oTs.TotalMinutes / this.UltraTimelineView.PrimaryInterval.Interval);

                DrawFilterManager oDfm = (DrawFilterManager)this.UltraTimelineView.DrawFilter;
                Rectangle oRect = ((DrawFilterUltraTimeLineViewSplitOwner)oDfm[0]).OwnerRectInsideBorders;

                this.UltraTimelineView.ColumnWidth = ((this.UltraTimelineView.Width - oRect.Width - SystemInformation.VerticalScrollBarWidth) / nSlot) -
        (SystemInformation.VerticalScrollBarWidth / nSlot);

            }
            catch
            {
            }

        }

        private void NuovoParametro(string codentita, string codsezione, string codvoce, DateTime dataevento)
        {

            Parametri op = null;
            SqlParameterExt[] spcoll = null;
            string xmlParam = string.Empty;
            DataTable oDt = null;
            DataRow[] foundRows = null;

            try
            {

                switch ((EnumEntita)Enum.Parse(typeof(EnumEntita), codentita))
                {

                    case EnumEntita.NTG:
                        CoreStatics.CoreApplication.MovNotaSelezionata = new MovNota(CoreStatics.CoreApplication.Paziente.ID,
                                                                                       CoreStatics.CoreApplication.Episodio.ID,
                                                                                       CoreStatics.CoreApplication.Trasferimento.ID, EnumEntita.NTG.ToString(), "");
                        CoreStatics.CoreApplication.MovNotaSelezionata.Azione = EnumAzioni.INS;
                        CoreStatics.CoreApplication.MovNotaSelezionata.DataInizio = dataevento;
                        CoreStatics.CoreApplication.MovNotaSelezionata.DataFine = dataevento;
                        _ucEasyPopUpNota = new ucEasyPopUpNota();
                        _ucEasyPopUpNota.DataOra = CoreStatics.CoreApplication.MovNotaSelezionata.DataInizio;
                        _ucEasyPopUpNota.Nota = CoreStatics.CoreApplication.MovNotaSelezionata.Oggetto;
                        _ucEasyPopUpNota.Cancella = false;
                        this.UltraPopupControlContainerNote.Show();
                        break;

                    case EnumEntita.PVT:
                        CoreStatics.CoreApplication.MovParametroVitaleSelezionato = new MovParametroVitale(CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                            CoreStatics.CoreApplication.Paziente.ID,
                                                            CoreStatics.CoreApplication.Episodio.ID,
                                                            CoreStatics.CoreApplication.Trasferimento.ID, CoreStatics.CoreApplication.Ambiente);
                        CoreStatics.CoreApplication.MovParametroVitaleSelezionato.Azione = EnumAzioni.INS;
                        CoreStatics.CoreApplication.MovParametroVitaleSelezionato.DataEvento = dataevento;

                        if (codvoce == string.Empty)
                        {
                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneTipoParametriVitali) == DialogResult.OK)
                            {
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingParametriVitali) == DialogResult.OK)
                                {
                                    this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovParametroVitaleSelezionato.IDMovParametroVitale, codsezione));
                                }
                            }
                        }
                        else
                        {
                            op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                            op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovParametroVitaleSelezionato.CodUA);
                            op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                            op.Parametro.Add("CodAzione", CoreStatics.CoreApplication.MovParametroVitaleSelezionato.Azione.ToString());
                            op.Parametro.Add("DatiEstesi", "0");
                            spcoll = new SqlParameterExt[1];
                            xmlParam = XmlProcs.XmlSerializeToString(op);
                            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                            oDt = Database.GetDataTableStoredProc("MSP_SelTipoParametroVitale", spcoll);

                            foundRows = oDt.Select("Codice = '" + codvoce + "'");
                            if (foundRows.Length == 1)
                            {
                                CoreStatics.CoreApplication.MovParametroVitaleSelezionato.CodTipoParametroVitale = foundRows[0]["Codice"].ToString();
                                CoreStatics.CoreApplication.MovParametroVitaleSelezionato.DescrTipoParametroVitale = foundRows[0]["Descrizione"].ToString();
                                CoreStatics.CoreApplication.MovParametroVitaleSelezionato.CodScheda = foundRows[0]["CodScheda"].ToString();
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingParametriVitali) == DialogResult.OK)
                                {
                                    this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovParametroVitaleSelezionato.IDMovParametroVitale, codsezione));
                                }
                            }
                            else
                            {
                                easyStatics.EasyMessageBox("Voce NON abilitata!" + Environment.NewLine + "Contattare l'amministratore del sistema per la soluzione del problema ...", "Foglio Unico", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        break;

                    case EnumEntita.PRF:
                        if (codsezione == "PFM")
                        {
                            this.AzioneMenuPrescrizione(codvoce, dataevento);
                        }
                        else if (codsezione == "PFA")
                        {
                            if (codvoce == string.Empty)
                            {
                                CoreStatics.CoreApplication.MovPrescrizioneSelezionata = new MovPrescrizione(CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                                                                               CoreStatics.CoreApplication.Paziente.ID,
                                                                                                               CoreStatics.CoreApplication.Episodio.ID,
                                                                                                               CoreStatics.CoreApplication.Trasferimento.ID,
                                                                                                               CoreStatics.CoreApplication.Ambiente);
                                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Azione = EnumAzioni.INS;
                                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DataEvento = DateTime.Now;
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneTipoPrescrizione) == DialogResult.OK)
                                {
                                    if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata != null)
                                    {
                                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingPrescrizione, false);
                                        this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.IDPrescrizioneTempi, codsezione));
                                    }
                                    else
                                    {
                                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingPrescrizioniProtocollo, false, CoreStatics.CoreApplication.ListaIDMovPrescrizioniCreate);
                                        CoreStatics.CoreApplication.ListaIDMovPrescrizioniCreate = null;
                                        this.Aggiorna();
                                    }
                                }
                            }
                            else
                            {
                                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = new MovPrescrizioneTempi(codvoce, "", CoreStatics.CoreApplication.Ambiente);
                                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataEvento = DateTime.Now;
                                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio = dataevento;
                                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraFine = dataevento;
                                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.AlBisogno = false;
                                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CreaTaskInfermieristici(EnumCodSistema.PRF, Database.GetConfigTable(EnumConfigTable.TipoSchedaTaskDaPrescrizione), EnumTipoRegistrazione.M);
                                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.IDUltimoTaskInfermieristicoGenerato,
                                                                                            EnumAzioni.VAL,
                                                                                            CoreStatics.CoreApplication.Ambiente);
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.ErogazioneTaskInfermieristici) == DialogResult.Cancel)
                                {
                                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Cancella();
                                }
                                this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.IDPrescrizioneTempi, codsezione));
                            }
                        }
                        break;

                    case EnumEntita.WKI:
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                                    CoreStatics.CoreApplication.Paziente.ID,
                                                                    CoreStatics.CoreApplication.Episodio.ID,
                                                                    CoreStatics.CoreApplication.Trasferimento.ID,
                                                                    EnumCodSistema.WKI, EnumTipoRegistrazione.M,
                                                                    CoreStatics.CoreApplication.Ambiente);
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Azione = EnumAzioni.INS;
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataEvento = dataevento;
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata = dataevento;

                        if (codvoce == string.Empty)
                        {
                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneTipoTaskInfermieristici) == DialogResult.OK)
                            {
                                DialogResult result = DialogResult.Cancel;
                                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato != null)
                                {
                                    if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.ErogazioneDiretta == false)
                                    {
                                        result = CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingTaskInfermieristici, false);
                                    }
                                    else
                                    {
                                        List<string> lstresult = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.GeneraErogazioneDiretta();
                                        if (lstresult.Count > 0)
                                        {
                                            var vmessaggio = String.Join(Environment.NewLine, lstresult.ToArray());
                                            easyStatics.EasyMessageBox(vmessaggio, "Generazione Erogazione Diretta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        }
                                        result = DialogResult.OK;
                                    }
                                }
                                else
                                {
                                    result = CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingTaskInfermieristiciProtocollo, false);
                                }

                                if (result == DialogResult.OK)
                                {
                                    if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato != null)
                                    {
                                        this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.IDMovTaskInfermieristico, codsezione));
                                    }

                                    if (CoreStatics.CoreApplication.MovProtocolloAttivitaSelezionato != null)
                                    {
                                        this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovProtocolloAttivitaSelezionato.UltimoIDMovTaskInfermieristicoGenerato, codsezione));
                                    }

                                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = null;

                                }
                            }
                        }
                        else
                        {
                            op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                            op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodUA);
                            op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                            op.Parametro.Add("CodAzione", CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Azione.ToString());
                            op.Parametro.Add("DatiEstesi", "0");
                            spcoll = new SqlParameterExt[1];
                            xmlParam = XmlProcs.XmlSerializeToString(op);
                            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                            oDt = Database.GetDataTableStoredProc("MSP_SelTipoTaskInfermieristico", spcoll);

                            foundRows = oDt.Select("Codice = '" + codvoce + "'");
                            if (foundRows.Length == 1)
                            {
                                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoTaskInfermieristico = foundRows[0]["Codice"].ToString();
                                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DescrTipoTaskInfermieristico = foundRows[0]["Descrizione"].ToString();
                                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodScheda = foundRows[0]["CodScheda"].ToString();
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingTaskInfermieristici) == DialogResult.OK)
                                {
                                    this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.IDMovTaskInfermieristico, codsezione));
                                }
                            }
                            else
                            {
                                easyStatics.EasyMessageBox("Voce NON abilitata!" + Environment.NewLine + "Contattare l'amministratore del sistema per la soluzione del problema ...", "Foglio Unico", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        break;

                    case EnumEntita.APP:
                        CoreStatics.CoreApplication.MovAppuntamentiGenerati = new List<MovAppuntamento>();
                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento(CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                                                                            CoreStatics.CoreApplication.Paziente.ID,
                                                                                                            CoreStatics.CoreApplication.Episodio.ID,
                                                                                                            CoreStatics.CoreApplication.Trasferimento.ID);
                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Azione = EnumAzioni.INS;
                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataInizio = dataevento;
                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.DataFine = dataevento;

                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneTipoAppuntamento) == DialogResult.OK)
                        {
                            CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CaricaAgende();
                            if (codvoce == string.Empty)
                            {
                                while (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneAgendeAppuntamento) == DialogResult.OK)
                                {
                                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneAppuntamento) == DialogResult.OK)
                                    {

                                        CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                                        if (CoreStatics.CoreApplication.MovAppuntamentiGenerati != null && CoreStatics.CoreApplication.MovAppuntamentiGenerati.Count > 1)
                                        {
                                            for (int ma = 0; ma < CoreStatics.CoreApplication.MovAppuntamentiGenerati.Count; ma++)
                                            {
                                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato = CoreStatics.CoreApplication.MovAppuntamentiGenerati[ma];

                                                PluginClientStatics.PluginClient(EnumPluginClient.APP_NUOVO_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                                this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDAppuntamento, codsezione));
                                            }

                                        }
                                        else
                                        {
                                            PluginClientStatics.PluginClient(EnumPluginClient.APP_NUOVO_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                            this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDAppuntamento, codsezione));
                                        }

                                        CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);

                                        break;
                                    }
                                }
                            }
                            else
                            {
                                MovAppuntamentoAgende oItem = null;
                                try
                                {
                                    oItem = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Elementi.Single(MovAppuntamentoAgende => MovAppuntamentoAgende.CodAgenda == codvoce);
                                }
                                catch (Exception)
                                {
                                    oItem = null;
                                }
                                if (oItem != null)
                                {

                                    oItem.Selezionata = true;
                                    oItem.Modificata = true;

                                    while (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneAgendeAppuntamento) == DialogResult.OK)
                                    {
                                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneAppuntamento) == DialogResult.OK)
                                        {

                                            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                                            if (CoreStatics.CoreApplication.MovAppuntamentiGenerati != null && CoreStatics.CoreApplication.MovAppuntamentiGenerati.Count > 1)
                                            {
                                                for (int ma = 0; ma < CoreStatics.CoreApplication.MovAppuntamentiGenerati.Count; ma++)
                                                {
                                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato = CoreStatics.CoreApplication.MovAppuntamentiGenerati[ma];

                                                    PluginClientStatics.PluginClient(EnumPluginClient.APP_NUOVO_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                                    this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDAppuntamento, codsezione));
                                                }

                                            }
                                            else
                                            {
                                                PluginClientStatics.PluginClient(EnumPluginClient.APP_NUOVO_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                                this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.IDAppuntamento, codsezione));
                                            }


                                            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);

                                            break;
                                        }
                                    }

                                }
                                else
                                {
                                    easyStatics.EasyMessageBox("Voce NON abilitata!" + Environment.NewLine + "Contattare l'amministratore del sistema per la soluzione del problema ...", "Foglio Unico", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                }
                            }
                        }
                        break;

                    case EnumEntita.DCL:
                        CoreStatics.CoreApplication.MovDiarioClinicoSelezionato = new MovDiarioClinico(CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                        CoreStatics.CoreApplication.Paziente.ID,
                                                        CoreStatics.CoreApplication.Episodio.ID,
                                                        CoreStatics.CoreApplication.Trasferimento.ID,
                                                        CoreStatics.CoreApplication.Ambiente);
                        CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.Azione = EnumAzioni.INS;
                        CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.DataEvento = dataevento;
                        CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodStatoDiario = "VA";

                        if (codvoce == string.Empty)
                        {
                            if (CoreStatics.CheckSelezionaTipoVoceDiario() == true)
                            {
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingVoceDiDiario) == DialogResult.OK)
                                {
                                    this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.IDMovDiario, codsezione));
                                }
                            }
                            else
                            {
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezionaTipoVoceDiario) == DialogResult.OK)
                                {
                                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingVoceDiDiario) == DialogResult.OK)
                                    {
                                        this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.IDMovDiario, codsezione));
                                    }
                                }
                            }
                        }
                        else
                        {
                            op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                            op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodUA);
                            op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                            op.Parametro.Add("CodAzione", CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.Azione.ToString());
                            spcoll = new SqlParameterExt[1];
                            xmlParam = XmlProcs.XmlSerializeToString(op);
                            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                            oDt = Database.GetDataTableStoredProc("MSP_SelTipoDiarioClinico", spcoll);

                            foundRows = oDt.Select("CodVoce = '" + codvoce + "'");
                            if (foundRows.Length == 1)
                            {
                                CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodTipoVoceDiario = foundRows[0]["CodVoce"].ToString();
                                CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.DescrTipoVoceDiario = foundRows[0]["Descrizione"].ToString();
                                CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodScheda = foundRows[0]["CodScheda"].ToString();
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingVoceDiDiario) == DialogResult.OK)
                                {
                                    this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.IDMovDiario, codsezione));
                                }
                            }
                            else
                            {
                                easyStatics.EasyMessageBox("Voce NON abilitata!" + Environment.NewLine + "Contattare l'amministratore del sistema per la soluzione del problema ...", "Foglio Unico", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        break;

                    case EnumEntita.EVC:

                        if (!_locknuovoordine)
                        {
                            _locknuovoordine = true;

                            string sCodAziOrdine = string.Empty;
                            if (CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento != null && CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento != string.Empty)
                            { sCodAziOrdine = CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento; }
                            else
                            { sCodAziOrdine = CoreStatics.CoreApplication.Episodio.CodAzienda; }

                            CoreStatics.CoreApplication.MovOrdineSelezionato = new MovOrdine(CoreStatics.CoreApplication.Ambiente,
                                                                    CoreStatics.CoreApplication.Episodio.ID,
                                                                    CoreStatics.CoreApplication.Episodio.CodTipoEpisodio,
                                                                    CoreStatics.CoreApplication.Episodio.NumeroEpisodio,
                                                                    CoreStatics.CoreApplication.Episodio.NumeroListaAttesa,
                                                                    sCodAziOrdine,
                                                                    CoreStatics.CoreApplication.Trasferimento.ID,
                                                                    CoreStatics.CoreApplication.Trasferimento.CodUO,
                                                                    CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                                    CoreStatics.CoreApplication.Paziente);

                            if (codvoce == string.Empty)
                            {
                            }
                            else
                            {
                                if (easyStatics.EasyMessageBox("Confermi la creazione di un nuovo ordine?", "Nuovo Ordine", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    if (CoreStatics.CoreApplication.MovOrdineSelezionato.CreaOrdine())
                                    {
                                        CoreStatics.CoreApplication.MovOrdineSelezionato.DataProgrammazione = dataevento;
                                        CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi = false;
                                        while (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingOrdine) == DialogResult.OK)
                                        {

                                            if (CoreStatics.CoreApplication.MovOrdineSelezionato != null && CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi)
                                            {
                                                this.Aggiorna();
                                                break;
                                            }
                                            else
                                            {
                                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingOrdineDatiAggiuntivi) == DialogResult.OK)
                                                {
                                                    this.Aggiorna();
                                                    break;
                                                }
                                            }
                                        }
                                        if (CoreStatics.CoreApplication.MovOrdineSelezionato != null)
                                            CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi = false;
                                    }
                                    else
                                    {
                                        easyStatics.EasyErrorMessageBox("Errore nella creazione ordine.", @"Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        CoreStatics.CoreApplication.MovOrdineSelezionato = null;
                                    }
                                }
                                else
                                    CoreStatics.CoreApplication.MovOrdineSelezionato = null;
                            }


                            _locknuovoordine = false;
                        }

                        break;

                    case EnumEntita.OE:

                        if (!_locknuovoordine)
                        {
                            _locknuovoordine = true;

                            string sCodAziOrdine = string.Empty;
                            if (CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento != null && CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento != string.Empty)
                            { sCodAziOrdine = CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento; }
                            else
                            { sCodAziOrdine = CoreStatics.CoreApplication.Episodio.CodAzienda; }

                            CoreStatics.CoreApplication.MovOrdineSelezionato = new MovOrdine(CoreStatics.CoreApplication.Ambiente,
                                                                    CoreStatics.CoreApplication.Episodio.ID,
                                                                    CoreStatics.CoreApplication.Episodio.CodTipoEpisodio,
                                                                    CoreStatics.CoreApplication.Episodio.NumeroEpisodio,
                                                                    CoreStatics.CoreApplication.Episodio.NumeroListaAttesa,
                                                                    sCodAziOrdine,
                                                                    CoreStatics.CoreApplication.Trasferimento.ID,
                                                                    CoreStatics.CoreApplication.Trasferimento.CodUO,
                                                                    CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                                    CoreStatics.CoreApplication.Paziente);

                            if (easyStatics.EasyMessageBox("Confermi la creazione di un nuovo ordine?", "Nuovo Ordine", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                if (CoreStatics.CoreApplication.MovOrdineSelezionato.CreaOrdine())
                                {
                                    CoreStatics.CoreApplication.MovOrdineSelezionato.DataProgrammazione = dataevento;
                                    CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi = false;
                                    if (codvoce == string.Empty)
                                    {
                                        while (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingOrdine) == DialogResult.OK)
                                        {

                                            if (CoreStatics.CoreApplication.MovOrdineSelezionato != null && CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi)
                                            {
                                                this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovOrdineSelezionato.IDOrdine, codsezione));
                                                break;
                                            }
                                            else
                                            {
                                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingOrdineDatiAggiuntivi) == DialogResult.OK)
                                                {
                                                    this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovOrdineSelezionato.IDOrdine, codsezione));
                                                    break;
                                                }
                                            }



                                        }
                                    }
                                    else
                                    {
                                        while (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingOrdine) == DialogResult.OK)
                                        {
                                            if (CoreStatics.CoreApplication.MovOrdineSelezionato != null && CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi)
                                            {
                                                this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovOrdineSelezionato.IDOrdine, codsezione));
                                                break;
                                            }
                                            else
                                            {
                                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingOrdineDatiAggiuntivi) == DialogResult.OK)
                                                {
                                                    this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovOrdineSelezionato.IDOrdine, codsezione));
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (CoreStatics.CoreApplication.MovOrdineSelezionato != null)
                                        CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi = false;
                                }
                                else
                                {
                                    easyStatics.EasyErrorMessageBox("Errore nella creazione ordine.", @"Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    CoreStatics.CoreApplication.MovOrdineSelezionato = null;
                                }

                            }
                            else
                                CoreStatics.CoreApplication.MovOrdineSelezionato = null;

                            _locknuovoordine = false;
                        }

                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "NuovoParametro", "ucFoglioUnico");
            }

        }

        private void SetWorkingHour(Owner oOwner)
        {

            try
            {

                TimeSpan oTimeSpan = new TimeSpan(this.UltraCalendarInfo.LogicalDayOffset.Hours, this.UltraTimelineView.PrimaryInterval.Interval, 0);

                oOwner.DayOfWeekSettings[DayOfWeekEnum.Monday].WorkDayStartTime = new DateTime(1, 1, 1, oTimeSpan.Hours, oTimeSpan.Minutes, 0);
                oOwner.DayOfWeekSettings[DayOfWeekEnum.Monday].WorkDayEndTime = new DateTime(1, 1, 1, 23, 59, 59);

                oOwner.DayOfWeekSettings[DayOfWeekEnum.Thursday].WorkDayStartTime = new DateTime(1, 1, 1, oTimeSpan.Hours, oTimeSpan.Minutes, 0);
                oOwner.DayOfWeekSettings[DayOfWeekEnum.Thursday].WorkDayEndTime = new DateTime(1, 1, 1, 23, 59, 59);

                oOwner.DayOfWeekSettings[DayOfWeekEnum.Wednesday].WorkDayStartTime = new DateTime(1, 1, 1, oTimeSpan.Hours, oTimeSpan.Minutes, 0);
                oOwner.DayOfWeekSettings[DayOfWeekEnum.Wednesday].WorkDayEndTime = new DateTime(1, 1, 1, 23, 59, 59);

                oOwner.DayOfWeekSettings[DayOfWeekEnum.Tuesday].WorkDayStartTime = new DateTime(1, 1, 1, oTimeSpan.Hours, oTimeSpan.Minutes, 0);
                oOwner.DayOfWeekSettings[DayOfWeekEnum.Tuesday].WorkDayEndTime = new DateTime(1, 1, 1, 23, 59, 59);

                oOwner.DayOfWeekSettings[DayOfWeekEnum.Friday].WorkDayStartTime = new DateTime(1, 1, 1, oTimeSpan.Hours, oTimeSpan.Minutes, 0);
                oOwner.DayOfWeekSettings[DayOfWeekEnum.Friday].WorkDayEndTime = new DateTime(1, 1, 1, 23, 59, 59);

                oOwner.DayOfWeekSettings[DayOfWeekEnum.Saturday].WorkDayStartTime = new DateTime(1, 1, 1, oTimeSpan.Hours, oTimeSpan.Minutes, 0);
                oOwner.DayOfWeekSettings[DayOfWeekEnum.Saturday].WorkDayEndTime = new DateTime(1, 1, 1, 23, 59, 59);
                oOwner.DayOfWeekSettings[DayOfWeekEnum.Saturday].IsWorkDay = DefaultableBoolean.True;

                oOwner.DayOfWeekSettings[DayOfWeekEnum.Sunday].WorkDayStartTime = new DateTime(1, 1, 1, oTimeSpan.Hours, oTimeSpan.Minutes, 0);
                oOwner.DayOfWeekSettings[DayOfWeekEnum.Sunday].WorkDayEndTime = new DateTime(1, 1, 1, 23, 59, 59);
                oOwner.DayOfWeekSettings[DayOfWeekEnum.Sunday].IsWorkDay = DefaultableBoolean.True;

                oOwner.NonWorkingHourTimeSlotAppearance.BackColor = _colorecambiogiorno;
                oOwner.WorkingHourTimeSlotAppearance.BackColor = Color.White;

            }
            catch (Exception)
            {

            }

        }

        private void CalcolaPermessiOE(string NumeroOrdineOE, out bool permessomodifica, out bool permessoinoltra,
                                       out bool permessocancella, out bool permessovisualizza, out bool permessocopia)
        {

            string sql = "SELECT * FROM T_MovOrdini WHERE NumeroOrdineOE = '" + NumeroOrdineOE + "'";
            DataTable odt = Database.GetDatatable(sql);
            EnumStatoOrdine statoordine = EnumStatoOrdine.NN;

            bool _permessomodifica = false;
            bool _permessoinoltra = false;
            bool _permessocancella = false;
            bool _permessovisualizza = false;
            bool _permessocopia = false;

            try
            {

                if (odt != null && odt.Rows.Count == 1)
                {

                    Enum.TryParse(odt.Rows[0]["CodStatoOrdine"].ToString(), true, out statoordine);

                    switch (CommonStatics.TraduciEnumStatoOrdine(statoordine))
                    {
                        case OEStato.Erogato:
                        case OEStato.Errato:
                        case OEStato.InCarico:
                        case OEStato.Inoltrato:
                        case OEStato.NN:
                        case OEStato.Programmato:
                        case OEStato.Accettato:
                            _permessomodifica = false;
                            _permessocancella = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Cancella);
                            _permessocopia = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Inserisci);
                            _permessovisualizza = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Visualizza);
                            _permessoinoltra = false;
                            break;

                        case OEStato.Cancellato:
                        case OEStato.Annullato:
                            _permessomodifica = false;
                            _permessocancella = false;
                            _permessocopia = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Inserisci);
                            _permessovisualizza = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Visualizza);
                            _permessoinoltra = false;
                            break;

                        case OEStato.Inserito:
                            _permessomodifica = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Modifica);
                            _permessocancella = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Cancella);
                            _permessocopia = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Inserisci);
                            _permessovisualizza = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Visualizza);
                            _permessoinoltra = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Ordini_Inoltra);
                            break;

                        default:
                            _permessomodifica = false;
                            _permessocancella = false;
                            _permessocopia = false;
                            _permessovisualizza = false;
                            _permessoinoltra = false;
                            break;
                    }
                }
                else
                {
                    _permessomodifica = false;
                    _permessocancella = false;
                    _permessocopia = false;
                    _permessovisualizza = false;
                    _permessoinoltra = false;
                }

            }
            catch (Exception)
            {
                _permessomodifica = false;
                _permessocancella = false;
                _permessocopia = false;
                _permessovisualizza = false;
                _permessoinoltra = false;
            }
            finally
            {
                if (odt != null)
                {
                    odt.Dispose();
                    odt = null;
                }
            }

            permessomodifica = _permessomodifica;
            permessocancella = _permessocancella;
            permessocopia = _permessocopia;
            permessovisualizza = _permessovisualizza;
            permessoinoltra = _permessoinoltra;

        }

        private Infragistics.Win.DefaultableBoolean ConvertBoolToDefaultableBoolean(bool inputvalue)
        {
            switch (inputvalue)
            {
                case true:
                    return DefaultableBoolean.True;
                case false:
                    return DefaultableBoolean.False;
                default:
                    return DefaultableBoolean.Default;
            }
        }

        #endregion

        #region Gestore

        private void CaricaGestore()
        {

            try
            {
                oGestore.SchedaXML = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.MovScheda.Scheda.StrutturaXML;
                oGestore.SchedaLayoutsXML = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.MovScheda.Scheda.LayoutXML;
                oGestore.Decodifiche = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.MovScheda.Scheda.DizionarioValori();

                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.MovScheda.DatiXML == string.Empty)
                {
                    oGestore.SchedaDati = new DcSchedaDati();
                }
                else
                {
                    try
                    {
                        oGestore.SchedaDatiXML = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.MovScheda.DatiXML;
                    }
                    catch (Exception)
                    {
                        oGestore.SchedaDati = new DcSchedaDati();
                    }
                }
                if (oGestore.SchedaDati.Dati.Count == 0) { oGestore.NuovaScheda(); }



            }
            catch (Exception ex)
            {
                throw new Exception(@"CaricaGestore()" + Environment.NewLine + ex.Message, ex);
            }

        }

        #endregion

        #region Events User Control

        private void ucFoglioUnico_Leave(object sender, EventArgs e)
        {
            CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.FoglioUnico.Range = this.uceRange.SelectedIndex;
            CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.FoglioUnico.Step = this.uceGridStep.SelectedIndex;
            CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.FoglioUnico.MostraNotte = this.chkFuoriOrario.Checked;
            CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.FoglioUnico.SoloAttivi = this.chkSoloPresenti.Checked;
            CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.FoglioUnico.TipoVisualizzazione = this.chkTipoVisualizzazione.Checked;
            CoreStatics.CoreApplication.Sessione.Utente.SalvaConfigUtente();
        }

        #endregion

        #region Events

        private void UltraDockManager_InitializePane(object sender, Infragistics.Win.UltraWinDock.InitializePaneEventArgs e)
        {

            e.Pane.Settings.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            e.Pane.Settings.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FILTRO_32);

            int filtroWidth = 20 * (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            this.UltraDockManager.ControlPanes[0].FlyoutSize = new Size(filtroWidth, this.UltraDockManager.ControlPanes[0].FlyoutSize.Height);
            this.UltraDockManager.ControlPanes[0].Size = new Size(filtroWidth, this.UltraDockManager.ControlPanes[0].Size.Height);
            this.UltraDockManager.DockAreas[0].Size = new Size(filtroWidth, this.UltraDockManager.DockAreas[0].Size.Height);
            this.pnlFiltro.Width = filtroWidth;

        }

        private void ucEasyButton_Click(object sender, EventArgs e)
        {

            try
            {

                CheckState oCs = (((ucEasyButton)sender).Tag.ToString() == @"1" ? CheckState.Checked : CheckState.Unchecked);

                this.ucEasyTreeView.EventManager.SetEnabled(TreeEventIds.AfterCheck, false);
                foreach (UltraTreeNode oNode in this.ucEasyTreeView.Nodes)
                {
                    oNode.CheckedState = oCs;
                }
                this.ucEasyTreeView.EventManager.SetEnabled(TreeEventIds.AfterCheck, true);

            }
            catch (Exception)
            {

            }

        }

        private void ucEasyTreeView_AfterCheck(object sender, NodeEventArgs e)
        {

            try
            {


                if (e.TreeNode.Key == CoreStatics.GC_TUTTI)
                {
                    foreach (Infragistics.Win.UltraWinTree.UltraTreeNode oNode in this.ucEasyTreeViewVie.Nodes[CoreStatics.GC_TUTTI].Nodes)
                    {
                        if (oNode.Override.NodeStyle == Infragistics.Win.UltraWinTree.NodeStyle.CheckBox)
                        {
                            oNode.CheckedState = e.TreeNode.CheckedState;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void udteFiltro_ValueChanged(object sender, EventArgs e)
        {
            this.TimerChange.Enabled = false;
            this.TimerChange.Enabled = true;
        }

        private void uceRange_ValueChanged(object sender, EventArgs e)
        {

            DateTime dtI = DateTime.Now;
            DateTime dtF = DateTime.Now;

            switch (this.uceRange.SelectedItem.DataValue.ToString())
            {

                case "24H":
                    dtI = DateTime.Now.AddHours(-12);
                    dtF = DateTime.Now.AddHours(12);
                    break;

                case "48H":
                    dtI = DateTime.Now.AddHours(-24);
                    dtF = DateTime.Now.AddHours(24);
                    break;

                case "4GG":
                    dtI = DateTime.Now.AddDays(-2);
                    dtF = DateTime.Now.AddDays(2);
                    break;

                case "3GG0524":
                    dtI = new DateTime(dtI.Year, dtI.Month, dtI.Day, 5, 0, 0).AddDays(-1);
                    dtF = dtI.AddHours(67);
                    break;

                case "U24H":
                    dtI = DateTime.Now.AddHours(-24);
                    dtF = DateTime.Now;
                    break;

                case "P24H":
                    dtI = DateTime.Now;
                    dtF = DateTime.Now.AddHours(24);
                    break;

                case "P48H":
                    dtI = DateTime.Now;
                    dtF = DateTime.Now.AddHours(48);
                    break;

                case "P72H":
                    dtI = DateTime.Now;
                    dtF = DateTime.Now.AddHours(72);
                    break;

                case "P7GG":
                    dtI = DateTime.Now;
                    dtF = DateTime.Now.AddDays(7);
                    break;

                case "OGGI":
                    dtI = new DateTime(dtI.Year, dtI.Month, dtI.Day, 4, 0, 0);
                    dtF = dtI.AddHours(20);
                    break;

                case "OGGI1":
                    dtI = new DateTime(dtI.Year, dtI.Month, dtI.Day, 4, 0, 0);
                    dtF = dtI.AddHours(44);
                    break;

                case "OGGI2":
                    dtI = new DateTime(dtI.Year, dtI.Month, dtI.Day, 4, 0, 0);
                    dtF = dtI.AddHours(68);
                    break;

                case "OGGI3":
                    dtI = new DateTime(dtI.Year, dtI.Month, dtI.Day, 4, 0, 0);
                    dtF = dtI.AddHours(92);
                    break;

            }

            this.udteFiltroDA.DateTime = dtI;
            this.udteFiltroA.DateTime = dtF;

            this.CalcolaDimensioneColonne();
            this.UltraTimelineView.Refresh();

        }

        private void uceGridStep_ValueChanged(object sender, EventArgs e)
        {
            this.CalcolaDimensioneColonne();
            foreach (Owner o in this.UltraCalendarInfo.Owners)
            {
                this.SetWorkingHour(o);
            }
        }

        private void chkFuoriOrario_CheckedChanged(object sender, EventArgs e)
        {
            this.SetFuoriOrario();
            this.Aggiorna();
        }

        private void chkSoloPresenti_CheckedChanged(object sender, EventArgs e)
        {
            this.Aggiorna();
        }

        private void chkTipoVisualizzazione_CheckedChanged(object sender, EventArgs e)
        {
            this.SetTipoVisualizzazione();
        }

        private void ubApplicaFiltro_Click(object sender, EventArgs e)
        {
            if (this.UltraDockManager.FlyoutPane != null && !this.UltraDockManager.FlyoutPane.Pinned) this.UltraDockManager.FlyIn();
            this.Aggiorna();
        }

        private void ubIndietro_Click(object sender, EventArgs e)
        {
            this.ubIndietro.Enabled = false;
            this.udteFiltroA.ValueChanged -= this.udteFiltro_ValueChanged;
            this.udteFiltroDA.ValueChanged -= this.udteFiltro_ValueChanged;

            TimeSpan oTs = ((DateTime)this.udteFiltroDA.Value).Subtract((DateTime)this.udteFiltroA.Value);
            this.udteFiltroDA.Value = ((DateTime)this.udteFiltroDA.Value).AddMinutes(oTs.TotalMinutes / 2);
            this.udteFiltroA.Value = ((DateTime)this.udteFiltroA.Value).AddMinutes(oTs.TotalMinutes / 2);

            this.Aggiorna();
            this.udteFiltroA.ValueChanged += new System.EventHandler(this.udteFiltro_ValueChanged);
            this.udteFiltroDA.ValueChanged += new System.EventHandler(this.udteFiltro_ValueChanged);
            this.ubIndietro.Enabled = true;
        }

        private void ubAvanti_Click(object sender, EventArgs e)
        {
            this.ubAvanti.Enabled = false;
            this.udteFiltroA.ValueChanged -= this.udteFiltro_ValueChanged;
            this.udteFiltroDA.ValueChanged -= this.udteFiltro_ValueChanged;

            TimeSpan oTs = ((DateTime)this.udteFiltroA.Value).Subtract((DateTime)this.udteFiltroDA.Value);
            this.udteFiltroA.Value = ((DateTime)this.udteFiltroA.Value).AddMinutes(oTs.TotalMinutes / 2);
            this.udteFiltroDA.Value = ((DateTime)this.udteFiltroDA.Value).AddMinutes(oTs.TotalMinutes / 2);

            this.Aggiorna();
            this.udteFiltroA.ValueChanged += new System.EventHandler(this.udteFiltro_ValueChanged);
            this.udteFiltroDA.ValueChanged += new System.EventHandler(this.udteFiltro_ValueChanged);
            this.ubAvanti.Enabled = true;
        }

        private void ubProsegui_Click(object sender, EventArgs e)
        {

            try
            {

                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.ProseguiTerapia) == DialogResult.OK)
                {
                    this.Aggiorna();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubProsegui_Click", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
            }

        }

        private void UltraTimelineView_SelectedDateTimeRangeChanging(object sender, SelectedDateTimeRangeChangingEventArgs e)
        {
            if ((e.Range != null) && (this.UltraTimelineView.ActiveOwner != null) && (this.UltraTimelineView.ActiveOwner.BindingListObject != null))
            {

                DataRowView oRowView = (System.Data.DataRowView)this.UltraTimelineView.ActiveOwner.BindingListObject;
                if (oRowView["PermessoInserimento"].ToString() == "1" && CoreStatics.CoreApplication.Cartella.CartellaChiusa == false)
                {
                    this.NuovoParametro(oRowView["CodEntita"].ToString(), oRowView["CodSezione"].ToString(), oRowView["CodVoce"].ToString(), e.Range.StartDateTime);
                    this.UltraTimelineView.ClearSelectedDateTimeRange();
                }

            }
            e.Cancel = true;
        }

        private void UltraTimelineView_PropertyChanged(object sender, Infragistics.Win.PropertyChangedEventArgs e)
        {



        }

        private void UltraCalendarInfo_AppointmentDataInitialized(object sender, Infragistics.Win.UltraWinSchedule.AppointmentDataInitializedEventArgs e)
        {

            try
            {

                DataRowView oRowView = (System.Data.DataRowView)e.Appointment.BindingListObject;

                if (oRowView["ColoreValore"] != null)
                {
                    e.Appointment.Appearance.BackColor = CoreStatics.GetColorFromString(oRowView["ColoreValore"].ToString());
                    e.Appointment.Appearance.FontData.Bold = DefaultableBoolean.True;
                    e.Appointment.Appearance.FontData.SizeInPoints = 10;

                }
                e.Appointment.Appearance.BackGradientStyle = Infragistics.Win.GradientStyle.None;


            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "UltraCalendarInfo_AppointmentDataInitialized", "ucFoglioUnico");
            }

            e.Appointment.Appearance.ForeColor = Color.Black;
            e.Appointment.Locked = true;
        }

        private void UltraCalendarInfo_OwnerDataInitialized(object sender, Infragistics.Win.UltraWinSchedule.OwnerDataInitializedEventArgs e)
        {


            try
            {

                if (((System.Data.DataRowView)e.Owner.BindingListObject)["ColoreAgenda"] != null)
                {
                    e.Owner.DayAppearance.BackColor = CoreStatics.GetColorFromString(((System.Data.DataRowView)e.Owner.BindingListObject)["ColoreAgenda"].ToString());
                    e.Owner.HeaderAppearance.BackColor = e.Owner.DayAppearance.BackColor;
                }


                if (((System.Data.DataRowView)e.Owner.BindingListObject)["IDIcona"].ToString() != string.Empty)
                {
                    if (oIcone.ContainsKey(((System.Data.DataRowView)e.Owner.BindingListObject)["IDIcona"].ToString()) == false)
                    {
                        oIcone.Add(((System.Data.DataRowView)e.Owner.BindingListObject)["IDIcona"].ToString(), CoreStatics.GetImageForGrid(Convert.ToInt32(((System.Data.DataRowView)e.Owner.BindingListObject)["IDIcona"].ToString()), 48));
                    }
                    e.Owner.HeaderAppearance.Image = DrawingProcs.GetImageFromByte(oIcone[((System.Data.DataRowView)e.Owner.BindingListObject)["IDIcona"].ToString()]);
                    e.Owner.HeaderAppearance.ImageHAlign = Infragistics.Win.HAlign.Left;
                }

                this.SetWorkingHour(e.Owner);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "UltraCalendarInfo_OwnerDataInitialized", "ucFoglioUnico");
            }

            e.Owner.Locked = true;
        }

        private void TimerChange_Tick(object sender, EventArgs e)
        {
            this.TimerChange.Enabled = false;
            this.Aggiorna();
        }

        private void ubMaximize_Click(object sender, EventArgs e)
        {
            try
            {
                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);
                Form frm = this.FindForm();
                if (frm != null && frm is Interfacce.IViewFormMain)
                {
                    (frm as Interfacce.IViewFormMain).ControlloCentraleMassimizzato = !(frm as Interfacce.IViewFormMain).ControlloCentraleMassimizzato;

                    AggiornaNomePaziente();

                    setPulsanteMax();
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubMaximize_Click", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
            }
        }

        private void UltraToolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {

            bool bMenuRiga = false;
            var btnMenuRiga = (PopupMenuTool)this.UltraToolbarsManager.Tools["MenuRiga"];
            Owner oOwner = null;

            switch (e.Tool.Key)
            {

                case "MRInserimento":
                    oOwner = (Owner)btnMenuRiga.Tag;
                    if (oOwner != null)
                    {
                        DataRowView oRowView = (System.Data.DataRowView)oOwner.BindingListObject;
                        this.NuovoParametro(oRowView["CodEntita"].ToString(), oRowView["CodSezione"].ToString(), oRowView["CodVoce"].ToString(), DateTime.Now);
                    }
                    bMenuRiga = true;
                    break;

                case "MRAzione":
                    oOwner = (Owner)btnMenuRiga.Tag;
                    if (oOwner != null)
                    {
                        DataRowView oRowView = (System.Data.DataRowView)oOwner.BindingListObject;
                        string sazione = (oRowView["Azione"].ToString() == "C" ? "Chiudere" : "Aprire");
                        if (easyStatics.EasyMessageBox("Sei sicuro di voler " + sazione + Environment.NewLine + "la prescrizione selezionata ?", sazione + " Prescrizioni", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            MovPrescrizione movpr = new MovPrescrizione(oRowView["IDEntita"].ToString(), CoreStatics.CoreApplication.Ambiente);
                            movpr.CodStatoContinuazione = (oRowView["Azione"].ToString() == "C" ? EnumStatoContinuazione.CH.ToString() : EnumStatoContinuazione.AP.ToString()); ;
                            if (movpr.Salva())
                            {
                                this.Aggiorna();
                            }
                        }
                    }
                    bMenuRiga = true;
                    break;

                case "MRSospendi":
                    oOwner = (Owner)btnMenuRiga.Tag;
                    if (oOwner != null)
                    {
                        DataRowView oRowView = (System.Data.DataRowView)oOwner.BindingListObject;
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;
                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata = new MovPrescrizione(oRowView["IDEntita"].ToString(), CoreStatics.CoreApplication.Ambiente);
                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.AnnullaPrescrizioneTempi) == DialogResult.OK)
                        {
                            foreach (MovPrescrizioneTempi movprt in CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Elementi)
                            {
                                if (movprt.CodStatoPrescrizioneTempi == EnumStatoPrescrizioneTempi.IC.ToString() ||
                                    movprt.CodStatoPrescrizioneTempi == EnumStatoPrescrizioneTempi.VA.ToString())
                                {

                                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = movprt;

                                    Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.PRT_SOSPENDI_PRIMA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente));

                                    if (oRispostaElaboraPrima.Successo)
                                    {
                                        movprt.CodStatoPrescrizioneTempi = EnumStatoPrescrizioneTempi.SS.ToString();
                                        movprt.Sospendi();
                                    }
                                }
                            }

                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;
                            this.Aggiorna();
                        }

                    }
                    bMenuRiga = true;
                    break;

                case "MRGrafico":
                    oOwner = (Owner)btnMenuRiga.Tag;
                    if (oOwner != null)
                    {
                        DataRowView oRowView = (System.Data.DataRowView)oOwner.BindingListObject;
                        if (oRowView["PermessoGrafico"].ToString() == "1")
                        {
                            string idEpisodio = (CoreStatics.CoreApplication.Episodio != null ? CoreStatics.CoreApplication.Episodio.ID : "");
                            DateTime datada = (this.udteFiltroDA.Value != null ? (DateTime)this.udteFiltroDA.Value : DateTime.MinValue);
                            DateTime dataa = (this.udteFiltroA.Value != null ? (DateTime)this.udteFiltroA.Value : DateTime.MinValue);
                            DateTime dataricovero = (CoreStatics.CoreApplication.Episodio != null ? CoreStatics.CoreApplication.Episodio.DataRicovero : DateTime.MinValue);

                            CoreStatics.CoreApplication.DefinizioneGraficoSelezionata = new ToolboxPerGrafici(idEpisodio, dataricovero, CoreStatics.CoreApplication.Paziente.CodSAC, (EnumEntita)Enum.Parse(typeof(EnumEntita), oRowView["CodEntita"].ToString()), oRowView["CodVoce"].ToString(), datada, dataa);
                            CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.GraficoParametriVitali);
                        }
                    }
                    bMenuRiga = true;
                    break;

                case "MRDettaglio":
                    oOwner = (Owner)btnMenuRiga.Tag;
                    if (oOwner != null)
                    {
                        DataRowView oRowView = (System.Data.DataRowView)oOwner.BindingListObject;
                        _ucEasyGridOrari = null;
                        _ucEasyGridOrari = CoreStatics.getGridOrariPrescrizioni(oRowView["IDEntita"].ToString());
                        if (_ucEasyGridOrari != null && _ucEasyGridOrari.DataSource != null)
                        {

                            _ucEasyGridOrari.Size = new Size(750, 500);

                            _ucEasyGridOrari.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                            CoreStatics.SetEasyUltraGridLayout(ref _ucEasyGridOrari);
                            _ucEasyGridOrari.DisplayLayout.CaptionVisible = DefaultableBoolean.True;

                            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerMain);



                            this.UltraPopupControlContainerMain.Show(sender as ucEasyGrid);

                            _ucEasyGridOrari.DisplayLayout.Bands[0].ClearGroupByColumns();

                            foreach (UltraGridColumn oCol in _ucEasyGridOrari.DisplayLayout.Bands[0].Columns)
                            {

                                oCol.SortIndicator = SortIndicator.Disabled;
                                switch (oCol.Key)
                                {
                                    case "DataRiferimento":
                                        oCol.Hidden = false;
                                        oCol.Header.Caption = "Data";
                                        oCol.Format = "dddd dd/MM/yyyy HH:mm";
                                        oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                        oCol.Header.VisiblePosition = 1;
                                        break;

                                    case "DescStato":
                                        oCol.Hidden = false;
                                        oCol.Header.Caption = "Stato";
                                        oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                        oCol.Header.VisiblePosition = 2;
                                        break;

                                    case "DescTipo":
                                        oCol.Hidden = false;
                                        oCol.Header.Caption = "Tipo";
                                        oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                        oCol.Header.VisiblePosition = 3;
                                        break;

                                    default:
                                        oCol.Hidden = true;
                                        break;
                                }
                            }

                            _ucEasyGridOrari.DisplayLayout.Bands[0].Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
                            _ucEasyGridOrari.Refresh();
                        }
                    }
                    bMenuRiga = true;
                    break;

                case "MRRtf":
                    oOwner = (Owner)btnMenuRiga.Tag;
                    if (oOwner != null)
                    {
                        DataRowView oRowView = (System.Data.DataRowView)oOwner.BindingListObject;
                        if (oRowView["RTF"].ToString() != string.Empty)
                        {
                            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                            _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(oRowView["RTF"].ToString());
                            this.UltraPopupControlContainer.Show(this.UltraTimelineView);
                        }
                    }
                    bMenuRiga = true;
                    break;

            }

            if (bMenuRiga == false)
            {

                if (this.UltraCalendarInfo.SelectedAppointments.Count == 1)
                {


                    DataRowView oRowView = (System.Data.DataRowView)this.UltraCalendarInfo.SelectedAppointments[0].BindingListObject;

                    switch ((EnumEntita)Enum.Parse(typeof(EnumEntita), oRowView["CodEntita"].ToString()))
                    {

                        case EnumEntita.PVT:
                            switch (e.Tool.Key)
                            {

                                case "ErogaRapida":
                                    break;

                                case "Eroga":
                                    break;

                                case "Modifica":
                                    CoreStatics.CoreApplication.MovParametroVitaleSelezionato = new MovParametroVitale(oRowView["IDRiferimento"].ToString(), EnumAzioni.MOD, CoreStatics.CoreApplication.Ambiente);
                                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingParametriVitali) == DialogResult.OK)
                                    {
                                        this.Aggiorna();
                                    }
                                    break;

                                case "Annulla":
                                    break;

                                case "Cancella":
                                    if (easyStatics.EasyMessageBox("Sei sicuro di voler CANCELLARE" + Environment.NewLine + "il parametro vitale selezionato ?", "Cancellazione Parametri Vitali", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        MovParametroVitale movpv = new MovParametroVitale(oRowView["IDRiferimento"].ToString(), CoreStatics.CoreApplication.Ambiente);
                                        movpv.CodStatoParametroVitale = @"CA";
                                        if (movpv.Cancella())
                                        {
                                            this.Aggiorna();
                                        }
                                    }
                                    break;

                                case "CambiaStato":
                                    break;

                                case "Visualizza":
                                    CoreStatics.CoreApplication.MovParametroVitaleSelezionato = new MovParametroVitale(oRowView["IDRiferimento"].ToString(), EnumAzioni.VIS, CoreStatics.CoreApplication.Ambiente);
                                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingParametriVitali) == DialogResult.OK)
                                    {
                                        this.Aggiorna();
                                    }
                                    break;

                                case "Copia":
                                    break;

                                case "Inoltra":
                                    break;

                            }
                            break;

                        case EnumEntita.WKI:
                        case EnumEntita.PRF:
                            switch (e.Tool.Key)
                            {

                                case "ErogaRapida":
                                    this.AzionePredefinitaRapida();
                                    break;

                                case "Eroga":
                                    this.AzionePredefinita();
                                    break;

                                case "Modifica":
                                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(oRowView["IDRiferimento"].ToString(), EnumAzioni.MOD, CoreStatics.CoreApplication.Ambiente);
                                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingTaskInfermieristici) == DialogResult.OK)
                                    {
                                        this.Aggiorna();
                                    }
                                    break;

                                case "Sposta":
                                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(oRowView["IDRiferimento"].ToString(),
                                                                                                                                EnumAzioni.MOD,
                                                                                                                                CoreStatics.CoreApplication.Ambiente);
                                    CoreStatics.SetUltraPopupControlContainer(this.ultraPopupControlContainerOra);
                                    _ucEasyPopUpOrario = new ucEasyPopUpOrario();
                                    _ucEasyPopUpOrario.DataOra = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata;
                                    _ucEasyPopUpOrario.Note = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Note;

                                    this.ultraPopupControlContainerOra.Show();

                                    break;

                                case "Annulla":
                                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(oRowView["IDRiferimento"].ToString(),
                                                                                                                                EnumAzioni.ANN,
                                                                                                                                CoreStatics.CoreApplication.Ambiente);
                                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.ErogazioneTaskInfermieristici) == DialogResult.OK)
                                    {
                                        this.Aggiorna();
                                    }
                                    break;

                                case "Cancella":
                                    if (easyStatics.EasyMessageBox("Confermi la cancellazione del task selezionato ?", "Cancellazione Task Infermieristici", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(oRowView["IDRiferimento"].ToString(), EnumAzioni.CAN, CoreStatics.CoreApplication.Ambiente);

                                        Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.WKI_CANCELLA_PRIMA.ToString(), new object[1] { "CANCELLAZIONE" }, CommonStatics.UAPadri(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente));
                                        if (oRispostaElaboraPrima.Successo)
                                        {
                                            if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Cancella())
                                            {
                                                this.Aggiorna();
                                            }
                                        }
                                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = null;

                                    }
                                    break;

                                case "CambiaStato":
                                    break;

                                case "Visualizza":
                                    this.AzionePredefinita();
                                    break;

                                case "Copia":
                                    if (easyStatics.EasyMessageBox("Confermi la copia del task infermieristico selezionato ?", "Copia Task Infermieristici", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        MovTaskInfermieristico movtiorigine = new MovTaskInfermieristico(oRowView["IDRiferimento"].ToString(), EnumAzioni.MOD, CoreStatics.CoreApplication.Ambiente);
                                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                                                                                                    CoreStatics.CoreApplication.Paziente.ID,
                                                                                                                                    CoreStatics.CoreApplication.Episodio.ID,
                                                                                                                                    CoreStatics.CoreApplication.Trasferimento.ID,
                                                                                                                                    EnumCodSistema.WKI, EnumTipoRegistrazione.M,
                                                                                                                                    CoreStatics.CoreApplication.Ambiente);
                                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CopiaDaOrigine(ref movtiorigine);
                                        movtiorigine = null;
                                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingTaskInfermieristici) == DialogResult.OK)
                                        {
                                            this.Aggiorna();
                                        }

                                    }
                                    break;

                                case "Inoltra":
                                    break;

                            }
                            break;

                        case EnumEntita.APP:
                            switch (e.Tool.Key)
                            {

                                case "ErogaRapida":
                                    break;

                                case "Eroga":
                                    break;

                                case "Modifica":
                                    this.AzionePredefinita();
                                    break;

                                case "Annulla":
                                    break;

                                case "Cancella":
                                    try
                                    {
                                        bool bEliminaAppuntamentoSingolo = true;
                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento(oRowView["IDRiferimento"].ToString(), EnumAzioni.MOD);

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
                                                            CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento(drApp["ID"].ToString(), EnumAzioni.MOD);

                                                            CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento = EnumStatoAppuntamento.CA.ToString();
                                                            PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                                            op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                                                            op.Parametro.Add("IDAppuntamento", drApp["ID"].ToString());
                                                            op.Parametro.Add("CodStatoAppuntamento", EnumStatoAppuntamento.CA.ToString());

                                                            op.TimeStamp.CodEntita = EnumEntita.APP.ToString();
                                                            op.TimeStamp.CodAzione = EnumAzioni.CAN.ToString();


                                                            spcoll = new SqlParameterExt[1];
                                                            xmlParam = XmlProcs.XmlSerializeToString(op);

                                                            xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                                                            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                                            Database.ExecStoredProc("MSP_AggMovAppuntamenti", spcoll);

                                                            PluginClientStatics.PluginClient(EnumPluginClient.APP_CANCELLA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                                        }
                                                    }
                                                    CoreStatics.ImpostaCursoreMainForm(Scci.Enums.enum_app_cursors.DefaultCursor);

                                                    this.Aggiorna();
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

                                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                                                op.Parametro.Add("IDAppuntamento", oRowView["IDRiferimento"].ToString());
                                                op.Parametro.Add("CodStatoAppuntamento", EnumStatoAppuntamento.CA.ToString());

                                                op.TimeStamp.CodEntita = EnumEntita.APP.ToString();
                                                op.TimeStamp.CodAzione = EnumAzioni.CAN.ToString();

                                                op.MovScheda = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.MovScheda;

                                                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                                                string xmlParam = XmlProcs.XmlSerializeToString(op);

                                                xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                                                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                                Database.ExecStoredProc("MSP_AggMovAppuntamenti", spcoll);

                                                PluginClientStatics.PluginClient(EnumPluginClient.APP_CANCELLA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                                CoreStatics.ImpostaCursoreMainForm(Scci.Enums.enum_app_cursors.DefaultCursor);


                                                this.Aggiorna();
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        CoreStatics.ExGest(ref ex, "UltraToolbarsManager_ToolClick", this.Name);
                                        CoreStatics.ImpostaCursoreMainForm(Scci.Enums.enum_app_cursors.DefaultCursor);
                                    }

                                    break;

                                case "CambiaStato":
                                    CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento(oRowView["IDRiferimento"].ToString(), EnumAzioni.MOD);

                                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneStatoAppuntamento) == DialogResult.OK)
                                    {

                                        PluginClientStatics.PluginClient(EnumPluginClient.APP_PRE_SALVA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));


                                        Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                                        op.Parametro.Add("IDAppuntamento", oRowView["IDRiferimento"].ToString());
                                        op.Parametro.Add("CodStatoAppuntamento", CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento);

                                        op.TimeStamp.CodEntita = EnumEntita.APP.ToString();
                                        if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.AN.ToString())
                                            op.TimeStamp.CodAzione = EnumAzioni.ANN.ToString();
                                        else
                                            op.TimeStamp.CodAzione = EnumAzioni.MOD.ToString();

                                        if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.ER.ToString() ||
                                            CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.AN.ToString()
                                            )
                                        {
                                            CoreStatics.CompletaDatiAppuntamento(CoreStatics.CoreApplication.MovAppuntamentoSelezionato, CoreStatics.CoreApplication.Ambiente);
                                        }


                                        op.MovScheda = CoreStatics.CoreApplication.MovAppuntamentoSelezionato.MovScheda;

                                        SqlParameterExt[] spcoll = new SqlParameterExt[1];
                                        string xmlParam = XmlProcs.XmlSerializeToString(op);

                                        xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                        Database.ExecStoredProc("MSP_AggMovAppuntamenti", spcoll);

                                        if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.AN.ToString())
                                        {
                                            PluginClientStatics.PluginClient(EnumPluginClient.APP_ANNULLA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                            CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);
                                        }
                                        else
                                        {

                                            if (CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodStatoAppuntamento == EnumStatoAppuntamento.ER.ToString())
                                            {
                                                CoreStatics.CoreApplication.MovAppuntamentoSelezionato.Ripianificazione(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente);
                                            }

                                        }
                                        this.Aggiorna();
                                    }
                                    break;

                                case "Visualizza":
                                    break;

                                case "Copia":
                                    if (easyStatics.EasyMessageBox("Confermi la copia dell'appuntamento selezionato ?", "Copia Appuntamento", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {

                                        MovAppuntamento movtiorigine = new MovAppuntamento(oRowView["IDRiferimento"].ToString(), EnumAzioni.MOD);
                                        movtiorigine.CodStatoAppuntamento = EnumStatoAppuntamento.PR.ToString();
                                        string pazienteid = "";
                                        string trasferimentocodua = "";
                                        string episodioid = "";
                                        string trasferimentoid = "";
                                        if (CoreStatics.CoreApplication.Trasferimento != null)
                                        {
                                            trasferimentoid = CoreStatics.CoreApplication.Trasferimento.ID;
                                            trasferimentocodua = CoreStatics.CoreApplication.Trasferimento.CodUA;
                                        }

                                        if (CoreStatics.CoreApplication.Paziente != null)
                                            pazienteid = CoreStatics.CoreApplication.Paziente.ID;

                                        if (CoreStatics.CoreApplication.Episodio != null)
                                            episodioid = CoreStatics.CoreApplication.Episodio.ID;

                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato = new MovAppuntamento(trasferimentocodua,
                                                                                                                     pazienteid,
                                                                                                                     episodioid,
                                                                                                                     trasferimentoid);

                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CopiaDaOrigine(ref movtiorigine);
                                        movtiorigine = null;

                                        while (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneAgendeAppuntamento) == DialogResult.OK)
                                        {
                                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneAppuntamento) == DialogResult.OK)
                                            {


                                                if (CoreStatics.CoreApplication.MovAppuntamentiGenerati != null && CoreStatics.CoreApplication.MovAppuntamentiGenerati.Count > 1)
                                                {
                                                    for (int ma = 0; ma < CoreStatics.CoreApplication.MovAppuntamentiGenerati.Count; ma++)
                                                    {
                                                        CoreStatics.CoreApplication.MovAppuntamentoSelezionato = CoreStatics.CoreApplication.MovAppuntamentiGenerati[ma];
                                                        PluginClientStatics.PluginClient(EnumPluginClient.APP_NUOVO_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));

                                                    }
                                                }
                                                else
                                                {
                                                    PluginClientStatics.PluginClient(EnumPluginClient.APP_NUOVO_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovAppuntamentoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                                }


                                                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);

                                                this.Aggiorna();
                                                break;
                                            }
                                        }

                                    }
                                    break;

                                case "Inoltra":
                                    break;

                            }
                            break;

                        case EnumEntita.OE:

                            if (!_locknuovoordine)
                            {
                                _locknuovoordine = true;

                                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                                string sCodAziOrdine = string.Empty;
                                if (CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento != null && CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento != string.Empty)
                                { sCodAziOrdine = CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento; }
                                else
                                { sCodAziOrdine = CoreStatics.CoreApplication.Episodio.CodAzienda; }

                                CoreStatics.CoreApplication.MovOrdineSelezionato = new MovOrdine(oRowView["IDRiferimento"].ToString(),
                                                                            CoreStatics.CoreApplication.Ambiente, CoreStatics.CoreApplication.Episodio.ID,
                                                                            CoreStatics.CoreApplication.Episodio.CodTipoEpisodio,
                                                                            CoreStatics.CoreApplication.Episodio.NumeroEpisodio,
                                                                            CoreStatics.CoreApplication.Episodio.NumeroListaAttesa,
                                                                            sCodAziOrdine,
                                                                            CoreStatics.CoreApplication.Trasferimento.ID,
                                                                            CoreStatics.CoreApplication.Trasferimento.CodUO,
                                                                            CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                                            CoreStatics.CoreApplication.Paziente);
                                switch (e.Tool.Key)
                                {
                                    case "Modifica":
                                        CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi = false;
                                        while (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingOrdine) == DialogResult.OK)
                                        {
                                            if (CoreStatics.CoreApplication.MovOrdineSelezionato != null && CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi)
                                            {
                                                this.Aggiorna();
                                                break;
                                            }
                                            else
                                            {
                                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingOrdineDatiAggiuntivi) == DialogResult.OK)
                                                {
                                                    this.Aggiorna();
                                                    break;
                                                }
                                            }

                                        }
                                        if (CoreStatics.CoreApplication.MovOrdineSelezionato != null)
                                            CoreStatics.CoreApplication.MovOrdineSelezionato.SkipDatiAggiuntivi = false;
                                        break;

                                    case "Copia":
                                        if (easyStatics.EasyMessageBox("Sei sicuro di voler COPIARE" + Environment.NewLine + "l'ordine selezionato ?", "Copia Ordini", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                        {
                                            UnicodeSrl.Scci.DataContracts.OEOrdineTestata ordinecopia = CoreStatics.CoreApplication.MovOrdineSelezionato.CopiaOrdine3(CoreStatics.getParameterCopiaOrdine(false));
                                            this.Aggiorna();
                                        }
                                        break;

                                    case "Cancella":
                                        if (CoreStatics.CoreApplication.MovOrdineSelezionato.Cancellabile)
                                        {
                                            if (easyStatics.EasyMessageBox("Sei sicuro di voler CANCELLARE" + Environment.NewLine + "l'ordine selezionato ?", "Cancellazione Ordini", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                            {
                                                bool bDelete = true;
                                                if (CoreStatics.CoreApplication.MovOrdineSelezionato.StatoOrdine != OEStato.Inserito)
                                                {
                                                    string sMsg = @"";
                                                    sMsg += @"L'ordine è già stato inoltrato";
                                                    if (CoreStatics.CoreApplication.MovOrdineSelezionato.UtenteInoltro != null && CoreStatics.CoreApplication.MovOrdineSelezionato.UtenteInoltro.Trim() != "")
                                                        sMsg += @" da " + CoreStatics.CoreApplication.MovOrdineSelezionato.UtenteInoltro;
                                                    if (CoreStatics.CoreApplication.MovOrdineSelezionato.DataInoltro > DateTime.MinValue)
                                                        sMsg += @" in data/ora " + CoreStatics.CoreApplication.MovOrdineSelezionato.DataInoltro.ToString(@"dd/MM/yyyy HH:mm");
                                                    sMsg += @"." + Environment.NewLine;
                                                    sMsg += @"L'operazione potrebbe non andare a buon fine o bloccare l'operatività clinica." + Environment.NewLine;
                                                    sMsg += @"Sei sicuro?";
                                                    if (easyStatics.EasyMessageBox(sMsg, "Cancellazione Ordini", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                                                        bDelete = false;
                                                }

                                                if (bDelete)
                                                {
                                                    CoreStatics.CoreApplication.MovOrdineSelezionato.CancellaOrdine();
                                                    this.Aggiorna();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            easyStatics.EasyMessageBox("Ordine NON cancellabile!!!", "Cancellazione Ordini", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        }
                                        break;

                                    case "Visualizza":
                                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.VisualizzaOrdine);
                                        break;

                                    case "Inoltra":
                                        if (CoreStatics.CoreApplication.MovOrdineSelezionato.StatoValiditaOrdine == OEValiditaOrdine.Valido &&
    CoreStatics.CoreApplication.MovOrdineSelezionato.Prestazioni.Count > 0)
                                        {
                                            if (CoreStatics.CoreApplication.MovOrdineSelezionato.DataProgrammazione > new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0))
                                            {
                                                CoreStatics.CoreApplication.MovOrdineSelezionato.InoltraOrdine();
                                                if (CoreStatics.CoreApplication.MovOrdineSelezionato != null && CoreStatics.CoreApplication.MovOrdineSelezionato.StatoValiditaOrdine != OEValiditaOrdine.Valido)
                                                {
                                                    easyStatics.EasyMessageBox("Impossibile inoltrare ordine, ritornato stato validazione non valido.", "Inoltro Ordine", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                                }
                                                else
                                                {
                                                    Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.OE_INOLTRA_DOPO.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovOrdineSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                                                    if (oRispostaElaboraPrima.Successo == true)
                                                    {
                                                    }
                                                }
                                                this.Aggiorna();
                                            }
                                            else
                                            {
                                                easyStatics.EasyMessageBox("Impossibile inoltrare un ordine con data di programmazione nel passato.", "Inoltro Ordine", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                            }
                                        }
                                        else
                                        {
                                            easyStatics.EasyMessageBox(@"Impossibile inoltrare un ordine non valido per l'inoltro." + Environment.NewLine +
                                                                        @"Controllare le prestazioni ed i dati aggiuntivi inseriti.", "Inoltro Ordine", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                        }
                                        break;

                                    default:
                                        break;
                                }

                                CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.DefaultCursor);

                                _locknuovoordine = false;
                            }


                            break;
                    }

                    this.UltraTimelineView.CalendarInfo.SelectedAppointments.Clear();

                }
                else
                {


                    var btnMenu = (PopupMenuTool)this.UltraToolbarsManager.Tools["MenuPrescrizione"];
                    string codvoce = btnMenu.Tag.ToString();
                    DateTime dataevento = (DateTime)e.Tool.Tag;

                    switch (e.Tool.Key)
                    {

                        case "Continua":
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = new MovPrescrizioneTempi(new Guid(codvoce), CoreStatics.CoreApplication.Ambiente);
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Azione = EnumAzioni.INS;
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataEvento = DateTime.Now;
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio = dataevento;
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraFine = dataevento;
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodStatoPrescrizioneTempi = EnumStatoPrescrizioneTempi.VA.ToString();
                            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerSomministrazione);
                            _ucEasyPopUpOrario = new ucEasyPopUpOrario();
                            _ucEasyPopUpOrario.TestoTextBox = "Posologia:";
                            _ucEasyPopUpOrario.DataOra = dataevento;
                            _ucEasyPopUpOrario.Note = getUltimaPosologia(codvoce, dataevento);
                            this.UltraPopupControlContainerSomministrazione.Show();
                            break;

                        case "Singola":
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = new MovPrescrizioneTempi(new Guid(codvoce), CoreStatics.CoreApplication.Ambiente);
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Azione = EnumAzioni.INS;
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataEvento = DateTime.Now;
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio = dataevento;
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraFine = dataevento;
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodStatoPrescrizioneTempi = EnumStatoPrescrizioneTempi.VA.ToString();
                            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerSomministrazione);
                            _ucEasyPopUpOrario = new ucEasyPopUpOrario();
                            _ucEasyPopUpOrario.TestoTextBox = "Posologia:";
                            _ucEasyPopUpOrario.DataOra = dataevento;
                            _ucEasyPopUpOrario.Note = string.Empty;
                            this.UltraPopupControlContainerSomministrazione.Show();
                            break;

                        case "Somministrazione":
                            CoreStatics.CoreApplication.MovPrescrizioneSelezionata = new MovPrescrizione(codvoce, CoreStatics.CoreApplication.Ambiente);
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = new MovPrescrizioneTempi(new Guid(codvoce), CoreStatics.CoreApplication.Ambiente);
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Azione = EnumAzioni.INS;
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataEvento = DateTime.Now;
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio = dataevento;
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraFine = dataevento;
                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingPrescrizioneTempi) == DialogResult.OK)
                            {
                                this.Aggiorna();
                            }
                            break;

                        case "TerapiaRapida":
                            CoreStatics.CoreApplication.MovPrescrizioneSelezionata = new MovPrescrizione(CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                                                                            CoreStatics.CoreApplication.Paziente.ID,
                                                                                                            CoreStatics.CoreApplication.Episodio.ID,
                                                                                                            CoreStatics.CoreApplication.Trasferimento.ID,
                                                                                                            CoreStatics.CoreApplication.Ambiente);
                            CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Azione = EnumAzioni.INS;
                            CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DataEvento = DateTime.Now;

                            CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodTipoPrescrizione = Database.GetConfigTable(EnumConfigTable.TipoPrescrizioneSemplice);

                            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                            op.Parametro.Add("CodUA", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodUA);
                            op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
                            op.Parametro.Add("CodAzione", EnumAzioni.INS.ToString());
                            op.Parametro.Add("DatiEstesi", "1");
                            op.Parametro.Add("Codice", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodTipoPrescrizione);
                            SqlParameterExt[] spcoll = new SqlParameterExt[1];
                            string xmlParam = XmlProcs.XmlSerializeToString(op);
                            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                            DataTable oDt = Database.GetDataTableStoredProc("MSP_SelTipoPrescrizione", spcoll);
                            if (oDt.Rows.Count == 1)
                            {
                                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DescrTipoPrescrizione = oDt.Rows[0]["Descrizione"].ToString();
                                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodScheda = oDt.Rows[0]["CodScheda"].ToString();
                                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodViaSomministrazione = oDt.Rows[0]["CodViaSomministrazione"].ToString();
                                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DescrViaSomministrazione = oDt.Rows[0]["DescrViaSomministrazione"].ToString();
                            }
                            _ucEasyPopUpTerapiaRapida = new ucEasyPopUpTerapiaRapida();
                            _ucEasyPopUpTerapiaRapida.DataOra = dataevento;
                            _ucEasyPopUpTerapiaRapida.ViaSomministrazione = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodViaSomministrazione;
                            _ucEasyPopUpTerapiaRapida.Prescrizione = string.Empty;
                            _ucEasyPopUpTerapiaRapida.Posologia = string.Empty;
                            this.UltraPopupControlContainerTerapiaRapida.Show();
                            break;

                        case "ProseguiTerapia":
                            DateTime dtInizio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                            DateTime dtFine = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 0);

                            DateTime dtInizioP = new DateTime(dataevento.Year, dataevento.Month, dataevento.Day, 0, 0, 0);
                            DateTime dtFineP = new DateTime(dataevento.Year, dataevento.Month, dataevento.Day, 23, 59, 0);

                            CoreStatics.CoreApplication.ProseguiTerapiaSelezionata = new ProseguiTerapia(dtInizio, dtFine, dtInizioP, dtFineP);
                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.ProseguiTerapia) == DialogResult.OK)
                            {
                                this.Aggiorna();
                            }
                            CoreStatics.CoreApplication.ProseguiTerapiaSelezionata = null;
                            break;

                        case "Terapia":
                            CoreStatics.CoreApplication.MovPrescrizioneSelezionata = new MovPrescrizione(CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                                                                           CoreStatics.CoreApplication.Paziente.ID,
                                                                                                           CoreStatics.CoreApplication.Episodio.ID,
                                                                                                           CoreStatics.CoreApplication.Trasferimento.ID,
                                                                                                           CoreStatics.CoreApplication.Ambiente);
                            CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Azione = EnumAzioni.INS;
                            CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DataEvento = DateTime.Now;
                            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneTipoPrescrizione) == DialogResult.OK)
                            {
                                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata != null)
                                {
                                    CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingPrescrizione, false);
                                    this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione, "PFM"));
                                }
                                else
                                {
                                    CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingPrescrizioniProtocollo, false, CoreStatics.CoreApplication.ListaIDMovPrescrizioniCreate);
                                    CoreStatics.CoreApplication.ListaIDMovPrescrizioniCreate = null;
                                    this.Aggiorna();
                                }
                            }
                            break;

                    }

                }

            }

        }

        private void UltraTimelineView_Click(object sender, EventArgs e)
        {

            Point oPoint = this.UltraTimelineView.PointToClient(Cursor.Position);

            Appointment oApp = this.UltraTimelineView.AppointmentFromPoint(oPoint);
            if (oApp != null)
            {

                this.UltraCalendarInfo.SelectedAppointments.Clear();

                this.UltraCalendarInfo.SelectedAppointments.Add(oApp);

                try
                {

                    if (this.UltraCalendarInfo.SelectedAppointments.Count == 1)
                    {

                        DataRowView oRowView = (System.Data.DataRowView)this.UltraCalendarInfo.SelectedAppointments[0].BindingListObject;

                        if (oRowView["PermessoModifica"].ToString() == "1" || oRowView["PermessoVisualizza"].ToString() == "1" || oRowView["PermessoEroga"].ToString() == "1")
                        {

                            switch ((EnumEntita)Enum.Parse(typeof(EnumEntita), oRowView["CodEntita"].ToString()))
                            {

                                case EnumEntita.NTG:
                                    this.AzionePredefinita();
                                    break;

                                case EnumEntita.PVT:
                                    this.AzioneMenu();
                                    break;

                                case EnumEntita.WKI:
                                    this.AzioneMenu();
                                    break;

                                case EnumEntita.PRF:
                                    if (oRowView["CodSezione"].ToString() == "PFM")
                                    {
                                        this.AzioneMenu();
                                    }
                                    else
                                    {
                                        this.AzionePredefinita();
                                    }
                                    break;

                                case EnumEntita.EVC:
                                    this.AzionePredefinita();
                                    break;

                                case EnumEntita.APP:
                                    this.AzioneMenu();
                                    break;

                                case EnumEntita.DCL:
                                    this.AzionePredefinita();
                                    break;

                                case EnumEntita.OE:
                                    this.AzioneMenu();
                                    break;

                            }

                        }

                    }

                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "UltraCalendarInfo_AfterSelectedAppointmentsChange", "ucFoglioUnico");
                }

            }

        }

        private void UltraTimelineView_Resize(object sender, EventArgs e)
        {
            this.SetTipoVisualizzazione();
        }

        #endregion

        #region UIElement Event


























































        private void OnCustomButtonMenuClick(Object sender, UIElementEventArgs e)
        {
            if (e.Element.GetContext(typeof(Owner)) is Owner oOwner)
            {

                DataRowView oRowView = (System.Data.DataRowView)oOwner.BindingListObject;

                var btnMenu = (PopupMenuTool)this.UltraToolbarsManager.Tools["MenuRiga"];

                btnMenu.Tag = oOwner;

                ((ButtonTool)btnMenu.Tools["MRInserimento"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_NUOVO_32);
                ((ButtonTool)btnMenu.Tools["MRAzione"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_SI_32);
                ((ButtonTool)btnMenu.Tools["MRSospendi"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_SOSPENDI_32);
                ((ButtonTool)btnMenu.Tools["MRGrafico"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_PARAMETRIVITALIGRAFICO_32);
                ((ButtonTool)btnMenu.Tools["MRDettaglio"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_VISUALIZZA_32);
                ((ButtonTool)btnMenu.Tools["MRRtf"]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_RTF_32);

                ((ButtonTool)btnMenu.Tools["MRInserimento"]).InstanceProps.Visible =
                    ((int.Parse(oRowView["PermessoInserimento"].ToString()) == 1 && CoreStatics.CoreApplication.Cartella.CartellaChiusa == false) ? DefaultableBoolean.True : DefaultableBoolean.False);

                ((ButtonTool)btnMenu.Tools["MRAzione"]).InstanceProps.Visible =
                    ((oRowView["Azione"].ToString() != string.Empty && CoreStatics.CoreApplication.Cartella.CartellaChiusa == false) ? DefaultableBoolean.True : DefaultableBoolean.False);
                ((ButtonTool)btnMenu.Tools["MRAzione"]).InstanceProps.Caption = (oRowView["Azione"].ToString() == "C" ? "Chiudi" : "Apri");
                ((ButtonTool)btnMenu.Tools["MRAzione"]).InstanceProps.AppearancesLarge.Appearance.Image = (oRowView["Azione"].ToString() == "C" ? Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSO_32) : Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOAPERTO_32));

                ((ButtonTool)btnMenu.Tools["MRSospendi"]).InstanceProps.Visible =
                    ((int.Parse(oRowView["PermessoSospendi"].ToString()) == 1 && CoreStatics.CoreApplication.Cartella.CartellaChiusa == false) ? DefaultableBoolean.True : DefaultableBoolean.False);

                ((ButtonTool)btnMenu.Tools["MRGrafico"]).InstanceProps.Visible =
                    ((int.Parse(oRowView["PermessoGrafico"].ToString()) == 1) ? DefaultableBoolean.True : DefaultableBoolean.False);

                ((ButtonTool)btnMenu.Tools["MRDettaglio"]).InstanceProps.Visible =
                    ((int.Parse(oRowView["PermessoDettaglio"].ToString()) == 1) ? DefaultableBoolean.True : DefaultableBoolean.False);

                ((ButtonTool)btnMenu.Tools["MRRtf"]).InstanceProps.Visible =
                    ((oRowView["RTF"].ToString() != string.Empty) ? DefaultableBoolean.True : DefaultableBoolean.False);

                btnMenu.ShowPopup();

            }

        }

        private void OnCustomButtonTreeAddClick(Object sender, UIElementEventArgs e)
        {

            UltraTreeNode oNode = (UltraTreeNode)e.Element.SelectableItem;
            this.NuovoParametro(oNode.Tag.ToString(), oNode.Key, "", DateTime.Now);

        }

        #endregion

        #region UltraPopupControlContainer

        private void UltraPopupControlContainer_Closed(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick -= ucRichTextBox_Click;
        }

        private void UltraPopupControlContainer_Opened(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick += ucRichTextBox_Click;
        }

        private void UltraPopupControlContainer_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucRichTextBox;
        }

        private void ucRichTextBox_Click(object sender, EventArgs e)
        {
        }

        #endregion

        #region UltraPopupControlContainerMain

        private void UltraPopupControlContainerMain_Closed(object sender, EventArgs e)
        {
            _ucEasyGridOrari.ClickCell -= ucEasyGridOrari_ClickCell;
        }

        private void UltraPopupControlContainerMain_Opened(object sender, EventArgs e)
        {
            _ucEasyGridOrari.ClickCell += ucEasyGridOrari_ClickCell;
            _ucEasyGridOrari.Focus();
        }

        private void UltraPopupControlContainerMain_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucEasyGridOrari;
        }

        private void ucEasyGridOrari_ClickCell(object sender, ClickCellEventArgs e)
        {
            this.UltraPopupControlContainerMain.Close();
        }


        #endregion

        #region UltraPopupControlContainerOra

        private void ultraPopupControlContainerOra_Closed(object sender, EventArgs e)
        {
            _ucEasyPopUpOrario.ConfermaClick -= ucEasyPopUpOrario_ConfermaClick;
            _ucEasyPopUpOrario.AnnullaClick -= ucEasyPopUpOrario_AnnullaClick;
        }

        private void ultraPopupControlContainerOra_Opened(object sender, EventArgs e)
        {
            _ucEasyPopUpOrario.ConfermaClick += ucEasyPopUpOrario_ConfermaClick;
            _ucEasyPopUpOrario.AnnullaClick += ucEasyPopUpOrario_AnnullaClick;
            _ucEasyPopUpOrario.Focus();
        }

        private void ultraPopupControlContainerOra_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popupora = sender as Infragistics.Win.Misc.UltraPopupControlContainer;

            int iWidth = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large)) * 20;
            int iHeight = Convert.ToInt32((double)iWidth / 1.52D);
            _ucEasyPopUpOrario.Size = new Size(iWidth, iHeight);
            popupora.PopupControl = _ucEasyPopUpOrario;
        }

        private void ucEasyPopUpOrario_AnnullaClick(object sender, EventArgs e)
        {
            this.ultraPopupControlContainerOra.Close();
            CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = null;
        }

        private void ucEasyPopUpOrario_ConfermaClick(object sender, EventArgs e)
        {
            if (_ucEasyPopUpOrario.DataOra != null)
            {
                try
                {
                    this.ultraPopupControlContainerOra.Close();

                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata = (DateTime)_ucEasyPopUpOrario.DataOra;
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Note = _ucEasyPopUpOrario.Note;

                    if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Salva())
                    {
                        this.Aggiorna();
                    }

                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "ucEasyPopUpOrario_ConfermaClick", this.Name);
                }
                finally
                {
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = null;
                }
            }
        }

        #endregion

        #region UltraPopupControlContainerSomministrazione

        private void UltraPopupControlContainerSomministrazione_Closed(object sender, EventArgs e)
        {
            _ucEasyPopUpOrario.ConfermaClick -= ucEasyPopUpOrarioSomministrazione_ConfermaClick;
            _ucEasyPopUpOrario.AnnullaClick -= ucEasyPopUpOrarioSomministrazione_AnnullaClick;
        }

        private void UltraPopupControlContainerSomministrazione_Opened(object sender, EventArgs e)
        {
            _ucEasyPopUpOrario.ConfermaClick += ucEasyPopUpOrarioSomministrazione_ConfermaClick;
            _ucEasyPopUpOrario.AnnullaClick += ucEasyPopUpOrarioSomministrazione_AnnullaClick;
            _ucEasyPopUpOrario.Focus();
        }

        private void UltraPopupControlContainerSomministrazione_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popupora = sender as Infragistics.Win.Misc.UltraPopupControlContainer;

            int iWidth = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large)) * 20;
            int iHeight = Convert.ToInt32((double)iWidth / 1.52D);
            _ucEasyPopUpOrario.Size = new Size(iWidth, iHeight);
            popupora.PopupControl = _ucEasyPopUpOrario;
        }

        private void ucEasyPopUpOrarioSomministrazione_AnnullaClick(object sender, EventArgs e)
        {
            this.UltraPopupControlContainerSomministrazione.Close();
            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;
        }

        private void ucEasyPopUpOrarioSomministrazione_ConfermaClick(object sender, EventArgs e)
        {

            if (_ucEasyPopUpOrario.DataOra != null)
            {

                try
                {

                    this.UltraPopupControlContainerSomministrazione.Close();

                    CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio = (DateTime)_ucEasyPopUpOrario.DataOra;
                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraFine = (DateTime)_ucEasyPopUpOrario.DataOra;
                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Posologia = _ucEasyPopUpOrario.Note;

                    if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Salva())
                    {
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CreaTaskInfermieristici(EnumCodSistema.PRF, Database.GetConfigTable(EnumConfigTable.TipoSchedaTaskDaPrescrizione), EnumTipoRegistrazione.A);
                        this.Aggiorna();
                    }

                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "ucEasyPopUpOrarioSomministrazione_ConfermaClick", this.Name);
                }
                finally
                {
                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;
                }

            }

        }

        private string getUltimaPosologia(string codvoce, DateTime dataevento)
        {

            string sRet = string.Empty;

            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPrescrizione", codvoce);
                op.Parametro.Add("DataEvento", Database.dataOra105PerParametri(dataevento));
                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_SelUltimaPosologia", spcoll);

                if (dt.Rows.Count == 1)
                {
                    if (dt.Rows[0][0] != DBNull.Value)
                    {
                        sRet = dt.Rows[0][0].ToString();
                    }
                }

            }
            catch (Exception)
            {
                sRet = string.Empty;
            }

            return sRet;

        }

        #endregion

        #region UltraPopupControlContainerTerapiaRapida

        private void UltraPopupControlContainerTerapiaRapida_Closed(object sender, EventArgs e)
        {
            _ucEasyPopUpTerapiaRapida.ConfermaClick -= ucEasyPopUpOrarioTerapiaRapida_ConfermaClick;
            _ucEasyPopUpTerapiaRapida.AnnullaClick -= ucEasyPopUpOrarioTerapiaRapida_AnnullaClick;
        }

        private void UltraPopupControlContainerTerapiaRapida_Opened(object sender, EventArgs e)
        {
            _ucEasyPopUpTerapiaRapida.ConfermaClick += ucEasyPopUpOrarioTerapiaRapida_ConfermaClick;
            _ucEasyPopUpTerapiaRapida.AnnullaClick += ucEasyPopUpOrarioTerapiaRapida_AnnullaClick;
            _ucEasyPopUpTerapiaRapida.Focus();
        }

        private void UltraPopupControlContainerTerapiaRapida_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popupora = sender as Infragistics.Win.Misc.UltraPopupControlContainer;

            int iWidth = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large)) * 30;
            int iHeight = Convert.ToInt32((double)iWidth / 1.52D);
            _ucEasyPopUpTerapiaRapida.Size = new Size(iWidth, iHeight);
            popupora.PopupControl = _ucEasyPopUpTerapiaRapida;
        }

        private void ucEasyPopUpOrarioTerapiaRapida_AnnullaClick(object sender, EventArgs e)
        {
            this.UltraPopupControlContainerTerapiaRapida.Close();
            CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;
        }

        private void ucEasyPopUpOrarioTerapiaRapida_ConfermaClick(object sender, EventArgs e)
        {

            if (_ucEasyPopUpTerapiaRapida.DataOra != null && _ucEasyPopUpTerapiaRapida.ViaSomministrazione != string.Empty)
            {

                try
                {

                    this.UltraPopupControlContainerTerapiaRapida.Close();

                    CoreStatics.impostaCursore(ref _ucc, Scci.Enums.enum_app_cursors.WaitCursor);

                    CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodViaSomministrazione = _ucEasyPopUpTerapiaRapida.ViaSomministrazione;
                    CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DescrViaSomministrazione = _ucEasyPopUpTerapiaRapida.ViaSomministrazioneDes;

                    oGestore = CoreStatics.GetGestore();
                    this.CaricaGestore();
                    oGestore.ModificaValore(Database.GetConfigTable(EnumConfigTable.CampoSchedaDaPrescrizioneSemplice), 1, _ucEasyPopUpTerapiaRapida.Prescrizione);
                    CoreStatics.CoreApplication.MovPrescrizioneSelezionata.MovScheda.DatiXML = oGestore.SchedaDatiXML;

                    if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Salva())
                    {

                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = new MovPrescrizioneTempi(new Guid(CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione), CoreStatics.CoreApplication.Ambiente);
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Azione = EnumAzioni.INS;
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataEvento = DateTime.Now;
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraInizio = (DateTime)_ucEasyPopUpTerapiaRapida.DataOra;
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataOraFine = (DateTime)_ucEasyPopUpTerapiaRapida.DataOra;
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Posologia = _ucEasyPopUpTerapiaRapida.Posologia;
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodStatoPrescrizioneTempi = EnumStatoPrescrizioneTempi.VA.ToString();

                        if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Salva())
                        {
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CreaTaskInfermieristici(EnumCodSistema.PRF, Database.GetConfigTable(EnumConfigTable.TipoSchedaTaskDaPrescrizione), EnumTipoRegistrazione.A);
                            this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione, "PFM"));
                        }

                    }

                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "ucEasyPopUpOrarioTerapiaRapida_ConfermaClick", this.Name);
                }
                finally
                {
                    CoreStatics.CoreApplication.MovPrescrizioneSelezionata = null;
                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;
                }

            }

        }

        #endregion

        #region UltraPopupControlContainerNote

        private void UltraPopupControlContainerNote_Closed(object sender, EventArgs e)
        {
            _ucEasyPopUpNota.ConfermaClick -= ucEasyPopUpNota_ConfermaClick;
            _ucEasyPopUpNota.AnnullaClick -= ucEasyPopUpNota_AnnullaClick;
            _ucEasyPopUpNota.CancellaClick -= ucEasyPopUpNota_CancellaClick;
        }

        private void UltraPopupControlContainerNote_Opened(object sender, EventArgs e)
        {
            _ucEasyPopUpNota.ConfermaClick += ucEasyPopUpNota_ConfermaClick;
            _ucEasyPopUpNota.AnnullaClick += ucEasyPopUpNota_AnnullaClick;
            _ucEasyPopUpNota.CancellaClick += ucEasyPopUpNota_CancellaClick;
            _ucEasyPopUpNota.Focus();
        }

        private void UltraPopupControlContainerNote_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popupnota = sender as Infragistics.Win.Misc.UltraPopupControlContainer;

            int iWidth = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large)) * 20;
            int iHeight = Convert.ToInt32((double)iWidth);
            _ucEasyPopUpNota.Size = new Size(iWidth, iHeight);
            popupnota.PopupControl = _ucEasyPopUpNota;
        }

        private void ucEasyPopUpNota_ConfermaClick(object sender, EventArgs e)
        {

            if (_ucEasyPopUpNota.DataOra != null && _ucEasyPopUpNota.Nota != string.Empty)
            {
                try
                {
                    this.UltraPopupControlContainerNote.Close();

                    CoreStatics.CoreApplication.MovNotaSelezionata.DataInizio = (DateTime)_ucEasyPopUpNota.DataOra;
                    CoreStatics.CoreApplication.MovNotaSelezionata.DataFine = (DateTime)_ucEasyPopUpNota.DataOra;
                    CoreStatics.CoreApplication.MovNotaSelezionata.Oggetto = _ucEasyPopUpNota.Nota;

                    if (CoreStatics.CoreApplication.MovNotaSelezionata.Salva())
                    {
                        this.Aggiorna(GeneraCodAgenda(CoreStatics.CoreApplication.MovNotaSelezionata.IDNota, EnumEntita.NTG.ToString()));
                    }

                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "ucEasyPopUpNota_ConfermaClick", this.Name);
                }
                finally
                {
                    CoreStatics.CoreApplication.MovNotaSelezionata = null;
                }
            }

        }

        private void ucEasyPopUpNota_AnnullaClick(object sender, EventArgs e)
        {
            this.UltraPopupControlContainerNote.Close();
            CoreStatics.CoreApplication.MovNotaSelezionata = null;
        }

        private void ucEasyPopUpNota_CancellaClick(object sender, EventArgs e)
        {

            try
            {

                this.UltraPopupControlContainerNote.Close();

                if (easyStatics.EasyMessageBox("Sei sicuro di voler CANCELLARE" + Environment.NewLine + "la nota selezionata ?", "Cancellazione Note", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    CoreStatics.CoreApplication.MovNotaSelezionata.CodStatoNota = EnumStatoNota.CA.ToString();

                    if (CoreStatics.CoreApplication.MovNotaSelezionata.Salva())
                    {
                        this.Aggiorna();
                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyPopUpNota_CancellaClick", this.Name);
            }
            finally
            {
                CoreStatics.CoreApplication.MovNotaSelezionata = null;
            }

        }

        #endregion

        #region Events Drag & Drop per ordinamenti IN SOSPESO NON ATTIVATI

        private void UltraTimelineView_MouseDown(object sender, MouseEventArgs e)
        {

            Owner oOwner = this.UltraTimelineView.OwnerFromPoint(new Point(e.X, e.Y));
            Appointment oApp = this.UltraTimelineView.AppointmentFromPoint(new Point(e.X, e.Y));
            Nullable<DateTime> oDt = this.UltraTimelineView.DateTimeFromPoint(new Point(e.X, e.Y));

            this.UltraTimelineView.AllowDrop = false;
            if (oOwner != null && oApp == null && oDt.HasValue == false)
            {
                DataRowView oRowView = (System.Data.DataRowView)oOwner.BindingListObject;
                if (oRowView["PermessoInserimento"].ToString() == "1" &&
                    CoreStatics.CoreApplication.Cartella.CartellaChiusa == false &&
                    (oRowView["CodSezione"].ToString() == "PFM" || oRowView["CodSezione"].ToString() == "PFA"))
                {
                    this.UltraTimelineView.AllowDrop = true;
                }
            }

        }

        private void UltraTimelineView_MouseMove(object sender, MouseEventArgs e)
        {

            try
            {

                if (e.Button == MouseButtons.Left && this.UltraTimelineView.AllowDrop == true)
                {
                    if (this.UltraTimelineView.ActiveOwner != null)
                    {
                        Type ViewType = sender.GetType();
                        MethodInfo ViewMethod = ViewType.GetMethod("DoDragDrop");
                        ViewMethod.Invoke(sender, new object[] { this.UltraTimelineView.ActiveOwner, DragDropEffects.Move });
                    }
                }

            }
            catch (Exception)
            {

            }

        }

        private void UltraTimelineView_DragOver(object sender, DragEventArgs e)
        {

            try
            {

                if (e.Data.GetDataPresent(typeof(Owner)) && e.AllowedEffect != DragDropEffects.None)
                {

                    Owner oOwner = this.UltraTimelineView.OwnerFromPoint(this.UltraTimelineView.PointToClient(new Point(e.X, e.Y)));
                    Appointment oApp = this.UltraTimelineView.AppointmentFromPoint(this.UltraTimelineView.PointToClient(new Point(e.X, e.Y)));
                    Nullable<DateTime> oDt = this.UltraTimelineView.DateTimeFromPoint(this.UltraTimelineView.PointToClient(new Point(e.X, e.Y)));
                    if (oOwner != null && oApp == null && oDt.HasValue == false)
                    {

                        Owner oOwnerInizio = e.Data.GetData(typeof(Owner)) as Owner;
                        DataRowView oRowViewInizio = (System.Data.DataRowView)oOwnerInizio.BindingListObject;

                        DataRowView oRowView = (System.Data.DataRowView)oOwner.BindingListObject;

                        if (oRowViewInizio["CodAgenda"].ToString() != oRowView["CodAgenda"].ToString() &&
oRowViewInizio["IDIcona"].ToString() == oRowView["IDIcona"].ToString())
                        {
                            this.lblPaziente.Text = oOwner.Key;
                            e.Effect = DragDropEffects.Move;
                        }
                        else
                        {
                            this.lblPaziente.Text = "";
                            e.Effect = DragDropEffects.None;
                        }

                    }
                    else
                    {
                        this.lblPaziente.Text = "";
                        e.Effect = DragDropEffects.None;
                    }

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void UltraTimelineView_DragDrop(object sender, DragEventArgs e)
        {

            try
            {

                if (e.Data.GetDataPresent(typeof(Owner)))
                {

                    Owner oOwner = this.UltraTimelineView.OwnerFromPoint(this.UltraTimelineView.PointToClient(new Point(e.X, e.Y)));
                    Appointment oApp = this.UltraTimelineView.AppointmentFromPoint(this.UltraTimelineView.PointToClient(new Point(e.X, e.Y)));
                    Nullable<DateTime> oDt = this.UltraTimelineView.DateTimeFromPoint(this.UltraTimelineView.PointToClient(new Point(e.X, e.Y)));
                    if (oOwner != null && oApp == null && oDt.HasValue == false)
                    {

                        Owner oOwnerInizio = e.Data.GetData(typeof(Owner)) as Owner;
                        DataRowView oRowViewInizio = (System.Data.DataRowView)oOwnerInizio.BindingListObject;

                        DataRowView oRowView = (System.Data.DataRowView)oOwner.BindingListObject;

                    }

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

    }
}
