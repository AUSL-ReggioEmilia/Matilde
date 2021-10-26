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
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;

using UnicodeSrl.Scci.DataContracts;

namespace UnicodeSrl.ScciCore
{
    public partial class frmRicercaSAC : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmRicercaSAC()
        {
            InitializeComponent();
        }

        #region INTERFACCIA

        public void Carica()
        {
            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_PAZIENTI_32);

                this.ucRicercaSAC.Carica();

                this.ShowDialog();

            }
            catch (Exception)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
        }

        #endregion

        #region EVENTI

        private void frmRicercaSAC_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmRicercaSAC_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {

            try
            {

                PazienteSac oPazienteSac = this.ucRicercaSAC.RigaPazienteSelezionato;
                if (oPazienteSac != null)
                {
                    CoreStatics.CoreApplication.Paziente = new Paziente(oPazienteSac, CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata, CoreStatics.CoreApplication.AmbulatorialeUADescrizioneSelezionata);
                    CoreStatics.CoreApplication.Episodio = null;
                    CoreStatics.CoreApplication.Trasferimento = null;
                    CoreStatics.CoreApplication.Cartella = null;

                    CoreStatics.CoreApplication.Paziente.AggiungiRecenti();

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }


        }

        #endregion

    }
}
