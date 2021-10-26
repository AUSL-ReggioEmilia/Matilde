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
using UnicodeSrl.ScciCore.ViewController;

namespace UnicodeSrl.ScciCore
{
    public partial class ucTopNonModale : UserControl, I_RefreshTimer_Controllo, IViewControllerTopNonModale
    {
        public ucTopNonModale()
        {
            InitializeComponent();
            m_sync = SynchronizationContext.Current;
        }

        #region Declare

        private SynchronizationContext m_sync = null;

        #endregion

        #region Events override

        protected override void OnHandleCreated(EventArgs e)
        {
            if (this.DesignMode == false)
            {
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

        public override void Refresh()
        {

            try
            {

                var oPaziente = this.ViewController.Paziente;
                var oEpisodio = this.ViewController.Episodio;
                var oMaschera = this.ViewController.Maschera;

                if (oPaziente != null && oPaziente.Attivo == true)
                {
                    this.lblPaziente.Text = oPaziente.Descrizione;
                }
                else
                {
                    this.lblPaziente.Text = "";
                }

                if (oEpisodio != null && oEpisodio.Attivo == true)
                {
                    this.lblNosologico.Text = string.Format("N.: {0}", (oEpisodio.NumeroEpisodio != string.Empty ? oEpisodio.NumeroEpisodio : oEpisodio.NumeroListaAttesa));
                }
                else
                {
                    this.lblNosologico.Text = "";
                }

                if (oMaschera != null)
                {
                    this.lblTitolo.Text = oMaschera.Descrizione;
                }
                else
                {
                    this.lblTitolo.Text = "";
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            base.Refresh();

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

        #endregion

        #region IViewControllerTopNonModale

        public ViewControllerTopNonModale ViewController { get; set; }

        public Maschera Maschera { get; set; }

        public void InitViewController(IViewController viewcontroller)
        {
            this.ViewController = (ViewControllerTopNonModale)viewcontroller;
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

    }
}
