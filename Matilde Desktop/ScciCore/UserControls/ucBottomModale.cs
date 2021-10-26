using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinToolbars;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.ScciResource;
using System.Threading;
using UnicodeSrl.ScciCore.Common.TimersCB;

namespace UnicodeSrl.ScciCore
{
    public partial class ucBottomModale : UserControl, I_RefreshTimer_Controllo
    {
        public ucBottomModale()
        {
            InitializeComponent();
            m_sync = SynchronizationContext.Current;
        }

        #region Declare

        public event PulsanteBottomClickHandler PulsanteIndietroClick;
        public event PulsanteBottomClickHandler PulsanteAvantiClick;

        private SynchronizationContext m_sync = null;

        #endregion

        #region Events override

        public override void Refresh()
        {

            try
            {

                var oNavigazione = CoreStatics.CoreApplication.Navigazione;

                if (oNavigazione != null && oNavigazione.Maschere != null && oNavigazione.Maschere.MascheraSelezionata != null)
                {
                    this.pbStampe.Visible = oNavigazione.Maschere.MascheraSelezionata.Stampe;
                }
                this.TableLayoutPanelHome.ColumnStyles[4].Width = (this.pbStampe.Visible == false ? 0 : 20);
                this.pbStampe.Width = Convert.ToInt32(Convert.ToDouble(this.pbStampe.Height) * 1.2D);

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

                this.pbStampe.Image = Risorse.GetImageFromResource(Risorse.GC_REPORT_256);
                this.Refresh();
                CoreStatics.MainWnd.RefreshControllo_Subscribers.Add(this);

            }
            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (this.DesignMode == false)
            {

                this.pbStampe.Image = null;

                CoreStatics.MainWnd.RefreshControllo_Subscribers.Remove(this);

            }
            base.OnHandleDestroyed(e);
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

        private void pbStampe_Click(object sender, EventArgs e)
        {

            if (frmMain.CounterNav != 0)
                return;

            Interlocked.Increment(ref frmMain.CounterNav);

            if (Interlocked.Equals(Maschere._navigare, 0))
            {
                return;
            }
            else
            {
                CoreStatics.CoreApplication.Navigazione.Maschere.NavigaMaschera((EnumMaschere)Enum.Parse(typeof(EnumMaschere), CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.ID.ToString()), EnumPulsante.PulsanteStampeBottom);
            }

            Interlocked.Decrement(ref frmMain.CounterNav);

        }

        #endregion

        #region     I_RefreshTimer_Controllo

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

    }
}
