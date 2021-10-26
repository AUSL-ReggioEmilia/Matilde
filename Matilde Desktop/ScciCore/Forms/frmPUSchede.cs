using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static UnicodeSrl.ScciCore.Interfacce;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUSchede : frmBaseModale, IViewFormlModal
    {
        public frmPUSchede()
        {
            InitializeComponent();
        }

        public new void Carica()
        {
            this.PulsanteIndietroVisibile = false;
            this.PulsanteAvantiVisibile = true ;

            this.PulsanteAvantiTesto = "CHIUDI";

            this.PulsanteAvantiClick += cmdClick;

                        if ( (this.CustomParamaters != null ) && (this.CustomParamaters is List<String>) )
            {
                this.schede.XpFiltro_CodScheda = this.CustomParamaters as List<String>;
            }

            this.schede.Carica();

            this.ShowDialog();
        }


        private void cmdClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }



    }
}
