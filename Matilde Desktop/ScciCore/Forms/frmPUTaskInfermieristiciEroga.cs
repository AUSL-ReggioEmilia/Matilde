using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using Infragistics.Win.UltraWinGrid;

using Microsoft.Data.SqlClient;
using System.Globalization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci.PluginClient;

using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Common;
using UnicodeSrl.Scci;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUTaskInfermieristiciEroga : frmBaseModale, Interfacce.IViewFormlModal
    {

        private Gestore oGestore = null;

        public frmPUTaskInfermieristiciEroga()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {
            try
            {
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_WORKLIST_32);

                InizializzaControlli();
                this.InizializzaGestore();

                
                this.ShowDialog();
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Carica", this.Text);
            }
        }

        #endregion

        #region Functions

        private void InizializzaControlli()
        {
            MovPrescrizione oMP = null;
            try
            {

                this.lblTipoTaskInfermieristico.Text = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DescrTipoTaskInfermieristico;
                lblDataProgrammataInizio.Text = @"Pianificato per: " + CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataProgrammata.ToString("dd/MM/yyyy HH:mm");

                this.pbIconaTask.Image = DrawingProcs.GetImageFromByte(CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Icona);

                this.pbObbligatorio.Image = Risorse.GetImageFromResource(Risorse.GC_OBBLIGATORIO_256);
                this.pbObbligatorio.Visible = false;

                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoTaskInfermieristico == UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.TipoSchedaTaskDaPrescrizione))
                {
                    oMP = new MovPrescrizione(CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.IDSistema, false, CoreStatics.CoreApplication.Ambiente);
                    this.txtPosologiaEffettiva.Visible = oMP.PrescrizioneASchema;
                    this.lblPosologiaEffettiva.Visible = oMP.PrescrizioneASchema;
                    this.pbObbligatorio.Visible = oMP.PrescrizioneASchema;
                }


                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Azione == EnumAzioni.VAL)
                {
                    this.lblNote.Text = "Note Erogazione:";
                    this.lblDataErogazione.Text = "Erogato il:";
                    if (oMP != null) { this.txtPosologiaEffettiva.Enabled = oMP.PrescrizioneASchema; }

                }
                else if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Azione == EnumAzioni.VIS)
                {
                    if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodStatoTaskInfermieristico == EnumStatoTaskInfermieristico.ER.ToString())
                    {
                        this.lblNote.Text = "Note Erogazione:";
                        this.lblDataErogazione.Text = "Erogato il:";
                    }
                    else if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodStatoTaskInfermieristico == EnumStatoTaskInfermieristico.AN.ToString())
                    {
                        this.lblNote.Text = "Note Annullamento";
                        this.lblDataErogazione.Text = "Annullato il:";
                    }

                }
                else
                {
                    this.lblNote.Text = "Note Annullamento";
                    this.lblDataErogazione.Text = "Annullato il:";
                }

                this.ucAnteprimaRtf.rtbRichTextBox.ReadOnly = true;
                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Azione == EnumAzioni.VIS)
                {
                    this.udteDataErogazione.ReadOnly = true;
                    this.txtNote.ReadOnly = true;
                    this.txtPosologiaEffettiva.ReadOnly = true;
                    this.ucBottomModale.ubAvanti.Visible = false;
                    this.pbObbligatorio.Visible = false;
                }


                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.SoloTestata)
                {
                                        this.ucEasyTableLayoutPanelBottom.Visible = false;

                    this.ucEasyTableLayoutPanel.RowStyles[4].Height = 0F;
                    this.ucEasyTableLayoutPanel.RowStyles[3].Height = 86F;                 }
                else
                {
                                        this.ucEasyTableLayoutPanel.RowStyles[3].Height = 47F;
                    this.ucEasyTableLayoutPanel.RowStyles[4].Height = 39F;

                    this.ucEasyTableLayoutPanelBottom.Visible = true;
                }
            }
            catch (Exception)
            {
            }

            oMP = null;
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
                                oGestore.SchedaXML = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.Scheda.StrutturaXML;
                oGestore.SchedaLayoutsXML = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.Scheda.LayoutXML;
                                oGestore.Decodifiche = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.Scheda.DizionarioValori();

                                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.DatiXML == string.Empty)
                {
                    oGestore.SchedaDati = new DcSchedaDati();
                }
                else
                {
                    try
                    {
                        oGestore.SchedaDatiXML = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.DatiXML;
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

                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataErogazione <= DateTime.MinValue)
                    this.udteDataErogazione.Value = DateTime.Now;
                else
                    this.udteDataErogazione.Value = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataErogazione;

                this.txtNote.Text = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Note;
                this.txtPosologiaEffettiva.Text = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PosologiaEffettiva;

                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoRegistrazione == UnicodeSrl.Scci.Enums.EnumTipoRegistrazione.A.ToString())
                {
                    this.SetDC(false);
                }
                else
                {
                    if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Azione == EnumAzioni.VIS)
                    {
                        this.SetDC(false);
                    }
                    else
                    {
                        if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodSistema == EnumCodSistema.PRF.ToString())
                        {
                            MovPrescrizioneTempi oMovPrescrizioneTempi = new MovPrescrizioneTempi(CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.IDGruppo, "", CoreStatics.CoreApplication.Ambiente);
                            if (oMovPrescrizioneTempi.AlBisogno == true)
                            {
                                this.SetDC(false);
                            }
                            else
                            {
                                this.SetDC(true);
                            }
                        }
                        else
                        {
                            this.SetDC(true);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Aggiorna", this.Text);
            }

        }

        private void CaricaScheda()
        {

            try
            {

                                                                Gestore oGestore = CoreStatics.GetGestore();

                                oGestore.SchedaXML = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.Scheda.StrutturaXML;

                                oGestore.SchedaLayoutsXML = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.Scheda.LayoutXML;

                                oGestore.Decodifiche = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.Scheda.DizionarioValori();

                                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.DatiXML == string.Empty)
                {
                    oGestore.SchedaDati = new DcSchedaDati();
                }
                else
                {
                    try
                    {
                        oGestore.SchedaDatiXML = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.DatiXML;
                    }
                    catch (Exception)
                    {
                        oGestore.SchedaDati = new DcSchedaDati();
                    }
                }
                if (oGestore.SchedaDati.Dati.Count == 0) { oGestore.NuovaScheda(); }

                this.ucDcViewer.VisualizzaTitoloScheda = false;

                this.ucDcViewer.CaricaDati(oGestore);

                this.ucDcViewer.RtfEvent -= ucDcViewer_RtfEvent;
                this.ucDcViewer.KeyEvent -= ucDcViewer_KeyEvent;
                this.ucDcViewer.RtfEvent += ucDcViewer_RtfEvent;
                this.ucDcViewer.KeyEvent += ucDcViewer_KeyEvent;


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

                this.ucAnteprimaRtf.MovScheda = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda;
                this.ucAnteprimaRtf.MovScheda.GeneraRTF();
                this.ucAnteprimaRtf.RefreshRTF();

            }
            catch (Exception ex)
            {
                throw new Exception(@"CaricaRtf()" + Environment.NewLine + ex.Message, ex);
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
                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.DatiXML = this.ucDcViewer.Gestore.SchedaDatiXML;
                    this.CaricaRtf();
                    break;

                case true:
                    this.ucEasyTableLayoutPanelDC.ColumnStyles[0].Width = 0;
                    this.ucEasyTableLayoutPanelDC.ColumnStyles[1].Width = 100;
                    this.ucDcViewer.CaricaDati();
                    break;
            }

            this.ucEasyTableLayoutPanelDC.Visible = true;

        }

        public bool Salva()
        {
            bool bReturn = false;
            try
            {
                this.ImpostaCursore(enum_app_cursors.WaitCursor);
                                enableControls(false);
                if (ControllaValori())
                {
                                                            if (DBUtils.ControllaAlertErogazione(CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato, true))
                    {
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.DataErogazione = (DateTime)this.udteDataErogazione.Value;
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Note = this.txtNote.Text;
                        CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.PosologiaEffettiva = this.txtPosologiaEffettiva.Text;

                        if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Azione == EnumAzioni.VAL)
                            CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodStatoTaskInfermieristico =
                                Enum.GetName(typeof(EnumStatoTaskInfermieristico), EnumStatoTaskInfermieristico.ER);
                        else
                            CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodStatoTaskInfermieristico =
                                Enum.GetName(typeof(EnumStatoTaskInfermieristico), EnumStatoTaskInfermieristico.AN);

                        if (SalvaScheda())
                        {

                            Risposta oRispostaElaboraPrima = new Risposta();
                            oRispostaElaboraPrima.Successo = true;
                            if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodStatoTaskInfermieristico ==
                                Enum.GetName(typeof(EnumStatoTaskInfermieristico), EnumStatoTaskInfermieristico.ER))
                            {
                                                                oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.WKI_EROGA_PRIMA_PU.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                            }
                            else
                            {
                                                                oRispostaElaboraPrima = PluginClientStatics.PluginClient(EnumPluginClient.WKI_ANNULLA_PRIMA_PU.ToString(), new object[1] { new object() }, CommonStatics.UAPadri(CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodUA, CoreStatics.CoreApplication.Ambiente));
                            }

                            if (oRispostaElaboraPrima.Successo)
                            {
                                bReturn = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Salva();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Salva", this.Text);
            }
            finally
            {
                                enableControls(true);
            }

            this.ImpostaCursore(enum_app_cursors.DefaultCursor);

            return bReturn;
        }

        private bool SalvaScheda()
        {
            bool bReturn = true;

            try
            {

                CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.DatiXML = this.ucDcViewer.Gestore.SchedaDatiXML;

                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.DatiObbligatoriMancantiRTF != string.Empty
                && CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.MovScheda.DatiObbligatoriMancantiRTF.Trim() != string.Empty)
                {
                    if (easyStatics.EasyMessageBox(@"Non sono stati compilati alcuni valori obbligatori della scheda!" + Environment.NewLine + @"Vuoi conitinuare col salvataggio?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                        bReturn = false;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return bReturn;
        }

        private bool ControllaValori()
        {
            bool bOK = true;

                        if (bOK && this.udteDataErogazione.Value == null)
            {
                easyStatics.EasyMessageBox("Inserire Data/Ora Erogazione!", "Worklist", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.udteDataErogazione.Focus();
                bOK = false;
            }

            if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.Azione == EnumAzioni.VAL)
            {

                                if (bOK && !DBUtils.ControllaAnticipoErogazione((DateTime)this.udteDataErogazione.Value))
                {
                    this.udteDataErogazione.Focus();
                    bOK = false;
                }

                if (bOK && this.txtPosologiaEffettiva.Enabled == true && this.txtPosologiaEffettiva.Text.Trim() == string.Empty)
                {
                    easyStatics.EasyMessageBox("Posologia effettiva OBBLIGATORIA !!!", "Worklist", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.txtPosologiaEffettiva.Focus();
                    bOK = false;
                }

            }

            return bOK;
        }

        private void enableControls(bool vEnable)
        {
            try
            {
                this.PulsanteAvantiAbilitato = vEnable;
                this.PulsanteIndietroAbilitato = vEnable;
                this.ucTopModale.Enabled = vEnable;
                                            }
            catch
            {
            }
        }

        #endregion

        #region Events Form

        private void frmPUTaskInfermieristiciEroga_Shown(object sender, EventArgs e)
        {
            this.Aggiorna();
        }

        #endregion

        #region Events

        private void frmPUTaskInfermieristiciEroga_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {

                if (Salva())
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

        private void frmPUTaskInfermieristiciEroga_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Events UserControl

        void ucDcViewer_KeyEvent(System.Windows.Input.Key key)
        {

            try
            {

                if (key == System.Windows.Input.Key.F1)
                {
                    frmPUTaskInfermieristiciEroga_PulsanteIndietroClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Indietro));
                }
                else if (key == System.Windows.Input.Key.F12)
                {
                    frmPUTaskInfermieristiciEroga_PulsanteAvantiClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti));
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

                                if (CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato != null)
                {
                    codua = CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodUA;
                }

                                CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato = new MovTestoPredefinito(UnicodeSrl.Scci.Enums.EnumEntita.WKI.ToString(), codua, CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice,
                                                                                                    CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoTaskInfermieristico, id);
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

                                                                bool bTestDebug = false;
#if DEBUG
                #endif
                if (bTestDebug)
                {
                    CoreStatics.CoreApplication.ImportTestiRefertiDWH = new ImportTestiRefertiDWH(EnumTipoContenutiReferto.RTF);
                    if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.ImportaDWH) == DialogResult.OK)
                    {
                                            }
                }
                else
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
                    string azione = string.Format("WKI{0}.{1}", CoreStatics.CoreApplication.MovTaskInfermieristicoSelezionato.CodTipoTaskInfermieristico, campo[0]);
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

    }
}
