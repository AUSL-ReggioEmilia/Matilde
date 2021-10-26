using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciCore.ViewController;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUSchedaNM : frmBaseNonModale, Interfacce.IViewFormlModal, IViewControllerScheda
    {
        public frmPUSchedaNM()
        {
            InitializeComponent();
        }

        #region Interface

        public string CodiceMaschera { get; set; }

        public void Carica()
        {

            this.LoadViewController();

            this.Show();

        }

                                public Object CustomParamaters { get; set; }

        #endregion

        #region Interface ViewController

        public ViewControllerScheda ViewController { get; set; }

        public Maschera Maschera
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void InitViewController(IViewController viewcontroller)
        {

                                                this.ViewController = (ViewControllerScheda)viewcontroller;

                                                ViewControllerTopNonModale oVCTop = new ViewControllerTopNonModale();
            oVCTop.Maschera = this.ViewController.Maschera;
            oVCTop.Paziente = this.ViewController.Paziente;
            oVCTop.Episodio = this.ViewController.Episodio;
            this.ucTopNonModale.InitViewController(oVCTop);

                                                ViewControllerBottomNonModale oVCBottom = new ViewControllerBottomNonModale();
            oVCBottom.Maschera = this.ViewController.Maschera;
            this.ucBottomNonModale.InitViewController(oVCBottom);

        }

        public void LoadViewController()
        {

                                                this.Text = this.ViewController.Maschera.Descrizione;
            this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_DIARIOMEDICO_16);

                                                this.ucAnteprimaRtf.rtbRichTextBox.ReadOnly = true;
            this.ucAnteprimaRtf.MovScheda = this.ViewController.MovScheda;
            this.ucAnteprimaRtf.MovScheda.GeneraRTF();
            this.ucAnteprimaRtf.RefreshRTF();

        }

        public IViewController SaveViewController()
        {
            this.ViewController.DialogResultReturn = this.DialogResult;
            return this.ViewController;
        }

        #endregion

        #region Events Form

        private void frmPUSchedaNM_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void frmPUSchedaNM_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmPUSchedaNM_Shown(object sender, EventArgs e)
        {
            try
            {
                this.ucAnteprimaRtf.RefreshRTF();
            }
            catch
            {
            }
        }

        #endregion

    }
}
