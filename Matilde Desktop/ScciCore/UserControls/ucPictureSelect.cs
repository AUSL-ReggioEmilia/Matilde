using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciCore;
using Infragistics.Win.UltraWinToolbars;
using UnicodeSrl.Scci.Enums;

namespace UnicodeSrl.ScciCore
{
    public partial class ucPictureSelect : UserControl, Interfacce.IViewPictureSelect
    {

        public ucPictureSelect()
        {
            InitializeComponent();

        }

        #region Declare

        bool _bCheckSquareImage = true;
        Image _picImage = null;

        string _ViewOpenFileDialogFilter = @"png files (*.png)|*.png";
        string _ViewSaveFileDialogFilter = @"png files (*.png)|*.png|jpg files (*.jpg)|*.jpg|bmp files (*.bmp)|*.bmp";

        enumZoomfactor _zf = enumZoomfactor.zf100;
        enumZoomfactor[] arZoomFactor = null;

        bool _centerImage = false;

        public event ChangeEventHandler PictureChange;

        #endregion

        #region Interface

        public bool ViewCheckSquareImage
        {
            get
            {
                return _bCheckSquareImage;
            }
            set
            {
                _bCheckSquareImage = value;
            }
        }

        public Image ViewImage
        {
            get
            {
                return _picImage;
            }
            set
            {
                SetPicture(ref value);
            }
        }

        public bool ViewShowAddImage
        {
            get
            {
                return this.UltraToolbarsManager.Tools["Aggiungi"].SharedProps.Visible;
            }
            set
            {
                this.UltraToolbarsManager.Tools["Aggiungi"].SharedProps.Visible = value;
            }
        }

        public bool ViewShowRemoveImage
        {
            get
            {
                return this.UltraToolbarsManager.Tools["Rimuovi"].SharedProps.Visible;
            }
            set
            {
                this.UltraToolbarsManager.Tools["Rimuovi"].SharedProps.Visible = value;
            }
        }

        public bool ViewShowSaveImage
        {
            get
            {
                return this.UltraToolbarsManager.Tools["Salva"].SharedProps.Visible;
            }
            set
            {
                this.UltraToolbarsManager.Tools["Salva"].SharedProps.Visible = value;
            }
        }

        public bool ViewShowToolbar
        {
            get
            {
                return this.UltraToolbarsManager.Toolbars["UltraToolbarMain"].Visible;
            }
            set
            {
                this.UltraToolbarsManager.Toolbars["UltraToolbarMain"].Visible = value;
            }
        }

        public ToolbarStyle ViewToolbarStyle
        {
            get
            {
                return this.UltraToolbarsManager.Style;
            }
            set
            {
                this.UltraToolbarsManager.Style = value;
            }
        }

        public bool ViewUseLargeImages
        {
            get
            {
                return (this.UltraToolbarsManager.ToolbarSettings.UseLargeImages == Infragistics.Win.DefaultableBoolean.False ? false : true);
            }
            set
            {
                this.UltraToolbarsManager.ToolbarSettings.UseLargeImages = (value == false ? Infragistics.Win.DefaultableBoolean.False : Infragistics.Win.DefaultableBoolean.True);
            }
        }

        public bool ViewCenterImage
        {
            get
            {
                return _centerImage;
            }
            set
            {
                _centerImage = value;
                PositionPicture();
            }
        }

        public enumZoomfactor ViewZoomFactor
        {
            get
            {
                return _zf;
            }
            set
            {
                _zf = value;
            }
        }

        public string ViewOpenFileDialogFilter
        {
            get
            {
                if (_ViewOpenFileDialogFilter.Trim() == "") _ViewOpenFileDialogFilter = @"png files (*.png)|*.png";
                return _ViewOpenFileDialogFilter;
            }
            set
            {
                _ViewOpenFileDialogFilter = value;
            }
        }

        public string ViewSaveFileDialogFilter
        {
            get
            {
                if (_ViewSaveFileDialogFilter.Trim() == "") _ViewSaveFileDialogFilter = @"png files (*.png)|*.png|jpg files (*.jpg)|*.jpg|bmp files (*.bmp)|*.bmp";
                return _ViewSaveFileDialogFilter;
            }
            set
            {
                _ViewSaveFileDialogFilter = value;
            }
        }

        public void ViewFitImage()
        {
            try
            {
                int nDimensione = (this.PanelPicture.Width < this.PanelPicture.Height ? this.PanelPicture.Width : this.PanelPicture.Height);
                this.pbPic.Size = new Size(nDimensione - 1, nDimensione - 1);
                PositionPicture();
            }
            catch
            {
            }
        }

