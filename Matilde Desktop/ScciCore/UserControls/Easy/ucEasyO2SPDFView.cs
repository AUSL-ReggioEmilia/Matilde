using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore;
using System.Diagnostics;
using System.Drawing.Printing;
using O2S.Components.PDFRender4NET;
using O2S.Components.PDFRender4NET.Printing;

namespace UnicodeSrl.ScciCore
{
    public partial class ucEasyO2SPDFView : UserControl, Interfacce.IViewUserControlMiddle
    {
        private string _PDFFileFullPath = "";
        private int _zoom = 100;
        private bool _shown = false;

        private easyStatics.DocumentBookmarkVisibility _bookmarksVisibility = easyStatics.DocumentBookmarkVisibility.auto;

        public ucEasyO2SPDFView()
        {
            InitializeComponent();

        }

        #region PROPRIETA'

        public string PDFFileFullPath
        {
            get
            {
                return _PDFFileFullPath;
            }
            set
            {
                _PDFFileFullPath = value;
                if (_PDFFileFullPath != null && _PDFFileFullPath != string.Empty && _PDFFileFullPath.Trim() != "" && System.IO.File.Exists(_PDFFileFullPath))
                {
                    try
                    {
                        ChiudiDocumento();

                        this.pdfViewer.Document = new O2S.Components.PDFView4NET.PDFDocument();
                        this.pdfViewer.Document.SerialNumber = CoreStatics.C_PDF_LICENCE_KEY;
                        this.pdfViewer.Document.Load(_PDFFileFullPath);

                        this.pdfViewer.PageDisplayLayout = O2S.Components.PDFView4NET.PDFPageDisplayLayout.SinglePage;
                        this.pdfViewer.ZoomMode = O2S.Components.PDFView4NET.PDFZoomMode.FitVisible;

                        setBookmarkVisible(_bookmarksVisibility);

                    }
                    catch
                    {
                        ChiudiDocumento();
                    }

                }
                else
                    ChiudiDocumento();
            }
        }

        public bool ShowPrint
        {
            get
            {
                return this.ubPrint.Visible;
            }
            set
            {
                this.ubPrint.Visible = value;
            }
        }

        public easyStatics.DocumentBookmarkVisibility BookmarksVisible
        {
            get
            {
                return _bookmarksVisibility;
            }
            set
            {
                _bookmarksVisibility = value;
                setBookmarkVisible(_bookmarksVisibility);
            }
        }

        #region PULSANTE CUSTOM

        public event EventHandler CustomActionClick;

        public bool ShowCustomAction
        {
            get
            {
                return this.ubCustomAction.Visible;
            }
            set
            {
                this.ubCustomAction.Visible = value;
            }
        }

        public bool CustomActionEnabled
        {
            get
            {
                return this.ubCustomAction.Enabled;
            }
            set
            {
                this.ubCustomAction.Enabled = value;
            }
        }

        public Keys CustomActionShortcutKey
        {
            get
            {
                return this.ubCustomAction.ShortcutKey;
            }
            set
            {
                this.ubCustomAction.ShortcutKey = value;
            }
        }

        public object CustomActionImage
        {
            get
            {
                return this.ubCustomAction.Appearance.Image;
            }
            set
            {
                this.ubCustomAction.Appearance.Image = value;
                if (value != null)
                {
                    this.ubCustomAction.PercImageFill = 0.75F;
                }
            }
        }

        #endregion

        #endregion

        #region INTERFACCIA

        public void Aggiorna()
        {
            this.pdfViewer.Refresh();
        }

        public void Carica()
        {
            try
            {

                this.ubZoomIn.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_ZOOMIN_256);
                this.ubZoomIn.PercImageFill = 0.75F;
                this.ubZoomIn.ShortcutKey = Keys.Add;

                this.ubZoomOut.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_ZOOMOUT_256);
                this.ubZoomOut.PercImageFill = 0.75F;
                this.ubZoomOut.ShortcutKey = Keys.Subtract;

                this.ubWholePage.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FIT_PAGE_256);
                this.ubWholePage.PercImageFill = 0.75F;
                this.ubWholePage.ShortcutKey = Keys.T;

