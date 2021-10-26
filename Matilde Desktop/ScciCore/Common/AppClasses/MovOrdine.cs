using System;
using System.Collections.Generic;
using System.Linq;

using System.Data;
using UnicodeSrl.Framework.Data;

using System.Xml.Linq;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.ScciCore.WebSvc;

using System.ComponentModel;
using UnicodeSrl.ScciCore.SvcOrderEntry;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.Common.Enums;
using UnicodeSrl.ScciCore.Common.Extensions;
using UnicodeSrl.DatiClinici.Interfaces;
using UnicodeSrl.Framework.Diagnostics;

namespace UnicodeSrl.ScciCore
{
    [Serializable()]
    public class MovOrdine
    {

        #region Declare

        public enum EnumMovOrdineTipoFiltro
        {
            [Description("Tutti")]
            Tutti = 0,
            [Description("Recenti")]
            Recenti = 1,
            [Description("Profili")]
            Profili = 2,
            [Description("ProfiliUtente")]
            ProfiliUtente = 3,
            [Description("Erogante")]
            Erogante = 4,
        }

        private string _utenteinoltro = string.Empty;
        private DateTime _datainoltro = DateTime.MinValue;
        private List<KeyValuePair<string, string>> _listaformule = null;

        public const string K_OECPY_NOBTN = @"OECPY_NOBTN";
        public const string K_OECPY_NOROWBTN = @"OECPY_NOROWBTN";

        private string[] C_SEPVALORI = { "§;" }; private string[] C_SEPVALORISING = { "#;" };
        private string C_SEPDECOD = @"§"; private string C_SEPDECONTROLSDC = @"|";
        private string C_SEP_CODICE_SPEC_US = @"0010"; private string C_SEP_CODICE_SPEC_P = @"0100"; private string C_SEP_CODICE_SPEC_OT = @"1000";
        public string C_BUTTON_PREFIX = "Mybtn"; public string C_BUTTON_PREFIX_SRC = "cmdSearch"; public string C_BUTTON_PREFIX_2 = "My2btn";
        #endregion

        #region Constructor

        public MovOrdine(Scci.DataContracts.ScciAmbiente ambiente, string idepisodio,
                            string codtipoepisodio, string numeroepisodio, string numerolista, string codazienda,
                            string idtrasferimento, string coduo, string codua, Paziente paziente)
        {


            NumeroOrdine = string.Empty;
            IDOrdine = string.Empty;
            this.Ambiente = ambiente;
            DataProgrammazione = DateTime.MinValue;
            Prestazioni = new List<OEOrdinePrestazione>();
            DatiAccessoriRichiedente = new List<OEDatoAccessorio>();
            this.Priorita = OEPrioritaOrdine.NN;
            this.StatoOrdine = OEStato.NN;
            this.StatoValiditaOrdine = OEValiditaOrdine.NN;

            this.Azione = EnumAzioni.INS;

            this.Paziente = paziente;
            this.IDEpisodio = idepisodio;
            this.CodTipoEpisodio = codtipoepisodio;
            this.NumeroEpisodio = numeroepisodio;
            this.NumeroListaAttesa = numerolista;
            this.CodAzienda = codazienda;
            this.IDTrasferimento = idtrasferimento;
            this.CodUO = coduo;
            this.CodUA = codua;

            this.CodSistema = string.Empty;
            this.IDSistema = string.Empty;
            this.IDGruppo = string.Empty;
            this.InfoSistema = string.Empty;
            this.InfoSistema2 = string.Empty;

            this.SkipDatiAggiuntivi = false;

        }

        public MovOrdine(string numeroordine,
                            Scci.DataContracts.ScciAmbiente ambiente, string idepisodio,
                            string codtipoepisodio, string numeroepisodio, string numerolista, string codazienda,
                            string idtrasferimento, string coduo, string codua, Paziente paziente)
        {


            this.NumeroOrdine = numeroordine;
            this.Ambiente = ambiente;

            this.Azione = EnumAzioni.MOD;

            this.Paziente = paziente;
            this.IDEpisodio = idepisodio;
            this.CodTipoEpisodio = codtipoepisodio;
            this.NumeroEpisodio = numeroepisodio;
            this.NumeroListaAttesa = numerolista;
            this.CodAzienda = codazienda;
            this.IDTrasferimento = idtrasferimento;
            this.CodUO = coduo;
            this.CodUA = codua;

            this.CodSistema = string.Empty;
            this.IDSistema = string.Empty;
            this.IDGruppo = string.Empty;
            this.InfoSistema = string.Empty;
            this.InfoSistema2 = string.Empty;

            this.UltimaEccezioneGenerata = null;
            this.CaricaDettaglio();

            this.SkipDatiAggiuntivi = false;

        }

        #endregion

        #region Private Property

        private string AmbienteCodLogin
        {
            get
            {
                string sReturn = this.Ambiente.Codlogin;

                return sReturn;
            }
        }

        private bool InseritoInLocale
        {
            get { return (this.IDOrdineSCCIDaNumeroOrdineOE(this.NumeroOrdine) != string.Empty); }
        }

        #endregion

        #region Public Property

        public ScciAmbiente Ambiente { get; set; }

        public string IDOrdine { get; set; }

        public string NumeroOrdine { get; set; }

        public OEPrioritaOrdine Priorita { get; set; }

        public DateTime DataProgrammazione { get; set; }

        public List<OEOrdinePrestazione> Prestazioni { get; set; }

        public EnumAzioni Azione { get; set; }

        public Paziente Paziente { get; set; }

        public string IDTrasferimento { get; set; }

        public string CodUO { get; set; }

        public string CodUA { get; set; }

        public string IDEpisodio { get; set; }

        public string NumeroEpisodio { get; set; }

        public string NumeroListaAttesa { get; set; }

        public string CodTipoEpisodio { get; set; }

        public string CodAzienda { get; set; }

        public OEStato StatoOrdine { get; set; }

        public OEValiditaOrdine StatoValiditaOrdine { get; set; }

        public string Eroganti { get; set; }

        public string UtenteInoltro { get { return _utenteinoltro; } }

        public DateTime DataInoltro { get { return _datainoltro; } }

        public List<OEDatoAccessorio> DatiAccessoriRichiedente { get; set; }

        public string ListaPrestazioni
        {
            get
            {
                string sret = string.Empty;

                if (this.Prestazioni.Count > 0)
                {
                    foreach (OEOrdinePrestazione prestazione in this.Prestazioni)
                        sret += prestazione.Prestazione.Descrizione + " (" + prestazione.Prestazione.Codice + ")" + ", ";

                    if (sret != string.Empty) sret = sret.Remove(sret.Length - 2, 2);
                }

                return sret;

            }
        }

        public bool Inoltrabile
        {
            get { return (this.StatoValiditaOrdine == OEValiditaOrdine.Valido && this.Prestazioni.Count > 0); }
        }

        public List<OESistemaErogante> ListaEroganti
        {
            get
            {
                List<OESistemaErogante> listaeroganti = new List<OESistemaErogante>();

                foreach (OEOrdinePrestazione prest in this.Prestazioni)
                {

                    if (!listaeroganti.Any<OESistemaErogante>(erogante => erogante.Codice == prest.Prestazione.Erogante.Codice &&
                                                      erogante.CodiceAzienda == prest.Prestazione.Erogante.CodiceAzienda))
                        listaeroganti.Add(prest.Prestazione.Erogante);

                }

                return listaeroganti;
            }
        }

        public Exception UltimaEccezioneGenerata { get; set; }

        public string CodSistema { get; set; }

        public string IDSistema { get; set; }

        public string IDGruppo { get; set; }

        public string InfoSistema { get; set; }

        public string InfoSistema2 { get; set; }

        public bool SkipDatiAggiuntivi { get; set; }

        public bool Cancellabile
        {
            get
            {

                bool bret = false;

                SvcOrderEntry.ScciOrderEntryClient oeclient = null;
                Scci.DataContracts.OEOrdineTestata ordine = null;

                try
                {

                    oeclient = ScciSvcRef.GetSvcOrderEntry();

                    ordine = oeclient.GetOrdineTestata(CoreStatics.CoreApplication.Ambiente.Codlogin, this.NumeroOrdine);

                    bret = ordine.Cancellabile;

                }
                catch (Exception)
                {

                }
                finally
                {
                    ordine = null;
                    oeclient = null;
                }

                return bret;

            }
        }

        public List<KeyValuePair<string, string>> ListaOEFormule
        {
            get
            {
                if (_listaformule == null)
                {

                    _listaformule = new List<KeyValuePair<string, string>>();

                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("CodUA", this.CodUA);

                    List<string> lst_prestazioni = new List<string>();
                    foreach (OEOrdinePrestazione prest in this.Prestazioni)
                    {
                        lst_prestazioni.Add($"\n<codazienda>{prest.Prestazione.Erogante.CodiceAzienda}</codazienda>" +
                                            $"\n<coderogante>{prest.Prestazione.Erogante.Codice}</coderogante>" +
                                            $"\n<codprestazione>{prest.Prestazione.Codice}</codprestazione>\n");
                    }
                    op.ParametroRipetibile.Add("Prestazione", lst_prestazioni.ToArray());

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = xmlParam.Replace(@"&lt;", "<");
                    xmlParam = xmlParam.Replace(@"&gt;", ">");
                    FwDataParametersList procParams = new FwDataParametersList();
                    procParams.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);

                    using (FwDataConnection fdc = new FwDataConnection(Database.ConnectionString))
                    {

                        DataTable oDt = fdc.Query<DataTable>("MSP_SelOEFormule", procParams, CommandType.StoredProcedure);

                        foreach (DataRow dr in oDt.Rows)
                        {
                            _listaformule.Add(new KeyValuePair<string, string>(dr[0].ToString(), dr[1].ToString()));
                        }

                    }

                }
                return _listaformule;
            }
        }

