using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.ScciResource;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.ScciCore
{
    public partial class frmCambiaUtente : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmCambiaUtente()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {
            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_LOGIN_256);
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_LOGIN_16);
                this.ucEasyComboEditorDominio.DataSource = UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.DominioPredefinito).Split(';');
                this.ucEasyComboEditorDominio.Value = CoreStatics.CoreApplication.Sessione.Utente.Codice.Split('\\')[0];                

                this.ShowDialog();
                if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
                {

                    CoreStatics.CoreApplication.Paziente = null;
                    CoreStatics.CoreApplication.Episodio = null;
                    CoreStatics.CoreApplication.Trasferimento = null;
                    CoreStatics.CoreApplication.Cartella = null;                    

                                        CoreStatics.CoreApplication.Navigazione.Maschere.CloseAllForm();

                    CoreStatics.CoreApplication.Navigazione.Maschere.Reset();
                    CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.MenuPrincipale);

                }

            }
            catch (Exception)
            {
                this.ImpostaCursore(Scci.Enums.enum_app_cursors.DefaultCursor);
            }

        }

        #endregion

        #region Subroutine

        private bool ValidateUserName(out string fullName)
        {
            bool flag = false;
            fullName = "";
            try
            {
                bool bContinue = true;

                if (bContinue)
                {
                    if (this.ucEasyTextBoxUtente.Text.Trim() == "")
                    {
                        easyStatics.EasyMessageBox("Inserire " + this.ucEasyLabelUtente.Text + @"!", "Login", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        bContinue = false;
                        this.ucEasyTextBoxUtente.Focus();
                    }
                }

                if (bContinue)
                {
                    if (this.ucEasyTextBoxPassword.Text.Trim() == "")
                    {
                        easyStatics.EasyMessageBox("Inserire " + this.ucEasyLabelPassword.Text + @"!", "Password", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        bContinue = false;
                        this.ucEasyTextBoxPassword.Focus();
                    }
                }

                if (bContinue)
                {
                    if (this.ucEasyComboEditorDominio.Text.Trim() == "")
                    {
                        easyStatics.EasyMessageBox("Inserire " + this.ucEasyLabelDominio.Text + @"!", "Dominio", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        bContinue = false;
                        this.ucEasyComboEditorDominio.Focus();
                    }
                }

                if (bContinue)
                {
                    this.ImpostaCursore(Scci.Enums.enum_app_cursors.WaitCursor);
                    string messaggioerrore = "";
                    if (easyStatics.ValidaLogin(this.ucEasyTextBoxUtente.Text.Trim(), this.ucEasyTextBoxPassword.Text.Trim(), this.ucEasyComboEditorDominio.Text.Trim(), out fullName, out messaggioerrore))
                    {
                        flag = true;
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
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ValidateUserName", this.Name);
            }
            this.ImpostaCursore(Scci.Enums.enum_app_cursors.DefaultCursor);
            return flag;
        }

        #endregion

        #region Events

        private void frmCambiaUtente_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {

            string codlogin = "";
            if (this.ucEasyComboEditorDominio.Text.Trim() != "") codlogin += this.ucEasyComboEditorDominio.Text.Trim() + @"\";
            codlogin += this.ucEasyTextBoxUtente.Text.Trim();
            string fullName = "";

            try
            {

                if (ValidateUserName(out fullName))
                {

                    CoreStatics.CoreApplication.Sessione.Utente = new Utente(codlogin);
                    if (CoreStatics.CoreApplication.Sessione.Utente.Abilitato == false)
                    {
                        MessageBox.Show("Utente NON abilitato!", "Scci Easy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        System.Environment.Exit(0);
                    }
                    if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato != null)
                    {
                        CoreStatics.CoreApplication.Ambiente.Codruolo = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice;
                    }
                    CoreStatics.CoreApplication.Sessione.Utente.SbloccaTutto();
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();

                }

            }
            catch (Exception ex)
            {
                                CoreStatics.ExGest(ref ex, "frmCambiaUtente_PulsanteAvantiClick", this.Name);
            }
            finally
            {
                this.ImpostaCursore(Scci.Enums.enum_app_cursors.DefaultCursor);
            }

        }

        private void frmCambiaUtente_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
