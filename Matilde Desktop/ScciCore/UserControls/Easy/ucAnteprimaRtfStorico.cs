using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.WpfControls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;
using UnicodeSrl.ScciCore.DifferenceEngine;

namespace UnicodeSrl.ScciCore
{
    public partial class ucAnteprimaRtfStorico : UserControl, Interfacce.IViewUserControlBase
    {
        public ucAnteprimaRtfStorico()
        {
            InitializeComponent();
        }

        #region declare

        private MovScheda _movscheda = null;
        private MovScheda _movschedastorico = null;
        private bool _binitialize = false;
        private string _DataUltimaModifica = "";

        private bool _bstorico = false;

        private float _zoomrtf = 1F;
        private float _zoomrtfstep = 0.10F;

        public event ChangeEventHandler StoricoChange;

        private bool _bclick = false;

        #endregion

        #region Interface

        public void ViewInit()
        {
            this.InitializeControls();
        }

        #endregion

        #region properties

        public MovScheda MovScheda
        {
            get { return _movscheda; }
            set
            {
                this.ucEasyTabControl.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard;
                _movscheda = value;
            }
        }

        public bool Storicizzata
        {
            get { return _bstorico; }
        }

        #endregion

        #region public methods

        public void RefreshRTF(float ZoomRtf)
        {
            _zoomrtf = ZoomRtf;
            this.RefreshRTF();
        }
        public void RefreshRTF()
        {

            if (this.IsDisposed == false)
            {
                this.InitializeControls();

                if (this.MovScheda != null)
                {
                    this.LoadStorico();
                    this.CaricaRTF();
                }
                else
                {
                    _movschedastorico = null;
                    this.ucEasyTabControl.Tabs["Allegati"].Visible = false;
                }
            }

        }

        #endregion

        #region private methods

        private void InitializeControls()
        {
            if (this.IsDisposed == false)
            {
                this._binitialize = true;

                this.lblUtenteUltimaModifica.Text = "";
                this.utbStorico.Enabled = false;
                this.cboStorico.Enabled = false;
                this.ubOggi.Enabled = false;
                this.ubSuccessivo.Enabled = false;
                this.ubPrecedente.Enabled = false;
                this.ubZoomMeno.Enabled = false;
                this.ubZoomPiu.Enabled = false;

                try
                {
                    if (this.rtbRichTextBox != null && !this.rtbRichTextBox.Disposing && !this.rtbRichTextBox.IsDisposed)
                    {
                        this.rtbRichTextBox.Rtf = @"";
                        this.rtbRichTextBox.BackColor = Color.LightGray;
                        this.rtbRichTextBox.ZoomFactor = 1;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }

                this.utbStorico.MinValue = 0;
                this.utbStorico.MaxValue = 0;

                this.cboStorico.Value = null;
                this.cboStorico.DataSource = null;

                this.sbDifference.UNCheckedImage = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_DIFFERENZE_256);
                this.sbDifference.CheckedImage = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_DIFFERENZE_256);
                this.sbDifference.PercImageFill = 0.75F;
                this.sbDifference.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.sbDifference.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ucMultiImageViewer.TopBottomScroll = true;
                this.ucMultiImageViewer.LeftRightScroll = false;
                if (SystemParameters.PrimaryScreenWidth == 1366 && SystemParameters.PrimaryScreenHeight == 768)
                {
                    this.ucMultiImageViewer.MultiColumns = 2;
                    this.ucMultiImageViewer.MultiRows = 2;
                }
                else
                {
                    this.ucMultiImageViewer.MultiColumns = 4;
                    this.ucMultiImageViewer.MultiRows = 3;
                }
                this.ucMultiImageViewer.MyImageSourceTop = ByteToBitmapImage(imageToByteArray(ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_SB_SU)));
                this.ucMultiImageViewer.MyImageSourceBottom = ByteToBitmapImage(imageToByteArray(ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_SB_GIU)));
                this.ucMultiImageViewer.OnClickEvent += ucMultiImageViewer_MultiImageViewerClick;

