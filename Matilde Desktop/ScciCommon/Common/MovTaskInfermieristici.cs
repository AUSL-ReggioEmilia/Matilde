using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;

using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.Scci.PluginClient;

namespace UnicodeSrl.Scci
{
    [Serializable()]
    public class MovTaskInfermieristico
    {
        private string _codua = string.Empty;
        private string _idmovtaskinfermieristico = string.Empty;
        private string _idepisodio = string.Empty;
        private string _idpaziente = string.Empty;
        private string _idtrasferimento = string.Empty;
        private DateTime _dataevento = DateTime.MinValue;
        private string _codsistema = string.Empty;
        private string _idsistema = string.Empty;
        private string _idgruppo = string.Empty;
        private string _codtipotaskinfermieristico = string.Empty;
        private string _descrtipotaskinfermieristico = string.Empty;
        private int _anticipominutitipotaskinfermieristico = 0;
        private string _codstatotaskinfermieristico = string.Empty;
        private string _descrstatotaskinfermieristico = string.Empty;

        private string _sottoclasse = string.Empty;

        private string _codtiporegistrazione = string.Empty;

        private DateTime _dataprogrammata = DateTime.MinValue;
        private DateTime _dataerogazione = DateTime.MinValue;

        private string _note = string.Empty;
        private string _descrizioneFUT = string.Empty;

        private string _codutenterilevazione = string.Empty;
        private string _descrutenterilevazione = string.Empty;

        private string _codutenteultimamodifica = string.Empty;
        private string _descrutenteultimamodifica = string.Empty;

        private byte[] _icona = null;

        private string _idscheda = string.Empty;
        private string _idtaskiniziale = string.Empty;
        private string _codscheda = string.Empty;
        private int _versionescheda = 0;

        private string _anteprimartf = string.Empty;

        private bool _periodicita = false;

        private int _permessoBloccato = 0;
        private int _permessoModifica = 0;
        private int _permessoErogazione = 0;
        private int _permessoAnnulla = 0;
        private int _permessoCancella = 0;
        private int _permessoCopia = 0;

        private EnumAzioni _azione = EnumAzioni.MOD;

        private MovScheda _movScheda = null;

        private DateTime _periodicitadatafine = DateTime.MinValue;
        private int _periodicitagiorni = 0;
        private int _periodicitaore = 0;
        private int _periodicitaminuti = 0;
        private int _durata = 0;

        private string _codprotocollo = string.Empty;
        private string _codtipoprotocollo = string.Empty;
        private bool _continuita = false;
        private string _codtipotaskinfermieristicotempi = string.Empty;

        private string _posologiaeffettiva = string.Empty;

        private string _alert = string.Empty;
        private string _barcode = string.Empty;

        private List<string> _list_codtipotaskinfermieristico = null;

        public MovTaskInfermieristico(string idmovtaskinfermieristico, DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = EnumAzioni.MOD;
            _idmovtaskinfermieristico = idmovtaskinfermieristico;
            _list_codtipotaskinfermieristico = null;
            this.SoloTestata = false;
            this.ErogazioneDiretta = false;
            this.Carica(idmovtaskinfermieristico);
        }
        public MovTaskInfermieristico(string idmovtaskinfermieristico, EnumAzioni azione, DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = azione;
            _idmovtaskinfermieristico = idmovtaskinfermieristico;
            _list_codtipotaskinfermieristico = null;
            this.SoloTestata = false;
            this.ErogazioneDiretta = false;
            this.Carica(idmovtaskinfermieristico);
        }
        public MovTaskInfermieristico(string codua, string idpaziente, string idepisodio, string idtrasferimento,
                                        EnumCodSistema codsistema, EnumTipoRegistrazione codtiporegistrazione, DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = EnumAzioni.INS;
            _codua = codua;
            _idpaziente = idpaziente;
            _idepisodio = idepisodio;
            _idtrasferimento = idtrasferimento;
            _idmovtaskinfermieristico = "";
            _codscheda = "";

            _codtiporegistrazione = Enum.GetName(typeof(EnumTipoRegistrazione), codtiporegistrazione);
            _codsistema = Enum.GetName(typeof(EnumCodSistema), codsistema);
            _dataprogrammata = DateTime.Now;
            _descrtipotaskinfermieristico = @"Selezionare Tipo Task Infermieristico";

            _sottoclasse = "";
            this.SoloTestata = false;
            this.ErogazioneDiretta = false;

            _list_codtipotaskinfermieristico = null;
        }

        public ScciAmbiente Ambiente { get; set; }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string CodUA
        {
            get { return _codua; }
            set { _codua = value; }
        }

        public string IDMovTaskInfermieristico
        {
            get { return _idmovtaskinfermieristico; }
            set
            {
                if (_idmovtaskinfermieristico != value) _movScheda = null;
                _idmovtaskinfermieristico = value;
            }
        }

        public string IDPaziente
        {
            get { return _idpaziente; }
        }

        public string IDEpisodio
        {
            get { return _idepisodio; }
            set { _idepisodio = value; }
        }

        public string IDTrasferimento
        {
            get { return _idtrasferimento; }
            set { _idtrasferimento = value; }
        }

        public string CodSistema
        {
            get { return _codsistema; }
            set { _codsistema = value; }
        }

        public string IDSistema
        {
            get { return _idsistema; }
            set { _idsistema = value; }
        }

