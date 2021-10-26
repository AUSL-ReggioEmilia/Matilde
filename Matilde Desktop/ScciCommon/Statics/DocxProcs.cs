using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using UnicodeSrl.DatiClinici.Gestore;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace UnicodeSrl.Scci.Statics
{
    public static class DocxProcs
    {
        public const string GC_CHR_START = @"#";
        public const string GC_CHR_END = @"§";

        public struct CreaDocxReturn
        {
            public string docxgeneratofullpath;
            public string errori;
        }

        public static CreaDocxReturn CreaReportDOCX(byte[] modellodocx, Gestore gestore)
        {
            try
            {
                return CreaReportDOCX(modellodocx, gestore, "");
            }
            catch (Exception ex)
            {
                CreaDocxReturn ret = new CreaDocxReturn();
                ret.docxgeneratofullpath = "";
                ret.errori = ex.Message;
                return ret;
            }
        }
        public static CreaDocxReturn CreaReportDOCX(byte[] modellodocx, Gestore gestore, string docxoutputfullpath)
        {
            CreaDocxReturn ret = new CreaDocxReturn();
            ret.docxgeneratofullpath = "";
            ret.errori = "";
            try
            {
                string modellodocxfullpath = "";

                if (modellodocx != null && modellodocx.Length > 0)
                {
                    modellodocxfullpath = "TMP" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + @".docx";
                    modellodocxfullpath = System.IO.Path.Combine(FileStatics.GetSCCITempPath() + modellodocxfullpath);
                    UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(modellodocxfullpath, ref modellodocx);

                    ret = CreaReportDOCX(modellodocxfullpath, gestore, docxoutputfullpath);

                    try
                    {
                        if (System.IO.File.Exists(modellodocxfullpath)) System.IO.File.Delete(modellodocxfullpath);
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                    ret.errori = "Modello mancante!";

            }
            catch (Exception ex)
            {
                ret.errori = ex.Message;
            }
            return ret;
        }
        public static CreaDocxReturn CreaReportDOCX(string modellodocxfullpath, Gestore gestore)
        {
            try
            {
                return CreaReportDOCX(modellodocxfullpath, gestore, "");
            }
            catch (Exception ex)
            {
                CreaDocxReturn ret = new CreaDocxReturn();
                ret.docxgeneratofullpath = "";
                ret.errori = ex.Message;
                return ret;
            }
        }
        public static CreaDocxReturn CreaReportDOCX(string modellodocxfullpath, Gestore gestore, string docxoutputfullpath)
        {
            CreaDocxReturn ret = new CreaDocxReturn();
            ret.docxgeneratofullpath = "";
            ret.errori = "";

            try
            {

                if (modellodocxfullpath != null && modellodocxfullpath != string.Empty && modellodocxfullpath.Trim() != "" && System.IO.File.Exists(modellodocxfullpath))
                {

                    DocX docx = DocX.Load(modellodocxfullpath);
                    string returnerr = "";


                    try
                    {
                        if (docx.Headers.First != null)
                        {
                            Container container = docx.Headers.First;
                            returnerr = "";
                            returnerr = ValutaFormuleParagrafi(container, gestore);
                            if (returnerr != null && returnerr != string.Empty && returnerr.Trim() != "")
                            {
                                if (ret.errori != null && ret.errori.Trim() != "") ret.errori += Environment.NewLine;
                                ret.errori += returnerr;
                            }
                        }
                        if (docx.Headers.Even != null)
                        {
                            Container container = docx.Headers.Even;
                            returnerr = "";
                            returnerr = ValutaFormuleParagrafi(container, gestore);
                            if (returnerr != null && returnerr != string.Empty && returnerr.Trim() != "")
                            {
                                if (ret.errori != null && ret.errori.Trim() != "") ret.errori += Environment.NewLine;
                                ret.errori += returnerr;
                            }
                        }
                        if (docx.Headers.Odd != null)
                        {
                            Container container = docx.Headers.Odd;
                            returnerr = "";
                            returnerr = ValutaFormuleParagrafi(container, gestore);
                            if (returnerr != null && returnerr != string.Empty && returnerr.Trim() != "")
                            {
                                if (ret.errori != null && ret.errori.Trim() != "") ret.errori += Environment.NewLine;
                                ret.errori += returnerr;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ret.errori != null && ret.errori.Trim() != "") ret.errori += Environment.NewLine;
                        ret.errori += ex.Message;
                    }

                    try
                    {
                        Container container = docx;
                        returnerr = "";
                        returnerr = ValutaFormuleParagrafi(container, gestore);
                        if (returnerr != null && returnerr != string.Empty && returnerr.Trim() != "")
                        {
                            if (ret.errori != null && ret.errori.Trim() != "") ret.errori += Environment.NewLine;
                            ret.errori += returnerr;
                        }

                    }
                    catch (Exception ex)
                    {
                        if (ret.errori != null && ret.errori.Trim() != "") ret.errori += Environment.NewLine;
                        ret.errori += ex.Message;
                    }

                    try
                    {
                        if (docx.Footers.First != null)
                        {
                            Container container = docx.Footers.First;
                            returnerr = "";
                            returnerr = ValutaFormuleParagrafi(container, gestore);
                            if (returnerr != null && returnerr != string.Empty && returnerr.Trim() != "")
                            {
                                if (ret.errori != null && ret.errori.Trim() != "") ret.errori += Environment.NewLine;
                                ret.errori += returnerr;
                            }
                        }
                        if (docx.Footers.Even != null)
                        {
                            Container container = docx.Footers.Even;
                            returnerr = "";
                            returnerr = ValutaFormuleParagrafi(container, gestore);
                            if (returnerr != null && returnerr != string.Empty && returnerr.Trim() != "")
                            {
                                if (ret.errori != null && ret.errori.Trim() != "") ret.errori += Environment.NewLine;
                                ret.errori += returnerr;
                            }
                        }
                        if (docx.Footers.Odd != null)
                        {
                            Container container = docx.Footers.Odd;
                            returnerr = "";
                            returnerr = ValutaFormuleParagrafi(container, gestore);
                            if (returnerr != null && returnerr != string.Empty && returnerr.Trim() != "")
                            {
                                if (ret.errori != null && ret.errori.Trim() != "") ret.errori += Environment.NewLine;
                                ret.errori += returnerr;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ret.errori != null && ret.errori.Trim() != "") ret.errori += Environment.NewLine;
                        ret.errori += ex.Message;
                    }


                    MemoryStream ms = new MemoryStream();


                    docx.SaveAs(ms);

                    if (docxoutputfullpath == null || docxoutputfullpath == string.Empty || docxoutputfullpath.Trim() == "")
                    {
                        ret.docxgeneratofullpath = "MTLD" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + @".docx";
                        ret.docxgeneratofullpath = System.IO.Path.Combine(FileStatics.GetSCCITempPath() + ret.docxgeneratofullpath);
                    }
                    else
                        ret.docxgeneratofullpath = docxoutputfullpath;

                    if (System.IO.File.Exists(ret.docxgeneratofullpath)) System.IO.File.Delete(ret.docxgeneratofullpath);

                    FileStream file = new FileStream(ret.docxgeneratofullpath, FileMode.Create, System.IO.FileAccess.Write);

                    ms.WriteTo(file);

                    file.Close();
                    ms.Close();

                    docx.Dispose();
                    file.Dispose();
                    ms.Dispose();


                }
                else
                {
                }

            }
            catch (Exception ex)
            {
                ret.docxgeneratofullpath = "";

                if (ret.errori != null && ret.errori.Trim() != "") ret.errori += Environment.NewLine;
                ret.errori += ex.Message;
            }

            return ret;
        }

        public static string ValutaFormula(string formula, Gestore gestore)
        {
            return ValutaFormula(formula, gestore, "");
        }
        public static string ValutaFormula(string formula, Gestore gestore, string ritornodefault)
        {

            string valoredecodificato = ritornodefault;

            try
            {
                valoredecodificato = gestore.Valutatore.Process(formula, gestore.Contesto);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
            return valoredecodificato;
        }

        private static string ValutaFormuleParagrafi(Container container, Gestore gestore)
        {
            string err = "";
            for (int p = 0; p < container.Paragraphs.Count; p++)
            {
                try
                {
                    Paragraph paragraph = container.Paragraphs[p];

                    string returnerr = "";
                    returnerr = ValutaFormuleParagrafo(ref paragraph, gestore);

                    if (returnerr != null && returnerr != string.Empty && returnerr.Trim() != "")
                    {
                        if (err != null && err.Trim() != "") err += Environment.NewLine;
                        err += returnerr;
                    }

                }
                catch (Exception ex)
                {
                    if (err != null && err.Trim() != "") err += Environment.NewLine;
                    err += ex.Message;
                }
            }

            return err;
        }

        private static string ValutaFormuleParagrafo(ref Paragraph container, Gestore gestore)
        {
            string returnerror = "";
            try
            {
                List<int> starts = container.FindAll(GC_CHR_START);
                List<int> ends = container.FindAll(GC_CHR_END);
                List<string> formule = new List<string>();

                if (starts != null && starts.Count > 0 && ends != null && ends.Count > 0)
                {

                    for (int ne = ends.Count - 1; ne >= 0; ne--)
                    {
                        try
                        {
                            int iEnd = ends[ne];

                            int iPrevEnd = -1;
                            if (ne > 0) iPrevEnd = ends[ne - 1];

                            int iStart = -1;
                            for (int ns = starts.Count - 1; ns >= 0; ns--)
                            {
                                if ((starts[ns] < iEnd && iPrevEnd == -1) || (starts[ns] < iEnd && starts[ns] > iPrevEnd))
                                {
                                    iStart = starts[ns];

                                    ns = -1;
                                }
                            }

                            if (iStart != -1)
                            {
                                string formula = "";
                                formula = container.Text.Substring(iStart, iEnd - iStart + 1);

                                if (formule == null) formule = new List<string>();
                                if (!formule.Contains(formula)) formule.Add(formula);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (returnerror != null && returnerror.Trim() != "") returnerror += Environment.NewLine;
                            returnerror += ex.Message;
                        }
                    }


                    if (formule != null && formule.Count > 0)
                    {
                        for (int f = 0; f < formule.Count; f++)
                        {
                            try
                            {
                                string result = ValutaFormula(formule[f], gestore, @"<MATILDE" + f.ToString("000") + @">");
                                container.ReplaceText(formule[f], result);
                            }
                            catch (Exception ex)
                            {
                                if (returnerror != null && returnerror.Trim() != "") returnerror += Environment.NewLine;
                                returnerror += ex.Message;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (returnerror != null && returnerror.Trim() != "") returnerror += Environment.NewLine;
                returnerror += ex.Message;
            }

            return returnerror;
        }

        public static string ValutaFormuleStringa(string vsTestoOriginale, Gestore gestore)
        {
            string stringavalutata = vsTestoOriginale;
            try
            {
                int iStart = stringavalutata.IndexOf(GC_CHR_START);
                int iEnd = -1;
                if (iStart >= 0) iEnd = stringavalutata.IndexOf(GC_CHR_END, iStart);

                while (iStart >= 0 && iStart < iEnd)
                {

                    string formula = stringavalutata.Substring(iStart, iEnd - iStart + 1);
                    string traduzione = gestore.Valutatore.Process(formula, gestore.Contesto);

                    string sleft = "";
                    string sright = "";
                    if (iStart > 0) sleft = stringavalutata.Substring(0, iStart);
                    if (iEnd < stringavalutata.Length - 1) sright = stringavalutata.Substring(iEnd + 1);

                    stringavalutata = sleft + traduzione + sright;

                    if (iEnd < stringavalutata.Length - 1)
                        iStart = stringavalutata.IndexOf(GC_CHR_START, iEnd);
                    else
                        iStart = -1;
                    if (iStart >= 0)
                        iEnd = stringavalutata.IndexOf(GC_CHR_END, iStart);
                    else
                        iEnd = -1;

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                stringavalutata = vsTestoOriginale;
            }

            return stringavalutata;
        }

    }
}