                this._binitialize = false;
            }
        }

        private void LoadStorico()
        {
            try
            {

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodEntita", this.MovScheda.CodEntita.ToString());
                op.Parametro.Add("IDEntita", this.MovScheda.IDEntita.ToString());
                op.Parametro.Add("Numero", this.MovScheda.Numero.ToString());
                op.Parametro.Add("CodScheda", this.MovScheda.CodScheda.ToString());
                op.Parametro.Add("IDScheda", this.MovScheda.IDMovScheda.ToString());

                if (this.MovScheda.IDSchedaPadre != null && this.MovScheda.IDSchedaPadre.ToString() != string.Empty)
                {
                    op.Parametro.Add("IDSchedaPadre", this.MovScheda.IDSchedaPadre.ToString());
                }


                op.Parametro.Add("ElencoStorico", @"1");

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_SelMovSchedaBase", spcoll);

                if (this.IsDisposed == false)
                {
                    if (dt.Rows.Count > 0)
                    {
                        this._binitialize = true;

                        this.utbStorico.Enabled = true;
                        this.cboStorico.Enabled = true;
                        this.ubOggi.Enabled = true;
                        this.ubSuccessivo.Enabled = true;
                        this.ubPrecedente.Enabled = true;
                        this.ubZoomMeno.Enabled = true;
                        this.ubZoomPiu.Enabled = true;

                        this.utbStorico.MinValue = 0;
                        this.utbStorico.MaxValue = dt.Rows.Count - 1;
                        this.utbStorico.Value = this.utbStorico.MaxValue;

                        this.cboStorico.ValueMember = @"ID";
                        this.cboStorico.DisplayMember = @"DataUltimaModifica";
                        this.cboStorico.DataSource = dt;
                        this.cboStorico.SelectedIndex = (this.utbStorico.MaxValue - this.utbStorico.Value);

                        this._binitialize = false;
                    }
                    else
                        this.InitializeControls();
                }
                else
                {
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private void CaricaRTF()
        {
            if (this.IsDisposed == false)
            {

                this._DataUltimaModifica = "";
                this.lblUtenteUltimaModifica.Text = "";
                if (this.utbStorico.Value == this.utbStorico.MaxValue)
                {
                    if (this.MovScheda != null)
                    {
                        this.rtbRichTextBox.Rtf = this.MovScheda.AnteprimaRTF;
                        if (this.MovScheda.DescUtenteUltimaModifica != null && this.MovScheda.DescUtenteUltimaModifica != string.Empty && this.MovScheda.DescUtenteUltimaModifica.Trim() != "")
                            this.lblUtenteUltimaModifica.Text = @"(" + this.MovScheda.DescUtenteUltimaModifica + @")";
                        this._DataUltimaModifica = this.MovScheda.DataUltimaModifica.ToString();
                    }
                    else
                        this.rtbRichTextBox.Rtf = @"";

                    this._bstorico = false;
                    this.rtbRichTextBox.BackColor = Color.White;
                    this.ucEasyTabControl.Tabs["Allegati"].Visible = (this.MovScheda.Allegati.Count != 0);
                    this.ucEasyTabControl.Tabs["Allegati"].Text = string.Format("Allegati : {0}", this.MovScheda.Allegati.Count);
                    this.ucEasyTabControl.SelectedTab = this.ucEasyTabControl.Tabs[0];
                    this.ucEasyTabControl.Style = (this.ucEasyTabControl.Tabs["Allegati"].Visible == false ? Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard : Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Default);
                }
                else
                {
                    this._movschedastorico = new MovScheda(this.cboStorico.Value.ToString(), CoreStatics.CoreApplication.Ambiente);
                    this._DataUltimaModifica = this._movschedastorico.DataUltimaModifica.ToString();

                    if (this._movschedastorico != null)
                        this.rtbRichTextBox.Rtf = this._movschedastorico.AnteprimaRTF;
                    else
                        this.rtbRichTextBox.Rtf = @"";

                    this._bstorico = true;
                    this.rtbRichTextBox.BackColor = Color.LightGray;

                    if (this._movschedastorico.DescUtenteUltimaModifica != null && this._movschedastorico.DescUtenteUltimaModifica != string.Empty && this._movschedastorico.DescUtenteUltimaModifica.Trim() != "")
                        this.lblUtenteUltimaModifica.Text = @"(" + this._movschedastorico.DescUtenteUltimaModifica + @")";
                    this.ucEasyTabControl.Tabs["Allegati"].Visible = (this.MovScheda.Allegati.Count != 0);
                    this.ucEasyTabControl.Tabs["Allegati"].Text = string.Format("Allegati : {0}", this.MovScheda.Allegati.Count);
                    this.ucEasyTabControl.SelectedTab = this.ucEasyTabControl.Tabs[0];
                    this.ucEasyTabControl.Style = (this.ucEasyTabControl.Tabs["Allegati"].Visible == false ? Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard : Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Default);
                }

                this.rtbRichTextBox.ZoomFactor = _zoomrtf;

                if (this.sbDifference.Checked)
                {
                    this.CaricaucDifferenceEngineSchede();
                }

            }


        }

        private void CaricaucDifferenceEngineSchede()
        {

            DiffList_Text sLF = new DiffList_Text("");
            if ((this.cboStorico.SelectedIndex + 1) < this.cboStorico.Items.Count)
            {
                MovScheda oMS = new MovScheda(this.cboStorico.Items[this.cboStorico.SelectedIndex + 1].DataValue.ToString(), CoreStatics.CoreApplication.Ambiente);
                sLF = new DiffList_Text(oMS.AnteprimaTXT);
                sLF.Caption = oMS.DataUltimaModifica.ToString();
                oMS = null;
            }
            DiffList_Text dLF = new DiffList_Text(this.rtbRichTextBox.Text);
            dLF.Caption = this._DataUltimaModifica;
            this.ucDifferenceEngineSchede.RefreshDiff(sLF, dLF, DiffEngineLevel.SlowPerfect);

        }

        private Miniature CaricaMultiView()
        {

            Miniatura _Miniatura = null;
            Miniature _Miniature = new Miniature();

            try
            {

                if (this.IsDisposed == false)
                {

                    this.MovScheda.TipoRichiesta = EnumTipoRichiestaAllegatoScheda.THUMB;
                    if (this.MovScheda.Allegati.Count > 0)
                    {

                        foreach (MovSchedaAllegato oMsa in this.MovScheda.Allegati)
                        {

                            try
                            {

                                _Miniatura = new Miniatura();
                                _Miniatura.Immagine = Scci.Statics.DrawingProcs.GetBitmapSourceFromByte(oMsa.Anteprima);
                                _Miniatura.Titolo = oMsa.DescrizioneCampo + Environment.NewLine + oMsa.DescrizioneAllegato;
                                _Miniatura.Key = oMsa.IDMovSchedaAllegato;
                                _Miniature.Add(_Miniatura.Key, _Miniatura);

                            }
                            catch (Exception)
                            {

                            }

                        }

                    }
                    else
                        this.InitializeControls();

                }
                else
                {

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return _Miniature;

        }

        private BitmapSource ByteToBitmapSource(byte[] image)
        {

            BitmapImage imageSource = new BitmapImage();

            if (image != null)
            {

                imageSource.BeginInit();
                imageSource.StreamSource = new System.IO.MemoryStream(image);
                imageSource.EndInit();

            }

            return imageSource;

        }

        private BitmapImage ByteToBitmapImage(byte[] image)
        {

            BitmapImage imageSource = new BitmapImage();

            if (image != null)
            {

                imageSource.BeginInit();
                imageSource.StreamSource = new MemoryStream(image.ToArray());
                imageSource.EndInit();
            }

            return imageSource;

        }

        private byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }

        #endregion

        #region Events

        private void ucEasyTabControl_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {

            switch (e.Tab.Index)
            {

                case 0:
                    this.ucMultiImageViewer.Carica();
                    break;

                case 1:
                    this.ucMultiImageViewer.Carica(CaricaMultiView());
                    break;

            }

        }

        private void ubPrecedente_Click(object sender, EventArgs e)
        {
            if (this.IsDisposed == false)
            {
                if (this.utbStorico.Value > this.utbStorico.MinValue)
                    this.utbStorico.Value -= 1;
            }

        }

        private void ubSuccessivo_Click(object sender, EventArgs e)
        {
            if (this.IsDisposed == false)
            {
                if (this.utbStorico.Value < this.utbStorico.MaxValue)
                    this.utbStorico.Value += 1;
            }
        }

        private void ubOggi_Click(object sender, EventArgs e)
        {
            if (this.IsDisposed == false)
            {
                this.utbStorico.Value = this.utbStorico.MaxValue;
            }
        }

        private void cboStorico_ValueChanged(object sender, EventArgs e)
        {
            if (this.IsDisposed == false)
            {
                if (!_binitialize)
                {
                    this.utbStorico.Value = (this.utbStorico.MaxValue - this.cboStorico.SelectedIndex);
                    this.CaricaRTF();
                    if (StoricoChange != null) StoricoChange(sender, new EventArgs());
                }
            }

        }

        private void utbStorico_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.IsDisposed == false)
            {
                _binitialize = true;
            }
        }

        private void utbStorico_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.IsDisposed == false)
            {
                _binitialize = false;
                this.cboStorico_ValueChanged(this, new System.EventArgs());
            }

        }

        private void utbStorico_ValueChanged(object sender, EventArgs e)
        {
            if (this.IsDisposed == false)
            {
                this.cboStorico.SelectedIndex = (this.utbStorico.MaxValue - this.utbStorico.Value);
            }
        }

        private void ubZoomMeno_Click(object sender, EventArgs e)
        {
            if (this.IsDisposed == false)
            {
                this.rtbRichTextBox.ZoomFactor -= _zoomrtfstep;
            }
        }

        private void ubZoomPiu_Click(object sender, EventArgs e)
        {
            if (this.IsDisposed == false)
            {
                this.rtbRichTextBox.ZoomFactor += _zoomrtfstep;
            }
        }

        private void sbDifference_Click(object sender, EventArgs e)
        {
            if (this.IsDisposed == false)
            {

                if (this.sbDifference.Checked)
                {
                    this.ucEasyTableLayoutPanelSchede.RowStyles[0].Height = 50;
                    this.ucEasyTableLayoutPanelSchede.RowStyles[1].Height = 50;
                    this.CaricaucDifferenceEngineSchede();
                }
                else
                {
                    this.ucEasyTableLayoutPanelSchede.RowStyles[0].Height = 100;
                    this.ucEasyTableLayoutPanelSchede.RowStyles[1].Height = 0;
                }

            }
        }

        private void ucMultiImageViewer_MultiImageViewerClick(object sender, string key)
        {

            foreach (Miniatura oMin in ((ucMultiImageViewer)sender).MultiMiniature.Values)
            {
                oMin.Selezione = null;
            }
            ((ucMultiImageViewer)sender).MultiMiniature[key].Selezione = System.Windows.Media.Brushes.Green;

            IEnumerable<MovSchedaAllegato> msaQuery = from msa in this.MovScheda.Allegati
                                                      where msa.IDMovSchedaAllegato == ((ucMultiImageViewer)sender).MultiMiniature[key].Key
                                                      select msa;

            if (msaQuery != null && _bclick == false)
            {

                CoreStatics.CoreApplication.MovSchedaAllegatoSelezionata = new MovSchedaAllegato(msaQuery.ElementAt(0).IDMovSchedaAllegato, EnumTipoRichiestaAllegatoScheda.DOC, CoreStatics.CoreApplication.Ambiente);
                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Immagine, true);
                CoreStatics.CoreApplication.MovSchedaAllegatoSelezionata = null;

                _bclick = true;
                this.TimerClick.Enabled = true;

            }

        }

        private void TimerClick_Tick(object sender, EventArgs e)
        {
            this.TimerClick.Enabled = false;
            _bclick = false;

        }

        #endregion

    }
}