        public string IDGruppo
        {
            get { return _idgruppo; }
            set { _idgruppo = value; }
        }

        public string CodTipoTaskInfermieristico
        {
            get { return _codtipotaskinfermieristico; }
            set
            {
                if (_codtipotaskinfermieristico != value) _movScheda = null;
                _codtipotaskinfermieristico = value;
            }

        }

        public string DescrTipoTaskInfermieristico
        {
            get { return _descrtipotaskinfermieristico; }
            set { _descrtipotaskinfermieristico = value; }
        }

        public int AnticipoMinutiTipoTaskInfermieristico
        {
            get { return _anticipominutitipotaskinfermieristico; }
            set { _anticipominutitipotaskinfermieristico = value; }
        }

        public string CodStatoTaskInfermieristico
        {
            get { return _codstatotaskinfermieristico; }
            set { _codstatotaskinfermieristico = value; }
        }

        public string DescrStatoTaskInfermieristico
        {
            get { return _descrstatotaskinfermieristico; }
        }

        public string CodTipoRegistrazione
        {
            get { return _codtiporegistrazione; }
            set { _codtiporegistrazione = value; }
        }

        public string IDScheda
        {
            get { return _idscheda; }
        }

        public string IDTaskIniziale
        {
            get { return _idtaskiniziale; }
            set { _idtaskiniziale = value; }
        }

        public string CodScheda
        {
            get { return _codscheda; }
            set
            {
                if (_codscheda != value) _movScheda = null;
                _codscheda = value;
            }
        }

        public string Sottoclasse
        {
            get
            {
                return _sottoclasse;
            }
            set
            {
                _sottoclasse = value;
            }
        }
        public string SottoclassexParamStored
        {
            get { return UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(this.Sottoclasse); }
        }

        public int VersioneScheda
        {
            get { return _versionescheda; }
        }

        public MovScheda MovScheda
        {
            get
            {
                if (_movScheda == null)
                {
                    if (_idmovtaskinfermieristico != null && _idmovtaskinfermieristico != string.Empty && _idmovtaskinfermieristico.Trim() != "")
                    {
                        _movScheda = new MovScheda(EnumEntita.WKI.ToString(), _idmovtaskinfermieristico, this.Ambiente);
                    }
                    else if (_codscheda != null && _codscheda != string.Empty && _codscheda.Trim() != "")
                    {
                        _movScheda = new MovScheda(_codscheda, EnumEntita.WKI, _codua, _idpaziente, _idepisodio, _idtrasferimento, this.Ambiente);
                        Gestore oGestore = CommonStatics.GetGestore(this.Ambiente);
                        oGestore.SchedaXML = _movScheda.Scheda.StrutturaXML;
                        oGestore.SchedaLayoutsXML = _movScheda.Scheda.LayoutXML;
                        oGestore.Decodifiche = _movScheda.Scheda.DizionarioValori();
                        oGestore.NuovaScheda();
                        _movScheda.DatiXML = oGestore.SchedaDatiXML;
                    }
                }

                return _movScheda;

            }
            set { _movScheda = value; }
        }

        public MovScheda CreaMovSchedaNoRtf(bool Ricalcolo)
        {
            if (_movScheda == null)
            {
                if (_idmovtaskinfermieristico != null && _idmovtaskinfermieristico != string.Empty && _idmovtaskinfermieristico.Trim() != "")
                {
                    _movScheda = new MovScheda(EnumEntita.WKI.ToString(), _idmovtaskinfermieristico, this.Ambiente);
                }
                else if (_codscheda != null && _codscheda != string.Empty && _codscheda.Trim() != "")
                {
                    _movScheda = new MovScheda(_codscheda, EnumEntita.WKI, _codua, _idpaziente, _idepisodio, _idtrasferimento, this.Ambiente);
                    Gestore oGestore = CommonStatics.GetGestore(this.Ambiente);
                    oGestore.SchedaXML = _movScheda.Scheda.StrutturaXML;
                    oGestore.SchedaLayoutsXML = _movScheda.Scheda.LayoutXML;
                    oGestore.Decodifiche = _movScheda.Scheda.DizionarioValori();
                    oGestore.NuovaScheda(Ricalcolo);
                    bool flagCache = _movScheda.FlagCreaRtfOnSet;
                    _movScheda.FlagCreaRtfOnSet = false;

                    _movScheda.DatiXML = oGestore.SchedaDatiXML;

                    _movScheda.FlagCreaRtfOnSet = flagCache;
                }
            }

            return _movScheda;

        }

        public DateTime DataEvento
        {
            get { return _dataevento; }
            set { _dataevento = value; }
        }

        public DateTime DataErogazione
        {
            get { return _dataerogazione; }
            set { _dataerogazione = value; }
        }

        public DateTime DataProgrammata
        {
            get { return _dataprogrammata; }
            set { _dataprogrammata = value; }
        }

        public string CodUtenteRilevazione
        {
            get { return _codutenterilevazione; }
            set { _codutenterilevazione = value; }
        }

        public string DescrUtenteRilevazione
        {
            get { return _descrutenterilevazione; }
        }

        public string CodUtenteUltimaModifica
        {
            get { return _codutenteultimamodifica; }
            set { _codutenteultimamodifica = value; }
        }

        public string DescrUtenteUltimaModifica
        {
            get { return _descrutenteultimamodifica; }
        }

