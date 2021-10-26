using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using UnicodeSrl.Scci.Enums;

namespace UnicodeSrl.ScciCore
{
    public partial class frmRichiediPassword : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmRichiediPassword()
        {
            InitializeComponent();
        }

        #region INTERFACCIA

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;

                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_LUCCHETTOCHIUSO_256);
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_LUCCHETTOCHIUSO_256);

                this.ucEasyLabel.Text = string.Format("Inserire la password per entrare con l'utente\n{0} ({1})\n\nPremere il tasto ESCI per chiudere il programma.",
                                                        CoreStatics.CoreApplication.Sessione.Utente.Codice,
                                                        CoreStatics.CoreApplication.Sessione.Utente.Descrizione);

                this.ucEasyTextBoxPassword.Focus();

                TopMost = true;

                this.ShowDialog();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }

        }

        #endregion

        #region EVENTI

        private void frmRichiediPassword_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
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
                CoreStatics.ExGest(ref ex, "frmRichiediPassword_PulsanteAvantiClick", this.Name);
            }

        }

        private void frmRichiediPassword_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ucEasyTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    frmRichiediPassword_PulsanteAvantiClick(this, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti));
            }
            catch (Exception)
            {
            }
        }

        #endregion

    }
}
