using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciCore
{
    public partial class frmConnettivita : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmConnettivita()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {
            try
            {

                this.Text = "Connettività";

                this.ucEasyLabel.Text = "Assenza di connettività!" + Environment.NewLine + "Attendere...";
                this.ucEasyPictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_CONNESSONO_256);

                this.PulsanteIndietroVisibile = false;
                this.PulsanteAvantiVisibile = false;

                TimerConnettivita.Enabled = true;
                                this.ShowDialog();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }
        }

        #endregion

        #region Events override

        public override void Refresh()
        {

            try
            {
                System.Windows.Forms.Application.DoEvents();

                
                try
                {

                    if (Database.GetConnettivita() == true)
                    {
                        if (Database.GetConnettivitaDB() == true)
                        {
                            TimerConnettivita.Enabled = false;
                            this.DialogResult = System.Windows.Forms.DialogResult.OK;
                            this.Close();
                        }
                    }
                }
                catch (Exception)
                {

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

            base.Refresh();

        }

        #endregion

        #region Events

        private void TimerConnettivita_Tick(object sender, EventArgs e)
        {
            this.Refresh();
        }

        #endregion

    }
}
