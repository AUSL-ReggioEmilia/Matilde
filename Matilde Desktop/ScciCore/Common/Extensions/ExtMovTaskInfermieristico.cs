using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.PluginClient;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.ScciCore.Common.Extensions
{
    public static class ExtMovTaskInfermieristico
    {

        #region     Erogazione

        public static bool Eroga(this MovTaskInfermieristico task, string codUA, ScciAmbiente ambiente, bool ripianificazione)
        {

            bool erogato = false;

            try
            {

                Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.WKI_EROGA_PRIMA.ToString(),
                                        new object[1] { new object() },
                                        CommonStatics.UAPadri(codUA, ambiente));


                if (oRispostaElaboraPrima.Successo == true)
                {
                    DialogResult dr = CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.ErogazioneTaskInfermieristici, false);

                    erogato = (dr == DialogResult.OK);

                    if (erogato)
                    {
                        PluginClientStatics.PluginClient(EnumPluginClient.WKI_EROGA_DOPO.ToString(),
                                new object[1] { new object() },
                                CommonStatics.UAPadri(codUA, ambiente));

                        if (ripianificazione)
                        {
                            bool ripToDo = task.ControlloRipianificazione(ambiente);

                            if (ripToDo)
                                task.Ripianifica(ambiente);

                            return true;
                        }

                    }

                }
                else if (oRispostaElaboraPrima.Successo == false)
                {
                    PluginClientStatics.PluginClient(EnumPluginClient.WKI_EROGA_ALTRIMENTI.ToString(),
    new object[1] { new object() }, CommonStatics.UAPadri(codUA, ambiente));
                }


            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                throw ex;
            }

            return false;

        }

        private static bool Ripianifica(this MovTaskInfermieristico task, ScciAmbiente ambiente)
        {

            CoreStatics.SetNavigazione(false);

            try
            {
                if (easyStatics.EasyMessageBox("Si desidera ripianificare il task appena erogato ?", "Ripianificazione", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return true;
                }


                MovTaskInfermieristico newTask = new MovTaskInfermieristico(task.CodUA,
                                                        task.IDPaziente,
                                                        task.IDEpisodio,
                                                        task.IDTrasferimento,
                                                        EnumCodSistema.WKI, EnumTipoRegistrazione.M,
                                                        ambiente);


                newTask.CodTipoTaskInfermieristico = task.CodTipoTaskInfermieristico;
                newTask.DescrTipoTaskInfermieristico = task.DescrTipoTaskInfermieristico;
                newTask.CodScheda = task.CodScheda;
                newTask.List_CodTipoTaskInfermieristico = null;

                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = newTask;

                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Azione = EnumAzioni.INS;
                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataEvento = DateTime.Now;

                DialogResult dr = CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingTaskInfermieristici, false);

                return (dr == DialogResult.OK);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                throw ex;
            }
            finally
            {
                CoreStatics.SetNavigazione(true);
            }

        }

        public static bool ErogaRapida(this MovTaskInfermieristico task, string codUA, ScciAmbiente ambiente, bool ripianificazione)
        {
            try
            {
                bool bAnticipo = DBUtils.ControllaAnticipoErogazione(DateTime.Now);
                if (bAnticipo == false) return false;

                MovPrescrizione movPresc = null;
                bool bPosologia = false;

                if (task.CodTipoTaskInfermieristico == Database.GetConfigTable(EnumConfigTable.TipoSchedaTaskDaPrescrizione))
                {
                    movPresc = new MovPrescrizione(CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.IDSistema, false, CoreStatics.CoreApplication.Ambiente);
                    if (movPresc != null) bPosologia = movPresc.PrescrizioneASchema;
                }

                if (bPosologia == true)
                {
                    easyStatics.EasyMessageBox("Posologia effettiva OBBLIGATORIA !!!" + Environment.NewLine +
                            "Usare erogazione standard.", "Worklist", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }

                bool bAlertAccettato = DBUtils.ControllaAlertErogazione(task, false);

                if (bAlertAccettato == false)
                    return false;


                Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.WKI_EROGA_PRIMA.ToString(),
                                        new object[1] { new object() },
                                        CommonStatics.UAPadri(codUA, ambiente));

                if (oRispostaElaboraPrima.Successo == false)
                    return false;


                Gestore oGestore = CoreStatics.GetGestore();
                task.CaricaGestore(ref oGestore);
                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataErogazione = DateTime.Now;
                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodStatoTaskInfermieristico = Enum.GetName(typeof(EnumStatoTaskInfermieristico), EnumStatoTaskInfermieristico.ER);
                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.DatiXML = oGestore.SchedaDatiXML;

                oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.WKI_EROGA_PRIMA_PU.ToString(),
                                    new object[1] { new object() },
                                    CommonStatics.UAPadri(codUA, ambiente));


                if (oRispostaElaboraPrima.Successo == true)
                {
                    if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Salva() == true)
                    {

                        PluginClientStatics.PluginClient(EnumPluginClient.WKI_EROGA_DOPO.ToString(),
                                new object[1] { new object() },
                                CommonStatics.UAPadri(codUA, ambiente));

                        if (ripianificazione)
                        {
                            bool ripToDo = task.ControlloRipianificazione(ambiente);

                            if (ripToDo)
                                task.Ripianifica(ambiente);

                            return true;
                        }

                        return true;

                    }
                }


            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                throw ex;
            }


            return false;

        }

        private static void CaricaGestore(this MovTaskInfermieristico task, ref Gestore oGestore)
        {
            try
            {
                oGestore.SchedaXML = task.MovScheda.Scheda.StrutturaXML;
                oGestore.SchedaLayoutsXML = task.MovScheda.Scheda.LayoutXML;
                oGestore.Decodifiche = task.MovScheda.Scheda.DizionarioValori();

                if (task.MovScheda.DatiXML == string.Empty)
                {
                    oGestore.SchedaDati = new DcSchedaDati();
                }
                else
                {
                    try
                    {
                        oGestore.SchedaDatiXML = task.MovScheda.DatiXML;
                    }
                    catch (Exception)
                    {
                        oGestore.SchedaDati = new DcSchedaDati();
                    }
                }
                if (oGestore.SchedaDati.Dati.Count == 0) { oGestore.NuovaScheda(); }

            }
            catch (Exception ex)
            {
                throw new Exception(@"CaricaGestore()" + Environment.NewLine + ex.Message, ex);
            }

        }

        public static bool ControlloRipianificazione(this MovTaskInfermieristico task, ScciAmbiente ambiente)
        {
            bool result = false;

            XmlParameter xp = new XmlParameter();
            xp.AddParameter("CodTipoTaskInfermieristico", task.CodTipoTaskInfermieristico);
            xp.AddParameter("IDEpisodio", task.IDEpisodio);

            string tsString = ExtMovTaskInfermieristico.GetTimeStampXml(ambiente);
            xp.AddParameter("TimeStamp", tsString);

            using (FwDataConnection fdc = new FwDataConnection(Database.ConnectionString))
            {
                DataTable table = fdc.Query<DataTable>("MSP_ControlloRipianificazioneTask", xp.ToFwDataParametersList(), CommandType.StoredProcedure);

                if (table == null)
                    throw new Exception("MSP_ControlloRipianificazioneTask risultato NULL");

                if (table.Rows.Count > 0)
                    result = Convert.ToBoolean(table.Rows[0]["Esito"]);

                fdc.Close();
            }

            return result;
        }

        #endregion  Erogazione

        #region ErogazioneDiretta

        public static List<string> GeneraErogazioneDiretta(this MovTaskInfermieristico task)
        {

            List<string> lstret = new List<string>();

            try
            {

                task.DataProgrammata = DateTime.Now;
                task.PeriodicitaDataFine = task.DataProgrammata;

                if (task.List_CodTipoTaskInfermieristico == null)
                {
                    task.List_CodTipoTaskInfermieristico = new List<string>();
                    task.List_CodTipoTaskInfermieristico.Add(CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoTaskInfermieristico);
                }

                foreach (string tipo in task.List_CodTipoTaskInfermieristico)
                {

                    string idtask = GeneraTaskErogazioneDiretta(task, tipo);
                    if (idtask != string.Empty)
                    {

                        string esito = ErogaTaskErogazioneDiretta(task, idtask);
                        if (esito != string.Empty) { lstret.Add(esito); }

                    }

                }

            }
            catch (Exception)
            {

            }

            return lstret;

        }

        private static string GeneraTaskErogazioneDiretta(this MovTaskInfermieristico task, string codtipo)
        {

            string sID = string.Empty;

            try
            {

                MovTaskInfermieristico oMov = new MovTaskInfermieristico(task.CodUA,
                            task.IDPaziente,
                            task.IDEpisodio,
                            task.IDTrasferimento,
                            EnumCodSistema.WKI, EnumTipoRegistrazione.M,
                            CoreStatics.CoreApplication.Ambiente);

                oMov.Azione = EnumAzioni.INS;
                oMov.DataEvento = DateTime.Now;

                oMov.CodTipoTaskInfermieristico = codtipo;
                oMov.DescrTipoTaskInfermieristico = GetDescrizioneTipoTask(codtipo, task.CodUA);
                oMov.CodScheda = GetCodSchedaTipoTask(codtipo, task.CodUA);

                Gestore oGestore = CoreStatics.GetGestore();

                oGestore.SchedaXML = oMov.MovScheda.Scheda.StrutturaXML;
                oGestore.SchedaLayoutsXML = oMov.MovScheda.Scheda.LayoutXML;
                oGestore.Decodifiche = oMov.MovScheda.Scheda.DizionarioValori();
                oGestore.SchedaDati = new DcSchedaDati();
                oGestore.NuovaScheda();
                oMov.MovScheda.DatiXML = oGestore.SchedaDatiXML;

                oMov.DataProgrammata = task.DataProgrammata;
                oMov.PeriodicitaDataFine = task.PeriodicitaDataFine;

                if (oMov.Salva())
                {
                    sID = oMov.IDMovTaskInfermieristico;
                }

            }
            catch (Exception)
            {

            }

            return sID;

        }

        private static string ErogaTaskErogazioneDiretta(this MovTaskInfermieristico task, string idtask)
        {

            string sret = string.Empty;

            try
            {

                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato = new MovTaskInfermieristico(idtask, CoreStatics.CoreApplication.Ambiente);

                Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.WKI_EROGA_PRIMA.ToString(),
                                                                    new object[1] { new object() },
                                                                    CommonStatics.UAPadri(task.CodUA, CoreStatics.CoreApplication.Ambiente));
                if (oRispostaElaboraPrima.Successo)
                {

                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataErogazione = DateTime.Now;
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodStatoTaskInfermieristico = Enum.GetName(typeof(EnumStatoTaskInfermieristico), EnumStatoTaskInfermieristico.ER);

                    oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.WKI_EROGA_PRIMA_PU.ToString(),
                                                            new object[1] { new object() },
                                                            CommonStatics.UAPadri(task.CodUA, CoreStatics.CoreApplication.Ambiente));
                    if (oRispostaElaboraPrima.Successo == true)
                    {

                        if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Salva() == true)
                        {

                            PluginClientStatics.PluginClient(EnumPluginClient.WKI_EROGA_DOPO.ToString(),
                                    new object[1] { new object() },
                                    CommonStatics.UAPadri(task.CodUA, CoreStatics.CoreApplication.Ambiente));
                            if (oRispostaElaboraPrima.Successo == false)
                            {
                                sret = oRispostaElaboraPrima.Parameters[0].ToString();
                            }

                        }

                    }
                    else
                    {
                        sret = oRispostaElaboraPrima.Parameters[0].ToString();
                    }

                }
                else
                {
                    sret = oRispostaElaboraPrima.Parameters[0].ToString();
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return sret;

        }

        private static string GetDescrizioneTipoTask(string tipo, string codUA)
        {

            string sret = string.Empty;

            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
            op.Parametro.Add("CodUA", codUA);
            op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
            op.Parametro.Add("CodTipoTaskInfermieristico", tipo);
            op.Parametro.Add("SoloFiltroTipoTaskInfermieristico", "1");
            SqlParameterExt[] spcoll = new SqlParameterExt[1];
            string xmlParam = XmlProcs.XmlSerializeToString(op);
            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
            DataTable oDt = Database.GetDataTableStoredProc("MSP_SelTipoTaskInfermieristico", spcoll);
            if (oDt != null && oDt.Rows.Count == 1 && !oDt.Rows[0].IsNull("Descrizione"))
            {
                sret = oDt.Rows[0]["Descrizione"].ToString();
            }
            return sret;

        }

        private static string GetCodSchedaTipoTask(string tipo, string codUA)
        {

            string sret = string.Empty;

            Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
            op.Parametro.Add("CodUA", codUA);
            op.Parametro.Add("CodRuolo", CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice);
            op.Parametro.Add("CodTipoTaskInfermieristico", tipo);
            op.Parametro.Add("SoloFiltroTipoTaskInfermieristico", "1");
            SqlParameterExt[] spcoll = new SqlParameterExt[1];
            string xmlParam = XmlProcs.XmlSerializeToString(op);
            spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
            DataTable oDt = Database.GetDataTableStoredProc("MSP_SelTipoTaskInfermieristico", spcoll);
            if (oDt != null && oDt.Rows.Count == 1 && !oDt.Rows[0].IsNull("CodScheda"))
            {
                sret = oDt.Rows[0]["CodScheda"].ToString();
            }
            return sret;

        }

        #endregion

        #region         Private Util

        private static String GetTimeStampXml(ScciAmbiente ambiente)
        {
            TimeStamp ts = new TimeStamp(ambiente);
            ts.CodEntita = EnumEntita.WKI.ToString();
            ts.CodAzione = EnumAzioni.VIS.ToString();

            string xmlString = ts.ToXmlString();


            XDocument doc = XDocument.Parse(xmlString);
            XElement root = doc.Root;

            foreach (XAttribute attr in root.Attributes())
            {
                attr.Remove();
            }

            return doc.ToString();
        }

        #endregion

    }
}
