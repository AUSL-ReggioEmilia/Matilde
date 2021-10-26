using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinGrid;
using UnicodeSrl.Framework.Data;
using Infragistics.Win;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci;

using UnicodeSrl.Framework.Threading;
using UnicodeSrl.ScciCore.ThreadingObj;
using System.Threading;

namespace UnicodeSrl.ScciCore
{
    public partial class ucCartellaAmbulatoriale : UserControl, Interfacce.IViewUserControlMiddle
    {

        private ucRichTextBox _ucRichTextBox = null;

        public ucCartellaAmbulatoriale()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
        }

        #region Interface

        public void Aggiorna()
        {

            try
            {
                CoreStatics.SetNavigazione(false);

                if (CoreStatics.CoreApplication.Paziente.ID != string.Empty)
                {
                    this.UltraPanel.Visible = false;
                    this.CaricaUcRtf();
                    this.UltraPanel.Visible = true;
                    this.ResumeLayout();
                }
                else
                {
                    easyStatics.EasyErrorMessageBox("Errore nel caricamento della scheda paziente.", @"Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                CoreStatics.SetNavigazione(true);
            }
            catch (Exception ex)
            {
                CoreStatics.SetNavigazione(true);

                CoreStatics.ExGest(ref ex, "Aggiorna", this.Name);
            }

        }

        public void Carica()
        {

            try
            {

                this.SuspendLayout();

                this.ucRtfAlertAllergieAnamnesi.Titolo = "Note Anamnestiche:";
                this.ucRtfAlertAllergieAnamnesi.Immagine = Risorse.GetImageFromResource(Risorse.GC_ALERTALLERGIA_256);

                this.ucRtfDatiMancanti.Titolo = "Dati mancanti:";
                this.ucRtfDatiMancanti.Immagine = Risorse.GetImageFromResource(Risorse.GC_DATOMANCANTE_256);

                this.ucRtfDatiRilievo.Titolo = "Dati Di Rilievo:";
                this.ucRtfDatiRilievo.Immagine = Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_256);


                this.ucRtfEvidenzaClinica.Titolo = "Evidenza Clinica:";
                this.ucRtfEvidenzaClinica.Immagine = Risorse.GetImageFromResource(Risorse.GC_EVIDENZACLINICA_256);

                this.ucRtfOrdini.Titolo = "Ultimi Ordini:";
                this.ucRtfOrdini.Immagine = Risorse.GetImageFromResource(Risorse.GC_ORDINE_256);

                this.ucRtfAppuntamenti.Titolo = "Agende:";
                this.ucRtfAppuntamenti.Immagine = Risorse.GetImageFromResource(Risorse.GC_APPUNTAMENTO_256);

                this.Aggiorna();

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

        }

        public void Ferma()
        {

            try
            {
                SvuotaUcRTF();
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Ferma", this.Name);
            }

        }

        #endregion

        #region SubRoutine

        public void SvuotaUcRTF()
        {
            try
            {
                if (this.ucRtfAlertAllergieAnamnesi != null)
                {
                    this.ucRtfAlertAllergieAnamnesi.Dati = null;
                }

                if (this.ucRtfAppuntamenti != null)
                {
                    this.ucRtfAppuntamenti.Dati = null;
                }

                if (this.ucRtfDatiMancanti != null)
                {
                    this.ucRtfDatiMancanti.Dati = null;
                }

                if (this.ucRtfDatiRilievo != null)
                {
                    this.ucRtfDatiRilievo.Dati = null;
                }

                if (ucRtfEvidenzaClinica != null)
                {
                    this.ucRtfEvidenzaClinica.Dati = null;
                }

                if (this.ucRtfOrdini != null)
                {
                    this.ucRtfOrdini.Dati = null;
                }


            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "SvuotaUcRTF", this.Name);
            }

        }

        public void CaricaUcRtf()
        {

            try
            {

                ClrThreadPool pool = new ClrThreadPool();
                pool.SynchronizationContext = SynchronizationContext.Current;

                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                op.Parametro.Add("VisualizzazioneSintetica", "1");
                if (int.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordAllergie)) != 0)
                {
                    op.Parametro.Add("NumRighe", UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordAllergie));
                }
                op.TimeStamp.CodEntita = EnumEntita.ALA.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                WorkerRunStoredProcedure wk_SelMovAlertAllergieAnamnesi = CreateWorker("MSP_SelMovAlertAllergieAnamnesi", op.ToXmlString(), cb_MSP_SelMovAlertAllergieAnamnesi);

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", "NULL");
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                op.Parametro.Add("Storicizzata", "NO");
                op.Parametro.Add("CodEntita", EnumEntita.PAZ.ToString());
                op.Parametro.Add("SoloRTF", "1");
                op.Parametro.Add("SoloDatiMancantiRTF", "1");