        #endregion

        #region Public Methods

        public bool CreaOrdine()
        {
            this.UltimaEccezioneGenerata = null;
            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();
            bool bret = false;

            try
            {




                OEOrdineDettaglio ordine = oeclient.CreaOrdineEsteso(this.AmbienteCodLogin, CoreStatics.CoreApplication.Sessione.Utente.Nome, CoreStatics.CoreApplication.Sessione.Utente.Cognome, this.GetTimestamp(),
                                                                    this.NumeroEpisodio, this.NumeroListaAttesa, this.CodTipoEpisodio, this.Priorita, this.Paziente.CodiceFiscale, this.Paziente.CodSACFuso,
                                                                    this.Paziente.Cognome, this.Paziente.Nome, this.Paziente.DataNascita, this.CodAzienda, this.CodUO, this.Ambiente.Nomepc);

                if (ordine != null && ordine.OrdineTestata != null && ordine.OrdineTestata.Stato != OEStato.Errato)
                {
                    this.NumeroOrdine = ordine.OrdineTestata.NumeroOrdine;

                    this.CaricaDettaglio(ordine);
                    this.Azione = EnumAzioni.MOD;
                    bret = true;
                }
                else
                    bret = false;
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                bret = false;
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }

            return bret;

        }

        public bool SalvaOrdine()
        {
            this.UltimaEccezioneGenerata = null;

            bool bret;
            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            try
            {
                OEOrdineDettaglio ordinedetail = oeclient.SalvaOrdineEsteso(this.AmbienteCodLogin, CoreStatics.CoreApplication.Sessione.Utente.Nome, CoreStatics.CoreApplication.Sessione.Utente.Cognome, this.NumeroOrdine,
                                                                            this.NumeroEpisodio, this.NumeroListaAttesa, this.CodTipoEpisodio, this.Priorita, this.DataProgrammazione,
                                                                            this.CodAzienda, this.CodUO, this.Ambiente.Nomepc);

                if (ordinedetail != null)
                {
                    this.CaricaDettaglio(ordinedetail);

                    this.SalvataggioOrdineLocale(false);

                    bret = true;
                }
                else
                {
                    bret = false;
                }
            }
            catch (Exception ex)
            {
                this.UltimaEccezioneGenerata = ex;
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                bret = false;
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }

            return bret;

        }