        public bool ViewAddImage()
        {
            try
            {
                bool bReturn = false;

                OpenFileDialog.Title = "Seleziona Immagine";
                OpenFileDialog.CheckFileExists = true;
                OpenFileDialog.CheckPathExists = true;
                OpenFileDialog.Multiselect = false;
                OpenFileDialog.Filter = ViewOpenFileDialogFilter; OpenFileDialog.FilterIndex = 0;
                if (OpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.SetPicture(OpenFileDialog.FileName);
                    bReturn = true;
                    if (PictureChange != null) { PictureChange(this, new EventArgs()); }
                }

                return bReturn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ViewInit()
        {

            this.InitializeUltraToolbarsManager();

            arZoomFactor = (enumZoomfactor[])Enum.GetValues(typeof(enumZoomfactor));
            this.utbZoom.MinValue = 0;
            this.utbZoom.MaxValue = arZoomFactor.Length - 1;
            this.utbZoom.Value = Array.IndexOf(arZoomFactor, enumZoomfactor.zf100);

        }

        #endregion

        #region UltraToolBar

        private void InitializeUltraToolbarsManager()
        {

            try
            {

                var utb = this.UltraToolbarsManager;

                utb.Style = ToolbarStyle.Office2007;
                utb.ToolbarSettings.UseLargeImages = Infragistics.Win.DefaultableBoolean.False;

                utb.Tools["Aggiungi"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.Immagine_32;
                utb.Tools["Aggiungi"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Immagine_16;

                utb.Tools["Rimuovi"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.Cancella_32;
                utb.Tools["Rimuovi"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Cancella_16;

                utb.Tools["Salva"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.Salva_32;
                utb.Tools["Salva"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Salva_16;

                utb.Tools["Zoom Più"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.ZoomIn_32;
                utb.Tools["Zoom Più"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.ZoomIn_16;

                utb.Tools["Zoom Meno"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.ZoomOut_32;
                utb.Tools["Zoom Meno"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.ZoomOut_16;

                utb.Tools["Adatta"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.Fit_32;
                utb.Tools["Adatta"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Fit_16;

                utb.Tools["Dimensioni Reali"].SharedProps.AppearancesLarge.Appearance.Image = Properties.Resources.Zoom100_32;
                utb.Tools["Dimensioni Reali"].SharedProps.AppearancesSmall.Appearance.Image = Properties.Resources.Zoom100_16;

            }
            catch (Exception)
            {

            }

        }

        private void ActionToolClick(ToolBase oTool)
        {

            try
            {

                switch (oTool.Key)
                {

                    case "Aggiungi":
                        OpenFileDialog.Title = "Seleziona Immagine";
                        OpenFileDialog.CheckFileExists = true;
                        OpenFileDialog.CheckPathExists = true;
                        OpenFileDialog.Multiselect = false;
                        OpenFileDialog.Filter = ViewOpenFileDialogFilter; OpenFileDialog.FilterIndex = 0;
                        if (OpenFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            this.SetPicture(OpenFileDialog.FileName);
                            if (PictureChange != null) { PictureChange(this, new EventArgs()); }
                        }
                        break;

                    case "Rimuovi":
                        if (MessageBox.Show("Confermi la cancellazione dell'immagine ?", "Cancellazione Immagine", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            this.RemovePicture();
                            if (PictureChange != null) { PictureChange(this, new EventArgs()); }
                        }
                        break;

                    case "Salva":
                        this.SavePicture();
                        break;

                    case "Zoom Più":
                        try
                        {
                            enumZoomfactor[] zfvalues = (enumZoomfactor[])Enum.GetValues(typeof(enumZoomfactor));
                            enumZoomfactor zfvalue = zfvalues.Cast<enumZoomfactor>().Where(item => (int)item > (int)this.ViewZoomFactor).OrderBy(item => item).First();
                            this.utbZoom.Value = Array.IndexOf(arZoomFactor, zfvalue);
                        }
                        catch
                        {
                        }
                        break;

                    case "Zoom Meno":
                        try
                        {
                            enumZoomfactor[] zfvalues = (enumZoomfactor[])Enum.GetValues(typeof(enumZoomfactor));
                            enumZoomfactor zfvalue = zfvalues.Cast<enumZoomfactor>().Where(item => (int)item < (int)this.ViewZoomFactor).OrderBy(item => item).Last();
                            this.utbZoom.Value = Array.IndexOf(arZoomFactor, zfvalue);
                        }
                        catch
                        {
                        }
                        break;

                    case "Adatta":
                        ViewFitImage();
                        break;

                    case "Dimensioni Reali":
                        if (this.utbZoom.Value == Array.IndexOf(arZoomFactor, enumZoomfactor.zf100))
                            this.ResizePicture();
                        else
                            this.utbZoom.Value = Array.IndexOf(arZoomFactor, enumZoomfactor.zf100);
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

        #region Subroutine

        private void SetPicture(string FilePath)
        {
            Image imgLoad = Image.FromFile(FilePath);
            SetPicture(ref imgLoad);
        }
        private void SetPicture(ref Image roImage)
        {

            try
            {

                if (roImage != null)
                {

                    if (CheckImage(ref roImage))
                    {
                        this._picImage = roImage;
                        this.pbPic.Width = _picImage.Width;
                        this.pbPic.Height = _picImage.Height;
                        this.pbPic.Image = _picImage;
                        this.pbPic.Refresh();
                        this.SetInfoImage(string.Format(@"Dimensione : {0}x{1}", _picImage.Width.ToString(), _picImage.Height.ToString()));
                        this.utbZoom.Enabled = true;
                        this.utbZoom.Value = Array.IndexOf(arZoomFactor, enumZoomfactor.zf100);
                        this.ResizePicture();
                    }
                    else
                    {
                        MessageBox.Show("L'immagine selezionata non è quadrata", "Selezione Immagine", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.pbPic.Width = 0;
                        this.pbPic.Height = 0;
                        this.pbPic.Image = null;
                        this.pbPic.Refresh();
                        this.SetInfoImage(@"");
                        this.utbZoom.Value = Array.IndexOf(arZoomFactor, enumZoomfactor.zf100);
                        this.utbZoom.Enabled = false;
                    }

                }
                else
                {
                    this.RemovePicture();
                }

            }
            catch
            {

            }

        }

        private void RemovePicture()
        {

            try
            {

                this._picImage = null;
                this.pbPic.Width = 16;
                this.pbPic.Height = 16;
                this.pbPic.Image = null;
                this.pbPic.Refresh();
                this.SetInfoImage(@"");
                this.utbZoom.Value = Array.IndexOf(arZoomFactor, enumZoomfactor.zf100);
                this.utbZoom.Enabled = false;

            }
            catch
            {

            }

        }

        private void ResizePicture()
        {

            try
            {

                if (this._picImage != null)
                {

                    this.pbPic.Width = (int)Math.Round((decimal)(this._picImage.Width * (int)this.ViewZoomFactor / 100), 0);
                    this.pbPic.Height = (int)Math.Round((decimal)(this._picImage.Height * (int)this.ViewZoomFactor / 100), 0);

                    PositionPicture();

                    this.pbPic.Refresh();

                }

            }
            catch
            {

            }

        }

        private void PositionPicture()
        {
            try
            {
                int left = 0;
                int top = 0;

                if (_centerImage)
                {
                    left = Convert.ToInt32((this.PanelPicture.Width - this.pbPic.Width) / 2);
                    top = Convert.ToInt32((this.PanelPicture.Height - this.pbPic.Height) / 2);
                }

                this.pbPic.Location = new Point(left, top);
            }
            catch (Exception)
            {
            }
        }

        private void SavePicture()
        {

            try
            {
                if (this._picImage != null)
                {
                    SaveFileDialog ofd = new SaveFileDialog();

                    ofd.CheckPathExists = true;
                    ofd.Filter = ViewSaveFileDialogFilter; ofd.FilterIndex = 0;

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        System.Drawing.Imaging.ImageFormat oimgFormat = null;

                        switch (System.IO.Path.GetExtension(ofd.FileName).ToUpper().Replace(".", ""))
                        {
                            case "BMP":
                                oimgFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                                break;
                            case "JPG":
                            case "JPEG":
                                oimgFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                                break;
                            case "PNG":
                                oimgFormat = System.Drawing.Imaging.ImageFormat.Png;
                                break;
                            case "GIF":
                                oimgFormat = System.Drawing.Imaging.ImageFormat.Gif;
                                break;
                            case "TIF":
                                oimgFormat = System.Drawing.Imaging.ImageFormat.Tiff;
                                break;
                            case "ICO":
                                oimgFormat = System.Drawing.Imaging.ImageFormat.Icon;
                                break;
                            default:
                                oimgFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                                break;
                        }

                        this._picImage.Save(ofd.FileName, oimgFormat);
                    }
                }
            }
            catch
            {

            }

        }

        private bool CheckImage(ref Image roImage)
        {

            bool bRet = true;

            if (this.ViewCheckSquareImage)
            {
                bRet = (roImage.Width == roImage.Height);
            }

            return bRet;

        }

        private void SetInfoImage(string info)
        {

            try
            {

                var utb = this.UltraToolbarsManager;
                utb.Tools["InfoImage"].SharedProps.Caption = info;

            }
            catch (Exception)
            {

            }

        }

        #endregion

        #region Events UserControl

        private void ucPictureSelect_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                this.InitializeUltraToolbarsManager();
            }
        }

        #endregion

        #region Events

        private void UltraToolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            this.ActionToolClick(e.Tool);
        }

        private void utbZoom_ValueChanged(object sender, EventArgs e)
        {
            this.ViewZoomFactor = (Enum.GetValues(typeof(enumZoomfactor)) as enumZoomfactor[])[this.utbZoom.Value];
            this.ResizePicture();
        }

        private void panelPicture_MouseWheel(object sender, MouseEventArgs e)
        {
            this.PanelPicture.Focus();
        }

        private void panelPicture_MouseHover(object sender, EventArgs e)
        {
            this.PanelPicture.Focus();
        }

        #endregion

    }
}
