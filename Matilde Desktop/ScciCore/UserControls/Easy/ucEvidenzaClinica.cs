using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;

namespace UnicodeSrl.ScciCore
{
    public partial class ucEvidenzaClinica : UserControl, Interfacce.IViewUserControlMiddle
    {

        private UserControl _ucc = null;
        private ucRichTextBox _ucRichTextBox = null;

        private Dictionary<int, byte[]> oIcone = new Dictionary<int, byte[]>();

        private bool _disableClick = false;

        private DataTable _dtTipiEvidenzaClinica = null;

        public ucEvidenzaClinica()
        {
            InitializeComponent();

            _ucc = (UserControl)this;
        }

        #region Interface

        public void Aggiorna()
        {

            this.AggiornaGriglia(true);

        }

        public void Carica()
        {
            try
            {
                _disableClick = false;
                InizializzaControlli();
                InizializzaUltraGridLayout();
                VerificaSicurezza();
                InizializzaValoriFiltri();

                CaricaGriglia(true);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }
        }

        public void Ferma()
        {

            try
            {

                oIcone = new Dictionary<int, byte[]>();

                CoreStatics.SetContesto(EnumEntita.EVC, null);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region private functions

        private static bool AltriEpisodiNosologico(string nosologico, string numerolista)
        {
            bool bret = false;
            int nqta = 0;
            try
            {
                Parametri op = new Parametri();

                op.Parametro.Add("NumeroNosologico", Database.testoSQL(nosologico));

                if (
    (numerolista != null && numerolista != string.Empty) &&
    (nosologico == null || nosologico == string.Empty)
   )
                    op.Parametro.Add("NumeroListaAttesa", Database.testoSQL(numerolista));

                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);

                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_ContaEpisodiNosologico", spcoll);

                if (dt.Rows.Count == 1)
                {
                    nqta = System.Convert.ToInt32(dt.Rows[0]["Qta"]);
                    if (nqta > 1)
                        bret = true;
                    else
                        bret = false;
                }
                else
                {
                    bret = false;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
                bret = false;
            }

            return bret;
        }
        private void InizializzaControlli()
        {

            try
            {
                this.uchkFiltro.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTRO_256);
                this.uchkFiltro.CheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTROAPPLICATO_256);
                this.uchkFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.uchkFiltro.Checked = false;

                this.uchkFiltro.PercImageFill = 0.75F;
                this.uchkFiltro.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;

                this.ubApplicaFiltro.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Large;

                this.chkSoloEpisodio.Visible = (CoreStatics.CoreApplication.Episodio == null ? false : true);
                this.chkSoloEpisodio.Checked = (CoreStatics.CoreApplication.Episodio == null ? false : true);
                this.lblSoloEpisodio.Visible = this.chkSoloEpisodio.Visible;


                this.chkSoloDaVistare.Visible = (CoreStatics.CoreApplication.Episodio == null ? false : true);
                this.chkSoloEpisodio.Checked = false;
                this.lblSoloDaVistare.Visible = this.chkSoloDaVistare.Visible;

                bool bSACConsensiAbilita = Convert.ToBoolean(Convert.ToInt32(Database.GetConfigTable(EnumConfigTable.SACConsensiAbilita)));
                if (bSACConsensiAbilita)
                {
                    if (CoreStatics.CoreApplication.Paziente.PazientiConsensiDossier.CodStatoConsenso != EnumStatoConsenso.SI.ToString())
                    {
                        this.lblSoloEpisodio.Enabled = false;
                        this.chkSoloEpisodio.Enabled = false;
                    }
                }


                if (CoreStatics.CoreApplication.Episodio != null)
                {
                    this.drFiltro.DataEpisodio = CoreStatics.CoreApplication.Episodio.DataRicovero;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaControlli", this.Name);
            }
        }

        private void InizializzaUltraGridLayout()
        {
            try
            {
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGrid);
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaUltraGridLayout", this.Name);
            }
        }

        private void VerificaSicurezza()
        {

            try
            {
            }
            catch (Exception)
            {
            }
        }

        private void InizializzaValoriFiltri()
        {
            try
            {
                this.chkSoloDefinitivi.Checked = false;
                this.chkSoloDaVistare.Checked = false;
                this.chkSoloEpisodio.Checked = (CoreStatics.CoreApplication.Episodio == null ? false : true);

                this.drFiltro.Value = null;
                this.udteFiltroDA.Value = null;
                this.udteFiltroA.Value = null;

                if (this.ucEasyGridFiltroTipo.Rows.Count > 0)
                {
                    this.ucEasyGridFiltroTipo.ActiveRow = null;
                    this.ucEasyGridFiltroTipo.Selected.Rows.Clear();
                    CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroTipo, "DescTipo", CoreStatics.GC_TUTTI);
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaValoriFiltri", this.Name);
            }

        }

