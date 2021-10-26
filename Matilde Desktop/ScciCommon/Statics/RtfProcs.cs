using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using UnicodeSrl.Scci.Enums;
using System.Data;
using Microsoft.Data.SqlClient;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Extension;

namespace UnicodeSrl.Scci.Statics
{
    public static class RtfProcs
    {

        public const string GC_TUTTI = @"Tutti";
        public const string GC_INIZIOGRASSETTO = @"Ç";
        public const string GC_FINEGRASSETTO = @"Ü";
        public const string GC_INIZIOITALICO = @"Ā";
        public const string GC_FINEITALICO = @"Ą";
        public const string GC_INIZIOSOTTOLINEATO = @"þ";
        public const string GC_FINESOTTOLINEATO = @"ñ";
        public const string GC_INIZIOBARRATO = @"Ê";
        public const string GC_FINEBARRATO = @"Ë";
        public const string GC_INIZIOGRASITALICO = @"õ";
        public const string GC_FINEGRASITALICO = @"ö";

        const string K_TAG_ROWD = @"\trowd";
        const string K_TAG_ROW = @"\row";

        const string K_TAG_CELLX = @"\cellx";
        const string K_TAG_CELL = @"\";

        public static void FormattaRTF(ref System.Windows.Forms.RichTextBox rtfbox, EnumTipoFormattazione tipoformattazione)
        {
            int inizioselezione = -1;
            int fineselezione = -1;
            string cariniziosel = "";
            string carfinesel = "";

            switch (tipoformattazione)
            {
                case EnumTipoFormattazione.Barrato:
                    cariniziosel = GC_INIZIOBARRATO;
                    carfinesel = GC_FINEBARRATO;
                    break;
                case EnumTipoFormattazione.Grassetto:
                    cariniziosel = GC_INIZIOGRASSETTO;
                    carfinesel = GC_FINEGRASSETTO;
                    break;
                case EnumTipoFormattazione.Italico:
                    cariniziosel = GC_INIZIOITALICO;
                    carfinesel = GC_FINEITALICO;
                    break;
                case EnumTipoFormattazione.Sottolineato:
                    cariniziosel = GC_INIZIOSOTTOLINEATO;
                    carfinesel = GC_FINESOTTOLINEATO;
                    break;

                case EnumTipoFormattazione.GrasItalico:
                    cariniziosel = GC_INIZIOGRASITALICO;
                    carfinesel = GC_FINEGRASITALICO;
                    break;


            }

            try
            {
                do
                {
                    inizioselezione = rtfbox.Find(cariniziosel);
                    if (inizioselezione != -1)
                    {

                        fineselezione = rtfbox.Find(carfinesel);

                        if (fineselezione != -1)
                        {
                            rtfbox.SelectionStart = inizioselezione;
                            rtfbox.SelectionLength = fineselezione - inizioselezione + 1;

                            System.Drawing.Font currentFont = rtfbox.SelectionFont;
                            System.Drawing.FontStyle newFontStyle;
                            System.Drawing.FontStyle font = new FontStyle();

                            switch (tipoformattazione)
                            {
                                case EnumTipoFormattazione.Barrato:
                                    font = FontStyle.Strikeout;
                                    break;
                                case EnumTipoFormattazione.Grassetto:
                                    font = FontStyle.Bold;
                                    break;
                                case EnumTipoFormattazione.Italico:
                                    font = FontStyle.Italic;
                                    break;
                                case EnumTipoFormattazione.Sottolineato:
                                    font = FontStyle.Underline;
                                    break;
                                case EnumTipoFormattazione.GrasItalico:
                                    font = FontStyle.Bold | FontStyle.Italic;
                                    break;


                            }

                            newFontStyle = rtfbox.SelectionFont.Style ^ font;

                            rtfbox.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);

                            rtfbox.SelectedText = rtfbox.SelectedText.Replace(cariniziosel, "").Replace(carfinesel, "");
                        }
                    }

                } while (inizioselezione != -1 && fineselezione != -1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void BarraTestoRTF(ref System.Windows.Forms.RichTextBox rtfbox)
        {
            rtfbox.Rtf = rtfbox.Rtf.Insert(rtfbox.Rtf.LastIndexOf("}", rtfbox.Rtf.Length - 4) + 1, @"\strike");
            rtfbox.Rtf = rtfbox.Rtf.Insert(rtfbox.Rtf.LastIndexOf("}", rtfbox.Rtf.Length) - 1, @"\strike0");
        }

        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.UTF8Encoding.UTF8.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        public static string InserisciAllegati(string rtfOrigine,
                                               string idMovScheda,
                                               ref UnicodeSrl.Scci.Plugin.MbrDatabase rDatabase,
                                               ref UnicodeSrl.Scci.DataContracts.ScciAmbiente rAmbiente)
        {
            string rtfReturn = rtfOrigine;
            try
            {
                if (rtfOrigine != null && rtfOrigine.Trim() != "")
                {

                    Parametri op = new Parametri(rAmbiente);
                    op.Parametro.Add("IDScheda", idMovScheda);
                    op.Parametro.Add("TipoRichiesta", EnumTipoRichiestaAllegatoScheda.LISTA.ToString());
                    SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataSet dsListaAllegati = rDatabase.GetDatasetStoredProc("MSP_SelMovSchedaAllegati", spcoll);

                    if (dsListaAllegati != null && dsListaAllegati.Tables.Count > 0 && dsListaAllegati.Tables[0].Rows.Count > 0)
                    {
                        int iMaxW = -1;
                        int iMaxH = -1;
                        System.Drawing.Size sz = System.Drawing.Size.Empty;
                        string tmpW = rDatabase.GetConfigTable(EnumConfigTable.AllegatiSchedeStampaWidth);
                        string tmpH = rDatabase.GetConfigTable(EnumConfigTable.AllegatiSchedeStampaHeight);
                        if (!int.TryParse(tmpW, out iMaxW)) iMaxW = -1;
                        if (!int.TryParse(tmpH, out iMaxH)) iMaxH = -1;
                        if (iMaxW > 0 && iMaxH > 0) sz = new Size(iMaxW, iMaxH);
                        iMaxW = -1;
                        iMaxH = -1;
                        tmpW = "";
                        tmpH = "";
                        System.Drawing.Size szA = System.Drawing.Size.Empty;
                        tmpW = rDatabase.GetConfigTable(EnumConfigTable.AllegatiSchedeAntemprimaWidth);
                        tmpH = rDatabase.GetConfigTable(EnumConfigTable.AllegatiSchedeAntemprimaHeight);
                        if (!int.TryParse(tmpW, out iMaxW)) iMaxW = -1;
                        if (!int.TryParse(tmpH, out iMaxH)) iMaxH = -1;
                        if (iMaxW > 0 && iMaxH > 0) szA = new Size(iMaxW, iMaxH);

                        UnicodeSrl.Scci.RTF.RtfFiles rtff = new RTF.RtfFiles();
                        string tmpRtf = rtfOrigine;
                        string sTag = "";
                        string tmpRtfSx = "";
                        string tmpRtfDx = "";

                        int iStartTag = tmpRtf.IndexOf(CommonConstants.C_ALLEGATO_TAG_START);
                        int iEndTag = -1;
                        while (iStartTag >= 0)
                        {
                            iEndTag = tmpRtf.IndexOf(CommonConstants.C_ALLEGATO_TAG_END, iStartTag);

                            if (iEndTag > iStartTag)
                            {
                                sTag = tmpRtf.Substring(iStartTag, iEndTag + 1 - iStartTag);

                                int iSxLen = iEndTag + 1;
                                int iDxStart = iEndTag + 1;
                                string sLine = @"";
                                string sPar = @"";



                                iSxLen = iStartTag;
                                sLine = @"\line";
                                if (tmpRtf.IndexOf(@"\line", iEndTag + 1) > 0)
                                {
                                    iDxStart = tmpRtf.IndexOf(@"\line", iEndTag + 1) + @"\line".Length;
                                    sPar = @"\par \par";
                                }
                                else
                                {
                                    if (tmpRtf.IndexOf(@"\tab", iEndTag + 1) > 0)
                                    {
                                        iDxStart = tmpRtf.IndexOf(@"\tab", iEndTag + 1) + 4;
                                        sPar = @"";
                                    }
                                }



                                tmpRtfSx = tmpRtf.Substring(0, iSxLen) + sLine;
                                tmpRtfDx = sPar + tmpRtf.Substring(iDxStart);

                                dsListaAllegati.Tables[0].DefaultView.RowFilter = @"DescrizioneAllegato = '" + rDatabase.testoSQL(sTag) + @"'";
                                if (dsListaAllegati.Tables[0].DefaultView.Count > 0)
                                {
                                    string idMovSchedaAllegato = dsListaAllegati.Tables[0].DefaultView[0][@"ID"].ToString();

                                    op = new Parametri(rAmbiente);
                                    op.Parametro.Add("IDSchedaAllegato", idMovSchedaAllegato);
                                    op.Parametro.Add("TipoRichiesta", "");
                                    spcoll = new SqlParameterExt[1];
                                    xmlParam = XmlProcs.XmlSerializeToString(op);
                                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                                    DataSet dsAllegato = rDatabase.GetDatasetStoredProc("MSP_SelMovSchedaAllegati", spcoll);
                                    if (dsAllegato != null && dsAllegato.Tables.Count > 0 && dsAllegato.Tables[0].Rows.Count > 0 && (!dsAllegato.Tables[0].Rows[0].IsNull("Documento") || !dsAllegato.Tables[0].Rows[0].IsNull("Anteprima")))
                                    {
                                        bool bImage = false;
                                        bool bAnteprima = false;
                                        if (dsAllegato.Tables[0].Rows[0].IsNull("Documento") && !dsAllegato.Tables[0].Rows[0].IsNull("Anteprima"))
                                        {
                                            bImage = false;
                                            bAnteprima = true;
                                        }
                                        else
                                        {

                                            string sTipo = "";
                                            if (!dsAllegato.Tables[0].Rows[0].IsNull("CodTipoAllegatoScheda")) sTipo = dsAllegato.Tables[0].Rows[0]["CodTipoAllegatoScheda"].ToString();
                                            sTipo = sTipo.Trim().ToUpper();
                                            if (sTipo == EnumTipoAllegatoScheda.IMG.ToString())
                                            {
                                                bImage = true;
                                                bAnteprima = false;
                                            }
                                            else
                                            {
                                                bImage = false;
                                                bAnteprima = !dsAllegato.Tables[0].Rows[0].IsNull("Anteprima");
                                            }
                                        }

                                        if (bImage)
                                        {
                                            try
                                            {
                                                byte[] byAllegato = (byte[])dsAllegato.Tables[0].Rows[0]["Documento"];

                                                string rtfImmagine = rtff.InsertImage(byAllegato, sz);

                                                tmpRtf = tmpRtfSx + rtfImmagine + tmpRtfDx;

                                            }
                                            catch
                                            {
                                                bAnteprima = !dsAllegato.Tables[0].Rows[0].IsNull("Anteprima");
                                            }
                                        }

                                        if (bAnteprima)
                                        {
                                            try
                                            {
                                                byte[] byAllegato = (byte[])dsAllegato.Tables[0].Rows[0]["Anteprima"];

                                                string rtfAnteprima = rtff.InsertImage(byAllegato, szA);

                                                tmpRtf = tmpRtfSx + rtfAnteprima + tmpRtfDx;
                                            }
                                            catch
                                            {
                                            }
                                        }

                                    }

                                }

                                iStartTag = tmpRtf.IndexOf(CommonConstants.C_ALLEGATO_TAG_START, iStartTag + 1);
                            }
                            else
                            {
                                iStartTag = -1;
                            }
                        }


                        rtfReturn = tmpRtf;

                    }

                }

            }
            catch (Exception)
            {
                throw;
            }
            return rtfReturn;
        }

        public static string CorrectRTFForReports(string rtfOrigine)
        {
            string rtfReturn = rtfOrigine;

            try
            {
                if (String.IsNullOrEmpty(rtfReturn) == false)
                {
                    rtfReturn = rtfReturn.Replace(@"{\pict\wmetafile8", @" \fs0 {\pict\wmetafile8");

                    rtfReturn = rtfReturn.Replace(@" \fs0 \fs0 {\pict\wmetafile8", @" \fs0 {\pict\wmetafile8");
                }
            }
            catch
            {
                rtfReturn = rtfOrigine;
            }

            return rtfReturn;

        }

        public static string FixRtfTableWidth(float controlWidth, string rtf)
        {

            const string K_TAG_ROWD = @"\trowd";
            const string K_TAG_ROW = @"\row";

            const string K_TAG_CELLX = @"\cellx";
            const string K_TAG_CELL = @"\";

            if (rtf.IsNullOrEmpty()) return rtf;

            rtf = rtf.Replace(@"\brdrw1 ", @"\brdrw2 ");
            rtf = rtf.Replace(@"\brdrw1\", @"\brdrw2\");

            rtf = rtf.Replace(@"\line", @"\par");

            if (rtf.Contains(K_TAG_ROWD) == false) return rtf;

            controlWidth = Convert.ToInt32(controlWidth * 0.99);

            List<string> listRows = RtfProcs.rtfGetTagContentList_trowd(rtf, K_TAG_ROWD, K_TAG_ROW);

            foreach (string rowstring in listRows)
            {
                List<string> listCells = RtfProcs.rtfGetTagContentList(rowstring, K_TAG_CELLX, K_TAG_CELL);
                List<float> listFloat = RtfProcs.rtfCellListTwips(listCells);

                float rowWidth = RtfProcs.rtfRowWidth(listFloat);

                if (rowWidth > controlWidth)
                {
                    string fixedRowString = rtfGetSizedRow(rowstring, listCells, listFloat, controlWidth);

                    rtf = rtf.Replace(rowstring, fixedRowString);
                }

            }


            return rtf;
        }

        private static List<String> rtfGetTagContentList_trowd(string rtf, string startTag, string endTag)
        {
            int pos = 0;

            List<String> list = new List<string>();

            for (int i = 0; i < rtf.Length; i++)
            {
                int rowstart = rtf.IndexOf(startTag, pos);

                if (rowstart <= 0) break;

                int rowendtrowd = rtf.IndexOf(startTag, rowstart + 1);
                int rowend = rtf.IndexOf(endTag, rowstart + 1);

                if (rowendtrowd != -1 && rowend > rowendtrowd)
                {
                    string rowstring = rtf.Substring(rowstart, rowendtrowd - rowstart - 1);
                    list.Add(rowstring);

                    pos = rowendtrowd - startTag.Length;
                }
                else
                {

                    string rowstring = rtf.Substring(rowstart, rowend - rowstart + endTag.Length);

                    list.Add(rowstring);

                    pos = rowend;

                }

            }

            return list;

        }

        private static List<String> rtfGetTagContentList(string rtf, string startTag, string endTag)
        {
            int pos = 0;

            List<String> list = new List<string>();

            for (int i = 0; i < rtf.Length; i++)
            {
                int rowstart = rtf.IndexOf(startTag, pos);

                if (rowstart <= 0) break;

                int rowend = rtf.IndexOf(endTag, rowstart + 1);

                string rowstring = rtf.Substring(rowstart, rowend - rowstart + endTag.Length);

                list.Add(rowstring.Replace("{", ""));

                pos = rowend;
            }


            return list;
        }

        private static List<float> rtfCellListTwips(List<string> listCells)
        {
            const string K_TAG_CELLX = @"\cellx";
            const string K_TAG_CELL = @"\";

            List<float> list = new List<float>();
            float prev = 0;

            foreach (string cell in listCells)
            {
                string cellval = cell.Replace(K_TAG_CELLX, "");
                cellval = cellval.Replace(K_TAG_CELL, "");

                float.TryParse(cellval, out float cellf);

                cellf = cellf - prev;
                prev += cellf;

                list.Add(cellf);
            }

            return list;
        }

        private static float rtfRowWidth(List<float> listFloat)
        {

            float sum = listFloat.Sum();
            return sum;
        }

        private static string rtfGetSizedRow(string rowString, List<string> listCells, List<float> listFloat, float controlWidth)
        {
            const string K_TAG_CLWWIDTH = @"\clwWidth";
            const string K_TAG_CELLX = @"\cellx";
            const string K_TAG_CELL = @"\";

            string result = rowString;

            float originalRowWidth = RtfProcs.rtfRowWidth(listFloat);
            float resizeFactor = controlWidth / originalRowWidth;

            float prev = 0;

            for (int i = 0; i < listCells.Count; i++)
            {
                float cellSz = listFloat[i];
                float cellWitdh = (float)Math.Truncate(cellSz * resizeFactor);
                string newCellString1 = $"{K_TAG_CLWWIDTH}{cellWitdh}{K_TAG_CELL}";

                cellWitdh = cellWitdh + prev;
                prev = cellWitdh;

                string newCellString = $"{K_TAG_CELLX}{cellWitdh}{K_TAG_CELL}";

                result = result.Replace(listCells[i], newCellString);
                result = result.Replace(listCells[i].Replace(K_TAG_CELLX, K_TAG_CLWWIDTH), newCellString1);
                result = result.Replace($"{K_TAG_CLWWIDTH}{cellSz}{K_TAG_CELL}", newCellString1);
            }


            return result;
        }

    }
}
