using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Framework.Data;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;

namespace UnicodeSrl.Scci.Statics
{

    public static class CommonStatics
    {

        private static Dictionary<string, List<string>> ListUAPadri = new Dictionary<string, List<string>>();

        public static Gestore GetGestore(ScciAmbiente ambiente)
        {

            try
            {

                Gestore oGestore = new Gestore();

                oGestore.Valutatore.Fillers.Add("FillerSistema", new EvaluatorFillerSistema());
                oGestore.Valutatore.Fillers.Add("FillerScci", new EvaluatorFillerScci());
                oGestore.Valutatore.Fillers.Add("FillerScheda", new EvaluatorFillerScheda());
                oGestore.Valutatore.Fillers.Add("FillerAltraScheda", new EvaluatorFillerAltraScheda());
                oGestore.Valutatore.Fillers.Add("FillerDLookUp", new EvaluatorFillerDLookUp());
                oGestore.Valutatore.Fillers.Add("FillerGeneric", new UnicodeSrl.Evaluator.EvaluatorFillerGeneric());

                oGestore.Contesto = new Dictionary<string, object>();
                oGestore.Contesto = (from x in ambiente.Contesto
                                     where (x.Value != null && x.Value.GetType() != typeof(UnicodeSrl.DatiClinici.DC.DcSchedaDati))
                                     select x).ToDictionary(x => x.Key, x => x.Value);

                if (oGestore.Contesto.ContainsKey("Ambiente") == false) oGestore.Contesto.Add("Ambiente", ambiente);

                return oGestore;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static string EncodeBase64StoredParameter(string string2encode)
        {
            string sret = string.Empty;

            try
            {
                byte[] pvarbin = Encoding.GetEncoding("ISO-8859-1").GetBytes(string2encode);
                if (pvarbin == null)
                    sret = string.Empty;
                else
                    sret = Convert.ToBase64String(pvarbin);

            }
            catch
            {
                sret = string.Empty;
            }

            return sret;
        }

        public static EnumStatoOrdine TraduciOEStato(OEStato stato)
        {

            EnumStatoOrdine oret = EnumStatoOrdine.CA;

            switch (stato)
            {
                case OEStato.Accettato:
                    oret = EnumStatoOrdine.AC;
                    break;
                case OEStato.Annullato:
                    oret = EnumStatoOrdine.AN;
                    break;
                case OEStato.Cancellato:
                    oret = EnumStatoOrdine.CA;
                    break;
                case OEStato.Erogato:
                    oret = EnumStatoOrdine.ER;
                    break;
                case OEStato.Errato:
                    oret = EnumStatoOrdine.ET;
                    break;
                case OEStato.InCarico:
                    oret = EnumStatoOrdine.IC;
                    break;
                case OEStato.Inoltrato:
                    oret = EnumStatoOrdine.VA;
                    break;
                case OEStato.Inserito:
                    oret = EnumStatoOrdine.IS;
                    break;
                case OEStato.NN:
                    oret = EnumStatoOrdine.NN;
                    break;
                case OEStato.Programmato:
                    oret = EnumStatoOrdine.PR;
                    break;
            }

            return oret;

        }

        public static OEStato TraduciEnumStatoOrdine(EnumStatoOrdine stato)
        {

            OEStato oret = OEStato.Cancellato;

            switch (stato)
            {
                case EnumStatoOrdine.AC:
                    oret = OEStato.Accettato;
                    break;
                case EnumStatoOrdine.AN:
                    oret = OEStato.Annullato;
                    break;
                case EnumStatoOrdine.CA:
                    oret = OEStato.Cancellato;
                    break;
                case EnumStatoOrdine.ER:
                    oret = OEStato.Erogato;
                    break;
                case EnumStatoOrdine.ET:
                    oret = OEStato.Errato;
                    break;
                case EnumStatoOrdine.IC:
                    oret = OEStato.InCarico;
                    break;
                case EnumStatoOrdine.VA:
                    oret = OEStato.Inoltrato;
                    break;
                case EnumStatoOrdine.IS:
                    oret = OEStato.Inserito;
                    break;
                case EnumStatoOrdine.NN:
                    oret = OEStato.NN;
                    break;
                case EnumStatoOrdine.PR:
                    oret = OEStato.Programmato;
                    break;
            }

            return oret;

        }

        public static List<string> UAPadri(string codua, ScciAmbiente ambiente)
        {

            if (!ListUAPadri.ContainsKey(codua))
            {

                Parametri op = new Parametri(ambiente);
                op.Parametro.Add("CodUA", codua);

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelUAPadri", spcoll);

                List<string> _uapadri = new List<string>();

                if (dt.Rows.Count > 0)
                {

                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                    {
                        _uapadri.Add(dt.Rows[i]["Codice"].ToString());
                    }

                }

                ListUAPadri.Add(codua, _uapadri);

            }

            return ListUAPadri[codua];

        }

        public static string ConvertiNosologicoGST(string nosologicogst)
        {
            string sret = string.Empty;
            DataTable dt = null;

            try
            {
                dt = Database.GetDatatable("SELECT dbo.MF_ModificaNosologico('" + nosologicogst + "') AS Nosologico");

                if (dt != null && dt.Rows.Count > 0)
                {
                    sret = dt.Rows[0]["Nosologico"].ToString();
                }
                else
                {
                    sret = string.Empty;
                }
            }
            catch
            {
                sret = string.Empty;
            }
            finally
            {
                if (dt != null)
                {
                    dt.Dispose();
                    dt = null;
                }
            }

            return sret;
        }

        public static string CercaPazienteDaSAC(string idsac, ScciAmbiente ambiente)
        {

            string IDPaziente = string.Empty;

            try
            {

                Parametri op = new Parametri(ambiente);
                op.Parametro.Add("CodSAC", Database.testoSQL(idsac));

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = UnicodeSrl.Scci.Statics.Database.GetDataTableStoredProc("MSP_CercaPazienteDaSAC", spcoll);

                if (dt.Rows.Count == 1)
                {
                    IDPaziente = (dt.Rows[0]["ID"].ToString());
                }

            }
            catch (Exception)
            {

            }

            return IDPaziente;

        }

        public static void UpdateConsensiDaSAC(string idpaziente, List<PazienteSacConsensi> pazientesacconsensi, ScciAmbiente ambiente)
        {

            string sRet = string.Empty;

            bool bGenerico = false;
            bool bDossier = false;
            bool bDossierStorico = false;
            PazientiConsensi oPC = null;

            try
            {

                if (pazientesacconsensi != null)
                {

                    foreach (PazienteSacConsensi consenso in pazientesacconsensi)
                    {

                        if (consenso.Tipo == EnumTipoConsenso.Generico.ToString() ||
                            consenso.Tipo == EnumTipoConsenso.Dossier.ToString() ||
                            consenso.Tipo == EnumTipoConsenso.DossierStorico.ToString())
                        {

                            oPC = new PazientiConsensi(idpaziente, consenso.Tipo, ambiente);
                            if (oPC.IDPaziente == string.Empty)
                            {

                                oPC = new PazientiConsensi(ambiente);
                                oPC.IDPaziente = idpaziente;
                                oPC.CodTipoConsenso = consenso.Tipo;

                            }

                            oPC.CodSistemaProvenienza = consenso.Provenienza;
                            oPC.IDProvenienza = consenso.IDProvenienza;
                            oPC.CodStatoConsenso = (consenso.Stato == false ? EnumStatoConsenso.NO.ToString() : EnumStatoConsenso.SI.ToString());
                            oPC.DataConsenso = consenso.DataStato;
                            oPC.CodOperatore = consenso.OperatoreId;
                            oPC.CognomeOperatore = consenso.OperatoreCognome;
                            oPC.NomeOperatore = consenso.OperatoreNome;
                            oPC.ComputerOperatore = consenso.OperatoreComputer;
                            oPC.Salva();
                            oPC = null;

                            switch ((EnumTipoConsenso)Enum.Parse(typeof(EnumTipoConsenso), consenso.Tipo))
                            {

                                case EnumTipoConsenso.Generico:
                                    bGenerico = true;
                                    break;

                                case EnumTipoConsenso.Dossier:
                                    bDossier = true;
                                    break;

                                case EnumTipoConsenso.DossierStorico:
                                    bDossierStorico = true;
                                    break;

                            }

                        }

                    }

                }

                if (bGenerico == false)
                {

                    oPC = new PazientiConsensi(idpaziente, EnumTipoConsenso.Generico.ToString(), ambiente);
                    if (oPC.IDPaziente != string.Empty)
                    {
                        oPC.CodStatoConsenso = EnumStatoConsenso.ND.ToString();
                        oPC.DataDisattivazione = DateTime.Now;
                        oPC.DataConsenso = DateTime.MinValue;
                        oPC.Salva();
                    }

                }
                if (bDossier == false)
                {

                    oPC = new PazientiConsensi(idpaziente, EnumTipoConsenso.Dossier.ToString(), ambiente);
                    if (oPC.IDPaziente != string.Empty)
                    {
                        oPC.CodStatoConsenso = EnumStatoConsenso.ND.ToString();
                        oPC.DataDisattivazione = DateTime.Now;
                        oPC.DataConsenso = DateTime.MinValue;
                        oPC.Salva();
                    }

                }
                if (bDossierStorico == false)
                {

                    oPC = new PazientiConsensi(idpaziente, EnumTipoConsenso.DossierStorico.ToString(), ambiente);
                    if (oPC.IDPaziente != string.Empty)
                    {
                        oPC.CodStatoConsenso = EnumStatoConsenso.ND.ToString();
                        oPC.DataDisattivazione = DateTime.Now;
                        oPC.DataConsenso = DateTime.MinValue;
                        oPC.Salva();
                    }

                }

                oPC = null;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public static EnumMaschere RecuperaMascheraApertura(EnumCommandLineModules? moduloApertura, bool ambulatoriale)
        {
            EnumMaschere ret = EnumMaschere.Ambulatoriale_Cartella;
            if (ambulatoriale) ret = EnumMaschere.CartellaPaziente;

            if (moduloApertura != null && moduloApertura.HasValue)
            {
                switch (moduloApertura.Value)
                {
                    case EnumCommandLineModules.Schede:
                        if (ambulatoriale)
                            ret = EnumMaschere.Ambulatoriale_Schede;
                        else
                            ret = EnumMaschere.Schede;
                        break;


                    case EnumCommandLineModules.Cartella:
                    default:
                        if (ambulatoriale)
                            ret = EnumMaschere.Ambulatoriale_Cartella;
                        else
                            ret = EnumMaschere.CartellaPaziente;
                        break;
                }
            }

            return ret;
        }

        public static void MergePDFFiles(List<string> sourceFiles, string destinationFile, bool addblankpage)
        {
            try
            {
                int f = 0;
                PdfReader reader = new PdfReader(sourceFiles[f]);
                int n = reader.NumberOfPages;

                Document document = new Document(reader.GetPageSizeWithRotation(1));
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(destinationFile, FileMode.Create));
                document.Open();
                PdfContentByte cb = writer.DirectContent;
                PdfImportedPage page;
                int rotation;
                while (f < sourceFiles.Count)
                {
                    int i = 0;
                    while (i < n)
                    {
                        i++;
                        document.SetPageSize(reader.GetPageSizeWithRotation(i));
                        document.NewPage();
                        page = writer.GetImportedPage(reader, i);
                        rotation = reader.GetPageRotation(i);
                        if (rotation == 90 || rotation == 270)
                        {
                            cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(i).Height);
                        }
                        else
                        {
                            cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                        }
                    }
                    if (addblankpage && f < sourceFiles.Count - 1)
                    {
                        if (i > 0 && (i == 1 || i % 2 != 0))
                        {
                            document.NewPage();
                            document.Add(new Paragraph(" "));
                        }
                    }

                    f++;

                    if (f < sourceFiles.Count)
                    {
                        reader = new PdfReader(sourceFiles[f]);
                        n = reader.NumberOfPages;
                    }
                }
                try
                {
                    document.Close();
                    document.Dispose();
                }
                catch
                {

                }
                try
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader.Dispose();
                    }
                }
                catch
                {
                }
                try
                {
                    if (writer != null)
                    {
                        writer.Close();
                        writer.Dispose();
                    }
                }
                catch
                {
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }

}
