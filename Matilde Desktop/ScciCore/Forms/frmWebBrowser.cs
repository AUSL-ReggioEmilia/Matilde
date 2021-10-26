using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnicodeSrl.ScciCore
{
    public partial class frmWebBrowser : UnicodeSrl.ScciCore.frmBaseModale, Interfacce.IViewFormlModal
    {

        #region Declare

        private string url = string.Empty;

        #endregion

        public frmWebBrowser()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {
            this.InizializzaControlli();

            this.ShowDialog();
        }

        #endregion

        #region Private

        private void InizializzaControlli()
        {
            this.ubBackward.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_FRECCIASX_256);
            this.ubBackward.PercImageFill = 0.75F;
            this.ubForward.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_FRECCIADX_256);
            this.ubForward.PercImageFill = 0.75F;
            this.ubRefresh.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_AGGIORNA_256);
            this.ubRefresh.PercImageFill = 0.75F;
            this.ubStop.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_SOSPENDI_256);
            this.ubStop.PercImageFill = 0.75F;

        }

        private void LinkClicked(object sender, EventArgs e)
        {
            HtmlElement link = webBrowser.Document.ActiveElement;
            url = link.GetAttribute("href");
        }

        #endregion

        #region Events

        private void frmWebBrowser_Load(object sender, EventArgs e)
        {
            this.webBrowser.Url = CoreStatics.CoreApplication.MovLinkSelezionato.Uri;
        }

        private void frmWebBrowser_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Web Browser Control

        private void ubBackward_Click(object sender, EventArgs e)
        {
            webBrowser.GoBack();
        }

        private void ubForward_Click(object sender, EventArgs e)
        {
            webBrowser.GoForward();
        }

        private void ubRefresh_Click(object sender, EventArgs e)
        {
            webBrowser.Refresh();
        }

        private void ubStop_Click(object sender, EventArgs e)
        {
            webBrowser.Stop();
            ubStop.Enabled = false;
            ubRefresh.Enabled = true;
            pbAnim.Visible = false;
        }

        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            ubStop.Enabled = true;
            pbAnim.Visible = true;
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            ubStop.Enabled = false;
            ubBackward.Enabled = webBrowser.CanGoBack;
            ubForward.Enabled = webBrowser.CanGoForward;
            ubRefresh.Enabled = true;
            pbAnim.Visible = false;

            HtmlElementCollection links = webBrowser.Document.Links;
            foreach (HtmlElement var in links)
            {
                var.AttachEventHandler("onclick", LinkClicked);
            }
        }

        void webBrowser_NewWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            webBrowser.Navigate(url);
        }

        #endregion

    }
}
