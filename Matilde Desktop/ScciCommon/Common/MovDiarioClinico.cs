using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Framework.Data;
using System.Web.UI;
using UnicodeSrl.DatiClinici.DC;

namespace UnicodeSrl.Scci
{

    [Serializable()]
    public class MovDiarioClinico
    {

        private string _codua = string.Empty;
        private string _idmovdiario = string.Empty;
        private string _idpaziente = string.Empty;
        private string _idepisodio = string.Empty;
        private string _idtrasferimento = string.Empty;
        private string _codtipodiario = string.Empty;
        private string _codtipovocediario = string.Empty;
        private string _codtiporegistrazione = string.Empty;
        private string _codstatodiario = string.Empty;
        private string _codutenterilevazione = string.Empty;
        private DateTime _dataevento = DateTime.MinValue;
        private DateTime _datainserimento = DateTime.MinValue;
        private DateTime _datavalidazione = DateTime.MinValue;
        private DateTime _dataannullamento = DateTime.MinValue;

        private string _descrtipodiario = string.Empty;
        private string _descrtipovocediario = string.Empty;
        private string _descrtiporegistrazione = string.Empty;
        private string _descrstatodiario = string.Empty;
        private string _descrutenterilevazione = string.Empty;

        private string _codsistema = string.Empty;
        private string _idsistema = string.Empty;

        private int _permessoValida = 0;
        private int _permessoModifica = 0;
        private int _permessoCopia = 0;
        private int _permessoAnnulla = 0;
        private int _permessoCancella = 0;
        private int _permessoUAFirma = 0;

        private byte[] _icona;

        private int _idIcona = 0;

        private string _codentitaregistrazione = string.Empty;
        private string _identitaregistrazione = string.Empty;

        private string _idscheda = string.Empty;
        private string _codscheda = string.Empty;
        private int _versionescheda = 0;

        private EnumAzioni _azione = EnumAzioni.MOD;

        private MovScheda _movScheda = null;

        public enum enumPopolaDaPrecedenteReturn
        {
            popolato_correttamente = 0,
            precedente_non_trovato = 1,
            errori = 2
        }

        public MovDiarioClinico(string idmovdiarioclinico, Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = EnumAzioni.MOD;
            _idmovdiario = idmovdiarioclinico;
            this.Carica(idmovdiarioclinico);
        }
        public MovDiarioClinico(string idmovdiarioclinico, EnumAzioni azione, Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = azione;
            _idmovdiario = idmovdiarioclinico;
            this.Carica(idmovdiarioclinico);
        }
        public MovDiarioClinico(string codua, string idpaziente, string idepisodio, string idtrasferimento, Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = EnumAzioni.INS;
            _codua = codua;
            _idpaziente = idpaziente;
            _idepisodio = idepisodio;
            _idtrasferimento = idtrasferimento;
            _idmovdiario = "";
            _codscheda = "";
            _movScheda = null;
        }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string CodUA
        {
            get { return _codua; }
        }

        public string IDMovDiario
        {
            get { return _idmovdiario; }
            set
            {
                if (_idmovdiario != value && value != "") _movScheda = null;
                _idmovdiario = value;
            }
        }

        public string IDPaziente
        {
            get { return _idpaziente; }
        }

        public string IDEpisodio
        {
            get { return _idepisodio; }
        }

        public string IDTrasferimento
        {
            get { return _idtrasferimento; }
        }

