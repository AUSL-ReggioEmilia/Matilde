using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicodeSrl.ScciResource;
using DevExpress.XtraPrinting.Native.WinControls;
using DevExpress.XtraBars.Docking;
using UnicodeSrl.Scci.Statics;
using DevExpress.XtraReports.UI;

namespace UnicodeSrl.ScciCore
{
    public partial class ucEasyDevExpressViewer : UserControl, Interfacce.IViewUserControlMiddle
    {

        #region Declare

        private DevExpress.XtraReports.UI.XtraReport _ActiveReport = null;

        public enum enumPrintModality
        {
            useWindowsPrintDialog = 0,

            useActiveReportPrintDialog = 1,

            checkDirectOrActiveReportPrintDialog = 2,

            checkDirectOrWindowsPrintDialog = 3,


            directPrint = 4

        }

        #endregion

        public ucEasyDevExpressViewer()
        {
            InitializeComponent();
            PrintButtonDefaultModality = enumPrintModality.useActiveReportPrintDialog;
        }

        #region Interface

        public void Aggiorna()
        {
            this.Viewer.Refresh();
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

                this.ubPrint.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_REPORT_256);
                this.ubPrint.PercImageFill = 0.75F;
                this.ubPrint.ShortcutKey = Keys.S;

                this.uchkBookmark.Visible = false;
                this.uchkBookmark.UNCheckedImage = Properties.Resources.sidebar_48;
                this.uchkBookmark.CheckedImage = Properties.Resources.sidebar_48;
                this.uchkBookmark.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.uchkBookmark.Checked = false;
                this.uchkBookmark.PercImageFill = 0.75F;
                this.uchkBookmark.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;

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

        #endregion

        #region Properties

        public DevExpress.XtraReports.UI.XtraReport ActiveReport
        {
            get
            {
                return _ActiveReport;
            }
            set
            {
                _ActiveReport = value;
                if (_ActiveReport != null)
                {
                    try
                    {
                        this.Viewer.DocumentSource = _ActiveReport;
                        this.Viewer.Font = new Font(this.Viewer.Font.FontFamily.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small));
                    }
                    catch
                    {
                        this.Viewer.Document = null;
                    }
                    this.Viewer.Refresh();
                    this.Viewer.Zoom = (float)1;

                    if (this.Viewer.DocumentHasBookmarks)
                    {
                        this.uchkBookmark.Visible = true;
                        this.uchkBookmark.Checked = true;
                        this.Viewer.SetDocumentMapVisibility(true);



                    }
                }
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

        public enumPrintModality PrintButtonDefaultModality { get; set; }

        #endregion

        #region Method

        public bool Stampa(enumPrintModality printModality)
        {
            PrintDialog printDialog = null;
            ReportPrintTool printTool = new ReportPrintTool(this.ActiveReport);
            printTool.PrinterSettings.PrinterName = this.ActiveReport.PrinterName;

            try
            {
                switch (printModality)
                {
                    case enumPrintModality.useActiveReportPrintDialog:
                        return printTool.PrintDialog().Value;

                    case enumPrintModality.directPrint:
                        printTool.Print();
                        return true;


                    case enumPrintModality.checkDirectOrActiveReportPrintDialog:
                        if (this.ActiveReport.Pages.Count > 1)
                        {
                            return printTool.PrintDialog().Value;
                        }
                        else
                        {
                            printTool.Print();
                            return true;
                        }

                    case enumPrintModality.checkDirectOrWindowsPrintDialog:
                        if (this.ActiveReport.Pages.Count > 1)
                        {
                            return printTool.PrintDialog().Value;
                        }
                        else
                        {
                            printTool.Print();
                            return true;
                        }

                    case enumPrintModality.useWindowsPrintDialog:
                    default:
                        return printTool.PrintDialog().Value;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Stampa", this.Name);
                return false;
            }
            finally
            {
                if (printDialog != null) printDialog.Dispose();
            }
        }

        public string EsportaPDF()
        {
            return EsportaPDF("");
        }
        public string EsportaPDF(string exportPDFfullpath)
        {

            string pdffilepath = "";

            try
            {
                if (this.ActiveReport != null)
                {
                    pdffilepath = exportPDFfullpath;

                    if (pdffilepath == null || pdffilepath == string.Empty || pdffilepath.Trim() == "")
                    {
                        pdffilepath = "CC" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + @".pdf";
                        pdffilepath = System.IO.Path.Combine(FileStatics.GetSCCITempPath() + pdffilepath);
                    }

                    this.ActiveReport.ExportToPdf(pdffilepath);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "EsportaPDF", this.Name);
                pdffilepath = "";
            }
            return pdffilepath;
        }

        public void setDocumentMap()
        {

            try
            {

                if (this.Viewer.DocumentHasBookmarks)
                {
                    DockPanel dockpanel = this.Viewer.DockManager.Panels[0] as DockPanel;
                    dockpanel.Options.AllowFloating = false;
                    dockpanel.Options.FloatOnDblClick = false;
                    dockpanel.Options.ShowCloseButton = false;
                    dockpanel.Options.ShowAutoHideButton = false;
                    dockpanel.Text = "";
                    dockpanel.Width = this.Viewer.Size.Width / 4;

                    BookmarkTreeView bookmarktreeview = this.Viewer.DockManager.Panels[0].Controls[0].Controls[0] as BookmarkTreeView;
                    bookmarktreeview.Appearance.Row.Font = new Font(this.Viewer.Font.FontFamily.Name, easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small));
                    bookmarktreeview.OptionsView.ShowRoot = false;
                }

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Events

        private void ubZoomIn_Click(object sender, EventArgs e)
        {
            this.Viewer.ZoomIn();
        }

        private void ubZoomOut_Click(object sender, EventArgs e)
        {
            this.Viewer.ZoomOut();
        }

        private void ubPageWidth_Click(object sender, EventArgs e)
        {
            this.Viewer.Zoom = (float)-1;
        }

        private void ubWholePage_Click(object sender, EventArgs e)
        {
            this.Viewer.ViewWholePage();
        }

        private void ubPagUp_Click(object sender, EventArgs e)
        {
            this.Viewer.SelectPrevPage();
        }

        private void ubPagDown_Click(object sender, EventArgs e)
        {
            this.Viewer.SelectNextPage();
        }

        private void ubPrint_Click(object sender, EventArgs e)
        {
            Stampa(PrintButtonDefaultModality);
        }

        private void uchkBookmark_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.uchkBookmark.Checked)
                {
                    if (this.Viewer.DocumentMapVisible == false)
                    {
                        this.Viewer.SetDocumentMapVisibility(true);
                    }
                }
                else
                {
                    if (this.Viewer.DocumentMapVisible)
                    {
                        this.Viewer.SetDocumentMapVisibility(false);
                    }
                }
            }
            catch
            {
            }
        }

        #endregion

    }
}