        private void CaricaGriglia(bool datiEstesi)
        {

            try
            {

                string idSAC = "";
                string idPaziente = "";
                string idEpisodio = "";

                DateTime? dataconsensodossier = null;
                bool bSACConsensiAbilita = false;

                bSACConsensiAbilita = Convert.ToBoolean(Convert.ToInt32(Database.GetConfigTable(EnumConfigTable.SACConsensiAbilita)));

                if (CoreStatics.CoreApplication.Episodio != null) idEpisodio = CoreStatics.CoreApplication.Episodio.ID;
                if (CoreStatics.CoreApplication.Paziente != null)
                {
                    idPaziente = CoreStatics.CoreApplication.Paziente.ID;
                    idSAC = CoreStatics.CoreApplication.Paziente.CodSACFuso;
                }


                if (CoreStatics.CoreApplication.Episodio != null)
                {



                    bool bcontrollaDWHEpisodi = false;

                    if (AltriEpisodiNosologico(CoreStatics.CoreApplication.Episodio.NumeroEpisodio, CoreStatics.CoreApplication.Episodio.NumeroListaAttesa))
                    {
                        bcontrollaDWHEpisodi = this.chkSoloEpisodio.Checked;
                    }

                    if (bSACConsensiAbilita &&
    CoreStatics.CoreApplication.Paziente != null &&
    CoreStatics.CoreApplication.Paziente.PazientiConsensiDossierStorico.CodStatoConsenso != EnumStatoConsenso.SI.ToString())
                    {
                        if (CoreStatics.CoreApplication.Paziente.PazientiConsensiDossier.CodStatoConsenso == EnumStatoConsenso.SI.ToString())
                        {
                            dataconsensodossier = CoreStatics.CoreApplication.Paziente.PazientiConsensiDossier.DataConsenso;
                        }
                    }

                    DataSet ds = DBUtils.getEvidenzaClinicaDataset(true,
                                                                   !this.chkSoloEpisodio.Checked,
                                                                   datiEstesi, datiEstesi,
                                                                   idPaziente,
                                                                   idEpisodio,
                                                                   idSAC,
                                                                   false,
                                                                   false,
                                                                   new List<string>(),
                                                                   DateTime.MinValue,
                                                                   DateTime.MinValue,
                                                                   null,
                                                                   true,
                                                                   bcontrollaDWHEpisodi,
                                                                   dataconsensodossier);

                    this.ucEasyGrid.DataSource = null;

                    this.ucEasyGrid.PerformAction(UltraGridAction.FirstRowInBand, false, false);


                    _dtTipiEvidenzaClinica = ds.Tables[1].Copy();
                    this.ucEasyGridFiltroTipo.DataSource = DBUtils.getDatatableTipiEVCRaggruppato(ref _dtTipiEvidenzaClinica, true);

                    this.ucEasyGridFiltroTipo.Refresh();


                    if (CoreStatics.CoreApplication.Episodio != null) this.drFiltro.Value = ucEasyDateRange.C_RNG_6M;

                    ubApplicaFiltro_Click(this.ubApplicaFiltro, new EventArgs());

                }
                else
                {


                    DateTime datainizio = DateTime.MinValue;
                    DateTime datafine = DateTime.MinValue;
                    bool bFiltro = false;

                    if (int.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroGiorniEvidenzaClinica)) != 0)
                        this.udteFiltroDA.Value = DateTime.Now.AddDays(-int.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroGiorniEvidenzaClinica)));
                    else
                        this.drFiltro.Value = ucEasyDateRange.C_RNG_60G;

                    this.CheckEVC(true);

                    if (this.udteFiltroDA.Value != null)
                    {
                        datainizio = (DateTime)this.udteFiltroDA.Value;
                        bFiltro = true;
                    }
                    if (this.udteFiltroA.Value != null)
                    {
                        datafine = (DateTime)this.udteFiltroA.Value;
                        bFiltro = true;
                    }
                    this.uchkFiltro.Checked = bFiltro;

                    bool bCaricaGriglia = true;

                    if (bSACConsensiAbilita)
                    {
                        bCaricaGriglia = (CoreStatics.CoreApplication.Paziente.PazientiConsensiDossierStorico.CodStatoConsenso == EnumStatoConsenso.SI.ToString());
                    }

