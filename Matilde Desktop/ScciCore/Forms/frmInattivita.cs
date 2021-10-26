using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.ScciResource;
using UnicodeSrl.Scci;
using UnicodeSrl.ScciCore;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Framework.Data;

namespace UnicodeSrl.ScciCore
{
    public partial class frmInattivita : frmBaseModale, Interfacce.IViewFormlModal
    {

        public frmInattivita()
        {
            InitializeComponent();
        }

        #region INTERFACCIA

        public void Carica()
        {
            try
            {

                this.Text = "Inattività";
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_LUCCHETTOCHIUSO_256);
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSO_256);

                this.ucEasyLabel.Text = string.Format("Il programma è bloccato dall'utente\n{0} ({1})\n\nInserire la password per sbloccarlo oppure\npremere il tasto ESCI per chiudere il programma.",
                                                        CoreStatics.CoreApplication.Sessione.Utente.Codice,
                                                        CoreStatics.CoreApplication.Sessione.Utente.Descrizione);

                this.ucEasyTextBoxPassword.Focus();

                this.PulsanteAvantiVisibile = true;
                this.PulsanteAvantiTesto = "SBLOCCA";
                this.PulsanteIndietroVisibile = true;
                this.PulsanteIndietroTesto = "ESCI";

                                this.ShowDialog();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }
        }

        #endregion

        #region EVENTI

        private void frmInattivita_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {

            try
            {

                this.ImpostaCursore(Scci.Enums.enum_app_cursors.WaitCursor);
                string fullName = "";
                string messaggioerrore = "";
                if (easyStatics.ValidaLogin(CoreStatics.CoreApplication.Sessione.Utente.Codice.Split('\\')[1],
                                            this.ucEasyTextBoxPassword.Text.Trim(),
                                            CoreStatics.CoreApplication.Sessione.Utente.Codice.Split('\\')[0],
                                            out fullName, out messaggioerrore))
                {
                    this.ImpostaCursore(Scci.Enums.enum_app_cursors.DefaultCursor);
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
                else
                {
                    this.ImpostaCursore(Scci.Enums.enum_app_cursors.DefaultCursor);
                    if (messaggioerrore != null && messaggioerrore.Trim() != "")
                        messaggioerrore = @"Le informazioni di login non sono corrette!" + Environment.NewLine + @"[" + messaggioerrore + @"]";
                    else
                        messaggioerrore = @"Le informazioni di login non sono corrette!";
                    easyStatics.EasyMessageBox(messaggioerrore, "Login Errato", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    this.ucEasyTextBoxPassword.Focus();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmInattivita_PulsanteAvantiClick", this.Name);
            }

        }

        private void frmInattivita_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ucEasyTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    frmInattivita_PulsanteAvantiClick(this, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti));
            }
            catch (Exception)
            {
            }
        }

        #endregion

    }
}
