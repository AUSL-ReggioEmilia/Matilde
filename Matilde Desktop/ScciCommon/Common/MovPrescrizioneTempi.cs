using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;

using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;

using UnicodeSrl.Scci.RTF;

namespace UnicodeSrl.Scci
{

    [Serializable()]
    public class MovPrescrizioneTempi
    {
        private string _idprescrizionetempi = string.Empty;
        private string _idprescrizione = string.Empty;
        private string _codstatoprescrizionetempi = string.Empty;
        private string _descrstatoprescrizionetempi = string.Empty;
        private string _codtipoprescrizionetempi = string.Empty;
        private DateTime _dataevento = DateTime.MinValue;
        private DateTime _dataorainizio = DateTime.MinValue;
        private DateTime _dataorafine = DateTime.MinValue;
        private DateTime _datavalidazione = DateTime.MinValue;

        private bool _albisogno = false;
        private int _durata = 0;
        private bool _continuita = false;

        private int _periodicitagiorni = 0;
        private int _periodicitaore = 0;
        private int _periodicitaminuti = 0;

        private string _codutenterilevazione = string.Empty;
        private string _descrutenterilevazione = string.Empty;
        private string _codutenteultimamodifica = string.Empty;
        private string _descrutenteultimamodifica = string.Empty;
        private DateTime _dataultimamodifica = DateTime.MinValue;

        private string _posologia = string.Empty;
        private string _codprotocollo = string.Empty;
        private string _codtipoprotocollo = string.Empty;

        private EnumAzioni _azione = EnumAzioni.MOD;

        private int _permessoDaValidare = 0;
        private int _permessoModifica = 0;
        private int _permessoAnnulla = 0;
        private int _permessoCancella = 0;
        private int _permessoCopia = 0;

        private string _idultimotaskinfermieristicogenerato = string.Empty;

        private bool _manuale = false;
        private string _tempimanuali = string.Empty;

        private bool _inmemoria = false;

        private MovScheda _movScheda = null;

        private string _codua = string.Empty;
        private string _idpaziente = string.Empty;
        private string _idepisodio = string.Empty;
        private string _idtrasferimento = string.Empty;

        public MovPrescrizioneTempi(string idprescrizionetempi, string idprescrizione, DataContracts.ScciAmbiente ambiente)
        {
            this.resetValori();
            this.Ambiente = ambiente;
            _azione = EnumAzioni.MOD;
            _idprescrizionetempi = idprescrizionetempi;
            this.Carica(idprescrizionetempi, idprescrizione);
            this.CaricaTestata(idprescrizione);
        }
        public MovPrescrizioneTempi(string idprescrizionetempi, string idprescrizione, EnumAzioni azione, DataContracts.ScciAmbiente ambiente)
        {
            this.resetValori();
            this.Ambiente = ambiente;
            _azione = azione;
            _idprescrizionetempi = idprescrizionetempi;
            this.Carica(idprescrizionetempi, idprescrizione);
            this.CaricaTestata(idprescrizione);
        }
        public MovPrescrizioneTempi(Guid idprescrizione, DataContracts.ScciAmbiente ambiente)
        {
            this.resetValori();
            this.Ambiente = ambiente;
            _azione = EnumAzioni.INS;
            _idprescrizione = idprescrizione.ToString();
            _idprescrizionetempi = "";
            _dataevento = DateTime.Now;
            this.CaricaTestata(idprescrizione.ToString());
        }

        public ScciAmbiente Ambiente { get; set; }

        public EnumAzioni Azione
        {
            get { return _azione; }
            set { _azione = value; }
        }

        public string IDPrescrizioneTempi
        {
            get { return _idprescrizionetempi; }
            set { _idprescrizionetempi = value; }
        }

        public string IDPrescrizione
        {
            get { return _idprescrizione; }
            set { _idprescrizione = value; }
        }

        public string CodStatoPrescrizioneTempi
        {
            get { return _codstatoprescrizionetempi; }
            set { _codstatoprescrizionetempi = value; }
        }

        public string DescrStatoPrescrizioneTempi
        {
            get { return _descrstatoprescrizionetempi; }
        }

        public string CodTipoPrescrizioneTempi
        {
            get { return _codtipoprescrizionetempi; }
            set { _codtipoprescrizionetempi = value; }
        }

        public DateTime DataEvento
        {
            get { return _dataevento; }
            set { _dataevento = value; }
        }

        public DateTime DataOraInizio
        {
            get { return _dataorainizio; }
            set { _dataorainizio = value; }
        }

        public DateTime DataOraFine
        {
            get { return _dataorafine; }
            set { _dataorafine = value; }
        }

