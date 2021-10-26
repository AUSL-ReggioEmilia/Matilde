using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPULettaraDimissione : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmPULettaraDimissione()
        {
            InitializeComponent();
        }

        #region INTERFACCIA

        public void Carica()
        {
            try
            {
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_LETTERA_DIMISSIONE_16);


                                this.webBrowser.Visible = true;
                string url = (CoreStatics.getTipoAzienda() == EnumTipoAzienda.ASMN ? Database.GetConfigTable(EnumConfigTable.LetteraDimissioneURL) : Database.GetConfigTable(EnumConfigTable.LetteraDimissioneURLAUSL));


                                if (CoreStatics.CoreApplication.Episodio != null && CoreStatics.CoreApplication.Episodio.NumeroEpisodio != string.Empty)
                    url += "?numeroepisodio=" + CoreStatics.CoreApplication.Episodio.NumeroEpisodio.ToString();

                this.webBrowser.Navigate(url);

                this.ShowDialog();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }
        }

        #endregion

        #region EVENTI

        private void frmPULettaraDimissione_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void frmPULettaraDimissione_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
