using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.Scci;
using UnicodeSrl.ScciResource;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.ScciCore.Common.TimersCB;
using System.Threading;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Framework.Data;

namespace UnicodeSrl.ScciCore
{
    public partial class ucTop :
        UserControl,
        I_RefreshTimer_Controllo
    {

        private ucRichTextBox _ucRichTextBox = null;
        private SynchronizationContext m_sync = null;

        private System.Timers.Timer m_timerTime;

        public ucTop()
        {
            InitializeComponent();

            m_sync = SynchronizationContext.Current;

        }

        void m_timerTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                CoreStatics.CoreApplication.Sessione.Ora = DateTime.Now;
                this.lblOrario.Text = DateTime.Now.ToLongTimeString();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        #region Declare

        public event ImmagineTopClickHandler ImmagineClick;

        #endregion

        #region Properties

        public string CodiceMaschera
        {
            get { return this.lblCodMaschera.Text; }
            set { this.lblCodMaschera.Text = value; }
        }

        #endregion

        #region Events override

        public override void Refresh()
        {

            try
            {

                var oSessione = CoreStatics.CoreApplication.Sessione;
                var oPaziente = CoreStatics.CoreApplication.Paziente;
                var oEpisodio = CoreStatics.CoreApplication.Episodio;
                var oCartella = CoreStatics.CoreApplication.Cartella;
                var oTrasferimento = CoreStatics.CoreApplication.Trasferimento;
                var oNavigazione = CoreStatics.CoreApplication.Navigazione;
                bool bCartellaChiusa = false;


                if (oSessione.Connettivita == true)
                {
                    if (oCartella != null && oCartella.CartellaChiusa == true)
                    {
                        bCartellaChiusa = true;
                    }
                    else
                        bCartellaChiusa = false;

                    this.pbUtente.Image = oSessione.Utente.Foto;
                    this.lblUtente.Text = oSessione.Utente.Descrizione;
                    this.lblComputer.Text = oSessione.Computer.Nome;

                    if (CoreStatics.CoreApplication.Sessione.Computer.SessioneRemota == true)
                    {
                        this.lblComputer.Text += " (V)";
                    }

                    this.lblConnesso.Text = string.Format("{0} ore {1}", oSessione.ConnessoDa.ToShortDateString(), oSessione.ConnessoDa.ToShortTimeString());
                    if (oSessione.Utente.Ruoli.RuoloSelezionato != null)
                    {
                        this.lblRuolo.Text = oSessione.Utente.Ruoli.RuoloSelezionato.Descrizione;
                        this.pbVociDiarioClinico.Visible = (oSessione.Utente.Ruoli.RuoloSelezionato.DaFirmare == 0 ? false : true);
                    }
                    else
                    {
                        this.lblRuolo.Text = "";
                        this.pbVociDiarioClinico.Visible = false;
                    }

                    if (oPaziente != null && oPaziente.Attivo == true)
                    {
                        this.lblPaziente.Text = string.Format("{0} {1}", oPaziente.Cognome, oPaziente.Nome);

                        this.lblSessoEta.Text = string.Format("({0}) ({1})", oPaziente.Sesso, oPaziente.Eta);

                        this.lblIndirizzo.Text = string.Format("Nato a {0}, il {1}", oPaziente.ComuneNascita, oPaziente.DataNascita.ToShortDateString());
                        this.lblCodiceFiscale.Text = string.Format("C.F. {0}", oPaziente.CodiceFiscale);

                        this.lblViewInfoPaziente.Visible = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Schede_Dettaglio_Paziente);

                        this.pbPaziente.Image = oPaziente.Foto;
                        this.pbPaziente.Visible = true;

                        if (bCartellaChiusa == true)
                        {
                            this.pbCartellaChiusa.Image = Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSO_256);
                            this.pbPaziente.Enabled = false;
                        }
                        else
                        {
                            this.pbCartellaChiusa.Image = null;
                        }
                        this.pbAllergie.Visible = true;
                        if (oPaziente.Allergie.Numero > 0)
                        {
                            this.pbAllergie.Image = Risorse.GetImageFromResource(Risorse.GC_ALERTALLERGIA_256);
                        }
                        else
                        {
                            this.pbAllergie.Image = Risorse.GetImageFromResource(Risorse.GC_ALERTALLERGIA_DISABLED_256);
                        }

                        oPaziente.RicaricaConsensoCalcolato();
                        this.pbConsensi.Image = DrawingProcs.GetImageFromByte(DBUtils.getIcona16ByTipoStato(EnumEntita.CNC, oPaziente.CodStatoConsensoCalcolato, ""));

                        this.pbConsegne.Visible = oSessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.ConsegneP_Visto);
                        this.pbConsegne.Image = Risorse.GetImageFromResource(Risorse.GC_CONSEGNEPAZIENTE_256);

                    }
                    else
                    {
                        this.lblPaziente.Text = "";
                        this.lblSessoEta.Text = "";
                        this.lblIndirizzo.Text = "";
                        this.lblCodiceFiscale.Text = "";

                        this.lblViewInfoPaziente.Visible = false;

                        this.pbPaziente.Image = null;
                        this.pbCartellaChiusa.Image = null;
                        this.pbPaziente.Visible = false;
                        this.pbAllergie.Visible = false;
                        this.pbConsensi.Image = null;
                        this.pbConsegne.Visible = false;
                        this.pbConsegne.Image = null;
                    }

                    if (oTrasferimento != null && oTrasferimento.Attivo == true)
                    {
                        if (oEpisodio != null && oEpisodio.NumeroEpisodio == string.Empty)
                        {
                            this.lblDatiRicovero.Text = string.Format("Prenotato in {0} il {1}", oTrasferimento.Descrizione, oTrasferimento.DataIngresso.ToShortDateString());
                        }
                        else
                        {
                            this.lblDatiRicovero.Text = string.Format("Ricoverato in {0} il {1}", oTrasferimento.Descrizione, oTrasferimento.DataIngresso.ToShortDateString());
                        }
                        this.lblNumCartella.Text = string.Format("N. Cartella: {0}", oTrasferimento.NumeroCartella);
                        this.lblLettoStanza.Text = (oTrasferimento.DescrLettoStanza.Trim() != "/" ? "Letto: " : "") + oTrasferimento.DescrLettoStanza;
                    }
                    else
                    {
                        if (oPaziente != null && oPaziente.CodUAAmbulatoriale != null && oPaziente.CodUAAmbulatoriale.Trim() != "")
                        {
                            this.lblDatiRicovero.Text = string.Format("Struttura di {0}", oPaziente.DescrUAAmbulatoriale);
                        }
                        else
                        {
                            this.lblDatiRicovero.Text = "";
                        }
                        this.lblNumCartella.Text = "";
                        this.lblLettoStanza.Text = "";
                    }


                    if (oEpisodio != null && oEpisodio.Attivo == true)
                    {
                        this.pbAlert.Visible = true;
                        this.lblViewInfoEpisodio.Visible = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Schede_Dettaglio_Episodio);
                        if (oEpisodio.AlertsGenerici.DaVistare > 0)
                            this.pbAlert.Image = Risorse.GetImageFromResource(Risorse.GC_ALERTGENERICO_256);
                        else
                            this.pbAlert.Image = Risorse.GetImageFromResource(Risorse.GC_ALERTGENERICO_DISABLED_256);
                        this.pbEvidenzaClinica.Visible = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.EvidenzaC_Menu);
                        if (oEpisodio.EvidenzeCliniche.DaVistare > 0)
                            this.pbEvidenzaClinica.Image = Risorse.GetImageFromResource(Risorse.GC_EVIDENZACLINICAALERT_256);
                        else
                            this.pbEvidenzaClinica.Image = Risorse.GetImageFromResource(Risorse.GC_EVIDENZACLINICAALERTDISABLE_256);
                        this.lblNosologico.Text = string.Format("Nos: {0}", (oEpisodio.NumeroEpisodio != string.Empty ? oEpisodio.NumeroEpisodio : oEpisodio.NumeroListaAttesa));
                    }
                    else
                    {
                        this.pbAlert.Visible = false;
                        this.pbEvidenzaClinica.Visible = false;
                        this.lblViewInfoEpisodio.Visible = false;
                        this.lblNosologico.Text = "";
                    }

                    if (oSessione.Utente.Ruoli.RuoloSelezionato != null)
                    {
                        if (oSessione.Utente.Ruoli.RuoloSelezionato.MatHome == false)
                        {
                            this.pbMatHome.Image = null;
                        }
                        else
                        {
                            this.pbMatHome.Image = Risorse.GetImageFromResource(Risorse.GC_MATHOME_256);
                        }

                        if (oSessione.Utente.Ruoli.RuoloSelezionato.Bookmarks == 0 || bCartellaChiusa == true)
                        {
                            this.pbSegnalibri.Image = null;
                        }
                        else
                        {
                            this.pbSegnalibri.Visible = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.SegnalibroVisualizza;
                            this.pbSegnalibri.Image = Risorse.GetImageFromResourceWithTip(Risorse.GC_SEGNALIBRI_256, oSessione.Utente.Ruoli.RuoloSelezionato.Bookmarks.ToString());
                        }


                        this.pbCartelleInVisione.Visible = (oCartella != null || oPaziente != null);

                        if (this.pbCartelleInVisione.Visible == true)
                        {
                            if (oCartella != null)
                            {
                                if (oSessione.Utente.Ruoli.RuoloSelezionato.CartelleInVisione == 0 && CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.CartellaIV_Inserisci))
                                {
                                    this.pbCartelleInVisione.Image = Risorse.GetImageFromResource(Risorse.GC_OCCHIOCHIUSO_256);
                                }
                                else if (oSessione.Utente.Ruoli.RuoloSelezionato.CartelleInVisione != 0 && CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.CartellaIV_Visualizza))
                                {
                                    this.pbCartelleInVisione.Image = Risorse.GetImageFromResourceWithTip(Risorse.GC_OCCHIOAPERTO_256, oSessione.Utente.Ruoli.RuoloSelezionato.CartelleInVisione.ToString());
                                }
                                else
                                {
                                    this.pbCartelleInVisione.Visible = false;
                                }
                            }
                            else
                            {
                                if (oSessione.Utente.Ruoli.RuoloSelezionato.PazientiInVisione == 0 && CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.CartellaIV_Inserisci))
                                {
                                    this.pbCartelleInVisione.Image = Risorse.GetImageFromResource(Risorse.GC_OCCHIOCHIUSO_256);
                                }
                                else if (oSessione.Utente.Ruoli.RuoloSelezionato.PazientiInVisione != 0 && CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.CartellaIV_Visualizza))
                                {
                                    this.pbCartelleInVisione.Image = Risorse.GetImageFromResourceWithTip(Risorse.GC_OCCHIOAPERTO_256, oSessione.Utente.Ruoli.RuoloSelezionato.PazientiInVisione.ToString());
                                }
                                else
                                {
                                    this.pbCartelleInVisione.Visible = false;
                                }
                            }
                        }

                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.PazientiSeguiti_Visualizza))
                        {

                            if (oPaziente == null)
                            {
                                if (oSessione.Utente.Ruoli.RuoloSelezionato.PazientiSeguiti == 0)
                                {
                                    this.pbPazientiSeguiti.Visible = false;
                                    this.pbPazientiSeguiti.Image = null;
                                }
                                else
                                {
                                    this.pbPazientiSeguiti.Image = Risorse.GetImageFromResourceWithTip(Risorse.GC_PREFERITIMIEI_256, oSessione.Utente.Ruoli.RuoloSelezionato.PazientiSeguiti.ToString());
                                    this.pbPazientiSeguiti.Visible = true;
                                }
                            }
                            else
                            {
                                if (oSessione.Utente.Ruoli.RuoloSelezionato.PazienteSeguito == 0 && oSessione.Utente.Ruoli.RuoloSelezionato.PazientiSeguitiDaAltri == 0)
                                {
                                    if (oSessione.Utente.Ruoli.RuoloSelezionato.PazientiSeguiti > 0)
                                    {
                                        this.pbPazientiSeguiti.Image = Risorse.GetImageFromResourceWithTip(Risorse.GC_PREFERITIAGGIUNGI_256, oSessione.Utente.Ruoli.RuoloSelezionato.PazientiSeguiti.ToString());
                                    }
                                    else
                                    {
                                        this.pbPazientiSeguiti.Image = Risorse.GetImageFromResource(Risorse.GC_PREFERITIAGGIUNGI_256);
                                    }
                                }
                                else if (oSessione.Utente.Ruoli.RuoloSelezionato.PazienteSeguito > 0 && oSessione.Utente.Ruoli.RuoloSelezionato.PazientiSeguitiDaAltri == 0)
                                {
                                    if (oSessione.Utente.Ruoli.RuoloSelezionato.PazientiSeguiti > 0)
                                    {
                                        this.pbPazientiSeguiti.Image = Risorse.GetImageFromResourceWithTip(Risorse.GC_PREFERITIMIEI_256, oSessione.Utente.Ruoli.RuoloSelezionato.PazientiSeguiti.ToString());
                                    }
                                    else
                                    {
                                        this.pbPazientiSeguiti.Image = Risorse.GetImageFromResource(Risorse.GC_PREFERITIMIEI_256);
                                    }
                                }
                                else if (oSessione.Utente.Ruoli.RuoloSelezionato.PazienteSeguito == 0 && oSessione.Utente.Ruoli.RuoloSelezionato.PazientiSeguitiDaAltri > 0)
                                {
                                    if (oSessione.Utente.Ruoli.RuoloSelezionato.PazientiSeguiti > 0)
                                    {
                                        this.pbPazientiSeguiti.Image = Risorse.GetImageFromResourceWithTip(Risorse.GC_PREFERITIALTRI_256, oSessione.Utente.Ruoli.RuoloSelezionato.PazientiSeguiti.ToString());
                                    }
                                    else
                                    {
                                        this.pbPazientiSeguiti.Image = Risorse.GetImageFromResource(Risorse.GC_PREFERITIALTRI_256);
                                    }
                                }
                                else if (oSessione.Utente.Ruoli.RuoloSelezionato.PazienteSeguito > 0 && oSessione.Utente.Ruoli.RuoloSelezionato.PazientiSeguitiDaAltri > 0)
                                {
                                    if (oSessione.Utente.Ruoli.RuoloSelezionato.PazientiSeguiti > 0)
                                    {
                                        this.pbPazientiSeguiti.Image = Risorse.GetImageFromResourceWithTip(Risorse.GC_PREFERITIMIEI_256, oSessione.Utente.Ruoli.RuoloSelezionato.PazientiSeguiti.ToString());
                                    }
                                    else
                                    {
                                        this.pbPazientiSeguiti.Image = Risorse.GetImageFromResource(Risorse.GC_PREFERITIMIEI_256);
                                    }
                                }
                                this.pbPazientiSeguiti.Visible = true;
                            }

                            if (bCartellaChiusa == true)
                                this.pbPazientiSeguiti.Visible = false;
                        }
                        else
                        {
                            this.pbPazientiSeguiti.Visible = false;
                            this.pbPazientiSeguiti.Image = null;
                        }

                        if (oSessione.Utente.Ruoli.RuoloSelezionato.NewsHard != 0 &&
oSessione.Utente.Ruoli.RuoloSelezionato.NewsHardInLettura == false)
                        {


                            oSessione.Utente.Ruoli.RuoloSelezionato.NewsHardInLettura = true;

                            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                            op.Parametro.Add("CodTipoNews", EnumTipoNews.HARD.ToString());
                            op.Parametro.Add("NonVisionate", "1");
                            op.Parametro.Add("DatiEstesi", "1");

                            SqlParameterExt[] spcoll = new SqlParameterExt[1];
                            string xmlParam = XmlProcs.XmlSerializeToString(op);
                            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                            DataTable dtnews = Database.GetDataTableStoredProc("MSP_SelMovNews", spcoll);

                            if (dtnews != null && dtnews.Rows.Count > 0)
                            {

                                foreach (DataRow oDr in dtnews.Rows)
                                {

                                    bool rilevante = false;
                                    string titolo = "";
                                    string testoRTF = "";
                                    string codnews = "";
                                    if (!oDr.IsNull("Rilevante")) rilevante = (bool)oDr["Rilevante"];
                                    if (!oDr.IsNull("Titolo")) titolo = oDr["Titolo"].ToString();
                                    if (!oDr.IsNull("TestoRTF")) testoRTF = oDr["TestoRTF"].ToString();
                                    if (!oDr.IsNull("Codice")) codnews = oDr["Codice"].ToString();

                                    CoreStatics.CoreApplication.News.NotiziaSelezionata = new Notizia((decimal)oDr["ID"], (DateTime)oDr["DataOra"], titolo, testoRTF, rilevante, true, codnews);
                                    CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.DettaglioNews);

                                }

                            }

                            oSessione.Utente.Ruoli.RuoloSelezionato.NewsHardInLettura = false;

                        }

                        if (oSessione.Utente.Ruoli.RuoloSelezionato.NewsLite != 0 &&
                            oSessione.Utente.Ruoli.RuoloSelezionato.NewsLiteInLettura == false)
                        {

                            oSessione.Utente.Ruoli.RuoloSelezionato.NewsLiteChange = false;
                            oSessione.Utente.Ruoli.RuoloSelezionato.NewsLiteInLettura = true;

                            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                            op.Parametro.Add("CodTipoNews", EnumTipoNews.LITE.ToString());
                            op.Parametro.Add("NonVisionate", "1");
                            op.Parametro.Add("DatiEstesi", "1");

                            SqlParameterExt[] spcoll = new SqlParameterExt[1];
                            string xmlParam = XmlProcs.XmlSerializeToString(op);
                            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                            DataTable dtnews = Database.GetDataTableStoredProc("MSP_SelMovNews", spcoll);

                            if (dtnews != null && dtnews.Rows.Count > 0)
                            {

                                foreach (DataRow oDr in dtnews.Rows)
                                {

                                    bool rilevante = false;
                                    string titolo = "";
                                    string testoRTF = "";
                                    string codnews = "";
                                    if (!oDr.IsNull("Rilevante")) rilevante = (bool)oDr["Rilevante"];
                                    if (!oDr.IsNull("Titolo")) titolo = oDr["Titolo"].ToString();
                                    if (!oDr.IsNull("TestoRTF")) testoRTF = oDr["TestoRTF"].ToString();
                                    if (!oDr.IsNull("Codice")) codnews = oDr["Codice"].ToString();

                                    CoreStatics.CoreApplication.News.NotiziaSelezionata = new Notizia((decimal)oDr["ID"], (DateTime)oDr["DataOra"], titolo, testoRTF, rilevante, true, codnews);
                                    CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.DettaglioNews);

                                }

                            }

                            oSessione.Utente.Ruoli.RuoloSelezionato.NewsLiteInLettura = false;

                        }

                    }
                    else
                    {
                        this.pbMatHome.Image = null;
                        this.pbSegnalibri.Image = null;
                        this.pbCartelleInVisione.Visible = false;
                        this.pbCartelleInVisione.Image = null;
                        this.pbPazientiSeguiti.Visible = false;
                        this.pbPazientiSeguiti.Image = null;
                        this.pbConsensi.Image = null;
                        this.pbConsegne.Image = null;
                    }

                }

                this.pbConnettivita.Visible = oSessione.Connettivita;

                this.lblOrario.Text = oSessione.Ora.ToLongTimeString();
                if (oNavigazione != null && oNavigazione.Maschere != null && oNavigazione.Maschere.MascheraSelezionata != null)
                {
                    this.lblTitolo.Text = oNavigazione.Maschere.MascheraSelezionata.Descrizione;
                }

                this.pbHelp.Image = Risorse.GetImageFromResource(Risorse.GC_HELP_256);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            base.Refresh();

        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (this.DesignMode == false)
            {
                this.pbVociDiarioClinico.Visible = false;
                this.pbVociDiarioClinico.Image = Risorse.GetImageFromResource(Risorse.GC_FIRMA_256);
                this.pbRefresh.Image = Risorse.GetImageFromResource(Risorse.GC_AGGIORNA_256);

                CoreStatics.MainWnd.RefreshControllo_Subscribers.Add(this);

                m_timerTime = new System.Timers.Timer();
                m_timerTime.SynchronizingObject = this;
                m_timerTime.Interval = 1000;
                m_timerTime.Elapsed += m_timerTime_Elapsed;
                m_timerTime.Enabled = true;

            }
            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (this.DesignMode == false)
            {
                this.pbVociDiarioClinico.Image = null;
                this.pbAllergie.Image = null;
                this.pbAlert.Image = null;
                this.pbEvidenzaClinica.Image = null;
                this.pbConnettivita.Image = null;
                this.pbSegnalibri.Image = null;
                this.pbCartelleInVisione.Image = null;
                this.pbPazientiSeguiti.Image = null;
                this.pbConsensi.Image = null;
                this.pbConsegne.Image = null;
                this.pbHelp.Image = null;

                CoreStatics.MainWnd.RefreshControllo_Subscribers.Remove(this);

                if (m_timerTime != null)
                {
                    m_timerTime.Elapsed -= m_timerTime_Elapsed;
                    m_timerTime.Enabled = false;
                    m_timerTime = null;
                }

            }
            base.OnHandleDestroyed(e);
        }

        #endregion

        #region Events

        private void pbUtente_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.Utente)); }
        }

        private void pbVociDiarioClinico_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.VociDiarioClinico)); }
        }

        private void pbPaziente_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.Paziente)); }
        }

        private void lblPaziente_Click(object sender, EventArgs e)
        {
            if (this.lblPaziente.Text != string.Empty)
            {

                var sb = new StringBuilder();
                sb.Append(this.lblPaziente.Text + " " + this.lblSessoEta.Text);
                sb.AppendLine();
                sb.Append(this.lblIndirizzo.Text);
                sb.AppendLine();
                sb.Append(this.lblCodiceFiscale.Text);
                sb.AppendLine();
                if (CoreStatics.CoreApplication.Paziente != null)
                {
                    if (CoreStatics.CoreApplication.Paziente.IndirizzoResidenza != string.Empty || CoreStatics.CoreApplication.Paziente.ComuneResidenza != string.Empty ||
                        CoreStatics.CoreApplication.Paziente.CAPResidenza != string.Empty || CoreStatics.CoreApplication.Paziente.ProvinciaResidenza != string.Empty)
                    {
                        sb.AppendLine();
                        sb.Append(@"Residente in ");

                        if (CoreStatics.CoreApplication.Paziente.IndirizzoResidenza != string.Empty)
                        {
                            sb.Append(String.Format(@"{0}", CoreStatics.CoreApplication.Paziente.IndirizzoResidenza));
                        }

                        if (CoreStatics.CoreApplication.Paziente.ComuneResidenza != string.Empty ||
                            CoreStatics.CoreApplication.Paziente.CAPResidenza != string.Empty)
                        {
                            sb.Append(String.Format(@" {0} {1}", CoreStatics.CoreApplication.Paziente.CAPResidenza,
                                                                    CoreStatics.CoreApplication.Paziente.ComuneResidenza));
                        }

                        if (CoreStatics.CoreApplication.Paziente.ProvinciaResidenza != string.Empty)
                        {
                            sb.Append(String.Format(@" ({0})", CoreStatics.CoreApplication.Paziente.ProvinciaResidenza));
                        }

                        sb.AppendLine();
                    }
                }
                sb.AppendLine();
                sb.Append(this.lblDatiRicovero.Text);
                sb.AppendLine();
                sb.Append(this.lblNumCartella.Text);
                sb.AppendLine();
                sb.Append(this.lblLettoStanza.Text);
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(this.lblNosologico.Text);

                _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(sb.ToString(), false);
                this.UltraPopupControlContainer.Show((ucEasyLabel)sender);

            }
        }

        private void pbConsensi_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.Consensi)); }
        }

        private void pbAllergie_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.Allergie)); }
        }

        private void pbAlert_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.Alert)); }
        }

        private void pbEvidenzaClinica_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.EvidenzaClinica)); }
        }

        private void pbConnettivita_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.Connettivita)); }
        }

        private void pbSegnalibri_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.Segnalibri)); }
        }

        private void pbCartelleInVisione_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.CartelleInVisione)); }
        }

        private void lblViewInfoPaziente_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.InfoPaziente)); }
        }

        private void lblViewInfoEpisodio_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.InfoEpisodio)); }
        }

        private void pbRefresh_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.Refresh)); }
        }

        private void pbPazientiSeguiti_Click(object sender, EventArgs e)
        {
            if (this.pbPazientiSeguiti.Visible == true)
            {
                if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.PazientiSeguiti)); }
            }
        }

        private void pbConsegne_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.Consegne)); }
        }

        private void lblCodMaschera_Click(object sender, EventArgs e)
        {

            try
            {

                if (CoreStatics.CoreApplication.Sessione.Utente.Admin == true)
                {

                    this.Cursor = Cursors.WaitCursor;

                    StringBuilder sb = CoreStatics.getPlaceHolder();
                    if (sb.Length > 0)
                    {
                        _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(sb.ToString(), false);
                        _ucRichTextBox.Size = new Size(800, 600);
                        this.UltraPopupControlContainer.Show((ucEasyLabel)sender);
                    }

                    this.Cursor = Cursors.Default;

                }

            }
            catch (Exception)
            {

            }

        }

        private void pbHelp_Click(object sender, EventArgs e)
        {
            if (ImmagineClick != null) { ImmagineClick(sender, new ImmagineTopClickEventArgs(EnumImmagineTop.Help)); }
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
            _ucRichTextBox.Focus();
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

        #region     I_RefreshTimer_Controllo

        public SynchronizationContext SyncContext
        {
            get
            {
                return m_sync;
            }
        }

        public void RefreshData(object state)
        {
            try
            {
                this.Refresh();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        #endregion I_RefreshTimer_Controllo

    }
}