        public void CaricaDettaglio()
        {
            this.UltimaEccezioneGenerata = null;

            OEOrdineDettaglio ordine = null;
            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            try
            {
                ordine = oeclient.GetOrdineDettaglio(this.AmbienteCodLogin, this.NumeroOrdine);

                if (ordine != null)
                {
                    CaricaDettaglio(ordine);
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                Prestazioni = new List<OEOrdinePrestazione>();
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }
        }

        public void CaricaDettaglio(OEOrdineDettaglio ordine)
        {
            this.UltimaEccezioneGenerata = null;

            try
            {

                if (ordine != null)
                {
                    this.Prestazioni = ordine.Prestazioni;
                    this.DatiAccessoriRichiedente = new List<OEDatoAccessorio>();

                    this.StatoOrdine = ordine.OrdineTestata.Stato;
                    this.StatoValiditaOrdine = ordine.OrdineTestata.StatoValidazione;

                    this.Eroganti = ordine.OrdineTestata.Eroganti;

                    this._utenteinoltro = ordine.OrdineTestata.Operatore; this._datainoltro = ordine.OrdineTestata.DataModifica;
                    if (ordine.OrdineTestata != null)
                    {
                        this.DataProgrammazione = ordine.OrdineTestata.DataOraProgrammata;
                        this.Priorita = ordine.OrdineTestata.Priorita;
                        this.IDOrdine = ordine.OrdineTestata.IdOrdine;
                    }
                    else
                    {
                        this.DataProgrammazione = DateTime.MinValue;
                        this.Priorita = OEPrioritaOrdine.NN;
                        this.IDOrdine = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                Prestazioni = new List<OEOrdinePrestazione>();
            }
        }

        public void AggiornaElencoPrestazioni()
        {
            this.UltimaEccezioneGenerata = null;
            OEOrdineDettaglio ordine = null;
            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            try
            {
                ordine = oeclient.GetOrdineDettaglio(this.AmbienteCodLogin, this.NumeroOrdine);

                if (ordine != null)
                {
                    this.Prestazioni = ordine.Prestazioni;
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                Prestazioni = new List<OEOrdinePrestazione>();
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }
        }

        public bool InserisciPrestazione(OEPrestazione prestazione)
        {
            this.UltimaEccezioneGenerata = null;
            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            try
            {
                return oeclient.InserisciPrestazione(this.AmbienteCodLogin, this.NumeroOrdine, prestazione);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                return false;
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }
        }

        public bool InserisciPrestazioni(List<OEPrestazione> prestazioni)
        {
            this.UltimaEccezioneGenerata = null;
            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            try
            {
                return oeclient.InserisciPrestazioni(this.AmbienteCodLogin, this.NumeroOrdine, prestazioni.ToArray());
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                return false;
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }
        }

        public bool CancellaPrestazioni(List<OEPrestazione> prestazioni)
        {
            this.UltimaEccezioneGenerata = null;
            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            try
            {
                return oeclient.CancellaPrestazioni(this.AmbienteCodLogin, this.NumeroOrdine, prestazioni.ToArray());
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                return false;
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }
        }

        public bool CancellaPrestazione(OEPrestazione prestazione)
        {
            this.UltimaEccezioneGenerata = null;

            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            try
            {
                return oeclient.CancellaPrestazione(this.AmbienteCodLogin, this.NumeroOrdine, prestazione);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                return false;
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }
        }

        public bool ScomponiProfilo(OEPrestazione profilo)
        {
            this.UltimaEccezioneGenerata = null;

            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            try
            {
                return oeclient.EsplodiProfilo(this.AmbienteCodLogin, this.NumeroOrdine, profilo);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                return false;
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }
        }

        public DataTable CaricaPrestazioniDaSelezionare(EnumMovOrdineTipoFiltro Tipo, string CodAziendaSistemaErogante, string CodSistemaErogante, string FiltroTesto)
        {
            this.UltimaEccezioneGenerata = null;
            DataTable odt = this.CreaDataTablePerGriglie();
            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            try
            {

                switch (Tipo)
                {
                    case EnumMovOrdineTipoFiltro.Tutti:
                        this.RiempiTabellaPrestazioniPerGriglie(ref odt,
        oeclient.GetPrestazioni(this.AmbienteCodLogin, this.CodTipoEpisodio,
                                     this.Priorita,
                                     this.CodAzienda, this.CodUO,
                                     "", "",
                                     FiltroTesto));

                        break;

                    case EnumMovOrdineTipoFiltro.Recenti:
                        this.RiempiTabellaPrestazioniPerGriglie(ref odt,
                                oeclient.PrestazioniRecenti(this.AmbienteCodLogin, this.CodTipoEpisodio,
                                                                    this.Priorita,
                                                                    this.CodAzienda, this.CodUO,
                                                                    "ASMN", "SCCI",
                                                                    "", "",
                                                                    this.Paziente.CodSAC, FiltroTesto));

                        break;

                    case EnumMovOrdineTipoFiltro.Profili:
                        this.RiempiTabellaPrestazioniPerGriglie(ref odt,
        oeclient.GetProfili(this.AmbienteCodLogin, this.CodTipoEpisodio,
                                    this.Priorita,
                                    this.CodAzienda, this.CodUO,
                                    "", "",
                                    FiltroTesto));

                        break;

                    case EnumMovOrdineTipoFiltro.ProfiliUtente:
                        this.RiempiTabellaPrestazioniPerGriglie(ref odt,
        oeclient.GetProfiliUtente(this.AmbienteCodLogin, this.CodTipoEpisodio,
                                        this.Priorita,
                                        this.CodAzienda, this.CodUO,
                                        "", "",
                                        FiltroTesto));

                        break;

                    case EnumMovOrdineTipoFiltro.Erogante:
                        this.RiempiTabellaPrestazioniPerGriglie(ref odt,
        oeclient.GetPrestazioni(this.AmbienteCodLogin, this.CodTipoEpisodio,
                                        this.Priorita,
                                        this.CodAzienda, this.CodUO,
                                        CodAziendaSistemaErogante, CodSistemaErogante,
                                        FiltroTesto));

                        break;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                odt = this.CreaDataTablePerGriglie();
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }

            return odt;

        }
        public DataTable CaricaPrestazioniDaSelezionare(string codiceregime, OEPrioritaOrdine priorita,
                                                        string codiceAzienda, string codiceUnita,
                                                        string IDGruppo, string FiltroPrestazioni)
        {
            this.UltimaEccezioneGenerata = null;
            DataTable odt = this.CreaDataTablePerGriglie();
            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            try
            {
                this.RiempiTabellaPrestazioniPerGriglie(ref odt, oeclient.GetPrestazioniInGruppo2(this.AmbienteCodLogin, codiceregime,
                                                                                  priorita, codiceAzienda, codiceUnita,
                                                                                  "ASMN", "SCCI",
                                                                                  IDGruppo, FiltroPrestazioni));
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                odt = this.CreaDataTablePerGriglie();
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }

            return odt;
        }

        public DataTable CaricaPrestazioniSelezionate()
        {
            return CaricaPrestazioniSelezionate(false);
        }
        public DataTable CaricaPrestazioniSelezionate(bool aggiungiDataPianificata)
        {
            this.UltimaEccezioneGenerata = null;

            DataTable odt = this.CreaDataTablePerGriglie(aggiungiDataPianificata);

            try
            {
                this.RiempiTabellaPrestazioniPerGriglie(ref odt, this.Prestazioni);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                odt = this.CreaDataTablePerGriglie();
            }

            return odt;

        }

        public DataTable CaricaAnteprimaEsplosioneProfilo(string CodProfilo, string CodErogante, string CodAziErogante, OEPrestazioneTipo TipoProfilo)
        {
            this.UltimaEccezioneGenerata = null;
            DataRow orow = null;
            DataTable odt = new DataTable();


            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            try
            {

                odt.Columns.Add("Erogante", typeof(string));
                odt.Columns.Add("Prestazione", typeof(string));

                Scci.DataContracts.OEPrestazione profilo = new OEPrestazione();
                profilo.Tipo = TipoProfilo;
                profilo.Erogante.CodiceAzienda = CodAziErogante;
                profilo.Erogante.Codice = CodErogante;
                profilo.Codice = CodProfilo;

                foreach (Scci.DataContracts.OEPrestazione prestazione in oeclient.GetPrestazioniInProfilo(this.AmbienteCodLogin, profilo))
                {
                    orow = odt.NewRow();
                    orow["Erogante"] = prestazione.Erogante.Descrizione;
                    orow["Prestazione"] = prestazione.Descrizione;
                    odt.Rows.Add(orow);
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                odt = new DataTable();
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }

            return odt;
        }

        public DataTable CaricaAnteprimaDatiAggiuntivi()
        {
            this.UltimaEccezioneGenerata = null;
            DataRow orow = null;
            DataTable odt = new DataTable();

            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            try
            {

                odt.Columns.Add("Descrizione", typeof(string));

                foreach (Scci.DataContracts.OEDatiAccessoriDescrittore datoaccessorio in oeclient.GetDatiAccessorNecessari(this.AmbienteCodLogin, this.NumeroOrdine))
                {
                    orow = odt.NewRow();
                    orow["Descrizione"] = datoaccessorio.Descrizione;
                    odt.Rows.Add(orow);
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                odt = new DataTable();
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }

            return odt;
        }

        public DataTable CaricaDatiAggiuntivi()
        {
            this.UltimaEccezioneGenerata = null;
            DataRow orow = null;
            DataTable odt = new DataTable();
            DataRow[] orowfilter = null;


            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            try
            {
                odt.Columns.Add("Codice", typeof(string));
                odt.Columns.Add("Descrizione", typeof(string));
                odt.Columns.Add("Etichetta", typeof(string));
                odt.Columns.Add("Gruppo", typeof(string));
                odt.Columns.Add("Obbligatorio", typeof(bool));
                odt.Columns.Add("Ordinamento", typeof(int));
                odt.Columns.Add("Ripetibile", typeof(bool));
                odt.Columns.Add("Tipo", typeof(EnumTipoDatoAggiuntivo));
                odt.Columns.Add("Valori", typeof(string));
                odt.Columns.Add("ValidazioneMessaggio", typeof(string));
                odt.Columns.Add("ValidazioneRegEx", typeof(string));
                odt.Columns.Add("Testata", typeof(bool));
                odt.Columns.Add("Valore", typeof(string));
                odt.Columns.Add("DescrizioneValore", typeof(string));
                odt.Columns.Add("Ripetizione", typeof(int));

                OEDatiAccessoriDescrittore[] arrayDatiAccN = oeclient.GetDatiAccessorNecessari(this.AmbienteCodLogin, this.NumeroOrdine);

                foreach (Scci.DataContracts.OEDatiAccessoriDescrittore datoaccessoriodescrittore in arrayDatiAccN)
                {
                    orow = odt.NewRow();
                    orow["Codice"] = datoaccessoriodescrittore.Codice;
                    orow["Descrizione"] = datoaccessoriodescrittore.Descrizione;
                    orow["Etichetta"] = datoaccessoriodescrittore.Etichetta;
                    orow["Gruppo"] = datoaccessoriodescrittore.Gruppo;
                    orow["Obbligatorio"] = datoaccessoriodescrittore.Obbligatorio;
                    orow["Ordinamento"] = datoaccessoriodescrittore.Ordinamento;
                    orow["Ripetibile"] = datoaccessoriodescrittore.Ripetibile;
                    orow["Tipo"] = ConversioneTipoDatoAggiuntivo(datoaccessoriodescrittore.Tipo);
                    orow["Valori"] = datoaccessoriodescrittore.Valori;
                    orow["ValidazioneMessaggio"] = datoaccessoriodescrittore.ValidazioneMessaggio;
                    orow["ValidazioneRegEx"] = datoaccessoriodescrittore.ValidazioneRegEx;
                    orow["Testata"] = datoaccessoriodescrittore.Testata;
                    orow["Ripetizione"] = 0;
                    odt.Rows.Add(orow);
                }


                EnumTipoDatoAggiuntivo tipodato = EnumTipoDatoAggiuntivo.Undefined;

                OEDatoAccessorio[] arrayDatiAcc = oeclient.GetDatiAccessori(this.AmbienteCodLogin, this.NumeroOrdine);

                foreach (Scci.DataContracts.OEDatoAccessorio datoaccessorio in arrayDatiAcc)
                {

                    orowfilter = odt.Select("Codice = '" + datoaccessorio.Codice + "'");

                    if (orowfilter != null && orowfilter.Count() > 0)
                    {
                        tipodato = (EnumTipoDatoAggiuntivo)Enum.Parse(typeof(EnumTipoDatoAggiuntivo), orowfilter[0]["Tipo"].ToString());
                        DateTime valoredatodatetime = DateTime.MinValue;
                        switch (tipodato)
                        {
                            case EnumTipoDatoAggiuntivo.DateBox:
                                DateTime.TryParse(datoaccessorio.Valore, out valoredatodatetime);
                                if (valoredatodatetime != DateTime.MinValue)
                                    orowfilter[0]["Valore"] = valoredatodatetime.ToString("dd/MM/yyyy");
                                else
                                    orowfilter[0]["Valore"] = string.Empty;
                                break;

                            case EnumTipoDatoAggiuntivo.DateTimeBox:
                                DateTime.TryParse(datoaccessorio.Valore, out valoredatodatetime);
                                if (valoredatodatetime != DateTime.MinValue)
                                    orowfilter[0]["Valore"] = valoredatodatetime.ToString("dd/MM/yyyy HH:mm");
                                else
                                    orowfilter[0]["Valore"] = string.Empty;
                                break;

                            case EnumTipoDatoAggiuntivo.TimeBox:
                                DateTime.TryParse(datoaccessorio.Valore, out valoredatodatetime);
                                if (valoredatodatetime != DateTime.MinValue)
                                    orowfilter[0]["Valore"] = valoredatodatetime.ToString("HH:mm");
                                else
                                    orowfilter[0]["Valore"] = string.Empty;
                                break;

                            default:
                                orowfilter[0]["Valore"] = datoaccessorio.Valore;
                                break;
                        }
                        orowfilter[0]["DescrizioneValore"] = this.CaricaDescrizioneDatoAggiuntivo
                                                                        (
                                                                            tipodato,
                                                                            orowfilter[0]["Valore"].ToString(),
                                                                            orowfilter[0]["Valori"].ToString()
                                                                        );

                        orowfilter[0]["Ripetizione"] = datoaccessorio.Ripetizione;
                    }
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                odt = new DataTable();
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }

            return odt;
        }

        public DataTable CaricaDatiAggiuntiviStato()
        {
            this.UltimaEccezioneGenerata = null;
            DataTable odt = new DataTable();

            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            try
            {
                odt.Columns.Add("Etichetta", typeof(string));
                odt.Columns.Add("DescrizioneValore", typeof(string));

                foreach (Scci.DataContracts.OEDatoAccessorio datoaccessorio in oeclient.GetDatiAccessori(this.AmbienteCodLogin, this.NumeroOrdine))
                {
                    DataRow o_selrow;

                    o_selrow = odt.NewRow();
                    odt.Rows.Add(o_selrow);

                    o_selrow["Etichetta"] = datoaccessorio.Codice;
                    o_selrow["DescrizioneValore"] = datoaccessorio.Valore;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                odt = new DataTable();
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }

            return odt;
        }

        public DataTable CaricaDatiAggiuntiviErogante()
        {
            this.UltimaEccezioneGenerata = null;
            DataRow orow = null;
            DataTable odt = new DataTable();


            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            try
            {
                odt.Columns.Add("Codice", typeof(string));
                odt.Columns.Add("Valore", typeof(string));
                odt.Columns.Add("Ripetizione", typeof(int));
                odt.Columns.Add("Tipo", typeof(string));
                odt.Columns.Add("PDF", typeof(string));

                foreach (OESistemaErogante erogante in this.ListaEroganti)
                {
                    foreach (Scci.DataContracts.OEDatoAccessorio datoaccessorio in oeclient.GetDatiAccessoriErogante2(this.AmbienteCodLogin, this.NumeroOrdine))
                    {
                        orow = odt.NewRow();
                        orow["Codice"] = datoaccessorio.Codice;
                        switch (datoaccessorio.Tipo)
                        {
                            case "xs:string":
                                orow["Valore"] = datoaccessorio.Valore;
                                orow["PDF"] = string.Empty;
                                break;
                            case "xs:base64Binary":
                                orow["Valore"] = "Documento PDF";
                                orow["PDF"] = datoaccessorio.Valore;
                                break;
                            default:
                                break;
                        }
                        orow["Tipo"] = datoaccessorio.Tipo;
                        orow["Ripetizione"] = datoaccessorio.Ripetizione;
                        odt.Rows.Add(orow);
                    }
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                odt = new DataTable();
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }

            return odt;
        }

        public List<OESistemaErogante> CaricaElencoErogantiOrdine()
        {
            this.UltimaEccezioneGenerata = null;

            List<OESistemaErogante> oRet = new List<OESistemaErogante>();

            try
            {
                foreach (OEOrdinePrestazione prestazione in this.Prestazioni)
                {
                    if (!oRet.Contains(prestazione.Prestazione.Erogante))
                        oRet.Add(prestazione.Prestazione.Erogante);
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                oRet = new List<OESistemaErogante>();
            }

            return oRet;

        }

        public List<OESistemaErogante> CaricaElencoCompletoEroganti()
        {
            this.UltimaEccezioneGenerata = null;
            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            List<OESistemaErogante> oRet = new List<OESistemaErogante>();

            try
            {
                foreach (OESistemaErogante erogante in oeclient.GetEroganti(this.AmbienteCodLogin))
                {
                    oRet.Add(erogante);
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                oRet = new List<OESistemaErogante>();
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }

            return oRet;

        }

        public List<OESistemaErogante> CaricaElencoCompletoEroganti2()
        {
            this.UltimaEccezioneGenerata = null;
            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            List<OESistemaErogante> oRet = new List<OESistemaErogante>();

            try
            {
                foreach (OESistemaErogante erogante in oeclient.GetEroganti2(this.AmbienteCodLogin))
                {
                    oRet.Add(erogante);
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                oRet = new List<OESistemaErogante>();
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }

            return oRet;

        }

        public List<OEGruppoPrestazione> CaricaElencoGruppi()
        {
            this.UltimaEccezioneGenerata = null;
            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            OEGruppoPrestazione[] list = null;
            List<OEGruppoPrestazione> oRet = new List<OEGruppoPrestazione>();

            try
            {
                list = oeclient.GetGruppiPreferenziali(this.AmbienteCodLogin, this.CodTipoEpisodio, this.Priorita, this.CodAzienda, this.CodUO, "ASMN", "SCCI", null);

                if (list != null)
                {
                    foreach (OEGruppoPrestazione gruppo in list)
                    {
                        oRet.Add(gruppo);
                    }
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                oRet = new List<OEGruppoPrestazione>();
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
                list = null;
            }

            return oRet;
        }

        public bool SalvaProfiloUtente(string NomeProfilo)
        {
            this.UltimaEccezioneGenerata = null;
            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            List<OEPrestazione> prestazioni = new List<OEPrestazione>();

            try
            {
                foreach (OEOrdinePrestazione prestazioneordine in this.Prestazioni)
                    prestazioni.Add(prestazioneordine.Prestazione);

                return oeclient.CreaProfilo(this.AmbienteCodLogin, NomeProfilo, prestazioni.ToArray());
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                return false;
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }

        }

        public bool EliminaProfiloUtente(string IDProfilo)
        {
            this.UltimaEccezioneGenerata = null;
            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            try
            {
                return oeclient.EliminaProfilo(this.AmbienteCodLogin, IDProfilo);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                return false;
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }

        }

        public bool SalvaDatiAccessori()
        {
            this.UltimaEccezioneGenerata = null;
            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            bool bret = false;
            try
            {

                OEOrdineDettaglio ordine = oeclient.FillDatiAccessoriEsteso(this.AmbienteCodLogin, this.NumeroOrdine, this.DatiAccessoriRichiedente.ToArray());
                this.CaricaDettaglio(ordine);

                bret = (ordine != null);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                bret = false;
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }

            return bret;
        }

        public bool InoltraOrdine()
        {
            try
            {
                return InoltraOrdine(false);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                return false;
            }
        }
        public bool InoltraOrdine(bool salvataggioLocaleDatiAccessori)
        {
            this.UltimaEccezioneGenerata = null;
            bool bret = false;

            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            try
            {



                OEOrdineDettaglio ordine = oeclient.InoltraOrdineDaWorkstationEsteso(this.AmbienteCodLogin, CoreStatics.CoreApplication.Sessione.Utente.Nome, CoreStatics.CoreApplication.Sessione.Utente.Cognome, this.NumeroOrdine, CoreStatics.CoreApplication.Sessione.Computer.NomeDominioCompleto);
                if (ordine != null)
                {
                    this.CaricaDettaglio(ordine);

                    this.SalvataggioOrdineLocale(salvataggioLocaleDatiAccessori);

                    bret = true;
                }
                else
                    bret = false;
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                bret = false;
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }

            return bret;
        }

        public bool CancellaOrdine()
        {
            this.UltimaEccezioneGenerata = null;
            bool bret = false;
            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();
            string IDOrdineSCCI = this.IDOrdineSCCIDaNumeroOrdineOE(this.NumeroOrdine);

            try
            {
                bret = oeclient.CancellaOrdine(this.AmbienteCodLogin, this.NumeroOrdine);

                if (bret && IDOrdineSCCI != string.Empty)
                {
                    this.ModificaOrdineLocale(IDOrdineSCCI, EnumStatoOrdine.CA, null);
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                bret = false;
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }
            return bret;
        }

        [Obsolete("CopiaOrdine è obsoleta.\nUsare CopiaOrdine3 che utilizza il nuovo metodo dell'OrderEntry CreaOrdineCopia3.")]
        public OEOrdineTestata CopiaOrdine()
        {
            this.UltimaEccezioneGenerata = null;
            OEOrdineTestata ordinecopia = null;
            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            try
            {

                ordinecopia = oeclient.CreaOrdineCopia2(this.AmbienteCodLogin, CoreStatics.CoreApplication.Sessione.Utente.Nome, CoreStatics.CoreApplication.Sessione.Utente.Cognome,
                                                                                this.NumeroOrdine, this.GetTimestamp(), this.Ambiente.Nomepc);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                ordinecopia = null;
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }

            return ordinecopia;
        }

        public OEOrdineTestata CopiaOrdine3(System.Dynamic.ExpandoObject oObject)
        {
            this.UltimaEccezioneGenerata = null;
            OEOrdineTestata ordinecopia = null;
            ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry();

            IDictionary<string, object> dictionary = oObject;

            try
            {

                ordinecopia = oeclient.CreaOrdineCopia3(this.AmbienteCodLogin, this.NumeroOrdine, this.GetTimestamp(),
                                                        dictionary["Nosologico"].ToString(),
                                                        dictionary["CodAzienda"].ToString(),
                                                        dictionary["CodUO"].ToString(),
                                                        false, this.Ambiente.Nomepc);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                this.UltimaEccezioneGenerata = ex;
                ordinecopia = null;
            }
            finally
            {
                if (oeclient != null && oeclient.State == System.ServiceModel.CommunicationState.Opened)
                    oeclient.Close();

                oeclient = null;
            }

            return ordinecopia;
        }

        public string CaricaDescrizioneDatoAggiuntivo(EnumTipoDatoAggiuntivo tipodato, string valore, string valori)
        {
            string sret = string.Empty;
            string[] arvalori = null;
            string[] separatorevaloresingolo = { "#;" };
            string[] separatorevalori = { "§;" };

            switch (tipodato)
            {
                case EnumTipoDatoAggiuntivo.DateBox:
                case EnumTipoDatoAggiuntivo.DateTimeBox:
                case EnumTipoDatoAggiuntivo.FloatBox:
                case EnumTipoDatoAggiuntivo.NumberBox:
                case EnumTipoDatoAggiuntivo.TextBox:
                case EnumTipoDatoAggiuntivo.TimeBox:
                    sret = valore;
                    break;

                case EnumTipoDatoAggiuntivo.ComboBox:
                case EnumTipoDatoAggiuntivo.ListBox:

                    arvalori = valori.Split(separatorevalori, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string val in arvalori)
                    {
                        string[] arvalore = val.Split(separatorevaloresingolo, StringSplitOptions.RemoveEmptyEntries);
                        if (valore == arvalore[0])
                        {
                            if (arvalore.Length > 1)
                                sret = arvalore[1];
                            else
                                sret = arvalore[0];
                            break;
                        }
                    }

                    break;

                case EnumTipoDatoAggiuntivo.ListMultiBox:
                    arvalori = valore.Split(separatorevalori, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string val in arvalori)
                    {
                        if (val != string.Empty)
                        {
                            string descrizione = valori.Substring(valori.IndexOf(val) + 2 + val.Length, valori.Length - (valori.IndexOf(val) + 2 + val.Length));
                            if (descrizione.IndexOf("§;") != -1)
                                descrizione = descrizione.Substring(0, descrizione.IndexOf("§;"));

                            if (descrizione != null && descrizione != string.Empty) sret += @" - " + descrizione + Environment.NewLine;
                        }
                    }

                    break;

                default:
                    sret = string.Empty;
                    break;
            }

            return sret;
        }

        #endregion

        #region rel. Dati Clinici

        public Gestore CaricaDatiOE(bool addButtonCopia = true)
        {
            DcScheda scheda = new DcScheda();

            scheda.ID = "SCH_" + this.NumeroOrdine.ToString();
            scheda.Descrizione = "Dati Agg Ordine " + this.NumeroOrdine.ToString();

            OEDatiAccessoriDescrittore[] arrayDatiAccN = null;
            OEDatoAccessorio[] arrayDatiAcc = null;

            using (ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry())
            {
                arrayDatiAccN = oeclient.GetDatiAccessorNecessari(this.AmbienteCodLogin, this.NumeroOrdine);
                arrayDatiAcc = oeclient.GetDatiAccessori(this.AmbienteCodLogin, this.NumeroOrdine);

                oeclient.Close();
            }

            DcDecodifiche decod = getDcDecodifiche(arrayDatiAccN);

            DcSchedaLayouts layout = new DcSchedaLayouts();
            layout.Layouts = getDcLayout(arrayDatiAccN, addButtonCopia);

            DcDecodifiche sottoschede = new DcDecodifiche();

            List<OEDatiAccessoriDescrittore> datiOE = arrayDatiAccN.OrderBy(x => x.Ordinamento).ToList();

            int sr = 0;

            foreach (OEDatiAccessoriDescrittore datoOE in datiOE)
            {
                List<OEDatiAccessoriDescrittore> datiSezione = new List<OEDatiAccessoriDescrittore>();
                datiSezione.Add(datoOE);

                bool ripetibile = datoOE.Ripetibile;

                DcSezione dcSezione = getDcSezione(datiSezione, layout, sr, ripetibile);
                scheda.Sezioni.Add(dcSezione);

                sr++;
            }



            DcSchedaDati dati = getDcSchedaDati(arrayDatiAccN, arrayDatiAcc);

            Gestore oGestore = CoreStatics.GetGestore();
            oGestore.Scheda = scheda;
            oGestore.SchedaLayouts = layout;
            oGestore.Decodifiche = decod;

            oGestore.NuovaScheda();
            foreach (DcDato oDcDato in dati.Dati.Values)
            {

                if (oDcDato.Value.ToString() == string.Empty)
                {
                    oDcDato.Value = oGestore.SchedaDati.Dati[oDcDato.Key].Value;
                }

            }
            oGestore.SchedaDati = dati;

            return oGestore;

        }

        public bool SalvaDatiOE(DcSchedaDati dati)
        {
            OEDatiAccessoriDescrittore[] arrayDatiAll = null;
            List<OEDatoAccessorio> listToSave = new List<OEDatoAccessorio>();

            using (ScciOrderEntryClient oeclient = ScciSvcRef.GetSvcOrderEntry())
            {
                try
                {
                    arrayDatiAll = oeclient.GetDatiAccessorNecessari(this.AmbienteCodLogin, this.NumeroOrdine);

                    foreach (OEDatiAccessoriDescrittore item in arrayDatiAll)
                    {
                        DcDato dato = null;
                        bool datofound = false;

                        string codice = encodeOeStringToDC(item.Codice);

                        int nMaxSeq = dati.LeggeSequenze(codice);

                        for (int nrow = 1; nrow <= nMaxSeq; nrow++)
                        {
                            datofound = dati.TryGetDato(codice, nrow, out dato);
                            int nRipetizione = nrow - 1;

                            if (datofound)
                            {
                                if (dato.Value == null) dato.Value = "";

                                string retval = dato.Value.ToString();

                                if (retval != null)
                                {
                                    EnumTipoDatoAggiuntivo tipo = ConversioneTipoDatoAggiuntivo(item.Tipo);
                                    if (tipo == EnumTipoDatoAggiuntivo.ListMultiBox)
                                    {
                                        retval = retval.Replace("|", C_SEPVALORI[0]);
                                    }

                                    OEDatoAccessorio oeDato = new OEDatoAccessorio(item.Codice, retval, item.Tipo, nRipetizione);
                                    listToSave.Add(oeDato);
                                }
                            }
                        }

                    }

                    OEOrdineDettaglio ordine = oeclient.FillDatiAccessoriEsteso(this.AmbienteCodLogin, this.NumeroOrdine, listToSave.ToArray());
                    bool bret = (ordine != null);

                    this.SalvataggioOrdineLocale(true);

                    return bret;
                }
                catch (Exception ex)
                {
                    UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                    return false;
                }
                finally
                {
                    oeclient.Close();
                }
            }
        }

        public string encodeOeStringToDC(string oeString)
        {
            string result = oeString;
            string pref = @"OE";

            string[] badchars = null;
            fillBadChars(ref badchars);

            for (int i = 0; i < badchars.Length; i++)
            {
                int v = i + 1;
                string bin = Convert.ToString(v, 2);
                bin = bin.PadLeft(8, '0');

                result = result.Replace(badchars[i], bin);
            }

            result = pref + result;

            return result;
        }

        private void fillBadChars(ref string[] badchars)
        {
            badchars = new string[18];
            badchars[0] = "_";
            badchars[1] = "|";
            badchars[2] = "<";
            badchars[3] = ">";
            badchars[4] = "#";
            badchars[5] = "(";
            badchars[6] = ")";
            badchars[7] = ",";
            badchars[8] = ".";
            badchars[9] = "$";
            badchars[10] = "[";
            badchars[11] = "]";
            badchars[12] = " ";
            badchars[13] = "-";
            badchars[14] = ":";
            badchars[15] = ";";
            badchars[16] = ".";
            badchars[17] = ",";
        }

        private DcSchedaDati getDcSchedaDati(OEDatiAccessoriDescrittore[] arrayDatiAccN, OEDatoAccessorio[] arrayDatiAcc)
        {
            DcSchedaDati dati = new DcSchedaDati();


            foreach (OEDatiAccessoriDescrittore dato in arrayDatiAccN)
            {
                EnumTipoDatoAggiuntivo tipo = ConversioneTipoDatoAggiuntivo(dato.Tipo);

                string codice = encodeOeStringToDC(dato.Codice);

                List<OEDatoAccessorio> itemValues = arrayDatiAcc.Where(x => x.Codice == dato.Codice).OrderBy(x => x.Ripetizione).ToList();

                if (itemValues.Count == 0)
                {
                    DcDato dcdato = new DcDato();
                    dcdato.ID = codice;
                    dcdato.Value = "";
                    if (tipo == EnumTipoDatoAggiuntivo.Titolo)
                    {
                        dcdato.Abilitato = false;
                        dcdato.Value = dato.Descrizione;
                    }
                    dati.Dati.Add(dcdato);
                }
                else
                {
                    foreach (OEDatoAccessorio itemValue in itemValues)
                    {
                        DcDato dcdato = new DcDato();
                        dcdato.ID = codice;
                        dcdato.Value = "";

                        dcdato.Sequenza = itemValue.Ripetizione + 1;
                        string tmpVal = itemValue.Valore;

                        if (tmpVal.Contains(C_SEPVALORI[0]))
                        {
                            string[] val = tmpVal.Split(C_SEPVALORI, StringSplitOptions.RemoveEmptyEntries);
                            tmpVal = "";
                            for (int i = 0; i < val.Length; i++)
                            {
                                tmpVal += val[i] + C_SEPDECONTROLSDC;
                            }

                            tmpVal = tmpVal.Substring(0, tmpVal.Length - 1);
                        }

                        dcdato.Value = tmpVal;
                        if (tipo == EnumTipoDatoAggiuntivo.Titolo)
                        {
                            dcdato.Abilitato = false;
                            dcdato.Value = dato.Descrizione;
                        }

                        dati.Dati.Add(dcdato);
                    }
                }


            }

            return dati;
        }

        private DcDecodifiche getDcDecodifiche(OEDatiAccessoriDescrittore[] arrayDatiAccN)
        {
            DcDecodifiche oDcDecodifiche = new DcDecodifiche();
            DataTable data = createDtDecod();

            foreach (OEDatiAccessoriDescrittore dato in arrayDatiAccN)
            {
                EnumTipoDatoAggiuntivo tipo = ConversioneTipoDatoAggiuntivo(dato.Tipo);

                if (oeTypeHasDictionary(tipo))
                {
                    string[] val = dato.Valori.Split(C_SEPVALORI, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < val.Length; i++)
                    {
                        string codice = encodeOeStringToDC(dato.Codice);

                        string[] ids = val[i].Split(C_SEPVALORISING, StringSplitOptions.RemoveEmptyEntries);
                        string id = ids[0];
                        string descr = ids[0];
                        string key = codice + C_SEPDECOD + id;

                        if (ids.Length > 1)
                            descr = ids[1];

                        DcObject oDcObject = new DcObject();
                        oDcObject.Key = key;
                        oDcObject.Value = descr;
                        oDcDecodifiche.Add(oDcObject.Key, oDcObject);

                        DataRow row = data.NewRow();
                        row["CodTab"] = codice;
                        row["CodValore"] = id;
                        row["Descrizione"] = descr;
                        row["Ordine"] = i;

                        data.Rows.Add(row);
                    }
                }

            }

            oDcDecodifiche.DataTable = data;

            return oDcDecodifiche;
        }

        private DataTable createDtDecod()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CodTab", typeof(String));
            dt.Columns.Add("CodValore", typeof(String));
            dt.Columns.Add("Descrizione", typeof(String));
            dt.Columns.Add("Ordine", typeof(long));

            return dt;
        }

        private DcLayouts getDcLayout(OEDatiAccessoriDescrittore[] arrayDatiAccN, bool addButtonCopia)
        {
            DcLayouts layouts = new DcLayouts();


            foreach (OEDatiAccessoriDescrittore dato in arrayDatiAccN)
            {
                string codice = encodeOeStringToDC(dato.Codice);

                EnumTipoDatoAggiuntivo tipo = ConversioneTipoDatoAggiuntivo(dato.Tipo);
                DcLayout layout = new DcLayout();

                layout.ID = codice;
                layout.Value = layout.ID;

                DcAttributo att = ExtSchedeDati.CreateDcAttributo(EnumAttributiSchedaLayout.NuovaRiga, "True");
                layout.Attributi.Add(att);

                DcAttributo attWidthL = ExtSchedeDati.CreateDcAttributo(EnumAttributiSchedaLayout.LarghezzaDes, "25");
                layout.Attributi.Add(attWidthL);

                DcAttributo attWidth = ExtSchedeDati.CreateDcAttributo(EnumAttributiSchedaLayout.LarghezzaTipo, "65");
                layout.Attributi.Add(attWidth);

                DcAttributo attHeight = ExtSchedeDati.CreateDcAttributo(EnumAttributiSchedaLayout.AltezzaTipo, "1");
                attHeight.Value = "1";

                DcAttributo attFontTipoValore = ExtSchedeDati.CreateDcAttributo(EnumAttributiSchedaLayout.FontTipo, enumFont.Grande.ToString());
                layout.Attributi.Add(attFontTipoValore);

                DcAttributo attFontDes = ExtSchedeDati.CreateDcAttributo(EnumAttributiSchedaLayout.FontDes, enumFont.Grande.ToString());
                layout.Attributi.Add(attFontDes);

                bool bAddButton = false;
                bool bAddButtonCopia = false;
                switch (tipo)
                {
                    case EnumTipoDatoAggiuntivo.Undefined:
                    case EnumTipoDatoAggiuntivo.TextBox:
                        layout.TipoVoce = DatiClinici.Common.Enums.enumTipoVoce.Testo;
                        bAddButton = true;
                        bAddButtonCopia = true;
                        break;
                    case EnumTipoDatoAggiuntivo.Titolo:
                        layout.TipoVoce = DatiClinici.Common.Enums.enumTipoVoce.Testo;
                        break;
                    case EnumTipoDatoAggiuntivo.ComboBox:
                        string[] val = dato.Valori.Split(C_SEPVALORI, StringSplitOptions.RemoveEmptyEntries);

                        if (val.Length > 10)
                            layout.TipoVoce = DatiClinici.Common.Enums.enumTipoVoce.Zoom;
                        else
                            layout.TipoVoce = DatiClinici.Common.Enums.enumTipoVoce.Combo;

                        bAddButtonCopia = true;

                        break;
                    case EnumTipoDatoAggiuntivo.DateBox:
                        layout.TipoVoce = DatiClinici.Common.Enums.enumTipoVoce.Data;
                        bAddButtonCopia = true;
                        break;
                    case EnumTipoDatoAggiuntivo.DateTimeBox:
                        layout.TipoVoce = DatiClinici.Common.Enums.enumTipoVoce.DataOra;
                        bAddButtonCopia = true;
                        break;
                    case EnumTipoDatoAggiuntivo.FloatBox:
                        layout.TipoVoce = DatiClinici.Common.Enums.enumTipoVoce.Decimale;
                        bAddButton = true;
                        bAddButtonCopia = true;
                        break;
                    case EnumTipoDatoAggiuntivo.ListBox:
                        layout.TipoVoce = DatiClinici.Common.Enums.enumTipoVoce.Combo;
                        bAddButtonCopia = true;
                        break;

                    case EnumTipoDatoAggiuntivo.ListMultiBox:

                        val = dato.Valori.Split(C_SEPVALORI, StringSplitOptions.RemoveEmptyEntries);
                        if (val.Length > 5)
                            attHeight.Value = "5";
                        else
                            attHeight.Value = val.Length.ToString();

                        layout.TipoVoce = DatiClinici.Common.Enums.enumTipoVoce.Multipla;

                        bAddButtonCopia = true;
                        break;

                    case EnumTipoDatoAggiuntivo.NumberBox:
                        layout.TipoVoce = DatiClinici.Common.Enums.enumTipoVoce.Numerico;
                        bAddButton = true;
                        bAddButtonCopia = true;
                        break;
                    case EnumTipoDatoAggiuntivo.TimeBox:
                        layout.TipoVoce = DatiClinici.Common.Enums.enumTipoVoce.Ora;
                        bAddButtonCopia = true;
                        break;
                    case EnumTipoDatoAggiuntivo.Tempi:
                        layout.TipoVoce = DatiClinici.Common.Enums.enumTipoVoce.Testo;
                        bAddButtonCopia = true;
                        break;
                }

                if (attHeight.Value.ToString() == "1")
                {
                    int nHeight = 0;

                    double rifDouble = 0;
                    int rifIntero = 0;
                    double rifDecimali = 0;

                    rifDouble = (dato.Descrizione.Length / 21.0);
                    rifIntero = (int)rifDouble;
                    rifDecimali = (int)((rifDouble - (int)rifDouble) * 100);

                    if (rifDecimali > 55)
                    {
                        nHeight = rifIntero + 1;
                    }
                    else
                    {
                        nHeight = rifIntero;
                    }

                    if (nHeight > 1)
                    {
                        attHeight.Value = nHeight.ToString();
                    }
                }

                layout.Attributi.Add(attHeight);

                layouts.Add(layout);

                if (bAddButton)
                {
                    DcLayout btnCdss = getLayoutButton(C_BUTTON_PREFIX + layout.ID, layout.ID);
                    layouts.Add(btnCdss);
                }

                if ((bAddButtonCopia) && (addButtonCopia))
                {
                    DcLayout btnSrcCpy = getLayoutButton(C_BUTTON_PREFIX_SRC + layout.ID, layout.ID);
                    layouts.Add(btnSrcCpy);
                }

                if ((bAddButton) && (!addButtonCopia))
                {
                    DcLayout btnCdss2 = getLayoutButton(C_BUTTON_PREFIX_2 + layout.ID, layout.ID);
                    layouts.Add(btnCdss2);
                }

            }

            return layouts;
        }

        private DcLayout getLayoutButton(string id, string value)
        {

            DcLayout btnlayout = new DcLayout();

            btnlayout.ID = id;
            btnlayout.Value = value;

            DcAttributo btnatt = ExtSchedeDati.CreateDcAttributo(EnumAttributiSchedaLayout.NuovaRiga, "False");
            btnlayout.Attributi.Add(btnatt);

            DcAttributo btnattWidthL = ExtSchedeDati.CreateDcAttributo(EnumAttributiSchedaLayout.LarghezzaDes, "0");
            btnlayout.Attributi.Add(btnattWidthL);

            DcAttributo btnattWidth = ExtSchedeDati.CreateDcAttributo(EnumAttributiSchedaLayout.LarghezzaTipo, "5");
            btnlayout.Attributi.Add(btnattWidth);

            DcAttributo btnattHeight = ExtSchedeDati.CreateDcAttributo(EnumAttributiSchedaLayout.AltezzaTipo, "1");
            btnlayout.Attributi.Add(btnattHeight);

            DcAttributo btnattFontTipoValore = ExtSchedeDati.CreateDcAttributo(EnumAttributiSchedaLayout.FontTipo, enumFont.Grande.ToString());
            btnlayout.Attributi.Add(btnattFontTipoValore);

            DcAttributo btnattFontDes = ExtSchedeDati.CreateDcAttributo(EnumAttributiSchedaLayout.FontDes, enumFont.Grande.ToString());
            btnlayout.Attributi.Add(btnattFontDes);

            btnlayout.TipoVoce = enumTipoVoce.Bottone;

            return btnlayout;
        }

        private bool oeTypeHasDictionary(EnumTipoDatoAggiuntivo tipo)
        {
            switch (tipo)
            {
                case EnumTipoDatoAggiuntivo.Undefined:
                case EnumTipoDatoAggiuntivo.DateBox:
                case EnumTipoDatoAggiuntivo.DateTimeBox:
                case EnumTipoDatoAggiuntivo.FloatBox:
                case EnumTipoDatoAggiuntivo.NumberBox:
                case EnumTipoDatoAggiuntivo.TextBox:
                case EnumTipoDatoAggiuntivo.TimeBox:
                case EnumTipoDatoAggiuntivo.Tempi:
                    return false;

                case EnumTipoDatoAggiuntivo.ComboBox:
                case EnumTipoDatoAggiuntivo.ListBox:
                case EnumTipoDatoAggiuntivo.ListMultiBox:
                    return true;

                default:
                    return false;
            }
        }

        private DcSezione getDcSezione(List<OEDatiAccessoriDescrittore> datiOE, DcSchedaLayouts layout, int secProgr, bool ripetibile)
        {
            DcSezione sezione = new DcSezione();
            sezione.ID = "S" + secProgr.ToString();
            sezione.Descrizione = "";

            DcAttributo attRipetibile = new DcAttributo();
            attRipetibile.ID = "Ripetibile";
            attRipetibile.Value = ripetibile;

            if (ripetibile)
            {
                sezione.Attributi.Add(attRipetibile);

                DcAttributo att = new DcAttributo();
                att.ID = "TipoSezione";
                att.Value = 0;

                sezione.Attributi.Add(att);
            }

            foreach (OEDatiAccessoriDescrittore dato in datiOE)
            {
                DcVoce field = getDcVoceFromOE(dato);
                field.Padre = new DcObject(sezione);
                field.Attributi = new DcAttributi();
                field.Attributi.Add(attRipetibile);

                sezione.Voci.Add(field);

                if (layout.Layouts.ContainsKey(C_BUTTON_PREFIX + field.ID))
                {
                    DcVoce fieldbutton = getDcVoceFromOE(dato);
                    fieldbutton.ID = C_BUTTON_PREFIX + field.ID;
                    fieldbutton.Descrizione = "...";
                    fieldbutton.Obbligatorio = false;
                    fieldbutton.Decodifica = "";
                    fieldbutton.Default = "";
                    sezione.Voci.Add(fieldbutton);
                }

                if (layout.Layouts.ContainsKey(C_BUTTON_PREFIX_SRC + field.ID))
                {
                    DcVoce fieldbutton = getDcVoceFromOE(dato);
                    fieldbutton.ID = C_BUTTON_PREFIX_SRC + field.ID;
                    fieldbutton.Descrizione = "COPIA";
                    fieldbutton.Obbligatorio = false;
                    fieldbutton.Decodifica = "";
                    fieldbutton.Default = "";
                    sezione.Voci.Add(fieldbutton);
                }

                if (layout.Layouts.ContainsKey(C_BUTTON_PREFIX_2 + field.ID))
                {
                    DcVoce fieldbutton = getDcVoceFromOE(dato);
                    fieldbutton.ID = C_BUTTON_PREFIX_2 + field.ID;
                    fieldbutton.Descrizione = "Ref";
                    fieldbutton.Obbligatorio = false;
                    fieldbutton.Decodifica = "";
                    fieldbutton.Default = "";
                    sezione.Voci.Add(fieldbutton);
                }

            }

            return sezione;

        }

        private DcVoce getDcVoceFromOE(OEDatiAccessoriDescrittore datiAccDescr)
        {
            DcVoce field = new DcVoce();

            string codice = encodeOeStringToDC(datiAccDescr.Codice);

            field.ID = codice;
            field.Descrizione = datiAccDescr.Descrizione;
            if (ConversioneTipoDatoAggiuntivo(datiAccDescr.Tipo) == EnumTipoDatoAggiuntivo.Titolo)
            {
                field.Descrizione = datiAccDescr.Etichetta;
            }
            field.Ordine = datiAccDescr.Ordinamento;
            field.Attributi = null;
            field.Obbligatorio = datiAccDescr.Obbligatorio;


            field.Decodifica = codice;
            field.VisualizzaInRilievo = false;

            field.Default = getFormulaDcVoce(datiAccDescr.Codice);

            return field;
        }

        private string getFormulaDcVoce(string CodDatoAccessorio)
        {

            string sRet = string.Empty;

            try
            {

                KeyValuePair<string, string> kvp_formula = this.ListaOEFormule.Find(k => k.Key == CodDatoAccessorio);
                if (kvp_formula.Value != null)
                {
                    sRet = kvp_formula.Value;
                }

            }
            catch (Exception)
            {

            }

            return sRet;

        }

        #endregion rel. Dati Clinici

        #region Private Methods

        private void RiempiTabellaPrestazioniPerGriglie(ref DataTable tabPrest, OEPrestazione[] prestazioni)
        {
            DataRow orow = null;
            try
            {
                foreach (Scci.DataContracts.OEPrestazione prestazione in prestazioni)
                {
                    orow = tabPrest.NewRow();
                    orow["Codice"] = prestazione.Codice;
                    orow["Descrizione"] = prestazione.Descrizione + " (" + prestazione.Erogante.Descrizione + "," + prestazione.Erogante.CodiceAzienda + ")";
                    orow["CodErogante"] = prestazione.Erogante.Codice;
                    orow["DescErogante"] = prestazione.Erogante.Descrizione;
                    orow["AziErogante"] = prestazione.Erogante.CodiceAzienda;
                    orow["DescAziErogante"] = prestazione.Erogante.DescrizioneAzienda;
                    orow["Tipo"] = prestazione.Tipo.ToString();
                    tabPrest.Rows.Add(orow);
                }

                tabPrest.DefaultView.Sort = "Descrizione Asc";
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
        }

        private void RiempiTabellaPrestazioniPerGriglie(ref DataTable tabPrest, List<Scci.DataContracts.OEOrdinePrestazione> prestazioni)
        {
            DataRow orow = null;
            try
            {
                foreach (Scci.DataContracts.OEOrdinePrestazione prestazione in prestazioni)
                {
                    orow = tabPrest.NewRow();
                    orow["Codice"] = prestazione.Prestazione.Codice;
                    orow["Descrizione"] = prestazione.Prestazione.Descrizione + " (" + prestazione.Prestazione.Erogante.Descrizione + "," + prestazione.Prestazione.Erogante.CodiceAzienda + ")";
                    orow["CodErogante"] = prestazione.Prestazione.Erogante.Codice;
                    orow["DescErogante"] = prestazione.Prestazione.Erogante.Descrizione;
                    orow["AziErogante"] = prestazione.Prestazione.Erogante.CodiceAzienda;
                    orow["DescAziErogante"] = prestazione.Prestazione.Erogante.DescrizioneAzienda;
                    orow["Tipo"] = prestazione.Prestazione.Tipo.ToString();
                    orow["Stato"] = CoreStatics.GetEnumDescription(prestazione.StatoErogante);

                    if (tabPrest.Columns.Contains("DataPianificata") && prestazione.DataPianificata != null)
                        orow["DataPianificata"] = prestazione.DataPianificata;

                    tabPrest.Rows.Add(orow);
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
        }

        private DataTable CreaDataTablePerGriglie()
        {
            return CreaDataTablePerGriglie(false);
        }
        private DataTable CreaDataTablePerGriglie(bool aggiungiDataPianificata)
        {

            DataTable oRet = new DataTable();

            try
            {
                oRet.Columns.Add("CodErogante", typeof(string));
                oRet.Columns.Add("DescErogante", typeof(string));
                oRet.Columns.Add("AziErogante", typeof(string));
                oRet.Columns.Add("DescAziErogante", typeof(string));
                oRet.Columns.Add("Codice", typeof(string));
                oRet.Columns.Add("Descrizione", typeof(string));
                oRet.Columns.Add("Tipo", typeof(string));
                oRet.Columns.Add("Stato", typeof(string));

                if (aggiungiDataPianificata) oRet.Columns.Add("DataPianificata", typeof(DateTime));
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                oRet = new DataTable();
            }

            return oRet;

        }

        private string SerializzaOrdine()
        {
            string sxml = string.Empty;

            try
            {

                XDocument miXML = new XDocument
                    (
                        new XDeclaration("1.0", "utf-8", "yes"),
                        new XElement(
                            "MovOrdine",
                            new XElement("IDOrdine", this.IDOrdine),
                            new XElement("NumeroOrdine", this.NumeroOrdine),
                            new XElement("Priorita", this.Priorita.ToString()),
                            new XElement("DataProgrammazione", this.DataProgrammazione.ToLongDateString()),
                            new XElement("Azione", this.Azione.ToString()),
                            new XElement("IDTrasferimento", this.IDTrasferimento),
                            new XElement("CodUO", this.CodUO),
                            new XElement("IDEpisodio", this.IDEpisodio),
                            new XElement("NumeroEpisodio", this.NumeroEpisodio),
                            new XElement("CodTipoEpisodio", this.CodTipoEpisodio),
                            new XElement("CodAzienda", this.CodAzienda),
                            new XElement("StatoOrdine", this.StatoOrdine.ToString()),
                            new XElement("Eroganti", this.Eroganti),
                            new XElement("UtenteInoltro", this.UtenteInoltro),
                            new XElement("DataInoltro", this.DataInoltro.ToLongDateString()),
                            new XElement("ListaPrestazioni", this.ListaPrestazioni)
                            )
                    );

                sxml = miXML.ToString();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                sxml = string.Empty;
            }

            return sxml;
        }

        private EnumTipoDatoAggiuntivo ConversioneTipoDatoAggiuntivo(string tipodatoOE)
        {
            EnumTipoDatoAggiuntivo ret = EnumTipoDatoAggiuntivo.Undefined;

            switch (tipodatoOE.ToUpper())
            {
                case "COMBOBOX":
                    ret = EnumTipoDatoAggiuntivo.ComboBox;
                    break;
                case "DATEBOX":
                    ret = EnumTipoDatoAggiuntivo.DateBox;
                    break;
                case "DATETIMEBOX":
                    ret = EnumTipoDatoAggiuntivo.DateTimeBox;
                    break;
                case "FLOATBOX":
                    ret = EnumTipoDatoAggiuntivo.FloatBox;
                    break;
                case "LISTBOX":
                    ret = EnumTipoDatoAggiuntivo.ListBox;
                    break;
                case "LISTMULTIBOX":
                    ret = EnumTipoDatoAggiuntivo.ListMultiBox;
                    break;
                case "NUMBERBOX":
                    ret = EnumTipoDatoAggiuntivo.NumberBox;
                    break;
                case "TEXTBOX":
                    ret = EnumTipoDatoAggiuntivo.TextBox;
                    break;
                case "TIMEBOX":
                    ret = EnumTipoDatoAggiuntivo.TimeBox;
                    break;
                case "":
                case "TITOLO":
                    ret = EnumTipoDatoAggiuntivo.Titolo;
                    break;
                default:
                    ret = EnumTipoDatoAggiuntivo.Undefined;
                    break;
            }

            return ret;
        }

        private string GetTimestamp()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
        }

        public string IDOrdineSCCIDaNumeroOrdineOE(string NumeroOrdineOE)
        {
            string sRet = string.Empty;

            this.UltimaEccezioneGenerata = null;

            try
            {
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                op.Parametro.Add("IDOrdineOE", this.IDOrdine);
                op.Parametro.Add("NumeroOrdineOE", this.NumeroOrdine);

                op.TimeStamp.CodEntita = EnumEntita.OE.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VAL.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable odt = Database.GetDataTableStoredProc("MSP_SelMovOrdini", spcoll);

                if (odt != null && odt.Rows.Count == 1)
                {
                    sRet = odt.Rows[0]["ID"].ToString();
                }
                else
                    sRet = string.Empty;

            }
            catch (Exception ex)
            {
                this.UltimaEccezioneGenerata = ex;
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                sRet = string.Empty;
            }

            return sRet;
        }


        private void SalvataggioOrdineLocale(bool salvaDatiAccessori)
        {

            Gestore gestore = null;
            if (salvaDatiAccessori)
                gestore = this.CaricaDatiOE();


            if (this.InseritoInLocale)
            {
                string IDOrdineSCCI = this.IDOrdineSCCIDaNumeroOrdineOE(this.NumeroOrdine);

                if (IDOrdineSCCI != string.Empty)
                {
                    this.ModificaOrdineLocale(IDOrdineSCCI, CommonStatics.TraduciOEStato(this.StatoOrdine), gestore, salvaDatiAccessori);
                }
                else
                {
                    this.InserisciOrdineLocale(CommonStatics.TraduciOEStato(this.StatoOrdine), gestore, salvaDatiAccessori);
                }
            }
            else
            {
                this.InserisciOrdineLocale(CommonStatics.TraduciOEStato(this.StatoOrdine), gestore, salvaDatiAccessori);
            }
        }

        private void InserisciOrdineLocale(EnumStatoOrdine StatoOrdine, Gestore gestoreDC)
        {
            try
            {
                InserisciOrdineLocale(StatoOrdine, gestoreDC, true);
            }
            catch (Exception ex)
            {
                this.UltimaEccezioneGenerata = ex;
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
        }
        private void InserisciOrdineLocale(EnumStatoOrdine StatoOrdine, Gestore gestoreDC, bool salvaDatiAccessori)
        {


            DataTable odt = null;
            Parametri op = null;
            SqlParameterExt[] spcoll = null;
            string xmlParam = string.Empty;

            this.UltimaEccezioneGenerata = null;

            try
            {
                op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                op.Parametro.Add("IDPaziente", this.Paziente.ID);
                op.Parametro.Add("IDEpisodio", this.IDEpisodio);
                op.Parametro.Add("IDTrasferimento", this.IDTrasferimento);
                op.Parametro.Add("IDOrdineOE", this.IDOrdine);
                op.Parametro.Add("NumeroOrdineOE", this.NumeroOrdine);
                op.Parametro.Add("XMLOE", this.SerializzaOrdine());
                op.Parametro.Add("Eroganti", this.Eroganti);
                op.Parametro.Add("Prestazioni", this.ListaPrestazioni);
                op.Parametro.Add("CodStatoOrdine", StatoOrdine.ToString());
                op.Parametro.Add("CodUtenteInserimento", this.AmbienteCodLogin);
                op.Parametro.Add("CodUtenteUltimaModifica", this.AmbienteCodLogin);
                op.Parametro.Add("CodUtenteInoltro", this.AmbienteCodLogin);
                op.Parametro.Add("DataProgrammazioneOE", Database.dataOra105PerParametri(this.DataProgrammazione));
                op.Parametro.Add("DataProgrammazioneOEUTC", Database.dataOra105PerParametri(this.DataProgrammazione.ToUniversalTime()));
                op.Parametro.Add("DataInserimento", Database.dataOra105PerParametri(DateTime.Now));
                op.Parametro.Add("DataInserimentoUTC", Database.dataOra105PerParametri(DateTime.Now.ToUniversalTime()));
                op.Parametro.Add("DataUltimaModifica", Database.dataOra105PerParametri(DateTime.Now));
                op.Parametro.Add("DataUltimaModificaUTC", Database.dataOra105PerParametri(DateTime.Now.ToUniversalTime()));
                op.Parametro.Add("DataInoltro", Database.dataOra105PerParametri(DateTime.Now));
                op.Parametro.Add("DataInoltroUTC", Database.dataOra105PerParametri(DateTime.Now.ToUniversalTime()));
                op.Parametro.Add("CodPriorita", this.Priorita.ToString());
                op.Parametro.Add("CodUAInserimento", this.CodUA);
                op.Parametro.Add("CodSistema", this.CodSistema);
                op.Parametro.Add("IDSistema", this.IDSistema);
                op.Parametro.Add("IDGruppo", this.IDGruppo);
                op.Parametro.Add("InfoSistema", this.InfoSistema);
                op.Parametro.Add("InfoSistema2", this.InfoSistema2);
                if (salvaDatiAccessori && gestoreDC != null)
                {
                    op.Parametro.Add("StrutturaDatiAccessori", gestoreDC.SchedaXML);
                    op.Parametro.Add("DatiDatiAccessori", gestoreDC.SchedaDatiXML);
                    op.Parametro.Add("LayoutDatiAccessori", gestoreDC.SchedaLayoutsXML);
                }

                op.TimeStamp.CodEntita = EnumEntita.OE.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VAL.ToString();

                spcoll = new SqlParameterExt[1];

                xmlParam = XmlProcs.XmlSerializeToString(op);
                xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                odt = Database.GetDataTableStoredProc("MSP_InsMovOrdini", spcoll);

                if (odt != null && odt.Rows.Count > 0)
                {
                    foreach (OESistemaErogante erog in this.ListaEroganti)
                    {
                        op.Parametro.Clear();
                        op.Parametro.Add("IDOrdine", odt.Rows[0]["ID"].ToString());
                        op.Parametro.Add("CodTipoOrdine", erog.CodiceAzienda + "|" + erog.Codice);
                        op.Parametro.Add("DescrizioneTipoOrdine", erog.Descrizione + " (" + erog.CodiceAzienda + ")");

                        spcoll = new SqlParameterExt[1];

                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        Database.ExecStoredProc("MSP_InsMovOrdiniEroganti", spcoll);
                    }
                }
            }
            catch (Exception ex)
            {
                this.UltimaEccezioneGenerata = ex;
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
            finally
            {
                if (odt != null)
                {
                    odt.Dispose();
                    odt = null;
                }
                op = null;
                spcoll = null;
                xmlParam = null;
            }
        }

        private void ModificaOrdineLocale(string IDOrdineSCCI, EnumStatoOrdine StatoOrdine, Gestore gestoreDC)
        {
            try
            {
                ModificaOrdineLocale(IDOrdineSCCI, StatoOrdine, gestoreDC, true);
            }
            catch (Exception ex)
            {
                this.UltimaEccezioneGenerata = ex;
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
        }
        private void ModificaOrdineLocale(string IDOrdineSCCI, EnumStatoOrdine StatoOrdine, Gestore gestoreDC, bool salvaDatiAccessori)
        {


            Parametri op = null;
            SqlParameterExt[] spcoll = null;
            string xmlParam = string.Empty;

            this.UltimaEccezioneGenerata = null;

            try
            {
                op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                op.Parametro.Add("IDOrdineOE", this.IDOrdine);
                op.Parametro.Add("NumeroOrdineOE", this.NumeroOrdine);
                op.Parametro.Add("XMLOE", this.SerializzaOrdine());
                op.Parametro.Add("Eroganti", this.Eroganti);
                op.Parametro.Add("Prestazioni", this.ListaPrestazioni);
                op.Parametro.Add("CodStatoOrdine", StatoOrdine.ToString());
                op.Parametro.Add("CodUtenteUltimaModifica", this.AmbienteCodLogin);
                op.Parametro.Add("DataProgrammazioneOE", Database.dataOra105PerParametri(this.DataProgrammazione));
                op.Parametro.Add("DataProgrammazioneOEUTC", Database.dataOra105PerParametri(this.DataProgrammazione.ToUniversalTime()));
                op.Parametro.Add("DataUltimaModifica", Database.dataOra105PerParametri(DateTime.Now));
                op.Parametro.Add("DataUltimaModificaUTC", Database.dataOra105PerParametri(DateTime.Now.ToUniversalTime()));
                op.Parametro.Add("CodPriorita", this.Priorita.ToString());
                op.Parametro.Add("CodUAUltimaModifica", this.CodUA);
                if (salvaDatiAccessori && gestoreDC != null)
                {
                    op.Parametro.Add("StrutturaDatiAccessori", gestoreDC.SchedaXML);
                    op.Parametro.Add("DatiDatiAccessori", gestoreDC.SchedaDatiXML);
                    op.Parametro.Add("LayoutDatiAccessori", gestoreDC.SchedaLayoutsXML);
                }

                if (this.CodSistema != string.Empty) { op.Parametro.Add("CodSistema", this.CodSistema); }
                if (this.IDSistema != string.Empty) { op.Parametro.Add("IDSistema", this.IDSistema); }
                if (this.IDGruppo != string.Empty) { op.Parametro.Add("IDGruppo", this.IDGruppo); }
                if (this.InfoSistema != string.Empty) { op.Parametro.Add("InfoSistema", this.InfoSistema); }
                if (this.InfoSistema2 != string.Empty) { op.Parametro.Add("InfoSistema2", this.InfoSistema2); }

                op.TimeStamp.CodEntita = EnumEntita.OE.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.CAN.ToString();

                spcoll = new SqlParameterExt[1];

                xmlParam = XmlProcs.XmlSerializeToString(op);
                xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                Database.ExecStoredProc("MSP_AggMovOrdini", spcoll);

                op.Parametro.Clear();
                op.Parametro.Add("IDOrdine", IDOrdineSCCI);

                spcoll = new SqlParameterExt[1];

                xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                Database.ExecStoredProc("MSP_CancMovOrdiniEroganti", spcoll);

                foreach (OESistemaErogante erog in this.ListaEroganti)
                {
                    op.Parametro.Clear();
                    op.Parametro.Add("IDOrdine", IDOrdineSCCI);
                    op.Parametro.Add("CodTipoOrdine", erog.CodiceAzienda + "|" + erog.Codice);
                    op.Parametro.Add("DescrizioneTipoOrdine", erog.Descrizione + " (" + erog.CodiceAzienda + ")");

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    Database.ExecStoredProc("MSP_InsMovOrdiniEroganti", spcoll);
                }
            }
            catch (Exception ex)
            {
                this.UltimaEccezioneGenerata = ex;
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
            finally
            {
                op = null;
                spcoll = null;
                xmlParam = null;
            }
        }

        #endregion

    }
}
