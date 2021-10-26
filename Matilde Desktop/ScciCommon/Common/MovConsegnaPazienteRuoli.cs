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
using UnicodeSrl.Scci.Model;
using UnicodeSrl.Framework.Types;

namespace UnicodeSrl.Scci
{

    [Serializable()]
    public class MovConsegnaPazienteRuoli
    {

        private string _idmovconsegnapazienteruoli = string.Empty;
        private string _idmovconsegnapaziente = string.Empty;

        private string _codstatoconsegnapazienteruolo = string.Empty;
        private string _codruolo = string.Empty;

        private string _codutenteinserimento = string.Empty;
        private string _codutenteultimamodifica = string.Empty;
        private string _codutenteannullamento = string.Empty;
        private string _codutentecancellazione = string.Empty;
        private string _codutentevisione = string.Empty;

        private DateTime _datainserimento = DateTime.MinValue;
        private DateTime _dataultimamodifica = DateTime.MinValue;
        private DateTime _dataannullamento = DateTime.MinValue;
        private DateTime _datacancellazione = DateTime.MinValue;
        private DateTime _datavisione = DateTime.MinValue;

        private string _descrutenteinserimento = string.Empty;
        private string _descrutenteultimamodifica = string.Empty;
        private string _descrutenteannullamento = string.Empty;
        private string _descrutentecancellazione = string.Empty;
        private string _descrutentevisione = string.Empty;

        private EnumAzioni _azione = EnumAzioni.MOD;
        private bool _modificato = false;

        private T_StatoConsegnaPazienteRuoli _statoconsegnapazienteruoli = null;

        public MovConsegnaPazienteRuoli(string idmovconsegnapazienteruoli, Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = EnumAzioni.MOD;
            _idmovconsegnapazienteruoli = idmovconsegnapazienteruoli;
            this.Carica(idmovconsegnapazienteruoli);
        }
        public MovConsegnaPazienteRuoli(string idmovconsegnapazienteruoli, EnumAzioni azione, Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = azione;
            _idmovconsegnapazienteruoli = idmovconsegnapazienteruoli;
            this.Carica(idmovconsegnapazienteruoli);
        }
        public MovConsegnaPazienteRuoli(Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = EnumAzioni.INS;
            _idmovconsegnapazienteruoli = "";
        }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string IDMovConsegnaPazienteRuoli
        {
            get { return _idmovconsegnapazienteruoli; }
            set
            {
                _idmovconsegnapazienteruoli = value;
                _modificato = true;
            }
        }

        public string IDMovConsegnaPaziente
        {
            get { return _idmovconsegnapaziente; }
            set
            {
                _idmovconsegnapaziente = value;
                _modificato = true;
            }
        }

        public string CodStatoConsegnaPazienteRuolo
        {
            get { return _codstatoconsegnapazienteruolo; }
            set
            {
                _codstatoconsegnapazienteruolo = value;
                _statoconsegnapazienteruoli = null;
                _modificato = true;
            }
        }

        public string CodRuolo
        {
            get { return _codruolo; }
            set
            {
                _codruolo = value;
                _modificato = true;
            }
        }

        public ScciAmbiente Ambiente { get; set; }

        public bool Modificato
        {
            get { return _modificato; }
        }

        public T_StatoConsegnaPazienteRuoli T_StatoConsegnaPazienteRuoli
        {
            get
            {
                if (_statoconsegnapazienteruoli == null)
                {
                    _statoconsegnapazienteruoli = new T_StatoConsegnaPazienteRuoli(_codstatoconsegnapazienteruolo);
                    _statoconsegnapazienteruoli.Select();
                }
                return _statoconsegnapazienteruoli;
            }
        }

        public byte[] StatoIcona
        {
            get
            {
                return T_StatoConsegnaPazienteRuoli.Icona.Value;
            }
        }

        public string StatoConsegna
        {
            get
            {
                return T_StatoConsegnaPazienteRuoli.Descrizione;
            }
        }

        public string Ruolo
        {
            get
            {
                T_RuoliRow t = new T_RuoliRow(CodRuolo);
                if (t.TrySelect())
                    return t.Descrizione;
                else
                    return string.Empty;
            }
        }

        public string CodUtenteInserimento
        {
            get { return _codutenteinserimento; }
        }

