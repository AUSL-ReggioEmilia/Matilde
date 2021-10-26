using Infragistics.Win;
using Infragistics.Win.UltraWinListView;
using NTwain;
using NTwain.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciCore.Common;
using UnicodeSrl.ScciCore.Common.Twain;

namespace UnicodeSrl.ScciCore
{
                public partial class frmPUAcquisizione :
        frmBaseModale,
        Interfacce.IViewFormlModal
    {

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public frmPUAcquisizione()
        {
            InitializeComponent();
        }

        #region     Dispose

                                        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (disposing && (this.ScciTwain != null))
            {
                this.ScciTwain.ImageCaptured -= ScciTwain_ImageCaptured;
                this.ScciTwain.TwainStateChanged -= ScciTwain_TwainStateChanged;
                this.ScciTwain.Dispose();
            }


            base.Dispose(disposing);
        }

        #endregion  Dispose

        #region IViewFormlModal

        public new void Carica()
        {
            try
            {
                                                
                                this.lstSources.View = UltraListViewStyle.List;
                this.lstSources.ViewSettingsList.ImageSize = new Size(64, 64);
                this.lstSources.ViewSettingsList.ColumnWidth = this.lstSources.Width / 2;
                this.lstSources.ViewSettingsList.MultiColumn = true;

                                this.picWaiting.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_CURSORWAIT);
                this.picWaiting.Visible = false;

                                this.Initializing = true;

                                ScciTwainData.Images.Clear();

                
                this.ScciTwain = new ScciTwain();

                this.ScciTwain.ImageCaptured += ScciTwain_ImageCaptured;
                this.ScciTwain.TwainStateChanged += ScciTwain_TwainStateChanged;

                this.ScciTwain.LoadSources();

                this.Images = new List<Image>();
                this.ActiveImageIndex = -1;

                this.lblPagina.Text = "-";

                                uiInitShortcuts();

                                uiLoadListSource();
                uiCommands();

                this.Initializing = false;

                this.ShowDialog();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                easyStatics.EasyMessageBox("Non è possibile inizializzare le periferiche di scansione.", "Acquisizione",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
            finally
            {

            }

        }


        #endregion

        #region     Prop

                                private bool Initializing { get; set; }

                                private ScciTwain ScciTwain { get; set; }

                                private List<Image> Images { get; set; }

                                private Image ActiveImage
        {
            get
            {
                if (this.Images == null) return null;

                if (this.ActiveImageIndex < 0) return null;

                if (this.Images.Count == 0) return null;

                return (this.Images[this.ActiveImageIndex]);
            }
        }

                                private int ActiveImageIndex { get; set; }

        #endregion  Prop

        #region     Twain 

        private void ScciTwain_ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            Bitmap capImg = null;
            Bitmap capBmp = (Bitmap)e.Image;

                        using (MemoryStream stream = new MemoryStream())
            {
                capBmp.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                capImg = (Bitmap)Image.FromStream(stream);
            }

            this.BeginInvoke
                (
                    new MethodInvoker
                    (
                        () => imageCapturedSync(capImg)
                    )
                );
            
        }

