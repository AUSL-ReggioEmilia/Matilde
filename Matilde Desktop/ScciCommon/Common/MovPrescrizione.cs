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

namespace UnicodeSrl.Scci
{

    [Serializable()]
    public class MovPrescrizione
    {
        private string _idprescrizione = string.Empty;
        private string _idpaziente = string.Empty;

        private int _tempidavalidare = 0;

        private string _idscheda = string.Empty;
        private string _codscheda = string.Empty;
        private int _versionescheda = 0;

        private string _anteprimartf = string.Empty;

        private MovScheda _movScheda = null;

        private int _numero = 0;
        private List<MovPrescrizioneTempi> _elementi = new List<MovPrescrizioneTempi>();

        public MovPrescrizione(string idprescrizione, DataContracts.ScciAmbiente ambiente)
        {
            this.resetValori();
            this.Ambiente = ambiente;
            this.Azione = EnumAzioni.MOD;
            _idprescrizione = idprescrizione;
            this.Carica(idprescrizione, true);
        }
        public MovPrescrizione(string idprescrizione, bool notempi, DataContracts.ScciAmbiente ambiente)
        {
            this.resetValori();
            this.Ambiente = ambiente;
            this.Azione = EnumAzioni.MOD;
            _idprescrizione = idprescrizione;
            this.Carica(idprescrizione, notempi);
        }
        public MovPrescrizione(string idprescrizione, EnumAzioni azione, DataContracts.ScciAmbiente ambiente)
        {
            this.resetValori();
            this.Ambiente = ambiente;
            this.Azione = azione;
            _idprescrizione = idprescrizione;
            this.Carica(idprescrizione, true);
        }
        public MovPrescrizione(string codua, string idpaziente, string idepisodio, string idtrasferimento, DataContracts.ScciAmbiente ambiente)
        {
            this.resetValori();
            this.Ambiente = ambiente;
            this.Azione = EnumAzioni.INS;
            this.CodUA = codua;
            _idpaziente = idpaziente;
            this.IDEpisodio = idepisodio;
            this.IDTrasferimento = idtrasferimento;
            _idprescrizione = "";
            _codscheda = "";
            this.DataEvento = DateTime.Now;
            this.DescrTipoPrescrizione = @"Selezionare Tipo Prescrizione";
        }

        public EnumAzioni Azione { get; set; }
        public string CodUA { get; set; }
        public string IDEpisodio { get; set; }
        public string IDTrasferimento { get; set; }
        public DateTime DataEvento { get; set; }
        public string CodViaSomministrazione { get; set; }
        public string DescrViaSomministrazione { get; set; }
        public string CodTipoPrescrizione { get; set; }
        public string DescrTipoPrescrizione { get; set; }
        public string CodStatoPrescrizione { get; set; }
        public string DescrStatoPrescrizione { get; set; }
        public string CodStatoContinuazione { get; set; }
        public DateTime DataValidazione { get; set; }
        public string CodUtenteRilevazione { get; set; }
        public string DescrUtenteRilevazione { get; set; }

        public int TempiDaValidare { get; set; }
        public int PermessoModifica { get; set; }
        public int PermessoModificaScheda { get; set; }
        public int PermessoTaskInfermieristici { get; set; }
        public int PermessoCancella { get; set; }
        public int PermessoCopia { get; set; }

        public bool SoloTestata { get; set; }

        public bool PrescrizioneASchema { get; set; }

        public ScciAmbiente Ambiente { get; set; }

        public string IDPrescrizione
        {
            get { return _idprescrizione; }
            set
            {
                if (_idprescrizione != value) _movScheda = null;
                _idprescrizione = value;
            }
        }

