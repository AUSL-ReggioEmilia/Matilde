using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci
{
    [Serializable()]
    public class MovAlertAllergieAnamnesi
    {

        private string _codua = string.Empty;
        private string _idepisodio = string.Empty;
        private string _idtrasferimento = string.Empty;
        private string _idmovalertallergieanamnesi = string.Empty;
        private string _idpaziente = string.Empty;
        private string _codtipo = string.Empty;
        private string _codstato = string.Empty;
        private string _codutenterilevazione = string.Empty;
        private DateTime _dataevento = DateTime.MinValue;
        private DateTime _dataultimamodifica = DateTime.MinValue;

        private string _descrtipo = string.Empty;
        private string _descrstato = string.Empty;
        private string _descrutenterilevazione = string.Empty;

        private int _permessoAnnulla = 0;

        private byte[] _icona;

        private string _idscheda = string.Empty;
        private string _codscheda = string.Empty;
        private int _versionescheda = 0;

        private string _codsistema = string.Empty;
        private string _idsistema = string.Empty;
        private string _idgruppo = string.Empty;

        private EnumAzioni _azione = EnumAzioni.MOD;

        private MovScheda _movScheda = null;

        public MovAlertAllergieAnamnesi(string idmovalertallergieanamnesi, string idpaziente, string codua, string idepisodio, string idtrasferimento, Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = EnumAzioni.MOD;
            _idpaziente = idpaziente;
            _idmovalertallergieanamnesi = idmovalertallergieanamnesi;
            _idepisodio = idepisodio;
            _idtrasferimento = idtrasferimento;
            _codua = codua;
            this.Carica(idmovalertallergieanamnesi);
        }
        public MovAlertAllergieAnamnesi(string idmovalertallergieanamnesi, EnumAzioni azione, string idpaziente, string codua, string idepisodio, string idtrasferimento, Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = azione;
            _idmovalertallergieanamnesi = idmovalertallergieanamnesi;
            _idpaziente = idpaziente;
            _idepisodio = idepisodio;
            _idtrasferimento = idtrasferimento;
            _codua = codua;
            this.Carica(idmovalertallergieanamnesi);
        }
        public MovAlertAllergieAnamnesi(string idpaziente, string codua, string idepisodio, string idtrasferimento, Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = EnumAzioni.INS;
            _idpaziente = idpaziente;
            _idepisodio = idepisodio;
            _idtrasferimento = idtrasferimento;
            _codua = codua;
            _idmovalertallergieanamnesi = "";
            _codscheda = "";
            _codtipo = "";
            _codstato = "AT";
            _movScheda = null;
        }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string IDMovAlertAllergieAnamnesi
        {
            get { return _idmovalertallergieanamnesi; }
            set
            {
                if (_idmovalertallergieanamnesi != value && value != "") _movScheda = null;
                _idmovalertallergieanamnesi = value;
            }
        }

        public string CodUA
        {
            get { return _codua; }
            set { _codua = value; }
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

        public string IDPaziente
        {
            get { return _idpaziente; }
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
                    if (_idmovalertallergieanamnesi != null && _idmovalertallergieanamnesi != string.Empty && _idmovalertallergieanamnesi.Trim() != "")
                    {
                        _movScheda = new MovScheda(EnumEntita.ALA.ToString(), _idmovalertallergieanamnesi, this.Ambiente);
                    }
                    else if (_codscheda != null && _codscheda != string.Empty && _codscheda.Trim() != "")
                    {
                        _movScheda = new MovScheda(_codscheda, EnumEntita.ALA, _codua, _idpaziente, _idepisodio, _idtrasferimento, this.Ambiente);
                        Gestore oGestore = CommonStatics.GetGestore(this.Ambiente);
                        oGestore.SchedaXML = _movScheda.Scheda.StrutturaXML;
                        oGestore.SchedaLayoutsXML = _movScheda.Scheda.LayoutXML;
                        oGestore.NuovaScheda();
                        _movScheda.DatiXML = oGestore.SchedaDatiXML;
                    }
                }

                return _movScheda;
            }
            set { _movScheda = value; }
        }

        public DateTime DataEvento
        {
            get { return _dataevento; }
            set { _dataevento = value; }
        }

        public DateTime DataUltimaModifica
        {
            get { return _dataultimamodifica; }
        }

        public string CodUtenteRilevazione
        {
            get { return _codutenterilevazione; }
        }

        public string CodStato
        {
            get { return _codstato; }
            set { _codstato = value; }
        }

        public string CodTipo
        {
            get { return _codtipo; }
            set { _codtipo = value; }
        }

        public string DescrStato
        {
            get { return _descrstato; }
        }

        public string DescrTipo
        {
            get { return _descrtipo; }
            set { _descrtipo = value; }
        }

        public string DescrUtenteRilevazione
        {
            get { return _descrutenterilevazione; }
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

        public ScciAmbiente Ambiente { get; set; }

        private void resetValori()
        {
            _codtipo = string.Empty;
            _codstato = string.Empty;
            _codutenterilevazione = string.Empty;
            _dataevento = DateTime.MinValue;
            _dataultimamodifica = DateTime.MinValue;

            _descrtipo = string.Empty;
            _descrstato = string.Empty;
            _descrutenterilevazione = string.Empty;

            _codsistema = string.Empty;
            _idsistema = string.Empty;
            _idgruppo = string.Empty;

            _permessoAnnulla = 0;

            _icona = null;

            _idscheda = string.Empty;
            _codscheda = string.Empty;
            _versionescheda = 0;
            _movScheda = null;

        }

        private void Carica(string idmovalertallergieanamnesi)
        {
            try
            {

                Parametri op = new Parametri(this.Ambiente);
                op.Parametro.Add("IDAllergieAnamnesi", idmovalertallergieanamnesi);
                op.Parametro.Add("DatiEstesi", "1");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovAlertAllergieAnamnesi", spcoll);

                if (dt.Rows.Count > 0)
                {
                    resetValori();

                    _idpaziente = dt.Rows[0]["IDPaziente"].ToString();

                    if (!dt.Rows[0].IsNull("CodTipo")) _codtipo = dt.Rows[0]["CodTipo"].ToString();
                    if (!dt.Rows[0].IsNull("DescrTipo")) _descrtipo = dt.Rows[0]["DescrTipo"].ToString();
                    if (!dt.Rows[0].IsNull("CodStato")) _codstato = dt.Rows[0]["CodStato"].ToString();
                    if (!dt.Rows[0].IsNull("DescrStato")) _descrstato = dt.Rows[0]["DescrStato"].ToString();
                    if (!dt.Rows[0].IsNull("CodUtente")) _codutenterilevazione = dt.Rows[0]["CodUtente"].ToString();
                    if (!dt.Rows[0].IsNull("DescrUtente")) _descrutenterilevazione = dt.Rows[0]["DescrUtente"].ToString();

                    if (!dt.Rows[0].IsNull("CodSistema")) _codsistema = dt.Rows[0]["CodSistema"].ToString();
                    if (!dt.Rows[0].IsNull("IDSistema")) _idsistema = dt.Rows[0]["IDSistema"].ToString();
                    if (!dt.Rows[0].IsNull("IDGruppo")) _idgruppo = dt.Rows[0]["IDGruppo"].ToString();

                    if (!dt.Rows[0].IsNull("DataEvento")) _dataevento = (DateTime)dt.Rows[0]["DataEvento"];
                    if (!dt.Rows[0].IsNull("DataUltimaModifica")) _dataultimamodifica = (DateTime)dt.Rows[0]["DataUltimaModifica"];

                    if (!dt.Rows[0].IsNull("PermessoAnnulla")) _permessoAnnulla = (int)dt.Rows[0]["PermessoAnnulla"];

                    if (!dt.Rows[0].IsNull("Icona")) _icona = (byte[])dt.Rows[0]["Icona"];

                    if (dt.Columns.Contains("IDScheda") && !dt.Rows[0].IsNull("IDScheda")) _idscheda = dt.Rows[0]["IDScheda"].ToString();
                    if (dt.Columns.Contains("CodScheda") && !dt.Rows[0].IsNull("CodScheda")) _codscheda = dt.Rows[0]["CodScheda"].ToString();
                    if (dt.Columns.Contains("Versione") && !dt.Rows[0].IsNull("Versione")) _versionescheda = (int)dt.Rows[0]["Versione"];

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
                CodStato = "AN";
                Azione = EnumAzioni.ANN;

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idmovalertallergieanamnesi);
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
                CodStato = "CA";
                Azione = EnumAzioni.CAN;

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idmovalertallergieanamnesi);
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
                if (_idmovalertallergieanamnesi != string.Empty && _idmovalertallergieanamnesi.Trim() != "")
                {
                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDAlertAllergiaAnamnesi", _idmovalertallergieanamnesi);
                    op.Parametro.Add("CodTipoAlertAllergiaAnamnesi", _codtipo);
                    op.Parametro.Add("CodStatoAlertAllergiaAnamnesi", _codstato);

                    op.Parametro.Add("CodSistema", _codsistema);
                    op.Parametro.Add("IDSistema", _idsistema);
                    op.Parametro.Add("IDGruppo", _idgruppo);

                    if (_azione == EnumAzioni.ANN)
                    {
                        this.MovScheda.Azione = EnumAzioni.ANN;
                    }
                    if (_azione == EnumAzioni.CAN)
                        this.MovScheda.Azione = EnumAzioni.CAN;

                    op.TimeStamp.CodEntita = EnumEntita.ALA.ToString();
                    op.TimeStamp.IDEntita = _idmovalertallergieanamnesi;
                    op.TimeStamp.CodAzione = _azione.ToString();

                    op.MovScheda = this.MovScheda;

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);

                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    Database.ExecStoredProc("MSP_AggMovAlertAllergieAnamnesi", spcoll);

                    this.MovScheda.Salva();

                    Carica(_idmovalertallergieanamnesi);
                }
                else
                {

                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDPaziente", _idpaziente);
                    op.Parametro.Add("CodTipoAlertAllergiaAnamnesi", _codtipo);
                    if (_codsistema != string.Empty)
                        op.Parametro.Add("CodSistema", _codsistema);

                    if (_idsistema != string.Empty)
                        op.Parametro.Add("IDSistema", _idsistema);

                    if (_idgruppo != string.Empty)
                        op.Parametro.Add("IDGruppo", _idgruppo);

                    op.Parametro.Add("DataEvento", Database.dataOra105PerParametri(_dataevento));
                    op.Parametro.Add("DataEventoUTC", Database.dataOra105PerParametri(_dataevento.ToUniversalTime()));

                    op.TimeStamp.CodEntita = EnumEntita.ALA.ToString();
                    op.TimeStamp.CodAzione = _azione.ToString();

                    op.MovScheda = this.MovScheda;

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovAlertAllergieAnamnesi", spcoll);
                    if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                    {
                        _idmovalertallergieanamnesi = dt.Rows[0][0].ToString();

                        this.MovScheda.IDEntita = _idmovalertallergieanamnesi;
                        this.MovScheda.Salva();

                        _azione = EnumAzioni.MOD;
                        Carica(_idmovalertallergieanamnesi);
                    }
                }

                return bReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(@"MovAlertAllergieAnamnesi.Salva()" + Environment.NewLine + ex.Message, ex);
            }
        }

        public bool CopiaDaOrigine(MovAlertAllergieAnamnesi movtiorigine)
        {

            bool bReturn = true;

            try
            {

                _codua = movtiorigine.CodUA;
                _idmovalertallergieanamnesi = string.Empty;
                _idpaziente = movtiorigine.IDPaziente;
                _idepisodio = movtiorigine.IDEpisodio;
                _idtrasferimento = movtiorigine.IDTrasferimento;
                _codtipo = movtiorigine.CodTipo;
                _codstato = movtiorigine.CodStato;

                this.MovScheda = new MovScheda(movtiorigine.MovScheda.CodScheda,
                                                    (EnumEntita)Enum.Parse(typeof(EnumEntita), movtiorigine.MovScheda.CodEntita),
                                                    movtiorigine.MovScheda.CodUA, movtiorigine.MovScheda.IDPaziente,
                                                    movtiorigine.MovScheda.IDEpisodio, movtiorigine.MovScheda.IDTrasferimento, this.Ambiente);
                this.MovScheda.CopiaDaOrigine(movtiorigine.MovScheda, 1);



            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovAlertAllergieAnamnesi.CopiaDaOrigine()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;
        }

    }
}
