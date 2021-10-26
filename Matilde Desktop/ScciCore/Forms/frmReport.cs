using System;
using System.Collections.Generic;
using System.Windows.Forms;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using ReportManager.ReportHandler;

using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.ScciCore.Common;
using UnicodeSrl.Scci.Model;
using UnicodeSrl.Scci.PluginClient;

namespace UnicodeSrl.ScciCore
{
    public partial class frmReport : frmBaseModale, Interfacce.IViewFormlModal
    {

        private Report _report = null;

        private string _docxgenerato = "";

        private PluginCaller m_caller = null;

        private string _defaultPrinter = "";


        public frmReport()
        {
                        
            InitializeComponent();
        }

        public void Carica()
        {
            try
            {
                bool bcontinua = true;
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_REPORT_16);
                bool bReportCodificato = true;

                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                                                                
                if (this.CustomParamaters == null)
                {
                                        bReportCodificato = true;
                }
                else
                {
                                        System.Type t = null;
                    t = this.CustomParamaters.GetType();
                    Type deType = typeof(DevExpress.XtraReports.UI.XtraReport);
                    bool isTypeAssignable = deType.IsAssignableFrom(t);
                    bReportCodificato = !isTypeAssignable;
                }

                if (bReportCodificato == true)
                {
                                        if (CoreStatics.CoreApplication.ReportSelezionato != null)
                    {
                                                                                                _report = CoreStatics.CoreApplication.ReportSelezionato;
                    }
                    else
                    {
                        _report = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.MascheraPartenza.MascheraPartenza.Reports.ReportSelezionato;
                    }
                    _defaultPrinter = "";

                    this.PulsanteAvantiTesto = "AVANTI";
                    this.PulsanteAvantiAbilitato = true;

                    this.ucTopModale.Titolo += @" " + _report.Descrizione;
                                        switch (_report.CodFormatoReport)
                    {
                        case Report.COD_FORMATO_REPORT_WORD:
                            bcontinua = CaricaWORD();
                            break;
                        case Report.COD_FORMATO_REPORT_REM:
                            bcontinua = CaricaREM();
                            break;
                        case Report.COD_FORMATO_REPORT_CABLATO:
                            T_CDSSPlugins oCDSSPlugins = new T_CDSSPlugins();
                            oCDSSPlugins.Codice = _report.Codice;
                            if (oCDSSPlugins.TrySelect())
                            {
                                bcontinua = CaricaDevExpress(oCDSSPlugins);
                            }
                            break;
                        case Report.COD_FORMATO_REPORT_PDF:
                            bcontinua = CaricaPDF();
                            break;
                        default:
                            break;
                    }

                                        string sTrace = @"Report: " + _report.CodFormatoReport;
                    sTrace += @"; Codice: " + _report.Codice;
                    sTrace += @"; Plugin: " + _report.NomePlugIn;
                    CoreStatics.CoreApplication.Navigazione.Maschere.TracciaNavigazione(sTrace);

                }
                else
                {
                    _report = new Report("", "", "", "", "", "", false, null, false, false, false);
                    bcontinua = CaricaDE();
                }

                if (bcontinua)
                    this.ShowDialog();
                else
                    this.Close();

                if (m_caller != null)
                    m_caller.Dispose();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
        }

        #region APERTURA

