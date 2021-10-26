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
    public class MovParametroVitale
    {

        private string _codua = string.Empty;
        private string _idmovparametrovitale = string.Empty;
        private string _idpaziente = string.Empty;
        private string _idepisodio = string.Empty;
        private string _idtrasferimento = string.Empty;
        private string _codstatoparametrovitale = string.Empty;
        private string _codtipoparametrovitale = string.Empty;
        private string _codutenterilevazione = string.Empty;
        private DateTime _dataevento = DateTime.MinValue;
        private DateTime _datainserimento = DateTime.MinValue;

        private string _descrtipoparametrovitale = string.Empty;
        private string _descrstatoparametrovitale = string.Empty;
        private string _descrutenterilevazione = string.Empty;

        private string _codsistema = string.Empty;
        private string _idsistema = string.Empty;

        private int _permessoModifica = 0;
        private int _permessoCancella = 0;
        private int _permessoGrafico = 0;

        private byte[] _icona = null;

        private string _idscheda = string.Empty;
        private string _codscheda = string.Empty;
        private int _versionescheda = 0;

        private EnumAzioni _azione = EnumAzioni.MOD;

        private string _anteprimartf = string.Empty;

        private MovScheda _movScheda = null;

        public MovParametroVitale(string idmovparametrovitale, Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = EnumAzioni.MOD;
            _idmovparametrovitale = idmovparametrovitale;
            this.Carica(idmovparametrovitale);
        }
        public MovParametroVitale(string idmovparametrovitale, EnumAzioni azione, Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = azione;
            _idmovparametrovitale = idmovparametrovitale;
            this.Carica(idmovparametrovitale);
        }
        public MovParametroVitale(string codua, string idpaziente, string idepisodio, string idtrasferimento, Scci.DataContracts.ScciAmbiente ambiente)
        {
            this.Ambiente = ambiente;
            _azione = EnumAzioni.INS;
            _codua = codua;
            _idpaziente = idpaziente;
            _idepisodio = idepisodio;
            _idtrasferimento = idtrasferimento;
            _idmovparametrovitale = "";
            _codscheda = "";
        }

        public String[] Filtro_CodTipo { get; set; }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string CodUA
        {
            get { return _codua; }
        }

        public string IDMovParametroVitale
        {
            get { return _idmovparametrovitale; }
            set
            {
                if (_idmovparametrovitale != value) _movScheda = null;
                _idmovparametrovitale = value;
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
                    if (_idmovparametrovitale != null && _idmovparametrovitale != string.Empty && _idmovparametrovitale.Trim() != "")
                    {
                        _movScheda = new MovScheda(EnumEntita.PVT.ToString(), _idmovparametrovitale, this.Ambiente);
                    }
                    else if (_codscheda != null && _codscheda != string.Empty && _codscheda.Trim() != "")
                    {
                        _movScheda = new MovScheda(_codscheda, EnumEntita.PVT, _codua, _idpaziente, _idepisodio, _idtrasferimento, this.Ambiente);
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

        public string CodTipoParametroVitale
        {
            get { return _codtipoparametrovitale; }
            set
            {
                if (_codtipoparametrovitale != value) _movScheda = null;
                _codtipoparametrovitale = value;
            }

        }

        public string DescrTipoParametroVitale
        {
            get { return _descrtipoparametrovitale; }
            set { _descrtipoparametrovitale = value; }
        }

        public string CodStatoParametroVitale
        {
            get { return _codstatoparametrovitale; }
            set { _codstatoparametrovitale = value; }
        }

        public string DescrStatoParametroVitale
        {
            get { return _descrstatoparametrovitale; }
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

        public int PermessoModifica
        {
            get { return _permessoModifica; }
            set { _permessoModifica = value; }
        }

        public int PermessoCancella
        {
            get { return _permessoCancella; }
            set { _permessoCancella = value; }
        }

        public int PermessoGrafico
        {
            get { return _permessoGrafico; }
            set { _permessoGrafico = value; }
        }

        public byte[] Icona
        {
            get { return _icona; }
        }

        public ScciAmbiente Ambiente { get; set; }

        public bool Cancella()
        {
            try
            {
                Azione = EnumAzioni.CAN;

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idmovparametrovitale);
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

            bool bReturn = true;

            try
            {
                Parametri op = null;
                SqlParameterExt[] spcoll;
                string xmlParam = "";
                if (_idmovparametrovitale != string.Empty && _idmovparametrovitale.Trim() != "")
                {
                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDParametroVitale", _idmovparametrovitale);
                    op.Parametro.Add("CodStatoParametroVitale", _codstatoparametrovitale);
                    op.Parametro.Add("DataEvento", Database.dataOra105PerParametri(_dataevento));
                    op.Parametro.Add("DataEventoUTC", Database.dataOra105PerParametri(_dataevento.ToUniversalTime()));

                    string valoriGrafici = string.Empty;
                    string valoriFUT = string.Empty;
                    this.generaValori(out valoriGrafici, out valoriFUT);
                    op.Parametro.Add("ValoriGrafici", valoriGrafici);
                    op.Parametro.Add("ValoriFUT", valoriFUT);

                    op.TimeStamp.CodEntita = EnumEntita.PVT.ToString();
                    op.TimeStamp.IDEntita = _idtrasferimento;
                    op.TimeStamp.CodAzione = _azione.ToString();

                    this.MovScheda.Azione = _azione;

                    op.MovScheda = this.MovScheda;

                    bReturn = this.MovScheda.Salva();
                    if (bReturn)
                    {
                        spcoll = new SqlParameterExt[1];

                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        Database.ExecStoredProc("MSP_AggMovParametriVitali", spcoll);
                    }
                }
                else
                {

                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("CodUA", _codua);
                    op.Parametro.Add("IDEpisodio", _idepisodio);
                    op.Parametro.Add("DataEvento", Database.dataOra105PerParametri(_dataevento));
                    op.Parametro.Add("DataEventoUTC", Database.dataOra105PerParametri(_dataevento.ToUniversalTime()));
                    op.Parametro.Add("CodTipoParametroVitale", _codtipoparametrovitale);

                    if (_codstatoparametrovitale != string.Empty && _codstatoparametrovitale.Trim() != "")
                        op.Parametro.Add("CodStatoParametroVitale", _codstatoparametrovitale);

                    if (_idtrasferimento != string.Empty && _idtrasferimento.Trim() != "")
                        op.Parametro.Add("IDTrasferimento", _idtrasferimento);

                    if (_codsistema != string.Empty && _codsistema.Trim() != "")
                        op.Parametro.Add("CodSistema", _codsistema);
                    if (_idsistema != string.Empty && _idsistema.Trim() != "")
                        op.Parametro.Add("IDSistema", _idsistema);

                    string valoriGrafici = string.Empty;
                    string valoriFUT = string.Empty;
                    this.generaValori(out valoriGrafici, out valoriFUT);
                    op.Parametro.Add("ValoriGrafici", valoriGrafici);
                    op.Parametro.Add("ValoriFUT", valoriFUT);

                    op.TimeStamp.CodEntita = EnumEntita.PVT.ToString();
                    op.TimeStamp.CodAzione = _azione.ToString();

                    op.MovScheda = this.MovScheda;

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovParametriVitali", spcoll);
                    if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                    {
                        _idmovparametrovitale = dt.Rows[0][0].ToString();

                        this.MovScheda.IDEntita = _idmovparametrovitale;
                        this.MovScheda.Azione = _azione;
                        bReturn = this.MovScheda.Salva();

                        _azione = EnumAzioni.MOD;

                        Carica(_idmovparametrovitale);
                    }
                    else
                        bReturn = false;
                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovParametroVitale.Salva()" + Environment.NewLine + ex.Message, ex);

            }

            return bReturn;

        }

        private void resetValori()
        {
            _codua = string.Empty;
            _idmovparametrovitale = string.Empty;
            _idpaziente = string.Empty;
            _idepisodio = string.Empty;
            _idtrasferimento = string.Empty;
            _codstatoparametrovitale = string.Empty;
            _codtipoparametrovitale = string.Empty;
            _codutenterilevazione = string.Empty;
            _dataevento = DateTime.MinValue;
            _datainserimento = DateTime.MinValue;

            _descrtipoparametrovitale = string.Empty;
            _descrstatoparametrovitale = string.Empty;
            _descrutenterilevazione = string.Empty;

            _codsistema = string.Empty;
            _idsistema = string.Empty;

            _permessoModifica = 0;
            _permessoCancella = 0;
            _permessoGrafico = 0;

            _icona = null;

            _idscheda = string.Empty;
            _codscheda = string.Empty;
            _versionescheda = 0;

            _anteprimartf = string.Empty;
        }

        private void Carica(string idmovparametrovitale)
        {
            try
            {

                Parametri op = new Parametri(this.Ambiente);
                op.Parametro.Add("IDParametroVitale", idmovparametrovitale);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovParametriVitali", spcoll);

                if (dt.Rows.Count > 0)
                {
                    resetValori();
                    _idmovparametrovitale = dt.Rows[0]["ID"].ToString();
                    _idpaziente = dt.Rows[0]["IDPaziente"].ToString();
                    _idepisodio = dt.Rows[0]["IDEpisodio"].ToString();
                    _idtrasferimento = dt.Rows[0]["IDTrasferimento"].ToString();

                    if (!dt.Rows[0].IsNull("CodUA")) _codua = dt.Rows[0]["CodUA"].ToString();
                    if (!dt.Rows[0].IsNull("CodStato")) _codstatoparametrovitale = dt.Rows[0]["CodStato"].ToString();
                    if (!dt.Rows[0].IsNull("DescStato")) _descrstatoparametrovitale = dt.Rows[0]["DescStato"].ToString();
                    if (!dt.Rows[0].IsNull("CodTipo")) _codtipoparametrovitale = dt.Rows[0]["CodTipo"].ToString();
                    if (!dt.Rows[0].IsNull("DescTipo")) _descrtipoparametrovitale = dt.Rows[0]["DescTipo"].ToString();
                    if (!dt.Rows[0].IsNull("CodUtente")) _codutenterilevazione = dt.Rows[0]["CodUtente"].ToString();
                    if (!dt.Rows[0].IsNull("DescrUtente")) _descrutenterilevazione = dt.Rows[0]["DescrUtente"].ToString();

                    if (!dt.Rows[0].IsNull("DataEvento")) _dataevento = (DateTime)dt.Rows[0]["DataEvento"];
                    if (!dt.Rows[0].IsNull("DataInserimento")) _datainserimento = (DateTime)dt.Rows[0]["DataInserimento"];

                    if (!dt.Rows[0].IsNull("CodSistema")) _codsistema = dt.Rows[0]["CodSistema"].ToString();
                    if (!dt.Rows[0].IsNull("IDSistema")) _idsistema = dt.Rows[0]["IDSistema"].ToString();

                    if (!dt.Rows[0].IsNull("PermessoCancella")) _permessoCancella = (int)dt.Rows[0]["PermessoCancella"];
                    if (!dt.Rows[0].IsNull("PermessoModifica")) _permessoModifica = (int)dt.Rows[0]["PermessoModifica"];
                    if (!dt.Rows[0].IsNull("PermessoGrafico")) _permessoGrafico = (int)dt.Rows[0]["PermessoGrafico"];

                    if (!dt.Rows[0].IsNull("Icona")) _icona = (byte[])dt.Rows[0]["Icona"];

                    if (!dt.Rows[0].IsNull("IDScheda")) _idscheda = dt.Rows[0]["IDScheda"].ToString();
                    if (!dt.Rows[0].IsNull("CodScheda")) _codscheda = dt.Rows[0]["CodScheda"].ToString();
                    if (!dt.Rows[0].IsNull("Versione")) _versionescheda = (int)dt.Rows[0]["Versione"];

                    if (!dt.Rows[0].IsNull("AnteprimaRTF")) _anteprimartf = dt.Rows[0]["AnteprimaRTF"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private void generaValori(out string valoriGrafici, out string valoriFUT)
        {

            valoriGrafici = string.Empty;
            valoriFUT = string.Empty;
            try
            {
                Parametri op = new Parametri(this.Ambiente);
                op.Parametro.Add("CodTipoParametroVitale", _codtipoparametrovitale);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelInfoParametroVitale", spcoll);

                if (dt.Rows.Count > 0)
                {

                    Gestore oGestore = CommonStatics.GetGestore(this.Ambiente);
                    oGestore.SchedaDatiXML = this.MovScheda.DatiXML;

                    if (dt.Rows[0]["CampiGrafici"].ToString() != "")
                    {
                        TipoParametroVitale param = XmlProcs.XmlDeserializeFromString<TipoParametroVitale>(dt.Rows[0]["CampiGrafici"].ToString());
                        ValoriPVT valori = new ValoriPVT();

                        foreach (DimensionePerGrafico dim in param.DimensioniPerGrafico)
                        {
                            ValorePVT valore = new ValorePVT();
                            valore.Nome = dim.Nome;
                            valore.Valore = (string)oGestore.LeggeValore(dim.DatoClinico.Campo, int.Parse(dim.DatoClinico.Sequenza));
                            valori.Valori.Add(valore);
                        }

                        valoriGrafici = valori.XmlSerializeToString();

                    }

                    if (dt.Rows[0]["CampiFUT"].ToString() != "")
                    {
                        TipoParametroVitale param = XmlProcs.XmlDeserializeFromString<TipoParametroVitale>(dt.Rows[0]["CampiFUT"].ToString());
                        ValoriPVT valori = new ValoriPVT();

                        foreach (DimensionePerFUT dim in param.DimensioniPerFUT)
                        {
                            ValorePVT valore = new ValorePVT();
                            valore.Nome = dim.Nome;
                            string sTranscodifica = oGestore.LeggeTranscodifica(dim.DatoClinico.Campo, int.Parse(dim.DatoClinico.Sequenza));
                            if (sTranscodifica == string.Empty)
                            {
                                valore.Valore = (string)oGestore.LeggeValore(dim.DatoClinico.Campo, int.Parse(dim.DatoClinico.Sequenza));
                            }
                            else
                            {
                                valore.Valore = sTranscodifica;
                            }
                            valori.Valori.Add(valore);
                        }

                        valoriFUT = valori.XmlSerializeToString();

                    }

                    oGestore = null;

                }
            }
            catch (Exception ex)
            {
                throw new Exception(@"MovParametroVitale.generaValori()" + Environment.NewLine + ex.Message, ex);
            }
            finally
            {
            }

        }

    }

}
