using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.CustomControls.Infra;
using UnicodeSrl.Scci.Enums;

using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Common;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci.PluginClient;
using Infragistics.Win.Misc;
using UnicodeSrl.ScciCore.ViewController;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUDiarioClinico : frmBaseModale, Interfacce.IViewFormlModal
    {

        #region DICHIARAZIONI

        private Gestore oGestore = null;

                private ucSegnalibri _ucSegnalibri = null;

        #endregion

        public frmPUDiarioClinico()
        {
            InitializeComponent();

        }

        #region INTERFACCIA

        public void Carica()
        {
            try
            {
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_DIARIOMEDICO_16);

                                this.ubScheda.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_DIZIONARI_256);

                if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.PermessoUAFirma == 1)
                {
                    this.uchkValidato.UNCheckedImage = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_TESSERAFIRMA_256);
                    this.uchkValidato.CheckedImage = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_TESSERAFIRMAESEGUITA_256);
                }
                else
                {
                    this.uchkValidato.UNCheckedImage = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_FIRMA_256);
                    this.uchkValidato.CheckedImage = Properties.Resources.FirmaEseguita_256;
                }

                this.ubScheda.PercImageFill = this.ubScheda.PercImageFill;
                this.uchkValidato.PercImageFill = this.uchkValidato.PercImageFill;
                this.uchkValidato.Text = "";

                this.utxtCodTipoVoceDiario.ReadOnly = true;
                this.utxtCodTipoVoceDiario.Visible = false;

                this.ubZoomCodTipoVoceDiario.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
                this.ubZoomCodTipoVoceDiario.Appearance.ImageHAlign = Infragistics.Win.HAlign.Center;
                this.ubZoomCodTipoVoceDiario.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_ZOOM_256);
                this.ubZoomCodTipoVoceDiario.PercImageFill = 0.9F;

                this.ubZoomCodTipoVoceDiario.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;
                this.ubZoomCodTipoVoceDiario.ShortcutKey = Keys.Z;

                                this.ubZoomCodTipoVoceDiario.Visible = (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.Azione == EnumAzioni.INS);

                                this.udteDataEvento.Value = null;
                this.utxtCodTipoVoceDiario.Text = string.Empty;
                this.uchkValidato.Checked = false;

                if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato != null)
                {
                    if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.DataEvento > DateTime.MinValue) this.udteDataEvento.Value = CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.DataEvento;
                    this.utxtCodTipoVoceDiario.Text = CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodTipoVoceDiario;
                    this.lblZoomCodTipoVoceDiario.Text = CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.DescrTipoVoceDiario;
                    this.uchkValidato.Checked = (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodStatoDiario == "VA");
                }

                this.InizializzaGestore();

                if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.Azione == EnumAzioni.VIS)
                {
                                        this.udteDataEvento.ReadOnly = true;
                    this.ubZoomCodTipoVoceDiario.Enabled = false;
                    this.uchkValidato.Enabled = false;
                    this.ubScheda.Enabled = false;
                }

                                                
                                string sIDCartella = string.Empty;
                if (CoreStatics.CoreApplication.Cartella == null)
                {
                    Trasferimento ot = new Trasferimento(CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.IDTrasferimento, CoreStatics.CoreApplication.Ambiente);
                    sIDCartella = ot.IDCartella;
                    ot = null;
                }
                else
                {                       sIDCartella = CoreStatics.CoreApplication.Cartella.ID;
                }

                CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.PathFileTemp = FileStatics.GetPathSalvataggioScheda("DCL",
                                                                                "DCL",
                                                                                sIDCartella,
                                                                                CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.MovScheda.CodScheda,
                                                                                CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.MovScheda.Versione,
                                                                                CoreStatics.CoreApplication.Sessione.Utente.Codice,
                                                                                CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.MovScheda.IDMovScheda,
                                                                                "DCL"
                                                                                );
                if (FileStatics.CheckSalvataggioScheda(CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.PathFileTemp) == true)
                {
                    if (easyStatics.EasyMessageBox("Vuoi recuperare la scheda non ancora salvata ?", "Recupero Scheda", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.MovScheda.DatiXML = FileStatics.ReadSalvataggioScheda(CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.PathFileTemp, CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.MovScheda.DatiXML);
                    }
                }

                
                this.ucTopModale.Distacco = true;

                                if ((this.CustomParamaters != null) && (this.CustomParamaters is List<String>))
                {
                    List<String> cp = this.CustomParamaters as List<String>;

                    if (cp.Contains("bloccafirma"))
                        uchkValidato.Enabled = false;
                }
                                this.ShowDialog();

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Carica", this.Text);
            }
        }

        #endregion

        #region FUNZIONI

        private void InizializzaGestore()
        {

            try
            {

                                                                oGestore = CoreStatics.GetGestore();

                this.ucDcViewer.RtfEvent -= ucDcViewer_RtfEvent;
                this.ucDcViewer.KeyEvent -= ucDcViewer_KeyEvent;
                this.ucDcViewer.ButtonEvent -= ucDcViewer_ButtonEvent;
                this.ucDcViewer.RtfEvent += ucDcViewer_RtfEvent;
                this.ucDcViewer.KeyEvent += ucDcViewer_KeyEvent;
                this.ucDcViewer.ButtonEvent += ucDcViewer_ButtonEvent;

                this.ucDcViewer.OnModifiedEvent += ucDcViewer_OnModifiedEvent;

            }
            catch (Exception ex)
            {
                throw new Exception(@"InizializzaGestore()" + Environment.NewLine + ex.Message, ex);
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
                    this.ucAnteprimaRtf.Visible = true;
                    this.CaricaRtf();
                    break;

                case true:
                    this.ucEasyTableLayoutPanelDC.ColumnStyles[0].Width = 0;
                    this.ucEasyTableLayoutPanelDC.ColumnStyles[1].Width = 100;
                    this.ehViewer.Visible = true;
                    this.CaricaScheda();
                    break;
            }

            this.ucEasyTableLayoutPanelDC.Visible = true;

        }

        private void CaricaScheda()
        {

            try
            {

                                                
                                oGestore.SchedaXML = CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.MovScheda.Scheda.StrutturaXML;

                                oGestore.SchedaLayoutsXML = CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.MovScheda.Scheda.LayoutXML;

                                oGestore.Decodifiche = CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.MovScheda.Scheda.DizionarioValori();

                                if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.MovScheda.DatiXML == string.Empty)
                {
                    oGestore.SchedaDati = new DcSchedaDati();
                }
                else
                {
                    try
                    {
                        oGestore.SchedaDatiXML = CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.MovScheda.DatiXML;
                    }
                    catch (Exception)
                    {
                        oGestore.SchedaDati = new DcSchedaDati();
                    }
                }
                if (oGestore.SchedaDati.Dati.Count == 0) { oGestore.NuovaScheda(); }

                this.ucDcViewer.VisualizzaTitoloScheda = false;

                this.ucDcViewer.CaricaDati(oGestore);

            }
            catch (Exception ex)
            {
                throw new Exception(@"CaricaScheda()" + Environment.NewLine + ex.Message, ex);
            }
        }

        private void CaricaRtf()
        {

            try
            {

                this.ucAnteprimaRtf.rtbRichTextBox.ReadOnly = true;
                this.ucAnteprimaRtf.MovScheda = CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.MovScheda;
                this.ucAnteprimaRtf.MovScheda.GeneraRTF();
                this.ucAnteprimaRtf.RefreshRTF();

            }
            catch (Exception ex)
            {
                throw new Exception(@"CaricaRtf()" + Environment.NewLine + ex.Message, ex);
            }

        }

                
                
                                
                
                
                
                                                                                                                                        
        
        
                
                                                
        private bool ControllaValori()
        {
            bool bOK = true;

                        if (bOK && this.udteDataEvento.Value == null)
            {
                easyStatics.EasyMessageBox("Inserire Data Evento!", "Diario Clinico", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.udteDataEvento.Focus();
                bOK = false;
            }
            if (bOK && this.udteDataEvento.Value != null && (DateTime)this.udteDataEvento.Value > DateTime.Now)
            {
                easyStatics.EasyMessageBox("Data Evento nel futuro!", "Diario Clinico", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.udteDataEvento.Focus();
                bOK = false;
            }
            if (bOK && (this.utxtCodTipoVoceDiario.Text == null || this.utxtCodTipoVoceDiario.Text == string.Empty || this.utxtCodTipoVoceDiario.Text.Trim() == "" || this.lblZoomCodTipoVoceDiario.Text == ""))
            {
                easyStatics.EasyMessageBox("Selezionare un Tipo Voce!", "Diario Clinico", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.ubZoomCodTipoVoceDiario.Focus();
                bOK = false;
            }

            return bOK;
        }

        private bool Salva()
        {
            bool bReturn = false;
            try
            {
                if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.Azione != EnumAzioni.VIS)
                {
                    if (ControllaValori())
                    {
                        bool bDaFirmare = false;
                        CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodTipoVoceDiario = this.utxtCodTipoVoceDiario.Text;
                        CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.DataEvento = (DateTime)this.udteDataEvento.Value;
                        if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.Azione == EnumAzioni.INS)
                        {
                                                        if (this.uchkValidato.Checked)
                            {
                                CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodStatoDiario = "VA";
                                bDaFirmare = true;                             }
                            else
                                CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodStatoDiario = "IC";
                        }
                        else
                        {
                            if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodStatoDiario != "VA" && this.uchkValidato.Checked)
                            {
                                                                bDaFirmare = true;                                 CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodStatoDiario = "VA";
                                CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.Azione = EnumAzioni.VAL;
                            }
                        }

                        if (SalvaScheda())
                        {

                            Risposta oRispostaElaboraPrima = new Risposta();
                            if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodStatoDiario == "VA")
                            {
                                                                oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.DCL_VALIDA_PRIMA_PU.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                            }
                            else
                            {
                                oRispostaElaboraPrima.Successo = true;
                            }

                            if (oRispostaElaboraPrima.Successo)
                            {

                                                                if (bDaFirmare) bDaFirmare = (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.PermessoUAFirma == 1);

                                if (bDaFirmare)
                                {

                                    bool bElimina = false;
                                    bool bContinua = true;
                                    bool bAnnullaValidazione = false;
                                    frmSmartCardProgress frmSC = null;
                                    this.ImpostaCursore(enum_app_cursors.WaitCursor);

                                    bool bEditDiVoceGiaValidata = false;
                                    if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.Azione == EnumAzioni.INS && CoreStatics.CoreApplication.MovDiarioClinicoDaAnnullare != null) bEditDiVoceGiaValidata = true;

                                    try
                                    {

                                                                                setNavigazione(false);
                                        frmSC = new frmSmartCardProgress();
                                        if (bEditDiVoceGiaValidata)
                                            frmSC.InizializzaEMostra(0, 6, this);
                                        else
                                            frmSC.InizializzaEMostra(0, 4, this);
                                                                                frmSC.SetCursore(enum_app_cursors.WaitCursor);

                                        frmSC.SetStato(@"Validazione Diario");

                                        bContinua = CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.Salva();

                                        if (bContinua)
                                        {
                                            try
                                            {
                                                bElimina = bEditDiVoceGiaValidata; 
                                                bReturn = true;                                                 bContinua = false;
                                                bAnnullaValidazione = true;

                                                                                                frmSC.SetStato(@"Generazione Documento...");

                                                                                                byte[] pdfContent = CoreStatics.GeneraPDFValidazioneDiario(CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.IDMovDiario, true);

                                                if (pdfContent == null || pdfContent.Length <= 0)
                                                {
                                                    frmSC.SetLog(@"Errore Generazione documento", true);

                                                }
                                                else
                                                {
                                                    bContinua = frmSC.ProvaAFirmare(ref pdfContent, EnumTipoDocumentoFirmato.DCLFM01, "Firma Digitale...", EnumEntita.DCL, CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.IDMovDiario);
                                                    bAnnullaValidazione = !bContinua;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                if (frmSC != null) frmSC.SetLog(@"Errore Generazione documento: " + ex.Message, true);
                                                bContinua = false;
                                            }

                                        } 
                                        if (bContinua && bEditDiVoceGiaValidata)
                                        {
                                                                                        if (CoreStatics.CoreApplication.MovDiarioClinicoDaAnnullare.Annulla())
                                            {
                                                try
                                                {
                                                    bContinua = false;

                                                                                                        frmSC.SetStato(@"Generazione Documento Annullato...");

                                                                                                        byte[] pdfAnnContent = CoreStatics.GeneraPDFValidazioneDiario(CoreStatics.CoreApplication.MovDiarioClinicoDaAnnullare.IDMovDiario, true);

                                                    if (pdfAnnContent == null || pdfAnnContent.Length <= 0)
                                                    {
                                                        frmSC.SetLog(@"Errore Generazione Documento Annullato", true);

                                                    }
                                                    else
                                                    {

                                                        bContinua = frmSC.ProvaAFirmare(ref pdfAnnContent, EnumTipoDocumentoFirmato.DCLFM01, @"Firma Digitale Documento Annullato...", EnumEntita.DCL, CoreStatics.CoreApplication.MovDiarioClinicoDaAnnullare.IDMovDiario);

                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    if (frmSC != null) frmSC.SetLog(@"Errore Generazione documento Annullato: " + ex.Message, true);
                                                    bContinua = false;
                                                }

                                                if (!bContinua)
                                                {
                                                    bAnnullaValidazione = true;

                                                    CoreStatics.CoreApplication.MovDiarioClinicoDaAnnullare.Azione = EnumAzioni.MOD;
                                                    CoreStatics.CoreApplication.MovDiarioClinicoDaAnnullare.DataAnnullamento = DateTime.MinValue;
                                                    CoreStatics.CoreApplication.MovDiarioClinicoDaAnnullare.CodStatoDiario = "VA";
                                                    CoreStatics.CoreApplication.MovDiarioClinicoDaAnnullare.Salva();
                                                }
                                            }
                                        }


                                        if (bAnnullaValidazione)
                                        {
                                            if (bElimina)
                                            {
                                                                                                                                                CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.Cancella();
                                            }
                                            else
                                            {
                                                CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodStatoDiario = "IC";
                                                CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.Azione = EnumAzioni.MOD;
                                                CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.DataValidazione = DateTime.MinValue;
                                                CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.Salva();
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        CoreStatics.ExGest(ref ex, "Salva", this.Name);
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
                                else                                 {
                                    bReturn = CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.Salva();
                                } 
                            }
                            else
                            {
                                if (oRispostaElaboraPrima.ex != null)
                                {
                                    easyStatics.EasyMessageBox(oRispostaElaboraPrima.ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }

                        }
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

        private bool SalvaScheda()
        {
            bool bReturn = true;

            try
            {

                CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.MovScheda.DatiXML = this.ucDcViewer.Gestore.SchedaDatiXML;

                if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.MovScheda.DatiObbligatoriMancantiRTF != string.Empty
                && CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.MovScheda.DatiObbligatoriMancantiRTF.Trim() != "")
                {
                    if (easyStatics.EasyMessageBox(@"Non sono stati compilati alcuni valori obbligatori della scheda!" + Environment.NewLine + @"Vuoi continuare col salvataggio?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                        bReturn = false;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Salva", this.Text);

                if (easyStatics.EasyMessageBox(@"Si è verificato un errore nel salvataggio della scheda!" + Environment.NewLine + @"Vuoi uscire ugualmente?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                    bReturn = false;
            }

            return bReturn;
        }

        private void setNavigazione(bool enable)
        {
            try
            {
                CoreStatics.SetNavigazione(enable);

                this.ucBottomModale.Enabled = enable;
                this.ubScheda.Enabled = enable;
                this.ubZoomCodTipoVoceDiario.Enabled = enable;
                this.uchkValidato.Enabled = enable;
                this.ucAnteprimaRtf.Enabled = enable;

            }
            catch
            {
                CoreStatics.SetNavigazione(true);
                this.ucBottomModale.Enabled = true;
            }
        }

        #endregion

        #region Events Form

        private void frmPUDiarioClinico_Shown(object sender, EventArgs e)
        {
            if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.Azione == EnumAzioni.VIS)
            {
                this.SetDC(false);
            }
            else
            {
                this.SetDC(true);
            }
        }

        #endregion

        #region EVENTI

        private void frmPUDiarioClinico_ImmagineClick(object sender, ImmagineTopClickEventArgs e)
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

                    case EnumImmagineTop.Distacco:
                        this.ucTopModale.Focus();
                        this.ImpostaCursore(enum_app_cursors.WaitCursor);
                        ViewControllerDiarioClinico oVC = new ViewControllerDiarioClinico();
                        oVC.Paziente = CoreStatics.CoreApplication.Paziente;
                        oVC.Episodio = CoreStatics.CoreApplication.Episodio;
                        oVC.Trasferimento = CoreStatics.CoreApplication.Trasferimento;
                        oVC.Cartella = CoreStatics.CoreApplication.Cartella;
                                                oVC.MovDiarioClinico = CoreStatics.CoreApplication.MovDiarioClinicoSelezionato;
                        oVC.MovDiarioClinico.CodTipoVoceDiario = this.utxtCodTipoVoceDiario.Text;
                        oVC.MovDiarioClinico.DataEvento = (DateTime)this.udteDataEvento.Value;
                        oVC.MovDiarioClinico.MovScheda.DatiXML = this.ucDcViewer.Gestore.SchedaDatiXML;
                                                CoreStatics.CoreApplication.Listener.ApriMaschera(EnumMaschere.VoceDiDiarioNM, oVC);
                        this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                        break;

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void frmPUDiarioClinico_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {

                if (Salva())
                {
                    FileStatics.DeleteSalvataggioScheda(CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.PathFileTemp);
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void frmPUDiarioClinico_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            FileStatics.DeleteSalvataggioScheda(CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.PathFileTemp);
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ubZoomCodTipoVoceDiario_Click(object sender, EventArgs e)
        {
            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezionaTipoVoceDiario) == DialogResult.OK)
            {
                this.utxtCodTipoVoceDiario.Text = CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodTipoVoceDiario;
                this.lblZoomCodTipoVoceDiario.Text = CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.DescrTipoVoceDiario;

                CaricaScheda();
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
                    frmPUDiarioClinico_PulsanteIndietroClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Indietro));
                }
                else if (key == System.Windows.Input.Key.F12)
                {
                    frmPUDiarioClinico_PulsanteAvantiClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti));
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

                                if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato != null)
                {
                    codua = CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodUA;
                }


                                CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato = new MovTestoPredefinito(UnicodeSrl.Scci.Enums.EnumEntita.DCL.ToString(), codua, CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice,
                                                                                                    CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodTipoVoceDiario, id);
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

        void ucDcViewer_OnModifiedEvent(string id)
        {

            try
            {

                                                this.ucDcViewer.Modificato = false;
                if (CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.PathFileTemp != string.Empty)
                {
                    UnicodeSrl.Scci.Statics.FileStatics.WriteSalvataggioScheda(CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.PathFileTemp, this.ucDcViewer.Gestore.SchedaDatiXML);
                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucDcViewer_OnModifiedEvent", this.Text);
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
                string azione = string.Format("DCL{0}.{1}", CoreStatics.CoreApplication.MovDiarioClinicoSelezionato.CodTipoVoceDiario, campo[0]);
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
