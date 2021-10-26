using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnicodeSrl.ScciResource;

namespace UnicodeSrl.ScciCore
{
    public partial class frmSmartCardInfo : Form
    {
        public frmSmartCardInfo()
        {
            InitializeComponent();

            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += new System.ComponentModel.DoWorkEventHandler(worker_DoWork);
            _worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

        }

        #region Declare

        BackgroundWorker _worker;
        int _lastPercent;
        string _lastStatus;
        DialogResult _formDialogResult = DialogResult.Cancel;

        private UnicodeSrl.SmartCard.SCHandler _SCHandler = null;
        
        public delegate void DoWorkEventHandler(frmSmartCardInfo sender, DoWorkEventArgs e);
        public event DoWorkEventHandler DoWork;

        #endregion

        #region Properties

        public ProgressBar ProgressBar { get { return this.MyProgressBar; } }
        
        public object Argument { get; set; }
        
        public RunWorkerCompletedEventArgs Result { get; private set; }

        public bool CancellationPending
        {
            get { return _worker.CancellationPending; }
        }

        public string CancellingText { get; set; }

        public string DefaultStatusText { get; set; }

        public UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum Stato { get; private set; }
                                                                                
                
                                
                                
                                
                                
                                
        
                
        #endregion

        #region Delegate

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
                                    if (DoWork != null)
                DoWork(this, e);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
                        if (e.ProgressPercentage >= this.ProgressBar.Minimum && e.ProgressPercentage <= this.ProgressBar.Maximum)
                this.ProgressBar.Value = e.ProgressPercentage;
                        if (e.UserState != null && !_worker.CancellationPending)
                SetLog(e.UserState.ToString(), true);         }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
                                    Result = e;
            if (e.Error != null)
                _formDialogResult = DialogResult.Abort;
            else if (e.Cancelled)
                _formDialogResult = DialogResult.Cancel;
            else
                _formDialogResult = DialogResult.OK;
            Close();
        }

        #endregion

        #region Method

        public void SetProgress(string status)
        {
                                    if (status != _lastStatus && !_worker.CancellationPending)
            {
                _lastStatus = status;
                _worker.ReportProgress(ProgressBar.Minimum - 1, status);
            }
        }
        public void SetProgress(int percent)
        {
                        if (percent != _lastPercent)
            {
                _lastPercent = percent;
                _worker.ReportProgress(percent);
            }
        }
        public void SetProgress(int percent, string status)
        {
                        if (percent != _lastPercent || (status != _lastStatus && !_worker.CancellationPending))
            {
                _lastPercent = percent;
                _lastStatus = status;
                _worker.ReportProgress(percent, status);
            }
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

            }
            catch
            {
            }
        }

        public byte[] SignDocument(ref byte[] documento)
        {
            byte[] baSigned = null;

            UnicodeSrl.SmartCard.SCStatus currstatus = _SCHandler.CurrentSCStatus;
            if (currstatus.SmartCardStatus != UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum.allOK && currstatus.SmartCardStatus != UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum.cardReady)
            {
                showSCStatus(currstatus, false);
            }
            else
                baSigned = _SCHandler.SignDocument(documento);

            return baSigned;
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

                                showSCStatus(_SCHandler.CurrentSCStatus, true);
            }
            catch (Exception ex)
            {
                Fw2010.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        private void _SCHandler_SCStatusChanged(object sender, UnicodeSrl.SmartCard.SCStatusChangedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                showSCStatusDelegate d = new showSCStatusDelegate(showSCStatus);
                this.Invoke(d, new object[] { e.SCStatus, !_worker.IsBusy });
            }
            else
                showSCStatus(e.SCStatus, !_worker.IsBusy);
        }

        delegate void showSCStatusDelegate(UnicodeSrl.SmartCard.SCStatus scstatus, bool startWorker);

        private void showSCStatus(UnicodeSrl.SmartCard.SCStatus scstatus, bool startWorker)
        {
            try
            {
                this.Stato = scstatus.SmartCardStatus;

                bool scReady = false;
                string sMsg = "";
                switch (scstatus.SmartCardStatus)
                {
                    case UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum._undefined:
                        sMsg = @"Impossibile identificare stato lettore.";
                        this.PictureBox.Image = null;
                        break;

                    case UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum.readerNotReady:
                                                sMsg = scstatus.Message;
                        this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAERRORE_256);
                        break;

                    case UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum.cardNotReady:
                                                sMsg = scstatus.Message;
                        this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAMANCANTE_256);
                        break;

                    case UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum.cardReady:
                                                                        scReady = true;
                        sMsg = scstatus.Message;
                        this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERA_256);
                        break;

                    case UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum.certificateNotFound:
                                                                        sMsg = scstatus.Message;
                        this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAERRORE_256);
                        break;

                    case UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum.allOK:
                                                sMsg = scstatus.Message;
                        this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAOK_256);
                        break;

                    case UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum.errorReadingCertificate:
                                                                        sMsg = scstatus.Message;
                        this.PictureBox.Image = Risorse.GetImageFromResource(Risorse.GC_TESSERAERRORE_256);

                        InitSCHandler();

                        break;
                    case UnicodeSrl.SmartCard.SCStatus.SmartCardStatusEnum.otherError:
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

                if (startWorker && scReady)
                {
                                        _worker.RunWorkerAsync(Argument);
                }

                            }
            catch (Exception ex)
            {
                Fw2010.Diagnostics.Statics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region Events

        private void frmSmartCardInfo_Load(object sender, EventArgs e)
        {

            
                        this.utxtLog.Text = "";
            Result = null;
            btnCancel.Enabled = true;
            ProgressBar.Value = ProgressBar.Minimum;
            _lastStatus = DefaultStatusText;
            _lastPercent = ProgressBar.Minimum;

                        InitSCHandler();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_worker.IsBusy)
            {
                                _worker.CancelAsync();
                                btnCancel.Enabled = false;
                lblInfo.Text = CancellingText;
            }
            else
            {
                _formDialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Close();
            }
        }

        private void btnAggiorna_Click(object sender, EventArgs e)
        {
            InitSCHandler();

                    }

        private void frmSmartCardInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = _formDialogResult;
        }

        #endregion

    }
}
