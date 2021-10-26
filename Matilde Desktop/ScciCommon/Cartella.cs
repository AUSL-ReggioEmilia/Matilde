using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci.Plugin;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Model;
using UnicodeSrl.Scci.PluginClient;

namespace UnicodeSrl.Scci
{
    [Serializable()]
    public class Cartella
    {
        DataContracts.ScciAmbiente _ambiente = new UnicodeSrl.Scci.DataContracts.ScciAmbiente();

        public Cartella(string idcartella, string numerocartella, DataContracts.ScciAmbiente ambiente)
        {
            this._ambiente = ambiente;
            this.ID = idcartella;
            this.NumeroCartella = string.Empty;
            this.PDFCartella = null;
            this.CodStatoCartella = string.Empty;
            this.CodStatoCartellaInfo = string.Empty;
            this.CodUtenteApertura = string.Empty;
            this.CodUtenteChiusura = string.Empty;
            this.UtenteApertura = string.Empty;
            this.UtenteChiusura = string.Empty;
            this.DataApertura = DateTime.MinValue;
            this.DataChiusura = DateTime.MinValue;
            this.Carica(idcartella);
        }

        public string ID { get; set; }

        public string CodUtenteApertura { get; set; }

        public string UtenteApertura { get; set; }

        public string CodUtenteChiusura { get; set; }

        public string UtenteChiusura { get; set; }

        public DateTime DataApertura { get; set; }

        public DateTime DataChiusura { get; set; }

        public string CodStatoCartella { get; set; }

        public string CodStatoCartellaInfo { get; set; }

        public string NumeroCartella { get; set; }

        public byte[] PDFCartella { get; set; }

        public bool PDFCartellaAggiornabile
        {
            get
            {
                return (this.NumeroCartella != null
                     && this.NumeroCartella != string.Empty
                     && this.NumeroCartella.Trim() != ""
                     && this.CodStatoCartella == Enums.EnumStatoCartella.AP.ToString());
            }
        }

        public bool CartellaChiusa
        {
            get
            {
                return (this.CodStatoCartella == Enums.EnumStatoCartella.CH.ToString());
            }
        }

        public Boolean WarningElaborati { get; set; }

