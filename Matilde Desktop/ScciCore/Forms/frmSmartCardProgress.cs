using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.Enums;

namespace UnicodeSrl.ScciCore
{
    public partial class frmSmartCardProgress : Form
    {

        private UnicodeSrl.SmartCard.SCHandler _SCHandler = null;
        private System.Windows.Forms.Timer _tmrControlloLettore = null;
        private Scci.Enums.enum_app_cursors _cursore = enum_app_cursors.DefaultCursor;

        private bool _controllaLettoreAbilitato = false;

        public frmSmartCardProgress()
        {
            InitializeComponent();
        }

        #region PROPERTIES

                                                
        
        internal ProgressBar ProgressBar { get { return this.MyProgressBar; } }

        internal UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum StatoSmartCard { get; private set; }

        internal bool TerminaOperazione { get; set; }

        #endregion

        #region METODI

        public void InizializzaEMostra(int progressValoreMin, int progressValoreMAX, object controlloChiamante, int delta = 0)
        {


            _controllaLettoreAbilitato = false;
            try
            {
                this.Icon = Risorse.GetIconFromResource(Risorse.GC_TESSERA_256);
            }
            catch
            {
            }
            this.TerminaOperazione = false;
            this.MyProgressBar.Minimum = progressValoreMin;
            this.MyProgressBar.Maximum = progressValoreMAX;
            this.MyProgressBar.Value = this.MyProgressBar.Minimum;
            this.lblInfo.Text = "";
            this.utxtLog.Text = "";


            this.btnCancel.Appearance.ImageHAlign = Infragistics.Win.HAlign.Left;
            this.btnCancel.Appearance.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.btnCancel.Appearance.Image = Risorse.GetImageFromResource(Risorse.GC_CANCELLATONDO_256);

            this.TopMost = true;
            Screen oScreen = Screen.PrimaryScreen;
            int nHeight = (oScreen.WorkingArea.Height / 100) * 28;
            int nWidth = oScreen.WorkingArea.Width;
            this.Size = new Size(nWidth, nHeight);

            InitSCHandler();

            this.Show();
                        if (controlloChiamante is frmBaseModale)
                this.Location = new Point(0, (oScreen.Bounds.Height - nHeight - delta));
            else
                this.Location = new Point(0, (oScreen.WorkingArea.Height - nHeight - delta));

            CoreStatics.CoreApplication.Navigazione.Maschere.TracciaNavigazione("Firma Digitale", false);

            Application.DoEvents();
        }

        internal void initTimerLettore()
        {
            try
            {
                if (_tmrControlloLettore == null)
                {
                    _tmrControlloLettore = new Timer();
                    _tmrControlloLettore.Tick += _tmrControlloLettore_Tick;
                    _tmrControlloLettore.Interval = 2000;                     _tmrControlloLettore.Enabled = false;
                }
            }
            catch
            {
            }
        }
        internal void startTimerLettore()
        {
            try
            {
                if (_tmrControlloLettore == null) initTimerLettore();
                if (_tmrControlloLettore != null && !_tmrControlloLettore.Enabled)
                {
                    _tmrControlloLettore.Enabled = true;
                    _tmrControlloLettore.Start();
                }
            }
            catch
            {
            }
        }
        internal void stopTimerLettore()
        {
            try
            {
                if (_tmrControlloLettore != null && _tmrControlloLettore.Enabled)
                {
                    _tmrControlloLettore.Stop();
                    _tmrControlloLettore.Enabled = false;
                }
            }
            catch
            {
            }
        }

        public void SetStato(string logText)
        {
            SetStato(-1, logText);
        }
        public void SetStato(int iValoreProgressBar, string logText)
        {
            try
            {
                if (iValoreProgressBar >= 0)
                    this.ProgressBar.Value = iValoreProgressBar;
                else
                    this.ProgressBar.Value += 1;
            }
            catch
            {
            }
            SetLog(logText, true);

        }

        public void SetLog(string logText, bool addTime)
        {
            try
            {
                if (this.utxtLog.Text != "") this.utxtLog.Text += Environment.NewLine;
                if (addTime) this.utxtLog.Text += DateTime.Now.ToString(@"HH:mm:ss.fff") + @" - ";
                this.utxtLog.Text += logText;

                this.utxtLog.SelectionStart = this.utxtLog.Text.Length;
                this.utxtLog.ScrollToCaret();

                Application.DoEvents();
            }
            catch
            {
            }
        }

