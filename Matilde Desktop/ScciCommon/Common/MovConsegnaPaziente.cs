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
    public class MovConsegnaPaziente
    {

        private EnumAzioni _azione = EnumAzioni.MOD;

        private string _idmovconsegnapaziente = string.Empty;
        private string _idepisodio = string.Empty;

        private string _codua = string.Empty;
        private string _codruoloinserimento = string.Empty;
        private string _codtipoconsegnapaziente = string.Empty;
        private string _codstatoconsegnapaziente = string.Empty;

        private string _codutenteinserimento = string.Empty;
        private string _codutenteultimamodifica = string.Empty;
        private string _codutenteannullamento = string.Empty;
        private string _codutentecancellazione = string.Empty;

        private DateTime _datainserimento = DateTime.MinValue;
        private DateTime _dataultimamodifica = DateTime.MinValue;
        private DateTime _dataannullamento = DateTime.MinValue;
        private DateTime _datacancellazione = DateTime.MinValue;

        private string _descrua = string.Empty;
        private string _descrtipoconsegnapaziente = string.Empty;
        private string _descrstatoconsegnapaziente = string.Empty;
        private string _descrutenteinserimento = string.Empty;
        private string _descrutenteultimamodifica = string.Empty;
        private string _descrutenteannullamento = string.Empty;
        private string _descrutentecancellazione = string.Empty;

        private List<MovConsegnaPazienteRuoli> _elementi = new List<MovConsegnaPazienteRuoli>();

        private string _idscheda = string.Empty;
        private string _codscheda = string.Empty;
        private int _versionescheda = 0;
        private MovScheda _movScheda = null;

        public MovConsegnaPaziente(string idmovconsegnapaziente, Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = EnumAzioni.MOD;
            _idmovconsegnapaziente = idmovconsegnapaziente;
            this.Carica(idmovconsegnapaziente);
            this.CaricaRuoli();
        }
        public MovConsegnaPaziente(Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = EnumAzioni.INS;
            _idmovconsegnapaziente = "";
            _idepisodio = "";
            _codscheda = "";
            _movScheda = null;
            _elementi = new List<MovConsegnaPazienteRuoli>();
        }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string IDMovConsegnaPaziente
        {
            get { return _idmovconsegnapaziente; }
            set
            {
                if (_idmovconsegnapaziente != value && value != "") _movScheda = null;
                _idmovconsegnapaziente = value;
            }
        }

        public string IDEpisodio
        {
            get { return _idepisodio; }
            set
            {
                _idepisodio = value;
            }
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

        public string CodRuoloInserimento
        {
            get { return _codruoloinserimento; }
            set { _codruoloinserimento = value; }
        }

        public string CodTipoConsegnaPaziente
        {
            get { return _codtipoconsegnapaziente; }
            set { _codtipoconsegnapaziente = value; }
        }

        public string DescrTipoConsegnaPaziente
        {
            get { return _descrtipoconsegnapaziente; }
            set { _descrtipoconsegnapaziente = value; }
        }

        public string CodStatoConsegnaPaziente
        {
            get { return _codstatoconsegnapaziente; }
            set { _codstatoconsegnapaziente = value; }
        }

        public string DescrStatoConsegnaPaziente
        {
            get { return _descrstatoconsegnapaziente; }
        }

        public string CodUtenteInserimento
        {
            get { return _codutenteinserimento; }
            set { _codutenteinserimento = value; }
        }

        public string DescrUtenteInserimento
        {
            get { return _descrutenteinserimento; }
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

        public string CodUtenteAnnullamento
        {
            get { return _codutenteannullamento; }
            set { _codutenteannullamento = value; }
        }

        public string DescrUtenteAnnullamento
        {
            get { return _descrutenteannullamento; }
        }

        public string CodUtenteCancellazione
        {
            get { return _codutentecancellazione; }
            set { _codutentecancellazione = value; }
        }

        public string DescrUtenteCancellazione
        {
            get { return _descrutentecancellazione; }
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

        public DateTime DataCancellazione
        {
            get { return _datacancellazione; }
            set { _datacancellazione = value; }
        }

        public List<MovConsegnaPazienteRuoli> Elementi
        {
            get { return _elementi; }
            set { _elementi = value; }
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
                    if (_idmovconsegnapaziente != null && _idmovconsegnapaziente != string.Empty && _idmovconsegnapaziente.Trim() != "")
                    {
                        _movScheda = new MovScheda(EnumEntita.CSP.ToString(), _idmovconsegnapaziente, this.Ambiente);
                    }
                    else if (_codscheda != null && _codscheda != string.Empty && _codscheda.Trim() != "")
                    {
                        _movScheda = new MovScheda(_codscheda, EnumEntita.CSP, _codua, "", _idepisodio, "", this.Ambiente);
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

        public ScciAmbiente Ambiente { get; set; }

        private void resetValori()
        {

            _idepisodio = string.Empty;
            _codua = string.Empty;
            _descrua = string.Empty;
            _codruoloinserimento = string.Empty;
            _codtipoconsegnapaziente = string.Empty;
            _descrtipoconsegnapaziente = string.Empty;
            _codstatoconsegnapaziente = string.Empty;
            _descrstatoconsegnapaziente = string.Empty;
            _codutenteinserimento = string.Empty;
            _descrutenteinserimento = string.Empty;
            _codutenteultimamodifica = string.Empty;
            _descrutenteultimamodifica = string.Empty;
            _codutenteannullamento = string.Empty;
            _descrutenteannullamento = string.Empty;
            _codutentecancellazione = string.Empty;
            _descrutentecancellazione = string.Empty;
            _datainserimento = DateTime.MinValue;
            _dataultimamodifica = DateTime.MinValue;
            _dataannullamento = DateTime.MinValue;
            _datacancellazione = DateTime.MinValue;

            _idscheda = string.Empty;
            _codscheda = string.Empty;
            _versionescheda = 0;
            _movScheda = null;

        }

        private void Carica(string idmovconsegnapaziente)
        {

            try
            {

                Parametri op = null;
                op = new Parametri(this.Ambiente);
                op.Parametro.Add("IDConsegnaPaziente", idmovconsegnapaziente);

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                FwDataParametersList plist = new FwDataParametersList();
                plist.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);
                using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                {

                    DataTable dt = conn.Query<DataTable>("MSP_SelMovConsegnePaziente", plist, CommandType.StoredProcedure);
                    if (dt.Rows.Count > 0)
                    {

                        resetValori();

                        if (!dt.Rows[0].IsNull("IDEpisodio")) _idepisodio = dt.Rows[0]["IDEpisodio"].ToString();
                        if (!dt.Rows[0].IsNull("CodUA")) _codua = dt.Rows[0]["CodUA"].ToString();
                        if (!dt.Rows[0].IsNull("DescrUA")) _descrua = dt.Rows[0]["DescrUA"].ToString();
                        if (!dt.Rows[0].IsNull("CodRuoloInserimento")) _codtipoconsegnapaziente = dt.Rows[0]["CodRuoloInserimento"].ToString();
                        if (!dt.Rows[0].IsNull("CodTipoConsegnaPaziente")) _codtipoconsegnapaziente = dt.Rows[0]["CodTipoConsegnaPaziente"].ToString();
                        if (!dt.Rows[0].IsNull("DescrTipoConsegnaPaziente")) _descrtipoconsegnapaziente = dt.Rows[0]["DescrTipoConsegnaPaziente"].ToString();
                        if (!dt.Rows[0].IsNull("CodStatoConsegnaPaziente")) _codstatoconsegnapaziente = dt.Rows[0]["CodStatoConsegnaPaziente"].ToString();
                        if (!dt.Rows[0].IsNull("DescrStatoConsegnaPaziente")) _descrstatoconsegnapaziente = dt.Rows[0]["DescrStatoConsegnaPaziente"].ToString();

                        if (!dt.Rows[0].IsNull("CodUtenteInserimento")) _codutenteinserimento = dt.Rows[0]["CodUtenteInserimento"].ToString();
                        if (!dt.Rows[0].IsNull("DescrUtenteInserimento")) _descrutenteinserimento = dt.Rows[0]["DescrUtenteInserimento"].ToString();
                        if (!dt.Rows[0].IsNull("CodUtenteUltimaModifica")) _codutenteultimamodifica = dt.Rows[0]["CodUtenteUltimaModifica"].ToString();
                        if (!dt.Rows[0].IsNull("DescrUtenteUltimaModifica")) _descrutenteultimamodifica = dt.Rows[0]["DescrUtenteUltimaModifica"].ToString();
                        if (!dt.Rows[0].IsNull("CodUtenteAnnullamento")) _codutenteannullamento = dt.Rows[0]["CodUtenteAnnullamento"].ToString();
                        if (!dt.Rows[0].IsNull("DescrUtenteAnnullamento")) _descrutenteannullamento = dt.Rows[0]["DescrUtenteAnnullamento"].ToString();
                        if (!dt.Rows[0].IsNull("CodUtenteCancellazione")) _codutentecancellazione = dt.Rows[0]["CodUtenteCancellazione"].ToString();
                        if (!dt.Rows[0].IsNull("DescrUtenteCancellazione")) _descrutentecancellazione = dt.Rows[0]["DescrUtenteCancellazione"].ToString();

                        if (!dt.Rows[0].IsNull("DataInserimento")) _datainserimento = (DateTime)dt.Rows[0]["DataInserimento"];
                        if (!dt.Rows[0].IsNull("DataUltimaModifica")) _dataultimamodifica = (DateTime)dt.Rows[0]["DataUltimaModifica"];
                        if (!dt.Rows[0].IsNull("DataAnnullamento")) _dataannullamento = (DateTime)dt.Rows[0]["DataAnnullamento"];
                        if (!dt.Rows[0].IsNull("DataCancellazione")) _datacancellazione = (DateTime)dt.Rows[0]["DataCancellazione"];

                        if (!dt.Rows[0].IsNull("IDScheda")) _idscheda = dt.Rows[0]["IDScheda"].ToString();
                        if (!dt.Rows[0].IsNull("CodScheda")) _codscheda = dt.Rows[0]["CodScheda"].ToString();
                        if (!dt.Rows[0].IsNull("Versione")) _versionescheda = (int)dt.Rows[0]["Versione"];

                    }

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public void CaricaRuoli()
        {

            try
            {

                if (_idmovconsegnapaziente != string.Empty)
                {

                    Parametri op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDConsegnaPaziente", _idmovconsegnapaziente);
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    FwDataParametersList plist = new FwDataParametersList();
                    plist.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);
                    using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                    {

                        DataTable dt = conn.Query<DataTable>("MSP_SelMovConsegnePazienteRuoli", plist, CommandType.StoredProcedure);

                        _elementi = new List<MovConsegnaPazienteRuoli>();
                        foreach (DataRow oDr in dt.Rows)
                        {
                            _elementi.Add(new MovConsegnaPazienteRuoli(oDr["ID"].ToString(), this.Ambiente));
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public bool Annulla()
        {

            try
            {

                CodStatoConsegnaPaziente = "AN";
                Azione = EnumAzioni.ANN;

                foreach (MovConsegnaPazienteRuoli oMovElemento in this.Elementi)
                {
                    if (oMovElemento.CodStatoConsegnaPazienteRuolo != "VS")
                    {
                        oMovElemento.Annulla();
                    }
                }

                return Salva(false);

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

                CodStatoConsegnaPaziente = "CA";
                Azione = EnumAzioni.CAN;

                foreach (MovConsegnaPazienteRuoli oMovElemento in this.Elementi)
                {
                    oMovElemento.Cancella();
                }

                return Salva(false);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool Vista(string codruolo)
        {

            bool bRet = false;

            try
            {

                MovConsegnaPazienteRuoli oMov = this.Elementi.SingleOrDefault<MovConsegnaPazienteRuoli>(M => M.CodRuolo == codruolo);
                if (oMov != null && oMov.CodStatoConsegnaPazienteRuolo != "VS")
                {
                    bRet = oMov.Vista();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return bRet;

        }

        public bool Salva(bool RicaricaMovimento)
        {

            bool bReturn = true;

            Parametri op = null;
            string xmlParam = "";

            try
            {

                if (_idmovconsegnapaziente != string.Empty && _idmovconsegnapaziente.Trim() != "")
                {

                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDConsegnaPaziente", _idmovconsegnapaziente);
                    op.Parametro.Add("CodStatoConsegnaPaziente", _codstatoconsegnapaziente);

                    op.TimeStamp.CodEntita = EnumEntita.CSP.ToString();

                    op.TimeStamp.IDEntita = _idmovconsegnapaziente;
                    op.TimeStamp.CodAzione = _azione.ToString();

                    op.MovScheda = this.MovScheda;
                    if (_azione == EnumAzioni.ANN)
                    {
                        this.MovScheda.Azione = EnumAzioni.ANN;
                    }


                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                    FwDataParametersList plist = new FwDataParametersList();
                    plist.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);

                    using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                    {

                        conn.ExecuteStored("MSP_AggMovConsegnePaziente", ref plist);

                        foreach (MovConsegnaPazienteRuoli oMaa in _elementi)
                        {

                            if (oMaa.Azione == EnumAzioni.MOD && oMaa.Modificato)
                            {
                                op = new Parametri(this.Ambiente);
                                op.Parametro.Add("IDConsegnaPazienteRuoli", oMaa.IDMovConsegnaPazienteRuoli);
                                op.Parametro.Add("CodStatoConsegnaPazienteRuolo", oMaa.CodStatoConsegnaPazienteRuolo);
                                op.Parametro.Add("CodRuolo", oMaa.CodRuolo);

                                op.TimeStamp.CodEntita = EnumEntita.CSR.ToString();
                                op.TimeStamp.CodAzione = (oMaa.CodStatoConsegnaPazienteRuolo == "AN" ? "ANN" : _azione.ToString());

                                xmlParam = XmlProcs.XmlSerializeToString(op);
                                plist = new FwDataParametersList();
                                plist.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);

                                conn.ExecuteStored("MSP_AggMovConsegnePazienteRuoli", ref plist);
                            }
                            else if (oMaa.Azione == EnumAzioni.INS && oMaa.Modificato)
                            {
                                op = new Parametri(this.Ambiente);
                                op.Parametro.Add("IDConsegnaPaziente", _idmovconsegnapaziente);
                                op.Parametro.Add("CodStatoConsegnaPazienteRuolo", oMaa.CodStatoConsegnaPazienteRuolo);
                                op.Parametro.Add("CodRuolo", oMaa.CodRuolo);

                                op.TimeStamp.CodEntita = EnumEntita.CSR.ToString();
                                op.TimeStamp.CodAzione = _azione.ToString();

                                xmlParam = XmlProcs.XmlSerializeToString(op);
                                plist = new FwDataParametersList();
                                plist.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);

                                DataTable dtRuoli = conn.Query<DataTable>("MSP_InsMovConsegnePazienteRuoli", plist, CommandType.StoredProcedure);
                            }

                        }

                        this.MovScheda.Salva();

                        if (RicaricaMovimento)
                        {
                            Carica(_idmovconsegnapaziente);
                        }

                    }

                }
                else
                {

                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDEpisodio", _idepisodio);
                    op.Parametro.Add("CodUA", _codua);
                    op.Parametro.Add("CodTipoConsegnaPaziente", _codtipoconsegnapaziente);
                    op.Parametro.Add("CodStatoConsegnaPaziente", _codstatoconsegnapaziente);

                    op.TimeStamp.CodEntita = EnumEntita.CSP.ToString();
                    op.TimeStamp.CodAzione = _azione.ToString();

                    op.MovScheda = this.MovScheda;

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                    FwDataParametersList plist = new FwDataParametersList();
                    plist.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);

                    using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                    {

                        DataTable dt = conn.Query<DataTable>("MSP_InsMovConsegnePaziente", plist, CommandType.StoredProcedure);
                        if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                        {

                            _idmovconsegnapaziente = dt.Rows[0][0].ToString();

                            foreach (MovConsegnaPazienteRuoli oMaa in _elementi)
                            {

                                op = new Parametri(this.Ambiente);
                                op.Parametro.Add("IDConsegnaPaziente", _idmovconsegnapaziente);
                                op.Parametro.Add("CodStatoConsegnaPazienteRuolo", oMaa.CodStatoConsegnaPazienteRuolo);
                                op.Parametro.Add("CodRuolo", oMaa.CodRuolo);

                                op.TimeStamp.CodEntita = EnumEntita.CSR.ToString();
                                op.TimeStamp.CodAzione = _azione.ToString();

                                xmlParam = XmlProcs.XmlSerializeToString(op);
                                plist = new FwDataParametersList();
                                plist.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);

                                DataTable dtRuoli = conn.Query<DataTable>("MSP_InsMovConsegnePazienteRuoli", plist, CommandType.StoredProcedure);

                            }

                            this.MovScheda.IDEntita = _idmovconsegnapaziente;
                            this.MovScheda.Salva();

                            if (RicaricaMovimento)
                            {
                                _azione = EnumAzioni.MOD;
                                Carica(_idmovconsegnapaziente);
                            }

                        }
                    }
                }

                return bReturn;

            }
            catch (Exception ex)
            {
                throw new Exception(@"MovConsegnaPaziente.Salva()" + Environment.NewLine + ex.Message, ex);
            }

        }

        public void AggiungiRuoloCreazione(string codruolo)
        {

            if (this.Elementi.Count == 0 ||
                this.Elementi.Single<MovConsegnaPazienteRuoli>(M => M.CodRuolo == codruolo) != null)
            {
                MovConsegnaPazienteRuoli oMovElemento = new MovConsegnaPazienteRuoli(Ambiente);
                oMovElemento.CodRuolo = codruolo;
                oMovElemento.CodStatoConsegnaPazienteRuolo = "IC";
                oMovElemento.IDMovConsegnaPaziente = this.IDMovConsegnaPaziente;
                oMovElemento.Azione = EnumAzioni.INS;
                this.Elementi.Add(oMovElemento);
            }

        }

        public string ListaRuoliVistati()
        {

            string sRet = string.Empty;

            try
            {

                List<string> lstVistati = _elementi.Where<MovConsegnaPazienteRuoli>(m => m.CodStatoConsegnaPazienteRuolo == "VS").Select(r => $"{r.Ruolo} ({r.CodRuolo})").ToList();
                sRet = string.Join(",\r\n", lstVistati);

            }
            catch (Exception)
            {

            }

            return sRet;

        }

        public bool CopiaDaOrigine(ref MovConsegnaPaziente movtiorigine)
        {

            bool bReturn = true;

            try
            {

                _idmovconsegnapaziente = string.Empty;

                _idepisodio = movtiorigine.IDEpisodio;
                _codua = movtiorigine.CodUA;
                _codruoloinserimento = movtiorigine.CodRuoloInserimento;
                _codtipoconsegnapaziente = movtiorigine.CodTipoConsegnaPaziente;
                _codstatoconsegnapaziente = "IC";

                foreach (MovConsegnaPazienteRuoli cprorigine in movtiorigine.Elementi)
                {

                    MovConsegnaPazienteRuoli oOrigine = cprorigine;
                    MovConsegnaPazienteRuoli cpr = new MovConsegnaPazienteRuoli(this.Ambiente);
                    cpr.CopiaDaOrigine(ref oOrigine);
                    _elementi.Add(cpr);
                    oOrigine = null;
                    cpr = null;

                }

                this.MovScheda = new MovScheda(movtiorigine.MovScheda.CodScheda,
                                                (EnumEntita)Enum.Parse(typeof(EnumEntita), movtiorigine.MovScheda.CodEntita),
                                                movtiorigine.MovScheda.CodUA, movtiorigine.MovScheda.IDPaziente,
                                                movtiorigine.MovScheda.IDEpisodio, movtiorigine.MovScheda.IDTrasferimento, this.Ambiente);
                this.MovScheda.CopiaDaOrigine(movtiorigine.MovScheda, 1);

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovConsegnaPaziente.CopiaDaOrigine()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;
        }

    }

}