                                        private void imageCapturedSync(Bitmap capImg)
        {
            this.picPreview.Image = capImg;

                        this.Images.Add(capImg);
            this.ActiveImageIndex = this.Images.Count - 1;

                        uiCommands();

                        uiFeedbackImages();

                        bool check = this.checkSize();

            if (check == false)
            {
                string msg = @"Il documento acquisito supera la dimensione massima consentita. " + Environment.NewLine +
                             @"Sarà necessario rimuovere alcune pagine prima di poter confermare l'inserimento.";

                easyStatics.EasyMessageBox(msg, "Acquisizione documento", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

                                private void ScciTwain_TwainStateChanged(object sender, TwainStateChangedEventArgs e)
        {
            try
            {
                                if ( (this.IsHandleCreated)  )
                {
                    this.BeginInvoke(new MethodInvoker(() => SetForegroundWindow(this.Handle)));
                }

                                uiCommands();

                                uiFeedbackState();
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ScciTwain_TwainStateChanged", this.Text);
            }
        }


        #endregion  Twain 

        #region     priv

                                private void uiLoadListSource()
        {
            try
            {
                this.lstSources.Items.Clear();

                if (this.ScciTwain.State == en_Twain_State.Undefined)
                    return;

                foreach (DataSource twainDS in this.ScciTwain.TwainSources)
                {
                    UltraListViewItem item = this.lstSources.Items.Add(twainDS.Name, twainDS.Name);
                    item.Tag = twainDS;
                }

                this.lstSources.MainColumn.Width = this.lstSources.Width - 10;

                if (this.lstSources.Items.Count > 0)
                    this.lstSources.SelectedItems.Add(this.lstSources.Items[0]);

            }

            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "uiLoadListSource", this.Text);
            }

        }

                                private void twainLoadCaps()
        {
            cboQual.Clear();

            DataSource src = this.ScciTwain.CurrentSource;
            if (src == null) return;

                        if (src.Capabilities.ICapPixelType.IsSupported)
            {
                ICapWrapper<PixelType> cap = src.Capabilities.ICapPixelType;

                List<PixelType> list = cap.GetValues().ToList();
                cboQual.DataSource = list;
                PixelType cur = cap.GetCurrent();

                if (list.Contains(cur))
                {
                    cboQual.Text = cur.ToString();
                }

            }

            
            if (src.Capabilities.ICapXResolution.IsSupported && src.Capabilities.ICapYResolution.IsSupported)
            {
                ICapWrapper<TWFix32> capDPI = src.Capabilities.ICapXResolution;

                List<TWFix32> list = capDPI.GetValues().Where(dpi => (dpi % 50) == 0).ToList();

                cboDPI.DataSource = list;
                TWFix32 cur = capDPI.GetCurrent();

                if (list.Contains(cur))
                {
                    cboDPI.Text = cur.ToString();
                }

            }

                        TWFix32 sel = this.ScciTwain.CurrentSource.Capabilities.ICapXResolution.GetDefault();
            ReturnCode rc = this.ScciTwain.CurrentSource.Capabilities.ICapXResolution.SetValue(sel);

            if (rc != ReturnCode.Success)
            {
                this.tlpCaps.Visible = false;
                this.tlpSourceList.ColumnStyles[0].Width = 100;
                this.tlpSourceList.ColumnStyles[1].Width = 0;
                this.tlpSourceList.ColumnStyles[2].Width = 0;
                this.tlpSourceList.ColumnStyles[3].Width = 0;
            }               
            else
            {
                this.tlpCaps.Visible = true;

                this.tlpSourceList.ColumnStyles[0].Width = 72;
                this.tlpSourceList.ColumnStyles[1].Width = 1;
                this.tlpSourceList.ColumnStyles[2].Width = 26;
                this.tlpSourceList.ColumnStyles[3].Width = 1;
            }

        }

                                private void uiCommands()
        {
            if (this.InvokeRequired)
            {
                MethodInvoker del = delegate { this.uiCommandsSync(); };
                this.BeginInvoke(del);
            }

            else
                this.uiCommandsSync();
        }

                                private void uiCommandsSync()
        {
                        if (this.ScciTwain.State == en_Twain_State.DsReadyToTransfer)
            {
                this.Enabled = false;
                return;
            }
            else
                this.Enabled = true;

            
                        this.cmdAcq.Enabled = ((this.ScciTwain != null) && (this.ScciTwain.State == en_Twain_State.DsOpened));

                        this.cmdDel.Enabled = (this.ActiveImage != null);

                        this.cmdPagPrev.Enabled = (this.ActiveImageIndex > 0);
            this.cmdPagNext.Enabled = (this.ActiveImageIndex < (this.Images.Count - 1));

                        this.cmdRuotaSx.Enabled = (this.ActiveImage != null);
            this.cmdRuotaDx.Enabled = (this.ActiveImage != null);
        }

                                private void uiFeedbackImages()
        {
            string feedback = @"Pagina {0} di {1}";

            this.BeginInvoke
                (
                    new MethodInvoker(() => this.lblPagina.Text = String.Format(feedback, this.ActiveImageIndex + 1, this.Images.Count))
                );
        }

                                private void uiFeedbackState()
        {
            if (this.InvokeRequired == false)
            {
                this.lblState.Text = "";
                this.lblState.Appearance.BackColor = Color.Transparent;
                this.picWaiting.Visible = false;
            }

            if (this.IsHandleCreated == false)
                return;

            Color background = Color.Transparent;
            String textOut = "";

            if (
                    (this.ScciTwain.State == en_Twain_State.DsReadyToTransfer) ||
                    (this.ScciTwain.State == en_Twain_State.ImageTransferring)
                )
            {
                background = Color.LightPink;
                textOut = "Acquisizione in corso...";
            }

            this.BeginInvoke
                (
                    new MethodInvoker
                        (
                            () =>
                                {
                                    this.lblState.Appearance.BackColor = background;
                                    this.lblState.Text = textOut;
                                    this.picWaiting.Visible = true;
                                }
                        )
                );

        }

                                private void uiInitShortcuts()
        {
            this.cmdAcq.ShortcutKey = Keys.Add;
            this.cmdAcq.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

            this.cmdDel.ShortcutKey = Keys.Delete;
            this.cmdDel.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

            this.cmdPagNext.ShortcutKey = Keys.PageDown;
            this.cmdPagNext.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

            this.cmdPagPrev.ShortcutKey = Keys.PageUp;
            this.cmdPagPrev.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

            this.cmdRuotaSx.ShortcutKey = Keys.S;
            this.cmdRuotaSx.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

            this.cmdRuotaDx.ShortcutKey = Keys.D;
            this.cmdRuotaDx.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
        }

                                                        public Image RotateImage(Image img, RotateFlipType rotate)
        {
            var bmp = new Bitmap(img);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.Clear(Color.White);
                gfx.DrawImage(img, 0, 0, img.Width, img.Height);
            }

            bmp.RotateFlip(rotate);
            return bmp;
        }

