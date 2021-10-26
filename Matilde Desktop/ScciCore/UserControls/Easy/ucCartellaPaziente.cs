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
    public partial class ucCartellaPaziente : UserControl, Interfacce.IViewUserControlMiddle
    {

        private ucRichTextBox _ucRichTextBox = null;

        ClrThreadPool m_pool = new ClrThreadPool();

        public ucCartellaPaziente()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);


            m_pool.SynchronizationContext = SynchronizationContext.Current;
        }

        #region Interface

        public void Aggiorna()
        {

            try
            {

                this.UltraPanel.Visible = false;
                this.CaricaUcRtf();
                this.UltraPanel.Visible = true;
                this.ResumeLayout();

            }
            catch (Exception ex)
            {
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

                this.ucRtfAlertGenerici.Titolo = "Warning:";
                this.ucRtfAlertGenerici.Immagine = Risorse.GetImageFromResource(Risorse.GC_ALERTGENERICO_256);

                this.ucRtfDatiMancanti.Titolo = "Dati mancanti:";
                this.ucRtfDatiMancanti.Immagine = Risorse.GetImageFromResource(Risorse.GC_DATOMANCANTE_256);

                this.ucRtfDatiRilievo.Titolo = "Dati Di Rilievo:";
                this.ucRtfDatiRilievo.Immagine = Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_256);

                this.ucRtfParametriVitali.Titolo = "Parametri Vitali:";
                this.ucRtfParametriVitali.Immagine = Risorse.GetImageFromResource(Risorse.GC_PARAMETRIVITALI_256);

                this.ucRtfTaskInfermieristici.Titolo = "Worklist:";
                this.ucRtfTaskInfermieristici.Immagine = Risorse.GetImageFromResource(Risorse.GC_WORKLIST_256);


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

                if (ucRtfAlertGenerici != null)
                {
                    this.ucRtfAlertGenerici.Dati = null;
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

                if (this.ucRtfParametriVitali != null)
                {
                    this.ucRtfParametriVitali.Dati = null;
                }

                if (ucRtfTaskInfermieristici != null)
                {
                    this.ucRtfTaskInfermieristici.Dati = null;
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


                if (m_pool.ThreadPoolState != enumThreadState.Idle)
                    return;

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
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                op.Parametro.Add("VisualizzazioneSintetica", "1");
                if (int.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordWarning)) != 0)
                {
                    op.Parametro.Add("NumRighe", UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordWarning));
                }
                op.TimeStamp.CodEntita = EnumEntita.ALG.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                WorkerRunStoredProcedure wk_SelMovAlertGenerici = CreateWorker("MSP_SelMovAlertGenerici", op.ToXmlString(), cb_MSP_SelMovAlertGenerici);

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                op.Parametro.Add("Storicizzata", "NO");
                op.ParametroRipetibile.Add("CodEntita", new string[2] { EnumEntita.PAZ.ToString(), EnumEntita.EPI.ToString() });
                op.Parametro.Add("SoloRTF", "1");
                op.Parametro.Add("SoloDatiMancantiRTF", "1");

                WorkerRunStoredProcedure wk_SelMovSchedaAvanzato = CreateWorker("MSP_SelMovSchedaAvanzato", op.ToXmlString(), cb_MSP_SelMovSchedaAvanzato);

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                op.Parametro.Add("Storicizzata", "NO");
                op.Parametro.Add("SoloRTF", "1");
                op.Parametro.Add("SoloDatiInRilievoRTF", "1");
                op.ParametroRipetibile.Add("CodEntita", new string[2] { EnumEntita.PAZ.ToString(), EnumEntita.EPI.ToString() });
                op.TimeStamp.CodEntita = EnumEntita.SCH.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                WorkerRunStoredProcedure wk_SelMovDatiRilievo = CreateWorker("MSP_SelMovSchedaAvanzato", op.ToXmlString(), cb_MSP_SelMovDatiRilievo);

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                op.Parametro.Add("CodStatoParametroVitale", EnumStatoParametroVitale.ER.ToString());
                op.Parametro.Add("VisualizzazioneSintetica", "1");
                if (int.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordParametriVitali)) != 0)
                {
                    op.Parametro.Add("NumRighe", UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordParametriVitali));
                }
                op.TimeStamp.CodEntita = EnumEntita.PVT.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                WorkerRunStoredProcedure wk_SelMovParametriVitali = CreateWorker("MSP_SelMovParametriVitali", op.ToXmlString(), cb_MSP_SelMovParametriVitali);

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                op.Parametro.Add("CodStatoTaskInfermieristico", "PR");
                op.Parametro.Add("DataProgrammataInizio", CoreStatics.getDateTimeNow());
                op.Parametro.Add("DataProgrammataFine", CoreStatics.GC_DATAFINE);
                op.Parametro.Add("TaskInRitardo", "1");
                op.Parametro.Add("VisualizzazioneSintetica", "1");
                if (int.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordTaskInfermieristici)) != 0)
                {
                    op.Parametro.Add("NumRighe", UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordTaskInfermieristici));
                }
                op.TimeStamp.CodEntita = EnumEntita.WKI.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                WorkerRunStoredProcedure wk_SelMovTaskInfermieristici = CreateWorker("MSP_SelMovTaskInfermieristici", op.ToXmlString(), cb_MSP_SelMovTaskInfermieristici);

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                op.Parametro.Add("CodStatoEvidenzaClinica", "CM");
                op.Parametro.Add("CodStatoEvidenzaClinicaVisione", "DV");
                op.Parametro.Add("VisualizzazioneSintetica", "1");
                if (int.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordEvidenzaClinica)) != 0)
                {
                    op.Parametro.Add("NumRighe", UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordEvidenzaClinica));
                }
                op.TimeStamp.CodEntita = EnumEntita.EVC.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                WorkerRunStoredProcedure wk_SelMovEvidenzaClinica = CreateWorker("MSP_SelMovEvidenzaClinica", op.ToXmlString(), cb_MSP_SelMovEvidenzaClinica);

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDEpisodio", CoreStatics.CoreApplication.Episodio.ID);
                op.Parametro.Add("VisualizzazioneSintetica", "1");
                if (int.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordOrdini)) != 0)
                {
                    op.Parametro.Add("NumRighe", UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordOrdini));
                }
                op.TimeStamp.CodEntita = EnumEntita.OE.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                WorkerRunStoredProcedure wk_SelMovOrdini = CreateWorker("MSP_SelMovOrdini", op.ToXmlString(), cb_MSP_SelMovOrdini);

                op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);
                op.Parametro.Add("DataInizio", CoreStatics.getDateTimeNow());
                op.Parametro.Add("DataFine", CoreStatics.GC_DATAFINE);
                op.Parametro.Add("CodStatoAppuntamento", "PR");
                op.Parametro.Add("VisualizzazioneSintetica", "1");
                if (int.Parse(UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordAppuntamenti)) != 0)
                {
                    op.Parametro.Add("NumRighe", UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.NumeroRecordAppuntamenti));
                }
                op.TimeStamp.CodEntita = EnumEntita.APP.ToString(); op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString();
                WorkerRunStoredProcedure wk_SelMovAppuntamenti = CreateWorker("MSP_SelMovAppuntamenti", op.ToXmlString(), cb_MSP_SelMovAppuntamenti);


                m_pool.QueueWorker(wk_SelMovAlertAllergieAnamnesi);
                m_pool.QueueWorker(wk_SelMovAlertGenerici);
                m_pool.QueueWorker(wk_SelMovSchedaAvanzato);
                m_pool.QueueWorker(wk_SelMovDatiRilievo);
                m_pool.QueueWorker(wk_SelMovParametriVitali);
                m_pool.QueueWorker(wk_SelMovTaskInfermieristici);
                m_pool.QueueWorker(wk_SelMovEvidenzaClinica);
                m_pool.QueueWorker(wk_SelMovOrdini);
                m_pool.QueueWorker(wk_SelMovAppuntamenti);

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

                Size oSizeSX = new Size(this.UltraPanel.Width / 2, this.UltraPanel.Height / 5);
                Size oSizeDX = new Size(this.UltraPanel.Width / 2, this.UltraPanel.Height / 5);

                Point oLocationSX = new Point(0, 0);
                Point oLocationDX = new Point(this.UltraPanel.Width / 2, 0);

                this.ucRtfAlertAllergieAnamnesi.Size = oSizeSX;
                this.ucRtfAlertAllergieAnamnesi.Location = oLocationSX;
                oLocationSX.Y += oSizeSX.Height;

                this.ucRtfAlertGenerici.Size = oSizeSX;
                this.ucRtfAlertGenerici.Location = oLocationSX;
                oLocationSX.Y += oSizeSX.Height;

                this.ucRtfDatiMancanti.Size = oSizeSX;
                this.ucRtfDatiMancanti.Location = oLocationSX;
                oLocationSX.Y += oSizeSX.Height;

                this.ucRtfDatiRilievo.Size = new Size(oSizeSX.Width, (oSizeSX.Height * 2));
                this.ucRtfDatiRilievo.Location = oLocationSX;
                oLocationSX.Y += oSizeSX.Height * 2;


                this.ucRtfParametriVitali.Size = oSizeDX;
                this.ucRtfParametriVitali.Location = oLocationDX;
                oLocationDX.Y += oSizeDX.Height;

                this.ucRtfTaskInfermieristici.Size = oSizeDX;
                this.ucRtfTaskInfermieristici.Location = oLocationDX;
                oLocationDX.Y += oSizeDX.Height;

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

        private void ucRtfOrdini_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {


            foreach (UltraGridColumn oCol in e.Layout.Bands[0].Columns)
            {
                try
                {
                    switch (oCol.Key)
                    {
                        case "Data":
                        case "Eroganti":
                        case "Prestazioni":
                            oCol.Hidden = false;
                            break;

                        default:
                            oCol.Hidden = true;
                            break;
                    }

                }
                catch (Exception ex)
                {
                    string aa = ex.Message;
                }
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
                    e.Layout.Bands[0].Columns["Descrizione"].CellAppearance.FontData.SizeInPoints = easyStatics.getFontSizeInPointsFromRelativeDimension(((ucEasyGrid)e.Layout.Grid).DataRowFontRelativeDimension) / (float)1.5;
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

                this.ucRtfAlertAllergieAnamnesi.ColonnaRTFResize = "AnteprimaRTF";
                this.ucRtfAlertAllergieAnamnesi.Dati = dt;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_SelMovAlertAllergieAnamnesi", this.Name);
            }

        }

        private void cb_MSP_SelMovAlertGenerici(object dataObject)
        {

            if (dataObject == null)
                return;

            try
            {
                DataTable dt = (DataTable)dataObject;
                this.ucRtfAlertGenerici.ColonnaRTFResize = "AnteprimaRTF";
                this.ucRtfAlertGenerici.Dati = dt;
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_SelMovAlertGenerici", this.Name);
            }

        }

        private void cb_MSP_SelMovSchedaAvanzato(object dataObject)
        {

            if (dataObject == null)
                return;

            try
            {
                DataTable dt = (DataTable)dataObject;
                this.ucRtfDatiMancanti.ColonnaRTFResize = "DatiObbligatoriMancantiRTF";
                this.ucRtfDatiMancanti.Dati = dt;
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
                this.ucRtfDatiRilievo.ColonnaRTFResize = "DatiRilievoRTF";
                this.ucRtfDatiRilievo.FattoreRidimensionamentoRTF = 29; this.ucRtfDatiRilievo.Dati = dt;
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_SelMovDatiRilievo", this.Name);
            }

        }

        private void cb_MSP_SelMovParametriVitali(object dataObject)
        {

            if (dataObject == null)
                return;

            try
            {
                DataTable dt = (DataTable)dataObject;
                this.ucRtfParametriVitali.ColonnaRTFResize = "AnteprimaRTF";
                this.ucRtfParametriVitali.Dati = dt;
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_SelMovParametriVitali", this.Name);
            }

        }

        private void cb_MSP_SelMovTaskInfermieristici(object dataObject)
        {

            if (dataObject == null)
                return;

            try
            {
                DataTable dt = (DataTable)dataObject;
                this.ucRtfTaskInfermieristici.ColonnaRTFResize = "AnteprimaRTF";
                this.ucRtfTaskInfermieristici.Dati = dt;
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_SelMovTaskInfermieristici", this.Name);
            }

        }

        private void cb_MSP_SelMovEvidenzaClinica(object dataObject)
        {

            if (dataObject == null)
                return;

            try
            {
                DataTable dt = (DataTable)dataObject;
                this.ucRtfEvidenzaClinica.Dati = dt;
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_SelMovEvidenzaClinica", this.Name);
            }

        }

        private void cb_MSP_SelMovOrdini(object dataObject)
        {

            if (dataObject == null)
                return;

            try
            {
                DataTable dt = (DataTable)dataObject;
                this.ucRtfOrdini.Dati = dt;
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
                this.ucRtfAppuntamenti.Dati = dt;
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "cb_MSP_SelMovAppuntamenti", this.Name);
            }

        }

        #endregion  ThreadingProc

    }
}
