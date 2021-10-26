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
    public partial class frmFotoPaziente : frmBaseModale, Interfacce.IViewFormlModal
    {
        private bool _shown = false;
        private bool _immagineModificata = false;

        public frmFotoPaziente()
        {
            InitializeComponent();
        }

        #region INTERFACCIA

        public void Carica()
        {
            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;

                if (CoreStatics.CoreApplication.Paziente.Sesso.ToUpper().IndexOf("F") == 0)
                    this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_PAZIENTEFEMMINA_16);
                else
                    this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_PAZIENTEMASCHIO_16);

                InizializzaControlli();
                VerificaSicurezza();

                this.ShowDialog();
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }
        }

        #endregion

        #region PRIVATE

        private void InizializzaControlli()
        {

            try
            {

                this.ucPictureSelect.ViewInit();
                                this.ucPictureSelect.ViewToolbarStyle = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2007;
                this.ucPictureSelect.ViewShowToolbar = true;
                this.ucPictureSelect.ViewShowSaveImage = true;
                this.ucPictureSelect.ViewShowRemoveImage = true;
                this.ucPictureSelect.ViewShowAddImage = false;

                this.ucPictureSelect.ViewOpenFileDialogFilter = @"Tutti i file Immagine (*.jpg;*.bmp;*.png;*.gif;*.tif)|*.jpg;*.bmp;*.png;*.gif;*.tif|JPEG (*.jpg)|*.jpg|Bitmap (*.bmp)|*.bmp|png files (*.png)|*.png|GIF (*.gif)|*.gif|TIFF (*.tif)|*.tif";
                this.ucPictureSelect.ViewSaveFileDialogFilter = @"JPEG (*.jpg)|*.jpg|Bitmap (*.bmp)|*.bmp|png files (*.png)|*.png|GIF (*.gif)|*.gif|TIFF (*.tif)|*.tif";

                this.ucPictureSelect.ViewCheckSquareImage = false;
                this.ucPictureSelect.ViewCenterImage = true;
                this.ucPictureSelect.ViewUseLargeImages = true;
               
                                this.ucPictureSelect.ViewImage = CoreStatics.CoreApplication.Paziente.Foto;
                _immagineModificata = false;
                

                this.ubCaricaDaFile.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;
                this.ubAcquisisci.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaControlli", this.Name);
            }
        }

        private void VerificaSicurezza()
        {

            try
            {
                                this.ubCaricaDaFile.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(Scci.Enums.EnumModules.Pazienti_Modifica_Foto);
                this.ubAcquisisci.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(Scci.Enums.EnumModules.Pazienti_Modifica_Foto);
            }
            catch (Exception)
            {
            }
        }

        private bool SalvaFoto()
        {
            bool bReturn = true;

            try
            {
                                                                                                                
                if (bReturn)
                {
                    if (_immagineModificata)
                    {
                                                Image oimg = null;
                        if (this.ucPictureSelect.ViewImage != null)
                        { 
                                                        oimg = this.ucPictureSelect.ViewImage;

                            if (UnicodeSrl.Scci.Statics.Database.GetConfigTable(Scci.Enums.EnumConfigTable.FotoPazienteMaxWidth) != "" && UnicodeSrl.Scci.Statics.Database.GetConfigTable(Scci.Enums.EnumConfigTable.FotoPazienteMaxHeight) != "")
                            {
                                int maxWidth = 0;
                                int maxHeight = 0;
                                int targetWidth = oimg.Width;
                                int targetHeight = oimg.Height;
                                if (int.TryParse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(Scci.Enums.EnumConfigTable.FotoPazienteMaxWidth), out maxWidth) 
                                    && int.TryParse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(Scci.Enums.EnumConfigTable.FotoPazienteMaxHeight), out maxHeight))
                                {
                                    if (targetWidth > maxWidth || targetHeight > maxHeight)
                                    { 
                                        while (targetWidth > maxWidth || targetHeight > maxHeight)
                                        {
                                            if (targetHeight > maxHeight)
                                            {
                                                float scale = (float)maxHeight / (float)targetHeight;
                                                targetHeight = maxHeight;
                                                targetWidth = Convert.ToInt32((float)targetWidth * scale);
                                            }
                                            if (targetWidth > maxWidth)
                                            {
                                                float scale = (float)maxWidth / (float)targetWidth;
                                                targetWidth = maxWidth;
                                                targetHeight = Convert.ToInt32((float)targetHeight * scale);
                                            }
                                        }

                                                                                oimg = new Bitmap(oimg, new Size(targetWidth, targetHeight));

                                    }
                                }
                            }
                        }
                        CoreStatics.CoreApplication.Paziente.Foto = oimg;
                        this.ImpostaCursore(Scci.Enums.enum_app_cursors.WaitCursor);
                        if (CoreStatics.CoreApplication.Paziente.SalvaFoto())
                            this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        else
                            bReturn = false;
                    }
                    else
                    {
                                                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    }
                }

            }
            catch (Exception ex)
            {
                bReturn = false;
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                CoreStatics.ExGest(ref ex, "SalvaFoto", this.Name);
            }
            finally
            {
                this.ImpostaCursore(Scci.Enums.enum_app_cursors.DefaultCursor);
            }

            return bReturn;
        }

        #endregion

        #region EVENTI

        private void frmFotoPaziente_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            if (SalvaFoto())
            {
                this.Close();
            }
        }

        private void frmFotoPaziente_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        } 

        private void ubCaricaDaFile_Click(object sender, EventArgs e)
        {
                        if (this.ucPictureSelect.ViewAddImage())
            {
                _immagineModificata = true;
            }
        }

        private void ubAcquisisci_Click(object sender, EventArgs e)
        {
            
            easyStatics.EasyMessageBox("Funzionalità non abilitata.", "Acquisizione da webcam", MessageBoxButtons.OK, MessageBoxIcon.Information);
           
        }

        private void frmFotoPaziente_Shown(object sender, EventArgs e)
        {
            try
            {
                if (!_shown)
                {
                    this.ucPictureSelect.ViewFitImage();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (!_shown) _shown = true;
            }
        }

        private void ucPictureSelect_PictureChange(object sender, EventArgs e)
        {
            _immagineModificata = true;
        }

        #endregion

    }
}
