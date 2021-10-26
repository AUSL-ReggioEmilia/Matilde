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
    public partial class frmImmagine : frmBaseModale, Interfacce.IViewFormlModal
    {

        #region Declare

        private bool _shown = false;

        #endregion

        public frmImmagine()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;

                InizializzaControlli();

                this.ShowDialog();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }

        }

        #endregion

        #region SubRoutine

        private void InizializzaControlli()
        {

            try
            {

                this.ucPictureSelect.ViewInit();
                                this.ucPictureSelect.ViewToolbarStyle = Infragistics.Win.UltraWinToolbars.ToolbarStyle.Office2007;
                this.ucPictureSelect.ViewShowToolbar = true;
                this.ucPictureSelect.ViewShowSaveImage = true;
                this.ucPictureSelect.ViewShowRemoveImage = false;
                this.ucPictureSelect.ViewShowAddImage = false;

                this.ucPictureSelect.ViewOpenFileDialogFilter = @"Tutti i file Immagine (*.jpg;*.bmp;*.png;*.gif;*.tif)|*.jpg;*.bmp;*.png;*.gif;*.tif|JPEG (*.jpg)|*.jpg|Bitmap (*.bmp)|*.bmp|png files (*.png)|*.png|GIF (*.gif)|*.gif|TIFF (*.tif)|*.tif";
                this.ucPictureSelect.ViewSaveFileDialogFilter = @"JPEG (*.jpg)|*.jpg|Bitmap (*.bmp)|*.bmp|png files (*.png)|*.png|GIF (*.gif)|*.gif|TIFF (*.tif)|*.tif";

                this.ucPictureSelect.ViewCheckSquareImage = false;
                this.ucPictureSelect.ViewCenterImage = true;
                this.ucPictureSelect.ViewUseLargeImages = true;

                this.ucPictureSelect.ViewImage = Scci.Statics.DrawingProcs.GetImageFromByte(CoreStatics.CoreApplication.MovSchedaAllegatoSelezionata.Documento);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaControlli", this.Name);
            }
        }

        #endregion

        #region Events Form

        private void frmImmagine_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.Close();
        }

        private void frmImmagine_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmImmagine_Shown(object sender, EventArgs e)
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

        #endregion

    }
}