        public string Note
        {
            get { return _note; }
            set { _note = value; }
        }
        public string NotexParamStored
        {
            get { return UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(this.Note); }
        }

        public int PermessoBloccato
        {
            get { return _permessoBloccato; }
            set { _permessoBloccato = value; }
        }

        public int PermessoModifica
        {
            get { return _permessoModifica; }
            set { _permessoModifica = value; }
        }


        public int PermessoErogazione
        {
            get { return _permessoErogazione; }
            set { _permessoErogazione = value; }
        }

        public int PermessoAnnulla
        {
            get { return _permessoAnnulla; }
            set { _permessoAnnulla = value; }
        }

        public int PermessoCancella
        {
            get { return _permessoCancella; }
            set { _permessoCancella = value; }
        }

        public int PermessoCopia
        {
            get { return _permessoCopia; }
            set { _permessoCopia = value; }
        }

        public byte[] Icona
        {
            get { return _icona; }
        }

        public bool Periodicita
        {
            get
            {
                return _periodicita;
            }
            set { _periodicita = value; }
        }

        public DateTime PeriodicitaDataFine
        {
            get { return _periodicitadatafine; }
            set { _periodicitadatafine = value; }
        }

        public int PeriodicitaGiorni
        {
            get { return _periodicitagiorni; }
            set { _periodicitagiorni = value; }
        }

        public int PeriodicitaOre
        {
            get { return _periodicitaore; }
            set { _periodicitaore = value; }
        }

        public int PeriodicitaMinuti
        {
            get { return _periodicitaminuti; }
            set { _periodicitaminuti = value; }
        }

        public int Durata
        {
            get { return _durata; }
            set { _durata = value; }
        }

        public string DescrizioneFUT
        {
            get { return _descrizioneFUT; }
            set { _descrizioneFUT = value; }
        }
        public string DescrizioneFUTxParamStored
        {
            get { return UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(this.DescrizioneFUT); }
        }

        public string CodProtocollo
        {
            get { return _codprotocollo; }
            set { _codprotocollo = value; }
        }

        public string CodTipoProtocollo
        {
            get { return _codtipoprotocollo; }
            set { _codtipoprotocollo = value; }
        }

        public bool Continuita
        {
            get { return _continuita; }
            set { _continuita = value; }
        }

        public string CodTipoTaskInfermieristicoTempi
        {
            get { return _codtipotaskinfermieristicotempi; }
            set { _codtipotaskinfermieristicotempi = value; }
        }

        public string PosologiaEffettiva
        {
            get { return _posologiaeffettiva; }
            set { _posologiaeffettiva = value; }
        }
        public string PosologiaEffettivaxParamStored
        {
            get { return UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(this.PosologiaEffettiva); }
        }

        public string Alert
        {
            get { return _alert; }
            set { _alert = value; }
        }
        public string AlertxParamStored
        {
            get { return UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(this.Alert); }
        }

        public string Barcode
        {
            get { return _barcode; }
            set { _barcode = value; }
        }
        public string BarcodexParamStored
        {
            get { return UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(this.Barcode); }
        }

        public List<string> List_CodTipoTaskInfermieristico
        {
            get { return _list_codtipotaskinfermieristico; }
            set { _list_codtipotaskinfermieristico = value; }

        }

        public bool SoloTestata { get; set; }

        public bool ErogazioneDiretta { get; set; }

        public bool Cancella()
        {
            return this.Cancella(true);
        }

        public bool Cancella(bool RicaricaMovimento)
        {
            return this.Cancella(RicaricaMovimento, true);
        }

