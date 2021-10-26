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

namespace UnicodeSrl.ScciCore
{
    public partial class frmCartellaChiusa : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmCartellaChiusa()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_CARTELLACLINICA_16);

                this.ucEasyPictureBox1.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_FORMATOPDF_256);
                this.ucEasyPictureBox2.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_REPORT_256);
                this.ucEasyPictureBox3.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_REPORT_256);
                this.ucEasyPictureBox4.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_REPORT_256);

                this.ShowDialog();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }

        }

        #endregion

        #region Events

        private void ucEasyButton1_Click(object sender, EventArgs e)
        {
            CoreStatics.apriPDFCartella(CoreStatics.CoreApplication.Cartella);
        }

        private void ucEasyButton2_Click(object sender, EventArgs e)
        {
            CoreStatics.apriPDFCartella(CoreStatics.CoreApplication.Cartella);
        }

        private void ucEasyButton3_Click(object sender, EventArgs e)
        {
                        try
            {
                this.ImpostaCursore(enum_app_cursors.WaitCursor);
                CoreStatics.apriPDFRefertiCartella(CoreStatics.CoreApplication.Episodio, null);

                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
            catch (Exception ex)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                CoreStatics.ExGest(ref ex, "ucEasyButton3_Click", this.Name);
            }
        }

        private void ucEasyButton4_Click(object sender, EventArgs e)
        {
                        try
            {
                this.ImpostaCursore(enum_app_cursors.WaitCursor);
                CoreStatics.apriPDFRefertiCartella(CoreStatics.CoreApplication.Episodio, CoreStatics.CoreApplication.Cartella);

                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
            catch (Exception ex)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                CoreStatics.ExGest(ref ex, "ucEasyButton3_Click", this.Name);
            }

        }

        private void frmCartellaChiusa_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmCartellaChiusa_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        #endregion

    }
}
