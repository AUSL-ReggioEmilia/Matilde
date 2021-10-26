using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci
{
    [Serializable()]
    public class Trasferimento
    {

        private string _id = String.Empty;
        private string _codua = String.Empty;
        private string _coduo = String.Empty;
        private string _descrizione = String.Empty;
        private string _descrizioneuo = String.Empty;
        private DateTime _dataingresso;
        private DateTime _datauscita;
        private string _coduapadre = String.Empty;
        private string _descrizioneuapadre = String.Empty;
        private bool _attivo = false;

        DataContracts.ScciAmbiente _ambiente = new UnicodeSrl.Scci.DataContracts.ScciAmbiente();

        public Trasferimento(string codtrasferimento, DataContracts.ScciAmbiente ambiente)
        {
            this._ambiente = ambiente;
            this.Carica(codtrasferimento);
        }

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string CodUA
        {
            get { return _codua; }
            set { _codua = value; }
        }

        public string CodUO
        {
            get { return _coduo; }
            set { _coduo = value; }
        }

        public string Descrizione
        {
            get { return _descrizione; }
            set { _descrizione = value; }
        }

        public string DescrizioneUO
        {
            get { return _descrizioneuo; }
            set { _descrizioneuo = value; }
        }

        public DateTime DataIngresso
        {
            get { return _dataingresso; }
            set { _dataingresso = value; }
        }

        public DateTime DataUscita
        {
            get { return _datauscita; }
            set { _datauscita = value; }
        }

        public string CodUAPadre
        {
            get { return _coduapadre; }
            set { _coduapadre = value; }
        }
        public string DescrizioneUAPadre
        {
            get { return _descrizioneuapadre; }
            set { _descrizioneuapadre = value; }
        }

        public bool Attivo
        {
            get { return _attivo; }
            set { _attivo = value; }
        }

        public string CodStatoTrasferimento { get; set; }

        public string CodStatoCartella { get; set; }

        public string NumeroCartella { get; set; }

        public string IDCartella { get; set; }

        public string IDEpisodio { get; set; }

        public string CodStanza;
        public string DescrStanza;
        public string CodLetto;
        public string DescrLetto;
        public string DescrLettoStanza;
        public string CodAziTrasferimento;

        private void Carica(string idtrasferimento)
        {

            try
            {

                Parametri op = new Parametri(this._ambiente);
                op.Parametro.Add("IDTrasferimento", idtrasferimento);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelTrasferimento", spcoll);

                if (dt.Rows.Count == 1)
                {
                    _id = dt.Rows[0]["IDTrasferimento"].ToString();

                    _codua = dt.Rows[0]["CodUA"].ToString();
                    _coduo = dt.Rows[0]["CodUO"].ToString();
                    _descrizione = dt.Rows[0]["Descrizione"].ToString();
                    _descrizioneuo = dt.Rows[0]["DescrizioneUO"].ToString();
                    CodStatoCartella = dt.Rows[0]["CodStatoCartella"].ToString();
                    CodStatoTrasferimento = dt.Rows[0]["CodStatoTrasferimento"].ToString();
                    NumeroCartella = dt.Rows[0]["NumeroCartella"].ToString();
                    IDCartella = dt.Rows[0]["IDCartella"].ToString();
                    IDEpisodio = dt.Rows[0]["IDEpisodio"].ToString();

                    this.CodStanza = dt.Rows[0]["CodStanza"].ToString();
                    this.DescrStanza = dt.Rows[0]["DescrStanza"].ToString();
                    this.CodLetto = dt.Rows[0]["CodLetto"].ToString();
                    this.DescrLetto = dt.Rows[0]["DescrLetto"].ToString();
                    this.DescrLettoStanza = dt.Rows[0]["DescrLettoStanza"].ToString();
                    this.CodAziTrasferimento = dt.Rows[0]["CodAziTrasferimento"].ToString();


                    try
                    {
                        String dataingresso = Convert.ToString(dt.Rows[0]["DataIngresso"]);
                        if (String.IsNullOrEmpty(dataingresso) == false)
                            _dataingresso = DateTime.Parse(dataingresso);
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        String dataUscita = Convert.ToString(dt.Rows[0]["DataUscita"]);
                        if (String.IsNullOrEmpty(dataUscita) == false)
                            _datauscita = DateTime.Parse(dataUscita);

                    }
                    catch (Exception)
                    {
                    }

                    CodUAPadre = dt.Rows[0]["CodUAPadre"].ToString();
                    DescrizioneUAPadre = dt.Rows[0]["DescrizioneUAPadre"].ToString();

                    _attivo = true;

                }
                else
                {
                    _attivo = false;
                }

            }
            catch (Exception ex)
            {
                _attivo = false;
                throw new Exception(ex.Message, ex);
            }

        }

    }
}
