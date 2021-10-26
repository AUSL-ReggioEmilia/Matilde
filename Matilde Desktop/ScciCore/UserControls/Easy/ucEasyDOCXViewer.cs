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

namespace UnicodeSrl.ScciCore
{
    public partial class ucEasyDOCXViewer : UserControl, Interfacce.IViewUserControlMiddle
    {
        private string _DOCXFileFullPath = "";
        private int _zoom = 95;
        private bool _shown = false;

        public event EventHandler DocumentOpenedOnWord;

        public ucEasyDOCXViewer()
        {
            InitializeComponent();

        }

        #region PROPRIETA'

        public string DOCXFileFullPath
        {
            get
            {
                return _DOCXFileFullPath;
            }
            set
            {
                _DOCXFileFullPath = value;
                if (_DOCXFileFullPath != null && _DOCXFileFullPath != string.Empty && _DOCXFileFullPath.Trim() != "" && System.IO.File.Exists(_DOCXFileFullPath))
                {
                    try
                    {
                        this.pdfViewer.LoadDocument(easyStatics.getPathDocumentDE(_DOCXFileFullPath));
                        this.pdfViewer.ZoomMode = DevExpress.XtraPdfViewer.PdfZoomMode.PageLevel;
                        this.pdfViewer.NavigationPanePageVisibility = DevExpress.XtraPdfViewer.PdfNavigationPanePageVisibility.Thumbnails;
                        _zoom = 50;
                    }
                    catch
                    {
                        if (this.pdfViewer.PageCount > 0) this.pdfViewer.CloseDocument();
                    }

                }
                else
                    if (this.pdfViewer.PageCount > 0) this.pdfViewer.CloseDocument();
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

                this.ubShell.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_WORD);
                this.ubShell.PercImageFill = 0.75F;
                this.ubShell.ShortcutKey = Keys.W;

                this.ubPrint.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_STAMPA_256);
                this.ubPrint.PercImageFill = 0.75F;
                this.ubPrint.ShortcutKey = Keys.S;


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

        public bool Stampa()
        {
            bool bReturn = false;
            try
            {
                if (this.pdfViewer.PageCount > 0)
                {
                    this.pdfViewer.Print();
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Stampa", this.Name);
            }
            return bReturn;
        }

        public void ChiudiDocumento()
        {
            try
            {
                if (this.pdfViewer.PageCount > 0) this.pdfViewer.CloseDocument();
            }
            catch
            {
            }
        }

        public void DistruggiControllo()
        {
            try
            {

                if (this.pdfViewer != null)
                {
                    if ((this.pdfViewer.IsDisposed == false) && ((this.pdfViewer.Disposing == false)))
                    {
                        ChiudiDocumento();
                    }
                }
            }
            catch
            {
            }
        }

        #endregion

        #region EVENTI

        private void ubZoomIn_Click(object sender, EventArgs e)
        {
            if (this.pdfViewer.PageCount > 0)
            {
                int delta = 10;

                _zoom += delta;
                this.pdfViewer.ZoomFactor = _zoom;
            }

        }

        private void ubZoomOut_Click(object sender, EventArgs e)
        {
            if (this.pdfViewer.PageCount > 0)
            {
                int delta = 10;
                _zoom -= delta;
                if (_zoom < 0) _zoom = 0;
                this.pdfViewer.ZoomFactor = _zoom;

            }
        }

        private void ubPageWidth_Click(object sender, EventArgs e)
        {
            if (this.pdfViewer.PageCount > 0)
                this.pdfViewer.ZoomMode = DevExpress.XtraPdfViewer.PdfZoomMode.FitToWidth;
        }

        private void ubWholePage_Click(object sender, EventArgs e)
        {
            if (this.pdfViewer.PageCount > 0)
                this.pdfViewer.ZoomMode = DevExpress.XtraPdfViewer.PdfZoomMode.PageLevel;
        }

        private void ubPagUp_Click(object sender, EventArgs e)
        {
            if (this.pdfViewer.PageCount > 0 && this.pdfViewer.CurrentPageNumber > 1)
            {
                this.pdfViewer.CurrentPageNumber = this.pdfViewer.CurrentPageNumber - 1;
            }
        }

        private void ubPagDown_Click(object sender, EventArgs e)
        {
            if (this.pdfViewer.PageCount > 0 && this.pdfViewer.CurrentPageNumber < this.pdfViewer.PageCount)
            {
                this.pdfViewer.CurrentPageNumber = this.pdfViewer.CurrentPageNumber + 1;
            }
        }

        private void ubPrint_Click(object sender, EventArgs e)
        {
            Stampa();
        }

        private void ucEasyDOCXViewer_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible && !_shown)
            {
                _shown = true;
                if (this.pdfViewer.PageCount > 0)
                    this.pdfViewer.ZoomMode = DevExpress.XtraPdfViewer.PdfZoomMode.PageLevel;
            }
        }

        private void docDocumentViewer_ZoomChanged(object sender, int zoomFactor)
        {
            _zoom = zoomFactor;
        }

        private void ubShell_Click(object sender, EventArgs e)
        {
            if (this.DOCXFileFullPath != null && this.DOCXFileFullPath != string.Empty && this.DOCXFileFullPath.Trim() != "" && System.IO.File.Exists(this.DOCXFileFullPath))
            {
                easyStatics.ShellExecute(this.DOCXFileFullPath, "");

                if (this.DocumentOpenedOnWord != null)
                    this.DocumentOpenedOnWord(new object(), new EventArgs());
            }
        }

        #endregion

    }
}