        public bool ProvaAFirmare(ref byte[] pdfContent, EnumTipoDocumentoFirmato codTipoDocumentoFirmato, string testoStato, EnumEntita codEntita, string idEntita)
        {
            bool bForseFirmato = false;
            try
            {
                this.SetStato(testoStato);

                                UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum statoSC = SmartCard.SCStatus.SmartCardStatusEnum._undefined;
                bool bExitWhile = false;

                while (statoSC != SmartCard.SCStatus.SmartCardStatusEnum.allOK
                       && statoSC != SmartCard.SCStatus.SmartCardStatusEnum.cardReady
                       && !this.TerminaOperazione
                       && !bExitWhile)
                {

                    statoSC = this.StatoSmartCard;
                    Application.DoEvents();

                    bForseFirmato = (!this.TerminaOperazione && (statoSC == SmartCard.SCStatus.SmartCardStatusEnum.allOK || statoSC == SmartCard.SCStatus.SmartCardStatusEnum.cardReady));

                    if (bForseFirmato)
                    {
                        bForseFirmato = false;

                                                byte[] bySigned = this.FirmaDocumento(ref pdfContent);

                        if (bySigned != null && !this.TerminaOperazione)
                        {
                            MovDocumentiFirmati odocf = new MovDocumentiFirmati();
                            odocf.Azione = EnumAzioni.INS;
                            odocf.CodEntita = codEntita.ToString();
                            odocf.CodTipoDocumentoFirmato = codTipoDocumentoFirmato.ToString();
                            odocf.IDEntita = idEntita;
                            odocf.PDFFirmato = bySigned;
                            odocf.PDFNonFirmato = pdfContent;
                            bForseFirmato = (odocf.Salva());
                            bExitWhile = true;
                        }

                    }

                                        if (this.StatoSmartCard != SmartCard.SCStatus.SmartCardStatusEnum.cardReady) statoSC = this.StatoSmartCard;
                }
            }
            catch (Exception ex)
            {
                this.SetLog(@"Errore Firma Documento: " + ex.Message, true);
                bForseFirmato = false;
            }
            return bForseFirmato;
        }

        public void SetCursore(Scci.Enums.enum_app_cursors cursore)
        {
            try
            {
                _cursore = cursore;
                Form objForm = (Form)this;
                CoreStatics.impostaCursore(ref objForm, cursore);

                setCursorePulsanti();

            }
            catch
            {
            }
        }

        #endregion

        #region EVENTI FORM

        private void frmSmartCardProgress_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;

                        if (!this.DesignMode)
            {
                string fontFamily = null;
                                fontFamily = CoreStatics.getFontPredefinitoForm();
                if (fontFamily != null && fontFamily != "") this.Font = new Font(fontFamily, this.Font.Size, this.Font.Style);
            }
        }

        private void frmSmartCardProgress_KeyDown(object sender, KeyEventArgs e)
        {
            ScciCore.easyStatics.checkShortcutKeyDown(e.KeyCode, this.Controls, false, e.Modifiers);
        }