        public string DescrUtenteInserimento
        {
            get { return _descrutenteinserimento; }
        }

        public string CodUtenteUltimaModifica
        {
            get { return _codutenteultimamodifica; }
        }

        public string DescrUtenteUltimaModifica
        {
            get { return _descrutenteultimamodifica; }
        }

        public string CodUtenteAnnullamento
        {
            get { return _codutenteannullamento; }
        }

        public string DescrUtenteAnnullamento
        {
            get { return _descrutenteannullamento; }
        }

        public string CodUtenteCancellazione
        {
            get { return _codutentecancellazione; }
        }

        public string DescrUtenteCancellazione
        {
            get { return _descrutentecancellazione; }
        }

        public string CodUtenteVisione
        {
            get { return _codutentevisione; }
        }

        public string DescrUtenteVisione
        {
            get { return _descrutentevisione; }
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
        }

        public DateTime DataCancellazione
        {
            get { return _datacancellazione; }
        }

        public DateTime DataVisione
        {
            get { return _datavisione; }
        }

        private void resetValori()
        {

            _idmovconsegnapaziente = string.Empty;
            _codstatoconsegnapazienteruolo = string.Empty;
            _codruolo = string.Empty;

            _codutenteinserimento = string.Empty;
            _descrutenteinserimento = string.Empty;
            _codutenteultimamodifica = string.Empty;
            _descrutenteultimamodifica = string.Empty;
            _codutenteannullamento = string.Empty;
            _descrutenteannullamento = string.Empty;
            _codutentevisione = string.Empty;
            _descrutentevisione = string.Empty;

            _datainserimento = DateTime.MinValue;
            _dataultimamodifica = DateTime.MinValue;
            _dataannullamento = DateTime.MinValue;
            _datavisione = DateTime.MinValue;

            _modificato = false;

        }