        public bool Cancella(bool RicaricaMovimento, bool FlagGeneraRTF)
        {
            try
            {
                this.Azione = EnumAzioni.CAN;
                this.CodStatoTaskInfermieristico = Enum.GetName(typeof(EnumStatoTaskInfermieristico), EnumStatoTaskInfermieristico.CA);

                if (Salva(RicaricaMovimento, FlagGeneraRTF))
                {
                    Azione = EnumAzioni.MOD;
                    if (RicaricaMovimento) Carica(_idmovtaskinfermieristico);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Annulla()
        {
            return this.Annulla(true);
        }
        public bool Annulla(bool RicaricaMovimento)
        {
            return this.Annulla(RicaricaMovimento, true);
        }
        public bool Annulla(bool RicaricaMovimento, bool FlagGeneraRTF)
        {
            try
            {
                this.Azione = EnumAzioni.ANN;
                this.CodStatoTaskInfermieristico = Enum.GetName(typeof(EnumStatoTaskInfermieristico), EnumStatoTaskInfermieristico.AN);

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    if (RicaricaMovimento) Carica(_idmovtaskinfermieristico);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Salva()
        {
            return this.Salva(true, true);
        }

        public bool Salva(bool RicaricaMovimento)
        {
            return this.Salva(RicaricaMovimento, true);
        }

        public bool Salva(bool RicaricaMovimento, bool FlagGeneraRTF)
        {
            bool bReturn = true;

            try
            {
                Parametri op = null;
                SqlParameterExt[] spcoll;
                string xmlParam = "";
                if (_idmovtaskinfermieristico != string.Empty && _idmovtaskinfermieristico.Trim() != "")
                {
                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDTaskInfermieristico", _idmovtaskinfermieristico);
                    op.Parametro.Add("IDEpisodio", _idepisodio);
                    op.Parametro.Add("IDTrasferimento", _idtrasferimento);
                    op.Parametro.Add("DataEvento", Database.dataOra105PerParametri(_dataevento));
                    op.Parametro.Add("DataEventoUTC", Database.dataOra105PerParametri(_dataevento.ToUniversalTime()));
                    op.Parametro.Add("CodSistema", _codsistema);
                    op.Parametro.Add("IDSistema", _idsistema);
                    op.Parametro.Add("IDGruppo", _idgruppo);
                    op.Parametro.Add("CodTipoTaskInfermieristico", _codtipotaskinfermieristico);
                    op.Parametro.Add("CodStatoTaskInfermieristico", _codstatotaskinfermieristico);
                    op.Parametro.Add("DataProgrammata", Database.dataOra105PerParametri(_dataprogrammata));
                    op.Parametro.Add("DataProgrammataUTC", Database.dataOra105PerParametri(_dataprogrammata.ToUniversalTime()));

                    op.Parametro.Add("Sottoclasse", this.SottoclassexParamStored);


                    if (_dataerogazione != DateTime.MinValue)
                    {
                        op.Parametro.Add("DataErogazione", Database.dataOra105PerParametri(_dataerogazione));
                        op.Parametro.Add("DataErogazioneUTC", Database.dataOra105PerParametri(_dataerogazione.ToUniversalTime()));
                    }
                    op.Parametro.Add("Note", this.NotexParamStored);
                    op.Parametro.Add("DescrizioneFUT", this.DescrizioneFUTxParamStored);
                    op.Parametro.Add("PosologiaEffettiva", this.PosologiaEffettivaxParamStored);
                    op.Parametro.Add("Alert", this.AlertxParamStored);
                    op.Parametro.Add("Barcode", this.BarcodexParamStored);

                    op.TimeStamp.CodEntita = EnumEntita.WKI.ToString();
                    op.TimeStamp.IDEntita = _idmovtaskinfermieristico;
                    op.TimeStamp.CodAzione = _azione.ToString();

                    this.MovScheda.IDEpisodio = _idepisodio;
                    this.MovScheda.IDTrasferimento = _idtrasferimento;
                    op.MovScheda = this.MovScheda;

                    this.MovScheda.Azione = _azione;

                    bReturn = this.MovScheda.Salva(RicaricaMovimento, FlagGeneraRTF);
                    if (bReturn)
                    {
                        spcoll = new SqlParameterExt[1];

                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        Database.ExecStoredProc("MSP_AggMovTaskInfermieristici", spcoll);
                    }
                }
                else
                {

                    if (this.Periodicita) _idgruppo = Guid.NewGuid().ToString();

                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDEpisodio", _idepisodio);
                    op.Parametro.Add("IDTrasferimento", _idtrasferimento);
                    op.Parametro.Add("DataEvento", Database.dataOra105PerParametri(_dataevento));
                    op.Parametro.Add("DataEventoUTC", Database.dataOra105PerParametri(_dataevento.ToUniversalTime()));
                    op.Parametro.Add("CodSistema", _codsistema);
                    op.Parametro.Add("IDSistema", _idsistema);
                    op.Parametro.Add("IDGruppo", _idgruppo);
                    op.Parametro.Add("IDTaskIniziale", _idtaskiniziale);
                    op.Parametro.Add("CodTipoTaskInfermieristico", _codtipotaskinfermieristico);
                    op.Parametro.Add("CodStatoTaskInfermieristico", _codstatotaskinfermieristico);
                    op.Parametro.Add("CodTipoRegistrazione", _codtiporegistrazione);

                    op.Parametro.Add("CodUtenteRilevazione", _codutenterilevazione);


                    List<IntervalloTempi> listaperiodicita = this.AnteprimaPeriodicita();
                    if (listaperiodicita.Count > 0)
                    {
                        _dataprogrammata = ((IntervalloTempi)listaperiodicita[0]).DataOraInizio;
                    }
                    op.Parametro.Add("DataProgrammata", Database.dataOra105PerParametri(_dataprogrammata));
                    op.Parametro.Add("DataProgrammataUTC", Database.dataOra105PerParametri(_dataprogrammata.ToUniversalTime()));

                    op.Parametro.Add("Sottoclasse", this.SottoclassexParamStored);

                    op.Parametro.Add("Note", this.NotexParamStored);
                    op.Parametro.Add("DescrizioneFUT", this.DescrizioneFUTxParamStored);
                    op.Parametro.Add("PosologiaEffettiva", this.PosologiaEffettivaxParamStored);
                    op.Parametro.Add("Alert", this.AlertxParamStored);
                    op.Parametro.Add("Barcode", this.BarcodexParamStored);

                    op.TimeStamp.CodEntita = EnumEntita.WKI.ToString();
                    op.TimeStamp.IDEntita = _idmovtaskinfermieristico;
                    op.TimeStamp.CodAzione = _azione.ToString();
                    op.MovScheda = this.MovScheda;

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovTaskInfermieristici", spcoll);
                    if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                    {
                        _idmovtaskinfermieristico = dt.Rows[0][0].ToString();

                        this.MovScheda.IDEntita = _idmovtaskinfermieristico;
                        this.MovScheda.Azione = _azione;
                        bReturn = this.MovScheda.Salva(RicaricaMovimento, FlagGeneraRTF);

                        if (this.Periodicita) this.GeneraPeriodicita(listaperiodicita);

                        _azione = EnumAzioni.MOD;

                        if (RicaricaMovimento) Carica(_idmovtaskinfermieristico);
                    }
                    else
                        bReturn = false;
                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovTaskInfermieristico.Salva()" + Environment.NewLine + ex.Message, ex);

            }

            return bReturn;

        }

        public List<IntervalloTempi> AnteprimaPeriodicita()
        {

            List<IntervalloTempi> listaperiodicita = new List<IntervalloTempi>();
            DateTime dataperiodicita = DateTime.MinValue;


            if (this.CodProtocollo == "")
            {
                if (this.DataProgrammata != DateTime.MinValue && this.PeriodicitaDataFine != DateTime.MinValue)
                {
                    DateTime orainizio = new DateTime(this.DataProgrammata.Year, this.DataProgrammata.Month, this.DataProgrammata.Day, this.DataProgrammata.Hour, this.DataProgrammata.Minute, 0);
                    DateTime orafine = new DateTime(this.PeriodicitaDataFine.Year, this.PeriodicitaDataFine.Month, this.PeriodicitaDataFine.Day, this.PeriodicitaDataFine.Hour, this.PeriodicitaDataFine.Minute, 0);

                    listaperiodicita.Add(new IntervalloTempi(orainizio, orainizio.AddMinutes(this.Durata), "", ""));

                    if (this.PeriodicitaGiorni > 0 || this.PeriodicitaOre > 0 || this.PeriodicitaMinuti > 0)
                    {
                        dataperiodicita = orainizio.AddDays(this.PeriodicitaGiorni).AddHours(this.PeriodicitaOre).AddMinutes(this.PeriodicitaMinuti);

                        while (dataperiodicita <= orafine)
                        {
                            listaperiodicita.Add(new IntervalloTempi(dataperiodicita, dataperiodicita.AddMinutes(this.Durata), "", ""));
                            dataperiodicita = dataperiodicita.AddDays(this.PeriodicitaGiorni).AddHours(this.PeriodicitaOre).AddMinutes(this.PeriodicitaMinuti);
                        }
                    }
                }
            }
            else
            {
                Parametri op = null;
                SqlParameterExt[] spcoll;
                string xmlParam = "";

                op = new Parametri(this.Ambiente);
                op.Parametro.Add("CodProtocollo", this.CodProtocollo);

                spcoll = new SqlParameterExt[1];

                xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelProtocolliTempi", spcoll);
                if (dt != null && dt.Rows.Count > 0)
                {

                    if (dt.Rows[0]["CodTipoProtocollo"].ToString() == EnumTipoProtocollo.DELTA.ToString())
                    {
                        int delta = 0;
                        foreach (DataRow orow in dt.Rows)
                        {
                            try
                            {
                                delta = int.Parse(orow["Delta"].ToString());
                            }
                            catch
                            {
                                delta = 0;
                            }

                            if (delta > 0)
                            {
                                dataperiodicita = this.DataProgrammata.AddMinutes(int.Parse(orow["Delta"].ToString()));
                                listaperiodicita.Add(new IntervalloTempi(dataperiodicita, dataperiodicita.AddMinutes(this.Durata), orow["DescProtocollo"].ToString(), orow["DescTempo"].ToString()));
                            }
                        }
                    }
                    else if (dt.Rows[0]["CodTipoProtocollo"].ToString() == EnumTipoProtocollo.ORA.ToString())
                    {

                        dataperiodicita = this.DataProgrammata;
                        DateTime ora = DateTime.MinValue;
                        while (dataperiodicita <= this.PeriodicitaDataFine)
                        {

                            foreach (DataRow orow in dt.Rows)
                            {

                                try
                                {
                                    ora = DateTime.Parse(orow["Ora"].ToString());
                                }
                                catch (Exception)
                                {
                                    ora = DateTime.MinValue;
                                }

                                if (ora != DateTime.MinValue)
                                {

                                    dataperiodicita = new DateTime(dataperiodicita.Year, dataperiodicita.Month, dataperiodicita.Day,
                                                                    ora.Hour, ora.Minute, 0);

                                    if (dataperiodicita >= this.DataProgrammata && dataperiodicita <= this.PeriodicitaDataFine)
                                    {

                                        listaperiodicita.Add(new IntervalloTempi(dataperiodicita, dataperiodicita.AddMinutes(this.Durata), orow["DescProtocollo"].ToString(), orow["DescTempo"].ToString()));

                                    }

                                }

                            }
                            dataperiodicita = new DateTime(dataperiodicita.Year, dataperiodicita.Month, dataperiodicita.Day, 0, 0, 0).AddDays(1);

                        }

                    }

                }

            }

            return listaperiodicita;

        }

        public bool CopiaDaOrigine(ref MovTaskInfermieristico movtiorigine)
        {
            return CopiaDaOrigine(ref movtiorigine, false);
        }
        public bool CopiaDaOrigine(ref MovTaskInfermieristico movtiorigine, bool copyall)
        {
            bool bReturn = true;

            try
            {
                _codua = movtiorigine.CodUA;
                _idmovtaskinfermieristico = string.Empty;
                if (copyall)
                {
                    _idmovtaskinfermieristico = movtiorigine.IDMovTaskInfermieristico;
                }
                _idepisodio = movtiorigine.IDEpisodio;
                _idtrasferimento = movtiorigine.IDTrasferimento;
                _dataevento = movtiorigine.DataEvento;
                _codsistema = movtiorigine.CodSistema;
                _idsistema = movtiorigine.IDSistema;
                _idgruppo = movtiorigine.IDGruppo;
                _codtipotaskinfermieristico = movtiorigine.CodTipoTaskInfermieristico;
                _codstatotaskinfermieristico = Enum.GetName(typeof(EnumStatoTaskInfermieristico), EnumStatoTaskInfermieristico.PR);
                _dataprogrammata = movtiorigine.DataProgrammata;
                _codtiporegistrazione = Enum.GetName(typeof(EnumTipoRegistrazione), EnumTipoRegistrazione.M);
                _descrizioneFUT = movtiorigine.DescrizioneFUT;
                _codutenterilevazione = this.Ambiente.Codlogin;

                _sottoclasse = movtiorigine.Sottoclasse;
                _posologiaeffettiva = movtiorigine.PosologiaEffettiva;
                _alert = movtiorigine.Alert;
                _barcode = movtiorigine.Barcode;

                this.MovScheda = new MovScheda(movtiorigine.MovScheda.CodScheda,
                                                    (EnumEntita)Enum.Parse(typeof(EnumEntita), movtiorigine.MovScheda.CodEntita),
                                                    movtiorigine.MovScheda.CodUA, movtiorigine.MovScheda.IDPaziente,
                                                    movtiorigine.MovScheda.IDEpisodio, movtiorigine.MovScheda.IDTrasferimento, this.Ambiente);
                this.MovScheda.CopiaDaOrigine(movtiorigine.MovScheda, 1);
            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovTaskInfermieristico.CopiaDaOrigine()" + Environment.NewLine + ex.Message, ex);

            }

            return bReturn;
        }

        public bool CopiaInDestinazione(ref MovTaskInfermieristico movtidestinazione)
        {
            return CopiaInDestinazione(ref movtidestinazione, false);
        }
        public bool CopiaInDestinazione(ref MovTaskInfermieristico movtidestinazione, bool copyall)
        {
            bool bReturn = true;

            try
            {
                movtidestinazione.CodUA = _codua;
                movtidestinazione.IDMovTaskInfermieristico = string.Empty;
                if (copyall)
                {
                    movtidestinazione.IDMovTaskInfermieristico = _idmovtaskinfermieristico;
                }
                movtidestinazione.IDEpisodio = _idepisodio;
                movtidestinazione.IDTrasferimento = _idtrasferimento;
                movtidestinazione.DataEvento = _dataevento;
                movtidestinazione.CodSistema = _codsistema;
                movtidestinazione.IDSistema = _idsistema;
                movtidestinazione.IDGruppo = _idgruppo;
                movtidestinazione.CodTipoTaskInfermieristico = _codtipotaskinfermieristico;
                movtidestinazione.CodStatoTaskInfermieristico = _codstatotaskinfermieristico;
                movtidestinazione.DataProgrammata = _dataprogrammata;
                movtidestinazione.CodTipoRegistrazione = _codtiporegistrazione;
                movtidestinazione.DescrizioneFUT = _descrizioneFUT;

                movtidestinazione.CodUtenteRilevazione = this.Ambiente.Codlogin;

                movtidestinazione.Sottoclasse = _sottoclasse;
                movtidestinazione.PosologiaEffettiva = _posologiaeffettiva;
                movtidestinazione.Alert = _alert;
                movtidestinazione.Barcode = _barcode;

                movtidestinazione.CodScheda = this.MovScheda.CodScheda;
                movtidestinazione.MovScheda = new MovScheda(this.MovScheda.CodScheda,
                                                    (EnumEntita)Enum.Parse(typeof(EnumEntita), this.MovScheda.CodEntita),
                                                    this.MovScheda.CodUA, this.MovScheda.IDPaziente,
                                                    this.MovScheda.IDEpisodio, this.MovScheda.IDTrasferimento, this.Ambiente);
                movtidestinazione.MovScheda.CopiaDaOrigine(this.MovScheda, 1);
            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovTaskInfermieristico.CopiaInDestinazione()" + Environment.NewLine + ex.Message, ex);

            }

            return bReturn;
        }

        public bool CopiaInStatoTrasmesso(string snote)
        {

            bool bReturn = true;

            try
            {

                MovTaskInfermieristico movTIDestinazione = new MovTaskInfermieristico(this.CodUA, this.IDPaziente, IDEpisodio, IDTrasferimento,
                                                                                    (EnumCodSistema)Enum.Parse(typeof(EnumCodSistema), this.CodSistema),
                                                                                    (EnumTipoRegistrazione)Enum.Parse(typeof(EnumTipoRegistrazione), this.CodTipoRegistrazione),
                                                                                    this.Ambiente);

                movTIDestinazione.CodUA = _codua;
                movTIDestinazione.IDEpisodio = IDEpisodio;
                movTIDestinazione.IDTrasferimento = IDTrasferimento;
                movTIDestinazione.DataEvento = _dataevento;
                movTIDestinazione.CodSistema = _codsistema;
                movTIDestinazione.IDSistema = _idsistema;
                movTIDestinazione.IDGruppo = _idgruppo;
                movTIDestinazione.CodTipoTaskInfermieristico = _codtipotaskinfermieristico;
                movTIDestinazione.CodStatoTaskInfermieristico = Enum.GetName(typeof(EnumStatoTaskInfermieristico), EnumStatoTaskInfermieristico.TR);
                movTIDestinazione.DataProgrammata = _dataprogrammata;
                movTIDestinazione.CodTipoRegistrazione = _codtiporegistrazione;
                movTIDestinazione.DescrizioneFUT = _descrizioneFUT;
                movTIDestinazione.CodUtenteRilevazione = _codutenterilevazione;
                movTIDestinazione.Sottoclasse = this.Sottoclasse;
                movTIDestinazione.Note = this.Note;
                movTIDestinazione.PosologiaEffettiva = this.PosologiaEffettiva;
                movTIDestinazione.Alert = this.Alert;
                movTIDestinazione.Barcode = this.Barcode;

                if (movTIDestinazione.Note != string.Empty) { movTIDestinazione.Note += Environment.NewLine; }
                movTIDestinazione.Note += snote;


                movTIDestinazione.MovScheda = new MovScheda(this.MovScheda.CodScheda,
                                                    (EnumEntita)Enum.Parse(typeof(EnumEntita), this.MovScheda.CodEntita),
                                                    this.MovScheda.CodUA, this.MovScheda.IDPaziente,
                                                    IDEpisodio, IDTrasferimento, this.Ambiente);

                if (movTIDestinazione.MovScheda.CopiaDaOrigine(this.MovScheda, 1))
                {
                    movTIDestinazione.MovScheda.IDTrasferimento = IDTrasferimento;
                    movTIDestinazione.MovScheda.IDEpisodio = IDEpisodio;
                    bReturn = movTIDestinazione.Salva(false);
                }
                else
                {
                    bReturn = false;
                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovTaskInfermieristico.CopiaInStatoTrasmesso()" + Environment.NewLine + ex.Message, ex);

            }

            return bReturn;
        }

        public bool ReImpostaProgrammato()
        {

            try
            {

                this.Azione = EnumAzioni.MOD;
                this.CodStatoTaskInfermieristico = Enum.GetName(typeof(EnumStatoTaskInfermieristico), EnumStatoTaskInfermieristico.PR);

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idmovtaskinfermieristico);
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void resetValori()
        {
            _idmovtaskinfermieristico = string.Empty;
            _idepisodio = string.Empty;
            _idpaziente = string.Empty;
            _idtrasferimento = string.Empty;
            _dataevento = DateTime.MinValue;
            _codsistema = string.Empty;
            _idsistema = string.Empty;
            _idgruppo = string.Empty;
            _codtipotaskinfermieristico = string.Empty;
            _descrtipotaskinfermieristico = string.Empty;
            _anticipominutitipotaskinfermieristico = 0;
            _codstatotaskinfermieristico = string.Empty;
            _descrstatotaskinfermieristico = string.Empty;
            _codtiporegistrazione = string.Empty;
            _dataprogrammata = DateTime.MinValue;
            _dataerogazione = DateTime.MinValue;
            _note = string.Empty;
            _codutenterilevazione = string.Empty;
            _descrutenterilevazione = string.Empty;
            _codutenteultimamodifica = string.Empty;
            _descrutenteultimamodifica = string.Empty;
            _descrizioneFUT = string.Empty;
            _icona = null;
            _idscheda = string.Empty;
            _codscheda = string.Empty;
            _versionescheda = 0;
            _anteprimartf = string.Empty;
            _permessoBloccato = 0;
            _permessoModifica = 0;
            _permessoErogazione = 0;
            _permessoAnnulla = 0;
            _permessoCancella = 0;
            _permessoCopia = 0;
            _movScheda = null;
            _sottoclasse = string.Empty;
            _posologiaeffettiva = string.Empty;
            _alert = string.Empty;
            _barcode = string.Empty;
            this.SoloTestata = false;
        }

        private void Carica(string idmovtaskinfermieristico)
        {
            try
            {

                Parametri op = new Parametri(this.Ambiente);
                op.Parametro.Add("IDTaskInfermieristico", idmovtaskinfermieristico);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovTaskInfermieristici", spcoll);

                if (dt.Rows.Count > 0)
                {
                    resetValori();
                    _idmovtaskinfermieristico = dt.Rows[0]["ID"].ToString();
                    _idpaziente = dt.Rows[0]["IDPaziente"].ToString();
                    _idepisodio = dt.Rows[0]["IDEpisodio"].ToString();
                    _idtrasferimento = dt.Rows[0]["IDTrasferimento"].ToString();

                    if (!dt.Rows[0].IsNull("DataEvento")) _dataevento = (DateTime)dt.Rows[0]["DataEvento"];
                    if (!dt.Rows[0].IsNull("CodSistema")) _codsistema = dt.Rows[0]["CodSistema"].ToString();
                    if (!dt.Rows[0].IsNull("IDSistema")) _idsistema = dt.Rows[0]["IDSistema"].ToString();
                    if (!dt.Rows[0].IsNull("IDGruppo")) _idgruppo = dt.Rows[0]["IDGruppo"].ToString();
                    if (!dt.Rows[0].IsNull("CodStato")) _codstatotaskinfermieristico = dt.Rows[0]["CodStato"].ToString();
                    if (!dt.Rows[0].IsNull("DescStato")) _descrstatotaskinfermieristico = dt.Rows[0]["DescStato"].ToString();
                    if (!dt.Rows[0].IsNull("CodTipo")) _codtipotaskinfermieristico = dt.Rows[0]["CodTipo"].ToString();
                    if (!dt.Rows[0].IsNull("DescTipo")) _descrtipotaskinfermieristico = dt.Rows[0]["DescTipo"].ToString();
                    if (dt.Columns.Contains("Anticipo") && !dt.Rows[0].IsNull("Anticipo")) _anticipominutitipotaskinfermieristico = (int)dt.Rows[0]["Anticipo"];
                    if (!dt.Rows[0].IsNull("CodUtenteRilevazione")) _codutenterilevazione = dt.Rows[0]["CodUtenteRilevazione"].ToString();
                    if (!dt.Rows[0].IsNull("DescrUtente")) _descrutenterilevazione = dt.Rows[0]["DescrUtente"].ToString();
                    if (!dt.Rows[0].IsNull("DescrizioneFUT")) _descrizioneFUT = dt.Rows[0]["DescrizioneFUT"].ToString();

                    if (!dt.Rows[0].IsNull("CodUtenteUltimaModifica")) _codutenteultimamodifica = dt.Rows[0]["CodUtenteUltimaModifica"].ToString();
                    if (!dt.Rows[0].IsNull("DescrUtenteUltimaModifica")) _descrutenteultimamodifica = dt.Rows[0]["DescrUtenteUltimaModifica"].ToString();

                    if (!dt.Rows[0].IsNull("CodTipoRegistrazione")) _codtiporegistrazione = dt.Rows[0]["CodTipoRegistrazione"].ToString();

                    if (!dt.Rows[0].IsNull("DataProgrammata")) _dataprogrammata = (DateTime)dt.Rows[0]["DataProgrammata"];
                    if (!dt.Rows[0].IsNull("DataErogazione")) _dataerogazione = (DateTime)dt.Rows[0]["DataErogazione"];

                    if (!dt.Rows[0].IsNull("Note")) _note = dt.Rows[0]["Note"].ToString();

                    if (!dt.Rows[0].IsNull("Icona")) _icona = (byte[])dt.Rows[0]["Icona"];

                    if (!dt.Rows[0].IsNull("IDScheda")) _idscheda = dt.Rows[0]["IDScheda"].ToString();
                    if (!dt.Rows[0].IsNull("CodScheda")) _codscheda = dt.Rows[0]["CodScheda"].ToString();
                    if (!dt.Rows[0].IsNull("Versione")) _versionescheda = (int)dt.Rows[0]["Versione"];

                    if (!dt.Rows[0].IsNull("AnteprimaRTF")) _anteprimartf = dt.Rows[0]["AnteprimaRTF"].ToString();

                    if (!dt.Rows[0].IsNull("PermessoBloccato")) _permessoBloccato = (int)dt.Rows[0]["PermessoBloccato"];
                    if (!dt.Rows[0].IsNull("PermessoModifica")) _permessoModifica = (int)dt.Rows[0]["PermessoModifica"];
                    if (!dt.Rows[0].IsNull("PermessoErogazione")) _permessoErogazione = (int)dt.Rows[0]["PermessoErogazione"];
                    if (!dt.Rows[0].IsNull("PermessoAnnulla")) _permessoAnnulla = (int)dt.Rows[0]["PermessoAnnulla"];
                    if (!dt.Rows[0].IsNull("PermessoCancella")) _permessoCancella = (int)dt.Rows[0]["PermessoCancella"];
                    if (!dt.Rows[0].IsNull("PermessoCopia")) _permessoCopia = (int)dt.Rows[0]["PermessoCopia"];

                    if (!dt.Rows[0].IsNull("Sottoclasse")) _sottoclasse = dt.Rows[0]["Sottoclasse"].ToString();

                    if (!dt.Rows[0].IsNull("codUA")) _codua = dt.Rows[0]["CodUA"].ToString();

                    if (!dt.Rows[0].IsNull("PosologiaEffettiva")) _posologiaeffettiva = dt.Rows[0]["PosologiaEffettiva"].ToString();
                    if (dt.Columns.Contains("Alert") && !dt.Rows[0].IsNull("Alert")) _alert = dt.Rows[0]["Alert"].ToString();
                    if (dt.Columns.Contains("Barcode") && !dt.Rows[0].IsNull("Barcode")) _barcode = dt.Rows[0]["Barcode"].ToString();

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private void GeneraPeriodicita(List<IntervalloTempi> listaperiodicita)
        {
            this.GeneraPeriodicita(listaperiodicita, true);
        }
        private void GeneraPeriodicita(List<IntervalloTempi> listaperiodicita, bool FlagGeneraRTF)
        {

            bool bFirst = false;

            if (listaperiodicita.Count > 0)
            {
                foreach (IntervalloTempi dataevento in listaperiodicita)
                {
                    if (bFirst == true)
                    {
                        MovTaskInfermieristico movtidestinazione = new MovTaskInfermieristico(_codua, _idpaziente, _idepisodio, _idtrasferimento,
                                                                                       (EnumCodSistema)Enum.Parse(typeof(EnumCodSistema), _codsistema),
                                                                                       (EnumTipoRegistrazione)Enum.Parse(typeof(EnumTipoRegistrazione), _codtiporegistrazione), this.Ambiente);
                        this.CopiaInDestinazione(ref movtidestinazione);
                        movtidestinazione.DataProgrammata = dataevento.DataOraInizio;
                        movtidestinazione.Salva(true, FlagGeneraRTF);
                    }
                    bFirst = true;

                }
            }

        }

    }
}
