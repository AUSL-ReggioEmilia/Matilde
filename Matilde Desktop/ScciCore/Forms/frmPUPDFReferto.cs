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
    public partial class frmPUPDFReferto : frmBaseModale, Interfacce.IViewFormlModal
    {
        private Control _axAcroPDF = null;
        private List<string> _tempfiles = new List<string>();
        public frmPUPDFReferto()
        {
            InitializeComponent();
        }

        #region INTERFACCIA
        
        public void Carica()
        {
            try
            {
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_EVIDENZACLINICA_16);

                CaricaPDF();

                this.ShowDialog();
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }
        } 

        #endregion

        #region PRIVATE

        private void CaricaPDF()
        {
            try
            {
                if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato != null && CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.PDF != null)
                {
                    string sreftemp = System.IO.Path.Combine(Scci.Statics.FileStatics.GetSCCITempPath(), "referto" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + @".pdf");
                    byte[] pdf = CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.PDF;
                    if (UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(sreftemp, ref pdf))
                    {
                        if (System.IO.File.Exists(sreftemp))
                        {
                            _tempfiles.Add(sreftemp);
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaPDF", this.Name);
            }
        }


        #endregion

        #region EVENTI

        private void frmPUPDFReferto_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmPUPDFReferto_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        } 

        private void frmPUPDFReferto_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_tempfiles != null && _tempfiles.Count > 0)
                {
                    for (int i = 0; i < _tempfiles.Count; i++)
                    {
                        try
                        {
                            if (System.IO.File.Exists(_tempfiles[i])) System.IO.File.Delete(_tempfiles[i]);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

    }
}