        private void Carica(string idmovconsegnapaziente)
        {

            try
            {

                Parametri op = null;
                op = new Parametri(this.Ambiente);
                op.Parametro.Add("IDConsegnaPazienteRuoli", idmovconsegnapaziente);
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                FwDataParametersList plist = new FwDataParametersList();
                plist.Add("xParametri", xmlParam, ParameterDirection.Input, DbType.Xml);
                using (FwDataConnection conn = new FwDataConnection(Database.ConnectionString))
                {

                    DataTable dt = conn.Query<DataTable>("MSP_SelMovConsegnePazienteRuoli", plist, CommandType.StoredProcedure);
                    if (dt.Rows.Count > 0)
                    {

                        resetValori();

                        if (!dt.Rows[0].IsNull("IDConsegnaPaziente")) _idmovconsegnapaziente = dt.Rows[0]["IDConsegnaPaziente"].ToString();
                        if (!dt.Rows[0].IsNull("CodStatoConsegnaPazienteRuolo")) _codstatoconsegnapazienteruolo = dt.Rows[0]["CodStatoConsegnaPazienteRuolo"].ToString();
                        if (!dt.Rows[0].IsNull("CodRuolo")) _codruolo = dt.Rows[0]["CodRuolo"].ToString();

                        if (!dt.Rows[0].IsNull("CodUtenteInserimento")) _codutenteinserimento = dt.Rows[0]["CodUtenteInserimento"].ToString();
                        if (!dt.Rows[0].IsNull("DescrUtenteInserimento")) _descrutenteinserimento = dt.Rows[0]["DescrUtenteInserimento"].ToString();
                        if (!dt.Rows[0].IsNull("CodUtenteUltimaModifica")) _codutenteultimamodifica = dt.Rows[0]["CodUtenteUltimaModifica"].ToString();
                        if (!dt.Rows[0].IsNull("DescrUtenteUltimaModifica")) _descrutenteultimamodifica = dt.Rows[0]["DescrUtenteUltimaModifica"].ToString();
                        if (!dt.Rows[0].IsNull("CodUtenteAnnullamento")) _codutenteannullamento = dt.Rows[0]["CodUtenteAnnullamento"].ToString();
                        if (!dt.Rows[0].IsNull("DescrUtenteAnnullamento")) _descrutenteannullamento = dt.Rows[0]["DescrUtenteAnnullamento"].ToString();
                        if (!dt.Rows[0].IsNull("CodUtenteCancellazione")) _codutentecancellazione = dt.Rows[0]["CodUtenteCancellazione"].ToString();
                        if (!dt.Rows[0].IsNull("DescrUtenteCancellazione")) _descrutentecancellazione = dt.Rows[0]["DescrUtenteCancellazione"].ToString();
                        if (!dt.Rows[0].IsNull("CodUtenteVisione")) _codutentevisione = dt.Rows[0]["CodUtenteVisione"].ToString();
                        if (!dt.Rows[0].IsNull("DescrUtenteVisione")) _descrutentevisione = dt.Rows[0]["DescrUtenteVisione"].ToString();

                        if (!dt.Rows[0].IsNull("DataInserimento")) _datainserimento = (DateTime)dt.Rows[0]["DataInserimento"];
                        if (!dt.Rows[0].IsNull("DataUltimaModifica")) _dataultimamodifica = (DateTime)dt.Rows[0]["DataUltimaModifica"];
                        if (!dt.Rows[0].IsNull("DataAnnullamento")) _dataannullamento = (DateTime)dt.Rows[0]["DataAnnullamento"];
                        if (!dt.Rows[0].IsNull("DataCancellazione")) _datacancellazione = (DateTime)dt.Rows[0]["DataCancellazione"];
                        if (!dt.Rows[0].IsNull("DataVisione")) _datavisione = (DateTime)dt.Rows[0]["DataVisione"];

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
                CodStatoConsegnaPazienteRuolo = "AN";
                Azione = EnumAzioni.ANN;

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idmovconsegnapazienteruoli);
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
                CodStatoConsegnaPazienteRuolo = "CA";
                Azione = EnumAzioni.CAN;

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idmovconsegnapazienteruoli);
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

        public bool Vista()
        {
            try
            {
                CodStatoConsegnaPazienteRuolo = "VS";
                Azione = EnumAzioni.VAL;

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idmovconsegnapazienteruoli);
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
                if (_idmovconsegnapazienteruoli != string.Empty && _idmovconsegnapazienteruoli.Trim() != "")
                {

                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDConsegnaPazienteRuoli", _idmovconsegnapazienteruoli);
                    op.Parametro.Add("CodStatoConsegnaPazienteRuolo", _codstatoconsegnapazienteruolo);
                    op.Parametro.Add("CodRuolo", _codruolo);

                    op.TimeStamp.CodEntita = EnumEntita.CSR.ToString();

                    op.TimeStamp.IDEntita = _idmovconsegnapazienteruoli;
                    op.TimeStamp.CodAzione = _azione.ToString();

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    Database.ExecStoredProc("MSP_AggMovConsegnePazienteRuoli", spcoll);

                    Carica(_idmovconsegnapazienteruoli);

                }
                else
                {

                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDConsegnaPaziente", _idmovconsegnapaziente);
                    op.Parametro.Add("CodStatoConsegnaPazienteRuolo", _codstatoconsegnapazienteruolo);
                    op.Parametro.Add("CodRuolo", _codruolo);

                    op.TimeStamp.CodEntita = EnumEntita.CSR.ToString();
                    op.TimeStamp.CodAzione = _azione.ToString();

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovConsegnePazienteRuoli", spcoll);
                    if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                    {
                        _idmovconsegnapaziente = dt.Rows[0][0].ToString();

                        _azione = EnumAzioni.MOD;
                        Carica(_idmovconsegnapazienteruoli);
                    }

                }

                return bReturn;

            }
            catch (Exception ex)
            {
                throw new Exception(@"MovConsegnaPazienteRuoli.Salva()" + Environment.NewLine + ex.Message, ex);
            }

        }

        public bool CopiaDaOrigine(ref MovConsegnaPazienteRuoli movtiorigine)
        {

            bool bReturn = true;

            try
            {

                _idmovconsegnapazienteruoli = string.Empty;
                _idmovconsegnapaziente = string.Empty;

                _codstatoconsegnapazienteruolo = "IC";
                _codruolo = movtiorigine.CodRuolo;

                _modificato = true;

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovConsegnaPazienteRuoli.CopiaDaOrigine()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;
        }

    }

}
