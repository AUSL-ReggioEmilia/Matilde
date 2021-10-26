using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUZoomScheda : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmPUZoomScheda()
        {
            InitializeComponent();
        }

        #region INTERFACE

        public void Carica()
        {
            try
            {
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_SCHEDA_16);
                this.ucAnteprimaRtfStorico.MovScheda = null;
                this.lblTitolo.Text = "";

                if (CoreStatics.CoreApplication.MovSchedaSelezionata != null)
                {
                    string sDescrizione = CoreStatics.CoreApplication.MovSchedaSelezionata.DescrScheda;

                    if (CoreStatics.CoreApplication.MovSchedaSelezionata.Scheda.NumerositaMassima > 1)
                        sDescrizione += @" (" + CoreStatics.CoreApplication.MovSchedaSelezionata.Numero.ToString() + @")";

                    if (CoreStatics.CoreApplication.MovSchedaSelezionata.DataCreazione >= DateTime.MinValue)
                        sDescrizione += @" [" + CoreStatics.CoreApplication.MovSchedaSelezionata.DataCreazione.ToString(@"dd/MM/yyyy HH:mm") + @"]";

                    this.lblTitolo.Text = sDescrizione;

                    this.ucAnteprimaRtfStorico.MovScheda = CoreStatics.CoreApplication.MovSchedaSelezionata;
                    this.ucAnteprimaRtfStorico.RefreshRTF();
                }

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.ShowDialog();

                CoreStatics.CoreApplication.News.NotiziaSelezionata = null;
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region EVENTI

        private void ucRichTextBox_RtfLinkClicked(object sender, LinkClickedEventArgs e)
        {

            try
            {
                System.Diagnostics.Process.Start(e.LinkText);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void frmDettaglioNews_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void frmDettaglioNews_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
                        this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmPUZoomScheda_Shown(object sender, EventArgs e)
        {
            try
            {
                this.ucAnteprimaRtfStorico.RefreshRTF(CoreStatics.CoreApplication.Sessione.Computer.RtfZoom);
            }
            catch
            {
            }
        }

        #endregion

    }
}
