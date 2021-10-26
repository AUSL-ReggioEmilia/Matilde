using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci
{

    [Serializable()]
    public class CartellaAmbulatoriale
    {

        public CartellaAmbulatoriale(DataContracts.ScciAmbiente ambiente)
        {
            this.SvoutaCampi();
            this.Ambiente = ambiente;
        }
        public CartellaAmbulatoriale(string idcartella, string numerocartella, DataContracts.ScciAmbiente ambiente)
        {
            this.SvoutaCampi();
            this.Ambiente = ambiente;
            this.ID = idcartella;
            this.NumeroCartella = numerocartella;
            this.Carica();
        }

        public DataContracts.ScciAmbiente Ambiente { get; set; }

        public string ID { get; set; }

        public string NumeroCartella { get; set; }

        public string CodStatoCartella { get; set; }

        public string CodUtenteApertura { get; set; }

        public string UtenteApertura { get; set; }

        public string CodUtenteChiusura { get; set; }

        public string UtenteChiusura { get; set; }

        public DateTime DataApertura { get; set; }

        public DateTime DataChiusura { get; set; }

        public byte[] PDFCartella { get; set; }

        private void SvoutaCampi()
        {
            this.ID = string.Empty;
            this.NumeroCartella = string.Empty;
            this.CodStatoCartella = string.Empty;
            this.CodUtenteApertura = string.Empty;
            this.CodUtenteChiusura = string.Empty;
            this.UtenteApertura = string.Empty;
            this.UtenteChiusura = string.Empty;
            this.DataApertura = DateTime.MinValue;
            this.DataChiusura = DateTime.MinValue;
            this.PDFCartella = null;
        }

        private void Carica()
        {

            try
            {

                Parametri op = new Parametri(this.Ambiente);
                if (this.ID != string.Empty)
                {
                    op.Parametro.Add("IDCartella", this.ID);
                }
                if (this.NumeroCartella != string.Empty)
                {
                    op.Parametro.Add("NumeroCartella", this.NumeroCartella);
                }
                op.Parametro.Add("DatiEstesi", "1");

                op.TimeStamp.CodAzione = Enums.EnumAzioni.VIS.ToString();
                op.TimeStamp.CodEntita = Enums.EnumEntita.CAC.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovCartelleAmbulatoriali", spcoll);

                if (dt.Rows.Count == 1)
                {
                    if (dt.Columns.Contains("ID") && !dt.Rows[0].IsNull("ID")) this.ID = dt.Rows[0]["ID"].ToString();
                    if (dt.Columns.Contains("NumeroCartella") && !dt.Rows[0].IsNull("NumeroCartella")) this.NumeroCartella = dt.Rows[0]["NumeroCartella"].ToString();
                    if (dt.Columns.Contains("CodStatoCartella") && !dt.Rows[0].IsNull("CodStatoCartella")) this.CodStatoCartella = dt.Rows[0]["CodStatoCartella"].ToString();
                    if (dt.Columns.Contains("CodUtenteApertura") && !dt.Rows[0].IsNull("CodUtenteApertura")) this.CodUtenteApertura = dt.Rows[0]["CodUtenteApertura"].ToString();
                    if (dt.Columns.Contains("CodUtenteChiusura") && !dt.Rows[0].IsNull("CodUtenteChiusura")) this.CodUtenteChiusura = dt.Rows[0]["CodUtenteChiusura"].ToString();
                    if (dt.Columns.Contains("UtenteApertura") && !dt.Rows[0].IsNull("UtenteApertura")) this.UtenteApertura = dt.Rows[0]["UtenteApertura"].ToString();
                    if (dt.Columns.Contains("UtenteChiusura") && !dt.Rows[0].IsNull("UtenteChiusura")) this.UtenteChiusura = dt.Rows[0]["UtenteChiusura"].ToString();
                    if (dt.Columns.Contains("DataApertura") && !dt.Rows[0].IsNull("DataApertura")) this.DataApertura = (DateTime)dt.Rows[0]["DataApertura"];
                    if (dt.Columns.Contains("DataChiusura") && !dt.Rows[0].IsNull("DataChiusura")) this.DataChiusura = (DateTime)dt.Rows[0]["DataChiusura"];
                    if (dt.Columns.Contains("PDFCartella") && !dt.Rows[0].IsNull("PDFCartella")) this.PDFCartella = (byte[])dt.Rows[0]["PDFCartella"];
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public bool Salva()
        {

            bool bReturn = true;

            try
            {

                if (ID == string.Empty || ID.Trim() == "")
                {
                    Parametri op = new Parametri(this.Ambiente);
                    op.Parametro.Add("NumeroCartella", this.NumeroCartella);
                    op.Parametro.Add("CodStatoCartella", this.CodStatoCartella);

                    op.TimeStamp.CodEntita = EnumEntita.CAC.ToString();
                    op.TimeStamp.CodAzione = EnumAzioni.INS.ToString();

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_InsMovCartelleAmbulatoriali", spcoll);
                    if (dt != null && dt.Rows.Count > 0 && !dt.Rows[0].IsNull(0))
                    {
                        this.ID = dt.Rows[0][0].ToString();
                    }
                    else
                    {
                        bReturn = false;
                    }
                }
                else
                {
                    Parametri op = new Parametri(this.Ambiente);
                    op.Parametro.Add("IDCartella", this.ID);
                    op.Parametro.Add("CodStatoCartella", this.CodStatoCartella);

                    op.TimeStamp.CodEntita = EnumEntita.CAC.ToString();
                    op.TimeStamp.CodAzione = EnumAzioni.MOD.ToString();

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    xmlParam = XmlProcs.CheckXMLDati(xmlParam);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    DataTable dt = Database.GetDataTableStoredProc("MSP_AggMovCartelleAmbulatoriali", spcoll);

                    bReturn = true;
                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"CartellaAmbulatoriale.Salva()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;

        }

    }

}
