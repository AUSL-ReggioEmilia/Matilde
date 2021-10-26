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
    public partial class frmPDFViewer : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmPDFViewer()
        {
            InitializeComponent();
        }

        #region DECLARE
                public bool Stampa = false;

        private bool _abilitaVisto = false;

        #endregion

        #region INTERFACCIA

        public string pdfFullPath { get; set; }

        public void Carica()
        {
                        try
            {
                bool bcontinua = true;
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_REPORT_16);

                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                this.PulsanteAvantiTesto = "STAMPA";
                this.PulsanteAvantiAbilitato = true;

                this.ucTopModale.Titolo = this.Text;

                if (this.pdfFullPath != null && this.pdfFullPath != string.Empty && this.pdfFullPath.Trim() != "" && System.IO.File.Exists(this.pdfFullPath.Trim()))
                {
                                        bcontinua = true;

                    this.ucEasyO2SPDFView.Carica();
                    this.ucEasyO2SPDFView.PDFFileFullPath = this.pdfFullPath;


                }
                else
                {
                    bcontinua = false;
                    easyStatics.EasyErrorMessageBox(@"Impossibile recuperare il documento!", "Apertura documento", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                }

                this.ImpostaCursore(enum_app_cursors.DefaultCursor);

                if (bcontinua)
                    this.ShowDialog();
                else
                    this.Close();

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

                                                public void AbilitaVisto(Keys shortcutKeyVisto, object buttonVistoImage)
        {
            _abilitaVisto = true;
            this.ucEasyO2SPDFView.ShowCustomAction = true;
            this.ucEasyO2SPDFView.CustomActionEnabled = true;
            this.ucEasyO2SPDFView.CustomActionShortcutKey = shortcutKeyVisto;
            this.ucEasyO2SPDFView.CustomActionImage = buttonVistoImage;

        }

        #endregion

        #region EVENTI

        private void ucEasyO2SPDFView_CustomActionClick(object sender, EventArgs e)
        {
            try
            {
                if (_abilitaVisto)
                {
                                        if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato == null)
                    {
                        easyStatics.EasyMessageBox(@"Impossibile vistare il referto: parametri mancanti.", "Evidenza Clinica", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    else
                    {
                                                                                                if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.Vista())
                        {
                            this.ucEasyO2SPDFView.CustomActionEnabled = false;
                        }
                                            }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyO2SPDFView_CustomActionClick", this.Text);
            }
        }

        private void frmPDFViewer_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {
                                this.ucEasyO2SPDFView.Stampa();

                this.Stampa = true;

                                                                                                                                
                                

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "frmPDFViewer_PulsanteAvantiClick", this.Text);
            }
        }

        private void frmPDFViewer_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.ucEasyO2SPDFView.DistruggiControllo();
            if (!this.ucEasyO2SPDFView.IsDisposed) this.ucEasyO2SPDFView.Dispose();

            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