                WorkerRunStoredProcedure wk_SelMovSchedaAvanzato = CreateWorker("MSP_SelMovSchedaAvanzato", op.ToXmlString(), cb_MSP_SelMovSchedaAvanzato);

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", "NULL");
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                op.Parametro.Add("Storicizzata", "NO");
                op.Parametro.Add("SoloRTF", "1");
                op.Parametro.Add("SoloDatiInRilievoRTF", "1");
                op.Parametro.Add("CodEntita", EnumEntita.PAZ.ToString());
                op.TimeStamp.CodEntita = EnumEntita.SCH.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                WorkerRunStoredProcedure wk_SelMovDatiRilievo = CreateWorker("MSP_SelMovSchedaAvanzato", op.ToXmlString(), cb_MSP_SelMovDatiRilievo);

                DateTime dtinizio = DateTime.MinValue;
                if (int.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroGiorniEvidenzaClinica)) != 0)
                    dtinizio = DateTime.Now.AddDays(-int.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroGiorniEvidenzaClinica)));

                bool bSACConsensiAbilita = Convert.ToBoolean(Convert.ToInt32(Database.GetConfigTable(EnumConfigTable.SACConsensiAbilita)));
                if (bSACConsensiAbilita)
                {
                    if (CoreStatics.CoreApplication.Paziente.PazientiConsensiDossierStorico.CodStatoConsenso == EnumStatoConsenso.SI.ToString())
                    {
                        DataSet dsEC = DBUtils.getEvidenzaClinicaDataset(false, true, false, false, CoreStatics.CoreApplication.Paziente.ID, "", CoreStatics.CoreApplication.Paziente.CodSACFuso, true, false, new List<string>(), dtinizio, DateTime.MinValue, null);
                        this.ucRtfEvidenzaClinica.Dati = dsEC.Tables[0];
                    }
                    else
                    {
                        this.ucRtfEvidenzaClinica.Dati = null;
                    }

                }
                else
                {
                    DataSet dsEC = DBUtils.getEvidenzaClinicaDataset(false, true, false, false, CoreStatics.CoreApplication.Paziente.ID, "", CoreStatics.CoreApplication.Paziente.CodSACFuso, true, false, new List<string>(), dtinizio, DateTime.MinValue, null);
                    this.ucRtfEvidenzaClinica.Dati = dsEC.Tables[0];
                }

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);

                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                op.Parametro.Add("VisualizzazioneSintetica", "1");
                if (int.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordOrdini)) != 0)
                {
                    op.Parametro.Add("NumRighe", UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordOrdini));
                }
                op.TimeStamp.CodEntita = EnumEntita.OE.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                WorkerRunStoredProcedure wk_SelMovOrdini = CreateWorker("MSP_SelMovOrdini", op.ToXmlString(), cb_MSP_SelMovOrdini);

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", "NULL");
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                op.Parametro.Add("DataInizio", CoreStatics.getDateTimeNow());
                op.Parametro.Add("DataFine", CoreStatics.GC_DATAFINE);
                op.Parametro.Add("VisualizzazioneSintetica", "1");
                if (int.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordAppuntamenti)) != 0)
                {
                    op.Parametro.Add("NumRighe", UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordAppuntamenti));
                }
                op.TimeStamp.CodEntita = EnumEntita.APP.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                WorkerRunStoredProcedure wk_SelMovAppuntamenti = CreateWorker("MSP_SelMovAppuntamenti", op.ToXmlString(), cb_MSP_SelMovAppuntamenti);

                pool.QueueWorker(wk_SelMovAlertAllergieAnamnesi);
                pool.QueueWorker(wk_SelMovSchedaAvanzato);
                pool.QueueWorker(wk_SelMovDatiRilievo);
                pool.QueueWorker(wk_SelMovOrdini);
                pool.QueueWorker(wk_SelMovAppuntamenti);

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region Events

        private void UltraPanel_Resize(object sender, EventArgs e)
        {

            try
            {

                this.SuspendLayout();

                Size oSizeSX = new Size(this.UltraPanel.Width / 2, this.UltraPanel.Height / 3); Size oSizeDX = new Size(this.UltraPanel.Width / 2, this.UltraPanel.Height / 3);

                Point oLocationSX = new Point(0, 0);
                Point oLocationDX = new Point(this.UltraPanel.Width / 2, 0);

                this.ucRtfAlertAllergieAnamnesi.Size = oSizeSX;
                this.ucRtfAlertAllergieAnamnesi.Location = oLocationSX;
                oLocationSX.Y += oSizeSX.Height;

                this.ucRtfDatiMancanti.Size = oSizeSX;
                this.ucRtfDatiMancanti.Location = oLocationSX;
                oLocationSX.Y += oSizeSX.Height;

                this.ucRtfDatiRilievo.Size = oSizeSX;
                this.ucRtfDatiRilievo.Location = oLocationSX;
                oLocationSX.Y += oSizeSX.Height;


                this.ucRtfEvidenzaClinica.Size = oSizeDX;
                this.ucRtfEvidenzaClinica.Location = oLocationDX;
                oLocationDX.Y += oSizeDX.Height;

                this.ucRtfOrdini.Size = oSizeDX;
                this.ucRtfOrdini.Location = oLocationDX;
                oLocationDX.Y += oSizeDX.Height;

                this.ucRtfAppuntamenti.Size = oSizeDX;
                this.ucRtfAppuntamenti.Location = oLocationDX;
                oLocationDX.Y += oSizeDX.Height;

                this.ResumeLayout();

            }
            catch (Exception)
            {

            }

        }

        private void ucRtf_ClickCell(object sender, ClickCellEventArgs e)
        {

            Infragistics.Win.UIElement uie;
            Point oPoint;

            try
            {

                switch (e.Cell.Column.Key)
                {

                    case "AnteprimaRTF":
                    case "DatiObbligatoriMancantiRTF":
                    case "DatiRilievoRTF":
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                        _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Text);

                        uie = e.Cell.GetUIElement();
                        oPoint = new Point(uie.Rect.Left + ((ucEasyGrid)sender).Parent.Parent.Parent.Location.X, uie.Rect.Top + ((ucEasyGrid)sender).Parent.Parent.Parent.Location.Y);

                        this.UltraPopupControlContainer.Show((ucEasyGrid)sender, oPoint);
                        break;

                    case "Eroganti":
                    case "Prestazioni":
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainer);
                        _ucRichTextBox = CoreStatics.GetRichTextBoxForPopupControlContainer(e.Cell.Text, false);

                        uie = e.Cell.GetUIElement();
                        oPoint = new Point(uie.Rect.Left + ((ucEasyGrid)sender).Parent.Parent.Parent.Location.X, uie.Rect.Top + ((ucEasyGrid)sender).Parent.Parent.Parent.Location.Y);

                        this.UltraPopupControlContainer.Show((ucEasyGrid)sender, oPoint);
                        break;

                    default:
                        break;

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "ucRtf_ClickCell", this.Name);
            }

        }

        private void ucRtf_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

            try
            {


                if (e.Layout.Bands[0].Columns.Exists("ID") == true)
                {
                    e.Layout.Bands[0].Columns["ID"].Hidden = true;
                }

                if (e.Layout.Bands[0].Columns.Exists("IDEpisodio") == true)
                {
                    e.Layout.Bands[0].Columns["IDEpisodio"].Hidden = true;
                }

                if (e.Layout.Bands[0].Columns.Exists("DataEvento") == true)
                {
                    e.Layout.Bands[0].Columns["DataEvento"].CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    e.Layout.Bands[0].Columns["DataEvento"].CellMultiLine = DefaultableBoolean.True;
                }

                if (e.Layout.Bands[0].Columns.Exists("DataProgrammata") == true)
                {
                    e.Layout.Bands[0].Columns["DataProgrammata"].CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    e.Layout.Bands[0].Columns["DataProgrammata"].CellMultiLine = DefaultableBoolean.True;
                }


                if (e.Layout.Bands[0].Columns.Exists("AnteprimaTXT") == true)
                {
                    e.Layout.Bands[0].Columns["AnteprimaTXT"].Hidden = true;
                }

                if (e.Layout.Bands[0].Columns.Exists("AnteprimaRTF") == true)
                {
                    e.Layout.Bands[0].Columns["AnteprimaRTF"].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    RichTextEditor a = new RichTextEditor();
                    e.Layout.Bands[0].Columns["AnteprimaRTF"].Editor = a;

                }

                if (e.Layout.Bands[0].Columns.Exists("DataInizio") == true)
                {
                    e.Layout.Bands[0].Columns["DataInizio"].CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    e.Layout.Bands[0].Columns["DataInizio"].CellMultiLine = DefaultableBoolean.True;
                }

                if (e.Layout.Bands[0].Columns.Exists("Icona") == true)
                {
                    e.Layout.Bands[0].Columns["Icona"].Hidden = true;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void ucRtfDatiMancanti_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("ID") == true)
                {
                    e.Layout.Bands[0].Columns["ID"].Hidden = true;
                }

                if (e.Layout.Bands[0].Columns.Exists("Descrizione") == true)
                {
                    e.Layout.Bands[0].Columns["Descrizione"].CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    e.Layout.Bands[0].Columns["Descrizione"].CellMultiLine = DefaultableBoolean.True;
                }

                if (e.Layout.Bands[0].Columns.Exists("AnteprimaRTF") == true)
                {
                    e.Layout.Bands[0].Columns["AnteprimaRTF"].Hidden = true;
                }

                if (e.Layout.Bands[0].Columns.Exists("DatiRilievoRTF") == true)
                {
                    e.Layout.Bands[0].Columns["DatiRilievoRTF"].Hidden = true;
                }

                if (e.Layout.Bands[0].Columns.Exists("DatiObbligatoriMancantiRTF") == true)
                {
                    e.Layout.Bands[0].Columns["DatiObbligatoriMancantiRTF"].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    RichTextEditor a = new RichTextEditor();
                    e.Layout.Bands[0].Columns["DatiObbligatoriMancantiRTF"].Editor = a;

                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void ucRtfDatiRilievo_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

            try
            {

                if (e.Layout.Bands[0].Columns.Exists("ID") == true)
                {
                    e.Layout.Bands[0].Columns["ID"].Hidden = true;
                }

                if (e.Layout.Bands[0].Columns.Exists("Descrizione") == true)
                {
                    e.Layout.Bands[0].Columns["Descrizione"].CellAppearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    e.Layout.Bands[0].Columns["Descrizione"].CellMultiLine = DefaultableBoolean.True;
                }

                if (e.Layout.Bands[0].Columns.Exists("AnteprimaRTF") == true)
                {
                    e.Layout.Bands[0].Columns["AnteprimaRTF"].Hidden = true;
                }

                if (e.Layout.Bands[0].Columns.Exists("DatiRilievoRTF") == true)
                {
                    e.Layout.Bands[0].Columns["DatiRilievoRTF"].CellMultiLine = Infragistics.Win.DefaultableBoolean.True;
                    RichTextEditor a = new RichTextEditor();
                    e.Layout.Bands[0].Columns["DatiRilievoRTF"].Editor = a;

                }

                if (e.Layout.Bands[0].Columns.Exists("DatiObbligatoriMancantiRTF") == true)
                {
                    e.Layout.Bands[0].Columns["DatiObbligatoriMancantiRTF"].Hidden = true;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        private void ucRtfEvidenzaClinica_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                foreach (UltraGridColumn ocol in e.Layout.Bands[0].Columns)
                {
                    switch (ocol.Key)
                    {
                        case "DataReferto":
                            ocol.Hidden = false;
                            ocol.Format = @"dd/MM/yyyy";
                            ocol.CellAppearance.TextHAlign = HAlign.Center;
                            break;
                        case "DescrTipo":
                            ocol.Hidden = false;
                            break;
                        default:
                            ocol.Hidden = true;
                            break;
                    }
                }
            }
            catch (Exception)
            {
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

        #region ThreadingProc

        private WorkerRunStoredProcedure CreateWorker(string sp, string xmlParam, SendOrPostCallback cb, bool returnDataset = false)
        {
            string cnn = Database.ConnectionString;
            WorkerRunStoredProcedure wk = new WorkerRunStoredProcedure(cnn, sp, xmlParam, cb, SynchronizationContext.Current, returnDataset);

            return wk;
        }

        private void cb_MSP_SelMovAlertAllergieAnamnesi(object dataObject)
        {

            if (dataObject == null)
                return;

            try
            {
                DataTable dt = (DataTable)dataObject;
                if (this.ucRtfAlertAllergieAnamnesi != null && this.ucRtfAlertAllergieAnamnesi.IsDisposed == false)
                {
                    this.ucRtfAlertAllergieAnamnesi.ColonnaRTFResize = "AnteprimaRTF";
                    this.ucRtfAlertAllergieAnamnesi.Dati = dt;
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_SelMovAlertAllergieAnamnesi", this.Name);
            }

        }

        private void cb_MSP_SelMovSchedaAvanzato(object dataObject)
        {

            if (dataObject == null)
                return;

            try
            {
                DataTable dt = (DataTable)dataObject;
                if (this.ucRtfDatiMancanti != null && this.ucRtfDatiMancanti.IsDisposed == false)
                {
                    this.ucRtfDatiMancanti.ColonnaRTFResize = "DatiObbligatoriMancantiRTF";
                    this.ucRtfDatiMancanti.Dati = dt;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_SelMovSchedaAvanzato", this.Name);
            }

        }

        private void cb_MSP_SelMovDatiRilievo(object dataObject)
        {

            if (dataObject == null)
                return;

            try
            {
                DataTable dt = (DataTable)dataObject;
                if (this.ucRtfDatiRilievo != null && this.ucRtfDatiRilievo.IsDisposed == false)
                {
                    this.ucRtfDatiRilievo.ColonnaRTFResize = "DatiRilievoRTF";
                    this.ucRtfDatiRilievo.FattoreRidimensionamentoRTF = 29; this.ucRtfDatiRilievo.Dati = dt;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_SelMovDatiRilievo", this.Name);
            }

        }

        private void cb_MSP_SelMovOrdini(object dataObject)
        {

            if (dataObject == null)
                return;

            try
            {
                DataTable dt = (DataTable)dataObject;
                if (this.ucRtfOrdini != null && this.ucRtfOrdini.IsDisposed == false)
                {
                    this.ucRtfOrdini.Dati = dt;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_SelMovOrdini", this.Name);
            }

        }

        private void cb_MSP_SelMovAppuntamenti(object dataObject)
        {

            if (dataObject == null)
                return;

            try
            {
                DataTable dt = (DataTable)dataObject;
                if (this.ucRtfAppuntamenti != null && this.ucRtfAppuntamenti.IsDisposed == false)
                {
                    this.ucRtfAppuntamenti.Dati = dt;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_SelMovAppuntamenti", this.Name);
            }

        }

        #endregion

    }
}