        private bool CaricaREM()
        {
            bool breportcaricato = false;
            try
            {

                this.utcReport.Tabs["tabREM"].Active = true;
                this.utcReport.Tabs["tabREM"].Selected = true;

                ReportLinkParams reportLink = _report.ReportLinkParams;
                string reportviewerURL = Database.GetConfigTable(EnumConfigTable.ReMViewer);

                                Gestore oGestore = CoreStatics.GetGestore(true);
                if (reportLink.ReportPromptParams.Count > 0)
                {
                    for (int i = 0; i < reportLink.ReportPromptParams.Count; i++)
                    {
                                                reportLink.ReportPromptParams[i].Value = DocxProcs.ValutaFormuleStringa(reportLink.ReportPromptParams[i].Value, oGestore);
                    }
                }

                if (_report.ApriBrowser)
                {
                                                                                
                                                                                                                                                                                                                            
                                                                                                                                                                                                                                                RHStatics.OpenReportInBrowser(reportviewerURL, reportLink, _report.ApriIE, System.Diagnostics.ProcessWindowStyle.Maximized, true, false);

                                                                                                                                                                                                                                                
                                        breportcaricato = false;

                }
                else
                {
                                        this.reportLinkViewer.LoadReport(reportviewerURL, reportLink);
                    breportcaricato = true;
                }
                DBUtils.storicizzaReport(_report.Codice, "", false);
            }
            catch (Exception)
            {

                throw;
            }
            return breportcaricato;
        }

