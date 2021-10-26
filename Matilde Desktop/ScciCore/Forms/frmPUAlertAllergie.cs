using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Scci.Enums;

using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Common;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci.PluginClient;
using System.IO;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUAlertAllergie : frmBaseModale, Interfacce.IViewFormlModal
    {
        public frmPUAlertAllergie()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {
            try
            {
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_ALERTALLERGIA_16);

                InizializzaControlli();

                Aggiorna();

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
            try
            {
                this.ubZoomTipo.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_ZOOM_256);
                this.ubZoomTipo.PercImageFill = 0.75F;

                this.ucDcViewer.VisualizzaTitoloScheda = false;

                                if (CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.IDMovAlertAllergieAnamnesi == string.Empty ||
                    CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.IDMovAlertAllergieAnamnesi == @"")
                {
                    this.ubZoomTipo.Visible = true;
                }
                else
                {
                    this.ubZoomTipo.Visible = false;
                }
            }
            catch (Exception)
            {
            }
        }

        public void Aggiorna()
        {
            try
            {
                this.udteDataEvento.Value = null;
                this.lblZoomTipoAlertAllergiaAnamnesi.Text = string.Empty;
                
                if (CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.Azione != EnumAzioni.INS)
                {
                    this.lblZoomTipoAlertAllergiaAnamnesi.Text = CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.DescrTipo;
                                    }
                else
                {
                    if (CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.DescrTipo == "")
                        this.lblZoomTipoAlertAllergiaAnamnesi.Text = @"Selezionare Tipo Nota Anamnestica";
                    else
                        this.lblZoomTipoAlertAllergiaAnamnesi.Text = CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.DescrTipo;
                }

                if (CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.DataEvento > DateTime.MinValue)
                    this.udteDataEvento.Value = CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.DataEvento;

                this.udteDataEvento.ReadOnly = (CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.Azione != EnumAzioni.INS);

                this.CaricaScheda();

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

                                oGestore.SchedaXML = CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.MovScheda.Scheda.StrutturaXML;

                                oGestore.SchedaLayoutsXML = CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.MovScheda.Scheda.LayoutXML;

                                oGestore.Decodifiche = CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.MovScheda.Scheda.DizionarioValori();

                                if (CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.MovScheda.DatiXML == string.Empty)
                {
                    oGestore.SchedaDati = new DcSchedaDati();
                }
                else
                {
                    try
                    {
                        oGestore.SchedaDatiXML = CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.MovScheda.DatiXML;
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
                this.ucDcViewer.ButtonEvent -= ucDcViewer_ButtonEvent;
                this.ucDcViewer.RtfEvent += ucDcViewer_RtfEvent;
                this.ucDcViewer.KeyEvent += ucDcViewer_KeyEvent;
                this.ucDcViewer.ButtonEvent += ucDcViewer_ButtonEvent;


            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "CaricaScheda", this.Text);
            }

        }


        public bool Salva()
        {
            bool bReturn = false;
            try
            {
                if (ControllaValori())
                {
                    CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.DataEvento = (DateTime)this.udteDataEvento.Value;

                    if (SalvaScheda())
                    {
                        if (CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.Azione == EnumAzioni.MOD)
                        {

                                                                                                                MovAlertAllergieAnamnesi oMov = new MovAlertAllergieAnamnesi(CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.IDPaziente,
                                                            CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.CodUA,
                                                            CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.IDEpisodio,
                                                            CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.IDTrasferimento, 
                                                            CoreStatics.CoreApplication.Ambiente);
                            bReturn = oMov.CopiaDaOrigine(CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato);
                            bReturn = oMov.Salva();
                            oMov = null;

                                                                                                                oMov = new MovAlertAllergieAnamnesi(CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.IDMovAlertAllergieAnamnesi,
                                                           CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.IDPaziente,
                                                           CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.CodUA,
                                                           CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.IDEpisodio,
                                                           CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.IDTrasferimento, 
                                                           CoreStatics.CoreApplication.Ambiente);
                            oMov.Cancella();
                            oMov = null;

                        }
                        else
                        {
                            bReturn = CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.Salva();
                        }
                    }
                }

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

                CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.MovScheda.DatiXML = this.ucDcViewer.Gestore.SchedaDatiXML;

                if (CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.MovScheda.DatiObbligatoriMancantiRTF != string.Empty
                && CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.MovScheda.DatiObbligatoriMancantiRTF.Trim() != string.Empty)
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

        private bool ControllaValori()
        {
            bool bOK = true;

                        if (bOK && CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.CodTipo == "")
            {
                easyStatics.EasyMessageBox("Inserire Tipo Nota Anamnestica !", "Note Anamnestiche", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.ubZoomTipo.Focus();
                bOK = false;
            }
            if (bOK && !this.udteDataEvento.ReadOnly && this.udteDataEvento.Value == null)
            {
                easyStatics.EasyMessageBox("Inserire Data Evento!", "Note Anamnestiche", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.udteDataEvento.Focus();
                bOK = false;
            }
            return bOK;
        }

        #endregion

        #region Events Form

        private void frmPUAlertAllergie_Shown(object sender, EventArgs e)
        {
            this.CaricaScheda();
        }

        #endregion

        #region Events

        private void ubZoomTipo_Click(object sender, EventArgs e)
        {
            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneTipoNoteAnamnesticheAlertAllergie) == DialogResult.OK)
            {
                this.lblZoomTipoAlertAllergiaAnamnesi.Text = CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.DescrTipo;
                this.ImpostaCursore(enum_app_cursors.WaitCursor);
                CaricaScheda();
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
        }

        private void frmPUAlertAllergie_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
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

        private void frmPUAlertAllergie_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
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
                    frmPUAlertAllergie_PulsanteIndietroClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Indietro));
                }
                else if (key == System.Windows.Input.Key.F12)
                {
                    frmPUAlertAllergie_PulsanteAvantiClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti));
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

                                if (CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato != null)
                {
                    codua = CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.CodUA;
                }


                                CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato = new MovTestoPredefinito(UnicodeSrl.Scci.Enums.EnumEntita.ALA.ToString(), codua, CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice,
                                                                                                    CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.CodTipo, id);
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
                string azione = string.Format("ALA{0}.{1}", CoreStatics.CoreApplication.MovAlertAllergieAnamnesiSelezionato.CodTipo, campo[0]);
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

    }
}