        public string IDPaziente
        {
            get { return _idpaziente; }
            set { _idpaziente = value; }
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
                    if (_idprescrizione != null && _idprescrizione != string.Empty && _idprescrizione.Trim() != "")
                    {
                        _movScheda = new MovScheda(EnumEntita.PRF.ToString(), _idprescrizione, this.Ambiente);
                    }
                    else if (_codscheda != null && _codscheda != string.Empty && _codscheda.Trim() != "")
                    {
                        _movScheda = new MovScheda(_codscheda, EnumEntita.PRF, this.CodUA, _idpaziente, this.IDEpisodio, this.IDTrasferimento, this.Ambiente);
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

        public int Numero
        {
            get { return _numero; }
        }

        public List<MovPrescrizioneTempi> Elementi
        {
            get { return _elementi; }
            set { _elementi = value; }
        }

        public bool Cancella()
        {
            return this.Cancella(true);
        }
        public bool Cancella(bool FlagGeneraRTF)
        {
            try
            {
                Azione = EnumAzioni.CAN;

                foreach (MovPrescrizioneTempi movprt in this.Elementi)
                    movprt.CodStatoPrescrizioneTempi = EnumStatoPrescrizioneTempi.CA.ToString();

                if (Salva())
                {
                    Azione = EnumAzioni.MOD;
                    Carica(_idprescrizione, true);
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
                if (_idprescrizione != string.Empty && _idprescrizione.Trim() != "")
                {
                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDPrescrizione", _idprescrizione);
                    op.Parametro.Add("IDEpisodio", this.IDEpisodio);
                    op.Parametro.Add("IDTrasferimento", this.IDTrasferimento);
                    op.Parametro.Add("DataEvento", Database.dataOra105PerParametri(this.DataEvento));
                    op.Parametro.Add("DataEventoUTC", Database.dataOra105PerParametri(this.DataEvento.ToUniversalTime()));
                    op.Parametro.Add("CodViaSomministrazione", this.CodViaSomministrazione);
                    op.Parametro.Add("CodTipoPrescrizione", this.CodTipoPrescrizione);
                    op.Parametro.Add("CodStatoPrescrizione", this.CodStatoPrescrizione);
                    op.Parametro.Add("CodStatoContinuazione", this.CodStatoContinuazione);

                    op.TimeStamp.CodEntita = EnumEntita.PRF.ToString();
                    op.TimeStamp.IDEntita = _idprescrizione;
                    op.TimeStamp.CodAzione = this.Azione.ToString();

                    op.MovScheda = this.MovScheda;

                    this.MovScheda.Azione = this.Azione;

                    bReturn = this.MovScheda.Salva(FlagGeneraRTF);
                    if (bReturn)
                    {
                        if (bReturn)
                        {
                            spcoll = new SqlParameterExt[1];

                            xmlParam = XmlProcs.XmlSerializeToString(op);
                            xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                            Database.ExecStoredProc("MSP_AggMovPrescrizioni", spcoll);
                        }
                    }
                }
                else
                {

                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDEpisodio", this.IDEpisodio);
                    op.Parametro.Add("IDTrasferimento", this.IDTrasferimento);
                    op.Parametro.Add("DataEvento", Database.dataOra105PerParametri(this.DataEvento));
                    op.Parametro.Add("DataEventoUTC", Database.dataOra105PerParametri(this.DataEvento.ToUniversalTime()));
                    op.Parametro.Add("CodViaSomministrazione", this.CodViaSomministrazione);
                    op.Parametro.Add("CodTipoPrescrizione", this.CodTipoPrescrizione);
                    op.Parametro.Add("CodStatoPrescrizione", this.CodStatoPrescrizione);
                    op.Parametro.Add("CodStatoContinuazione", this.CodStatoContinuazione);

                    op.TimeStamp.CodEntita = EnumEntita.PRF.ToString();
                    op.TimeStamp.IDEntita = _idprescrizione;
                    op.TimeStamp.CodAzione = this.Azione.ToString();

                    op.MovScheda = this.MovScheda;

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);

                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovPrescrizioni", spcoll);
                    if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                    {
                        _idprescrizione = dt.Rows[0][0].ToString();

                        this.MovScheda.IDEntita = _idprescrizione;
                        this.MovScheda.Azione = this.Azione;

                        bReturn = this.MovScheda.Salva(FlagGeneraRTF);

                        bReturn = this.SalvaTempi(_idprescrizione, this.Azione);

                        this.Azione = EnumAzioni.MOD;

                        Carica(_idprescrizione, true);
                    }
                    else
                        bReturn = false;
                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovPrescrizione.Salva()" + Environment.NewLine + ex.Message, ex);

            }

            return bReturn;

        }

        public bool CopiaDaOrigine(ref MovPrescrizione movprorigine)
        {
            bool bReturn = true;

            try
            {
                this.CodUA = movprorigine.MovScheda.CodUA;
                _idprescrizione = string.Empty;
                this.IDEpisodio = movprorigine.IDEpisodio;
                _idpaziente = movprorigine.IDPaziente;
                this.IDTrasferimento = movprorigine.IDTrasferimento;
                this.DataEvento = movprorigine.DataEvento;
                this.CodViaSomministrazione = movprorigine.CodViaSomministrazione;
                this.DescrViaSomministrazione = movprorigine.DescrViaSomministrazione;
                this.CodTipoPrescrizione = movprorigine.CodTipoPrescrizione;
                this.DescrTipoPrescrizione = movprorigine.DescrTipoPrescrizione;
                this.CodStatoPrescrizione = movprorigine.CodStatoPrescrizione;
                this.DescrStatoPrescrizione = movprorigine.DescrStatoPrescrizione;
                this.CodStatoContinuazione = movprorigine.CodStatoContinuazione;
                this.DataValidazione = DateTime.MinValue;

                _tempidavalidare = 1;
                this.PermessoModifica = movprorigine.PermessoModifica;
                this.PermessoTaskInfermieristici = movprorigine.PermessoTaskInfermieristici;
                this.PermessoCancella = movprorigine.PermessoCancella;
                this.PermessoCopia = movprorigine.PermessoCopia;

                this.PrescrizioneASchema = movprorigine.PrescrizioneASchema;

                this.CodUtenteRilevazione = this.Ambiente.Codlogin;
                this.DescrUtenteRilevazione = string.Empty;

                this.Azione = EnumAzioni.INS;

                this.MovScheda = new MovScheda(movprorigine.MovScheda.CodScheda,
                                                    (EnumEntita)Enum.Parse(typeof(EnumEntita), movprorigine.MovScheda.CodEntita),
                                                    movprorigine.MovScheda.CodUA, movprorigine.MovScheda.IDPaziente,
                                                    movprorigine.MovScheda.IDEpisodio, movprorigine.MovScheda.IDTrasferimento, this.Ambiente);
                this.MovScheda.CopiaDaOrigine(movprorigine.MovScheda, 1, true);

                _numero = movprorigine.Numero;

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovPrescrizione.CopiaDaOrigine()" + Environment.NewLine + ex.Message, ex);

            }

            return bReturn;
        }

        public void CaricaTempi(string idprescrizione)
        {
            try
            {

                _elementi.Clear();

                Parametri op = new Parametri(this.Ambiente);
                op.Parametro.Add("IDPrescrizione", idprescrizione);
                op.Parametro.Add("DatiEstesi", "0");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovPrescrizioniTempi", spcoll);

                foreach (DataRow oDr in dt.Rows)
                {
                    _elementi.Add(new MovPrescrizioneTempi(oDr["ID"].ToString(), oDr["IDPrescrizione"].ToString(), this.Ambiente));
                }

                _numero = dt.Rows.Count;

            }
            catch (Exception ex)
            {
                _numero = 0;
                _elementi = new List<MovPrescrizioneTempi>();
                throw new Exception(ex.Message, ex);
            }
        }

        private void resetValori()
        {
            this.CodUA = string.Empty;
            _idprescrizione = string.Empty;
            this.IDEpisodio = string.Empty;
            _idpaziente = string.Empty;

            this.IDTrasferimento = string.Empty;
            this.DataEvento = DateTime.MinValue;
            this.CodViaSomministrazione = string.Empty;
            this.DescrViaSomministrazione = string.Empty;
            this.CodTipoPrescrizione = string.Empty;
            this.DescrTipoPrescrizione = string.Empty;
            this.CodStatoPrescrizione = string.Empty;
            this.DescrStatoPrescrizione = string.Empty;
            this.CodStatoContinuazione = string.Empty;
            this.DataValidazione = DateTime.MinValue;

            this.CodUtenteRilevazione = string.Empty;
            this.DescrUtenteRilevazione = string.Empty;

            _tempidavalidare = 0;
            this.PermessoModifica = 0;
            this.PermessoModificaScheda = 1;
            this.PermessoCopia = 0;
            this.PermessoCancella = 0;
            this.PermessoTaskInfermieristici = 0;

            this.SoloTestata = false;

            this.PrescrizioneASchema = false;

            _idscheda = string.Empty;
            _codscheda = string.Empty;
            _versionescheda = 0;

            _anteprimartf = string.Empty;

            _movScheda = null;

            _numero = 0;
            _elementi = new List<MovPrescrizioneTempi>();
        }

        private void Carica(string idprescrizione, bool notempi)
        {
            try
            {

                Parametri op = new Parametri(this.Ambiente);
                op.Parametro.Add("IDPrescrizione", idprescrizione);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovPrescrizioni", spcoll);

                if (dt.Rows.Count > 0)
                {
                    resetValori();

                    _idprescrizione = dt.Rows[0]["ID"].ToString();
                    _idpaziente = dt.Rows[0]["IDPaziente"].ToString();
                    this.IDEpisodio = dt.Rows[0]["IDEpisodio"].ToString();
                    this.IDTrasferimento = dt.Rows[0]["IDTrasferimento"].ToString();
                    this.CodUA = dt.Rows[0]["CodUA"].ToString();

                    if (!dt.Rows[0].IsNull("DataEvento")) this.DataEvento = (DateTime)dt.Rows[0]["DataEvento"];
                    if (!dt.Rows[0].IsNull("CodViaSomministrazione")) this.CodViaSomministrazione = dt.Rows[0]["CodViaSomministrazione"].ToString();
                    if (!dt.Rows[0].IsNull("DescrViaSomministrazione")) this.DescrViaSomministrazione = dt.Rows[0]["DescrViaSomministrazione"].ToString();
                    if (!dt.Rows[0].IsNull("CodTipoPrescrizione")) this.CodTipoPrescrizione = dt.Rows[0]["CodTipoPrescrizione"].ToString();
                    if (!dt.Rows[0].IsNull("DescrTipoPrescrizione")) this.DescrTipoPrescrizione = dt.Rows[0]["DescrTipoPrescrizione"].ToString();
                    if (!dt.Rows[0].IsNull("CodStatoPrescrizione")) this.CodStatoPrescrizione = dt.Rows[0]["CodStatoPrescrizione"].ToString();
                    if (!dt.Rows[0].IsNull("DescrStatoPrescrizione")) this.DescrStatoPrescrizione = dt.Rows[0]["DescrStatoPrescrizione"].ToString();
                    if (!dt.Rows[0].IsNull("CodStatoContinuazione")) this.CodStatoContinuazione = dt.Rows[0]["CodStatoContinuazione"].ToString();
                    if (!dt.Rows[0].IsNull("DataValidazione")) this.DataValidazione = (DateTime)dt.Rows[0]["DataValidazione"];
                    if (!dt.Rows[0].IsNull("CodUtente")) this.CodUtenteRilevazione = dt.Rows[0]["CodUtente"].ToString();
                    if (!dt.Rows[0].IsNull("DescrUtente")) this.DescrUtenteRilevazione = dt.Rows[0]["DescrUtente"].ToString();
                    if (!dt.Rows[0].IsNull("TempiDaValidare")) _tempidavalidare = (int)dt.Rows[0]["TempiDaValidare"];
                    if (!dt.Rows[0].IsNull("PermessoModifica")) this.PermessoModifica = (int)dt.Rows[0]["PermessoModifica"];
                    if (!dt.Rows[0].IsNull("PermessoModificaScheda")) this.PermessoModificaScheda = (int)dt.Rows[0]["PermessoModificaScheda"];
                    if (!dt.Rows[0].IsNull("PermessoTaskInfermiristici")) this.PermessoTaskInfermieristici = (int)dt.Rows[0]["PermessoTaskInfermiristici"];
                    if (!dt.Rows[0].IsNull("PermessoCancella")) this.PermessoCancella = (int)dt.Rows[0]["PermessoCancella"];
                    if (!dt.Rows[0].IsNull("PermessoCopia")) this.PermessoCopia = (int)dt.Rows[0]["PermessoCopia"];

                    if (!dt.Rows[0].IsNull("PrescrizioneASchema")) this.PrescrizioneASchema = (bool)dt.Rows[0]["PrescrizioneASchema"];

                    if (!dt.Rows[0].IsNull("IDScheda")) _idscheda = dt.Rows[0]["IDScheda"].ToString();
                    if (!dt.Rows[0].IsNull("CodScheda")) _codscheda = dt.Rows[0]["CodScheda"].ToString();
                    if (!dt.Rows[0].IsNull("Versione")) _versionescheda = (int)dt.Rows[0]["Versione"];

                    if (!dt.Rows[0].IsNull("AnteprimaRTF")) _anteprimartf = dt.Rows[0]["AnteprimaRTF"].ToString();

                    if (notempi == true) { this.CaricaTempi(idprescrizione); }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private bool SalvaTempi(string idprescrizione, EnumAzioni azione)
        {
            bool bret = true;

            try
            {
                foreach (MovPrescrizioneTempi movprt in this.Elementi)
                {
                    movprt.Azione = azione;
                    movprt.IDPrescrizione = idprescrizione;
                    bret = movprt.Salva();
                    if (!bret) break;
                }
            }
            catch (Exception ex)
            {
                bret = false;
                throw new Exception(ex.Message, ex);
            }

            return bret;

        }

    }

}