        private void Carica(string idcartella)
        {

            try
            {

                Parametri op = new Parametri(this._ambiente);
                op.Parametro.Add("IDCartella", idcartella);
                op.Parametro.Add("DatiEstesi", "1");

                op.TimeStamp.CodAzione = Enums.EnumAzioni.VIS.ToString();
                op.TimeStamp.CodEntita = Enums.EnumEntita.CAR.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovCartelle", spcoll);

                if (dt.Rows.Count == 1)
                {
                    if (dt.Columns.Contains("ID") && !dt.Rows[0].IsNull("ID")) this.ID = dt.Rows[0]["ID"].ToString();
                    if (dt.Columns.Contains("NumeroCartella") && !dt.Rows[0].IsNull("NumeroCartella")) this.NumeroCartella = dt.Rows[0]["NumeroCartella"].ToString();
                    if (dt.Columns.Contains("CodStatoCartella") && !dt.Rows[0].IsNull("CodStatoCartella")) this.CodStatoCartella = dt.Rows[0]["CodStatoCartella"].ToString();
                    if (dt.Columns.Contains("CodStatoCartellaInfo") && !dt.Rows[0].IsNull("CodStatoCartellaInfo")) this.CodStatoCartellaInfo = dt.Rows[0]["CodStatoCartellaInfo"].ToString();
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

        public bool archiviaPDF(string pdffullpath, bool ignoraControlloCartellaAggiornabile = false)
        {
            try
            {
                bool bReturn = false;

                if (this.PDFCartellaAggiornabile || ignoraControlloCartellaAggiornabile == true)
                {
                    if (pdffullpath != null && pdffullpath != string.Empty && pdffullpath.Trim() != "" && System.IO.File.Exists(pdffullpath))
                    {
                        this.PDFCartella = UnicodeSrl.Framework.Utility.FileSystem.FileToByteArray(pdffullpath);
                        bReturn = archiviaPDF(ignoraControlloCartellaAggiornabile);
                    }
                }

                return bReturn;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool archiviaPDF(bool ignoraControlloCartellaAggiornabile = false)
        {
            try
            {
                bool bReturn = false;

                if (this.PDFCartellaAggiornabile || ignoraControlloCartellaAggiornabile == true)
                {
                    Parametri op = new Parametri(_ambiente);

                    op.Parametro.Add("IDCartella", this.ID);
                    op.Parametro.Add("NumeroCartella", this.NumeroCartella);
                    op.Parametro.Add("PDFCartella", Convert.ToBase64String(this.PDFCartella));

                    op.TimeStamp.CodAzione = Enums.EnumAzioni.MOD.ToString();
                    op.TimeStamp.CodEntita = Enums.EnumEntita.CAR.ToString();
                    op.TimeStamp.IDEntita = this.ID;

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    Database.ExecStoredProc("MSP_AggMovCartelle", spcoll);

                    bReturn = true;
                }

                return bReturn;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string generaPDF(string plugindllfullpath, bool firmaDigitale, string utenteFirma, bool sessioneRemota, bool isOSServer)
        {
            try
            {
                return generaPDF(plugindllfullpath, firmaDigitale, utenteFirma, "", sessioneRemota, isOSServer);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string generaPDF(string plugindllfullpath, bool firmaDigitale, string utenteFirma, string pdfexportfullpath, bool sessioneRemota, bool isOSServer)
        {
            try
            {
                string pdfgeneratofullpath = "";

                if (this.NumeroCartella != null
                 && this.NumeroCartella != string.Empty
                 && this.NumeroCartella.Trim() != ""
                 && plugindllfullpath != null
                 && plugindllfullpath.Trim() != "")
                {

                    Dictionary<string, object> oDictionary = new Dictionary<string, object>();
                    oDictionary.Add("StringaConnessione", Database.ConnectionString);
                    oDictionary.Add("NumeroCartella", this.NumeroCartella);
                    oDictionary.Add("IDCartella", this.ID);
                    oDictionary.Add("XmlAmbiente", _ambiente.XmlSerializeToString());

                    oDictionary.Add("ExportPDF", "1");
                    if (pdfexportfullpath != null && pdfexportfullpath != string.Empty && pdfexportfullpath.Trim() != "")
                        oDictionary.Add("ExportPDFFullPath", pdfexportfullpath);

                    if (firmaDigitale)
                        oDictionary.Add("FirmaDigitale", "1");
                    else
                        oDictionary.Add("FirmaDigitale", "0");

                    oDictionary.Add("UtenteFirma", utenteFirma);

                    T_CDSSPlugins oCDSSPlugins = new T_CDSSPlugins();
                    oCDSSPlugins.Codice = "CTLPZN1";
                    if (oCDSSPlugins.TrySelect())
                    {

                        PluginClient.Plugin oPlugin = new PluginClient.Plugin(oCDSSPlugins.Codice, oCDSSPlugins.Descrizione, oCDSSPlugins.NomePlugin, oCDSSPlugins.Comando, "", "", 0, null);
                        object[] myparam = new object[1] { oDictionary };

                        Risposta oRisposta = PluginClientStatics.PluginClientMenuEsegui(oPlugin, myparam);
                        if (oRisposta.Parameters != null)
                        {

                            if (oRisposta.Parameters.Length == 1 && oRisposta.Parameters[0].GetType() == typeof(string))
                            {

                                string spdf = oRisposta.Parameters[0] as string;
                                if (System.IO.File.Exists(spdf))
                                {
                                    pdfgeneratofullpath = spdf;
                                }

                            }

                        }

                    }
                    else
                    {

                        string nomeplugin = System.IO.Path.GetFileNameWithoutExtension(plugindllfullpath);
                        PluginCaller proxy = new PluginCaller(plugindllfullpath, nomeplugin,
                                                                SessioneRemota: sessioneRemota,
                                                                IsOSServer: isOSServer);
                        if (proxy != null)
                        {
                            object ret = proxy.Esegui(oDictionary);
                            if (ret != null) pdfgeneratofullpath = (string)ret;
                        }
                        else
                        {
                            throw new Exception("Errore nel caricamento del plugin");
                        }
                        proxy.Dispose();
                        proxy = null;

                    }

                }

                return pdfgeneratofullpath;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }



        public bool generaearchiviaPDF(string plugindllfullpath, bool firmaDigitale, string utenteFirma, bool sessioneRemota, bool isOSServer, bool evc, bool allegati)
        {
            try
            {
                return generaearchiviaPDF(plugindllfullpath: plugindllfullpath,
                                          firmaDigitale: firmaDigitale,
                                          utenteFirma: utenteFirma,
                                          sessioneRemota: sessioneRemota,
                                          isOSServer: isOSServer,
                                          evc: evc,
                                          soloAllegaInCartella: false,
                                          allegati: allegati);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool generaearchiviaPDF(string plugindllfullpath, bool firmaDigitale, string utenteFirma, bool sessioneRemota, bool isOSServer, bool evc, bool soloAllegaInCartella, bool allegati)
        {
            try
            {
                byte[] tmp = null;
                return generaearchiviaPDF(plugindllfullpath: plugindllfullpath,
                                          firmaDigitale: firmaDigitale,
                                          utenteFirma: utenteFirma,
                                          pdfexportfullpath: "",
                                          sessioneRemota: sessioneRemota,
                                          isOSServer: isOSServer,
                                          evc: evc,
                                          soloAllegaInCartella: soloAllegaInCartella,
                                          allegati: allegati,
                                          fileContent: out tmp);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool generaearchiviaPDF(string plugindllfullpath,
                                       bool firmaDigitale,
                                       string utenteFirma,
                                       bool sessioneRemota,
                                       bool isOSServer,
                                       bool evc,
                                       bool allegati,
                                       out byte[] fileContent)
        {
            try
            {
                return generaearchiviaPDF(plugindllfullpath: plugindllfullpath,
                                          firmaDigitale: firmaDigitale,
                                          utenteFirma: utenteFirma,
                                          sessioneRemota: sessioneRemota,
                                          isOSServer: isOSServer,
                                          evc: evc,
                                          soloAllegaInCartella: false,
                                          allegati: allegati,
                                          fileContent: out fileContent);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool generaearchiviaPDF(string plugindllfullpath,
                                       bool firmaDigitale,
                                       string utenteFirma,
                                       bool sessioneRemota,
                                       bool isOSServer,
                                       bool evc,
                                       bool soloAllegaInCartella,
                                       bool allegati,
                                       out byte[] fileContent)
        {
            try
            {
                return generaearchiviaPDF(plugindllfullpath: plugindllfullpath,
                                          firmaDigitale: firmaDigitale,
                                          utenteFirma: utenteFirma,
                                          pdfexportfullpath: "",
                                          sessioneRemota: sessioneRemota,
                                          isOSServer: isOSServer,
                                          evc: evc,
                                          soloAllegaInCartella: soloAllegaInCartella,
                                          allegati: allegati,
                                          fileContent: out fileContent);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool generaearchiviaPDF(string plugindllfullpath,
                                       bool firmaDigitale,
                                       string utenteFirma,
                                       string pdfexportfullpath,
                                       bool sessioneRemota,
                                       bool isOSServer,
                                       bool evc,
                                       bool allegati,
                                       out byte[] fileContent,
                                       bool ignoraControlloCartellaAggiornabile = false)
        {
            try
            {
                return generaearchiviaPDF(plugindllfullpath: plugindllfullpath,
                                          firmaDigitale: firmaDigitale,
                                          utenteFirma: utenteFirma,
                                          pdfexportfullpath: pdfexportfullpath,
                                          sessioneRemota: sessioneRemota,
                                          isOSServer: isOSServer,
                                          evc: evc,
                                          soloAllegaInCartella: false,
                                          allegati: allegati,
                                          fileContent: out fileContent);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool generaearchiviaPDF(string plugindllfullpath,
                                       bool firmaDigitale,
                                       string utenteFirma,
                                       string pdfexportfullpath,
                                       bool sessioneRemota,
                                       bool isOSServer,
                                       bool evc,
                                       bool soloAllegaInCartella,
                                       bool allegati,
                                       out byte[] fileContent,
                                       bool ignoraControlloCartellaAggiornabile = false)
        {
            try
            {
                bool bReturn = false;
                fileContent = null;
                if (this.PDFCartellaAggiornabile || ignoraControlloCartellaAggiornabile)
                {
                    string pdfgeneratofullpath = generaPDF(plugindllfullpath, firmaDigitale, utenteFirma, pdfexportfullpath, sessioneRemota, isOSServer);

                    if (pdfgeneratofullpath != null && pdfgeneratofullpath != string.Empty && pdfgeneratofullpath.Trim() != "" && System.IO.File.Exists(pdfgeneratofullpath))
                    {

                        if (evc)
                        {
                            pdfgeneratofullpath = aggiungoEvc(pdfgeneratofullpath, soloAllegaInCartella);
                        }

                        if (allegati)
                        {
                            pdfgeneratofullpath = aggiungoAllegati(pdfgeneratofullpath);
                        }

                        bReturn = archiviaPDF(pdfgeneratofullpath, true);

                        try
                        {
                            if (System.IO.File.Exists(pdfgeneratofullpath)) fileContent = System.IO.File.ReadAllBytes(pdfgeneratofullpath);
                        }
                        catch
                        {
                        }

                        try
                        {
                            System.IO.File.Delete(pdfgeneratofullpath);
                        }
                        catch (Exception)
                        {
                        }
                    }


                }

                return bReturn;

            }
            catch (Exception)
            {
                throw;
            }
        }


        private string aggiungoEvc(string plugindllfullpath)
        {
            return aggiungoEvc(plugindllfullpath, false);
        }

        private string aggiungoEvc(string plugindllfullpath, bool soloAllegaInCartella)
        {

            string sret = plugindllfullpath;

            try
            {

                List<string> filesreferti = new List<string>();

                filesreferti.Add(plugindllfullpath);

                Parametri op = new Parametri(_ambiente);
                op.Parametro.Add("IDCartella", this.ID);

                if (soloAllegaInCartella)
                    op.Parametro.Add("SoloAllegaInCartella", "1");

                op.TimeStamp.CodEntita = EnumEntita.EVC.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelStampaCartellaEvidenzaClinica", spcoll);
                if (dt != null)
                {
                    int i = 0;

                    foreach (DataRow drevc in dt.Rows)
                    {
                        i = i + 1;
                        if (!drevc.IsNull("PDFDWH"))
                        {

                            byte[] documento = (byte[])drevc["PDFDWH"];
                            string snewfilename = System.IO.Path.Combine(FileStatics.GetSCCITempPath() + @"EVC_" + this.ID + @"_" + DateTime.Now.ToString("MMddHHmmssffff") + @"_" + i.ToString() + @".pdf");
                            if (UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(snewfilename, ref documento))
                            {
                                if (System.IO.File.Exists(snewfilename))
                                {
                                    filesreferti.Add(snewfilename);
                                }
                            }

                        }

                    }

                }

                if (filesreferti != null && filesreferti.Count > 1)
                {
                    string sReturn = "EVCALL_" + this.ID + @"_" + DateTime.Now.ToString("MMddHHmmssffff") + @".pdf";
                    sReturn = System.IO.Path.Combine(FileStatics.GetSCCITempPath() + sReturn);

                    CommonStatics.MergePDFFiles(filesreferti, sReturn, false);

                    if (System.IO.File.Exists(sReturn))
                    {
                        sret = sReturn;
                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return sret;

        }

        private string aggiungoAllegati(string plugindllfullpath)
        {

            string sret = plugindllfullpath;

            try
            {

                List<string> filesreferti = new List<string>();

                filesreferti.Add(plugindllfullpath);

                Parametri op = new Parametri(_ambiente);
                op.Parametro.Add("IDCartella", this.ID);
                op.TimeStamp.CodEntita = EnumEntita.ALL.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelStampaCartellaAllegati", spcoll);
                if (dt != null)
                {
                    int i = 0;

                    foreach (DataRow drall in dt.Rows)
                    {

                        i++;

                        if (!drall.IsNull("Documento"))
                        {

                            byte[] documento = (byte[])drall["Documento"];
                            string snewfilename = System.IO.Path.Combine(FileStatics.GetSCCITempPath() + @"ALL_" + this.ID + @"_" + DateTime.Now.ToString("MMddHHmmssffff") + i.ToString() + @".pdf");

                            if (UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(snewfilename, ref documento))
                            {
                                if (System.IO.File.Exists(snewfilename))
                                {
                                    filesreferti.Add(snewfilename);
                                }
                            }

                        }

                    }

                }

                if (filesreferti != null && filesreferti.Count > 1)
                {
                    string sReturn = "ALLALL_" + this.ID + "_" + DateTime.Now.ToString("MMddHHmmssffff") + @".pdf";
                    sReturn = System.IO.Path.Combine(FileStatics.GetSCCITempPath() + sReturn);

                    CommonStatics.MergePDFFiles(filesreferti, sReturn, false);

                    if (System.IO.File.Exists(sReturn))
                    {
                        sret = sReturn;
                    }

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            return sret;

        }

    }

}
