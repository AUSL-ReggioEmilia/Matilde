using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciCore.WebSvc;
using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciCore
{
    public partial class frmRichiestaConsenso : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmRichiestaConsenso()
        {
            InitializeComponent();
        }

        #region Declare 

        private bool bEventEnabled = true;

        #endregion

        #region Interface

        public new void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;

                this.Icon = Risorse.GetIconFromResource(Risorse.GC_MODIFICACHECK_256);
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICACHECK_256);

                
                                switch ((EnumStatoConsenso)Enum.Parse(typeof(EnumStatoConsenso), CoreStatics.CoreApplication.Paziente.PazientiConsensiGenerico.CodStatoConsenso))
                {
                    case EnumStatoConsenso.ND:
                        this.chkGenerico.Enabled = true;
                        this.chkGenerico.Checked = false;
                        break;
                    case EnumStatoConsenso.NO:
                        this.chkGenerico.Enabled = false;
                        this.chkGenerico.Checked = false;
                        this.chkGenerico.Appearance.ImageBackground = Risorse.GetImageFromResource(Risorse.GC_XCHECKED_48);
                        break;
                    case EnumStatoConsenso.SI:
                        this.chkGenerico.Enabled = false;
                        this.chkGenerico.Checked = true;
                        break;
                }
                if (CoreStatics.CoreApplication.Paziente.PazientiConsensiGenerico.DataConsenso != DateTime.MinValue)
                {
                    this.lblGenericoData.Text = string.Format("{0:dd/MM/yyyy HH:mm}", CoreStatics.CoreApplication.Paziente.PazientiConsensiGenerico.DataConsenso);
                }

                                switch ((EnumStatoConsenso)Enum.Parse(typeof(EnumStatoConsenso), CoreStatics.CoreApplication.Paziente.PazientiConsensiDossier.CodStatoConsenso))
                {
                    case EnumStatoConsenso.ND:
                        this.chkDossier.Enabled = (CoreStatics.CoreApplication.Paziente.PazientiConsensiGenerico.CodStatoConsenso != EnumStatoConsenso.NO.ToString());
                        this.chkDossier.Checked = false;
                        break;
                    case EnumStatoConsenso.NO:                        
                        this.chkDossier.Enabled = false;
                        this.chkDossier.Checked = false;                        
                        this.chkDossier.Appearance.ImageBackground = Risorse.GetImageFromResource(Risorse.GC_XCHECKED_48);
                        break;
                    case EnumStatoConsenso.SI:
                        this.chkDossier.Enabled = false;
                        this.chkDossier.Checked = true;
                        break;
                }
                if (CoreStatics.CoreApplication.Paziente.PazientiConsensiDossier.DataConsenso != DateTime.MinValue)
                {
                    this.lblDossierData.Text = string.Format("{0:dd/MM/yyyy HH:mm}", CoreStatics.CoreApplication.Paziente.PazientiConsensiDossier.DataConsenso);
                }

                                switch ((EnumStatoConsenso)Enum.Parse(typeof(EnumStatoConsenso), CoreStatics.CoreApplication.Paziente.PazientiConsensiDossierStorico.CodStatoConsenso))
                {
                    case EnumStatoConsenso.ND:
                        this.chkDossierStorico.Enabled = (CoreStatics.CoreApplication.Paziente.PazientiConsensiDossier.CodStatoConsenso != EnumStatoConsenso.NO.ToString());
                        this.chkDossierStorico.Checked = false;
                        break;
                    case EnumStatoConsenso.NO:
                        this.chkDossierStorico.Enabled = false;
                        this.chkDossierStorico.Checked = false;
                        this.chkDossierStorico.Appearance.ImageBackground = Risorse.GetImageFromResource(Risorse.GC_XCHECKED_48);
                        break;
                    case EnumStatoConsenso.SI:
                        this.chkDossierStorico.Enabled = false;
                        this.chkDossierStorico.Checked = true;
                        break;
                }
                if (CoreStatics.CoreApplication.Paziente.PazientiConsensiDossierStorico.DataConsenso != DateTime.MinValue)
                {
                    this.lblDossierStoricoData.Text = string.Format("{0:dd/MM/yyyy HH:mm}", CoreStatics.CoreApplication.Paziente.PazientiConsensiDossierStorico.DataConsenso);
                }

                this.ShowDialog();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }

        }

        #endregion

        #region Subroutine

        private string RichiediConsenso()
        {

            string sret = string.Empty;

            SvcConsensiSAC.ScciConsensiSACClient sac = null;

            try
            {

                                sac = ScciSvcRef.GetSvcConsensiSAC();

                                if (this.chkGenerico.Checked == true && this.chkGenerico.Enabled == true && sret == string.Empty)
                {
                    sret = sac.AggiungiConsenso(CoreStatics.CoreApplication.Paziente.CodSACFuso,
                                                CoreStatics.CoreApplication.Paziente.Cognome, CoreStatics.CoreApplication.Paziente.Nome, CoreStatics.CoreApplication.Paziente.CodiceFiscale,
                                                CoreStatics.CoreApplication.Sessione.Utente.Codice, CoreStatics.CoreApplication.Sessione.Utente.Cognome, CoreStatics.CoreApplication.Sessione.Utente.Nome,
                                                CoreStatics.CoreApplication.Sessione.Computer.Nome,
                                                Scci.Enums.EnumTipoConsenso.Generico.ToString());
                }

                                if (this.chkDossier.Checked == true && this.chkDossier.Enabled == true && sret == string.Empty)
                {
                    sret = sac.AggiungiConsenso(CoreStatics.CoreApplication.Paziente.CodSACFuso,
                                                CoreStatics.CoreApplication.Paziente.Cognome, CoreStatics.CoreApplication.Paziente.Nome, CoreStatics.CoreApplication.Paziente.CodiceFiscale,
                                                CoreStatics.CoreApplication.Sessione.Utente.Codice, CoreStatics.CoreApplication.Sessione.Utente.Cognome, CoreStatics.CoreApplication.Sessione.Utente.Nome,
                                                CoreStatics.CoreApplication.Sessione.Computer.Nome,
                                                Scci.Enums.EnumTipoConsenso.Dossier.ToString());
                }

                                if (this.chkDossierStorico.Checked == true && this.chkDossierStorico.Enabled == true && sret == string.Empty)
                {
                    sret = sac.AggiungiConsenso(CoreStatics.CoreApplication.Paziente.CodSACFuso,
                                                CoreStatics.CoreApplication.Paziente.Cognome, CoreStatics.CoreApplication.Paziente.Nome, CoreStatics.CoreApplication.Paziente.CodiceFiscale,
                                                CoreStatics.CoreApplication.Sessione.Utente.Codice, CoreStatics.CoreApplication.Sessione.Utente.Cognome, CoreStatics.CoreApplication.Sessione.Utente.Nome,
                                                CoreStatics.CoreApplication.Sessione.Computer.Nome,
                                                Scci.Enums.EnumTipoConsenso.DossierStorico.ToString());
                }

            }
            catch (Exception ex)
            {
                sret = ex.Message;
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
            finally
            {
                if (sac != null) { sac.Close(); }
                sac = null;
            }

            return sret;

        }

        #endregion

        #region Events Form

        private void frmRichiestaConsenso_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {

            try
            {

                if (this.chkDossier.Checked == false && this.chkDossierStorico.Checked == false)
                {
                    easyStatics.EasyMessageBox("Selezionare almeno un consenso!", "Selezione Consensi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    this.ImpostaCursore(Scci.Enums.enum_app_cursors.WaitCursor);
                    string messaggioerrore = this.RichiediConsenso();
                    this.ImpostaCursore(Scci.Enums.enum_app_cursors.DefaultCursor);

                    if (messaggioerrore == string.Empty)
                    {
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    }
                    else
                    {
                        easyStatics.EasyMessageBox(messaggioerrore, "Consenso Errato", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    }
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "frmRichiestaConsenso_PulsanteAvantiClick", this.Name);
            }

        }

        private void frmRichiestaConsenso_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Events

        private void chkGenerico_CheckedChanged(object sender, EventArgs e)
        {
            if (bEventEnabled)
            {
                bEventEnabled = false;
                this.chkDossier.Checked = false;
                this.chkDossierStorico.Checked = false;
                bEventEnabled = true;
            }
        }

        private void chkDossier_CheckedChanged(object sender, EventArgs e)
        {
            if (bEventEnabled)
            {
                bEventEnabled = false;
                this.chkGenerico.Checked = true;
                this.chkDossierStorico.Checked = false;
                bEventEnabled = true;
            }
        }

        private void chkDossierStorico_CheckedChanged(object sender, EventArgs e)
        {
            if (bEventEnabled)
            {
                bEventEnabled = false;
                this.chkGenerico.Checked = true;
                this.chkDossier.Checked = true;
                bEventEnabled = true;
            }
        }

        #endregion

    }
}
