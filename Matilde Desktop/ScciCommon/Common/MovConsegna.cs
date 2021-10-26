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

namespace UnicodeSrl.Scci
{

    [Serializable()]
    public class MovConsegna
    {

        private string _codua = string.Empty;
        private string _descrua = string.Empty;
        private string _idmovconsegna = string.Empty;
        private string _codtipoconsegna = string.Empty;
        private string _codstatoconsegna = string.Empty;
        private string _codutenterilevazione = string.Empty;
        private DateTime _dataevento = DateTime.MinValue;
        private DateTime _datainserimento = DateTime.MinValue;
        private DateTime _dataultimamodifica = DateTime.MinValue;
        private DateTime _dataannullamento = DateTime.MinValue;

        private string _descrtipoconsegna = string.Empty;
        private string _descrstatoconsegna = string.Empty;
        private string _descrutenterilevazione = string.Empty;

        private int _permessoModifica = 0;
        private int _permessoAnnulla = 0;

        private byte[] _icona;

        private int _idIcona = 0;


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

        public MovConsegna(string idmovconsegna, Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = EnumAzioni.MOD;
            _idmovconsegna = idmovconsegna;
            this.Carica(idmovconsegna);
        }
        public MovConsegna(string idmovconsegna, EnumAzioni azione, Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = azione;
            _idmovconsegna = idmovconsegna;
            this.Carica(idmovconsegna);
        }
        public MovConsegna(Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = EnumAzioni.INS;
            _idmovconsegna = "";
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
            set { _codua = value; }
        }

        public string DescrUA
        {
            get { return _descrua; }
            set { _descrua = value; }
        }

