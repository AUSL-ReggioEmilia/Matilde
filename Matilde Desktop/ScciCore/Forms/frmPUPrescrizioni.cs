using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.Scci.PluginClient;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUPrescrizioni : frmBaseModale, Interfacce.IViewFormlModal
    {

        private Gestore oGestore = null;

        private ucEasyGrid _ucEasyGridOrari = null;
                private ucSegnalibri _ucSegnalibri = null;

        private bool _bFirmaDigitale = false;

        private bool _bMostraValidazione = true;

        public frmPUPrescrizioni()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {
            try
            {
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_PRESCRIZIONE_16);

                _bFirmaDigitale = DBUtils.ModuloUAAbilitato(CoreStatics.CoreApplication.Trasferimento.CodUA, EnumUAModuli.FirmaD_Prescrizioni);

                                if (this.CustomParamaters != null && this.CustomParamaters.GetType() == typeof(bool))
                {
                    _bMostraValidazione = (bool)this.CustomParamaters;
                }
                else
                {
                    _bMostraValidazione = true;
                }
                this.InizializzaControlli();
                this.InizializzaUltraGridLayout();
                this.VerificaSicurezza();
                this.InizializzaFiltri();
                this.InizializzaGestore();

                Aggiorna();

                                                                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.SoloTestata)
                {
                    bool bPermessoModifica = getPermessoModificaScheda();
                    if (bPermessoModifica)
                    {
                        this.ucEasyCheckEditorDC.Checked = true;
                                                                                            }
                }

                this.ShowDialog();
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Carica", this.Text);
            }
        }

        #endregion

        #region Private Functions

        private void InizializzaControlli()
        {
            try
            {
                CoreStatics.SetEasyUltraDockManager(ref this.ultraDockManager);

                this.ubZoomTipoPrescrizione.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_ZOOM_256);
                this.ubZoomTipoPrescrizione.PercImageFill = 0.75F;

                this.ubZoomViaSomministrazione.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_ZOOM_256);
                this.ubZoomViaSomministrazione.PercImageFill = 0.75F;

                this.uchkFiltro.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTRO_256);
                this.uchkFiltro.CheckedImage = Risorse.GetImageFromResource(Risorse.GC_FILTROAPPLICATO_256);
                this.uchkFiltro.Checked = false;
                this.uchkFiltro.PercImageFill = 0.75F;
                this.uchkFiltro.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.XSmall;
                this.uchkFiltro.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;

                this.ucEasyCheckEditorDC.Appearance.ImageBackground = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_MODIFICA_256);
                this.ucEasyCheckEditorDC.CheckedAppearance.ImageBackground = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_MODIFICACHECK_256);

                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodStatoContinuazione == EnumStatoContinuazione.CH.ToString())
                {
                    this.ucEasyPBStatoContinuazione.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_LUCCHETTOCHIUSO_256);
                }

                this.ubAdd.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_NUOVO_256);
                this.ubAdd.PercImageFill = 0.75F;

                this.ubSospendiTutto.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_SOSPENDI_256);
                this.ubSospendiTutto.PercImageFill = 0.75F;
                this.ubSospendiTutto.Visible = _bMostraValidazione;

                this.ubValidaTutto.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_FIRMAMULTIPLA_256);
                this.ubValidaTutto.PercImageFill = 0.75F;
                this.ubValidaTutto.Visible = _bMostraValidazione;


                                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione == string.Empty ||
                    CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione == @"")
                {
                    this.ubZoomTipoPrescrizione.Visible = true;
                    this.ubZoomViaSomministrazione.Visible = true;
                }
                else
                {
                    this.ubZoomTipoPrescrizione.Visible = false;
                    this.ubZoomViaSomministrazione.Visible = false;
                }

                this.ucAnteprimaRtf.rtbRichTextBox.ReadOnly = true;

                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.SoloTestata)
                {
                                        this.ucEasyGrid.Visible = false;
                    this.ucEasyTableLayoutPanelTempi.Visible = false;

                    this.ucEasyTableLayoutPanel.RowStyles[6].Height = 0F;
                    this.ucEasyTableLayoutPanel.RowStyles[4].Height = 81F;

                }
                else
                {
                                        this.ucEasyTableLayoutPanel.RowStyles[4].Height = 41F;
                    this.ucEasyTableLayoutPanel.RowStyles[6].Height = 40F;

                    this.ucEasyGrid.Visible = true;
                    this.ucEasyTableLayoutPanelTempi.Visible = true;
                }

            }
            catch (Exception)
            {
            }
        }

        private void InizializzaUltraGridLayout()
        {
            try
            {
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGrid);
                this.ucEasyGrid.DisplayLayout.Override.RowSizing = RowSizing.AutoFree;
                this.ucEasyGrid.DisplayLayout.Override.RowSizingAutoMaxLines = 3;
                CoreStatics.SetEasyUltraGridLayout(ref this.ucEasyGridFiltroStato);
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
                                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodStatoContinuazione == EnumStatoContinuazione.CH.ToString())
                {
                    this.ubAdd.Enabled = false;
                    this.ubValidaTutto.Enabled = false;
                }
                else
                {
                    this.ubAdd.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Prescr_Modifica);
                    this.ubValidaTutto.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Prescr_Valida);
                }
                                this.ucEasyCheckEditorDC.Visible = getPermessoModificaScheda();
                this.ubSospendiTutto.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Prescr_Annulla);
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "VerificaSicurezza", this.Name);
            }
        }

        private void InizializzaFiltri()
        {
            try
            {

                if (this.ucEasyGridFiltroStato.Rows.Count > 0)
                {
                    this.ucEasyGridFiltroStato.ActiveRow = null;
                    this.ucEasyGridFiltroStato.Selected.Rows.Clear();
                    CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroStato, "Codice", CoreStatics.GC_TUTTI);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "InizializzaFiltri", this.Name);
            }

        }

        private void InizializzaGestore()
        {

            try
            {

                                                                oGestore = CoreStatics.GetGestore();

                this.CaricaGestore();

                                this.ucDcViewer.VisualizzaTitoloScheda = false;

                this.ucDcViewer.CaricaDati(oGestore);

                this.ucDcViewer.RtfEvent -= ucDcViewer_RtfEvent;
                this.ucDcViewer.KeyEvent -= ucDcViewer_KeyEvent;
                this.ucDcViewer.ButtonEvent -= ucDcViewer_ButtonEvent;
                this.ucDcViewer.RtfEvent += ucDcViewer_RtfEvent;
                this.ucDcViewer.KeyEvent += ucDcViewer_KeyEvent;
                this.ucDcViewer.ButtonEvent += ucDcViewer_ButtonEvent;


            }
            catch (Exception ex)
            {
                throw new Exception(@"InizializzaGestore()" + Environment.NewLine + ex.Message, ex);
            }

        }

        private void CaricaGestore()
        {

            try
            {
                                oGestore.SchedaXML = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.MovScheda.Scheda.StrutturaXML;
                oGestore.SchedaLayoutsXML = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.MovScheda.Scheda.LayoutXML;
                                oGestore.Decodifiche = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.MovScheda.Scheda.DizionarioValori();

                                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.MovScheda.DatiXML == string.Empty)
                {
                    oGestore.SchedaDati = new DcSchedaDati();
                }
                else
                {
                    try
                    {
                        oGestore.SchedaDatiXML = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.MovScheda.DatiXML;
                    }
                    catch (Exception)
                    {
                        oGestore.SchedaDati = new DcSchedaDati();
                    }
                }
                if (oGestore.SchedaDati.Dati.Count == 0) { oGestore.NuovaScheda(); }



            }
            catch (Exception ex)
            {
                throw new Exception(@"CaricaGestore()" + Environment.NewLine + ex.Message, ex);
            }

        }

        public void Aggiorna()
        {

            try
            {

                this.lblZoomTipoPrescrizione.Text = string.Empty;
                this.lblZoomViaSomministrazione.Text = string.Empty;

                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Azione != EnumAzioni.INS)
                {
                                        if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata != null &&
                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata.SoloTestata == true)
                    {
                        this.ultraDockManager.Visible = false;
                    }
                    else
                    {
                        this.ultraDockManager.Visible = true;
                    }

                    this.lblZoomTipoPrescrizione.Text = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DescrTipoPrescrizione;
                    this.lblZoomViaSomministrazione.Text = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DescrViaSomministrazione;
                    this.SetDC(false);
                }
                else
                {
                                        this.ultraDockManager.Visible = false;

                    if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DescrTipoPrescrizione == "")
                    {
                        this.lblZoomTipoPrescrizione.Text = @"Selezionare Tipo Prescrizione";
                    }
                    else
                    {
                        this.lblZoomTipoPrescrizione.Text = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DescrTipoPrescrizione;
                    }

                    if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DescrViaSomministrazione == "")
                    {
                        this.lblZoomViaSomministrazione.Text = @"Selezionare Via di somministrazione";
                    }
                    else
                    {
                        this.lblZoomViaSomministrazione.Text = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DescrViaSomministrazione;
                    }
                    this.ucEasyCheckEditorDC.Checked = true;
                                        if (this.CustomParamaters != null && this.CustomParamaters.GetType() == typeof(string) && this.CustomParamaters.ToString().ToUpper() == "DAOE")
                    {
                        this.ucEasyCheckEditorDC.Checked = false;
                    }

                }

                this.CaricaGriglia(true);

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Aggiorna", this.Text);
            }

        }

        private void CaricaGriglia(bool datiEstesi)
        {

            try
            {

                                                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione != "")
                    op.Parametro.Add("IDPrescrizione", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione);
                else
                    op.Parametro.Add("IDPrescrizione", new Guid().ToString());

                if (datiEstesi)
                    op.Parametro.Add("DatiEstesi", "1");
                else
                    op.Parametro.Add("DatiEstesi", "0");

                op.TimeStamp.CodEntita = EnumEntita.PRF.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovPrescrizioniTempi", spcoll);

                DataTable dtEdit = ds.Tables[0].Copy();
                foreach (DataColumn dcCol in dtEdit.Columns)
                {
                    if (dcCol.ColumnName.ToUpper().IndexOf("PERMESSODAVALIDARE") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOMODIFICA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOANNULLA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCANCELLA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCOPIA") == 0)
                        dcCol.ReadOnly = false;


                    if (dcCol.ColumnName.ToUpper() == "DescrUtente".ToUpper() || dcCol.ColumnName.ToUpper() == "DescrUtenteValidazione".ToUpper())
                    {
                        dcCol.MaxLength = 250;
                    }

                }

                                DataColumn colsp = new DataColumn(CoreStatics.C_COL_SPAZIO, typeof(string));
                colsp.AllowDBNull = true;
                colsp.DefaultValue = "";
                colsp.Unique = false;
                dtEdit.Columns.Add(colsp);

                this.ucEasyGrid.DisplayLayout.Bands[0].Columns.ClearUnbound();

                                this.ucEasyGrid.DataSource = dtEdit;
                this.ucEasyGrid.Refresh();

                this.ucEasyGrid.PerformAction(UltraGridAction.FirstRowInBand, false, false);

                                SetIconaValidaTutti();

                if (datiEstesi)
                {
                                        this.ucEasyGridFiltroStato.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[1], true);
                    this.ucEasyGridFiltroStato.Refresh();
                }

                                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodStatoContinuazione == EnumStatoContinuazione.CH.ToString())
                {
                    this.ubValidaTutto.Enabled = false;
                }
                else
                {
                    this.ubValidaTutto.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Prescr_Valida) &&
                                                    this.ucEasyGrid.Rows.Count > 0;
                }
                this.ubSospendiTutto.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Prescr_Annulla) &&
                                                this.ucEasyGrid.Rows.Count > 0;

                this.ubZoomViaSomministrazione.Visible = !(this.ucEasyGrid.Rows.Count > 0);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaGriglia", this.Name);
            }
        }

        private void CaricaRtf()
        {

            try
            {

                this.ucAnteprimaRtf.MovScheda = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.MovScheda;
                this.ucAnteprimaRtf.MovScheda.GeneraRTF();
                this.ucAnteprimaRtf.RefreshRTF();

            }
            catch (Exception ex)
            {
                throw new Exception(@"CaricaRtf()" + Environment.NewLine + ex.Message, ex);
            }

        }

        private void CaricaScheda()
        {

            try
            {

                this.ucDcViewer.CaricaDati(oGestore);

            }
            catch (Exception ex)
            {
                throw new Exception(@"CaricaScheda()" + Environment.NewLine + ex.Message, ex);
            }

        }

        private void SetDC(bool bDc)
        {

            this.ucEasyTableLayoutPanelDC.Visible = false;

            switch (bDc)
            {

                case false:
                    this.ucEasyTableLayoutPanelDC.ColumnStyles[0].Width = 100;
                    this.ucEasyTableLayoutPanelDC.ColumnStyles[1].Width = 0;
                    CoreStatics.CoreApplication.MovPrescrizioneSelezionata.MovScheda.DatiXML = this.ucDcViewer.Gestore.SchedaDatiXML;
                    this.CaricaRtf();
                    this.VerificaSicurezza();
                    if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodStatoContinuazione == EnumStatoContinuazione.CH.ToString())
                    {
                        this.ubValidaTutto.Enabled = false;
                    }
                    else
                    {
                        this.ubValidaTutto.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Prescr_Valida) &&
                                                  this.ucEasyGrid.Rows.Count > 0;
                    }
                    this.ubSospendiTutto.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Prescr_Annulla) &&
                                                    this.ucEasyGrid.Rows.Count > 0;
                    break;

                case true:
                    this.ucEasyTableLayoutPanelDC.ColumnStyles[0].Width = 0;
                    this.ucEasyTableLayoutPanelDC.ColumnStyles[1].Width = 100;
                    this.ucDcViewer.CaricaDati();
                    this.uchkFiltro.Enabled = false;
                    this.ubAdd.Enabled = false;
                    this.ubSospendiTutto.Enabled = false;
                    this.ubValidaTutto.Enabled = false;
                    break;
            }

            this.ucEasyTableLayoutPanelDC.Visible = true;

        }

        private void AggiornaGriglia(bool datiEstesi)
        {

            string codstatoselezionato = string.Empty;

            try
            {
                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                                                                bool bFiltro = false;

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione != "")
                    op.Parametro.Add("IDPrescrizione", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione);
                else
                    op.Parametro.Add("IDPrescrizione", new Guid().ToString());

                if (datiEstesi)
                    op.Parametro.Add("DatiEstesi", "1");
                else
                    op.Parametro.Add("DatiEstesi", "0");

                if (this.ucEasyGridFiltroStato.ActiveRow != null && this.ucEasyGridFiltroStato.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                {
                    op.Parametro.Add("CodStatoPrescrizioneTempi", this.ucEasyGridFiltroStato.ActiveRow.Cells["Codice"].Text);
                    bFiltro = true;
                }

                this.uchkFiltro.Checked = bFiltro;

                op.TimeStamp.CodEntita = EnumEntita.PRF.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovPrescrizioniTempi", spcoll);

                DataTable dtEdit = ds.Tables[0].Copy();
                foreach (DataColumn dcCol in dtEdit.Columns)
                {
                    if (dcCol.ColumnName.ToUpper().IndexOf("PERMESSODAVALIDARE") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOMODIFICA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOANNULLA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCANCELLA") == 0 ||
                        dcCol.ColumnName.ToUpper().IndexOf("PERMESSOCOPIA") == 0)
                        dcCol.ReadOnly = false;

                    if (dcCol.ColumnName.ToUpper() == "DescrUtente".ToUpper() || dcCol.ColumnName.ToUpper() == "DescrUtenteValidazione".ToUpper())
                        dcCol.MaxLength = 250;

                }
                this.ucEasyGrid.DisplayLayout.Bands[0].Columns.ClearUnbound();
                this.ucEasyGrid.DataSource = dtEdit;
                this.ucEasyGrid.Refresh();


                                SetIconaValidaTutti();

                if (datiEstesi)
                {
                    this.ucEasyGridFiltroStato.DisplayLayout.Bands[0].Columns.ClearUnbound();
                    if (this.ucEasyGridFiltroStato.ActiveRow != null && this.ucEasyGridFiltroStato.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                        codstatoselezionato = this.ucEasyGridFiltroStato.ActiveRow.Cells["Codice"].Text;
                    this.ucEasyGridFiltroStato.DataSource = CoreStatics.AggiungiTuttiDataTable(ds.Tables[1], true);
                    this.ucEasyGridFiltroStato.Refresh();
                    if (this.ucEasyGridFiltroStato.ActiveRow != null && this.ucEasyGridFiltroStato.ActiveRow.Cells["Codice"].Text.Contains(CoreStatics.GC_TUTTI) != true)
                        CoreStatics.SelezionaRigaInGriglia(ref ucEasyGridFiltroStato, "Codice", codstatoselezionato);
                }

                                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodStatoContinuazione == EnumStatoContinuazione.CH.ToString())
                {
                    this.ubValidaTutto.Enabled = false;
                }
                else
                {
                    this.ubValidaTutto.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Prescr_Valida) &&
                                                    this.ucEasyGrid.Rows.Count > 0;
                }
                this.ubSospendiTutto.Enabled = CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Prescr_Annulla) &&
                                                this.ucEasyGrid.Rows.Count > 0;

                this.ubZoomViaSomministrazione.Visible = !(this.ucEasyGrid.Rows.Count > 0);

                                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CaricaTempi(CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "AggiornaGriglia", this.Name);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
        }

        public bool Salva(bool bAvanti)
        {
            bool bReturn = false;
            try
            {

                                                                                                                if (this.ucEasyCheckEditorDC.Visible)
                {
                    if (ControllaValori())
                    {

                        if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.SoloTestata == false)
                        {

                            if (bAvanti == true)
                            {
                                if (this.ucEasyGrid.Rows.Count == 0)
                                {
                                    if (easyStatics.EasyMessageBox(@"Non è stata impostata alcuna tempistica, vuoi procedere comunque?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                                    {
                                        return bReturn;
                                    }
                                }
                            }

                        }

                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DataEvento = DateTime.Now;

                        bReturn = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Salva();

                        this.PulsanteIndietroAbilitato = false;
                    }
                }
                else
                    bReturn = true;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Salva", this.Text);
            }

            return bReturn;
        }

        private bool ControllaValori()
        {
            bool bOK = true;

                        if (bOK && CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodTipoPrescrizione == "")
            {
                easyStatics.EasyMessageBox("Prima di procedere con la definizione dei tempi è necessario inserire il Tipo di Prescrizione.", "Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.ubZoomTipoPrescrizione.Focus();
                bOK = false;
            }
            if (bOK && CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodViaSomministrazione == "")
            {
                easyStatics.EasyMessageBox("Prima di procedere con la definizione dei tempi è necessario la Via di Somministrazione.", "Prescrizione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.ubZoomViaSomministrazione.Focus();
                bOK = false;
            }
                                                                                                                                                                                                                                    if (bOK && CoreStatics.CoreApplication.MovPrescrizioneSelezionata.MovScheda.DatiObbligatoriMancantiRTF != string.Empty)
            {
                easyStatics.EasyMessageBox(@"Non sono stati compilati alcuni valori obbligatori della scheda!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.ucDcViewer.Focus();
                bOK = false;
            }

            return bOK;
        }

        private bool ControllaValidazioneTempi()
        {
            bool bRet = true;

            try
            {
                                                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPrescrizione", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione);

                op.TimeStamp.CodEntita = EnumEntita.PRF.ToString();
                op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();

                                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataTable dt = Database.GetDataTableStoredProc("MSP_EsistePrescrizioneTempiValidata", spcoll);

                if (dt != null)
                {
                    if (dt.Rows.Count == 1)
                    {
                        bRet = Convert.ToBoolean(dt.Rows[0]["Esiste"]);
                    }
                    else
                        bRet = false;
                }
                else
                    bRet = false;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ControllaValidazioneTempi", this.Name);
                bRet = false;
            }

            return bRet;
        }

        private void SetIconaValidaTutti()
        {
            try
            {
                                if (_bFirmaDigitale)
                {
                    this.ubValidaTutto.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAFIRMATUTTI_256);
                    this.ubSospendiTutto.Appearance.Image = Risorse.GetImageFromResource(ScciResource.Risorse.GC_SOSPENDITESSERA_256);
                }
                else
                {
                    this.ubValidaTutto.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FIRMAMULTIPLA_256);
                    this.ubSospendiTutto.Appearance.Image = Risorse.GetImageFromResource(ScciResource.Risorse.GC_SOSPENDI_256);
                }

            }
            catch
            {
                this.ubValidaTutto.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FIRMAMULTIPLA_256);
                this.ubSospendiTutto.Appearance.Image = Risorse.GetImageFromResource(ScciResource.Risorse.GC_SOSPENDI_256);
            }
        }

        private void setNavigazione(bool enable)
        {
            try
            {
                CoreStatics.SetNavigazione(enable);

                this.ucEasyTableLayoutPanelFiltri.Enabled = enable;
                this.ucEasyTableLayoutPanel.Enabled = enable;

                this.ucBottomModale.Enabled = enable;

            }
            catch
            {
                CoreStatics.SetNavigazione(true);
                this.ucBottomModale.Enabled = true;
            }
        }

                                        private bool getPermessoModificaScheda()
        {
            if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Prescr_Modifica) &&
                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.PermessoModificaScheda == 1)
                return true;
            else
                return false;
        }

        #endregion

        #region Events

        private void ultraDockManager_InitializePane(object sender, Infragistics.Win.UltraWinDock.InitializePaneEventArgs e)
        {
            e.Pane.Settings.Appearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            e.Pane.Settings.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FILTRO_32);

            int filtroWidth = 15 * (int)easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large);
            this.ultraDockManager.ControlPanes[0].FlyoutSize = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].FlyoutSize.Height);
            this.ultraDockManager.ControlPanes[0].Size = new Size(filtroWidth, this.ultraDockManager.ControlPanes[0].Size.Height);
            this.ultraDockManager.DockAreas[0].Size = new Size(filtroWidth, this.ultraDockManager.DockAreas[0].Size.Height);
            this.pnlFiltro.Width = filtroWidth;
        }

        private void ucEasyGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                int refWidth = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall) * 3);
                                Graphics g = this.CreateGraphics();
                int refBtnWidth = (CoreStatics.PointToPixel(easyStatics.getFontSizeInPointsFromRelativeDimension(this.ucEasyGrid.DataRowFontRelativeDimension), g.DpiY) * 3);
                g.Dispose();
                g = null;
                                e.Layout.Bands[0].HeaderVisible = false;
                e.Layout.Bands[0].ColHeadersVisible = false;
                e.Layout.Bands[0].RowLayoutStyle = RowLayoutStyle.GroupLayout;
                                foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
                {

                    try
                    {
                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {
                            case "DescrTempo":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Small);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = this.ucEasyGrid.Width - (refWidth * 24) - Convert.ToInt32(refBtnWidth * 2.5) - 30;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;

                                oCol.RowLayoutColumnInfo.OriginX = 0;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
                            
                                                                                                                
                            
                            case "Posologia":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = (int)(refWidth * 3);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;

                                oCol.RowLayoutColumnInfo.OriginX = 1;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;


                            case "EPA":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = refWidth * 5;
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;

                                oCol.RowLayoutColumnInfo.OriginX = 2;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
                            
                                                                                                                
                            
                            case "DescrUtente":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = (int)(refWidth * 5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;

                                oCol.RowLayoutColumnInfo.OriginX = 4;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DescrUtenteValidazione":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = (int)(refWidth * 5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;

                                oCol.RowLayoutColumnInfo.OriginX = 5;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case "DescrUtenteSospensione":
                                oCol.Hidden = false;
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XSmall);
                                oCol.CellAppearance.ForeColor = Color.Red;
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                                oCol.AutoSizeMode = ColumnAutoSizeMode.None;
                                oCol.CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.None;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = (int)(refWidth * 5);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.CellAppearance.TextVAlign = Infragistics.Win.VAlign.Top;

                                oCol.RowLayoutColumnInfo.OriginX = 6;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

                                break;

                            case CoreStatics.C_COL_SPAZIO:
                                oCol.Hidden = false;
                                oCol.CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.XXSmall);
                                oCol.CellClickAction = CellClickAction.RowSelect;
                                oCol.CellActivation = Activation.NoEdit;
                                oCol.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                                oCol.VertScrollBar = false;
                                try
                                {
                                    oCol.MaxWidth = Convert.ToInt32(refWidth * 1);
                                    oCol.MinWidth = oCol.MaxWidth;
                                    oCol.Width = oCol.MaxWidth;
                                }
                                catch (Exception)
                                {
                                }

                                oCol.RowLayoutColumnInfo.WeightY = 1;   
                                oCol.RowLayoutColumnInfo.OriginX = 7;
                                oCol.RowLayoutColumnInfo.OriginY = 0;
                                oCol.RowLayoutColumnInfo.SpanX = 1;
                                oCol.RowLayoutColumnInfo.SpanY = 1;

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

                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_ICO_STATO))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_ICO_STATO);
                    colEdit.Hidden = false;

                                        colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;

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
                    colEdit.RowLayoutColumnInfo.SpanY = 1;
                }
                                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_ICO_STATO + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_ICO_STATO + @"_SP");
                    colEdit.Hidden = false;
                    colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
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
                    colEdit.RowLayoutColumnInfo.SpanY = 2;
                }

                                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_MENU))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_MENU);
                    colEdit.Hidden = false;

                                        colEdit.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
                    colEdit.ButtonDisplayStyle = ButtonDisplayStyle.Always;
                    colEdit.CellActivation = Activation.AllowEdit;
                    colEdit.CellButtonAppearance.TextVAlign = Infragistics.Win.VAlign.Bottom;
                    colEdit.CellButtonAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                    colEdit.CellButtonAppearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                    colEdit.CellButtonAppearance.Image = Risorse.GetImageFromResource(Risorse.GC_MENUPOPUP_32);

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
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                    colEdit.LockedWidth = true;


                    colEdit.RowLayoutColumnInfo.OriginX = 10;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 1;
                }
                if (!e.Layout.Bands[0].Columns.Exists(CoreStatics.C_COL_BTN_MENU + @"_SP"))
                {
                    UltraGridColumn colEdit = e.Layout.Bands[0].Columns.Add(CoreStatics.C_COL_BTN_MENU + @"_SP");
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
                    colEdit.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
                    colEdit.LockedWidth = true;
                    colEdit.RowLayoutColumnInfo.OriginX = 11;
                    colEdit.RowLayoutColumnInfo.OriginY = 0;
                    colEdit.RowLayoutColumnInfo.SpanX = 1;
                    colEdit.RowLayoutColumnInfo.SpanY = 1;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_InitializeLayout", this.Name);
            }
        }

        private void ucEasyGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {

                foreach (UltraGridCell ocell in e.Row.Cells)
                {


                    if (ocell.Column.Key.Trim().ToUpper() == "DescrUtente".ToUpper() && ocell.Text.Trim() != "" && ocell.Text.IndexOf("Inserito da") < 0)
                    {
                        ocell.Value = "Inserito da: " + ocell.Value.ToString();
                        if (ocell.Row.Cells["DataEvento"].Text.Trim() != "")
                        {
                            ocell.Value += " in data: " + ((DateTime)ocell.Row.Cells["DataEvento"].Value).ToString("dd/MM/yyyy HH:mm");
                        }
                        e.Row.Update();
                    }


                    if (ocell.Column.Key.Trim().ToUpper() == "DescrUtenteValidazione".ToUpper() && ocell.Text.Trim() != "" && ocell.Text.IndexOf("Validato da") < 0)
                    {
                        ocell.Value = "Validato da: " + ocell.Value.ToString();
                        if (ocell.Row.Cells["DataValidazione"].Text.Trim() != "")
                        {
                            ocell.Value += " in data: " + ((DateTime)ocell.Row.Cells["DataValidazione"].Value).ToString("dd/MM/yyyy HH:mm");
                        }
                        e.Row.Update();
                    }

                    if (ocell.Column.Key.Trim().ToUpper() == "DescrUtenteSospensione".ToUpper() && ocell.Text.Trim() != "" && ocell.Text.IndexOf("Sospeso da") < 0)
                    {
                        ocell.Value = "Sospeso da: " + ocell.Value.ToString();
                        if (ocell.Row.Cells["DataSospensione"].Text.Trim() != "")
                        {
                            ocell.Value += " in data: " + ((DateTime)ocell.Row.Cells["DataSospensione"].Value).ToString("dd/MM/yyyy HH:mm");
                        }
                        e.Row.Update();
                    }

                    if (ocell.Column.Key == CoreStatics.C_COL_ICO_STATO && ocell.Row.Cells["PermessoDaValidare"].Value.ToString() == "1")
                        if (_bFirmaDigitale)
                            ocell.Appearance.ImageBackground = Risorse.GetImageFromResource(Risorse.GC_TESSERAFIRMA_32);
                        else
                            ocell.Appearance.ImageBackground = Risorse.GetImageFromResource(Risorse.GC_FIRMA_32);

                    if (ocell.Column.Key == CoreStatics.C_COL_BTN_EDIT && ocell.Row.Cells["PermessoModifica"].Value.ToString() == "0")
                        ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                    if (ocell.Column.Key == CoreStatics.C_COL_BTN_VALID && ocell.Row.Cells["PermessoDaValidare"].Value.ToString() == "0")
                        ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                    if (ocell.Column.Key == CoreStatics.C_COL_BTN_SOSPENDI && ocell.Row.Cells["PermessoAnnulla"].Value.ToString() == "0")
                        ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                    if (ocell.Column.Key == CoreStatics.C_COL_BTN_DEL && ocell.Row.Cells["PermessoCancella"].Value.ToString() == "0")
                        ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;

                    if (ocell.Column.Key == CoreStatics.C_COL_BTN_COPY && ocell.Row.Cells["PermessoCopia"].Value.ToString() == "0")
                        ocell.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Integer;
                }



            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_InitializeRow", this.Name);
            }
        }

        private void ucEasyGrid_ClickCell(object sender, ClickCellEventArgs e)
        {
            if (e.Cell.Row.Cells["PermessoDaValidare"].Value.ToString() == "0")
            {
                                this.PreparaGridValidati((UltraGrid)sender, e.Cell);
            }
            else
            {
                                this.PreparaGridDaValidare((UltraGrid)sender, e.Cell);
            }
        }

        private void ucEasyGrid_ClickCellButton(object sender, CellEventArgs e)
        {

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case CoreStatics.C_COL_BTN_MENU:

                        var menu = (PopupMenuTool)this.UltraToolbarsManager.Tools["PopupMenuTool"];

                        ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_EDIT]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICA_32);

                        if (_bFirmaDigitale)
                        {
                            ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_VALID]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAFIRMA_32);
                            ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_SOSPENDI]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_SOSPENDITESSERA_32);
                        }
                        else
                        {
                            ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_VALID]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FIRMA_32);
                            ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_SOSPENDI]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_SOSPENDI_32);
                        }

                        if (_bFirmaDigitale &&
                            (e.Cell.Row.Cells[@"CodStatoPrescrizioneTempi"].Text.Trim().ToUpper() == EnumStatoPrescrizioneTempi.VA.ToString() || e.Cell.Row.Cells[@"CodStatoPrescrizioneTempi"].Text.Trim().ToUpper() == EnumStatoPrescrizioneTempi.SS.ToString()))
                            ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_DEL]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_CANCELLATESSERA_32);
                        else
                            ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_DEL]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_ELIMINA_32);

                        ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_COPY]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_COPIA_32);

                        menu.Tools[CoreStatics.C_COL_BTN_EDIT].InstanceProps.Visible = (e.Cell.Row.Cells["PermessoModifica"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);
                        menu.Tools[CoreStatics.C_COL_BTN_VALID].InstanceProps.Visible = (e.Cell.Row.Cells["PermessoDaValidare"].Text == "1" && _bMostraValidazione ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);
                        menu.Tools[CoreStatics.C_COL_BTN_SOSPENDI].InstanceProps.Visible = (e.Cell.Row.Cells["PermessoAnnulla"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);
                        menu.Tools[CoreStatics.C_COL_BTN_DEL].InstanceProps.Visible = (e.Cell.Row.Cells["PermessoCancella"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);
                        menu.Tools[CoreStatics.C_COL_BTN_COPY].InstanceProps.Visible = (e.Cell.Row.Cells["PermessoCopia"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);

                        ((PopupMenuTool)this.UltraToolbarsManager.Tools["PopupMenuTool"]).ShowPopup();

                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCellButton", this.Name);
            }

        }

        private void UltraToolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {

            switch (e.Tool.Key)
            {

                case CoreStatics.C_COL_BTN_EDIT:
                    
                                        if (this.Salva(false))
                    {
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata =
                            CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Elementi.Find(MovPrescrizioneTempi => MovPrescrizioneTempi.IDPrescrizioneTempi == this.ucEasyGrid.ActiveRow.Cells["ID"].Text);


                        if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingPrescrizioneTempi, true, this.CustomParamaters) == DialogResult.OK)
                        {
                            this.AggiornaGriglia(true);
                            if (this.ucEasyCheckEditorDC.Visible) this.ucEasyCheckEditorDC.Visible = !ControllaValidazioneTempi();
                            if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata != null)
                            {
                                CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.IDPrescrizione);
                                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;
                            }
                        }
                    }
                    break;

                case CoreStatics.C_COL_BTN_VALID:
                    if (easyStatics.EasyMessageBox("Confermi la VALIDAZIONE della periodicità selezionata ?", "Validazione Tempi Prescrizioni", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        this.ImpostaCursore(enum_app_cursors.WaitCursor);

                                                if (this.Salva(false))
                        {
                            MovPrescrizioneTempi movpr =
                                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Elementi.Find(MovPrescrizioneTempi => MovPrescrizioneTempi.IDPrescrizioneTempi == this.ucEasyGrid.ActiveRow.Cells["ID"].Text);

                            bool bContinua = true;

                                                        if (_bFirmaDigitale)
                            {
                                                                bContinua = false;

                                frmSmartCardProgress frmSC = null;
                                try
                                {

                                                                        setNavigazione(false);
                                    frmSC = new frmSmartCardProgress();
                                    frmSC.InizializzaEMostra(0, 4, this);
                                                                        frmSC.SetCursore(enum_app_cursors.WaitCursor);

                                    frmSC.SetStato(@"Validazione Prescrizione " + movpr.Posologia);
                                    try
                                    {
                                                                                frmSC.SetStato(@"Generazione Documento...");

                                                                                byte[] pdfContent = CoreStatics.GeneraPDFPrescrizioneTempi(movpr.IDPrescrizioneTempi, EnumStatoPrescrizioneTempi.VA, true);

                                        if (pdfContent == null || pdfContent.Length <= 0)
                                        {
                                            frmSC.SetLog(@"Errore Generazione Documento", true);
                                        }
                                        else
                                        {
                                                                                        bContinua = frmSC.ProvaAFirmare(ref pdfContent, EnumTipoDocumentoFirmato.PRTFM01, "Firma Digitale...", EnumEntita.PRT, movpr.IDPrescrizioneTempi);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (frmSC != null) frmSC.SetLog(@"Errore Generazione documento: " + ex.Message, true);
                                        bContinua = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    CoreStatics.ExGest(ref ex, "UltraToolbarsManager_ToolClick", this.Name);
                                }
                                finally
                                {
                                    if (frmSC != null)
                                    {
                                        frmSC.Close();
                                        frmSC.Dispose();
                                    }

                                                                        setNavigazione(true);
                                }

                            } 

                            if (bContinua)
                            {
                                                                movpr.CodStatoPrescrizioneTempi = EnumStatoPrescrizioneTempi.VA.ToString();                                 movpr.Azione = EnumAzioni.VAL;

                                if (movpr.Salva())
                                {
                                    movpr.CreaTaskInfermieristici(EnumCodSistema.PRF, Database.GetConfigTable(EnumConfigTable.TipoSchedaTaskDaPrescrizione), EnumTipoRegistrazione.A);

                                                                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = movpr;
                                    Risposta oRispostaValidaDopo = new Risposta();
                                    oRispostaValidaDopo.Successo = true;
                                    oRispostaValidaDopo = PluginClientStatics.PluginClient(EnumPluginClient.PRT_VALIDA_DOPO_PU.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente));
                                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;

                                                                        AggiornaGriglia(true);
                                    if (this.ucEasyCheckEditorDC.Visible) this.ucEasyCheckEditorDC.Visible = !ControllaValidazioneTempi();
                                }
                            }

                        }

                        this.ImpostaCursore(enum_app_cursors.DefaultCursor);

                    }
                    break;

                case CoreStatics.C_COL_BTN_SOSPENDI:
                    this.ImpostaCursore(enum_app_cursors.WaitCursor);

                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata =
                        CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Elementi.Find(MovPrescrizioneTempi => MovPrescrizioneTempi.IDPrescrizioneTempi == this.ucEasyGrid.ActiveRow.Cells["ID"].Text);

                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodStatoPrescrizioneTempi = EnumStatoPrescrizioneTempi.SS.ToString();                     if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.AnnullaPrescrizioneTempi) == DialogResult.OK)
                    {

                        bool bContinua = true;
                        setNavigazione(false);

                                                                        Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.PRT_SOSPENDI_PRIMA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente));
                        bContinua = oRispostaElaboraPrima.Successo;

                        if (bContinua)
                        {
                            if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Sospendi())
                            {
                                                                AggiornaGriglia(true);
                                if (this.ucEasyCheckEditorDC.Visible) this.ucEasyCheckEditorDC.Visible = !ControllaValidazioneTempi();
                            }
                        }
                        setNavigazione(true);
                    }
                    this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                    break;

                case CoreStatics.C_COL_BTN_DEL:
                    if (easyStatics.EasyMessageBox("Confermi la CANCELLAZIONE della periodicità selezionata ?", "Cancellazione Tempi Prescrizioni", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        this.ImpostaCursore(enum_app_cursors.WaitCursor);

                                                if (this.Salva(false))
                        {

                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata =
                                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Elementi.Find(MovPrescrizioneTempi => MovPrescrizioneTempi.IDPrescrizioneTempi == this.ucEasyGrid.ActiveRow.Cells["ID"].Text);

                            bool bContinua = true;

                                                        Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.PRT_CANCELLA_PRIMA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente));
                            bContinua = oRispostaElaboraPrima.Successo;

                            if (bContinua)
                            {
                                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodStatoPrescrizioneTempi = EnumStatoPrescrizioneTempi.CA.ToString();                                 if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Cancella())
                                {
                                                                        AggiornaGriglia(true);
                                    CoreStatics.CoreApplication.MovPrescrizioneSelezionata = new MovPrescrizione(CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione, CoreStatics.CoreApplication.Ambiente);
                                    this.VerificaSicurezza();
                                    if (this.ucEasyCheckEditorDC.Visible) this.ucEasyCheckEditorDC.Visible = !ControllaValidazioneTempi();
                                }
                            }

                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;

                            this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                        }
                    }
                    break;

                case CoreStatics.C_COL_BTN_COPY:
                                        if (easyStatics.EasyMessageBox("Confermi la copia della periodicità selezionata ?", "Copia Tempi Prescrizioni",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        this.ImpostaCursore(enum_app_cursors.WaitCursor);

                        MovPrescrizioneTempi movprtorigine =
                            CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Elementi.Find(MovPrescrizioneTempi => MovPrescrizioneTempi.IDPrescrizioneTempi == this.ucEasyGrid.ActiveRow.Cells["ID"].Text);

                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = new MovPrescrizioneTempi(new Guid(CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione), CoreStatics.CoreApplication.Ambiente);

                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CopiaDaOrigine(movprtorigine);
                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Salva();
                        movprtorigine = null;
                        this.ImpostaCursore(enum_app_cursors.DefaultCursor);

                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingPrescrizioneTempi);

                        this.AggiornaGriglia(true);
                        if (this.ucEasyCheckEditorDC.Visible) this.ucEasyCheckEditorDC.Visible = !ControllaValidazioneTempi();
                        if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata != null)
                        {
                            CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.IDPrescrizione);
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;
                        }

                    }
                    break;

            }

        }

        private void ucEasyGridFiltroStato_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].HeaderVisible = false;
            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                switch (oCol.Key)
                {
                    case "Descrizione":
                        oCol.Hidden = false;
                        oCol.Header.Caption = @"Stato";
                        break;
                    default:
                        oCol.Hidden = true;
                        break;
                }
            }
        }

        private void ubZoomTipoPrescrizione_Click(object sender, EventArgs e)
        {
            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneTipoPrescrizione) == DialogResult.OK)
            {
                this.lblZoomTipoPrescrizione.Text = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DescrTipoPrescrizione;
                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DescrViaSomministrazione == "")
                {
                    this.lblZoomViaSomministrazione.Text = @"Selezionare Via di somministrazione";
                }
                else
                {
                    this.lblZoomViaSomministrazione.Text = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DescrViaSomministrazione;
                }
                this.CaricaGestore();
                this.ucEasyCheckEditorDC.Checked = true;
                this.SetDC(true);
                this.ImpostaCursore(enum_app_cursors.WaitCursor);
                CaricaRtf();
                CaricaScheda();
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
        }

        private void ubZoomViaSomministrazione_Click(object sender, EventArgs e)
        {
            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneViaSomministrazione) == DialogResult.OK)
            {
                this.lblZoomViaSomministrazione.Text = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.DescrViaSomministrazione;
                this.ImpostaCursore(enum_app_cursors.WaitCursor);
                CaricaRtf();
                CaricaScheda();
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
        }

        private void uchkFiltro_Click(object sender, EventArgs e)
        {
                        if (!this.uchkFiltro.Checked)
            {
                this.InizializzaFiltri();
                this.AggiornaGriglia(false);
            }
            else
                this.uchkFiltro.Checked = !this.uchkFiltro.Checked;
        }

        private void ucEasyCheckEditorDC_CheckedChanged(object sender, EventArgs e)
        {

            this.SetDC(this.ucEasyCheckEditorDC.Checked);

        }

        private void ubAdd_Click(object sender, EventArgs e)
        {
            try
            {
                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                                if (this.Salva(false))
                {
                                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = new MovPrescrizioneTempi(new Guid(CoreStatics.CoreApplication.MovPrescrizioneSelezionata.IDPrescrizione), CoreStatics.CoreApplication.Ambiente);

                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Azione = EnumAzioni.INS;
                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.DataEvento = DateTime.Now;

                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EditingPrescrizioneTempi, true, _bMostraValidazione) == DialogResult.OK)
                    {
                        this.InizializzaControlli();
                        this.AggiornaGriglia(true);
                        this.SetDC(false);
                        if (this.ucEasyCheckEditorDC.Visible) this.ucEasyCheckEditorDC.Visible = !ControllaValidazioneTempi();
                        if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata != null)
                            CoreStatics.SelezionaRigaInGriglia(ref this.ucEasyGrid, "ID", CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.IDPrescrizioneTempi);
                    }
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ubAdd_Click", this.Name);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
        }

        private void ubValidaTutto_Click(object sender, EventArgs e)
        {

#if (DEBUG)
                                                            #endif

            if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Prescr_Valida))
            {
                                if (easyStatics.EasyMessageBox("Confermi la VALIDAZIONE di tutte le righe visibili ?", "Validazione Tempi Prescrizioni", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    frmSmartCardProgress frmSC = null;
                    bool bAggiorna = false;
                    this.ImpostaCursore(enum_app_cursors.WaitCursor);

                    try
                    {
                        if (_bFirmaDigitale)
                        {
                                                        int iCount = 0;
                            foreach (MovPrescrizioneTempi movprt in CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Elementi)
                            {
                                if (movprt.CodStatoPrescrizioneTempi == EnumStatoPrescrizioneTempi.IC.ToString()) iCount += 3;
                            }

                                                        setNavigazione(false);
                            frmSC = new frmSmartCardProgress();
                            frmSC.InizializzaEMostra(0, iCount + 1, this);
                                                        frmSC.SetCursore(enum_app_cursors.WaitCursor);
                        }


                        for (int t = 0; t < CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Elementi.Count; t++)
                        {
                            MovPrescrizioneTempi movprt = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Elementi[t];

                            try
                            {
                                if (movprt.CodStatoPrescrizioneTempi == EnumStatoPrescrizioneTempi.IC.ToString())
                                {
                                    bool bContinua = true;

                                    if (_bFirmaDigitale)
                                    {
                                        bContinua = false;
                                        frmSC.SetStato(@"Validazione " + movprt.Posologia);

                                        if (frmSC.TerminaOperazione)
                                        {
                                                                                        t = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Elementi.Count + 1;
                                        }
                                        else
                                        {
                                                                                        frmSC.SetStato(@"Generazione Documento...");

                                                                                        byte[] pdfContent = CoreStatics.GeneraPDFPrescrizioneTempi(movprt.IDPrescrizioneTempi, EnumStatoPrescrizioneTempi.VA, true);

                                            if (pdfContent == null || pdfContent.Length <= 0)
                                            {
                                                frmSC.SetLog(@"Errore Generazione Documento", true);
                                            }
                                            else
                                            {
                                                                                                bContinua = frmSC.ProvaAFirmare(ref pdfContent, EnumTipoDocumentoFirmato.PRTFM01, "Firma Digitale...", EnumEntita.PRT, movprt.IDPrescrizioneTempi);
                                            }
                                        }
                                    } 

                                    if (bContinua)
                                    {
                                                                                movprt.CodStatoPrescrizioneTempi = EnumStatoPrescrizioneTempi.VA.ToString();
                                        movprt.Azione = EnumAzioni.VAL;
                                        if (movprt.Salva())
                                        {
                                            bAggiorna = true;
                                            movprt.CreaTaskInfermieristici(EnumCodSistema.PRF, Database.GetConfigTable(EnumConfigTable.TipoSchedaTaskDaPrescrizione), EnumTipoRegistrazione.A);

                                                                                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = movprt;
                                            Risposta oRispostaValidaDopo = new Risposta();
                                            oRispostaValidaDopo.Successo = true;
                                            oRispostaValidaDopo = PluginClientStatics.PluginClient(EnumPluginClient.PRT_VALIDA_DOPO_PU.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente));
                                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;

                                        }

                                    }                                 }                             }
                            catch (Exception ex)
                            {
                                if (frmSC != null) frmSC.SetLog(@"ERRORE " + ex.Message, true);
                            }

                        } 
                        if (bAggiorna)
                        {
                            this.AggiornaGriglia(true);
                            if (this.ucEasyCheckEditorDC.Visible) this.ucEasyCheckEditorDC.Visible = !ControllaValidazioneTempi();
                        }
                    }
                    catch (Exception ex)
                    {
                        CoreStatics.ExGest(ref ex, "ubValidaTutto_Click", this.Text);
                    }
                    finally
                    {
                        if (frmSC != null)
                        {
                            frmSC.Close();
                            frmSC.Dispose();
                        }

                                                setNavigazione(true);

                        this.ImpostaCursore(enum_app_cursors.DefaultCursor);

                    }
                }
            }
        }

        private void ubSospendi_Click(object sender, EventArgs e)
        {
                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Esiste(EnumModules.Prescr_Annulla))
            {
                CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;
                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.AnnullaPrescrizioneTempi) == DialogResult.OK)
                {
                    this.ImpostaCursore(enum_app_cursors.WaitCursor);

                    frmSmartCardProgress frmSC = null;
                    bool bAggiorna = false;

                    try
                    {
                        for (int t = 0; t < CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Elementi.Count; t++)
                        {
                                                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Elementi[t];
                            try
                            {
                                if (CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodStatoPrescrizioneTempi == EnumStatoPrescrizioneTempi.IC.ToString() ||
                                    CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodStatoPrescrizioneTempi == EnumStatoPrescrizioneTempi.VA.ToString())
                                {

                                    bool bContinua = true;
                                    setNavigazione(false);

                                                                                                            Risposta oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.PRT_SOSPENDI_PRIMA.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.Trasferimento.CodUA, CoreStatics.CoreApplication.Ambiente));
                                    bContinua = oRispostaElaboraPrima.Successo;

                                                                        if (bContinua)
                                    {
                                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.CodStatoPrescrizioneTempi = EnumStatoPrescrizioneTempi.SS.ToString();
                                        CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata.Sospendi();
                                        bAggiorna = true;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                if (frmSC != null) frmSC.SetLog(@"ERRORE " + ex.Message, true);
                            }
                            CoreStatics.CoreApplication.MovPrescrizioneTempiSelezionata = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        CoreStatics.ExGest(ref ex, "ubSospendi_Click", this.Text);
                    }
                    finally
                    {
                        if (frmSC != null)
                        {
                            frmSC.Close();
                            frmSC.Dispose();
                        }

                                                setNavigazione(true);

                        this.ImpostaCursore(enum_app_cursors.DefaultCursor);

                    }

                    if (bAggiorna)
                    {
                        this.AggiornaGriglia(true);
                        if (this.ucEasyCheckEditorDC.Visible) this.ucEasyCheckEditorDC.Visible = !ControllaValidazioneTempi();
                    }

                    this.Cursor = CoreStatics.setCursor(enum_app_cursors.DefaultCursor);
                }
            }
        }

        private void ubApplicaFiltro_Click(object sender, EventArgs e)
        {
            if (this.ultraDockManager.FlyoutPane != null && !this.ultraDockManager.FlyoutPane.Pinned) this.ultraDockManager.FlyIn();
            this.AggiornaGriglia(false);
            this.ucEasyGrid.Focus();
        }

        private void frmPUPrescrizioni_ImmagineClick(object sender, ImmagineTopClickEventArgs e)
        {

            try
            {

                switch (e.Tipo)
                {

                    case EnumImmagineTop.Segnalibri:
                        if (CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Bookmarks > 0)
                        {
                            CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerSegnalibri);
                            _ucSegnalibri = new ucSegnalibri();
                            int iWidth = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Large)) * 40;
                            int iHeight = Convert.ToInt32((double)iWidth / 1.52D);
                            _ucSegnalibri.Size = new Size(iWidth, iHeight);
                            _ucSegnalibri.ViewInit();
                            _ucSegnalibri.Focus();
                            this.UltraPopupControlContainerSegnalibri.Show();
                        }
                        break;

                    case EnumImmagineTop.SegnalibroAdd:
                        break;

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void frmPUPrescrizioni_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {

                CoreStatics.CoreApplication.MovPrescrizioneSelezionata.MovScheda.DatiXML = this.ucDcViewer.Gestore.SchedaDatiXML;

                if (Salva(true))
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void frmPUPrescrizioni_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata.Azione == EnumAzioni.INS)
            {
                if (easyStatics.EasyMessageBox(@"La prescrizione non è stata salvata, vuoi uscire ugualmente?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    this.Close();
                }
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Close();
            }
        }

        private void frmPUPrescrizioni_Shown(object sender, EventArgs e)
        {
            this.CaricaRtf();
            this.CaricaScheda();
        }

        #endregion

        #region UltraPopupControlContainer

        private void UltraPopupControlContainer_Closed(object sender, EventArgs e)
        {
            _ucEasyGridOrari.ClickCell -= ucEasyGridOrari_ClickCell;
        }

        private void UltraPopupControlContainer_Opened(object sender, EventArgs e)
        {

            _ucEasyGridOrari.ClickCell += ucEasyGridOrari_ClickCell;
            _ucEasyGridOrari.Focus();

        }

        private void UltraPopupControlContainer_Opening(object sender, CancelEventArgs e)
        {
            Infragistics.Win.Misc.UltraPopupControlContainer popup = sender as Infragistics.Win.Misc.UltraPopupControlContainer;

            popup.PopupControl = _ucEasyGridOrari;
        }

        private void ucEasyGridOrari_ClickCell(object sender, ClickCellEventArgs e)
        {
            this.UltraPopupControlContainerMain.Close();
        }

        private void PreparaGridValidati(UltraGrid Grid, UltraGridCell Cell)
        {
            int altezza = 12;
            int larghezza = 6;

            try
            {

                _ucEasyGridOrari = null;


                _ucEasyGridOrari = CoreStatics.getGridOrariValidati(Cell.Row.Cells["ID"].Text, Cell.Row.Cells["IDPrescrizione"].Text);


                if (_ucEasyGridOrari != null && _ucEasyGridOrari.DataSource != null)
                {
                    
                    _ucEasyGridOrari.Size = new Size(Grid.Width * larghezza / 10, Grid.Height * altezza / 8);

                    _ucEasyGridOrari.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                    CoreStatics.SetEasyUltraGridLayout(ref _ucEasyGridOrari);
                    _ucEasyGridOrari.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.True;

                                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerMain);

                    Infragistics.Win.UIElement uie = Cell.GetUIElement();
                    Point oPoint = new Point(uie.Rect.Left, uie.Rect.Top);


                    this.UltraPopupControlContainerMain.Show(Grid, Grid.PointToScreen(oPoint));


                                        _ucEasyGridOrari.DisplayLayout.Bands[0].ClearGroupByColumns();

                    foreach (UltraGridColumn oCol in _ucEasyGridOrari.DisplayLayout.Bands[0].Columns)
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {
                            case "DataRiferimento":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "Data";
                                oCol.Format = "dddd dd/MM/yyyy HH:mm";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.Header.VisiblePosition = 1;
                                break;

                            case "DescStato":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "Stato";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.Header.VisiblePosition = 2;
                                break;

                            case "DescTipo":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "Tipo";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                oCol.Header.VisiblePosition = 3;
                                break;

                            default:
                                oCol.Hidden = true;
                                break;
                        }
                    }

                                        _ucEasyGridOrari.DisplayLayout.Bands[0].Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
                    _ucEasyGridOrari.Refresh();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "PreparaGridValidati", this.Name);
            }
        }

        private void PreparaGridDaValidare(UltraGrid Grid, UltraGridCell Cell)
        {
            bool bVisualizzaOraFine = false;
            bool bVisualizzaEtichetta = false;
            int altezza = 12;
            int larghezza = 3;

            try
            {

                _ucEasyGridOrari = null;


                _ucEasyGridOrari = CoreStatics.getGridOrariDaValidare(Cell.Row.Cells["ID"].Text, Cell.Row.Cells["IDPrescrizione"].Text, ref bVisualizzaOraFine, ref bVisualizzaEtichetta);

                if (_ucEasyGridOrari != null && _ucEasyGridOrari.DataSource != null)
                {
                                        if (bVisualizzaOraFine == true)
                        larghezza = larghezza + 3;
                    if (bVisualizzaEtichetta == true)
                        larghezza = larghezza + 4;

                    _ucEasyGridOrari.Size = new Size(Grid.Width * larghezza / 10, Grid.Height * altezza / 10);

                    _ucEasyGridOrari.DataRowFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                    CoreStatics.SetEasyUltraGridLayout(ref _ucEasyGridOrari);

                                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerMain);

                    Infragistics.Win.UIElement uie = Cell.GetUIElement();
                    Point oPoint = new Point(uie.Rect.Left, uie.Rect.Top);


                    this.UltraPopupControlContainerMain.Show(Grid, Grid.PointToScreen(oPoint));


                                        _ucEasyGridOrari.DisplayLayout.Bands[0].ClearGroupByColumns();

                    foreach (UltraGridColumn oCol in _ucEasyGridOrari.DisplayLayout.Bands[0].Columns)
                    {

                        oCol.SortIndicator = SortIndicator.Disabled;
                        switch (oCol.Key)
                        {
                            case "DataOraInizio":
                                oCol.Hidden = false;
                                oCol.Header.Caption = "Data/Ora Inizio";
                                oCol.Format = "dddd dd/MM/yyyy HH:mm";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                break;

                            case "DataOraFine":
                                oCol.Hidden = !bVisualizzaOraFine;
                                oCol.Header.Caption = "Data/Ora Fine";
                                oCol.Format = "dddd dd/MM/yyyy HH:mm";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Center;
                                break;

                            case "EtichettaTempo":
                                oCol.Hidden = !bVisualizzaEtichetta;
                                oCol.Header.Caption = "Descrizione Somministrazione";
                                oCol.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
                                break;

                            default:
                                oCol.Hidden = true;
                                break;
                        }
                    }

                                        _ucEasyGridOrari.DisplayLayout.Bands[0].Layout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
                    _ucEasyGridOrari.Refresh();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "PreparaGridDaValidare", this.Name);
            }
        }

        #endregion

        #region Events UserControl

        void ucDcViewer_KeyEvent(System.Windows.Input.Key key)
        {

            try
            {

                if (key == System.Windows.Input.Key.F1)
                {
                    frmPUPrescrizioni_PulsanteIndietroClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Indietro));
                }
                else if (key == System.Windows.Input.Key.F12)
                {
                    frmPUPrescrizioni_PulsanteAvantiClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti));
                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucDcViewer_KeyEvent", this.Text);
            }

        }

        void ucDcViewer_RtfEvent(string id)
        {

            try
            {

                string codua = "";

                                if (CoreStatics.CoreApplication.MovPrescrizioneSelezionata != null)
                {
                    codua = CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodUA;
                }

                                CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato = new MovTestoPredefinito(UnicodeSrl.Scci.Enums.EnumEntita.SCH.ToString(), codua, CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice,
                                                                                                    CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodTipoPrescrizione, id);
                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(Scci.Enums.EnumMaschere.TestiPredefiniti) == System.Windows.Forms.DialogResult.OK)
                {
                                                            
                    string sRTFOriginale = this.ucDcViewer.Gestore.LeggeValore(id).ToString();
                    string sRTFDaAccodare = CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato.RitornoRTF;
                    UnicodeSrl.Scci.RTF.RtfFiles rtf = new UnicodeSrl.Scci.RTF.RtfFiles();
                    sRTFOriginale = rtf.joinRtf(sRTFDaAccodare, sRTFOriginale, true);
                    rtf = null;
                    this.ucDcViewer.Gestore.ModificaValore(id, sRTFOriginale);

                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucDcViewer_RtfEvent", this.Text);
            }

        }

        void ucDcViewer_ButtonEvent(string id)
        {

            string _codua = string.Empty;

            try
            {

                this.ImpostaCursore(enum_app_cursors.WaitCursor);
                this.Tag = id;
                if (CoreStatics.CoreApplication.Trasferimento != null)
                {
                    _codua = CoreStatics.CoreApplication.Trasferimento.CodUA;
                }
                else
                {
                    _codua = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata;
                }
                string[] azioni = id.Split('.');
                string[] campo = azioni[2].Split('_');
                string azione = string.Format("PRF{0}.{1}", CoreStatics.CoreApplication.MovPrescrizioneSelezionata.CodTipoPrescrizione, campo[0]);
                                object[] myparam = new object[5] { this, campo[0], int.Parse(campo[1]), this.ucDcViewer.Gestore, azioni[0] };

                Risposta oRisposta = PluginClientStatics.PluginClient(azione, myparam, CommonStatics.UAPadri(_codua, CoreStatics.CoreApplication.Ambiente));
                if (oRisposta.Successo == true)
                {
                }
                else
                {
                    if (oRisposta.ex != null)
                    {
                        Exception rex = oRisposta.ex;
                        CoreStatics.ExGest(ref rex, @"Si è verificato un errore nell'elaborazione della procedura.", "ucDcViewer_ButtonEvent", this.Text, true, this.Name, "Errore", MessageBoxIcon.Warning, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        easyStatics.EasyMessageBox(oRisposta.Parameters[0].ToString(), azione, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                this.Tag = null;


            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucDcViewer_ButtonEvent", this.Text);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

        #endregion

        private void ucEasyGrid_MouseEnterElement(object sender, Infragistics.Win.UIElementEventArgs e)
        {

                                                
        }

        private void ucEasyGrid_MouseLeaveElement(object sender, Infragistics.Win.UIElementEventArgs e)
        {


        }

        private void PopUpMenu(Infragistics.Win.UltraWinGrid.CellButtonUIElement element)
        {

            try
            {

                var menu = (PopupMenuTool)this.UltraToolbarsManager.Tools["PopupMenuTool"];

                ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_EDIT]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_MODIFICA_32);
                if (_bFirmaDigitale)
                {
                    ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_VALID]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAFIRMA_32);
                    ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_SOSPENDI]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_SOSPENDITESSERA_32);
                }
                else
                {
                    ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_VALID]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FIRMA_32);
                    ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_SOSPENDI]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_SOSPENDI_32);
                }

                if (_bFirmaDigitale &&
                    (element.Row.Cells[@"CodStatoPrescrizioneTempi"].Text.Trim().ToUpper() == EnumStatoPrescrizioneTempi.VA.ToString() || element.Row.Cells[@"CodStatoPrescrizioneTempi"].Text.Trim().ToUpper() == EnumStatoPrescrizioneTempi.SS.ToString()))
                    ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_DEL]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_CANCELLATESSERA_32);
                else
                    ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_DEL]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_ELIMINA_32);

                ((ButtonTool)menu.Tools[CoreStatics.C_COL_BTN_COPY]).InstanceProps.AppearancesLarge.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_COPIA_32);

                menu.Tools[CoreStatics.C_COL_BTN_EDIT].InstanceProps.Visible = (element.Row.Cells["PermessoModifica"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);
                menu.Tools[CoreStatics.C_COL_BTN_VALID].InstanceProps.Visible = (element.Row.Cells["PermessoDaValidare"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);
                menu.Tools[CoreStatics.C_COL_BTN_SOSPENDI].InstanceProps.Visible = (element.Row.Cells["PermessoAnnulla"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);
                menu.Tools[CoreStatics.C_COL_BTN_DEL].InstanceProps.Visible = (element.Row.Cells["PermessoCancella"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);
                menu.Tools[CoreStatics.C_COL_BTN_COPY].InstanceProps.Visible = (element.Row.Cells["PermessoCopia"].Text == "1" ? Infragistics.Win.DefaultableBoolean.True : Infragistics.Win.DefaultableBoolean.False);

                ((PopupMenuTool)this.UltraToolbarsManager.Tools["PopupMenuTool"]).ClosePopup();
                ((PopupMenuTool)this.UltraToolbarsManager.Tools["PopupMenuTool"]).ShowPopup();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucEasyGrid_ClickCellButton", this.Name);
            }

        }

        #region UltraPopupControlContainerSegnalibri

        private void UltraPopupControlContainerSegnalibri_Closed(object sender, EventArgs e)
        {
            _ucSegnalibri.SegnalibriClick -= UltraPopupControlContainerSegnalibri_ModificaClick;
        }

        private void UltraPopupControlContainerSegnalibri_Opened(object sender, EventArgs e)
        {
            _ucSegnalibri.SegnalibriClick += UltraPopupControlContainerSegnalibri_ModificaClick;
            _ucSegnalibri.Focus();
        }

        private void UltraPopupControlContainerSegnalibri_Opening(object sender, CancelEventArgs e)
        {
            UltraPopupControlContainer popup = sender as UltraPopupControlContainer;
            popup.PopupControl = _ucSegnalibri;
        }

        private void UltraPopupControlContainerSegnalibri_ModificaClick(object sender, SegnalibriClickEventArgs e)
        {

            try
            {

                switch (e.Pulsante)
                {

                    case EnumPulsanteSegnalibri.Modifica:
                        this.UltraPopupControlContainerSegnalibri.Close();
                        this.ucTopModale.Focus();
                        CoreStatics.CaricaPopup(EnumMaschere.Scheda, EnumEntita.SCH, e.ID);
                        break;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        #endregion

    }
}
