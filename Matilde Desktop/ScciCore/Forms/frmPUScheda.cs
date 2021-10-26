using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.Framework.Data;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Common;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Statics;
using Infragistics.Win.Misc;
using UnicodeSrl.Scci.PluginClient;
using UnicodeSrl.Scci.RTF;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore.ViewController;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUScheda : frmBaseModale, Interfacce.IViewFormlModal
    {
                private bool _schedaModificata = false;

                private ucSegnalibri _ucSegnalibri = null;

                                private bool _bNuovaRevisione = false;

        public frmPUScheda()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {

            try
            {

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_SCHEDA_16);


                if (this.CustomParamaters != null)
                {
                    if (this.CustomParamaters is Dictionary<String, object>)
                    {
                        Dictionary<String, object> dict = this.CustomParamaters as Dictionary<String, object>;
                        if (dict.Keys.Contains(CoreStatics.C_PARAM_NEW_REV) && dict[CoreStatics.C_PARAM_NEW_REV] != null && dict[CoreStatics.C_PARAM_NEW_REV] is bool)
                            _bNuovaRevisione = (bool)dict[CoreStatics.C_PARAM_NEW_REV];
                    }
                }


                CoreStatics.CoreApplication.MovSchedaSelezionata.PathFileTemp = FileStatics.GetPathSalvataggioScheda("SCH",
                                                                                CoreStatics.CoreApplication.MovSchedaSelezionata.CodEntita,
                                                                                CoreStatics.CoreApplication.MovSchedaSelezionata.IDEntita,
                                                                                CoreStatics.CoreApplication.MovSchedaSelezionata.CodScheda,
                                                                                CoreStatics.CoreApplication.MovSchedaSelezionata.Versione,
                                                                                CoreStatics.CoreApplication.Sessione.Utente.Codice,
                                                                                CoreStatics.CoreApplication.MovSchedaSelezionata.IDMovScheda,
                                                                                CoreStatics.CoreApplication.MovSchedaSelezionata.IDSchedaPadre
                                                                                );
                if (FileStatics.CheckSalvataggioScheda(CoreStatics.CoreApplication.MovSchedaSelezionata.PathFileTemp) == true)
                {
                    if (easyStatics.EasyMessageBox("Vuoi recuperare la scheda non ancora salvata ?", "Recupero Scheda", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        CoreStatics.CoreApplication.MovSchedaSelezionata.DatiXML = FileStatics.ReadSalvataggioScheda(CoreStatics.CoreApplication.MovSchedaSelezionata.PathFileTemp, CoreStatics.CoreApplication.MovSchedaSelezionata.DatiXML);
                    }
                }

                this.ucSbInEvidenza.UNCheckedImage = Risorse.GetImageFromResource(Risorse.GC_INRILIEVORIMUOVI_256);
                this.ucSbInEvidenza.CheckedImage = Risorse.GetImageFromResource(Risorse.GC_INRILIEVO_256);
                this.ucSbInEvidenza.Checked = CoreStatics.CoreApplication.MovSchedaSelezionata.InEvidenza;

                this.ucSbValida.UNCheckedImage = Risorse.GetImageFromResource((CoreStatics.CoreApplication.MovSchedaSelezionata.PermessoUAFirma == 0 ? Risorse.GC_LUCCHETTOAPERTO_256 : Risorse.GC_LUCCHETTOAPERTOFIRMA_256));
                this.ucSbValida.CheckedImage = Risorse.GetImageFromResource((CoreStatics.CoreApplication.MovSchedaSelezionata.PermessoUAFirma == 0 ? Risorse.GC_LUCCHETTOCHIUSO_256 : Risorse.GC_LUCCHETTOCHIUSOFIRMA_256));
                if (_bNuovaRevisione)
                {
                                                            this.ucSbValida.Visible = true;
                    this.ucSbValida.Checked = true;
                    this.ucSbValida.Enabled = false;
                }
                else
                {
                    if (CoreStatics.CoreApplication.MovSchedaSelezionata.IDMovScheda == string.Empty || CoreStatics.CoreApplication.MovSchedaSelezionata.IDMovScheda.Trim() == "")
                    {
                        this.ucSbValida.Visible = CoreStatics.CoreApplication.MovSchedaSelezionata.Scheda.Validabile;
                    }
                    else
                    {
                        this.ucSbValida.Visible = CoreStatics.CoreApplication.MovSchedaSelezionata.Validabile;
                    }
                    this.ucSbValida.Checked = CoreStatics.CoreApplication.MovSchedaSelezionata.Validata;
                }


                this.ucTopModale.Distacco = true;

                this.ShowDialog();

            }
            catch (Exception ex)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                CoreStatics.ExGest(ref ex, "Carica", this.Name);
            }

        }

        #endregion

        #region SubRoutine

        private void CaricaScheda()
        {

            try
            {

                                                                Gestore oGestore = CoreStatics.GetGestore();

                                oGestore.SchedaXML = CoreStatics.CoreApplication.MovSchedaSelezionata.Scheda.StrutturaXML;
                oGestore.SchedaLayoutsXML = CoreStatics.CoreApplication.MovSchedaSelezionata.Scheda.LayoutXML;

                                oGestore.Decodifiche = CoreStatics.CoreApplication.MovSchedaSelezionata.Scheda.DizionarioValori();

                                if (CoreStatics.CoreApplication.MovSchedaSelezionata.DatiXML == string.Empty)
                {
                    oGestore.SchedaDati = new DcSchedaDati();
                }
                else
                {
                    try
                    {
                        oGestore.SchedaDatiXML = CoreStatics.CoreApplication.MovSchedaSelezionata.DatiXML;
                    }
                    catch (Exception)
                    {
                        oGestore.SchedaDati = new DcSchedaDati();
                    }
                }
                if (oGestore.SchedaDati.Dati.Count == 0) { oGestore.NuovaScheda(); }

                                this.ucDcViewer.VisualizzaTitoloScheda = false;

                this.ucDcViewer.CaricaDati(oGestore);
                this.ucDcViewer.TitoloScheda = CoreStatics.CoreApplication.MovSchedaSelezionata.DescrScheda;
                this.ucDcViewer.VisualizzaTitoloScheda = true;

                this.ucDcViewer.RtfEvent += ucDcViewer_RtfEvent;
                this.ucDcViewer.KeyEvent += ucDcViewer_KeyEvent;

                this.ucDcViewer.OnModifiedEvent += ucDcViewer_OnModifiedEvent;
                this.ucDcViewer.ButtonEvent += ucDcViewer_ButtonEvent;

                this.ucDcViewer.AddRigaRipetibileEvent += ucDcViewer_AddRigaRipetibileEvent;
                this.ucDcViewer.RemoveRigaRipetibileEvent += ucDcViewer_RemoveRigaRipetibileEvent;

                
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaScheda", this.Name);
            }

        }

                                                private bool SalvaScheda(bool controllaDatiObbligatori)
        {

            bool bReturn = false;
            bool bSalva = true;

            try
            {

                                if (_bNuovaRevisione)
                {
                                        string idmovscheda = CoreStatics.CoreApplication.MovSchedaSelezionata.IDMovScheda;

                    Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                    op.Parametro.Add("IDScheda", idmovscheda);
                    op.TimeStamp.CodEntita = EnumEntita.SCH.ToString();
                    op.TimeStamp.CodAzione = EnumAzioni.MOD.ToString();

                    SqlParameterExt[] spcoll = new SqlParameterExt[1];

                    string xmlParam = XmlProcs.XmlSerializeToString(op);
                    spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                    Database.GetDataTableStoredProc("MSP_InsMovSchedeRevisione", spcoll);
                }

                CoreStatics.CoreApplication.MovSchedaSelezionata.InEvidenza = this.ucSbInEvidenza.Checked;
                CoreStatics.CoreApplication.MovSchedaSelezionata.ValidataNew = this.ucSbValida.Checked;

                CoreStatics.CoreApplication.MovSchedaSelezionata.DatiXML = this.ucDcViewer.Gestore.SchedaDatiXML;

                if (controllaDatiObbligatori == true)
                {
                    if (CoreStatics.CoreApplication.MovSchedaSelezionata.DatiObbligatoriMancantiRTF != string.Empty)
                    {
                        if (easyStatics.EasyMessageBox(@"Non sono stati compilati alcuni valori obbligatori della scheda!" + Environment.NewLine + @"Vuoi continuare col salvataggio?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                            bSalva = false;
                    }
                }

                if (bSalva == true)
                {
                    bReturn = true;
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "SalvaScheda", this.Text);
            }

            return bReturn;

        }

                                private void SalvaChiudiForm(bool controllaDatiObbligatori)
        {
            if (SalvaScheda(controllaDatiObbligatori) == true)
            {
                FileStatics.DeleteSalvataggioScheda(CoreStatics.CoreApplication.MovSchedaSelezionata.PathFileTemp);
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        #endregion

        #region Events Form

        private void frmPUScheda_Shown(object sender, EventArgs e)
        {
            if (this.ucSbValida.Visible == false)
            {
                this.ucEasyTableLayoutPanel.RowStyles[this.ucEasyTableLayoutPanel.RowCount - 1].Height = 0;
            }
            this.CaricaScheda();
        }

        #endregion

        #region Events

        private void frmPUScheda_ImmagineClick(object sender, ImmagineTopClickEventArgs e)
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
                        if (CoreStatics.CoreApplication.MovSchedaSelezionata.Azione == EnumAzioni.INS)
                        {
                            CoreStatics.CoreApplication.MovSchedaSelezionata.AddPin = true;
                            this.ucTopModale.Focus();
                            SalvaChiudiForm(false);
                        }
                        else if (CoreStatics.CoreApplication.MovSchedaSelezionata.Azione == EnumAzioni.MOD)
                        {
                            CoreStatics.CoreApplication.MovSchedaSelezionata.AggiungiSegnalibro();
                            this.ucTopModale.Focus();
                            SalvaChiudiForm(false);
                        }
                        break;

                    case EnumImmagineTop.Distacco:
                        this.ucTopModale.Focus();
                        this.ImpostaCursore(enum_app_cursors.WaitCursor);
                        ViewControllerScheda oVC = new ViewControllerScheda();
                        oVC.Paziente = CoreStatics.CoreApplication.Paziente;
                        oVC.Episodio = CoreStatics.CoreApplication.Episodio;
                        oVC.Trasferimento = CoreStatics.CoreApplication.Trasferimento;
                        oVC.Cartella = CoreStatics.CoreApplication.Cartella;
                                                oVC.MovScheda = CoreStatics.CoreApplication.MovSchedaSelezionata;
                        oVC.MovScheda.DatiXML = this.ucDcViewer.Gestore.SchedaDatiXML;
                                                CoreStatics.CoreApplication.Listener.ApriMaschera(EnumMaschere.SchedaNM, oVC);
                        this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                        break;

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void frmPUScheda_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
                        try
            {
                SalvaChiudiForm(true);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void frmPUScheda_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
                                    bool bExit = true;

                                    if (easyStatics.EasyMessageBox(@"La scheda non è stata salvata, vuoi uscire ugualmente?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                bExit = false;
            
            if (bExit)
            {
                                FileStatics.DeleteSalvataggioScheda(CoreStatics.CoreApplication.MovSchedaSelezionata.PathFileTemp);
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Close();
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
                    frmPUScheda_PulsanteIndietroClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Indietro));
                }
                else if (key == System.Windows.Input.Key.F12)
                {
                    this.ucDcViewer.SalvaDati();
                    SalvaChiudiForm(true);
                }

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucDcViewer_KeyEvent", this.Text);
            }

        }

        void ucDcViewer_RtfEvent(string id)
        {
            _schedaModificata = true;
            try
            {

                string codua = "";

                                if (CoreStatics.CoreApplication.Trasferimento != null)
                {
                    codua = CoreStatics.CoreApplication.Trasferimento.CodUA;
                }
                else
                {
                    codua = CoreStatics.CoreApplication.Sessione.Utente.ConfigUtente.CodUAAmbulatorialeSelezionata;
                }

                                CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato = new MovTestoPredefinito(UnicodeSrl.Scci.Enums.EnumEntita.SCH.ToString(), codua, CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice,
                                                                                                    CoreStatics.CoreApplication.MovSchedaSelezionata.CodScheda, id);
                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(Scci.Enums.EnumMaschere.TestiPredefiniti) == System.Windows.Forms.DialogResult.OK)
                {
                                                            string sSourceRTF = this.ucDcViewer.Gestore.LeggeValore(id).ToString();
                    string sDestRTF = CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato.RitornoRTF;
                    RtfFiles rtf = new RtfFiles();
                    sSourceRTF = rtf.joinRtf(sDestRTF, sSourceRTF, true);
                                        rtf = null;
                    this.ucDcViewer.Gestore.ModificaValore(id, sSourceRTF);
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
            _schedaModificata = true;

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
                    string azione = string.Format("{0}.{1}", azioni[1], campo[0]);
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

        void ucDcViewer_OnModifiedEvent(string id)
        {

            try
            {

                                                this.ucDcViewer.Modificato = false;
                if (CoreStatics.CoreApplication.MovSchedaSelezionata.PathFileTemp != string.Empty)
                {
                    UnicodeSrl.Scci.Statics.FileStatics.WriteSalvataggioScheda(CoreStatics.CoreApplication.MovSchedaSelezionata.PathFileTemp, this.ucDcViewer.Gestore.SchedaDatiXML);
                }

                _schedaModificata = true;
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucDcViewer_OnModifiedEvent", this.Text);
            }

        }

        void ucDcViewer_AddRigaRipetibileEvent(object sender, string key, int sequenza)
        {
            _schedaModificata = true;
            try
            {
                CoreStatics.CoreApplication.MovSchedaSelezionata.MovOperazioni.Add(new List<string>(new string[] { key, sequenza.ToString(), EnumAzioni.INS.ToString() }));
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucDcViewer_AddRigaRipetibileEvent", this.Text);
            }

        }

        void ucDcViewer_RemoveRigaRipetibileEvent(object sender, string key, int sequenza)
        {
            _schedaModificata = true;
            try
            {
                CoreStatics.CoreApplication.MovSchedaSelezionata.MovOperazioni.Add(new List<string>(new string[] { key, sequenza.ToString(), EnumAzioni.CAN.ToString() }));
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucDcViewer_RemoveRigaRipetibileEvent", this.Text);
            }

        }

        void ucDcViewer_OnChangedEvent(object sender, string id)
        {

            try
            {

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ucDcViewer_OnChangedEvent", this.Text);
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
                        CoreStatics.CoreApplication.MovSchedaSelezionata.IDPin = e.ID;
                        frmPUScheda_PulsanteAvantiClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti));
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
