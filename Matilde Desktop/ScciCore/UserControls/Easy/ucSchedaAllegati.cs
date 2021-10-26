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
using UnicodeSrl.WpfControls;

namespace UnicodeSrl.ScciCore
{
    public partial class ucSchedaAllegati : UserControl, Interfacce.IViewUserControlBase
    {

        #region DECLARE

        private MovScheda _movscheda = null;

        private bool _bstorico = false;

        private const int C_MAX_COLS = 6;

        #endregion

        public ucSchedaAllegati()
        {
            InitializeComponent();
        }

        #region properties

        public MovScheda MovScheda
        {
            get { return _movscheda; }
            set { _movscheda = value; }
        }

        public bool Storicizzata
        {
            get { return _bstorico; }
        }

        #endregion

        #region Interface

        public void ViewInit()
        {
            this.InitializeControls();
        }

        #endregion

        #region Public

        public void RefreshAnteprime()
        {
            try
            {
                if (!this.IsDisposed)
                {
                    if (this.ucMultiImageViewer.MultiMiniature != null) this.ucMultiImageViewer.MultiMiniature.Clear();
                    this.ucMultiImageViewer.MultiRows = 0;
                    this.ucMultiImageViewer.MultiColumns = 0;

                    if (this.MovScheda != null)
                    {
                        int totAllegati = this.MovScheda.Allegati.Count;
                        if (totAllegati > 0)
                        {
                            if (totAllegati <= C_MAX_COLS)
                            {
                                this.ucMultiImageViewer.MultiRows = 1;
                                this.ucMultiImageViewer.MultiColumns = totAllegati;
                            }
                            else
                            {
                                this.ucMultiImageViewer.MultiRows = (totAllegati / C_MAX_COLS) + 1;
                                this.ucMultiImageViewer.MultiColumns = C_MAX_COLS;
                            }


                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "RefreshAnteprime", this.Name);
            }

        }

        #endregion

        #region PRIVATE

        private void InitializeControls()
        {
            try
            {
                if (!this.IsDisposed)
                {
                    this.ucMultiImageViewer.TopBottomScroll = true;
                    this.ucMultiImageViewer.LeftRightScroll = false;
                    this.ucMultiImageViewer.MultiColumns = 4;
                    this.ucMultiImageViewer.MultiRows = 3;

                    if (this.ucMultiImageViewer.MultiMiniature != null)
                        this.ucMultiImageViewer.MultiMiniature.Clear();

                    this.ucMultiImageViewer.OnClickEvent += ucMultiImageViewer_OnClickEvent;
                }
            }
            catch
            {
            }
        }

        #endregion

        #region Events

        private void ucMultiImageViewer_OnClickEvent(object sender, string key)
        {
            foreach (Miniatura oMin in ((ucMultiImageViewer)sender).MultiMiniature.Values)
            {
                oMin.Selezione = null;
            }
                                    ((ucMultiImageViewer)sender).MultiMiniature[key].Selezione = System.Windows.Media.Brushes.Green;
            ((ucMultiImageViewer)sender).Carica();
        }


        #endregion

    }
}