        public DateTime DataValidazione
        {
            get { return _datavalidazione; }
            set { _datavalidazione = value; }
        }

        public bool AlBisogno
        {
            get { return _albisogno; }
            set { _albisogno = value; }
        }

        public int Durata
        {
            get { return _durata; }
            set { _durata = value; }
        }

        public DateTime DurataOrario
        {
            get { return DateTime.MinValue.AddMinutes(this.Durata); }
        }

        public bool Continuita
        {
            get { return _continuita; }
            set { _continuita = value; }
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

        public string CodUtenteRilevazione
        {
            get { return _codutenterilevazione; }
        }

        public string DescrUtenteRilevazione
        {
            get { return _descrutenterilevazione; }
        }

        public string CodUtenteUltimaModifica
        {
            get { return _codutenteultimamodifica; }
        }

        public string DescrUtenteUltimaModifica
        {
            get { return _descrutenteultimamodifica; }
        }

        public DateTime DataUltimaModifica
        {
            get { return _dataultimamodifica; }
        }

        public string Posologia
        {
            get { return _posologia; }
            set { _posologia = value; }
        }
        public string PosologiaxParamStored
        {
            get { return UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(this.Posologia); }
        }

        public string CodProtocollo
        {
            get { return _codprotocollo; }
            set { _codprotocollo = value; }
        }

        public string CodProtocolloxParamStored
        {
            get { return UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(this.CodProtocollo); }
        }

        public string CodTipoProtocollo
        {
            get { return _codtipoprotocollo; }
            set { _codtipoprotocollo = value; }
        }

        public int PermessoDaValidare
        {
            get { return _permessoDaValidare; }
        }

        public int PermessoModifica
        {
            get { return _permessoModifica; }
        }

        public int PermessoAnnulla
        {
            get { return _permessoAnnulla; }
        }

        public int PermessoCancella
        {
            get { return _permessoCancella; }
        }

        public int PermessoCopia
        {
            get { return _permessoCopia; }
        }

        public List<IntervalloTempi> Periodicita
        {
            get { return this.GeneraPeriodicita(); }
        }

        public string IDUltimoTaskInfermieristicoGenerato
        {
            get { return _idultimotaskinfermieristicogenerato; }
        }

        public bool Manuale
        {
            get { return _manuale; }
            set { _manuale = value; }
        }

        public string TempiManuali
        {
            get { return _tempimanuali; }
            set { _tempimanuali = value; }
        }

        public bool InMemoria
        {
            get { return _inmemoria; }
            set { _inmemoria = value; }
        }

        public string CodSchedaPosologia
        {
            get
            {

                string sret = string.Empty;

                try
                {

                    Parametri op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDPrescrizione", _idprescrizione);

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_SelCodSchedaPosologia", spcoll);

                    if (dt.Rows.Count == 1)
                    {
                        sret = dt.Rows[0][0].ToString();
                    }

                }
                catch (Exception)
                {

                }

                return sret;

            }
        }

        public MovScheda MovScheda
        {
            get
            {

                if (_movScheda == null)
                {
                    string _codscheda = this.CodSchedaPosologia;
                    if (_idprescrizionetempi != null && _idprescrizionetempi != string.Empty && _idprescrizionetempi.Trim() != "")
                    {
                        _movScheda = new MovScheda(EnumEntita.PRT.ToString(), _idprescrizionetempi, this.Ambiente);
                    }
                    else if (_codscheda != null && _codscheda != string.Empty && _codscheda.Trim() != "")
                    {
                        _movScheda = new MovScheda(_codscheda, EnumEntita.PRT, this.CodUA, this.IDPaziente, this.IDEpisodio, this.IDTrasferimento, this.Ambiente);
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

        public string CodUA
        {
            get { return _codua; }
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

        public bool Cancella()
        {
            try
            {
                Azione = EnumAzioni.CAN;

                if (Salva())
                {
                    if (this.CancellaTaskInfermieristici())
                    {
                        Azione = EnumAzioni.MOD;
                        Carica(_idprescrizionetempi, _idprescrizione);
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Sospendi()
        {
            try
            {
                Azione = EnumAzioni.ANN;

                if (Salva())
                {
                    if (this.SospendiTaskInfermieristici())
                    {
                        Azione = EnumAzioni.MOD;
                        Carica(_idprescrizionetempi, _idprescrizione);
                        return true;
                    }
                    else
                        return false;
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
            return this.Salva(true);
        }
        public bool Salva(bool FlagGeneraRTF)
        {

            bool bReturn = true;

            try
            {
                Parametri op = null;
                SqlParameterExt[] spcoll;
                string xmlParam = "";
                if (_idprescrizionetempi != string.Empty && _idprescrizionetempi.Trim() != "")
                {
                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDPrescrizioneTempi", _idprescrizionetempi);
                    op.Parametro.Add("IDPrescrizione", _idprescrizione);
                    op.Parametro.Add("CodStatoPrescrizioneTempi", _codstatoprescrizionetempi);
                    op.Parametro.Add("CodTipoPrescrizioneTempi", _codtipoprescrizionetempi);
                    op.Parametro.Add("AlBisogno", _albisogno.ToString());
                    op.Parametro.Add("DataOraInizio", Database.dataOra105PerParametri(_dataorainizio));
                    op.Parametro.Add("DataOraFine", Database.dataOra105PerParametri(_dataorafine));
                    op.Parametro.Add("Durata", _durata.ToString());
                    op.Parametro.Add("Continuita", _continuita.ToString());
                    op.Parametro.Add("PeriodicitaGiorni", _periodicitagiorni.ToString());
                    op.Parametro.Add("PeriodicitaOre", _periodicitaore.ToString());
                    op.Parametro.Add("PeriodicitaMinuti", _periodicitaminuti.ToString());


                    if (this.CodSchedaPosologia != string.Empty && this.MovScheda != null)
                    {
                        op.Parametro.Add("Posologia", UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(this.MovScheda.AnteprimaTXT));
                    }
                    else
                    {
                        op.Parametro.Add("Posologia", this.PosologiaxParamStored);
                    }

                    op.Parametro.Add("CodProtocollo", this.CodProtocolloxParamStored);


                    op.Parametro.Add("Manuale", _manuale.ToString());
                    op.Parametro.Add("TempiManuali", _tempimanuali);

                    op.TimeStamp.CodEntita = EnumEntita.PRF.ToString();
                    op.TimeStamp.IDEntita = _idprescrizionetempi;
                    op.TimeStamp.CodAzione = _azione.ToString();

                    op.MovScheda = new MovScheda(EnumEntita.PRF.ToString(), _idprescrizione, this.Ambiente);

                    if (this.CodSchedaPosologia != string.Empty)
                    {
                        this.MovScheda.Salva(true, FlagGeneraRTF);
                    }
                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);

                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    Database.ExecStoredProc("MSP_AggMovPrescrizioniTempi", spcoll);

                }
                else
                {

                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDPrescrizione", _idprescrizione);
                    op.Parametro.Add("CodStatoPrescrizioneTempi", _codstatoprescrizionetempi);
                    op.Parametro.Add("CodTipoPrescrizioneTempi", _codtipoprescrizionetempi);
                    op.Parametro.Add("AlBisogno", _albisogno.ToString());
                    op.Parametro.Add("DataOraInizio", Database.dataOra105PerParametri(_dataorainizio));
                    op.Parametro.Add("DataOraFine", Database.dataOra105PerParametri(_dataorafine));
                    op.Parametro.Add("Durata", _durata.ToString());
                    op.Parametro.Add("Continuita", _continuita.ToString());
                    op.Parametro.Add("PeriodicitaGiorni", _periodicitagiorni.ToString());
                    op.Parametro.Add("PeriodicitaOre", _periodicitaore.ToString());
                    op.Parametro.Add("PeriodicitaMinuti", _periodicitaminuti.ToString());

                    if (this.CodSchedaPosologia != string.Empty && this.MovScheda != null)
                    {
                        op.Parametro.Add("Posologia", UnicodeSrl.Scci.Statics.CommonStatics.EncodeBase64StoredParameter(this.MovScheda.AnteprimaTXT));
                    }
                    else
                    {
                        op.Parametro.Add("Posologia", this.PosologiaxParamStored);
                    }

                    if (_codprotocollo != string.Empty) op.Parametro.Add("CodProtocollo", this.CodProtocolloxParamStored);

                    op.Parametro.Add("Manuale", _manuale.ToString());
                    op.Parametro.Add("TempiManuali", _tempimanuali);

                    op.TimeStamp.CodEntita = EnumEntita.PRF.ToString();
                    op.TimeStamp.IDEntita = _idprescrizionetempi;
                    op.TimeStamp.CodAzione = _azione.ToString();

                    op.MovScheda = new MovScheda(EnumEntita.PRF.ToString(), _idprescrizione, this.Ambiente);

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);

                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovPrescrizioniTempi", spcoll);
                    if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                    {
                        _idprescrizionetempi = dt.Rows[0][0].ToString();

                        if (this.CodSchedaPosologia != string.Empty)
                        {
                            this.MovScheda.IDEntita = _idprescrizionetempi;
                            this.MovScheda.Azione = this.Azione;
                            this.MovScheda.Salva(true, FlagGeneraRTF);
                        }

                        _azione = EnumAzioni.MOD;

                        Carica(_idprescrizionetempi, _idprescrizione);
                    }
                    else
                        bReturn = false;
                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovPrescrizioneTempi.Salva()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;

        }

        public List<IntervalloTempi> GeneraPeriodicita()
        {
            List<IntervalloTempi> listaperiodicita = new List<IntervalloTempi>();
            DateTime dataperiodicita = DateTime.MinValue;

            if (!this.AlBisogno)
            {

                if (this.Manuale == false)
                {

                    if (this.CodProtocollo == "")
                    {
                        if (this.DataOraInizio != DateTime.MinValue && this.DataOraFine != DateTime.MinValue)
                        {
                            DateTime orainizio = new DateTime(this.DataOraInizio.Year, this.DataOraInizio.Month, this.DataOraInizio.Day, this.DataOraInizio.Hour, this.DataOraInizio.Minute, 0);
                            DateTime orafine = new DateTime(this.DataOraFine.Year, this.DataOraFine.Month, this.DataOraFine.Day, this.DataOraFine.Hour, this.DataOraFine.Minute, 0);

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
                                        dataperiodicita = this.DataOraInizio.AddMinutes(int.Parse(orow["Delta"].ToString()));
                                        listaperiodicita.Add(new IntervalloTempi(dataperiodicita, dataperiodicita.AddMinutes(this.Durata), orow["DescProtocollo"].ToString(), orow["DescTempo"].ToString()));
                                    }
                                }
                            }
                            else if (dt.Rows[0]["CodTipoProtocollo"].ToString() == EnumTipoProtocollo.ORA.ToString())
                            {

                                dataperiodicita = this.DataOraInizio;
                                DateTime ora = DateTime.MinValue;
                                while (dataperiodicita <= this.DataOraFine)
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

                                            if (dataperiodicita >= this.DataOraInizio && dataperiodicita <= this.DataOraFine)
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
                }
                else
                {

                    DataSet oDs = new DataSet();
                    DataTable oDt = new DataTable();
                    DataColumn oDc = null;
                    oDc = new DataColumn("DataOraInizio", typeof(DateTime));
                    oDt.Columns.Add(oDc);
                    oDc = new DataColumn("DataOraFine", typeof(DateTime));
                    oDt.Columns.Add(oDc);
                    oDc = new DataColumn("NomeProtocollo", typeof(string));
                    oDt.Columns.Add(oDc);
                    oDc = new DataColumn("EtichettaTempo", typeof(string));
                    oDt.Columns.Add(oDc);

                    oDs.Tables.Add(oDt);

                    System.IO.StringReader xmlSR = new System.IO.StringReader(this.TempiManuali);
                    oDs.ReadXml(xmlSR, XmlReadMode.IgnoreSchema);
                    oDs.Tables[0].DefaultView.Sort = "DataOraInizio";

                    foreach (DataRow oRow in oDs.Tables[0].Rows)
                    {
                        listaperiodicita.Add(new IntervalloTempi((DateTime)oRow["DataOraInizio"], (DateTime)oRow["DataOraFine"], "", oRow["EtichettaTempo"].ToString()));
                    }

                }

            }

            return listaperiodicita;

        }

        public bool CopiaDaOrigine(MovPrescrizioneTempi movprtorigine)
        {
            bool bReturn = true;

            try
            {
                _idprescrizionetempi = string.Empty;
                _idprescrizione = movprtorigine.IDPrescrizione;
                _codstatoprescrizionetempi = Enums.EnumStatoPrescrizioneTempi.IC.ToString();
                _descrstatoprescrizionetempi = movprtorigine.DescrStatoPrescrizioneTempi;
                _codtipoprescrizionetempi = movprtorigine.CodTipoPrescrizioneTempi;
                _dataevento = DateTime.Now;
                if (movprtorigine.DataOraInizio > DateTime.Now)
                {
                    _dataorainizio = movprtorigine.DataOraInizio;
                    _dataorafine = movprtorigine.DataOraFine;
                }
                else
                {
                    _dataorainizio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                                                    movprtorigine.DataOraInizio.Hour, movprtorigine.DataOraInizio.Minute, 0);
                    if (movprtorigine.DataOraFine == DateTime.MinValue)
                    {
                        _dataorafine = DateTime.MinValue;
                    }
                    else
                    {
                        _dataorafine = _dataorainizio.AddTicks((movprtorigine.DataOraFine - movprtorigine.DataOraInizio).Ticks);
                    }
                }

                _datavalidazione = DateTime.MinValue;
                _albisogno = movprtorigine.AlBisogno;
                _durata = movprtorigine.Durata;
                _continuita = movprtorigine.Continuita;

                _periodicitagiorni = movprtorigine.PeriodicitaGiorni;
                _periodicitaore = movprtorigine.PeriodicitaOre;
                _periodicitaminuti = movprtorigine.PeriodicitaMinuti;

                _codutenterilevazione = Ambiente.Codlogin;
                _descrutenterilevazione = string.Empty;
                _codutenteultimamodifica = Ambiente.Codlogin;
                _descrutenteultimamodifica = string.Empty;
                _dataultimamodifica = DateTime.Now;

                _posologia = movprtorigine.Posologia;
                _codprotocollo = movprtorigine.CodProtocollo;

                _manuale = movprtorigine.Manuale;
                _tempimanuali = movprtorigine.TempiManuali;
                _codtipoprotocollo = movprtorigine.CodTipoProtocollo;

                _permessoDaValidare = movprtorigine.PermessoDaValidare;
                _permessoModifica = movprtorigine.PermessoModifica;
                _permessoAnnulla = movprtorigine.PermessoAnnulla;
                _permessoCancella = movprtorigine.PermessoCancella;
                _permessoCopia = movprtorigine.PermessoCopia;
            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovPrescrizioneTempi.CopiaDaOrigine()" + Environment.NewLine + ex.Message, ex);

            }

            return bReturn;
        }

        public bool CreaTaskInfermieristici(EnumCodSistema sistema, string codtipotaskinfermieristico, EnumTipoRegistrazione tiporegistrazione)
        {
            bool bReturn;

            bReturn = CreaTaskInfermieristici(sistema, codtipotaskinfermieristico, tiporegistrazione, true);

            return bReturn;
        }
        public bool CreaTaskInfermieristici(EnumCodSistema sistema, string codtipotaskinfermieristico, EnumTipoRegistrazione tiporegistrazione, bool FlagGeneraRtf)
        {
            bool bReturn = true;
            string anteprimartfinizio = string.Empty;
            string anteprimartffine = string.Empty;
            RtfFiles rtf = new RtfFiles();
            System.Drawing.Font f = rtf.getFontFromString(Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF), false, false);
            System.Drawing.Font fbold = rtf.getFontFromString(Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF), true, false);

            try
            {
                MovPrescrizione movpr = new MovPrescrizione(this.IDPrescrizione, false, this.Ambiente);

                Parametri op = new Parametri(this.Ambiente);
                op.Parametro.Add("CodEntita", EnumEntita.WKI.ToString());
                op.Parametro.Add("CodTipo", codtipotaskinfermieristico);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelSchedaDaTipo", spcoll);

                if (dt.Rows.Count > 0)
                {

                    foreach (IntervalloTempi inttempo in this.GeneraPeriodicita())
                    {
                        if (bReturn)
                        {
                            if (FlagGeneraRtf == true)
                            {
                                anteprimartfinizio = rtf.initRtf(f);
                                if (this.Continuita == true) { anteprimartfinizio = rtf.appendRtfText(anteprimartfinizio, @"INIZIO SOMMINISTRAZIONE\line ", f); }
                                anteprimartffine = rtf.initRtf(f);
                                if (this.Continuita == true) { anteprimartffine = rtf.appendRtfText(anteprimartffine, @"FINE SOMMINISTRAZIONE\line ", f); }

                                if (this.Posologia != null && this.Posologia != string.Empty)
                                {
                                    anteprimartfinizio = rtf.appendRtfText(anteprimartfinizio, "Posologia: ", fbold);
                                    anteprimartfinizio = rtf.appendRtfText(anteprimartfinizio, this.Posologia + @"\line ", f);

                                    anteprimartffine = rtf.appendRtfText(anteprimartffine, "Posologia: ", fbold);
                                    anteprimartffine = rtf.appendRtfText(anteprimartffine, this.Posologia + @"\line ", f);
                                }
                            }

                            if (this.CodProtocollo != string.Empty)
                            {
                                if (FlagGeneraRtf == true)
                                {
                                    if (inttempo.NomeProtocollo != string.Empty || inttempo.EtichettaTempo != string.Empty)
                                    {
                                        if (inttempo.NomeProtocollo != string.Empty)
                                        {
                                            anteprimartfinizio = rtf.appendRtfText(anteprimartfinizio, inttempo.NomeProtocollo, f);
                                            anteprimartffine = rtf.appendRtfText(anteprimartffine, inttempo.NomeProtocollo, f);
                                        }
                                        if (inttempo.NomeProtocollo != string.Empty && inttempo.EtichettaTempo != string.Empty)
                                        {
                                            anteprimartfinizio = rtf.appendRtfText(anteprimartfinizio, " - ", f);
                                            anteprimartffine = rtf.appendRtfText(anteprimartffine, " - ", f);
                                        }
                                        if (inttempo.EtichettaTempo != string.Empty)
                                        {
                                            anteprimartfinizio = rtf.appendRtfText(anteprimartfinizio, inttempo.EtichettaTempo, f);
                                            anteprimartffine = rtf.appendRtfText(anteprimartffine, inttempo.EtichettaTempo, f);
                                        }
                                        anteprimartfinizio = rtf.appendRtfText(anteprimartfinizio, @"\line ", f);
                                        anteprimartffine = rtf.appendRtfText(anteprimartffine, @"\line ", f);
                                    }
                                    string rtfcopia = movpr.MovScheda.AnteprimaRTF;
                                    anteprimartfinizio = rtf.joinRtf(rtfcopia, anteprimartfinizio);
                                    rtfcopia = movpr.MovScheda.AnteprimaRTF;
                                    anteprimartffine = rtf.joinRtf(rtfcopia, anteprimartffine);
                                }
                            }
                            else
                            {
                                if (FlagGeneraRtf == true)
                                {
                                    string rtfcopia = movpr.MovScheda.AnteprimaRTF;
                                    anteprimartfinizio = rtf.joinRtf(rtfcopia, anteprimartfinizio);
                                    rtfcopia = movpr.MovScheda.AnteprimaRTF;
                                    anteprimartffine = rtf.joinRtf(rtfcopia, anteprimartffine);
                                }
                            }
                            bReturn = this.GeneraSingoloTask(movpr.CodUA, movpr.IDPaziente, movpr.IDEpisodio,
                                                             movpr.IDTrasferimento, sistema, codtipotaskinfermieristico,
                                                             dt.Rows[0]["CodScheda"].ToString(), inttempo.DataOraInizio, tiporegistrazione,
                                                             anteprimartfinizio, "", FlagGeneraRtf);

                            if (bReturn && this.Continuita)
                            {
                                bReturn = this.GeneraSingoloTask(movpr.CodUA, movpr.IDPaziente, movpr.IDEpisodio,
                                                                 movpr.IDTrasferimento, sistema, codtipotaskinfermieristico,
                                                                 dt.Rows[0]["CodScheda"].ToString(), inttempo.DataOraFine, tiporegistrazione,
                                                                 anteprimartffine, this.IDUltimoTaskInfermieristicoGenerato, FlagGeneraRtf);
                            }
                        }

                        anteprimartfinizio = string.Empty;
                        anteprimartffine = string.Empty;
                        rtf = new RtfFiles();

                    }
                }
                else
                    bReturn = false;
            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovPrescrizione.CreaTaskInfermieristici()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;
        }

        public bool CancellaTaskInfermieristici()
        {
            bool bReturn = true;
            MovTaskInfermieristico movti = null;

            try
            {
                Parametri op = new Parametri(this.Ambiente);
                op.Parametro.Add("IDGruppo", _idprescrizionetempi);

                op.TimeStamp.CodAzione = Enums.EnumAzioni.CAN.ToString();
                op.TimeStamp.CodEntita = Enums.EnumEntita.WKI.ToString();

                op.MovScheda = new MovScheda(EnumEntita.PRF.ToString(), _idprescrizione, this.Ambiente);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovTaskInfermieristiciGruppo", spcoll);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        movti = new MovTaskInfermieristico(row["ID"].ToString(), EnumAzioni.CAN, this.Ambiente);
                        movti.Cancella(false);
                    }

                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovPrescrizione.CancellaTaskInfermieristici()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;


        }

        public bool SospendiTaskInfermieristici()
        {
            bool bReturn = true;
            MovTaskInfermieristico movti = null;

            try
            {
                Parametri op = new Parametri(this.Ambiente);
                op.Parametro.Add("IDGruppo", _idprescrizionetempi);

                op.TimeStamp.CodAzione = Enums.EnumAzioni.ANN.ToString();
                op.TimeStamp.CodEntita = Enums.EnumEntita.WKI.ToString();

                op.MovScheda = new MovScheda(EnumEntita.PRF.ToString(), _idprescrizione, this.Ambiente);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovTaskInfermieristiciGruppo", spcoll);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        movti = new MovTaskInfermieristico(row["ID"].ToString(), EnumAzioni.ANN, this.Ambiente);
                        movti.Annulla();
                    }

                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovPrescrizione.SospendiTaskInfermieristici()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;


        }

        private void resetValori()
        {
            _idprescrizionetempi = string.Empty;
            _idprescrizione = string.Empty;
            _codstatoprescrizionetempi = string.Empty;
            _descrstatoprescrizionetempi = string.Empty;
            _dataevento = DateTime.MinValue;
            _dataorainizio = DateTime.MinValue;
            _dataorafine = DateTime.MinValue;
            _datavalidazione = DateTime.MinValue;

            _albisogno = false;
            _durata = 0;
            _continuita = false;

            _periodicitagiorni = 0;
            _periodicitaore = 0;
            _periodicitaminuti = 0;

            _codutenterilevazione = string.Empty;
            _descrutenterilevazione = string.Empty;
            _codutenteultimamodifica = string.Empty;
            _descrutenteultimamodifica = string.Empty;
            _dataultimamodifica = DateTime.MinValue;

            _posologia = string.Empty;
            _codprotocollo = string.Empty;

            _permessoDaValidare = 0;
            _permessoModifica = 0;
            _permessoAnnulla = 0;
            _permessoCancella = 0;
            _permessoCopia = 0;

            _manuale = false;
            _tempimanuali = string.Empty;

        }

        private void Carica(string idprescrizionetempi, string idprescrizione)
        {
            try
            {

                Parametri op = new Parametri(this.Ambiente);
                op.Parametro.Add("IDPrescrizioneTempi", idprescrizionetempi);
                op.Parametro.Add("IDPrescrizione", idprescrizione);

                op.TimeStamp.CodEntita = Enums.EnumEntita.WKI.ToString();
                op.TimeStamp.CodAzione = Enums.EnumAzioni.CAN.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovPrescrizioniTempiClasse", spcoll);

                if (dt.Rows.Count > 0)
                {
                    resetValori();

                    _idprescrizionetempi = dt.Rows[0]["ID"].ToString();
                    _idprescrizione = dt.Rows[0]["IDPrescrizione"].ToString();


                    if (!dt.Rows[0].IsNull("CodStatoPrescrizioneTempi")) _codstatoprescrizionetempi = dt.Rows[0]["CodStatoPrescrizioneTempi"].ToString();
                    if (!dt.Rows[0].IsNull("DescrStatoPrescrizione")) _descrstatoprescrizionetempi = dt.Rows[0]["DescrStatoPrescrizione"].ToString();
                    if (!dt.Rows[0].IsNull("CodTipoPrescrizioneTempi")) _codtipoprescrizionetempi = dt.Rows[0]["CodTipoPrescrizioneTempi"].ToString();
                    if (!dt.Rows[0].IsNull("DataOraInizio")) _dataorainizio = (DateTime)dt.Rows[0]["DataOraInizio"];
                    if (!dt.Rows[0].IsNull("DataOraFine")) _dataorafine = (DateTime)dt.Rows[0]["DataOraFine"];
                    if (!dt.Rows[0].IsNull("DataValidazione")) _datavalidazione = (DateTime)dt.Rows[0]["DataValidazione"];

                    if (!dt.Rows[0].IsNull("AlBisogno")) _albisogno = (bool)dt.Rows[0]["AlBisogno"];
                    if (!dt.Rows[0].IsNull("Durata")) _durata = (int)dt.Rows[0]["Durata"];
                    if (!dt.Rows[0].IsNull("Continuita")) _continuita = (bool)dt.Rows[0]["Continuita"];

                    if (!dt.Rows[0].IsNull("PeriodicitaGiorni")) _periodicitagiorni = (int)dt.Rows[0]["PeriodicitaGiorni"];
                    if (!dt.Rows[0].IsNull("PeriodicitaOre")) _periodicitaore = (int)dt.Rows[0]["PeriodicitaOre"];
                    if (!dt.Rows[0].IsNull("PeriodicitaMinuti")) _periodicitaminuti = (int)dt.Rows[0]["PeriodicitaMinuti"];

                    if (!dt.Rows[0].IsNull("CodUtente")) _codutenterilevazione = dt.Rows[0]["CodUtente"].ToString();
                    if (!dt.Rows[0].IsNull("DescrUtente")) _descrutenterilevazione = dt.Rows[0]["DescrUtente"].ToString();
                    if (!dt.Rows[0].IsNull("DataValidazione")) _dataevento = (DateTime)dt.Rows[0]["DataEvento"];

                    if (!dt.Rows[0].IsNull("Posologia")) _posologia = dt.Rows[0]["Posologia"].ToString();
                    if (!dt.Rows[0].IsNull("CodProtocollo")) _codprotocollo = dt.Rows[0]["CodProtocollo"].ToString();

                    if (!dt.Rows[0].IsNull("PermessoDaValidare")) _permessoDaValidare = (int)dt.Rows[0]["PermessoDaValidare"];
                    if (!dt.Rows[0].IsNull("PermessoModifica")) _permessoModifica = (int)dt.Rows[0]["PermessoModifica"];
                    if (!dt.Rows[0].IsNull("PermessoAnnulla")) _permessoAnnulla = (int)dt.Rows[0]["PermessoAnnulla"];
                    if (!dt.Rows[0].IsNull("PermessoCancella")) _permessoCancella = (int)dt.Rows[0]["PermessoCancella"];
                    if (!dt.Rows[0].IsNull("PermessoCopia")) _permessoCopia = (int)dt.Rows[0]["PermessoCopia"];

                    if (!dt.Rows[0].IsNull("Manuale")) _manuale = (bool)dt.Rows[0]["Manuale"];
                    if (!dt.Rows[0].IsNull("TempiManuali")) _tempimanuali = dt.Rows[0]["TempiManuali"].ToString();

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private void CaricaTestata(string idprescrizione)
        {

            try
            {

                Parametri op = new Parametri(this.Ambiente);
                op.Parametro.Add("IDPrescrizione", idprescrizione);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelCodSchedaPosologia", spcoll);

                if (dt.Rows.Count == 1)
                {
                    _codua = dt.Rows[0]["CodUA"].ToString();
                    _idpaziente = dt.Rows[0]["IDPaziente"].ToString();
                    _idepisodio = dt.Rows[0]["IDEpisodio"].ToString();
                    _idtrasferimento = dt.Rows[0]["IDTrasferimento"].ToString();
                }

            }
            catch (Exception)
            {

            }

        }

        private bool GeneraSingoloTask(string CodUA, string IDPaziente, string IDEpisodio,
                                  string IDTrasferimento, EnumCodSistema sistema,
                                  string codtipotaskinfermieristico, string CodScheda,
                                  DateTime DataTask, EnumTipoRegistrazione tiporegistrazione,
                                  string AnteprimaTXT, string IDTaskIniziale)
        {
            return this.GeneraSingoloTask(CodUA, IDPaziente, IDEpisodio,
                                     IDTrasferimento, sistema,
                                    codtipotaskinfermieristico, CodScheda,
                                     DataTask, tiporegistrazione,
                                     AnteprimaTXT, IDTaskIniziale, true);
        }

        private bool GeneraSingoloTask(string CodUA, string IDPaziente, string IDEpisodio,
                                       string IDTrasferimento, EnumCodSistema sistema,
                                       string codtipotaskinfermieristico, string CodScheda,
                                       DateTime DataTask, EnumTipoRegistrazione tiporegistrazione,
                                       string AnteprimaTXT, string IDTaskIniziale, bool FlagGeneraRtf)
        {
            bool bReturn = true;
            try
            {
                MovTaskInfermieristico movti = new MovTaskInfermieristico(CodUA, IDPaziente, IDEpisodio,
                                                                                   IDTrasferimento, sistema,
                                                                                   tiporegistrazione, this.Ambiente);

                movti.IDSistema = IDPrescrizione;
                movti.IDGruppo = this.IDPrescrizioneTempi;
                movti.DataProgrammata = DataTask;
                movti.CodTipoTaskInfermieristico = codtipotaskinfermieristico;
                movti.CodScheda = CodScheda;
                movti.MovScheda = new MovScheda(CodScheda, EnumEntita.WKI, CodUA, IDPaziente, IDEpisodio, IDTrasferimento, this.Ambiente);
                movti.IDTaskIniziale = IDTaskIniziale;

                Gestore oGestore = CommonStatics.GetGestore(this.Ambiente);
                oGestore.SchedaXML = movti.MovScheda.Scheda.StrutturaXML;
                oGestore.SchedaDatiXML = movti.MovScheda.DatiXML;
                oGestore.SchedaLayoutsXML = movti.MovScheda.Scheda.LayoutXML;

                oGestore.NuovaScheda();
                foreach (DcSezione oDcSezione in oGestore.Scheda.Sezioni.Values)
                {

                    foreach (DcVoce oDcVoce in oDcSezione.Voci.Values)
                    {

                        oGestore.ModificaValore(oDcVoce.Key, 1, AnteprimaTXT);

                    }

                }

                movti.MovScheda.FlagCreaRtfOnSet = false;
                movti.MovScheda.DatiXML = oGestore.SchedaDatiXML;
                movti.MovScheda.FlagCreaRtfOnSet = true;

                bReturn = movti.Salva(false, FlagGeneraRtf);

                this._idultimotaskinfermieristicogenerato = movti.IDMovTaskInfermieristico;

            }
            catch (Exception ex)
            {
                bReturn = false;
                this._idultimotaskinfermieristicogenerato = string.Empty;
                throw new Exception(@"MovPrescrizione.GeneraSingoloTask()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;
        }

    }
}
