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
    public partial class ucEasyPDFViewer : UserControl, Interfacce.IViewUserControlMiddle
    {
        private string _PDFFileFullPath = "";
        private int _zoom = 100;
        private bool _shown = false;

        private const string PDF_LICENCE_KEY = @"PDFVW4WIN-YA3MW-NF22B-DOI1O-UQFAU-SL8Y5";

        public ucEasyPDFViewer()
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
                        this.pdfViewer.LoadFromFile(_PDFFileFullPath);
                        this.pdfViewer.SetViewerMode(Spire.PdfViewer.Forms.PdfViewerMode.PdfViewerMode.Auto);
                        this.pdfViewer.SetPageLayoutMode(Spire.PdfViewer.Forms.PageLayoutMode.SinglePageContinuous);

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

                this.ubShell.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FORMATOPDF_256);
                this.ubShell.PercImageFill = 0.75F;
                this.ubShell.ShortcutKey = Keys.P;

                this.ubPrint.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_STAMPA_256);
                this.ubPrint.PercImageFill = 0.75F;
                this.ubPrint.ShortcutKey = Keys.S;

                                Spire.License.LicenseProvider.SetLicenseKey(@"c7wp6S6Yxih4W9ajAQA1UoWUzlReczsCEQFeHsbeI5bVPJ2112gFvBA1hZsLiui6n2aKh7zYVp9tIKji+3enHE2fu5ZOYTmiImG0b30ZIL4AwWmD1iu0q5W0mfbg1GMvoo67a+wTz4Hi1lLh1XFZkhm3OAA2ausDXYIjOqt7M8bcVJn9njWD9wwhhjQgPc/Rzkzw9qd0ffnk53vS9zKjaSU6z+LLf/6809yWgDJFTgKwDFvJxceJzabvad99Upyn7MQPHYPH5kbwM8fuwCMa2x7hfWNhirAl4FCKnYH8skDGPnp7muCegK58FNpl7Dy191wFsb2WipR9gQ6CupOglIHFWK2vkziW4thQrt5YFwjV+Wmoj8bBAg24Ll4wLKSmNR4vs0T9nj1MxEaECLgcuSajDM5rZA3tUnQBm9mjKCYyR0808XjYfrhHJbhBbPIixZQRGnRcdiUqSGQn7ykZuYwrKU5CwyZcEW2qcloc/ztCdUMlXwkm+HLo+1syKj8GZF0qwO/85BZ2z7JuWvNq0Naf9a0dCGTXikeCYUXrK6tmX671DarS1EStS1RzjddOovYIum5rAf93mRapMq3NqRjr3OR0KTDH6aJBdvgAa2GIqEIkIAQkD4nY9scKuq7idqZf7wMvjkkJK5bpJsODzLThj40Y48op93In5lzFZftnvxjfZ8vtOko7pvVOCtT3J163EHlNUoFhyVjLXW33tVAN21jTiJuNJ+h9KA2Fk56FHyiyNnsU/yeDEcV1Sw0jDAwxBLRE56PqeTJnY26q6MhoHnncubG8CRCoJLrVsAzC8JsBidN6nNIsmDBUDlKqkMHWuo7gYCM4Td/rwWzEQcUg4OjMd3EwleIOc+Qe7au1XZViiwQqoGgcdy2xc+JjNCE2PBfuBJR/BlsC9B27EJ15rGJl4oRgYzFdVOKRP7i1CkQt+enEy7aw1yxG3Zdn0K0JUShGKnCF5eowz4FLcf3Q3R2AlzjtOGgkcpCWt5zy0R4B4r/dgmFSfJgR3fHJ9phL+iomm6SVEH+GzL2d6+KxItcSvsmIfC8oe7OYuT0Lurq9aePgdPFsd38BsyMnZDuR2HDvYLKibnzGzXAvrTdmebdMm2dzjI0cb4wXsl70hKwklIJnigdoxcJ0JpYacpbYec11QMOgNfZHRL7L/XeN938Ci+GCOuEfMKEAvj6tJ6coJe6P9ImIZ0Cse0yhxknmHiCcfujALhtHn2TlK0KYKB3xmR8KRbHbQKjaQfAqrNO/SjS3kAsb133V+MJZ6vW6BSaRLC7EsRfsrrtG1VKS1RnUhPiepT4v4qXNxlxTc4H9wiNXE8Za3aQo6SVHSsau5FcjFEb/IeN+DYM/SJ4BWafa4ZNBywf0hVhdl0Lx0hJSpB9QGpxQHP3Wn9gNrUq6QVedmmjtqcTqv8R+QXEqxfMPLBnfy90UDC9DhIFpbIxgYQqbXgLz+UY0r8hvtcxClGHCOFVtx6AHtgr085XxeP4mmV3v");

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
            /*
             
                NUOVO METODO DI STAMPA CHE UTILIZZA IL COMPONENTE O2S.Components.PDFRender4NET
              
            */
            try
            {
                if (this.pdfViewer.PageCount > 0)
                {

                    PDFFile filepdf = null;
                    PrintDialog objPrintD = null;
                    try
                    {
                                                filepdf = PDFFile.Open(PDFFileFullPath);
                        filepdf.SerialNumber = PDF_LICENCE_KEY;

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
            /*
             
                METODO DI STAMPA CHE UTILIZZA ACROBAT READER
              
            */
            try
            {
                if (this.pdfViewer.PageCount > 0)
                {
                    PrintAcrobatReader(this.PDFFileFullPath, printerName);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "StampaAcrobat", this.Name);
            }
        }

        [Obsolete("StampaOldViewerComponent is deprecated, please use Stampa instead.", false)]
        public void StampaOldViewerComponent()
        {
            /*
             
                VECCHIO METODO DI STAMPA CHE UTILIZZA LA STAMPA DEL COMPONENTE VIEWER, CHE PERO' DA' UN SACCO DI PROBLEMI: DA EVITARE!!!!
              
            */
            try
            {
                if (this.pdfViewer.PageCount > 0)
                {
                    this.pdfViewer.Print();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "StampaViewerComponent", this.Name);
            }
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

        #endregion

        #region EVENTI

        private void ubZoomIn_Click(object sender, EventArgs e)
        {
            if (this.pdfViewer.PageCount > 0)
            {
                int delta = 25;
                _zoom = Convert.ToInt32(this.pdfViewer.ZoomFator * 100F);
                _zoom += delta;
                this.pdfViewer.ZoomTo(_zoom);
            }

        }

        private void ubZoomOut_Click(object sender, EventArgs e)
        {
            if (this.pdfViewer.PageCount > 0)
            {
                int delta = 25;
                _zoom = Convert.ToInt32(this.pdfViewer.ZoomFator * 100F);
                _zoom -= delta;
                if (_zoom < 0) _zoom = 0;
                this.pdfViewer.ZoomTo(_zoom);
                
            }
        }

        private void ubPageWidth_Click(object sender, EventArgs e)
        {
            this.pdfViewer.SetZoom(Spire.PdfViewer.Forms.ZoomMode.FitWidth);
        }

        private void ubWholePage_Click(object sender, EventArgs e)
        {
            this.pdfViewer.SetZoom(Spire.PdfViewer.Forms.ZoomMode.FitPage);
        }

        private void ubPagUp_Click(object sender, EventArgs e)
        {
            if (this.pdfViewer.PageCount > 0 && this.pdfViewer.CurrentPageNumber > 1)
            {
                this.pdfViewer.GoToPage(this.pdfViewer.CurrentPageNumber - 1);
            }
        }

        private void ubPagDown_Click(object sender, EventArgs e)
        {
            if (this.pdfViewer.PageCount > 0 && this.pdfViewer.CurrentPageNumber < this.pdfViewer.PageCount)
            {
                this.pdfViewer.GoToPage(this.pdfViewer.CurrentPageNumber + 1);
                            }
        }

        private void ubPrint_Click(object sender, EventArgs e)
        {
            Stampa();
        }

        private void ucEasyPDFViewer_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible && !_shown)
            {
                _shown = true;
                if (this.pdfViewer.PageCount > 0)
                    this.pdfViewer.SetZoom(Spire.PdfViewer.Forms.ZoomMode.FitPage);
            }
        }

        private void ubShell_Click(object sender, EventArgs e)
        {
            if (this.PDFFileFullPath != null && this.PDFFileFullPath != string.Empty && this.PDFFileFullPath.Trim() != "" && System.IO.File.Exists(this.PDFFileFullPath))
                easyStatics.ShellExecute(this.PDFFileFullPath, "", true);
        }

        #endregion

    }
}