        public string IDMovConsegna
        {
            get { return _idmovconsegna; }
            set
            {
                if (_idmovconsegna != value && value != "") _movScheda = null;
                _idmovconsegna = value;
            }
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
                    if (_idmovconsegna != null && _idmovconsegna != string.Empty && _idmovconsegna.Trim() != "")
                    {
                        _movScheda = new MovScheda(EnumEntita.CSG.ToString(), _idmovconsegna, this.Ambiente);
                    }
                    else if (_codscheda != null && _codscheda != string.Empty && _codscheda.Trim() != "")
                    {
                        _movScheda = new MovScheda(_codscheda, EnumEntita.CSG, _codua, "", "", "", this.Ambiente);
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

        public DateTime DataUltimaModifica
        {
            get { return _dataultimamodifica; }
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

        public string CodStatoConsegna
        {
            get { return _codstatoconsegna; }
            set { _codstatoconsegna = value; }
        }

        public string CodTipoConsegna
        {
            get { return _codtipoconsegna; }
            set { _codtipoconsegna = value; }
        }


        public string DescrStatoConsegna
        {
            get { return _descrstatoconsegna; }
        }

        public string DescrTipoConsegna
        {
            get { return _descrtipoconsegna; }
            set { _descrtipoconsegna = value; }
        }

        public string DescrUtenteRilevazione
        {
            get { return _descrutenterilevazione; }
        }

        public int PermessoModifica
        {
            get { return _permessoModifica; }
            set { _permessoModifica = value; }
        }

        public int PermessoAnnulla
        {
            get { return _permessoAnnulla; }
            set { _permessoAnnulla = value; }
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
            _descrua = string.Empty;
            _codtipoconsegna = string.Empty;
            _codstatoconsegna = string.Empty;
            _codutenterilevazione = string.Empty;
            _dataevento = DateTime.MinValue;
            _datainserimento = DateTime.MinValue;
            _dataultimamodifica = DateTime.MinValue;
            _dataannullamento = DateTime.MinValue;

            _descrtipoconsegna = string.Empty;
            _descrstatoconsegna = string.Empty;
            _descrutenterilevazione = string.Empty;

            _permessoModifica = 0;
            _permessoAnnulla = 0;

            _icona = null;
            _idIcona = 0;

            _idscheda = string.Empty;
            _codscheda = string.Empty;
            _versionescheda = 0;
            _movScheda = null;
        }

        private void Carica(string idmovconsegna)
        {
            try
            {

                Parametri op = null;
                op = new Parametri(this.Ambiente);
                op.Parametro.Add("IDConsegna", idmovconsegna);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovConsegne", spcoll);

                if (dt.Rows.Count > 0)
                {
                    resetValori();


                    if (!dt.Rows[0].IsNull("CodUA")) _codua = dt.Rows[0]["CodUA"].ToString();
                    if (!dt.Rows[0].IsNull("DescrUA")) _descrua = dt.Rows[0]["DescrUA"].ToString();
                    if (!dt.Rows[0].IsNull("CodTipoConsegna")) _codtipoconsegna = dt.Rows[0]["CodTipoConsegna"].ToString();
                    if (!dt.Rows[0].IsNull("DescrTipoConsegna")) _descrtipoconsegna = dt.Rows[0]["DescrTipoConsegna"].ToString();
                    if (!dt.Rows[0].IsNull("CodStatoConsegna")) _codstatoconsegna = dt.Rows[0]["CodStatoConsegna"].ToString();
                    if (!dt.Rows[0].IsNull("DescrStatoConsegna")) _descrstatoconsegna = dt.Rows[0]["DescrStatoConsegna"].ToString();
                    if (!dt.Rows[0].IsNull("CodUtenteRilevazione")) _codutenterilevazione = dt.Rows[0]["CodUtenteRilevazione"].ToString();
                    if (!dt.Rows[0].IsNull("DescrUtenteRilevazione")) _descrutenterilevazione = dt.Rows[0]["DescrUtenteRilevazione"].ToString();

                    if (!dt.Rows[0].IsNull("DataEvento")) _dataevento = (DateTime)dt.Rows[0]["DataEvento"];
                    if (!dt.Rows[0].IsNull("DataEvento")) _dataevento = (DateTime)dt.Rows[0]["DataEvento"];
                    if (!dt.Rows[0].IsNull("DataInserimento")) _datainserimento = (DateTime)dt.Rows[0]["DataInserimento"];
                    if (!dt.Rows[0].IsNull("DataUltimaModifica")) _dataultimamodifica = (DateTime)dt.Rows[0]["DataUltimaModifica"];
                    if (!dt.Rows[0].IsNull("DataAnnullamento")) _dataannullamento = (DateTime)dt.Rows[0]["DataAnnullamento"];

                    if (!dt.Rows[0].IsNull("PermessoAnnulla")) _permessoAnnulla = (int)dt.Rows[0]["PermessoAnnulla"];
                    if (!dt.Rows[0].IsNull("PermessoModifica")) _permessoModifica = (int)dt.Rows[0]["PermessoModifica"];

                    if (!dt.Rows[0].IsNull("Icona")) _icona = (byte[])dt.Rows[0]["Icona"];
                    if (!dt.Rows[0].IsNull("IDIcona")) _idIcona = Convert.ToInt32(dt.Rows[0]["IDIcona"]);

                    if (!dt.Rows[0].IsNull("IDScheda")) _idscheda = dt.Rows[0]["IDScheda"].ToString();
                    if (!dt.Rows[0].IsNull("CodScheda")) _codscheda = dt.Rows[0]["CodScheda"].ToString();
                    if (!dt.Rows[0].IsNull("Versione")) _versionescheda = (int)dt.Rows[0]["Versione"];

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public MovConsegna Copia()
        {
            MovConsegna mdcCopia = null;
            if (_idmovconsegna != string.Empty && _idmovconsegna.Trim() != "")
            {
                mdcCopia = new MovConsegna(_idmovconsegna, this.Ambiente);
                string test = mdcCopia.MovScheda.IDEntita;

                mdcCopia.IDMovConsegna = "";
                mdcCopia.Azione = EnumAzioni.INS;
                mdcCopia.CodStatoConsegna = "IC";
                mdcCopia.DataAnnullamento = DateTime.MinValue;

                mdcCopia.MovScheda.IDEntita = "";
                mdcCopia.MovScheda.IDMovScheda = "";
                mdcCopia.MovScheda.Azione = EnumAzioni.INS;
                mdcCopia.MovScheda.CodStatoScheda = "IC";
            }
            return mdcCopia;
        }

        public string CercaPrecedente()
        {
            string idMovConsegnaPrecedente = string.Empty;

            try
            {
                Parametri op = null;
                op = new Parametri(this.Ambiente);
                op.Parametro.Add("CodTipoConsegna", this.CodTipoConsegna);
                op.Parametro.Add("CodUA", this.CodUA);

                if (this.IDMovConsegna != string.Empty)
                {
                    op.Parametro.Add("IDMovConsegnaDaIgnorare", this.IDMovConsegna);
                }

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_CercaConsegnaPrecedente", spcoll);

                if (dt.Rows.Count > 0 && !dt.Rows[0].IsNull("IDMovConsegna"))
                {
                    idMovConsegnaPrecedente = dt.Rows[0]["IDMovConsegna"].ToString().Trim();
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return idMovConsegnaPrecedente;
        }
        public enumPopolaDaPrecedenteReturn PopolaDaPrecedente()
        {
            enumPopolaDaPrecedenteReturn ret = enumPopolaDaPrecedenteReturn.popolato_correttamente;

            try
            {

                string idMovConsegnaPrecedente = string.Empty;

                idMovConsegnaPrecedente = CercaPrecedente();

                if (idMovConsegnaPrecedente == string.Empty)
                {
                    ret = enumPopolaDaPrecedenteReturn.precedente_non_trovato;
                }
                else
                {

                    MovConsegna movconsegnaprec = new MovConsegna(idMovConsegnaPrecedente, this.Ambiente);

                    _codua = movconsegnaprec.CodUA;
                    _codtipoconsegna = movconsegnaprec.CodTipoConsegna;
                    this.CodUtenteRilevazione = this.Ambiente.Codlogin;

                    _codscheda = movconsegnaprec.CodScheda;

                    this.MovScheda.CodStatoScheda = Enum.GetName(typeof(EnumStatoScheda), EnumStatoScheda.IC);
                    this.MovScheda.DatiXML = movconsegnaprec.MovScheda.DatiXML;
                    this.MovScheda.AnteprimaRTF = movconsegnaprec.MovScheda.AnteprimaRTF;
                    this.MovScheda.DatiObbligatoriMancantiRTF = movconsegnaprec.MovScheda.DatiObbligatoriMancantiRTF;
                    this.MovScheda.DatiRilievoRTF = movconsegnaprec.MovScheda.DatiRilievoRTF;
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
                CodStatoConsegna = "AN";
                Azione = EnumAzioni.ANN;

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idmovconsegna);
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
                if (_idmovconsegna != string.Empty && _idmovconsegna.Trim() != "")
                {
                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDConsegna", _idmovconsegna);
                    op.Parametro.Add("CodUA", _codua);
                    op.Parametro.Add("CodTipoConsegna", _codtipoconsegna);
                    op.Parametro.Add("CodStatoConsegna", _codstatoconsegna);

                    op.Parametro.Add("DataEvento", Database.dataOra105PerParametri(_dataevento));
                    op.Parametro.Add("DataEventoUTC", Database.dataOra105PerParametri(_dataevento.ToUniversalTime()));


                    if (_azione == EnumAzioni.ANN)
                    {
                        _dataannullamento = DateTime.Now;
                        op.Parametro.Add("DataAnnullamento", Database.dataOra105PerParametri(_dataannullamento));
                        op.Parametro.Add("DataAnnullamentoUTC", Database.dataOra105PerParametri(_dataannullamento.ToUniversalTime()));

                        this.MovScheda.Azione = EnumAzioni.ANN;
                    }

                    op.TimeStamp.CodEntita = EnumEntita.CSG.ToString();

                    op.TimeStamp.IDEntita = _idmovconsegna;
                    op.TimeStamp.CodAzione = _azione.ToString();


                    op.MovScheda = this.MovScheda;

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    Database.ExecStoredProc("MSP_AggMovConsegne", spcoll);

                    this.MovScheda.Salva();

                    Carica(_idmovconsegna);
                }
                else
                {

                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("CodUA", _codua);
                    op.Parametro.Add("CodStatoConsegna", _codstatoconsegna);
                    op.Parametro.Add("CodTipoConsegna", _codtipoconsegna);

                    if (_codutenterilevazione != string.Empty)
                        op.Parametro.Add("CodUtenteRilevazione", _codutenterilevazione);

                    op.Parametro.Add("DataEvento", Database.dataOra105PerParametri(_dataevento));
                    op.Parametro.Add("DataEventoUTC", Database.dataOra105PerParametri(_dataevento.ToUniversalTime()));


                    op.TimeStamp.CodEntita = EnumEntita.CSG.ToString();
                    op.TimeStamp.CodAzione = _azione.ToString();

                    op.MovScheda = this.MovScheda;

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovConsegne", spcoll);
                    if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                    {
                        _idmovconsegna = dt.Rows[0][0].ToString();

                        this.MovScheda.IDEntita = _idmovconsegna;
                        this.MovScheda.Salva();

                        _azione = EnumAzioni.MOD;
                        Carica(_idmovconsegna);
                    }
                }

                return bReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(@"MovConsegna.Salva()" + Environment.NewLine + ex.Message, ex);
            }
        }

    }
}