                                private void updateActiveImage(Image newImg)
        {
            this.Images[this.ActiveImageIndex] = newImg;
        }

                                        private bool checkSize()
        {
                        int iMaxSize = 0;
            string maxSize = Database.GetConfigTable(EnumConfigTable.AllegatiDimensioneMassimaMb);
            iMaxSize = Convert.ToInt32(maxSize) * 1048576;

            int szImages = 0;

            foreach (Image img in this.Images)
            {
                byte[] buffer = ImageToByte2(img);

                int ln = buffer.Length;
                szImages = szImages + ln;

                if (iMaxSize < szImages)
                    return false;
            }

            return true;
        }

        private byte[] ImageToByte2(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }

        #endregion  priv

        #region     Control events

        private void lstSources_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            bool initRestore = this.Initializing;

            try
            {
                if (this.ScciTwain.CurrentSource != null)
                    this.ScciTwain.CurrentSource.Close();

                if ((this.lstSources.SelectedItems == null) || (this.lstSources.SelectedItems.Count == 0))
                {
                    this.ScciTwain.CloseSession();
                    return;
                }

                                if (this.ScciTwain.TwainSession == null)
                    return;

                this.Initializing = true;

                DataSource twainDS = (DataSource)this.lstSources.SelectedItems[0].Tag;

                twainDS.Open();

                twainLoadCaps();

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "lstSources_ItemSelectionChanged", this.Text);
            }
            finally
            {
                this.Initializing = initRestore;
            }

        }


        private void frmPUAcquisizione_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {

                        bool check = this.checkSize();

            if (check == false)
            {
                string msg = @"Il documento acquisito supera la dimensione massima consentita. ";
                easyStatics.EasyMessageBox(msg, "Acquisizione documento", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            ScciTwainData.Images.AddRange(this.Images);
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void frmPUAcquisizione_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            ScciTwainData.Images.Clear();
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

                                                private void cmdAcq_Click(object sender, EventArgs e)
        {       
            bool start = this.ScciTwain.Scan(SourceEnableMode.ShowUI, this.Handle);
            
        }

                                                private void cboDPI_SelectionChanged(object sender, EventArgs e)
        {
            if (this.Initializing)
                return;

            if (this.ScciTwain.State == en_Twain_State.DsOpened)
            {
                TWFix32 cached = this.ScciTwain.CurrentSource.Capabilities.ICapXResolution.GetCurrent();

                TWFix32 sel = (TWFix32)cboDPI.SelectedItem.ListObject;
                ReturnCode rc = this.ScciTwain.CurrentSource.Capabilities.ICapXResolution.SetValue(sel);
                if (rc == ReturnCode.Success) rc = this.ScciTwain.CurrentSource.Capabilities.ICapYResolution.SetValue(sel);

                                this.Initializing = true;

                if (rc != ReturnCode.Success)
                {
                                        this.ScciTwain.CurrentSource.Capabilities.ICapXResolution.SetValue(cached);
                    this.ScciTwain.CurrentSource.Capabilities.ICapYResolution.SetValue(cached);
                    this.cboDPI.Text = cached.ToString();
                }

                this.Initializing = false;
                            }

        }

                                                private void cboQual_SelectionChanged(object sender, EventArgs e)
        {
            if (this.Initializing)
                return;

            if (this.ScciTwain.State == en_Twain_State.DsOpened)
            {
                PixelType cached = this.ScciTwain.CurrentSource.Capabilities.ICapPixelType.GetCurrent();

                PixelType sel = (PixelType)cboQual.SelectedItem.ListObject;
                ReturnCode rc = this.ScciTwain.CurrentSource.Capabilities.ICapPixelType.SetValue(sel);

                                this.Initializing = true;

                if (rc != ReturnCode.Success)
                {
                    this.ScciTwain.CurrentSource.Capabilities.ICapPixelType.SetValue(cached);
                    this.cboQual.Text = cached.ToString();
                }

                this.Initializing = false;
                            }
        }

                                                private void cmdRuotaSx_Click(object sender, EventArgs e)
        {
            if (this.ActiveImage == null) return;

            Image newImg = this.RotateImage(this.ActiveImage, RotateFlipType.Rotate270FlipNone);

            this.updateActiveImage(newImg);
            this.picPreview.Image = newImg;
        }

                                                private void cmdRuotaDx_Click(object sender, EventArgs e)
        {
            if (this.ActiveImage == null) return;

            Image newImg = this.RotateImage(this.ActiveImage, RotateFlipType.Rotate90FlipNone);
            this.updateActiveImage(newImg);
            this.picPreview.Image = newImg;
        }

                                                private void cmdPagPrev_Click(object sender, EventArgs e)
        {
            this.ActiveImageIndex = this.ActiveImageIndex - 1;
            this.picPreview.Image = this.ActiveImage;

            uiCommands();
            uiFeedbackImages();
        }

                                                private void cmdPagNext_Click(object sender, EventArgs e)
        {
            this.ActiveImageIndex = this.ActiveImageIndex + 1;
            this.picPreview.Image = this.ActiveImage;

            uiCommands();
            uiFeedbackImages();
        }

                                                private void cmdDel_Click(object sender, EventArgs e)
        {
            this.Images.RemoveAt(this.ActiveImageIndex);

            if (this.ActiveImageIndex == 0)
                this.ActiveImageIndex = 0;
            else
                this.ActiveImageIndex = this.ActiveImageIndex - 1;

            this.picPreview.Image = this.ActiveImage;

            uiCommands();
            uiFeedbackImages();

        }


        #endregion  Control events        
    }
}
