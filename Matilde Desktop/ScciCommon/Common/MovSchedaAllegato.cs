using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci
{
    [Serializable()]
    public class MovSchedaAllegato
    {

        private string _idmovschedaallegato = string.Empty;
        private string _idmovscheda = string.Empty;
        private int _idnum = -1;

        private string _codcampo = string.Empty;
        private string _codsezione = string.Empty;
        private int _sequenza = 0;
        private string _idgruppo = string.Empty;

        private byte[] _documento = null;
        private byte[] _anteprima = null;

        private string _nomefile = string.Empty;
        private string _estensione = string.Empty;
        private string _descrizioneallegato = string.Empty;
        private string _descrizionecampo = string.Empty;

        private string _codtipoallegatoscheda = string.Empty;
        private string _descrizionetipoallegatoscheda = string.Empty;
        private string _codstatoallegatoscheda = string.Empty;
        private string _descrizionestatoallegatoscheda = string.Empty;
        private string _colorestatoallegatoscheda = string.Empty;

        private DateTime _dataevento = DateTime.MinValue;
        private DateTime _datarilevazione = DateTime.MinValue;
        private DateTime _dataultimamodifica = DateTime.MinValue;

        private string _codutenterilevazione = string.Empty;
        private string _codutenteultimamodifica = string.Empty;

        private EnumAzioni _azione = EnumAzioni.MOD;

        public ScciAmbiente Ambiente { get; set; }

        public string IDMovSchedaAllegato
        {
            get { return _idmovschedaallegato; }
            set { _idmovschedaallegato = value; }
        }

        public string IDMovScheda
        {
            get { return _idmovscheda; }
            set { _idmovscheda = value; }
        }

        public int IDNum
        {
            get { return _idnum; }
        }

        public string CodCampo
        {
            get { return _codcampo; }
            set { _codcampo = value; }
        }

        public string CodSezione
        {
            get { return _codsezione; }
            set { _codsezione = value; }
        }

        public int Sequenza
        {
            get { return _sequenza; }
            set { _sequenza = value; }
        }

        public int SequenzaNuova { get; set; }

        public string CodCampoNuovo { get; set; }

        public string IDGruppo
        {
            get { return _idgruppo; }
            set { _idgruppo = value; }
        }

        public byte[] Documento
        {
            get { return _documento; }
            set { _documento = value; }
        }

        public byte[] Anteprima
        {
            get { return _anteprima; }
            set { _anteprima = value; }
        }

        public string NomeFile
        {
            get { return _nomefile; }
            set { _nomefile = value; }
        }

        public string Estensione
        {
            get { return _estensione; }
            set { _estensione = value; }
        }

        public string DescrizioneAllegato
        {
            get { return _descrizioneallegato; }
            set { _descrizioneallegato = value; }
        }

        public string DescrizioneCampo
        {
            get { return _descrizionecampo; }
            set { _descrizionecampo = value; }
        }

        public string CodTipoAllegatoScheda
        {
            get { return _codtipoallegatoscheda; }
            set { _codtipoallegatoscheda = value; }
        }

        public string DescrizioneTipoAllegatoScheda
        {
            get { return _descrizionetipoallegatoscheda; }
            set { _descrizionetipoallegatoscheda = value; }
        }

        public string CodStatoAllegatoScheda
        {
            get { return _codstatoallegatoscheda; }
            set { _codstatoallegatoscheda = value; }
        }

        public string DescrizioneStatoAllegatoScheda
        {
            get { return _descrizionestatoallegatoscheda; }
            set { _descrizionestatoallegatoscheda = value; }
        }

        public string ColoreStatoAllegatoScheda
        {
            get { return _colorestatoallegatoscheda; }
            set { _colorestatoallegatoscheda = value; }
        }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public DateTime DataEvento
        {
            get { return _dataevento; }
            set { _dataevento = value; }
        }

        public DateTime DataRilevazione
        {
            get { return _datarilevazione; }
        }

        public DateTime DataUltimaModifica
        {
            get { return _dataultimamodifica; }
        }

        public string CodUtenteUltimaModifica
        {
            get { return _codutenteultimamodifica; }
        }

        public string CodUtenteRilevazione
        {
            get { return _codutenterilevazione; }
        }


        public MovSchedaAllegato(string idmovschedaallegato, EnumTipoRichiestaAllegatoScheda tiporichiesta, DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            this.Carica(idmovschedaallegato, tiporichiesta);
        }

        public MovSchedaAllegato(string idmovscheda, string codcampo, DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _codcampo = codcampo;

            _codstatoallegatoscheda = EnumStatoAllegatoScheda.IC.ToString();
            _azione = EnumAzioni.INS;

        }

        public bool Salva()
        {
            return this.Salva(true);
        }
        public bool Salva(bool RicaricaMovimento)
        {

            bool bReturn = true;
            Parametri op = null;
            SqlParameterExt[] spcoll;
            string xmlParam = "";

            try
            {

                if (_idmovschedaallegato == string.Empty || _idmovschedaallegato.Trim() == "")
                {
                    if (this.IDMovScheda == null || this.IDMovScheda == string.Empty || this.IDMovScheda.Trim() == "")
                        bReturn = false;
                    else
                    {

                        op = new Parametri(this.Ambiente);
                        op.Parametro.Add("IDScheda", _idmovscheda);
                        op.Parametro.Add("CodCampo", _codcampo);
                        op.Parametro.Add("CodSezione", _codsezione);
                        op.Parametro.Add("Sequenza", _sequenza.ToString());
                        op.Parametro.Add("IDGruppo", _idgruppo);
                        op.Parametro.Add("Documento", Convert.ToBase64String(_documento));
                        op.Parametro.Add("Anteprima", Convert.ToBase64String(_anteprima));

                        op.Parametro.Add("NomeFile", RtfProcs.EncodeTo64(_nomefile));
                        op.Parametro.Add("Estensione", _estensione);
                        op.Parametro.Add("DescrizioneAllegato", _descrizioneallegato);
                        op.Parametro.Add("DescrizioneCampo", _descrizionecampo);
                        op.Parametro.Add("CodTipoAllegatoScheda", _codtipoallegatoscheda);

                        op.Parametro.Add("CodStatoAllegatoScheda", EnumStatoAllegatoScheda.IC.ToString());

                        op.Parametro.Add("DataEvento", Database.dataOra105PerParametri(_dataevento));
                        op.Parametro.Add("DataEventoUTC", Database.dataOra105PerParametri(_dataevento.ToUniversalTime()));


                        op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                        op.TimeStamp.CodAzione = EnumAzioni.INS.ToString();

                        spcoll = new SqlParameterExt[1];

                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovSchedeAllegati", spcoll);
                        if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                        {
                            _idmovschedaallegato = dt.Rows[0][0].ToString();
                            if (RicaricaMovimento == true) { Carica(_idmovschedaallegato, EnumTipoRichiestaAllegatoScheda.LISTA); }
                        }
                        else
                        {
                            bReturn = false;
                        }
                    }
                }
                else
                {
                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDAllegato", _idmovschedaallegato);
                    op.Parametro.Add("IDScheda", _idmovscheda);
                    op.Parametro.Add("CodCampo", _codcampo);
                    op.Parametro.Add("CodSezione", _codsezione);
                    op.Parametro.Add("Sequenza", _sequenza.ToString());
                    op.Parametro.Add("IDGruppo", _idgruppo);
                    if (_documento != null) { op.Parametro.Add("Documento", Convert.ToBase64String(_documento)); };
                    if (_anteprima != null) { op.Parametro.Add("Anteprima", Convert.ToBase64String(_anteprima)); }

                    op.Parametro.Add("NomeFile", RtfProcs.EncodeTo64(_nomefile));
                    op.Parametro.Add("Estensione", _estensione);
                    op.Parametro.Add("DescrizioneAllegato", _descrizioneallegato);
                    op.Parametro.Add("DescrizioneCampo", _descrizionecampo);
                    op.Parametro.Add("CodTipoAllegatoScheda", _codtipoallegatoscheda);

                    if (_azione == EnumAzioni.CAN)
                        op.Parametro.Add("CodStatoAllegatoScheda", EnumStatoAllegatoScheda.CA.ToString());
                    else if (_azione == EnumAzioni.ANN)
                        op.Parametro.Add("CodStatoAllegatoScheda", EnumStatoAllegatoScheda.CA.ToString());
                    else
                    {
                        op.Parametro.Add("CodStatoAllegatoScheda", _codstatoallegatoscheda);
                    }

                    op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                    op.TimeStamp.IDEntita = _idmovschedaallegato;
                    op.TimeStamp.CodAzione = _azione.ToString();

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_AggMovSchedeAllegati", spcoll);

                    if (RicaricaMovimento) Carica(_idmovschedaallegato, EnumTipoRichiestaAllegatoScheda.LISTA);

                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovSchedaAllegato.Salva()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;

        }

        public bool Cancella()
        {

            try
            {
                _codstatoallegatoscheda = EnumStatoAllegatoScheda.CA.ToString();
                Azione = EnumAzioni.CAN;

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idmovscheda, EnumTipoRichiestaAllegatoScheda.LISTA);
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

        private void Carica(string idmovschedaallegato, EnumTipoRichiestaAllegatoScheda tiporichiesta)
        {
            try
            {

                Parametri op = new Parametri(this.Ambiente);
                op.Parametro.Add("IDSchedaAllegato", idmovschedaallegato);
                op.Parametro.Add("TipoRichiesta", tiporichiesta.ToString());

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovSchedaAllegati", spcoll);

                if (dt.Rows.Count > 0)
                {

                    _idmovschedaallegato = dt.Rows[0]["ID"].ToString();
                    _idnum = (int)dt.Rows[0]["IDNum"];
                    _idmovscheda = dt.Rows[0]["IDScheda"].ToString();
                    _codcampo = dt.Rows[0]["CodCampo"].ToString();
                    _codsezione = dt.Rows[0]["CodSezione"].ToString();
                    _sequenza = (int)dt.Rows[0]["Sequenza"];

                    CodCampoNuovo = _codcampo;
                    SequenzaNuova = _sequenza;

                    if (!dt.Rows[0].IsNull("Documento")) _documento = (byte[])dt.Rows[0]["Documento"];
                    if (!dt.Rows[0].IsNull("Anteprima")) _anteprima = (byte[])dt.Rows[0]["Anteprima"];

                    _nomefile = dt.Rows[0]["NomeFile"].ToString();
                    _estensione = dt.Rows[0]["Estensione"].ToString();
                    _descrizioneallegato = dt.Rows[0]["DescrizioneAllegato"].ToString();
                    _descrizionecampo = dt.Rows[0]["DescrizioneCampo"].ToString();

                    _codtipoallegatoscheda = dt.Rows[0]["CodTipoAllegatoScheda"].ToString();
                    _descrizionetipoallegatoscheda = dt.Rows[0]["TipoAllegatoScheda"].ToString();
                    _codstatoallegatoscheda = dt.Rows[0]["CodStatoAllegatoScheda"].ToString();
                    _descrizionestatoallegatoscheda = dt.Rows[0]["StatoAllegatoScheda"].ToString();
                    _colorestatoallegatoscheda = dt.Rows[0]["ColoreStatoAllegatoScheda"].ToString();

                    _dataevento = dt.Rows[0]["DataEvento"] != DBNull.Value ? (DateTime)dt.Rows[0]["DataEvento"] : DateTime.MinValue;
                    _datarilevazione = dt.Rows[0]["DataRilevazione"] != DBNull.Value ? (DateTime)dt.Rows[0]["DataRilevazione"] : DateTime.MinValue;
                    _dataultimamodifica = dt.Rows[0]["DataUltimaModifica"] != DBNull.Value ? (DateTime)dt.Rows[0]["DataUltimaModifica"] : DateTime.MinValue;
                    _codutenterilevazione = dt.Rows[0]["CodUtenteRilevazione"].ToString();
                    _codutenteultimamodifica = dt.Rows[0]["CodUtenteUltimaModifica"].ToString();

                    _azione = EnumAzioni.MOD;

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
