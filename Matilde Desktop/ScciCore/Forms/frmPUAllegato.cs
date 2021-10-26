using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;
using UnicodeSrl.ScciResource;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinTree;
using UnicodeSrl.Scci;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.ScciCore.Common.Twain;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;

namespace UnicodeSrl.ScciCore
{
    public partial class frmPUAllegato : frmBaseModale, Interfacce.IViewFormlModal
    {

        #region Declare

        private bool _alertetichettastampata = false;

                private ucSegnalibri _ucSegnalibri = null;

                private ucEasyTreeView _ucEasyTreeView = null;
        private ucEasyPopUpFolder _ucEasyPopUpFolder = null;

        #endregion

        public frmPUAllegato()
        {
            InitializeComponent();
        }

        #region Interface

        public void Carica()
        {
            try
            {
                this.Text = CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.Descrizione;
                this.Icon = ScciResource.Risorse.GetIconFromResource(ScciResource.Risorse.GC_ALLEGATI_16);

                _alertetichettastampata = false;

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

                                if (CoreStatics.CoreApplication.MovAllegatoSelezionato.IDMovAllegato == string.Empty ||
                    CoreStatics.CoreApplication.MovAllegatoSelezionato.IDMovAllegato == @"")
                {
                    this.ubZoomTipo.Visible = true;
                }
                else
                {
                    this.ubZoomTipo.Visible = false;
                }

                this.rtfTesto.ViewInit();
                this.rtfNota.ViewInit();

                this.rtfTesto.ViewFont = DrawingProcs.getFontFromString(Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF));
                this.rtfNota.ViewFont = DrawingProcs.getFontFromString(Database.GetConfigTable(EnumConfigTable.FontPredefinitoRTF));

                this.rtfTesto.ViewUseLargeImages = true;
                this.rtfNota.ViewUseLargeImages = true;

                if (CoreStatics.CoreApplication.MovAllegatoSelezionato.CodFormatoAllegato == MovAllegato.FORMATO_ELETTRONICO)
                {
                    
                    if (CoreStatics.CoreApplication.MovAllegatoSelezionato.Azione == EnumAzioni.INS)
                    {
                                                this.lblDocumento.Text = "Cerca File:";
                        this.ubDocumento.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_ALLEGATOELETTRONICO_IMPORTA_256);
                        this.ubDocumento.ShortcutKey = Keys.Add;

                                                this.ubScanner.Appearance.Image = ScciResource.Risorse.GetImageFromResource(ScciResource.Risorse.GC_SCANNERIMPORTA_256);
                        this.ubScanner.PercImageFill = 0.75F;
                        this.ubScanner.ShortcutKey = Keys.S;
                    }
                    else
                    {
                                                this.lblDocumento.Text = "Documento:";
                        this.ubDocumento.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_DOWNLOADDOCUMENTO_256);
                        this.ubDocumento.ShortcutKey = Keys.A;
                    }

                }
                else
                {
                    
                    this.lblDocumento.Text = "ID Documento:";

                    this.ubDocumento.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_ALLEGATOSTAMPAETICHETTE_256);
                    this.ubDocumento.ShortcutKey = Keys.S;
                }
                this.ubDocumento.PercImageFill = 0.75F;
                this.ubDocumento.ShortcutFontRelativeDimension = easyStatics.easyRelativeDimensions.Small;

                this.utxtFolder.ButtonsRight["Sel"].Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FOLDER_256);
                this.utxtFolder.ButtonsRight["Add"].Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_FOLDERAGGIUNGI_256);

                this.uceCodEntita.Items.Clear();
                if (CoreStatics.CoreApplication.Trasferimento != null)
                {
                    this.uceCodEntita.Items.Add("CAR", "Cartella");
                }
                this.uceCodEntita.Items.Add("PAZ", "Paziente");

                this.ubRTFTesto.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_RTF_256);
                this.ubRTFTesto.PercImageFill = 0.75F;
                this.ubRTFNota.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_RTF_256);
                this.ubRTFNota.PercImageFill = 0.75F;

            }
            catch (Exception)
            {
            }
        }

        public void Aggiorna()
        {
            try
            {
                this.ubScanner.Visible = false;

                if (CoreStatics.CoreApplication.MovAllegatoSelezionato.DataEvento > DateTime.MinValue)
                    this.udteDataEvento.Value = CoreStatics.CoreApplication.MovAllegatoSelezionato.DataEvento;

                if (CoreStatics.CoreApplication.MovAllegatoSelezionato.Azione != EnumAzioni.INS)
                {
                                        this.lblInfoRilevazione.Text = "Caricato da " + CoreStatics.CoreApplication.MovAllegatoSelezionato.DescrUtenteRilevazione.ToUpper();
                    this.lblInfoRilevazione.Text += " il " + CoreStatics.CoreApplication.MovAllegatoSelezionato.DataRilevazione.ToString("dd/MM/yyyy");
                    this.lblInfoRilevazione.Text += " alle " + CoreStatics.CoreApplication.MovAllegatoSelezionato.DataRilevazione.ToString("HH:mm");
                    if (CoreStatics.CoreApplication.MovAllegatoSelezionato.InfoFirmaDigitale != string.Empty)
                    {
                        this.lblInfoRilevazione.Text += " " + CoreStatics.CoreApplication.MovAllegatoSelezionato.InfoFirmaDigitale;
                    }
                }


                this.lblZoomTipoAllegato.Text = string.Empty;
                if (CoreStatics.CoreApplication.MovAllegatoSelezionato.Azione != EnumAzioni.INS)
                {
                    this.lblZoomTipoAllegato.Text = CoreStatics.CoreApplication.MovAllegatoSelezionato.DescrTipoAllegato;
                    this.tlpFirmaDigitale.Visible = false;
                }
                else
                {
                    if (CoreStatics.CoreApplication.MovAllegatoSelezionato.DescrTipoAllegato == "")
                        this.lblZoomTipoAllegato.Text = @"Selezionare Tipo Allegato";
                    else
                        this.lblZoomTipoAllegato.Text = CoreStatics.CoreApplication.MovAllegatoSelezionato.DescrTipoAllegato;

                                                                                string codua = string.Empty;

                    if (CoreStatics.CoreApplication.Trasferimento != null)
                    {
                        codua = CoreStatics.CoreApplication.Trasferimento.CodUA;
                    }
                    else
                    {
                        codua = CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata;
                    }

                    this.tlpFirmaDigitale.Visible = DBUtils.ModuloUAAbilitato(codua, EnumUAModuli.FirmaD_Allegati);

                }

                if (CoreStatics.CoreApplication.MovAllegatoSelezionato.CodFormatoAllegato == MovAllegato.FORMATO_ELETTRONICO)
                {
                                        if (CoreStatics.CoreApplication.MovAllegatoSelezionato.Azione != EnumAzioni.INS)
                    {
                        this.utxtDocumento.Text = CoreStatics.CoreApplication.MovAllegatoSelezionato.NomeFile;
                        this.ubScanner.Visible = false;
                    }
                    else
                    {
                                                this.ubScanner.Visible = true;
                    }
                }
                else
                {
                                        this.utxtDocumento.Text = CoreStatics.CoreApplication.MovAllegatoSelezionato.IDDocumento;
                    this.ubScanner.Visible = false;
                }

                                if (CoreStatics.CoreApplication.MovAllegatoSelezionato.CodEntita == string.Empty)
                {
                    this.uceCodEntita.SelectedIndex = 0;
                    this.uceCodEntita.Enabled = true;
                }
                else
                {
                    this.uceCodEntita.Value = CoreStatics.CoreApplication.MovAllegatoSelezionato.CodEntita;
                    this.uceCodEntita.Enabled = false;
                }
                this.utxtFolder.Text = CoreStatics.CoreApplication.MovAllegatoSelezionato.Folder;

                this.rtfTesto.ViewRtf = CoreStatics.CoreApplication.MovAllegatoSelezionato.TestoRTF;
                this.rtfNota.ViewRtf = CoreStatics.CoreApplication.MovAllegatoSelezionato.NotaRTF;

                BloccaControlli(false);

            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "Aggiorna", this.Text);
            }

        }

        public bool Salva()
        {
            bool bReturn = false;
            try
            {
                if (ControllaValori())
                {
                    this.ImpostaCursore(enum_app_cursors.WaitCursor);

                    BloccaControlli(true);

                    CoreStatics.CoreApplication.MovAllegatoSelezionato.DataEvento = (DateTime)this.udteDataEvento.Value;
                    CoreStatics.CoreApplication.MovAllegatoSelezionato.TestoRTF = this.rtfTesto.ViewRtf;
                    CoreStatics.CoreApplication.MovAllegatoSelezionato.NotaRTF = this.rtfNota.ViewRtf;
                                                            CoreStatics.CoreApplication.MovAllegatoSelezionato.CodEntita = this.uceCodEntita.Value.ToString();
                    if (this.uceFirmaDigitale.Checked == true && CoreStatics.CoreApplication.MovAllegatoSelezionato.Azione == EnumAzioni.INS)
                    {
                        CoreStatics.CoreApplication.MovAllegatoSelezionato.ID = Guid.NewGuid().ToString();
                        if (this.FirmaDigitaleDocumento())
                        {
                            bReturn = CoreStatics.CoreApplication.MovAllegatoSelezionato.Salva();
                        }
                    }
                    else
                    {
                        bReturn = CoreStatics.CoreApplication.MovAllegatoSelezionato.Salva();
                    }


                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "Salva", this.Text);
            }
            finally
            {
                BloccaControlli(false);
                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

            return bReturn;
        }

        private bool ControllaValori()
        {
            bool bOK = true;

                        if (bOK && CoreStatics.CoreApplication.MovAllegatoSelezionato.CodTipoAllegato == "")
            {
                easyStatics.EasyMessageBox("Inserire Tipo Allegato !", "Allegato", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.ubZoomTipo.Focus();
                bOK = false;
            }
            if (bOK && !this.udteDataEvento.ReadOnly && this.udteDataEvento.Value == null)
            {
                easyStatics.EasyMessageBox("Inserire Data Evento!", "Allegato", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                this.udteDataEvento.Focus();
                bOK = false;
            }

            if (bOK)
            {
                if (CoreStatics.CoreApplication.MovAllegatoSelezionato.CodFormatoAllegato == MovAllegato.FORMATO_ELETTRONICO)
                {
                    if (CoreStatics.CoreApplication.MovAllegatoSelezionato.Documento == null || this.utxtDocumento.Text.Trim() == "")
                    {
                        easyStatics.EasyMessageBox("Nessun documento allegato!", "Allegato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        bOK = false;
                    }
                }
                else
                {
                    
                    if (this.utxtDocumento.Text.Trim() == "")
                    {
                        easyStatics.EasyMessageBox("Inserire un ID Documento!", "Allegato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.utxtDocumento.Focus();
                        bOK = false;
                    }
                }
            }

            return bOK;
        }

        private void BloccaControlli(bool blocca)
        {
            try
            {
                this.udteDataEvento.ReadOnly = blocca;
                this.ubZoomTipo.Enabled = !blocca;

                if (CoreStatics.CoreApplication.MovAllegatoSelezionato.CodFormatoAllegato == MovAllegato.FORMATO_ELETTRONICO)
                {
                                        this.utxtDocumento.ReadOnly = true;
                }
                else
                {
                                        this.utxtDocumento.ReadOnly = true;                 }
                this.ubDocumento.Enabled = !blocca;

                this.rtfTesto.ViewReadOnly = blocca;
                this.rtfTesto.ViewShowToolbar = true;
                this.ubRTFTesto.Enabled = !blocca;

                this.rtfNota.ViewReadOnly = blocca;
                this.rtfNota.ViewShowToolbar = true;
                this.ubRTFNota.Enabled = !blocca;

                this.PulsanteAvantiAbilitato = !blocca;
                this.PulsanteIndietroAbilitato = !blocca;
            }
            catch (Exception)
            {
            }
        }

        private bool FirmaDigitaleDocumento()
        {

            bool bret = false;

            frmSmartCardProgress frmSC = new frmSmartCardProgress();
            frmSC.InizializzaEMostra(0, 4, this);
            frmSC.SetCursore(enum_app_cursors.WaitCursor);
            frmSC.SetStato(@"Firma digitale documento " + CoreStatics.CoreApplication.MovAllegatoSelezionato.NomeFile);

            try
            {

                try
                {

                                        frmSC.SetStato(@"Generazione Documento...");

                                        byte[] pdfContent = CoreStatics.CoreApplication.MovAllegatoSelezionato.Documento;
                    if (pdfContent == null || pdfContent.Length <= 0)
                    {
                        frmSC.SetLog(@"Errore Generazione documento", true);
                    }
                    else
                    {
                        bret = frmSC.ProvaAFirmare(ref pdfContent, EnumTipoDocumentoFirmato.ALLFM01, "Firma Digitale...", EnumEntita.ALL, CoreStatics.CoreApplication.MovAllegatoSelezionato.ID);
                    }
                }
                catch (Exception ex)
                {
                    if (frmSC != null) frmSC.SetLog(@"Errore Generazione documento: " + ex.Message, true);
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "FirmaDigitaleDocumento", this.Name);
            }
            finally
            {
                if (frmSC != null)
                {
                    frmSC.Close();
                    frmSC.Dispose();
                }

            }

            return bret;

        }

        #endregion

        #region Events Form

        private void frmPUAllegato_ImmagineClick(object sender, ImmagineTopClickEventArgs e)
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

        private void frmPUAllegato_PulsanteAvantiClick(object sender, PulsanteBottomClickEventArgs e)
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

        private void frmPUAllegato_PulsanteIndietroClick(object sender, PulsanteBottomClickEventArgs e)
        {
            bool bClose = true;

                        if (_alertetichettastampata)
            {
                if (easyStatics.EasyMessageBox(@"Hai stampato l'Etichetta ma stai uscendo senza salvare l'Allegato!" + Environment.NewLine + @"Vuoi veramente uscire senza salvare il Nuovo Allegato?", "Nuovo Allegato", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                    bClose = false;
            }

            if (bClose)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Close();
            }
        }

        #endregion

        #region Events

        private void ubZoomTipo_Click(object sender, EventArgs e)
        {
            if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.SelezioneTipoAllegato) == DialogResult.OK)
            {
                this.lblZoomTipoAllegato.Text = CoreStatics.CoreApplication.MovAllegatoSelezionato.DescrTipoAllegato;
            }
        }

        private void ubDocumento_Click(object sender, EventArgs e)
        {
            try
            {

                if (CoreStatics.CoreApplication.MovAllegatoSelezionato.CodFormatoAllegato == MovAllegato.FORMATO_ELETTRONICO)
                {
                    
                    if (CoreStatics.CoreApplication.MovAllegatoSelezionato.Azione != EnumAzioni.INS)
                    {
                                                if (CoreStatics.CoreApplication.MovAllegatoSelezionato.Documento == null)
                            easyStatics.EasyMessageBox("Nessun documento allegato!", "Apri Allegato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            CoreStatics.ApriAllegato(CoreStatics.CoreApplication.MovAllegatoSelezionato.Documento, CoreStatics.CoreApplication.MovAllegatoSelezionato.NomeFile);
                    }
                    else
                    {
                        
                                                int iMaxSize = 0;
                        string maxSize = Database.GetConfigTable(EnumConfigTable.AllegatiDimensioneMassimaMb);
                        if (maxSize.Trim() != "" && int.TryParse(maxSize, out iMaxSize)) iMaxSize *= 1048576;   
                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.Title = "Seleziona Documento";
                        if (iMaxSize > 0) ofd.Title += @" (max " + maxSize + @"MB)";
                        ofd.CheckFileExists = true;
                        ofd.CheckPathExists = true;
                        ofd.Multiselect = false;
                        ofd.Filter = @"Tutti i documenti (*.*)|*.*|Documenti PDF (*.pdf)|*.pdf|Documenti Word (*.doc)|*.doc|Documenti Word 2007 (*.docx)|*.docx";
                        ofd.FilterIndex = 0;
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            System.IO.FileInfo oFI = new System.IO.FileInfo(ofd.FileName);
                            if (iMaxSize > 0 && iMaxSize < oFI.Length)
                            {
                                string msg = @"Il documento selezionato """ + oFI.Name + @""" (" + (Convert.ToDecimal(oFI.Length) / Convert.ToDecimal(1048576)).ToString("0.00") + @"MB)" + Environment.NewLine;
                                msg += "supera la dimensione massima consentita (" + maxSize + "MB)!";
                                easyStatics.EasyMessageBox(msg, "Inserisci documento", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                            else
                            {
                                this.utxtDocumento.Text = ofd.FileName;

                                CoreStatics.CoreApplication.MovAllegatoSelezionato.NomeFile = System.IO.Path.GetFileName(ofd.FileName);
                                CoreStatics.CoreApplication.MovAllegatoSelezionato.Documento = UnicodeSrl.Framework.Utility.FileSystem.FileToByteArray(ofd.FileName);
                                this.uceFirmaDigitale.Enabled = (CoreStatics.CoreApplication.MovAllegatoSelezionato.EstensioneFile.ToUpper() == "PDF");
                                
                                                                if (this.uceFirmaDigitale.Enabled == false)
                                {
                                    this.uceFirmaDigitale.Checked = false;
                                }
                                
                            }

                        }
                        ofd.Dispose();
                    }
                }
                else
                {
                    
                                        this.Cursor = Cursors.WaitCursor;

                    Report item = new Report(Report.COD_REPORT_ETICHETTA_ALLEGATO, "", "Allegati", Report.COD_FORMATO_REPORT_CABLATO, "", "ReportEtichettaAllegati", false, null, false, false);

                    if (item != null)
                    {
                        CoreStatics.CoreApplication.Navigazione.Maschere.MascheraSelezionata.MascheraPartenza.Reports.ReportSelezionato = item;
                        CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.Report);

                                                if (CoreStatics.CoreApplication.MovAllegatoSelezionato.Azione == EnumAzioni.INS)
                            _alertetichettastampata = true;
                    }

                    this.Cursor = Cursors.Default;
                }
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ubDocumento_Click", this.Text);
            }
        }

        private void uceCodEntita_ValueChanged(object sender, EventArgs e)
        {
            if (CoreStatics.CoreApplication.MovAllegatoSelezionato.Azione == EnumAzioni.INS)
            {
                this.utxtFolder.Text = "";
            }
        }

        private void utxtFolder_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            try
            {

                switch (e.Button.Key)
                {

                    case "Sel":
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerFolder);
                        _ucEasyTreeView = new ucEasyTreeView();
                        int iWidth = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium)) * 40;
                        int iHeight = Convert.ToInt32((double)iWidth / 1.52D);
                        _ucEasyTreeView.Size = new Size(iWidth, iHeight);
                        _ucEasyTreeView.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Medium;
                        this.CaricaFolder(_ucEasyTreeView);
                        _ucEasyTreeView.Focus();
                        this.UltraPopupControlContainerFolder.Show();
                        break;

                    case "Add":
                        CoreStatics.SetUltraPopupControlContainer(this.UltraPopupControlContainerFolderAdd);
                        _ucEasyPopUpFolder = new ucEasyPopUpFolder();
                        _ucEasyPopUpFolder.Tag = "ADD";
                        _ucEasyPopUpFolder.Init(CoreStatics.CoreApplication.MovAllegatoSelezionato.CodEntita);
                        int iWidthAdd = Convert.ToInt32(easyStatics.getFontSizeInPointsFromRelativeDimension(easyStatics.easyRelativeDimensions.Medium)) * 40;
                        int iHeightAdd = Convert.ToInt32((double)iWidthAdd / 1.52D);
                        _ucEasyPopUpFolder.Size = new Size(iWidthAdd, iHeightAdd);
                        _ucEasyPopUpFolder.tvFolder.TextFontRelativeDimension = easyStatics.easyRelativeDimensions.Medium;
                        this.CaricaFolder(_ucEasyPopUpFolder.tvFolder);
                        _ucEasyPopUpFolder.Focus();
                        this.UltraPopupControlContainerFolderAdd.Show();
                        break;

                }

            }
            catch (Exception)
            {

            }

        }

        private void ubRTFTesto_Click(object sender, EventArgs e)
        {
            try
            {
                string codua = "";
                if (CoreStatics.CoreApplication.MovAllegatoSelezionato.Azione == EnumAzioni.INS)
                {
                                        if (CoreStatics.CoreApplication.Trasferimento != null)
                    {
                        codua = CoreStatics.CoreApplication.Trasferimento.CodUA;
                    }
                    else
                    {
                                                codua = CoreStatics.CoreApplication.AmbulatorialeUACodiceSelezionata;
                    }
                }
                else
                {
                                        codua = CoreStatics.CoreApplication.MovAllegatoSelezionato.CodUA;
                }
                CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato = new MovTestoPredefinito(UnicodeSrl.Scci.Enums.EnumEntita.ALL.ToString(), codua, CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice,
                                                                                                    CoreStatics.CoreApplication.MovAllegatoSelezionato.CodTipoAllegato, "");
                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(Scci.Enums.EnumMaschere.TestiPredefiniti) == System.Windows.Forms.DialogResult.OK)
                {
                                                            
                    string sRTFOriginale = this.rtfTesto.rtbRichTextBox.Rtf;
                    string sRTFDaAccodare = CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato.RitornoRTF;
                    UnicodeSrl.Scci.RTF.RtfFiles rtf = new UnicodeSrl.Scci.RTF.RtfFiles();
                    sRTFOriginale = rtf.joinRtf(sRTFDaAccodare, sRTFOriginale, true);
                    rtf = null;
                    this.rtfTesto.rtbRichTextBox.Rtf = sRTFOriginale;

                }
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ubRTFTesto_Click", this.Text);
            }
        }

        private void ubRTFNota_Click(object sender, EventArgs e)
        {
            try
            {
                string codua = "";
                if (CoreStatics.CoreApplication.Trasferimento != null) codua = CoreStatics.CoreApplication.Trasferimento.CodUA;
                CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato = new MovTestoPredefinito(UnicodeSrl.Scci.Enums.EnumEntita.ALL.ToString(), codua, CoreStatics.CoreApplication.Sessione.Utente.Ruoli.RuoloSelezionato.Codice,
                                                                                                    CoreStatics.CoreApplication.MovAllegatoSelezionato.CodTipoAllegato, "");
                if (CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(Scci.Enums.EnumMaschere.TestiPredefiniti) == System.Windows.Forms.DialogResult.OK)
                {
                                                            
                    string sRTFOriginale = this.rtfNota.rtbRichTextBox.Rtf;
                    string sRTFDaAccodare = CoreStatics.CoreApplication.MovTestoPredefinitoSelezionato.RitornoRTF;
                    UnicodeSrl.Scci.RTF.RtfFiles rtf = new UnicodeSrl.Scci.RTF.RtfFiles();
                    sRTFOriginale = rtf.joinRtf(sRTFDaAccodare, sRTFOriginale, true);
                    rtf = null;
                    this.rtfNota.rtbRichTextBox.Rtf = sRTFOriginale;
                }
            }
            catch (Exception ex)
            {
                ScciCore.CoreStatics.ExGest(ref ex, "ubRTFNota_Click", this.Text);
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

        #region UltraPopupControlContainerFolder

        private void UltraPopupControlContainerFolder_Closed(object sender, EventArgs e)
        {
            _ucEasyTreeView.AfterActivate -= _ucEasyTreeView_AfterActivate;
        }

        private void UltraPopupControlContainerFolder_Opened(object sender, EventArgs e)
        {
            _ucEasyTreeView.AfterActivate += _ucEasyTreeView_AfterActivate;
            _ucEasyTreeView.Focus();
        }

        private void UltraPopupControlContainerFolder_Opening(object sender, CancelEventArgs e)
        {
            UltraPopupControlContainer popup = sender as UltraPopupControlContainer;
            popup.PopupControl = _ucEasyTreeView;
        }

        private void _ucEasyTreeView_AfterActivate(object sender, Infragistics.Win.UltraWinTree.NodeEventArgs e)
        {

            CoreStatics.CoreApplication.MovAllegatoSelezionato.IDFolder = e.TreeNode.Key;
            this.utxtFolder.Text = e.TreeNode.FullPath;
            this.UltraPopupControlContainerFolder.Close();
        }

        private void CaricaFolder(ucEasyTreeView utv)
        {

            UltraTreeNode oNodeRoot = null;

            try
            {

                utv.Nodes.Clear();

                                oNodeRoot = new UltraTreeNode(CoreStatics.TV_ROOT, CoreStatics.TV_ROOT_ALLEGATI);
                oNodeRoot.LeftImages.Add(Risorse.GetImageFromResource(Risorse.GC_FOLDER_16));
                oNodeRoot.Tag = CoreStatics.TV_ROOT;
                utv.Nodes.Add(oNodeRoot);

                                                                Parametri op = new Parametri(CoreStatics.CoreApplication.Ambiente);
                op.Parametro.Add("IDPaziente", CoreStatics.CoreApplication.Paziente.ID);

                                if (CoreStatics.CoreApplication.Trasferimento == null)
                {
                    op.Parametro.Add("CodEntita", "PAZ");
                }
                else
                {
                    op.Parametro.Add("CodEntita", this.uceCodEntita.Value.ToString());
                }

                op.TimeStamp.CodEntita = EnumEntita.ALL.ToString();                 op.TimeStamp.CodAzione = EnumAzioni.VIS.ToString(); 
                                SqlParameterExt[] spcoll = new SqlParameterExt[1];

                string xmlParam = XmlProcs.XmlSerializeToString(op);
                spcoll[0] = new SqlParameterExt("xParametri", xmlParam, ParameterDirection.Input, SqlDbType.Xml);

                DataSet ds = Database.GetDatasetStoredProc("MSP_SelMovFolder", spcoll);

                                var results = from myRow in ds.Tables[0].AsEnumerable()
                              where myRow.IsNull("IDFolderPadre")
                              select myRow;
                if (results.Count() > 0)
                {
                    DataTable dtItem = results.CopyToDataTable();
                    this.CaricaFolderChild(ds, oNodeRoot, dtItem);
                }

                utv.PerformAction(UltraTreeAction.FirstNode, false, false);
                utv.PerformAction(UltraTreeAction.ExpandAllNode, false, false);

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaFolder", this.Name);
            }

        }

        private void CaricaFolderChild(DataSet ds, UltraTreeNode oNodeParent, DataTable dt)
        {

            try
            {

                foreach (DataRow oRow in dt.Rows)
                {

                    Infragistics.Win.UltraWinTree.UltraTreeNode oNodeChild = new Infragistics.Win.UltraWinTree.UltraTreeNode(oRow["ID"].ToString(), oRow["Descrizione"].ToString());
                    oNodeChild.LeftImages.Add(Risorse.GetImageFromResource((oRow["CodEntita"].ToString() == "CAR" ? Risorse.GC_FOLDERCARTELLA_16 : Risorse.GC_FOLDERPAZIENTE_16)));
                    oNodeChild.Tag = oRow["CodEntita"].ToString();
                    oNodeParent.Nodes.Add(oNodeChild);

                                        var results = from myRow in ds.Tables[0].AsEnumerable()
                                  where myRow["IDFolderPadre"].Equals(oRow["ID"])
                                  select myRow;
                    if (results.Count() > 0)
                    {
                        DataTable dtItem = results.CopyToDataTable();
                        this.CaricaFolderChild(ds, oNodeChild, dtItem);
                    }

                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "CaricaFolderChild", this.Name);
            }

        }

        #endregion

        #region UltraPopupControlContainerFolderAdd

        private void UltraPopupControlContainerFolderAdd_Closed(object sender, EventArgs e)
        {
            _ucEasyPopUpFolder.ucEasyButtonCancel.Click -= UcEasyButtonCancel_Click;
            _ucEasyPopUpFolder.ucEasyButtonConferma.Click -= UcEasyButtonConferma_Click;
        }

        private void UltraPopupControlContainerFolderAdd_Opened(object sender, EventArgs e)
        {
            _ucEasyPopUpFolder.ucEasyButtonCancel.Click += UcEasyButtonCancel_Click;
            _ucEasyPopUpFolder.ucEasyButtonConferma.Click += UcEasyButtonConferma_Click;
            _ucEasyPopUpFolder.Focus();
        }

        private void UltraPopupControlContainerFolderAdd_Opening(object sender, CancelEventArgs e)
        {
            UltraPopupControlContainer popup = sender as UltraPopupControlContainer;
            popup.PopupControl = _ucEasyPopUpFolder;
        }

        private void UcEasyButtonConferma_Click(object sender, EventArgs e)
        {
            try
            {


                if (_ucEasyPopUpFolder.txtFolder.Text != string.Empty)
                {

                    MovFolder oMovFolder = new MovFolder(CoreStatics.CoreApplication.MovAllegatoSelezionato.IDPaziente,
                                                            CoreStatics.CoreApplication.MovAllegatoSelezionato.IDEpisodio,
                                                            "",
                                                            (_ucEasyPopUpFolder.tvFolder.ActiveNode.Key == CoreStatics.TV_ROOT ? "" : _ucEasyPopUpFolder.tvFolder.ActiveNode.Key));
                    oMovFolder.CodStatoFolder = "AT";
                    oMovFolder.Descrizione = _ucEasyPopUpFolder.txtFolder.Text;
                    oMovFolder.CodUA = CoreStatics.CoreApplication.MovAllegatoSelezionato.CodUA;
                    oMovFolder.CodEntita = _ucEasyPopUpFolder.uceCodEntita.Value.ToString();
                    oMovFolder.Salva();
                    CoreStatics.CoreApplication.MovAllegatoSelezionato.IDFolder = oMovFolder.IDMovFolder;
                    this.utxtFolder.Text = oMovFolder.Descrizione;
                    oMovFolder = null;
                    this.UltraPopupControlContainerFolderAdd.Close();
                }

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "UcEasyButtonConferma_Click", this.Text);
            }

        }

        private void UcEasyButtonCancel_Click(object sender, EventArgs e)
        {
            this.UltraPopupControlContainerFolderAdd.Close();
        }

        #endregion


                                                private void ubScanner_Click(object sender, EventArgs e)
        {
                        if (utxtDocumento.Text != "")
            {
                DialogResult r = easyStatics.EasyMessageBox("Esiste un documento già specificato. Si desidera procedere?", "Acquisizione da scanner", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (r != DialogResult.Yes)
                    return;
            }

            DialogResult racq = CoreStatics.CoreApplication.Navigazione.Maschere.CaricaMaschera(EnumMaschere.AllegatiAcquisizione, false);

                                                
            if ((racq == DialogResult.OK) && (ScciTwainData.Images.Count > 0))
            {
                this.ImpostaCursore(enum_app_cursors.WaitCursor);

                string file = createPdfFromAcq();
                utxtDocumento.Text = file;

                FileInfo f = new FileInfo(file);

                CoreStatics.CoreApplication.MovAllegatoSelezionato.NomeFile = f.Name;
                CoreStatics.CoreApplication.MovAllegatoSelezionato.Documento = UnicodeSrl.Framework.Utility.FileSystem.FileToByteArray(file);

                                                                if (CoreStatics.CoreApplication.MovAllegatoSelezionato.EstensioneFile.ToUpper() == "PDF")
                {
                    this.uceFirmaDigitale.Enabled = true;
                }
                else
                {
                    this.uceFirmaDigitale.Enabled = false;
                    this.uceFirmaDigitale.Checked = false;
                }                

                this.ImpostaCursore(enum_app_cursors.DefaultCursor);
            }

        }


                                private string createPdfFromAcq()
        {
            try
            {
                string pathTmp = Path.GetTempPath();
                string fileName = Path.GetTempFileName().Replace(@".tmp", @".pdf");

                string file = Path.Combine(pathTmp, fileName);

                Document document = new Document(PageSize.LETTER);
                PdfWriter.GetInstance(document, new FileStream(file, FileMode.Create));

                document.Open();

                foreach (System.Drawing.Image image in ScciTwainData.Images)
                {
                    iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(image, System.Drawing.Imaging.ImageFormat.Bmp);

                                        float perc = 0f;

                    if (pic.Height > pic.Width)
                        perc = (document.PageSize.Height / pic.Height);
                    else
                        perc = (document.PageSize.Width / pic.Width);

                    perc = perc * .925f * 100;

                    pic.ScalePercent(perc);


                    document.Add(pic);
                    document.NewPage();
                }

                document.Close();

                return file;

            }
            catch (Exception ex)
            {
                CoreStatics.ExGest(ref ex, "createPdfFromAcq", this.Text);
            }

            return null;
        }


    }
}
