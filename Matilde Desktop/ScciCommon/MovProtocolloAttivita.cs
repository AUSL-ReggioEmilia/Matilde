using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.RTF;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci
{
    [Serializable()]
    public class MovProtocolloAttivita
    {
        public MovProtocolloAttivita(string codice, string idPaziente, string codUA, string idEpisodio,
                                        string idTrasferimento, EnumCodSistema codSistema,
                                        string idSistema,
                                        string idGruppo,
                                        EnumTipoRegistrazione tipoRegistrazione,
                                        ScciAmbiente ambiente
                                    )

        {
            this.Codice = codice;
            this.Ambiente = ambiente;
            this.IDPaziente = idPaziente;
            this.CodUA = codUA;
            this.IDEpisodio = idEpisodio;
            this.IDTrasferimento = idTrasferimento;

            this.DataOraInizio = DateTime.MinValue;
            this.FlagManuale = false;

            this.Tempi = new List<ProtocolloAttivitaTempo>();

            this.Periodicita = new List<IntervalloTempiAttivita>();

            this.UltimoIDMovTaskInfermieristicoGenerato = string.Empty;

            this.CodSistema = codSistema;
            this.IDSistema = idSistema;
            this.IDGruppo = idGruppo;
            this.TipoRegistrazione = tipoRegistrazione;

            this.Carica();
        }

        public ScciAmbiente Ambiente { get; set; }

        public string Codice { get; set; }
        public string Descrizione { get; set; }

        public DateTime DataOraInizio { get; set; }

        public bool FlagManuale { get; set; }

        public List<ProtocolloAttivitaTempo> Tempi { get; set; }

        public List<IntervalloTempiAttivita> Periodicita { get; set; }

        public string UltimoIDMovTaskInfermieristicoGenerato { get; set; }

        public string IDPaziente { get; set; }

        public string CodUA { get; set; }

        public string IDEpisodio { get; set; }

        public string IDTrasferimento { get; set; }

        public string IDSistema { get; set; }

        public string IDGruppo { get; set; }

        public EnumCodSistema CodSistema { get; set; }

        public EnumTipoRegistrazione TipoRegistrazione { get; set; }

        public List<IntervalloTempiAttivita> GeneraPeriodicita()
        {

            List<IntervalloTempiAttivita> listaperiodicita = new List<IntervalloTempiAttivita>();
            DateTime datapartenza = DateTime.MinValue;
            DateTime dataperiodicita = DateTime.MinValue;

            if (this.DataOraInizio != DateTime.MinValue)
            {
                foreach (ProtocolloAttivitaTempo pt in this.Tempi)
                {
                    if (pt.DeltaAlle00)
                    {
                        datapartenza = new DateTime(this.DataOraInizio.Year, this.DataOraInizio.Month, this.DataOraInizio.Day, 0, 0, 0);
                    }
                    else
                    {
                        datapartenza = this.DataOraInizio;
                    }

                    dataperiodicita = datapartenza.AddDays(pt.DeltaGiorni).AddHours(pt.DeltaOre).AddMinutes(pt.DeltaMinuti);

                    foreach (KeyValuePair<string, string> tipotask in pt.TipiTaskInfermieristici)
                    {
                        listaperiodicita.Add(new IntervalloTempiAttivita(dataperiodicita, this.Descrizione, pt.Descrizione, tipotask.Key, tipotask.Value));
                    }
                }
            }

            return listaperiodicita;

        }

        public bool CreaTaskInfermieristici()
        {
            bool bReturn = true;

            try
            {
                foreach (IntervalloTempiAttivita inttempo in this.Periodicita)
                {

                    string codScheda = this.CaricaCodScheda(inttempo.CodiceTask);

                    bReturn = this.GeneraSingoloTask(inttempo.CodiceTask,
                                                     codScheda,
                                                     inttempo.DataOraInizio);

                    if (bReturn == false)
                        return false;

                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovProtocolloAttivita.CreaTaskInfermieristici()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;
        }



        private void Carica()
        {
            Parametri op = null;
            SqlParameterExt[] spcoll = null;
            string xmlParam = string.Empty;
            DataTable dt = null;
            ProtocolloAttivitaTempo tp = null;

            op = new Parametri(this.Ambiente);
            op.Parametro.Add("CodProtocolloAttivita", this.Codice);
            op.Parametro.Add("DatiEstesi", "0");

            spcoll = new SqlParameterExt[1];

            xmlParam = XmlProcs.XmlSerializeToString(op);
            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

            dt = Database.GetDataTableStoredProc("MSP_SelProtocolliAttivita", spcoll);

            if (dt.Rows.Count > 0)
            {
                this.Descrizione = dt.Rows[0]["Descrizione"].ToString();

                op = new Parametri(this.Ambiente);
                op.Parametro.Add("CodProtocolloAttivita", this.Codice);

                spcoll = new SqlParameterExt[1];

                xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                dt = Database.GetDataTableStoredProc("MSP_SelProtocolliAttivitaTempi", spcoll);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        tp = new ProtocolloAttivitaTempo();

                        tp.Codice = dr["Codice"].ToString();
                        tp.CodiceProtocollo = this.Codice;
                        tp.Descrizione = dr["Descrizione"].ToString();

                        if (dr["DeltaGiorni"] != DBNull.Value) tp.DeltaGiorni = int.Parse(dr["DeltaGiorni"].ToString());
                        if (dr["DeltaOre"] != DBNull.Value) tp.DeltaOre = int.Parse(dr["DeltaOre"].ToString());
                        if (dr["DeltaMinuti"] != DBNull.Value) tp.DeltaMinuti = int.Parse(dr["DeltaMinuti"].ToString());

                        if (dr["DeltaAlle00"] != DBNull.Value) tp.DeltaAlle00 = bool.Parse(dr["DeltaAlle00"].ToString());

                        tp.TipiTaskInfermieristici = new Dictionary<string, string>();

                        this.Tempi.Add(tp);
                    }

                    op = new Parametri(this.Ambiente);
                    op.Parametro.Add("CodProtocolloAttivita", this.Codice);

                    spcoll = new SqlParameterExt[1];

                    xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    dt = Database.GetDataTableStoredProc("MSP_SelProtocolliAttivitaTipoTask", spcoll);
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            tp = this.Tempi.Find(a => a.Codice == dr["CodProtocolloAttivitaTempi"].ToString());
                            if (tp != null)
                            {
                                tp.TipiTaskInfermieristici.Add(dr["CodTipoTaskInfermieristico"].ToString(), dr["DescrizioneTipoTask"].ToString());
                            }
                        }
                    }
                }
            }
        }

        private bool GeneraSingoloTask(string codtipotaskinfermieristico, string CodScheda,
                               DateTime DataTask)
        {
            bool bReturn = true;
            try
            {
                MovTaskInfermieristico movti = new MovTaskInfermieristico(CodUA, this.IDPaziente, this.IDEpisodio,
                                                                                   this.IDTrasferimento, this.CodSistema,
                                                                                   this.TipoRegistrazione, this.Ambiente);

                movti.IDSistema = this.IDSistema;
                movti.IDGruppo = this.IDGruppo;
                movti.CodSistema = this.CodSistema.ToString();
                movti.DataProgrammata = DataTask;
                movti.CodTipoTaskInfermieristico = codtipotaskinfermieristico;
                movti.CodScheda = CodScheda;

                bReturn = movti.Salva(false);

                this.UltimoIDMovTaskInfermieristicoGenerato = movti.IDMovTaskInfermieristico;

            }
            catch (Exception ex)
            {
                bReturn = false;
                throw new Exception(@"MovProtocolloAttivita.GeneraSingoloTask()" + Environment.NewLine + ex.Message, ex);
            }

            return bReturn;
        }

        private string CaricaCodScheda(string CodTipoTaskInfermieristico)
        {

            string scodscheda = string.Empty;
            Parametri op = null;
            SqlParameterExt[] spcoll = null;
            string xmlParam = string.Empty;
            DataTable dt = null;

            try
            {

                op = new Parametri(this.Ambiente);
                op.Parametro.Add("CodEntita", EnumEntita.WKI.ToString());
                op.Parametro.Add("CodTipo", CodTipoTaskInfermieristico);

                spcoll = new SqlParameterExt[1];

                xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                dt = Database.GetDataTableStoredProc("MSP_SelSchedaDaTipo", spcoll);

                if (dt != null && dt.Rows.Count > 0)
                    scodscheda = dt.Rows[0]["CodScheda"].ToString();
                else
                    scodscheda = string.Empty;
            }
            catch
            {
                scodscheda = string.Empty;
            }

            return scodscheda;

        }

    }
}