        private bool CaricaDevExpress(T_CDSSPlugins oCDSSPlugins)
        {

            bool breportcaricato = false;
            PrintDialog printDialog = null;
            Dictionary<string, object> oDictionary = null;

            try
            {

                bool bcontinua = true;

                                bcontinua = CoreStatics.controlloPreliminareParametriPlugin(_report);

                if (bcontinua)
                {

                    this.utcReport.Tabs["tabDE"].Active = true;
                    this.utcReport.Tabs["tabDE"].Selected = true;
                    this.ucEasyDevExpressViewer.Carica();

                    this.PulsanteAvantiTesto = "STAMPA ED ESCI";
                    this.PulsanteAvantiAbilitato = false;

                    if (_report.NomePlugIn == null || _report.NomePlugIn.Trim() == "")
                    {
                        easyStatics.EasyMessageBox(@"Impossibile recuperare il nome del plugin!", "Stampa Plugin", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    else
                    {

                                                oDictionary = CoreStatics.getDictionaryParametriPlugin(_report);
                                                if (_report.RichiediStampante)
                        {
                            printDialog = new PrintDialog();
                            printDialog.AllowSomePages = false;
                            printDialog.AllowSelection = false;
                            printDialog.AllowCurrentPage = false;
                            if (printDialog.ShowDialog() == DialogResult.OK)
                            {
                                oDictionary.Add("PrinterSettings", printDialog.PrinterSettings);
                            }
                            else
                            {
                                bcontinua = false;
                            }
                        }

                    }

                }

                if (bcontinua)
                {

                    Plugin oPlugin = new Plugin(oCDSSPlugins.Codice, oCDSSPlugins.Descrizione, oCDSSPlugins.NomePlugin, oCDSSPlugins.Comando, _report.ParametriXML, "", 0, null);
                    object[] myparam = new object[1] { oDictionary };

                                                            Risposta oRisposta = PluginClientStatics.PluginClientMenuEsegui(oPlugin, myparam);
                    if (oRisposta.Parameters == null)
                    {

                                                switch (_report.Codice)
                        {

                            case Report.COD_REPORT_PDF_TUTTI_REFERTI:
                                easyStatics.EasyMessageBox("Nessun referto da stampare.", "Stampa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;

                            default:
                                easyStatics.EasyMessageBox("Stampa non generata correttamente.", "Stampa", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                break;

                        }
                        breportcaricato = false;

                    }
                    else
                    {

                        switch (_report.Codice)
                        {

                            case Report.COD_REPORT_PDF_TUTTI_REFERTI:
                                                                                                bool bOK = false;
                                if (oRisposta.Parameters.Length == 1 && oRisposta.Parameters[0].GetType() == typeof(string))
                                {

                                    string spdf = oRisposta.Parameters[0] as string;
                                    if (System.IO.File.Exists(spdf))
                                    {
                                        bOK = true;
                                                                                bool bCaricamentoViaShell = false;
                                        string sAR = UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.ParolaChiaveAperturaAcrobat);
                                        if (sAR != null && sAR != string.Empty && sAR != "" && _report.Descrizione.IndexOf(sAR) >= 0) bCaricamentoViaShell = true;
                                                                                easyStatics.ShellExecute(spdf, "", bCaricamentoViaShell, _report.Codice, false);
                                    }

                                }
                                if (!bOK)
                                {
                                                                        easyStatics.EasyMessageBox(@"Impossibile recuperare i referti!", "Stampa Referti", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                }
                                break;

                            case Report.COD_REPORT_RPTWKIANCI:
                                if (oRisposta.Successo && oRisposta.Parameters.Length == 1 && oRisposta.Parameters[0].GetType() == typeof(string))
                                {
                                                                        string spdf = oRisposta.Parameters[0] as string;
                                    if (System.IO.File.Exists(spdf))
                                    {
                                        easyStatics.ShellExecute(spdf, "", false, _report.Codice, false);
                                    }
                                }
                                else if (oRisposta.Parameters.Length == 1 && oRisposta.Parameters[0].GetType() == typeof(string))
                                {
                                    easyStatics.EasyMessageBox(oRisposta.Parameters[0].ToString(), "Stampa Etichette", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                }
                                break;

                            default:
                                if (oRisposta.Parameters.Length == 1 && oRisposta.Parameters[0].GetType() == typeof(string))
                                {
                                                                        easyStatics.EasyMessageBox(oRisposta.Parameters[0] as string, "Stampa", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                }
                                else
                                {

                                    this.ucEasyDevExpressViewer.ActiveReport = (DevExpress.XtraReports.UI.XtraReport)oRisposta.Parameters[0];
                                    if (_report.RichiediStampante)
                                    {
                                                                                                                                                                                                        
                                                                                this.ucEasyDevExpressViewer.ActiveReport.PrinterName = printDialog.PrinterSettings.PrinterName;

                                                                                                                    }
                                    this.PulsanteAvantiAbilitato = true;
                                    breportcaricato = true;

                                }
                                break;

                        }

                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaCAB", this.Name);
                breportcaricato = false;
            }

            return breportcaricato;

        }

        private bool CaricaWORD()
        {
            bool breportcaricato = false;
            _docxgenerato = "";
            try
            {
                bool bcontinua = true;

                                switch (_report.Codice)
                {

                    default:
                        break;
                }

                if (bcontinua)
                {

                    this.utcReport.Tabs["tabWORD"].Active = true;
                    this.utcReport.Tabs["tabWORD"].Selected = true;

                    this.PulsanteAvantiTesto = "STAMPA ED ESCI";
                    this.PulsanteAvantiAbilitato = false;

                    if (_report.Modello == null || _report.Modello.Length <= 0)
                        easyStatics.EasyMessageBox(@"Impossibile recuperare il modello!", "Stampa Word", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    else
                    {

                        Gestore oGestore = CoreStatics.GetGestore(true);

                        DocxProcs.CreaDocxReturn ret = DocxProcs.CreaReportDOCX(_report.Modello, oGestore);

                        if (ret.docxgeneratofullpath != null && ret.docxgeneratofullpath != string.Empty && ret.docxgeneratofullpath.Trim() != "" && System.IO.File.Exists(ret.docxgeneratofullpath))
                        {

                            if (ret.errori != null && ret.errori != string.Empty && ret.errori.Trim() != "")
                                easyStatics.EasyMessageBox(@"Documento Word generato con errori:" + Environment.NewLine + ret.errori, "Stampa Word", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            _docxgenerato = ret.docxgeneratofullpath;

                            bool bCaricamentoViaShell = false;

                            if (UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.ApriDOCXtramiteshell) == "1") bCaricamentoViaShell = true;
                            if (!bCaricamentoViaShell)
                            {
                                                                string sW = UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.ParolaChiaveAperturaWord);
                                if (sW != null && sW != string.Empty && sW != "" && _report.Descrizione.IndexOf(sW) >= 0) bCaricamentoViaShell = true;
                            }

                            if (bCaricamentoViaShell)
                            {
                                                                easyStatics.ShellExecute(ret.docxgeneratofullpath, "", true);
                            }
                            else
                            {
                                                                                                                                this.ucEasyDOCXViewer.Carica();
                                this.ucEasyDOCXViewer.DOCXFileFullPath = ret.docxgeneratofullpath;

                                _docxgenerato = ret.docxgeneratofullpath;
                                this.PulsanteAvantiAbilitato = true;
                                breportcaricato = true;

                            }


                                                                                                                                                                                                                                                                                                                                                                            
                        }
                        else
                            easyStatics.EasyMessageBox(@"Impossibile generare il documento word!" + Environment.NewLine + ret.errori, "Stampa Word", MessageBoxButtons.OK, MessageBoxIcon.Stop);


                    }
                }


            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaWORD", this.Name);
                breportcaricato = false;
            }
            return breportcaricato;
        }

        private bool CaricaPDF()
        {

            bool breportcaricato = false;

            try
            {

                this.utcReport.Tabs["tabDE"].Active = true;
                this.utcReport.Tabs["tabDE"].Selected = true;
                this.ucEasyDevExpressViewer.Carica();

                this.PulsanteAvantiTesto = "STAMPA ED ESCI";
                this.PulsanteAvantiAbilitato = false;

                                                                string spdf = "TMP" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + @".pdf";
                spdf = System.IO.Path.Combine(FileStatics.GetSCCITempPath() + spdf);

                if (_report.Modello != null)
                {
                    byte[] bytepdf = _report.Modello;
                    UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(spdf, ref bytepdf);
                }

                if (System.IO.File.Exists(spdf))
                {

                                        bool bCaricamentoViaShell = false;
                    string sAR = UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.ParolaChiaveAperturaAcrobat);
                    if (sAR != null && sAR != string.Empty && sAR != "" && _report.Descrizione.IndexOf(sAR) >= 0) bCaricamentoViaShell = true;

                    if (this.CustomParamaters != null && this.CustomParamaters.GetType() == typeof(string))
                    {
                        bCaricamentoViaShell = (this.CustomParamaters.ToString() == "S" ? true : false);
                    }

                                        easyStatics.ShellExecute(spdf, "", bCaricamentoViaShell, _report.Codice, false);

                }
                else
                {
                    easyStatics.EasyMessageBox(@"Impossibile recuperare il file!", "Stampa PDF", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaPDF", this.Name);
                breportcaricato = false;
            }

            return breportcaricato;

        }

        private bool CaricaDE()
        {

            bool breportcaricato = false;

            try
            {

                this.utcReport.Tabs["tabDE"].Active = true;
                this.utcReport.Tabs["tabDE"].Selected = true;
                this.ucEasyDevExpressViewer.Carica();

                this.PulsanteAvantiTesto = "STAMPA ED ESCI";
                this.PulsanteAvantiAbilitato = false;

                this.ucEasyDevExpressViewer.ActiveReport = (DevExpress.XtraReports.UI.XtraReport)this.CustomParamaters;

                this.PulsanteAvantiAbilitato = true;
                breportcaricato = true;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaDE", this.Name);
                breportcaricato = false;
            }

            return breportcaricato;

        }

        #endregion

        #region CHIUSURA

        private bool ChiusuraDevExpress(T_CDSSPlugins oCDSSPlugins)
        {
            bool bReturn = false;
            try
            {
                bool bcontinue = true;
                string pdffullpath = "";


                                if (bcontinue)
                {
                    if (_report.Codice == Report.COD_REPORT_ETICHETTA_CARTELLA
                        || _report.Codice == Report.COD_REPORT_ETICHETTA_CARTELLA_QR
                        || _report.Codice == Report.COD_REPORT_ETICHETTA_CART_INT_REP
                        || _report.Codice == Report.COD_REPORT_ETICHETTA_CART_INT_REP_QR
                        || _report.Codice == Report.COD_REPORT_ETICHETTA_ALLEGATO
                        || _report.Codice == Report.COD_REPORT_SCHEDA_ETICHETTA
                        || _report.Codice == Report.COD_REPORT_BRACCIALE)
                    {
                        if (_report.RichiediStampante)
                            bcontinue = this.ucEasyDevExpressViewer.Stampa(ucEasyDevExpressViewer.enumPrintModality.checkDirectOrActiveReportPrintDialog);
                        else
                            bcontinue = this.ucEasyDevExpressViewer.Stampa(ucEasyDevExpressViewer.enumPrintModality.useActiveReportPrintDialog);
                    }
                    else
                    {
                        if (_report.RichiediStampante)
                            bcontinue = this.ucEasyDevExpressViewer.Stampa(ucEasyDevExpressViewer.enumPrintModality.checkDirectOrWindowsPrintDialog);
                        else
                            bcontinue = this.ucEasyDevExpressViewer.Stampa(ucEasyDevExpressViewer.enumPrintModality.useWindowsPrintDialog);
                    }
                }

                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                                if (bcontinue)
                {
                    bcontinue = false;

                                                                                                                                                                                                        pdffullpath = this.ucEasyDevExpressViewer.EsportaPDF(pdffullpath);

                                        if (pdffullpath != null && pdffullpath != string.Empty && pdffullpath.Trim() != "" && System.IO.File.Exists(pdffullpath))
                    {
                        bcontinue = DBUtils.storicizzaReport(_report.Codice, pdffullpath, _report.DaStoricizzare);
                    }
                }

                                if (bcontinue)
                {
                    switch (_report.Codice)
                    {
                        case Report.COD_REPORT_CARTELLA_PAZIENTE:
                                                        
                            if (CoreStatics.CoreApplication.Cartella.PDFCartellaAggiornabile)
                            {
                                bcontinue = false;
                                if (pdffullpath == null || pdffullpath == string.Empty || pdffullpath.Trim() == "")
                                {
                                                                        
                                    pdffullpath = this.ucEasyDevExpressViewer.EsportaPDF(pdffullpath);
                                }

                                if (pdffullpath != null && pdffullpath != string.Empty && pdffullpath.Trim() != "" && System.IO.File.Exists(pdffullpath))
                                {
                                    bcontinue = CoreStatics.CoreApplication.Cartella.archiviaPDF(pdffullpath);
                                }

                            }

                            break;

                        default:
                            break;
                    }
                }

                try
                {
                    if (pdffullpath != null && pdffullpath != string.Empty && pdffullpath.Trim() != "" && System.IO.File.Exists(pdffullpath))
                        System.IO.File.Delete(pdffullpath);
                }
                catch (Exception)
                {
                }

                bReturn = bcontinue;
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ChiusuraDevExpress", this.Name);
                bReturn = false;
            }
            this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            return bReturn;
        }

        private bool ChiusuraWORD()
        {
            bool bReturn = false;
            try
            {
                bool bcontinue = true;

                                if (bcontinue)
                    bcontinue = this.ucEasyDOCXViewer.Stampa();

                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                                if (bcontinue)
                {
                    bcontinue = false;

                    if (_docxgenerato != null && _docxgenerato != string.Empty && _docxgenerato.Trim() != "" && System.IO.File.Exists(_docxgenerato))
                    {
                        bcontinue = DBUtils.storicizzaReport(_report.Codice, _docxgenerato, _report.DaStoricizzare);
                    }
                    else
                    {
                                                                        bcontinue = DBUtils.storicizzaReport(_report.Codice, "", false);
                    }
                }

                                if (bcontinue)
                {
                    switch (_report.Codice)
                    {
                        default:
                            break;
                    }
                }

                try
                {
                    if (_docxgenerato != null && _docxgenerato != string.Empty && _docxgenerato.Trim() != "" && System.IO.File.Exists(_docxgenerato))
                        System.IO.File.Delete(_docxgenerato);
                }
                catch (Exception)
                {
                }

                bReturn = bcontinue;
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ChiusuraWORD", this.Name);
                bReturn = false;
            }
            this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            return bReturn;
        }

        private void ChiudiDocumenti()
        {
            try
            {
                                                                                
                this.ucEasyDOCXViewer.DistruggiControllo();
                if (!this.ucEasyDOCXViewer.IsDisposed) this.ucEasyDOCXViewer.Dispose();
                            }
            catch
            {
            }
        }

        private bool ChiusuraDE()
        {

            bool bReturn = false;

            try
            {

                if (_report.RichiediStampante)
                {
                    bReturn = this.ucEasyDevExpressViewer.Stampa(ucEasyDevExpressViewer.enumPrintModality.checkDirectOrWindowsPrintDialog);
                }
                else
                {
                    bReturn = this.ucEasyDevExpressViewer.Stampa(ucEasyDevExpressViewer.enumPrintModality.useWindowsPrintDialog);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ChiusuraDE", this.Name);
            }

            return bReturn;

        }

        #endregion

        #region EVENTI

        private void ucEasyDOCXViewer_DocumentOpenedOnWord(object sender, EventArgs e)
        {
            try
            {
                                if (_docxgenerato != null && _docxgenerato != string.Empty && _docxgenerato.Trim() != "" && System.IO.File.Exists(_docxgenerato))
                {
                    DBUtils.storicizzaReport(_report.Codice, _docxgenerato, _report.DaStoricizzare);
                }
                else
                {
                                                            DBUtils.storicizzaReport(_report.Codice, "", false);
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
        }

        private void frmReport_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {
                bool bClose = false;

                switch (_report.CodFormatoReport)
                {
                    case Report.COD_FORMATO_REPORT_CABLATO:
                        T_CDSSPlugins oCDSSPlugins = new T_CDSSPlugins();
                        oCDSSPlugins.Codice = _report.Codice;
                        if (oCDSSPlugins.TrySelect())
                        {
                            bClose = ChiusuraDevExpress(oCDSSPlugins);
                        }
                        break;

                    case Report.COD_FORMATO_REPORT_WORD:
                        bClose = ChiusuraWORD();
                        break;

                    case Report.COD_FORMATO_REPORT_PDF:
                        bClose = true;
                        break;

                    default:
                        if (this.CustomParamaters == null)
                        {
                            bClose = true;
                        }
                        else
                        {
                            bClose = ChiusuraDE();
                        }
                        break;
                }

                if (bClose)
                {
                    ChiudiDocumenti();
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmReport_PulsanteAvantiClick", this.Name);
            }
        }

        private void frmReport_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {

            ChiudiDocumenti();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmReport_Shown(object sender, EventArgs e)
        {
            try
            {
                switch (_report.CodFormatoReport)
                {

                    case Report.COD_FORMATO_REPORT_WORD:
                        break;
                    case Report.COD_FORMATO_REPORT_REM:
                        break;
                    case Report.COD_FORMATO_REPORT_CABLATO:
                        T_CDSSPlugins oCDSSPlugins = new T_CDSSPlugins();
                        oCDSSPlugins.Codice = _report.Codice;
                        if (oCDSSPlugins.TrySelect())
                        {
                            this.ucEasyDevExpressViewer.setDocumentMap();
                        }
                        break;

                    case Report.COD_FORMATO_REPORT_PDF:
                        break;

                    default:
                        break;

                }
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
            catch (Exception)
            {
            }
        }

        private void frmReport_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                                if (_defaultPrinter != "")
                {
                                                        }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
        }

        #endregion

    }
}