                    if (bCaricaGriglia == true)
                    {
                        DataSet ds = DBUtils.getEvidenzaClinicaDataset(false,
                                         true,
                                         false, datiEstesi,
                                         idPaziente,
                                         idEpisodio,
                                         idSAC,
                                         false,
                                         false,
                                         new List<string>(),
                                         datainizio,
                                         datafine, null,
                                         true);

                        this.ucEasyGrid.DataSource = null;
                        this.ucEasyGrid.DataSource = ds.Tables[0].DefaultView;
                        this.ucEasyGrid.Refresh();

                        this.ucEasyGrid.PerformAction(UltraGridAction.FirstRowInBand, false, false);


                        _dtTipiEvidenzaClinica = ds.Tables[1].Copy();
                        this.ucEasyGridFiltroTipo.DataSource = DBUtils.getDatatableTipiEVCRaggruppato(ref _dtTipiEvidenzaClinica, true);

                        this.ucEasyGridFiltroTipo.Refresh();
                    }
                    else
                    {
                        this.ucEasyGrid.DataSource = null;
                        this.ucEasyGrid.Refresh();
                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                CoreStatics.ExGest(ref ex, @"Errore Accesso Data Warehouse." + Environment.NewLine + @"Contattare amministratori di sistema.", "CaricaGriglia", this.Name);
            }
        }

        private void AggiornaGriglia(bool datiEstesi)
        {

            bool bFiltro = false;
            DateTime? dataconsensodossier = null;
            bool bSACConsensiAbilita = false;

            bSACConsensiAbilita = Convert.ToBoolean(Convert.ToInt32(Database.GetConfigTable(EnumConfigTable.SACConsensiAbilita)));

            string desctiposelezionato = string.Empty;

            CoreStatics.SetNavigazione(false);

            this.CheckEVC(false);

            try
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                CoreStatics.SetContesto(EnumEntita.EVC, null);

                DateTime datainizio = DateTime.MinValue;
                DateTime datafine = DateTime.MinValue;
                List<string> tipi = new List<string>();

                if (this.udteFiltroDA.Value != null)
                {
                    datainizio = (DateTime)this.udteFiltroDA.Value;
                    bFiltro = true;
                }
                if (this.udteFiltroA.Value != null)
                {
                    datafine = (DateTime)this.udteFiltroA.Value;
                    bFiltro = true;
                }

                tipi = getActiveRowCodiciTipi();
                if (tipi != null && tipi.Count > 0) bFiltro = true;

                if (this.chkSoloDaVistare.Checked) bFiltro = true;
                if (this.chkSoloDefinitivi.Checked) bFiltro = true;


                this.uchkFiltro.Checked = bFiltro;

                string idSAC = "";
                string idPaziente = "";
                string idEpisodio = "";
                if (CoreStatics.CoreApplication.Episodio != null) idEpisodio = CoreStatics.CoreApplication.Episodio.ID;
                if (CoreStatics.CoreApplication.Paziente != null)
                {
                    idPaziente = CoreStatics.CoreApplication.Paziente.ID;
                    idSAC = CoreStatics.CoreApplication.Paziente.CodSACFuso;
                }

                bool bcontrollaDWHEpisodi = false;

                if ((CoreStatics.CoreApplication.Episodio != null) &&
                    AltriEpisodiNosologico(CoreStatics.CoreApplication.Episodio.NumeroEpisodio, CoreStatics.CoreApplication.Episodio.NumeroListaAttesa))
                {
                    bcontrollaDWHEpisodi = this.chkSoloEpisodio.Checked;
                }

                bool bCaricaGriglia = true;
                if (bSACConsensiAbilita)
                {

                    if (CoreStatics.CoreApplication.Episodio == null)
                    {
                        bCaricaGriglia = (CoreStatics.CoreApplication.Paziente.PazientiConsensiDossierStorico.CodStatoConsenso == EnumStatoConsenso.SI.ToString());
                    }
                    else
                    {
                        if (CoreStatics.CoreApplication.Paziente != null &&
    CoreStatics.CoreApplication.Paziente.PazientiConsensiDossierStorico.CodStatoConsenso != EnumStatoConsenso.SI.ToString())
                        {
                            if (CoreStatics.CoreApplication.Paziente.PazientiConsensiDossier.CodStatoConsenso == EnumStatoConsenso.SI.ToString())
                            {
                                dataconsensodossier = CoreStatics.CoreApplication.Paziente.PazientiConsensiDossier.DataConsenso;
                            }
                        }
                    }
                }

                if (bCaricaGriglia == true)
                {
                    DataSet ds = DBUtils.getEvidenzaClinicaDataset((CoreStatics.CoreApplication.Episodio != null),
                                         !this.chkSoloEpisodio.Checked,
                                         datiEstesi, !this.chkSoloEpisodio.Checked,
                                         idPaziente,
                                         idEpisodio,
                                         idSAC,
                                         this.chkSoloDefinitivi.Checked,
                                         this.chkSoloDaVistare.Checked,
                                         tipi,
                                         datainizio,
                                         datafine,
                                         _dtTipiEvidenzaClinica, true,
                                         bcontrollaDWHEpisodi,
                                         dataconsensodossier
                                         );

                    this.ucEasyGrid.DataSource = null;
                    this.ucEasyGrid.DataSource = ds.Tables[0].DefaultView;
                    this.ucEasyGrid.Refresh();

                    if (datiEstesi || !this.chkSoloEpisodio.Checked)
                    {
                        this.ucEasyGridFiltroTipo.DisplayLayout.Bands[0].Columns.ClearUnbound();
                        if (this.ucEasyGridFiltroTipo.ActiveRow != null && !this.ucEasyGridFiltroTipo.ActiveRow.Cells["DescTipo"].Text.Contains(CoreStatics.GC_TUTTI))
                            desctiposelezionato = this.ucEasyGridFiltroTipo.ActiveRow.Cells["DescTipo"].Text;



                        _dtTipiEvidenzaClinica = ds.Tables[1].Copy();
                        this.ucEasyGridFiltroTipo.DataSource = DBUtils.getDatatableTipiEVCRaggruppato(ref _dtTipiEvidenzaClinica, true);

                        this.ucEasyGridFiltroTipo.Refresh();
                        if (desctiposelezionato != null && desctiposelezionato.Trim() != "" && !desctiposelezionato.Contains(CoreStatics.GC_TUTTI))
                            CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroTipo, "DescTipo", desctiposelezionato);
                    }
                }
                else
                {
                    this.ucEasyGridFiltroTipo.DataSource = null;
                    this.ucEasyGrid.DataSource = null;
                    this.ucEasyGrid.Refresh();
                    this.ucEasyGridFiltroTipo.Refresh();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                CoreStatics.ExGest(ref ex, @"Errore Accesso Data Warehouse." + Environment.NewLine + @"Contattare amministratori di sistema.", "AggiornaGriglia", this.Name);
            }
            finally
            {
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                _disableClick = false;
            }

            CoreStatics.SetNavigazione(true);

        }

        private string ImportDWH(ref DataRow drReferto)
        {
            string retIDSCCI = "";
            try
            {

                bool bImported = false;
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);


                string idEpisodio = "";
                if (CoreStatics.CoreApplication.Episodio != null) idEpisodio = CoreStatics.CoreApplication.Episodio.ID;


                Parametri op = null;
                SqlParameterExt[] spcoll = null;
                string xmlParam = string.Empty;

                op = new Parametri(CoreStatics.CoreApplication.Ambiente); op.Parametro.Add("CodTipoEvidenzaClinicaDWH", drReferto["SistemaEroganteDWH"].ToString()); spcoll = new SqlParameterExt[1];
                xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);
                Database.GetDataTableStoredProc("MSP_ControlloTipoEvidenzaClinica", spcoll);

                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                MovEvidenzaClinica tmpEC = new MovEvidenzaClinica(drReferto["IDRefertoDWH"].ToString(), drReferto["CodTipo"].ToString(), drReferto["DescrTipo"].ToString(), drReferto["CodStato"].ToString(), drReferto["DescrStato"].ToString(), drReferto["Anteprima"].ToString(), (DateTime)drReferto["DataReferto"], (DateTime)drReferto["DataEventoDWH"]);
                op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                op.Parametro.Add("IDEpisodio", idEpisodio);
                op.Parametro.Add("DataEvento", Database.dataOra105PerParametri(tmpEC.DataReferto));
                op.Parametro.Add("DataEventoUTC", Database.dataOra105PerParametri(tmpEC.DataReferto.ToUniversalTime()));
                op.Parametro.Add("CodStatoEvidenzaClinica", tmpEC.CodStatoEvidenzaClinica);
                op.Parametro.Add("CodTipoEvidenzaClinica", tmpEC.CodTipoEvidenzaClinica);
                op.Parametro.Add("CodStatoEvidenzaClinicaVisione", drReferto["Firmato"].ToString());
                op.Parametro.Add("IDRefertoDWH", tmpEC.IDRefertoDWH);

                if (!drReferto.IsNull("NumeroRefertoDWH") && drReferto["NumeroRefertoDWH"].ToString().Trim() != "")
                    op.Parametro.Add("NumeroRefertoDWH", drReferto["NumeroRefertoDWH"].ToString());

                op.Parametro.Add("Anteprima", drReferto["Anteprima"].ToString());
                if (tmpEC.DataEventoDWH != DateTime.MinValue)
                {
                    op.Parametro.Add("DataEventoDWH", Database.dataOra105PerParametri(tmpEC.DataEventoDWH));
                    op.Parametro.Add("DataEventoDWHUTC", Database.dataOra105PerParametri(tmpEC.DataEventoDWH.ToUniversalTime()));
                }

                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                byte[] pdf = tmpEC.PDF;
                if (pdf != null && pdf.Length > 0)
                {
                    op.Parametro.Add("PDFDWH", Convert.ToBase64String(pdf));
                }

                op.TimeStamp.CodAzione = EnumAzioni.MOD.ToString();
                op.TimeStamp.CodEntita = EnumEntita.EVC.ToString();

                spcoll = new SqlParameterExt[1];

                xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_ControlloEvidenzaClinica", spcoll);
                string sRet = "";
                Guid newIdSCCI = Guid.Empty;
                if (dt.Rows.Count == 1)
                {
                    sRet = dt.Rows[0]["Risultato"].ToString();
                    if (sRet.ToUpper().IndexOf("Inserito".ToUpper()) >= 0)
                    {
                        bImported = true;
                        newIdSCCI = new Guid(sRet.ToUpper().Replace(@"Inserito, IDEvidenzaClinica :".ToUpper(), ""));
                        retIDSCCI = newIdSCCI.ToString();

                        op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                        op.Parametro.Add("IDEvidenzaClinica", retIDSCCI);

                        op.TimeStamp.CodAzione = EnumAzioni.MOD.ToString();
                        op.TimeStamp.CodEntita = EnumEntita.EVC.ToString();

                        spcoll = new SqlParameterExt[1];

                        xmlParam = XmlProcs.XmlSerializeToString(op);
                        spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                        Database.GetDataTableStoredProc("MSP_ControlloEvidenzaClinicaCasiParticolari", spcoll);

                    }
                }
                tmpEC = null;


                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                if (bImported && newIdSCCI != Guid.Empty)
                {
                    if (drReferto["Firmato"].ToString() == "DV")
                    {
                        CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = new MovEvidenzaClinica(retIDSCCI, drReferto["IDRefertoDWH"].ToString().Trim(), EnumAzioni.VAL);

                        if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.Vista())
                        {
                            AggiornaGriglia(false);
                            if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato != null)
                            {
                                CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "IDSCCI", CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.IDMovEvidenzaClinica);
                                CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = null;
                            }
                        }
                    }
                    else
                    {
                        AggiornaGriglia(false);
                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "IDSCCI", retIDSCCI);
                        CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = null;
                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ImportDWH", this.Name);
            }
            return retIDSCCI;
        }

        private void CancellaEVC(string IDSCCI, string IDRefertoDWH)
        {

            if (easyStatics.EasyMessageBox("Confermi la cancellazione dell'evidenza clinica selezionata ?", "Cancellazione TEvidenza Clinica", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                MovEvidenzaClinica oMovEvidenzaClinicaSelezionato = new MovEvidenzaClinica(IDSCCI, IDRefertoDWH);
                if (oMovEvidenzaClinicaSelezionato.Cancella())
                {
                    this.AggiornaGriglia(false);
                }
                oMovEvidenzaClinicaSelezionato = null;

            }

        }

        private void CheckEVC(bool bload)
        {

            if (CoreStatics.CoreApplication.Episodio == null)
            {

                if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.LimitaEVCAmbulatoriale == true &&
int.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.GGPregressiEVCConsultazione)) != 0)
                {

                    DateTime datainizio = DateTime.MinValue;
                    DateTime datafine = DateTime.MinValue;

                    int ggconsentiti = -int.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.GGPregressiEVCConsultazione));
                    DateTime dataconsentita = DateTime.Today.Date.AddDays(ggconsentiti);

                    if (this.udteFiltroDA.Value != null)
                    {
                        datainizio = (DateTime)this.udteFiltroDA.Value;
                    }
                    if (this.udteFiltroA.Value != null)
                    {
                        datafine = (DateTime)this.udteFiltroA.Value;
                    }

                    if (datainizio < dataconsentita)
                    {

                        if (!bload)
                        {
                            string sMsg = $"Attenzione: la ricerca dell'evidenza clinica è limitata agli ultimi {ggconsentiti} giorni." +
                                            $"\nSarà applicata la data minima consentita {dataconsentita.ToString("dd/MM/yyy")}.";
                            easyStatics.EasyMessageBox(sMsg, "Applica Filtro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        this.udteFiltroDA.Value = dataconsentita;

                    }

                    if (datafine > DateTime.MinValue && datafine < dataconsentita)
                    {

                        if (!bload)
                        {
                            string sMsg = $"Attenzione: la data fine ha superato il limite consentito di {ggconsentiti} giorni." +
                                        $"\nSarà applicata la data odierna {DateTime.Now.ToString("dd/MM/yyy")}.";
                            easyStatics.EasyMessageBox(sMsg, "Applica Filtro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        this.udteFiltroA.Value = DateTime.Now.Date;

                    }

                }

            }

        }

        #endregion

        #region Nuova Gestione Filtro Tipo

        private List<string> getActiveRowCodiciTipi()
        {
            List<string> lstRet = new List<string>();

            if (this.ucEasyGridFiltroTipo.ActiveRow != null
                && this.ucEasyGridFiltroTipo.ActiveRow.IsDataRow
                && !this.ucEasyGridFiltroTipo.ActiveRow.IsFilteredOut
                && this.ucEasyGridFiltroTipo.ActiveRow.Cells["DescTipo"].Text.Trim().ToUpper() != CoreStatics.GC_TUTTI.Trim().ToUpper()
                && _dtTipiEvidenzaClinica != null
                && _dtTipiEvidenzaClinica.Rows.Count > 0)
            {
                _dtTipiEvidenzaClinica.DefaultView.RowFilter = @"DescTipo = '" + Database.testoSQL(this.ucEasyGridFiltroTipo.ActiveRow.Cells["DescTipo"].Text) + @"'";
                if (_dtTipiEvidenzaClinica.DefaultView.Count > 0)
                {
                    foreach (DataRowView drv in _dtTipiEvidenzaClinica.DefaultView)
                    {
                        if (!lstRet.Contains(drv["CodTipo"].ToString())) lstRet.Add(drv["CodTipo"].ToString());
                    }
                }
                _dtTipiEvidenzaClinica.DefaultView.RowFilter = "";
            }

            return lstRet;
        }

        #endregion

        #region Events

        private void ucEasyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {
                Graphics g = this.CreateGraphics();
                int refWidth = CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 4;

                int refBtnWidth = CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 4;
                g.Dispose();
                g = null;
                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = false;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;

                e.Layout.Override.RowSizing = RowSizing.Fixed;
                e.Layout.Override.RowSizingArea = RowSizingArea.EntireRow;
                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {
                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {


                            case "Icona":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                                oCol.CellAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.VertScrollBar = false;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.LockedWidth = true;
                                try
                                {
                                    oCol.MaxWidth = refWidth;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 3;

                                break;

                            case "DataReferto":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Format = "dd/MM/yyyy";
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 1.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DataEventoDWH":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                oCol.Format = "dd/MM/yyyy HH:mm";
                                try
                                {
                                    oCol.MaxWidth = refWidth * 2;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;


                            case "DescrUtenteVisione":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Middle;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 3.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 1;
                                oCol.RowLayoutColumnInfo.SpanX = 2;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DataVisione":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.FontData.Italic = Infragistics.Win.DefaultableBoolean.True;
                                oCol.CellAppearance.ForeColor = Color.Gray;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Right;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.Format = "(dd/MM/yyyy HH:mm)";
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 3.5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }
                                oCol.RowLayoutColumnInfo.WeightY = 1;
                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 2;
                                oCol.RowLayoutColumnInfo.SpanX = 2;
                                oCol.RowLayoutColumnInfo.SpanY = 1;
                                break;

                            case "Anteprima":
                                oCol.Hidden = false;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - Convert.ToInt32(refWidth * 4.5) - Convert.ToInt32(refBtnWidth * 6.05) - 30;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.OriginX = 3;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 3;

                                break;

                            default:
                                oCol.Hidden = true;
                                break;

                        }
                    }
                    catch (Exception)
                    {
                    }

                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_VALID))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_VALID);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_FIRMA_32);


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 4;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_VALID + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_VALID + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 5;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_VIEW))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_VIEW);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_EVIDENZACLINICA_32);


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 6;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_VIEW + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_VIEW + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 7;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_REFERTPDF))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_REFERTPDF);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_DOWNLOADDOCUMENTO_32);


                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 8;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_REFERTPDF + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_REFERTPDF + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 9;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_GRAPH))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_GRAPH);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_PARAMETRIVITALIGRAFICO_32);

                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 10;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }
                if (!e.Layout.Bands[0].Columns.Exists(@"COLFINE_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(@"COLFINE_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.25);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 11;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_DEL))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_DEL);
                    colEdit.Hidden = false;

                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_ELIMINA_32);

                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = refBtnWidth;
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 12;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }
                if (!e.Layout.Bands[0].Columns.Exists(@"COLDEL_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(@"COLDEL_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
                    colEdit.CellActivation = Activation.Disabled;
                    colEdit.AutoSizeMode = ColumnAutoSizeMode.None;
                    try
                    {
                        colEdit.MinWidth = Convert.ToInt32(refBtnWidth * 0.1);
                        colEdit.MaxWidth = colEdit.MinWidth;
                        colEdit.Width = colEdit.MaxWidth;
                    }
                    catch (Exception)
                    {
                    }
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 13;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 3;
                }

            }
            catch (Exception)
            {
            }
        }

        private void ucEasyGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {

                if (e.Row.Cells.Exists("Icona") && e.Row.Cells.Exists("IDIcona") && e.Row.Cells["IDIcona"].Text.ToString() != string.Empty && e.Row.Cells["IDIcona"].Text.ToString() != "")
                {
                    if (!oIcone.ContainsKey(Convert.ToInt32(e.Row.Cells["IDIcona"].Value)))
                    {
                        oIcone.Add(Convert.ToInt32(e.Row.Cells["IDIcona"].Value), CoreStatics.GetImageForGrid(Convert.ToInt32(e.Row.Cells["IDIcona"].Value), 256));
                    }
                    byte[] icona = oIcone[Convert.ToInt32(e.Row.Cells["IDIcona"].Value)];
                    if (icona != null)
                    {
                        e.Row.Cells["Icona"].Value = icona;
                        e.Row.Update();
                    }
                }

                foreach (UltraGridCell ocell in e.Row.Cells)
                {
                    if (ocell.Column.Key.ToUpper().IndexOf("ICON") == 0 && e.Row.Cells.Exists("DescrTipo"))
                    {
                        ocell.ToolTipText = e.Row.Cells["DescrTipo"].Text;
                        if (e.Row.Cells.Exists("DescrStato")) ocell.ToolTipText += @" - " + e.Row.Cells["DescrStato"].Text;
                    }

                    if (CoreStatics.CoreApplication.Cartella != null && CoreStatics.CoreApplication.Cartella.CartellaChiusa == true)
                    {
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_VALID)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_GRAPH && ocell.Row.Cells["PermessoGrafico"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_DEL)
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                    }
                    else
                    {
                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_VALID && ocell.Row.Cells["PermessoVista"].Value.ToString() == "0")
                        {
                            if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.EvidenzaC_Vista)
                                && CoreStatics.CoreApplication.Episodio != null
                                && ocell.Row.Cells["IDSCCI"].Value.ToString() == "")
                                ocell.ButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_EVIDENZACLINICAIMPORTA_32);
                            else
                                ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                        }

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_GRAPH && ocell.Row.Cells["PermessoGrafico"].Value.ToString() == "0")
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                        if (ocell.Column.Key == CoreStatics.C_COL_BTN_DEL && ocell.Row.Cells["PermessoCancella"].Value.ToString() == "0")
                        {
                            ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_InitializeRow", this.Name);
            }
        }

        private void ucEasyGrid_ClickCellButton(object sender, CellEventArgs e)
        {
            if (!_disableClick)
            {
                try
                {
                    switch (e.Cell.Column.Key)
                    {
                        case CoreStatics.C_COL_BTN_VALID:
                            if (CoreStatics.CoreApplication.Episodio != null && e.Cell.Row.Cells["IDSCCI"].Value.ToString() == "")
                            {
                                _disableClick = true;

                                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);

                                CoreStatics.SetNavigazione(false);

                                DataRow rowDWH = ((DataRowView)e.Cell.Row.ListObject).Row;

                                ImportDWH(ref rowDWH);

                                CoreStatics.SetNavigazione(true);

                                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);


                            }
                            else
                            {

                                if (e.Cell.Row.Cells["PermessoVista"].Text == "1")
                                {
                                    _disableClick = true;


                                    CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = new MovEvidenzaClinica(e.Cell.Row.Cells["IDSCCI"].Text, e.Cell.Row.Cells["IDRefertoDWH"].Text.Trim(), EnumAzioni.VAL);



                                    if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.Vista())
                                    {
                                        AggiornaGriglia(false);
                                        if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato != null)
                                        {
                                            CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "IDSCCI", CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.IDMovEvidenzaClinica);
                                            CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = null;
                                        }
                                    }

                                }
                            }

                            break;

                        case CoreStatics.C_COL_BTN_VIEW:


                            if (e.Cell.Row.Cells["PermessoGrafico"].Text == "1")
                            {
                                string idEpisodio = (CoreStatics.CoreApplication.Episodio != null ? CoreStatics.CoreApplication.Episodio.ID : "");
                                DateTime datada = (this.udteFiltroDA.Value != null ? (DateTime)this.udteFiltroDA.Value : DateTime.MinValue);
                                DateTime dataa = (this.udteFiltroA.Value != null ? (DateTime)this.udteFiltroA.Value : DateTime.MinValue);
                                DateTime dataricovero = (CoreStatics.CoreApplication.Episodio != null ? CoreStatics.CoreApplication.Episodio.DataRicovero : DateTime.MinValue);


                                dataa = DateTime.MinValue;
                                datada = DateTime.MinValue;

                                CoreStatics.CoreApplication.DefinizioneGraficoSelezionata = new ToolboxPerGrafici(idEpisodio, dataricovero, CoreStatics.CoreApplication.Paziente.CodSAC, EnumEntita.EVC, "", datada, dataa);
                            }

                            if (e.Cell.Row.Cells["IDSCCI"].Text.Trim() != "")
                            {
                                _disableClick = true;
                                CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = new MovEvidenzaClinica(e.Cell.Row.Cells["IDSCCI"].Text, e.Cell.Row.Cells["IDRefertoDWH"].Text.Trim(), EnumAzioni.VIS);

                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingEvidenzaClinica) == DialogResult.OK)
                                {
                                    AggiornaGriglia(false);
                                    if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato != null)
                                    {
                                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "IDSCCI", CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.IDMovEvidenzaClinica);
                                    }
                                }
                            }
                            else
                            {


                                _disableClick = true;
                                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                                CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = new MovEvidenzaClinica(e.Cell.Row.Cells["IDRefertoDWH"].Text, e.Cell.Row.Cells["CodTipo"].Text, e.Cell.Row.Cells["DescrTipo"].Text, e.Cell.Row.Cells["CodStato"].Text, e.Cell.Row.Cells["DescrStato"].Text, e.Cell.Row.Cells["Anteprima"].Text, (DateTime)e.Cell.Row.Cells["DataReferto"].Value, (DateTime)e.Cell.Row.Cells["DataEventoDWH"].Value);
                                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingEvidenzaClinica);

                            }
                            CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = null;
                            CoreStatics.CoreApplication.DefinizioneGraficoSelezionata = null;
                            break;

                        case CoreStatics.C_COL_BTN_REFERTPDF:
                            if (e.Cell.Row.Cells["IDSCCI"].Text.Trim() != "")
                            {
                                _disableClick = true;
                                CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = new MovEvidenzaClinica(e.Cell.Row.Cells["IDSCCI"].Text, e.Cell.Row.Cells["IDRefertoDWH"].Text.Trim(), EnumAzioni.VIS);
                            }
                            else
                            {
                                _disableClick = true;
                                CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = new MovEvidenzaClinica(e.Cell.Row.Cells["IDRefertoDWH"].Text, e.Cell.Row.Cells["CodTipo"].Text, e.Cell.Row.Cells["DescrTipo"].Text, e.Cell.Row.Cells["CodStato"].Text, e.Cell.Row.Cells["DescrStato"].Text, e.Cell.Row.Cells["Anteprima"].Text, (DateTime)e.Cell.Row.Cells["DataReferto"].Value, (DateTime)e.Cell.Row.Cells["DataEventoDWH"].Value);
                            }
                            if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.AbilitaAperturaPDF)
                            {
                                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.WaitCursor);
                                string sreftemp = System.IO.Path.Combine(FileStatics.GetSCCITempPath(), "referto" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + @".pdf");
                                byte[] pdf = CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.PDF;
                                if (pdf == null || pdf.Length <= 0)
                                {
                                    CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                                    easyStatics.EasyMessageBox("Documento non presente.", "Apertura Referto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    if (UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(sreftemp, ref pdf))
                                    {
                                        if (System.IO.File.Exists(sreftemp))
                                        {
                                            bool bAbilitaVisto = false;
                                            if (e.Cell.Row.Cells["PermessoVista"].Text == "1") bAbilitaVisto = true;

                                            easyStatics.ShellExecute(sreftemp, "", false, string.Empty, bAbilitaVisto);

                                            if (bAbilitaVisto)
                                            {
                                                if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.CodStatoEvidenzaClinicaVisione == EnumStatoEvidenzaClinicaVisione.VS.ToString())
                                                {
                                                    AggiornaGriglia(false);
                                                    if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato != null)
                                                    {
                                                        CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "IDSCCI", CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.IDMovEvidenzaClinica);
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                            }
                            CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato = null;
                            break;

                        case CoreStatics.C_COL_BTN_GRAPH:
                            if (e.Cell.Row.Cells["PermessoGrafico"].Text == "1")
                            {

                                _disableClick = true;
                                string idEpisodio = (CoreStatics.CoreApplication.Episodio != null ? CoreStatics.CoreApplication.Episodio.ID : "");
                                DateTime datada = (this.udteFiltroDA.Value != null ? (DateTime)this.udteFiltroDA.Value : DateTime.MinValue);
                                DateTime dataa = (this.udteFiltroA.Value != null ? (DateTime)this.udteFiltroA.Value : DateTime.MinValue);
                                DateTime dataricovero = (CoreStatics.CoreApplication.Episodio != null ? CoreStatics.CoreApplication.Episodio.DataRicovero : DateTime.MinValue);

                                dataa = DateTime.MinValue;
                                datada = DateTime.MinValue;

                                CoreStatics.CoreApplication.DefinizioneGraficoSelezionata = new ToolboxPerGrafici(idEpisodio, dataricovero, CoreStatics.CoreApplication.Paziente.CodSAC, EnumEntita.EVC, "", datada, dataa);

                                CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.GraficiEvidenzaClinica);


                            }
                            break;

                        case CoreStatics.C_COL_BTN_DEL:
                            if (e.Cell.Row.Cells["PermessoCancella"].Text == "1")
                            {
                                _disableClick = true;
                                this.CancellaEVC(e.Cell.Row.Cells["IDSCCI"].Text, e.Cell.Row.Cells["IDRefertoDWH"].Text.Trim());
                            }
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCellButton", this.Name);
                }
                CoreStatics.impostaCursore(ref _ucc, enum_app_cursors.DefaultCursor);
                CoreStatics.SetNavigazione(true);
                _disableClick = false;
            }
        }

        private void ucEasyGrid_ClickCell(object sender, ClickCellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case "Anteprima":
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                        _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Text, false);

                        Infragistics.Win.UIElement uie = e.Cell.GetUIElement();
                        Point oPoint = new Point(uie.Rect.Left, uie.Rect.Top);

                        this.UltraPopupControlContainer.Show(this.ucEasyGrid, oPoint);
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCell", this.Name);
            }

        }

        private void ucEasyGrid_AfterRowActivate(object sender, EventArgs e)
        {
            CoreStatics.SetContesto(EnumEntita.EVC, this.ucEasyGrid.ActiveRow);
        }

        private void ucEasyGridFiltroTipo_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderVisible = false;
            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                switch (oCol.Key)
                {
                    case "DescTipo":
                        oCol.Hidden = false;
                        oCol.Header.Caption = @"Tipo";
                        break;
                    default:
                        oCol.Hidden = true;
                        break;
                }
            }

        }

        private void drFiltro_ValueChanged(object sender, EventArgs e)
        {
            this.udteFiltroDA.Value = drFiltro.DataOraDa;
            this.udteFiltroA.Value = drFiltro.DataOraA;
        }

        private void ubApplicaFiltro_Click(object sender, EventArgs e)
        {
            if (!this.chkSoloEpisodio.Checked && this.udteFiltroDA.Value == null && this.udteFiltroA.Value == null)
                easyStatics.EasyMessageBox("Il filtro non è applicabile: è necessario definire un intervallo di date",
                    "Errore Applica Filtro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                this.AggiornaGriglia(false);
        }

        private void uchkFiltro_Click(object sender, EventArgs e)
        {
            if (!this.uchkFiltro.Checked)
            {
                this.InizializzaValoriFiltri();
                this.AggiornaGriglia(false);
            }
            else
                this.uchkFiltro.Checked = !this.uchkFiltro.Checked;
        }

        private void ucEvidenzaClinica_Enter(object sender, EventArgs e)
        {
            try
            {
                this.ucEasyGrid.PerformLayout();
            }
            catch (Exception)
            {
            }
        }

        private void chkSoloEpisodio_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.chkSoloEpisodio.Checked && (this.udteFiltroDA.Value == null || this.udteFiltroA.Value == null))
            {
                this.drFiltro.Value = ucEasyDateRange.C_RNG_6M;
            }
        }

        #endregion

        #region UltraPopupControlContainer

        private void UltraPopupControlContainer_Closed(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick -= ucRichTextBox_Click;
        }

        private void UltraPopupControlContainer_Opened(object sender, EventArgs e)
        {
            _ucRichTextBox.RtfClick += ucRichTextBox_Click;
        }

        private void UltraPopupControlContainer_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;
            popup.PopupControl = _ucRichTextBox;
        }

        private void ucRichTextBox_Click(object sender, EventArgs e)
        {
        }

        #endregion

    }
}
