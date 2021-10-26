using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.Scci;
using UnicodeSrl.ScciResource;
using UnicodeSrl.ScciCore;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.ScciCore.Common;
using Infragistics.Win.Misc;
using UnicodeSrl.Scci.Model;
using UnicodeSrl.Scci.PluginClient;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUEvidenzaClinica : frmBaseModale, Interfacce.IViewFormlModal
    {

        private List<string> _tempfiles = new List<string>();
        UnicodeSrl.ScciCore.Interfacce.IViewUserControlMiddle _ucEasyRefertoLAB1 = null;
        System.Windows.Forms.DialogResult _formDialogResult = DialogResult.Cancel;

                private ucSegnalibri _ucSegnalibri = null;

        public frmPUEvidenzaClinica()
        {
            InitializeComponent();
        }

        #region INTERFACCIA

        public void Carica()
        {
            try
            {
                _formDialogResult = System.Windows.Forms.DialogResult.Cancel;

                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_EVIDENZACLINICA_16);


                                this.ubGrafico.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_PARAMETRIVITALIGRAFICO_256);
                this.ubPDF.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FORMATOPDF_256);                 this.ubOrdini.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_ORDINE_256);
                this.uchkVista.UNCheckedImage = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_FIRMA_256);
                this.uchkVista.CheckedImage = Properties.Resources.FirmaEseguita_256;
                                this.ubPACS.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_RX_256);
                                this.ubOrdini.PercImageFill = 0.75F;
                this.ubPACS.PercImageFill = 0.75F;
                this.ubGrafico.PercImageFill = 0.75F;
                this.ubPDF.PercImageFill = 0.75F;
                this.uchkVista.PercImageFill = 0.75F;

                                if (CoreStatics.CoreApplication.Cartella != null)
                {
                    if (CoreStatics.CoreApplication.Cartella.CodStatoCartella == EnumStatoCartella.CH.ToString())
                    {
                                                this.uchkVista.Visible = false;
                    }
                    else
                    {
                                                this.uchkVista.Visible = (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.PermessoVista == 1);
                    }
                }
                else
                {
                    this.uchkVista.Visible = false;
                }

                this.ubGrafico.Visible = (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.PermessoGrafico == 1);

                if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.DataReferto > DateTime.MinValue)
                    this.lblDataRefertoValore.Text = CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.DataReferto.ToString(@"dd/MM/yyyy HH:mm");

                this.lblTipoRefertoValore.Text = CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.DescrTipoEvidenzaClinica;

                this.webBrowser.Visible = false;

                if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.CodTipoEvidenzaClinica == EnumCodTipoEvidenzaClinica.RX.ToString()
                 || CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.CodTipoEvidenzaClinica == EnumCodTipoEvidenzaClinica.LAB.ToString())
                    CaricaCDSSLAB();
                else
                    CaricaDWH();

                this.ubPDF.Visible = CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.AbilitaAperturaPDF;
                this.ubPACS.Visible = (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.CodTipoEvidenzaClinica == EnumCodTipoEvidenzaClinica.RX.ToString());

                this.ShowDialog();
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Carica", this.Text);
            }
        }

        #endregion

        #region PRIVATE

        private void CaricaDWH()
        {
            try
            {
                this.ubPACS.Visible = false;

                this.ucEasyTabControl.SelectedTab = this.ucEasyTabControl.Tabs["tabDWH"];
                if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.URLRefertoDWH != "")
                {
                    this.ucEasyTabControl.SelectedTab = this.ucEasyTabControl.Tabs["tabDWH"];
                    this.webBrowser.Visible = true;
                    this.webBrowser.Navigate(CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.URLRefertoDWH);
                }
            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, @"Impossibile accedere al referto web." + Environment.NewLine + @"Contattare amministratori di sistema", "CaricaDWH", this.Name);
                            }
        }

        private void CaricaPluginLAB()
        {
            try
            {
                const string C_pluginname = @"PluginScciLAB";

                this.ubPACS.Visible = true;
                this.webBrowser.Visible = false;
                this.ucEasyTabControl.SelectedTab = this.ucEasyTabControl.Tabs["tabLAB"];

                string path = Application.StartupPath + @"\Plugins\PluginScciLAB\PluginScciLAB.dll";

                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                PluginCaller caller = new PluginCaller(path, C_pluginname, true,
                                                        SessioneRemota: CoreStatics.CoreApplication.Sessione.Computer.SessioneRemota,
                                                        IsOSServer: CoreStatics.CoreApplication.Sessione.Computer.IsOSServer);

                Dictionary<string, object> oDictionary = new Dictionary<string, object>();

                                                                                                                oDictionary.Add("StringaConnessione", Database.ConnectionString);

                                
                                oDictionary.Add("IDReferto", CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.IDRefertoDWH);
                oDictionary.Add("Cognome", CoreStatics.CoreApplication.Paziente.Cognome);
                oDictionary.Add("Nome", CoreStatics.CoreApplication.Paziente.Nome);
                oDictionary.Add("DataNascita", CoreStatics.CoreApplication.Paziente.DataNascita);
                oDictionary.Add("SvcRefertiDWHURL", UnicodeSrl.Scci.Statics.Database.GetConfigTable(UnicodeSrl.Scci.Enums.EnumConfigTable.WebServiceSCCI) + @"/ScciRefertiDWH.svc");
                oDictionary.Add("CodTipoEvidenzaClinica", CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.CodTipoEvidenzaClinica);

                oDictionary.Add("ReturnPACS", false);                   
                _ucEasyRefertoLAB1 = (UnicodeSrl.ScciCore.Interfacce.IViewUserControlMiddle)caller.Esegui(oDictionary);

                ((Control)_ucEasyRefertoLAB1).BackColor = System.Drawing.Color.Transparent;
                ((Control)_ucEasyRefertoLAB1).Location = new System.Drawing.Point(0, 0);
                ((Control)_ucEasyRefertoLAB1).Name = "ucEasyRefertoLAB1";
                ((Control)_ucEasyRefertoLAB1).Size = new System.Drawing.Size(689, 555);
                ((Control)_ucEasyRefertoLAB1).TabIndex = 1;

                this.ultraTabPageControl2.Controls.Add(((Control)_ucEasyRefertoLAB1));

                ((Control)_ucEasyRefertoLAB1).BringToFront();
                ((Control)_ucEasyRefertoLAB1).Dock = System.Windows.Forms.DockStyle.Fill;

                _ucEasyRefertoLAB1.Carica();

                caller.Dispose();

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, @"Impossibile Caricare il Plugin di Labooratorio." + Environment.NewLine + @"Contattare amministratori di sistema", "CaricaPluginLAB", this.Name);
                            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
        }

        private void CaricaCDSSLAB()
        {

            try
            {

                this.ubPACS.Visible = true;
                this.webBrowser.Visible = false;
                this.ucEasyTabControl.SelectedTab = this.ucEasyTabControl.Tabs["tabLAB"];

                T_CDSSPlugins oCDSSPlugins = new T_CDSSPlugins();
                oCDSSPlugins.Codice = @"CDSSLAB-LAB";
                if (oCDSSPlugins.TrySelect())
                {

                    Plugin oPlugin = new Plugin(oCDSSPlugins.Codice, oCDSSPlugins.Descrizione, oCDSSPlugins.NomePlugin, oCDSSPlugins.Comando, "", "", 0, null);
                    Risposta oRispostaMenuEsegui = PluginClientStatics.PluginClientMenuEsegui(oPlugin, new object[1] { new object() });
                    if (oRispostaMenuEsegui.Successo == true)
                    {

                        _ucEasyRefertoLAB1 = ((Interfacce.IViewUserControlMiddle)oRispostaMenuEsegui.Parameters[0]);
                        _ucEasyRefertoLAB1.Carica();

                        ((Control)_ucEasyRefertoLAB1).BackColor = System.Drawing.Color.Transparent;
                        ((Control)_ucEasyRefertoLAB1).Location = new System.Drawing.Point(0, 0);
                        ((Control)_ucEasyRefertoLAB1).Name = "ucEasyRefertoLAB1";
                        ((Control)_ucEasyRefertoLAB1).Size = new System.Drawing.Size(689, 555);
                        ((Control)_ucEasyRefertoLAB1).TabIndex = 1;

                        this.ultraTabPageControl2.Controls.Add(((Control)_ucEasyRefertoLAB1));

                        ((Control)_ucEasyRefertoLAB1).BringToFront();
                        ((Control)_ucEasyRefertoLAB1).Dock = System.Windows.Forms.DockStyle.Fill;

                    }
                    else
                    {
                        Exception rex = oRispostaMenuEsegui.ex;
                        CoreStatics.ExGest(ref rex, oRispostaMenuEsegui.Parameters[0].ToString(), "CaricaCDSS", this.Text, true, this.Name, "Errore", MessageBoxIcon.Warning, MessageBoxButtons.OK, MessageBoxDefaultButton.Button1);
                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaCDSS", this.Text);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

        #endregion

        #region EVENTI

        private void frmPUEvidenzaClinica_ImmagineClick(object sender, ImmagineTopClickEventArgs e)
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
                            frmPUEvidenzaClinica_PulsanteAvantiClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti));
                        }
                        else if (CoreStatics.CoreApplication.MovSchedaSelezionata.Azione == EnumAzioni.MOD)
                        {
                            CoreStatics.CoreApplication.MovSchedaSelezionata.AggiungiSegnalibro();
                            this.ucTopModale.Focus();
                            frmPUEvidenzaClinica_PulsanteAvantiClick(null, new PulsanteBottomClickEventArgs(EnumPulsanteBottom.Avanti));
                        }
                        break;

                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        private void frmPUEvidenzaClinica_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            this.DialogResult = _formDialogResult;
            this.Close();
        }

        private void frmPUEvidenzaClinica_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
        {
            if (this.uchkVista.Visible && this.uchkVista.Checked)
            {
                                if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.Vista())
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
                else
                    this.uchkVista.Checked = false;
            }
            else
            {
                this.DialogResult = _formDialogResult;
                this.Close();
            }
        }

        private void frmPUEvidenzaClinica_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (this.webBrowser.Visible) this.webBrowser.Navigate("about:blank");
                if (_tempfiles != null && _tempfiles.Count > 0)
                {
                    for (int i = 0; i < _tempfiles.Count; i++)
                    {
                        try
                        {
                            if (System.IO.File.Exists(_tempfiles[i])) System.IO.File.Delete(_tempfiles[i]);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void ubOrdini_Click(object sender, EventArgs e)
        {
                        try
            {
                                this.ImpostaCursore(enum_app_cursors.WaitCursor);
                string sNumeroOrdine = CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.NumeroOrdine;
                if (sNumeroOrdine == null || sNumeroOrdine.Trim() == "")
                {
                    this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                    easyStatics.EasyMessageBox("Numero Ordine non presente per il referto selezionato." + Environment.NewLine + @"Impossibile aprire l'Order Entry.", "Apertura Ordine", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {

                                        if (CoreStatics.CoreApplication.Episodio != null && CoreStatics.CoreApplication.Trasferimento != null && CoreStatics.CoreApplication.Paziente != null)
                    {
                                                string sCodAziOrdine = string.Empty;
                        if (CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento != null && CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento != string.Empty)
                        { sCodAziOrdine = CoreStatics.CoreApplication.Trasferimento.CodAziTrasferimento; }
                        else
                        { sCodAziOrdine = CoreStatics.CoreApplication.Episodio.CodAzienda; }

                        CoreStatics.CoreApplication.MovOrdineSelezionato = new MovOrdine(sNumeroOrdine,
                                                                                        CoreStatics.CoreApplication.Ambiente, CoreStatics.CoreApplication.Episodio.ID,
                                                                                        CoreStatics.CoreApplication.Episodio.CodTipoEpisodio,
                                                                                        CoreStatics.CoreApplication.Episodio.NumeroEpisodio,
                                                                                        CoreStatics.CoreApplication.Episodio.NumeroListaAttesa,
                                                                                        sCodAziOrdine,
                                                                                        CoreStatics.CoreApplication.Trasferimento.ID,
                                                                                        CoreStatics.CoreApplication.Trasferimento.CodUO,
                                                                                        CoreStatics.CoreApplication.Trasferimento.CodUA,
                                                                                        CoreStatics.CoreApplication.Paziente);
                    }

                    else
                    {
                                                if (CoreStatics.CoreApplication.Paziente != null)
                            CoreStatics.CoreApplication.MovOrdineSelezionato = new MovOrdine(sNumeroOrdine,
                                                                                            CoreStatics.CoreApplication.Ambiente,
                                                                                            "",
                                                                                            "",
                                                                                            "",
                                                                                            "",
                                                                                            "",
                                                                                            "",
                                                                                            "",
                                                                                            CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata,
                                                                                            CoreStatics.CoreApplication.Paziente);
                        else
                            CoreStatics.CoreApplication.MovOrdineSelezionato = new MovOrdine(sNumeroOrdine,
                                                                                            CoreStatics.CoreApplication.Ambiente,
                                                                                            "",
                                                                                            "",
                                                                                            "",
                                                                                            "",
                                                                                            "",
                                                                                            "",
                                                                                            "",
                                                                                            CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata,
                                                                                            null);
                    }

                    if (CoreStatics.CoreApplication.MovOrdineSelezionato == null)
                    {
                        this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                        easyStatics.EasyMessageBox("Impossibile recuperare i dettagli del Numero Ordine [" + sNumeroOrdine + @"]." + Environment.NewLine + @"Impossibile aprire l'Order Entry.", "Apertura Ordine", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (CoreStatics.CoreApplication.MovOrdineSelezionato.UltimaEccezioneGenerata != null)
                    {
                        this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                        Exception rex = CoreStatics.CoreApplication.MovOrdineSelezionato.UltimaEccezioneGenerata;
                        CoreStatics.ExGest(ref rex, @"Impossibile recuperare i dettagli del Numero Ordine [" + sNumeroOrdine + @"]." + Environment.NewLine + @"Impossibile aprire l'Order Entry.", "Apertura Ordine", this.Name);
                    }
                    else
                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.VisualizzaOrdine);
                }

            }
            catch (Exception ex)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                CoreStatics.ExGest(ref ex, @"Apertura Ordini non disponibile." + Environment.NewLine + @"Contattare amministratori di sistema", "ubOrdini_Click", this.Name);
            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }
        }

        private void ubGrafico_Click(object sender, EventArgs e)
        {

            try
            {
                                if (CoreStatics.CoreApplication.DefinizioneGraficoSelezionata != null)
                {
                    this.ImpostaCursore(enum_app_cursors.WaitCursor);

                                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.GraficiEvidenzaClinica);
                }
                else
                    easyStatics.EasyMessageBox("Parametri mancanti!", "Grafico", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            catch (Exception ex)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                CoreStatics.ExGest(ref ex, @"Apertura Grafico non disponibile." + Environment.NewLine + @"Contattare amministratori di sistema", "ubGrafico_Click", this.Name);
                            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

        private void ubPDF_Click(object sender, EventArgs e)
        {
            try
            {
                this.ImpostaCursore(enum_app_cursors.WaitCursor);
                string sreftemp = System.IO.Path.Combine(FileStatics.GetSCCITempPath(), "referto" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + @".pdf");
                byte[] pdf = CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.PDF;
                if (pdf == null || pdf.Length <= 0)
                {
                    this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                    easyStatics.EasyMessageBox("Documento non presente.", "Apertura Referto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (UnicodeSrl.Framework.Utility.FileSystem.ByteArrayToFile(sreftemp, ref pdf))
                    {
                        if (System.IO.File.Exists(sreftemp))
                        {
                            _tempfiles.Add(sreftemp);

                                                        bool bAbilitaVisto = false;
                            if (this.uchkVista.Visible && this.uchkVista.Enabled && !this.uchkVista.Checked) bAbilitaVisto = true;

                            easyStatics.ShellExecute(sreftemp, "", false, string.Empty, bAbilitaVisto);
                            
                            if (bAbilitaVisto)
                            {
                                                                if (CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.CodStatoEvidenzaClinicaVisione == EnumStatoEvidenzaClinicaVisione.VS.ToString())
                                {
                                    this.uchkVista.Visible = false;
                                                                        _formDialogResult = System.Windows.Forms.DialogResult.OK;
                                }
                            }

                        }
                    }
                }
                            }
            catch (Exception ex)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                CoreStatics.ExGest(ref ex, @"Apertura Documento non disponibile." + Environment.NewLine + @"Contattare amministratori di sistema", "ubPDF_Click", this.Name);
                            }
            finally
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }

        private void ubPACS_Click(object sender, EventArgs e)
        {
            try
            {

                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                                CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.AccessNumber = string.Empty;

                if (_ucEasyRefertoLAB1 != null)
                {
                    object ret = ((UnicodeSrl.ScciCore.Interfacce.IViewUserControlMiddlePlugin)_ucEasyRefertoLAB1).EseguiComando(null);
                    if (ret != null)
                    {
                        Dictionary<string, string> dicret = (Dictionary<string, string>)ret;
                        if (dicret.Count > 0 && dicret.ContainsKey("AccessNumber"))
                        {
                            string accessnumber = dicret["AccessNumber"];
                            if (accessnumber != null && accessnumber != string.Empty && accessnumber.Trim() != "")
                            {
                                CoreStatics.CoreApplication.MovEvidenzaClinicaSelezionato.AccessNumber = accessnumber;
                                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.EvidenzaClinicaPACS) == DialogResult.OK)
                                {
                                                                    }


                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
                CoreStatics.ExGest(ref ex, @"Accesso PACS non disponibile." + Environment.NewLine + @"Contattare amministratori di sistema", "ubPACS_Click", this.Name);
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