        private void frmSmartCardProgress_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (_SCHandler != null)
                {
                    _SCHandler.Dispose();
                    _SCHandler = null;
                }
            }
            catch
            {
            }
        }

        #endregion

        #region EVENTI

        private void PictureBox_Click(object sender, EventArgs e)
        {
            if (_controllaLettoreAbilitato) InitSCHandler();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.TerminaOperazione = true;

            try
            {
                SetLog("Termina Operazione...", true);
                stopTimerLettore();
                _controllaLettoreAbilitato = false;
                this.btnCancel.Enabled = false;

                setCursorePulsanti();
            }
            catch
            {
            }
        }

        private void _tmrControlloLettore_Tick(object sender, EventArgs e)
        {
            InitSCHandler();
        }

        private void setCursorePulsanti()
        {
            try
            {
                Control ctrlBtn1 = (Control)this.btnCancel;
                if (ctrlBtn1.Enabled)
                    CoreStatics.impostaCursore(ref ctrlBtn1, enum_app_cursors.DefaultCursor);
                else
                    CoreStatics.impostaCursore(ref ctrlBtn1, _cursore);
            }
            catch
            {
            }
        }

        #endregion

        #region SmartCard

        private void InitSCHandler()
        {
            try
            {
                if (_SCHandler != null)
                {
                    _SCHandler.SCStatusChanged -= _SCHandler_SCStatusChanged;
                    _SCHandler.Dispose();
                    GC.SuppressFinalize(_SCHandler);
                    _SCHandler = null;
                }

                _SCHandler = new UnicodeSrl.SmartCard.SCHandler();
                _SCHandler.SCStatusChanged += _SCHandler_SCStatusChanged;

                                mostraStatusSmartCard(_SCHandler.CurrentSCStatus);
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }
        }

        private void _SCHandler_SCStatusChanged(object sender, UnicodeSrl.SmartCard.SCStatusChangedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                showSCStatusDelegate d = new showSCStatusDelegate(mostraStatusSmartCard);
                this.Invoke(d, new object[] { e.SCStatus });
            }
            else
                mostraStatusSmartCard(e.SCStatus);
        }

        delegate void showSCStatusDelegate(UnicodeSrl.SmartCard.SCStatus scstatus);

        private void mostraStatusSmartCard(UnicodeSrl.SmartCard.SCStatus scstatus)
        {
            try
            {
                _controllaLettoreAbilitato = false;
                this.StatoSmartCard = scstatus.SmartCardStatus;
                System.Drawing.Color backcolor = System.Drawing.Color.LightGreen;
                string sMsg = "";
                bool starttimer = false;

                switch (scstatus.SmartCardStatus)
                {
                    case UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum._undefined:
                        backcolor = Color.LightGoldenrodYellow;
                        sMsg = @"Impossibile identificare stato lettore.";
                        this.PictureBox.Image = Properties.Resources.msg_warning;
                        _controllaLettoreAbilitato = true;
                        starttimer = true;
                        break;

                    case UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum.readerNotReady:
                                                _controllaLettoreAbilitato = true;
                        backcolor = Color.LightSalmon;
                        sMsg = scstatus.Message;
                        this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAERRORE_256);
                        starttimer = true;
                        break;

                    case UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum.cardNotReady:
                                                backcolor = Color.LightSalmon;
                        sMsg = scstatus.Message;
                        this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAMANCANTE_256);
                        break;

                    case UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum.cardReady:
                                                                        backcolor = System.Drawing.Color.LightGoldenrodYellow;
                        sMsg = scstatus.Message;
                        this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAATTESA_256);
                        break;

                    case UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum.certificateNotFound:
                                                                        backcolor = System.Drawing.Color.LightSalmon;
                        sMsg = scstatus.Message;
                        this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAERRORE_256);
                        break;

                    case UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum.allOK:
                                                backcolor = System.Drawing.Color.LightGreen;
                        sMsg = scstatus.Message;
                        this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAOK_256);
                        break;

                    case UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum.errorReadingCertificate:
                                                                        backcolor = System.Drawing.Color.LightSalmon;
                        sMsg = scstatus.Message;
                        this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAERRORE_256);

                        
                        break;
                    case UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum.otherError:
                                                backcolor = System.Drawing.Color.LightSalmon;
                        sMsg = scstatus.Message;
                        this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAERRORE_256);
                        break;
                    default:
                        break;
                }

                if (scstatus.lastException != null)
                {
                    if (sMsg.Trim() != "") sMsg += Environment.NewLine;
                    sMsg += @"[" + scstatus.lastException.Message + @"]";
                }

                if (sMsg.Trim() != "")
                {
                    this.lblInfo.Text = sMsg;
                }
                this.lblInfo.Appearance.BackColor = backcolor;

                if (starttimer)
                    startTimerLettore();
                else
                    stopTimerLettore();

                
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

        }

        internal byte[] FirmaDocumento(ref byte[] documento)
        {
            byte[] baFirmato = null;
            Image imgtss = null;
            try
            {
                this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAMANCANTE_256);
                UnicodeSrl.SmartCard.SCStatus currstatus = _SCHandler.CurrentSCStatus;
                if (currstatus.SmartCardStatus != UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum.allOK && currstatus.SmartCardStatus != UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum.cardReady)
                {
                    mostraStatusSmartCard(currstatus);
                }
                else
                {
                    imgtss = this.PictureBox.Image;
                    this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAATTESA_256);
                                        baFirmato = _SCHandler.SignDocument(documento);                 }
            }
            catch (Exception ex)
            {
                if (imgtss != null) this.PictureBox.Image = imgtss;
                if (ex.Message.ToUpper().IndexOf("USER") < 0 && ex.Message.ToUpper().IndexOf("UTENTE") < 0) throw ex;
            }

            return baFirmato;
        }

        #endregion

    }
}
