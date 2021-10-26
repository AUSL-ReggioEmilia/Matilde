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

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUConsulenza : frmBaseModale, Interfacce.IViewFormlModal
    {

        #region DICHIARAZIONI

        #endregion

        public frmPUConsulenza()
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

                
                                this.lblConsulente.Text = @"Nome Consulente: " + CoreStatics.CoreApplication.Consulenza.Consulente.Descrizione;
                this.lblUserName.Text = @"Nome Utente Consulente: " + CoreStatics.CoreApplication.Consulenza.Consulente.Codice;
                this.lblTipo.Text = @"Tipo di Consulenza: " + CoreStatics.CoreApplication.Consulenza.MovDiarioClinicoConsulenza.DescrTipoVoceDiario;
                this.lblDataOra.Text = @"Data/Ora Inserimento Consulenza: " + CoreStatics.CoreApplication.Consulenza.MovDiarioClinicoConsulenza.DataEvento.ToString("dd/MM/yyyy HH:mm");

                                this.ShowDialog();

                                try
                {
                    Maschera mDC = CoreStatics.CoreApplication.Navigazione.Maschere.Elementi.Find(m => m.CodMaschera == "1-1-2");
                    if (mDC != null) mDC.ControlMiddle.Carica();
                }
                catch
                { }
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Carica", this.Text);
            }
        }

        #endregion

        #region FUNZIONI

        private void CaricaScheda()
        {

            try
            {

                                                                Gestore oGestore = CoreStatics.GetGestore();

                                oGestore.SchedaXML = CoreStatics.CoreApplication.Consulenza.MovDiarioClinicoConsulenza.MovScheda.Scheda.StrutturaXML;

                                oGestore.SchedaLayoutsXML = CoreStatics.CoreApplication.Consulenza.MovDiarioClinicoConsulenza.MovScheda.Scheda.LayoutXML;

                                oGestore.Decodifiche = CoreStatics.CoreApplication.Consulenza.MovDiarioClinicoConsulenza.MovScheda.Scheda.DizionarioValori();

                                if (CoreStatics.CoreApplication.Consulenza.MovDiarioClinicoConsulenza.MovScheda.DatiXML == string.Empty)
                {
                    oGestore.SchedaDati = new DcSchedaDati();
                }
                else
                {
                    try
                    {
                        oGestore.SchedaDatiXML = CoreStatics.CoreApplication.Consulenza.MovDiarioClinicoConsulenza.MovScheda.DatiXML;
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


                this.ucDcViewer.OnModifiedEvent += ucDcViewer_OnModifiedEvent;
                this.ucDcViewer.ButtonEvent += ucDcViewer_ButtonEvent;

            }
            catch (Exception ex)
            {
                throw new Exception(@"CaricaScheda()" + Environment.NewLine + ex.Message, ex);
            }
        }

        private bool ControllaValori()
        {
            bool bOK = true;

                                                                                                                                                            
            return bOK;
        }

        private bool SalvaValida()
        {
            this.ImpostaCursore(enum_app_cursors.WaitCursor);
            bool bReturn = false;
            try
            {
                if (CoreStatics.CoreApplication.Consulenza.MovDiarioClinicoConsulenza.Azione != EnumAzioni.VIS)
                {
                    if (ControllaValori())
                    {
                                                CoreStatics.CoreApplication.Consulenza.MovDiarioClinicoConsulenza.CodStatoDiario = "VA";
                        CoreStatics.CoreApplication.Consulenza.MovDiarioClinicoConsulenza.Azione = EnumAzioni.VAL;

                        if (SalvaScheda())
                            bReturn = CoreStatics.CoreApplication.Consulenza.MovDiarioClinicoConsulenza.Salva();
                    }
                }
                else
                    bReturn = true;
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Salva", this.Text);
            }

            this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            return bReturn;
        }

        private bool SalvaScheda()
        {
            bool bReturn = true;

            try
            {

                CoreStatics.CoreApplication.Consulenza.MovDiarioClinicoConsulenza.MovScheda.DatiXML = this.ucDcViewer.Gestore.SchedaDatiXML;

                if (CoreStatics.CoreApplication.Consulenza.MovDiarioClinicoConsulenza.MovScheda.DatiObbligatoriMancantiRTF != string.Empty
                && CoreStatics.CoreApplication.Consulenza.MovDiarioClinicoConsulenza.MovScheda.DatiObbligatoriMancantiRTF.Trim() != "")
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

        #endregion

        #region Events Form

        private void frmPUConsulenza_Shown(object sender, EventArgs e)
        {
            this.CaricaScheda();
        }

        #endregion

        #region EVENTI

        private void frmPUConsulenza_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            try
            {

                if (SalvaValida())
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

        private void frmPUConsulenza_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
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
                    frmPUConsulenza_PulsanteIndietroClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Indietro));
                }
                else if (key == System.Windows.Input.Key.F12)
                {
                    this.ucDcViewer.SalvaDati();
                    frmPUConsulenza_PulsanteAvantiClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti));
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

                                if (CoreStatics.CoreApplication.Trasferimento != null)
                {
                    codua = CoreStatics.CoreApplication.Trasferimento.CodUA;
                }

                                CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato = new MovTestoPredefinito(UnicodeSrl.Scci.Enums.EnumEntita.SCH.ToString(), codua, CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice,
                                                                                                    CoreStatics.CoreApplication.Consulenza.MovDiarioClinicoConsulenza.CodTipoVoceDiario , id);
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
                string azione = string.Format("{0}.{1}", azioni[1], campo[0]);
                                object[] myparam = new object[5] { this, campo[0], int.Parse(campo[1]), this.ucDcViewer.Gestore, azioni[0] };

                Risposta oRisposta = PluginClientStatics.PluginClient(azione, myparam, CommonStatics.UAPadri(_codua, CoreStatics.CoreApplication.Ambiente));
                if (oRisposta.Successo == true)
                {

                }
                else
                {

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

        void ucDcViewer_OnModifiedEvent(string id)
        {

            try
            {

                this.ucDcViewer.Modificato = false;

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucDcViewer_OnModifiedEvent", this.Text);
            }

        }

        #endregion

    }
}
