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
using System.DirectoryServices.AccountManagement; 
namespace UnicodeSrl.ScciCore
{
    public partial class frmLoginConsulente : frmBaseModale, Interfacce.IViewFormlModal
    {

        private bool _activated = false;

        public frmLoginConsulente()
        {
            InitializeComponent();
        }

        #region INTERFACCIA

        public void Carica()
        {
            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_CONSULENZA_16);
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_CONSULENZA_256);
                this.ucEasyComboEditorDominio.DataSource = UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.DominioPredefinito).Split(';');
                this.ucEasyComboEditorDominio.Value = CoreStatics.CoreApplication.Sessione.Utente.Codice.Split('\\')[0];

                loadCombo();
                if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                {
                    this.PulsanteAvantiAbilitato = false;
                }
                _activated = false;
                this.ShowDialog();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }
        }

        #endregion

        #region PRIVATE

        private void loadCombo()
        {
            try
            {
                
                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("CodUA", CoreStatics.CoreApplication.Consulenza.Trasferimento.CodUA);
                op.Parametro.Add("CodRuolo", Database.GetConfigTable(EnumConfigTable.RuoloConsulente));
                op.Parametro.Add("CodAzione", EnumAzioni.INS.ToString());
                op.Parametro.Add("DatiEstesi", "0");


                SqlParameterExt[] spcoll = new SqlParameterExt[1];
                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                DataTable dt = Database.GetDataTableStoredProc("MSP_SelTipoDiarioClinico", spcoll);

                if (dt != null)
                {

                    this.uceTipoConsulenza.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
                    this.uceTipoConsulenza.ValueMember = "CodVoce";
                    this.uceTipoConsulenza.DisplayMember = "Descrizione";
                    this.uceTipoConsulenza.DataSource = dt;
                    this.uceTipoConsulenza.Refresh();
                                    }

            }
            catch (Exception)
            {
                throw;
            }
        }

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
                    if (this.uceTipoConsulenza.SelectedItem == null)
                    {
                        easyStatics.EasyMessageBox("Inserire " + this.ucEasyLabelTipoCons.Text + @"!", "Login", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        bContinue = false;
                        this.uceTipoConsulenza.Focus();
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

        #region EVENTI

        private void frmLoginConsulente_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            string codlogin = "";
            if (this.ucEasyComboEditorDominio.Text.Trim() != "") codlogin += this.ucEasyComboEditorDominio.Text.Trim() + @"\";
            codlogin += this.ucEasyTextBoxUtente.Text.Trim();
            string fullName = "";

            try
            {

                if (ValidateUserName(out fullName))
                {

                                        Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("CodLogin", codlogin);
                    op.Parametro.Add("CodRuolo", Database.GetConfigTable(EnumConfigTable.RuoloConsulente));
                    op.Parametro.Add("Descrizione", fullName);
                    op.Parametro.Add("InserisciSeMancante", "1");
                                        SqlParameterExt[] spcoll = new SqlParameterExt[1];
                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                    DataTable dt = Database.GetDataTableStoredProc("MSP_ControlloConsulente", spcoll);

                                        CoreStatics.CoreApplication.Consulenza.Consulente = new Utente(codlogin);
                    if (CoreStatics.CoreApplication.Consulenza.Consulente.Abilitato)
                    {

                        var item = CoreStatics.CoreApplication.Consulenza.Consulente.Ruoli.Elementi.Single(Ruolo => Ruolo.Codice == Database.GetConfigTable(EnumConfigTable.RuoloConsulente));
                        if (item != null)
                        {
                            CoreStatics.CoreApplication.Consulenza.Consulente.Ruoli.RuoloSelezionato = item;
                        }

                                                string codVoce = this.uceTipoConsulenza.SelectedItem.DataValue.ToString();
                        string voceDescrizione = this.uceTipoConsulenza.SelectedItem.DisplayText.ToString();
                        string codScheda = ((DataRowView)this.uceTipoConsulenza.SelectedItem.ListObject)["CodScheda"].ToString();

                                                MovDiarioClinico movdc = new MovDiarioClinico(CoreStatics.CoreApplication.Consulenza.Trasferimento.CodUA, CoreStatics.CoreApplication.Consulenza.Paziente.ID, CoreStatics.CoreApplication.Consulenza.Episodio.ID, CoreStatics.CoreApplication.Consulenza.Trasferimento.ID, CoreStatics.CoreApplication.Consulenza.Ambiente);
                        movdc.Azione = EnumAzioni.INS;
                        movdc.DataEvento = DateTime.Now;
                                                movdc.CodTipoVoceDiario = codVoce;
                        movdc.DescrTipoVoceDiario = voceDescrizione;
                        movdc.CodScheda = codScheda;
                                                CoreStatics.CoreApplication.Consulenza.MovDiarioClinicoConsulenza = movdc;

                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                        this.Close();

                    }
                    else
                    {
                        easyStatics.EasyMessageBox($"Utente {codlogin} NON abilitato!!!", "Login Errato", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmLoginConsulente_PulsanteAvantiClick", this.Name);
            }

        }

        private void frmLoginConsulente_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void frmLoginConsulente_Activated(object sender, EventArgs e)
        {
            try
            {
                if (!_activated)
                {
                    _activated = true;
                    this.ucEasyTextBoxUtente.Focus();
                }
            }
            catch (Exception)
            {
            }
        }

        private void ucEasyTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    frmLoginConsulente_PulsanteAvantiClick(this, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti));
            }
            catch (Exception)
            {
            }
        }

        #endregion

    }
}
