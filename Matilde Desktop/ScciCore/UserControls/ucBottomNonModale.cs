using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using UnicodeSrl.ScciCore.Common.TimersCB;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.ScciCore.ViewController;

namespace UnicodeSrl.ScciCore
{
    public partial class ucBottomNonModale : UserControl, I_RefreshTimer_Controllo, IViewControllerBottomNonModale
    {
        public ucBottomNonModale()
        {
            InitializeComponent();
            m_sync = SynchronizationContext.Current;
        }

        #region Declare

        private SynchronizationContext m_sync = null;

        public event PulsanteBottomClickHandler PulsanteIndietroClick;
        public event PulsanteBottomClickHandler PulsanteAvantiClick;

        #endregion

        #region Events override

        public override void Refresh()
        {

            try
            {

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            base.Refresh();

        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (this.DesignMode == false)
            {

                this.Refresh();
                CoreStatics.MainWnd.RefreshControllo_Subscribers.Add(this);

            }
            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (this.DesignMode == false)
            {

                CoreStatics.MainWnd.RefreshControllo_Subscribers.Remove(this);

            }
            base.OnHandleDestroyed(e);
        }

        #endregion

        #region I_RefreshTimer_Controllo

        public SynchronizationContext SyncContext
        {
            get
            {
                return m_sync;
            }
        }

        public void RefreshData(object state)
        {
            try
            {
                this.Refresh();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        #endregion I_RefreshTimer_Controllo

        #region IViewControllerBottomNonModale

        public ViewControllerBottomNonModale ViewController { get; set; }

        public Maschera Maschera { get; set; }

        public void InitViewController(IViewController viewcontroller)
        {
            this.ViewController = (ViewControllerBottomNonModale)viewcontroller;
        }

        public void LoadViewController()
        {
            this.Refresh();
        }

        public IViewController SaveViewController()
        {
            return this.ViewController;
        }

        #endregion

        #region Events

        private void ubIndietro_Click(object sender, EventArgs e)
        {
            if (PulsanteIndietroClick != null) { PulsanteIndietroClick(sender, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Indietro)); }
        }

        private void ubAvanti_Click(object sender, EventArgs e)
        {
            if (PulsanteAvantiClick != null) { PulsanteAvantiClick(sender, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti)); }
        }

        #endregion    

    }
}