        public string IDScheda
        {
            get { return _idscheda; }
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
                    if (_idmovdiario != null && _idmovdiario != string.Empty && _idmovdiario.Trim() != "")
                    {
                        _movScheda = new MovScheda(EnumEntita.DCL.ToString(), _idmovdiario, this.Ambiente);
                    }
                    else if (_codscheda != null && _codscheda != string.Empty && _codscheda.Trim() != "")
                    {
                        _movScheda = new MovScheda(_codscheda, EnumEntita.DCL, _codua, _idpaziente, _idepisodio, _idtrasferimento, this.Ambiente);
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
            set
            {
                _movScheda = value;

            }


        }

        public DateTime DataEvento
        {
            get { return _dataevento; }
            set { _dataevento = value; }
        }

        public DateTime DataInserimento
        {
            get { return _datainserimento; }
        }

        public DateTime DataValidazione
        {
            get { return _datavalidazione; }
            set { _datavalidazione = value; }
        }

        public DateTime DataAnnullamento
        {
            get { return _dataannullamento; }
            set { _dataannullamento = value; }
        }

        public string CodUtenteRilevazione
        {
            get { return _codutenterilevazione; }
            set { _codutenterilevazione = value; }
        }

        public string CodStatoDiario
        {
            get { return _codstatodiario; }
            set { _codstatodiario = value; }
        }

        public string CodTipoDiario
        {
            get { return _codtipodiario; }
        }

        public string CodTipoRegistrazione
        {
            get { return _codtiporegistrazione; }
            set
            {
                _codtiporegistrazione = value;

            }
        }

        public string CodTipoVoceDiario
        {
            get { return _codtipovocediario; }
            set
            {
                if (_codtipovocediario != value) _movScheda = null;
                _codtipovocediario = value;
            }
        }

        public string DescrStatoDiario
        {
            get { return _descrstatodiario; }
        }

        public string DescrTipoDiario
        {
            get { return _descrtipodiario; }
        }

        public string CodEntitaRegistrazione
        {
            get { return _codentitaregistrazione; }
            set { _codentitaregistrazione = value; }
        }

        public string IdEntitaRegistrazione
        {
            get { return _identitaregistrazione; }
            set { _identitaregistrazione = value; }
        }

        public string DescrTipoVoceDiario
        {
            get { return _descrtipovocediario; }
            set { _descrtipovocediario = value; }
        }

        public string DescrUtenteRilevazione
        {
            get { return _descrutenterilevazione; }
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

        public int PermessoValida
        {
            get { return _permessoValida; }
            set { _permessoValida = value; }
        }

        public int PermessoModifica
        {
            get { return _permessoModifica; }
            set { _permessoModifica = value; }
        }

        public int PermessoUAFirma
        {
            get { return _permessoUAFirma; }
            set { _permessoUAFirma = value; }
        }

        public int PermessoCopia
        {
            get { return _permessoCopia; }
            set { _permessoCopia = value; }
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

        public byte[] Icona
        {
            get { return _icona; }
        }

        public int IDIcona
        {
            get { return _idIcona; }
        }

        public ScciAmbiente Ambiente { get; set; }

        public string PathFileTemp { get; set; }

        private void resetValori()
        {
            _codua = string.Empty;
            _idpaziente = string.Empty;
            _idepisodio = string.Empty;
            _idtrasferimento = string.Empty;
            _codtipodiario = string.Empty;
            _codtipovocediario = string.Empty;
            _codtiporegistrazione = string.Empty;
            _codstatodiario = string.Empty;
            _codutenterilevazione = string.Empty;
            _dataevento = DateTime.MinValue;
            _datainserimento = DateTime.MinValue;
            _datavalidazione = DateTime.MinValue;
            _dataannullamento = DateTime.MinValue;

            _descrtipodiario = string.Empty;
            _descrtipovocediario = string.Empty;
            _descrtiporegistrazione = string.Empty;
            _descrstatodiario = string.Empty;
            _descrutenterilevazione = string.Empty;

            _codsistema = string.Empty;
            _idsistema = string.Empty;

            _permessoValida = 0;
            _permessoModifica = 0;
            _permessoCopia = 0;
            _permessoAnnulla = 0;
            _permessoCancella = 0;
            _permessoUAFirma = 0;

            _icona = null;
            _idIcona = 0;

            _idscheda = string.Empty;
            _codscheda = string.Empty;
            _versionescheda = 0;
            _movScheda = null;
        }

        private void Carica(string idmovdiarioclinico)
        {
            try
            {

                Parametri op = null;
                op = new Parametri(this.Ambiente);
                op.Parametro.Add("IDDiarioClinico", idmovdiarioclinico);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovDiarioClinico", spcoll);

                if (dt.Rows.Count > 0)
                {
                    resetValori();

                    _idpaziente = dt.Rows[0]["IDPaziente"].ToString();
                    _idepisodio = dt.Rows[0]["IDEpisodio"].ToString();
                    _idtrasferimento = dt.Rows[0]["IDTrasferimento"].ToString();

                    if (!dt.Rows[0].IsNull("CodUA")) _codua = dt.Rows[0]["CodUA"].ToString();
                    if (!dt.Rows[0].IsNull("CodTipoDiario")) _codtipodiario = dt.Rows[0]["CodTipoDiario"].ToString();
                    if (!dt.Rows[0].IsNull("DescrTipoDiario")) _descrtipodiario = dt.Rows[0]["DescrTipoDiario"].ToString();
                    if (!dt.Rows[0].IsNull("CodTipo")) _codtipovocediario = dt.Rows[0]["CodTipo"].ToString();
                    if (!dt.Rows[0].IsNull("DescrTipo")) _descrtipovocediario = dt.Rows[0]["DescrTipo"].ToString();
                    if (!dt.Rows[0].IsNull("CodTipoRegistrazione")) _codtiporegistrazione = dt.Rows[0]["CodTipoRegistrazione"].ToString();
                    if (!dt.Rows[0].IsNull("DescrTipoRegistrazione")) _descrtiporegistrazione = dt.Rows[0]["DescrTipoRegistrazione"].ToString();
                    if (!dt.Rows[0].IsNull("CodStato")) _codstatodiario = dt.Rows[0]["CodStato"].ToString();
                    if (!dt.Rows[0].IsNull("DescrStato")) _descrstatodiario = dt.Rows[0]["DescrStato"].ToString();
                    if (!dt.Rows[0].IsNull("CodUtente")) _codutenterilevazione = dt.Rows[0]["CodUtente"].ToString();
                    if (!dt.Rows[0].IsNull("DescrUtente")) _descrutenterilevazione = dt.Rows[0]["DescrUtente"].ToString();

                    if (!dt.Rows[0].IsNull("DataEvento")) _dataevento = (DateTime)dt.Rows[0]["DataEvento"];
                    if (!dt.Rows[0].IsNull("DataInserimento")) _datainserimento = (DateTime)dt.Rows[0]["DataInserimento"];
                    if (!dt.Rows[0].IsNull("DataValidazione")) _datavalidazione = (DateTime)dt.Rows[0]["DataValidazione"];
                    if (!dt.Rows[0].IsNull("DataAnnullamento")) _dataannullamento = (DateTime)dt.Rows[0]["DataAnnullamento"];

                    if (!dt.Rows[0].IsNull("CodSistema")) _codsistema = dt.Rows[0]["CodSistema"].ToString();
                    if (!dt.Rows[0].IsNull("IDSistema")) _idsistema = dt.Rows[0]["IDSistema"].ToString();

                    if (!dt.Rows[0].IsNull("PermessoValida")) _permessoValida = (int)dt.Rows[0]["PermessoValida"];
                    if (!dt.Rows[0].IsNull("PermessoCancella")) _permessoCancella = (int)dt.Rows[0]["PermessoCancella"];
                    if (!dt.Rows[0].IsNull("PermessoCopia")) _permessoCopia = (int)dt.Rows[0]["PermessoCopia"];
                    if (!dt.Rows[0].IsNull("PermessoAnnulla")) _permessoAnnulla = (int)dt.Rows[0]["PermessoAnnulla"];
                    if (!dt.Rows[0].IsNull("PermessoModifica")) _permessoModifica = (int)dt.Rows[0]["PermessoModifica"];
                    if (dt.Columns.Contains("PermessoUAFirma") && !dt.Rows[0].IsNull("PermessoUAFirma")) _permessoUAFirma = (int)dt.Rows[0]["PermessoUAFirma"];

                    if (!dt.Rows[0].IsNull("Icona")) _icona = (byte[])dt.Rows[0]["Icona"];
                    if (!dt.Rows[0].IsNull("IDIcona")) _idIcona = Convert.ToInt32(dt.Rows[0]["IDIcona"]);

                    if (!dt.Rows[0].IsNull("IDScheda")) _idscheda = dt.Rows[0]["IDScheda"].ToString();
                    if (!dt.Rows[0].IsNull("CodScheda")) _codscheda = dt.Rows[0]["CodScheda"].ToString();
                    if (!dt.Rows[0].IsNull("Versione")) _versionescheda = (int)dt.Rows[0]["Versione"];

                    if (!dt.Rows[0].IsNull("CodEntitaRegistrazione")) _codentitaregistrazione = dt.Rows[0]["CodEntitaRegistrazione"].ToString();
                    if (!dt.Rows[0].IsNull("IdEntitaRegistrazione")) _identitaregistrazione = dt.Rows[0]["IdEntitaRegistrazione"].ToString();


                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public MovDiarioClinico Copia()
        {
            MovDiarioClinico mdcCopia = null;
            if (_idmovdiario != string.Empty && _idmovdiario.Trim() != "")
            {
                mdcCopia = new MovDiarioClinico(_idmovdiario, this.Ambiente);
                string test = mdcCopia.MovScheda.IDEntita;

                mdcCopia.IDMovDiario = "";
                mdcCopia.Azione = EnumAzioni.INS;
                mdcCopia.CodStatoDiario = "VA";
                mdcCopia.DataAnnullamento = DateTime.MinValue;
                mdcCopia.DataValidazione = DateTime.MinValue;

                mdcCopia.MovScheda.IDEntita = "";
                mdcCopia.MovScheda.IDMovScheda = "";
                mdcCopia.MovScheda.Azione = EnumAzioni.INS;
                mdcCopia.MovScheda.CodStatoScheda = "IC";
            }
            return mdcCopia;
        }

        public enumPopolaDaPrecedenteReturn PopolaDaPrecedente(bool showUI)
        {
            enumPopolaDaPrecedenteReturn ret = enumPopolaDaPrecedenteReturn.popolato_correttamente;

            try
            {

                string idMovDiarioClinicoPrecedente = "";

                Parametri op = null;
                op = new Parametri(this.Ambiente);
                op.Parametro.Add("CodTipoVoceDiario", this.CodTipoVoceDiario);
                op.Parametro.Add("IDCartella", this.Ambiente.IdCartella);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_CercaDiarioClinicoPrecedente", spcoll);

                if (dt.Rows.Count > 0 && !dt.Rows[0].IsNull("IDMovDiarioClinico")) idMovDiarioClinicoPrecedente = dt.Rows[0]["IDMovDiarioClinico"].ToString().Trim();

                if (idMovDiarioClinicoPrecedente == "")
                {
                    ret = enumPopolaDaPrecedenteReturn.precedente_non_trovato;
                }
                else
                {

                    MovDiarioClinico movdiarioprec = new MovDiarioClinico(idMovDiarioClinicoPrecedente, this.Ambiente);


                    _codtipodiario = movdiarioprec.CodTipoDiario;
                    _codtipovocediario = movdiarioprec.CodTipoVoceDiario;
                    this.CodEntitaRegistrazione = string.Empty;
                    this.IdEntitaRegistrazione = string.Empty;
                    this.CodUtenteRilevazione = this.Ambiente.Codlogin;
                    this.CodSistema = string.Empty;
                    this.IDSistema = string.Empty;


                    _codscheda = movdiarioprec.CodScheda;

                    this.MovScheda.CodStatoScheda = Enum.GetName(typeof(EnumStatoScheda), EnumStatoScheda.IC);
                    Gestore oGestorePrec = CommonStatics.GetGestore(this.Ambiente);
                    oGestorePrec.SchedaXML = movdiarioprec.MovScheda.Scheda.StrutturaXML;
                    oGestorePrec.SchedaLayoutsXML = movdiarioprec.MovScheda.Scheda.LayoutXML;
                    oGestorePrec.Decodifiche = movdiarioprec.MovScheda.Scheda.DizionarioValori();
                    oGestorePrec.SchedaDatiXML = movdiarioprec.MovScheda.DatiXML;

                    Gestore oGestoreCorr = CommonStatics.GetGestore(this.Ambiente);
                    oGestoreCorr.SchedaXML = this.MovScheda.Scheda.StrutturaXML;
                    oGestoreCorr.SchedaLayoutsXML = this.MovScheda.Scheda.LayoutXML;
                    oGestoreCorr.Decodifiche = this.MovScheda.Scheda.DizionarioValori();
                    oGestoreCorr.SchedaDatiXML = this.MovScheda.DatiXML;

                    foreach (DcDato dato in oGestorePrec.SchedaDati.Dati.Values)
                    {

                        DcVoce voce = oGestoreCorr.LeggeVoce(dato.ID);
                        if (voce != null && String.IsNullOrEmpty(voce.Default))
                        {
                            oGestoreCorr.ModificaValore(dato.Key, dato.Value);
                        }

                    }
                    this.MovScheda.DatiXML = oGestoreCorr.SchedaDatiXML;

                    this.MovScheda.AnteprimaRTF = movdiarioprec.MovScheda.AnteprimaRTF;
                    this.MovScheda.DatiObbligatoriMancantiRTF = movdiarioprec.MovScheda.DatiObbligatoriMancantiRTF;
                    this.MovScheda.DatiRilievoRTF = movdiarioprec.MovScheda.DatiRilievoRTF;
                    this.MovScheda.Azione = EnumAzioni.INS;
                }



            }
            catch (Exception ex)
            {
                ret = enumPopolaDaPrecedenteReturn.errori;
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return ret;
        }

        public bool Annulla()
        {
            try
            {
                CodStatoDiario = "AN";
                Azione = EnumAzioni.ANN;

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idmovdiario);
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

        public bool Cancella()
        {
            try
            {
                CodStatoDiario = "CA";
                Azione = EnumAzioni.CAN;

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idmovdiario);
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

        public bool Valida()
        {
            try
            {
                CodStatoDiario = "VA";
                Azione = EnumAzioni.VAL;

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idmovdiario);
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
            try
            {
                bool bReturn = true;
                Parametri op = null;
                SqlParameterExt[] spcoll;
                string xmlParam = "";
                if (_idmovdiario != string.Empty && _idmovdiario.Trim() != "")
                {
                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDDiarioClinico", _idmovdiario);
                    op.Parametro.Add("CodTipoVoceDiario", _codtipovocediario);
                    op.Parametro.Add("CodStatoDiario", _codstatodiario);

                    op.Parametro.Add("DataEvento", Database.dataOra105PerParametri(_dataevento));
                    op.Parametro.Add("DataEventoUTC", Database.dataOra105PerParametri(_dataevento.ToUniversalTime()));




                    if (_azione == EnumAzioni.VAL)
                    {
                        _datavalidazione = DateTime.Now;
                        op.Parametro.Add("DataValidazione", Database.dataOra105PerParametri(_datavalidazione));
                        op.Parametro.Add("DataValidazioneUTC", Database.dataOra105PerParametri(_datavalidazione.ToUniversalTime()));

                    }
                    if (_azione == EnumAzioni.ANN)
                    {
                        _dataannullamento = DateTime.Now;
                        op.Parametro.Add("DataAnnullamento", Database.dataOra105PerParametri(_dataannullamento));
                        op.Parametro.Add("DataAnnullamentoUTC", Database.dataOra105PerParametri(_dataannullamento.ToUniversalTime()));

                        this.MovScheda.Azione = EnumAzioni.ANN;
                    }
                    if (_azione == EnumAzioni.CAN)
                        this.MovScheda.Azione = EnumAzioni.CAN;

                    op.TimeStamp.CodEntita = EnumEntita.DCL.ToString();

                    op.TimeStamp.IDEntita = _idmovdiario;
                    op.TimeStamp.CodAzione = _azione.ToString();

                    op.TimeStamp.IDEpisodio = _idepisodio;
                    op.TimeStamp.IDTrasferimento = _idtrasferimento;
                    op.TimeStamp.IDPaziente = _idpaziente;

                    op.MovScheda = this.MovScheda;

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    Database.ExecStoredProc("MSP_AggMovDiarioClinico", spcoll);

                    this.MovScheda.Salva();

                    Carica(_idmovdiario);
                }
                else
                {

                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("CodUA", _codua);
                    op.Parametro.Add("IDEpisodio", _idepisodio);
                    op.Parametro.Add("IDTrasferimento", _idtrasferimento);
                    op.Parametro.Add("CodTipoVoceDiario", _codtipovocediario);


                    if (_codtiporegistrazione != string.Empty)
                        op.Parametro.Add("CodTipoRegistrazione", _codtiporegistrazione);

                    if (_identitaregistrazione != string.Empty)
                        op.Parametro.Add("IDEntitaRegistrazione", _identitaregistrazione);

                    if (_codutenterilevazione != string.Empty)
                        op.Parametro.Add("CodUtenteRilevazione", _codutenterilevazione);

                    if (_codentitaregistrazione != string.Empty)
                        op.Parametro.Add("CodEntitaRegistrazione", _codentitaregistrazione);
                    else
                        op.Parametro.Add("CodEntitaRegistrazione", EnumEntita.DCL.ToString());

                    op.Parametro.Add("DataEvento", Database.dataOra105PerParametri(_dataevento));
                    op.Parametro.Add("DataEventoUTC", Database.dataOra105PerParametri(_dataevento.ToUniversalTime()));

                    if (_codsistema != string.Empty)
                        op.Parametro.Add("CodSistema", _codsistema);
                    if (_idsistema != string.Empty)
                        op.Parametro.Add("IDSistema", _idsistema);

                    op.TimeStamp.CodEntita = EnumEntita.DCL.ToString();
                    op.TimeStamp.CodAzione = _azione.ToString();
                    op.Parametro.Add("CodStatoDiario", _codstatodiario);
                    if (_azione == EnumAzioni.VAL || _codstatodiario == "VA")
                    {
                        _datavalidazione = DateTime.Now;
                        op.Parametro.Add("DataValidazione", Database.dataOra105PerParametri(_datavalidazione));
                        op.Parametro.Add("DataValidazioneUTC", Database.dataOra105PerParametri(_datavalidazione.ToUniversalTime()));
                    }

                    op.TimeStamp.IDEpisodio = _idepisodio;
                    op.TimeStamp.IDTrasferimento = _idtrasferimento;
                    op.TimeStamp.IDPaziente = _idpaziente;

                    op.MovScheda = this.MovScheda;

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovDiarioClinico", spcoll);
                    if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                    {
                        _idmovdiario = dt.Rows[0][0].ToString();

                        this.MovScheda.IDEntita = _idmovdiario;
                        this.MovScheda.Salva();

                        _azione = EnumAzioni.MOD;
                        Carica(_idmovdiario);
                    }
                }

                return bReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(@"MovDiarioClinico.Salva()" + Environment.NewLine + ex.Message, ex);
            }
        }

    }

}
