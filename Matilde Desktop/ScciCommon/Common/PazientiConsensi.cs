using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci
{

    [Serializable()]
    public class PazientiConsensi
    {

        private string _idpaziente = string.Empty;
        private string _codtipoconsenso = string.Empty;
        private string _codsistemaprovenienza = string.Empty;
        private string _idprovenienza = string.Empty;
        private string _codstatoconsenso = EnumStatoConsenso.ND.ToString();
        private DateTime _dataconsenso = DateTime.MinValue;
        private DateTime _datadisattivazione = DateTime.MinValue;
        private string _codoperatore = string.Empty;
        private string _cognomeoperatore = string.Empty;
        private string _nomeoperatore = string.Empty;
        private string _computeroperatore = string.Empty;

        private EnumAzioni _azione = EnumAzioni.MOD;

        public PazientiConsensi(Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = EnumAzioni.INS;
        }
        public PazientiConsensi(string idpaziente, string codtipoconsenso, Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            this.Carica(idpaziente, codtipoconsenso);
        }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string IDPaziente
        {
            get { return _idpaziente; }
            set { _idpaziente = value; }
        }

        public string CodTipoConsenso
        {
            get { return _codtipoconsenso; }
            set { _codtipoconsenso = value; }
        }

        public string CodSistemaProvenienza
        {
            get { return _codsistemaprovenienza; }
            set { _codsistemaprovenienza = value; }
        }

        public string IDProvenienza
        {
            get { return _idprovenienza; }
            set { _idprovenienza = value; }
        }

        public string CodStatoConsenso
        {
            get { return _codstatoconsenso; }
            set { _codstatoconsenso = value; }
        }

        public DateTime DataConsenso
        {
            get { return _dataconsenso; }
            set { _dataconsenso = value; }
        }

        public DateTime DataDisattivazione
        {
            get { return _datadisattivazione; }
            set { _datadisattivazione = value; }
        }

        public string CodOperatore
        {
            get { return _codoperatore; }
            set { _codoperatore = value; }
        }

        public string CognomeOperatore
        {
            get { return _cognomeoperatore; }
            set { _cognomeoperatore = value; }
        }

        public string NomeOperatore
        {
            get { return _nomeoperatore; }
            set { _nomeoperatore = value; }
        }

        public string ComputerOperatore
        {
            get { return _computeroperatore; }
            set { _computeroperatore = value; }
        }

        public ScciAmbiente Ambiente { get; set; }

        private void resetValori()
        {
            _idpaziente = string.Empty;
            _codtipoconsenso = string.Empty;
            _codsistemaprovenienza = string.Empty;
            _idprovenienza = string.Empty;
            _codstatoconsenso = EnumStatoConsenso.ND.ToString();
            _dataconsenso = DateTime.MinValue;
            _datadisattivazione = DateTime.MinValue;
            _codoperatore = string.Empty;
            _cognomeoperatore = string.Empty;
            _nomeoperatore = string.Empty;
            _computeroperatore = string.Empty;
        }

        private void Carica(string idpaziente, string codtipoconsenso)
        {
            try
            {

                Parametri op = null;
                op = new Parametri(this.Ambiente);
                op.Parametro.Add("IDPaziente", idpaziente);
                op.Parametro.Add("CodTipoConsenso", codtipoconsenso);

                op.TimeStamp.CodEntita = EnumEntita.CNS.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelPazientiConsensi", spcoll);

                if (dt.Rows.Count > 0)
                {
                    resetValori();

                    _idpaziente = dt.Rows[0]["IDPaziente"].ToString();
                    _codtipoconsenso = dt.Rows[0]["CodTipoConsenso"].ToString();

                    if (!dt.Rows[0].IsNull("CodSistemaProvenienza")) _codsistemaprovenienza = dt.Rows[0]["CodSistemaProvenienza"].ToString();
                    if (!dt.Rows[0].IsNull("IDProvenienza")) _idprovenienza = dt.Rows[0]["IDProvenienza"].ToString();
                    if (!dt.Rows[0].IsNull("CodStatoConsenso")) _codstatoconsenso = dt.Rows[0]["CodStatoConsenso"].ToString();

                    if (!dt.Rows[0].IsNull("DataConsenso")) _dataconsenso = (DateTime)dt.Rows[0]["DataConsenso"];
                    if (!dt.Rows[0].IsNull("DataDisattivazione")) _datadisattivazione = (DateTime)dt.Rows[0]["DataDisattivazione"];

                    if (!dt.Rows[0].IsNull("CodOperatore")) _codoperatore = dt.Rows[0]["CodOperatore"].ToString();
                    if (!dt.Rows[0].IsNull("CognomeOperatore")) _cognomeoperatore = dt.Rows[0]["CognomeOperatore"].ToString();
                    if (!dt.Rows[0].IsNull("NomeOperatore")) _nomeoperatore = dt.Rows[0]["NomeOperatore"].ToString();
                    if (!dt.Rows[0].IsNull("ComputerOperatore")) _computeroperatore = dt.Rows[0]["ComputerOperatore"].ToString();

                    _azione = EnumAzioni.MOD;

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
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

                if (_azione != EnumAzioni.INS)
                {

                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDPaziente", _idpaziente);
                    op.Parametro.Add("CodTipoConsenso", _codtipoconsenso);

                    if (_codsistemaprovenienza != string.Empty)
                        op.Parametro.Add("CodSistemaProvenienza", _codsistemaprovenienza);

                    if (_idprovenienza != string.Empty)
                        op.Parametro.Add("IDProvenienza", _idprovenienza);

                    if (_codstatoconsenso != string.Empty)
                        op.Parametro.Add("CodStatoConsenso", _codstatoconsenso);

                    if (_dataconsenso != DateTime.MinValue)
                        op.Parametro.Add("DataConsenso", Database.dataOra105PerParametri(_dataconsenso));

                    if (_datadisattivazione != DateTime.MinValue)
                        op.Parametro.Add("DataDisattivazione", Database.dataOra105PerParametri(_datadisattivazione));

                    if (_codoperatore != string.Empty)
                        op.Parametro.Add("CodOperatore", _codoperatore);

                    if (_cognomeoperatore != string.Empty)
                        op.Parametro.Add("CognomeOperatore", _cognomeoperatore);

                    if (_nomeoperatore != string.Empty)
                        op.Parametro.Add("NomeOperatore", _nomeoperatore);

                    if (_computeroperatore != string.Empty)
                        op.Parametro.Add("ComputerOperatore", _computeroperatore);

                    op.TimeStamp.CodEntita = EnumEntita.CNS.ToString();
                    op.TimeStamp.CodAzione = _azione.ToString();

                    spcoll = new SqlParameterExt[1];
                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    Database.ExecStoredProc("MSP_AggPazientiConsensi", spcoll);

                    Carica(_idpaziente, _codtipoconsenso);

                }
                else
                {

                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDPaziente", _idpaziente);
                    op.Parametro.Add("CodTipoConsenso", _codtipoconsenso);

                    if (_codsistemaprovenienza != string.Empty)
                        op.Parametro.Add("CodSistemaProvenienza", _codsistemaprovenienza);

                    if (_idprovenienza != string.Empty)
                        op.Parametro.Add("IDProvenienza", _idprovenienza);

                    if (_codstatoconsenso != string.Empty)
                        op.Parametro.Add("CodStatoConsenso", _codstatoconsenso);

                    if (_dataconsenso != DateTime.MinValue)
                        op.Parametro.Add("DataConsenso", Database.dataOra105PerParametri(_dataconsenso));

                    if (_datadisattivazione != DateTime.MinValue)
                        op.Parametro.Add("DataDisattivazione", Database.dataOra105PerParametri(_datadisattivazione));

                    if (_codoperatore != string.Empty)
                        op.Parametro.Add("CodOperatore", _codoperatore);

                    if (_cognomeoperatore != string.Empty)
                        op.Parametro.Add("CognomeOperatore", _cognomeoperatore);

                    if (_nomeoperatore != string.Empty)
                        op.Parametro.Add("NomeOperatore", _nomeoperatore);

                    if (_computeroperatore != string.Empty)
                        op.Parametro.Add("ComputerOperatore", _computeroperatore);

                    op.TimeStamp.CodEntita = EnumEntita.CNS.ToString();
                    op.TimeStamp.CodAzione = _azione.ToString();

                    spcoll = new SqlParameterExt[1];
                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataTable dt = Database.GetDataTableStoredProc("MSP_InsPazientiConsensi", spcoll);
                    if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                    {
                        _azione = EnumAzioni.MOD;
                        Carica(_idpaziente, _codtipoconsenso);
                    }

                }

                return bReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(@"PazientiConsensi.Salva()" + Environment.NewLine + ex.Message, ex);
            }

        }

    }

}