                this.ubPageWidth.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FIT_H_256);
                this.ubPageWidth.PercImageFill = 0.75F;
                this.ubPageWidth.ShortcutKey = Keys.L;

                this.ubPagUp.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_PAGSU_256);
                this.ubPagUp.PercImageFill = 0.75F;
                this.ubPagUp.ShortcutKey = Keys.PageUp;

                this.ubPagDown.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_PAGGIU_256);
                this.ubPagDown.PercImageFill = 0.75F;
                this.ubPagDown.ShortcutKey = Keys.PageDown;

                this.ubShell.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FORMATOPDF_256);
                this.ubShell.PercImageFill = 0.75F;
                this.ubShell.ShortcutKey = Keys.P;

                this.ubPrint.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_STAMPA_256);
                this.ubPrint.PercImageFill = 0.75F;
                this.ubPrint.ShortcutKey = Keys.S;

                this.uchkBookmark.Visible = false;
                this.uchkBookmark.UNCheckedImage = Properties.Resources.sidebar_48;
                this.uchkBookmark.CheckedImage = Properties.Resources.sidebar_48;
                this.uchkBookmark.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.uchkBookmark.Checked = false;
                this.uchkBookmark.PercImageFill = 0.75F;
                this.uchkBookmark.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;

                this.pdfBookmarksView.Font = new Font(this.pdfBookmarksView.Font.FontFamily.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small));

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }
        }

        public void Ferma()
        {

            try
            {

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        public void Stampa()
        {
            try
            {
                if (this.pdfViewer.Document != null && this.pdfViewer.Document.PageCount > 0)
                {

                    PDFFile filepdf = null;
                    PrintDialog objPrintD = null;
                    try
                    {
                        filepdf = PDFFile.Open(PDFFileFullPath);
                        filepdf.SerialNumber = CoreStatics.C_PDF_LICENCE_KEY;

                        objPrintD = new PrintDialog();
                        objPrintD.ShowHelp = false;
                        objPrintD.AllowCurrentPage = false;
                        objPrintD.AllowSelection = false;
                        objPrintD.AllowSomePages = true;
                        objPrintD.PrinterSettings.MaximumPage = filepdf.PageCount;
                        objPrintD.PrinterSettings.FromPage = 1;
                        objPrintD.PrinterSettings.ToPage = filepdf.PageCount;

                        if (objPrintD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {

                            PrinterSettings settings = objPrintD.PrinterSettings;

                            PDFPrintSettings pdfPrintSettings = new PDFPrintSettings(settings);
                            pdfPrintSettings.PageScaling = PageScaling.FitToPrinterMargins;


                            filepdf.Print(pdfPrintSettings);
                        }

                    }
                    catch (Exception ex)
                    {
                        CoreStatics.ExGest(ref ex, @"Stampa-1", this.Name);
                    }
                    finally
                    {
                        if (filepdf != null) filepdf.Dispose();
                        if (objPrintD != null) objPrintD.Dispose();
                    }


                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, @"Stampa-0", this.Name);
            }
        }

        public void StampaAcrobat(string printerName)
        {
            try
            {
                if (this.pdfViewer.Document != null && this.pdfViewer.Document.PageCount > 0)
                {
                    PrintAcrobatReader(this.PDFFileFullPath, printerName);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "StampaAcrobat", this.Name);
            }
        }

        public void ChiudiDocumento()
        {
            try
            {
                if (this.pdfViewer.Document != null && this.pdfViewer.Document.PageCount > 0)
                {
                    this.pdfViewer.Document.Close();
                    this.pdfViewer.Document.Dispose();
                    this.pdfViewer.Document = null;
                }
            }
            catch
            {
            }
        }

        public void DistruggiControllo()
        {
            try
            {
                ChiudiDocumento();
                if (!this.pdfViewer.IsDisposed) this.pdfViewer.Dispose();
            }
            catch
            {
            }
        }

        #endregion

        #region PRIVATE

        private void PrintAcrobatReader(string sTempFile, string sStampante)
        {
            try
            {
                Process MyProcess = new Process();
                MyProcess.StartInfo.FileName = GetPath_Adobe_Acrobat();
                MyProcess.StartInfo.Arguments = @"/p /t " + @"""" + sTempFile + @"""" + @" " + @"""" + sStampante + @"""";
                MyProcess.StartInfo.UseShellExecute = false;
                MyProcess.StartInfo.CreateNoWindow = true;
                MyProcess.StartInfo.Verb = @"print";
                MyProcess.Start();
                MyProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                if (!MyProcess.HasExited)
                {
                    MyProcess.WaitForExit(10000);
                    MyProcess.Kill();
                }
                MyProcess.EnableRaisingEvents = true;
                MyProcess.CloseMainWindow();
                MyProcess.Close();
            }
            catch
            {
            }
        }

        private string GetPath_Adobe_Acrobat()
        {
            try
            {
                Microsoft.Win32.RegistryKey RegKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("SOFTWARE").OpenSubKey("Adobe").OpenSubKey("Acrobat").OpenSubKey("Exe");
                return RegKey.GetValue(RegKey.GetValueNames()[0]).ToString();
            }
            catch
            {
                return "";
            }
        }

        private void setBookmarkVisible(easyStatics.DocumentBookmarkVisibility visibility)
        {
            try
            {
                bool bVisibile = false;

                switch (visibility)
                {
                    case easyStatics.DocumentBookmarkVisibility.auto:
                        if (this.pdfViewer.Document != null && this.pdfViewer.Document.Bookmarks.Count > 0) bVisibile = true;

                        break;
                    case easyStatics.DocumentBookmarkVisibility.visible:
                        bVisibile = true;
                        break;
                    case easyStatics.DocumentBookmarkVisibility.hidden:
                    default:
                        bVisibile = false;
                        break;
                }


                this.uchkBookmark.Visible = bVisibile;
                this.uchkBookmark.Checked = bVisibile;

                if (bVisibile)
                {

                    this.splitContainerPDF.Panel1Collapsed = false;

                    if (this.pdfViewer.Document == null)
                        this.pdfBookmarksView.Document = null;
                    else
                    {
                        if (this.pdfBookmarksView.Document == null || this.pdfBookmarksView.Document != this.pdfViewer.Document)
                            this.pdfBookmarksView.Document = this.pdfViewer.Document;
                    }
                }
                else
                {

                    this.splitContainerPDF.Panel1Collapsed = true;
                    this.splitContainerPDF.SplitterDistance = 0;
                }
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        #endregion

        #region EVENTI

        private void ubZoomIn_Click(object sender, EventArgs e)
        {
            if (this.pdfViewer.Document != null && this.pdfViewer.Document.PageCount > 0)
            {
                this.pdfViewer.ZoomMode = O2S.Components.PDFView4NET.PDFZoomMode.Custom;
                int delta = 25;
                _zoom = Convert.ToInt32(this.pdfViewer.Zoom);
                _zoom += delta;
                this.pdfViewer.Zoom = _zoom;
            }

        }

        private void ubZoomOut_Click(object sender, EventArgs e)
        {
            if (this.pdfViewer.Document != null && this.pdfViewer.Document.PageCount > 0)
            {
                this.pdfViewer.ZoomMode = O2S.Components.PDFView4NET.PDFZoomMode.Custom;
                int delta = 25;
                _zoom = Convert.ToInt32(this.pdfViewer.Zoom);
                _zoom -= delta;
                if (_zoom < 0) _zoom = 0;
                this.pdfViewer.Zoom = _zoom;
            }
        }

        private void ubPageWidth_Click(object sender, EventArgs e)
        {
            this.pdfViewer.ZoomMode = O2S.Components.PDFView4NET.PDFZoomMode.FitWidth;
        }

        private void ubWholePage_Click(object sender, EventArgs e)
        {
            this.pdfViewer.ZoomMode = O2S.Components.PDFView4NET.PDFZoomMode.FitVisible;
        }

        private void ubPagUp_Click(object sender, EventArgs e)
        {
            if (this.pdfViewer.Document != null && this.pdfViewer.Document.PageCount > 0 && this.pdfViewer.PageNumber > 0)
            {
                this.pdfViewer.PageNumber -= 1;
            }
        }

        private void ubPagDown_Click(object sender, EventArgs e)
        {


            if (this.pdfViewer.Document != null && this.pdfViewer.Document.PageCount > 0 && this.pdfViewer.PageNumber < this.pdfViewer.Document.PageCount - 1)
            {
                this.pdfViewer.PageNumber += 1;
            }
        }

        private void ubPrint_Click(object sender, EventArgs e)
        {
            Stampa();
        }

        private void ucEasyO2SPDFView_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible && !_shown)
            {
                _shown = true;


                if (this.pdfViewer.Document != null && this.pdfViewer.Document.PageCount > 0)
                    this.pdfViewer.ZoomMode = O2S.Components.PDFView4NET.PDFZoomMode.FitVisible;
            }
        }

        private void ubShell_Click(object sender, EventArgs e)
        {
            if (this.PDFFileFullPath != null && this.PDFFileFullPath != string.Empty && this.PDFFileFullPath.Trim() != "" && System.IO.File.Exists(this.PDFFileFullPath))
                easyStatics.ShellExecute(this.PDFFileFullPath, "", true);
        }

        private void uchkBookmark_Click(object sender, EventArgs e)
        {
            try
            {

                this.splitContainerPDF.Panel1Collapsed = !this.uchkBookmark.Checked;
            }
            catch
            {
            }
        }

        private void ubCustomAction_Click(object sender, EventArgs e)
        {
            if (this.CustomActionClick != null)
                this.CustomActionClick(this, new EventArgs());
        }

        #endregion

    }
}
