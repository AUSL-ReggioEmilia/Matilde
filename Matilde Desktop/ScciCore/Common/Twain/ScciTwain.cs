using NTwain;
using NTwain.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnicodeSrl.ScciCore.Common
{
    public class ScciTwain :
IDisposable
    {
        public event EventHandler<ImageCapturedEventArgs> ImageCaptured;

        public event EventHandler<TwainStateChangedEventArgs> TwainStateChanged;

        public ScciTwain()
        {
            this.TwainSources = new List<DataSource>();

            this.Initialize();
        }

        #region IDisposable Support
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if ((disposing) && (this.TwainSession != null))
                {
                    this.CloseSession();
                }

                disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region     Props

        private bool IsInitialized { get; set; }

        public TwainSession TwainSession { get; set; }

        public List<DataSource> TwainSources { get; set; }

        public DataSource CurrentSource
        {
            get
            {
                if (this.TwainSession == null) return null;
                return this.TwainSession.CurrentSource;
            }
        }

        public en_Twain_State State
        {
            get
            {
                if (this.TwainSession != null)
                    return (en_Twain_State)this.TwainSession.State;
                else
                    return en_Twain_State.Undefined;
            }
        }

        #endregion  Props

        #region Metodi Pub

        public void Initialize()
        {
            if (this.IsInitialized)
            {
                this.throwException("Twain già inizializzato");
                return;
            }

            TWIdentity appId = TWIdentity.CreateFromAssembly(DataGroups.Image, Assembly.GetEntryAssembly());
            this.TwainSession = new TwainSession(appId);

            this.TwainSession.TransferError += TwainSession_TransferError;
            this.TwainSession.DataTransferred += TwainSession_DataTransferred;
            this.TwainSession.SourceDisabled += TwainSession_SourceDisabled;
            this.TwainSession.SourceChanged += TwainSession_SourceChanged;
            this.TwainSession.TransferReady += TwainSession_TransferReady;
            this.TwainSession.StateChanged += TwainSession_StateChanged;

            if (this.TwainSession.State < 3)
                this.TwainSession.Open();

            this.IsInitialized = true;
        }


        public void CloseSession()
        {
            if (this.TwainSession == null) return;

            this.TwainSession.TransferError -= TwainSession_TransferError;
            this.TwainSession.DataTransferred -= TwainSession_DataTransferred;
            this.TwainSession.SourceDisabled -= TwainSession_SourceDisabled;
            this.TwainSession.SourceChanged -= TwainSession_SourceChanged;
            this.TwainSession.TransferReady -= TwainSession_TransferReady;

            if (this.TwainSession.State == 4)
            {
                this.TwainSession.CurrentSource.Close();
            }
            if (this.TwainSession.State == 3)
            {
                this.TwainSession.Close();
            }
            if (this.TwainSession.State > 2)
            {
                this.TwainSession.ForceStepDown(2);
            }

            this.TwainSession = null;
            this.IsInitialized = false;
        }

        public void LoadSources()
        {
            this.TwainSources.Clear();

            if (this.TwainSession.State >= 3)
            {
                foreach (DataSource src in this.TwainSession)
                {
                    this.TwainSources.Add(src);
                }
            }
            else
                this.throwException("LoadSources, State non previsto: " + this.TwainSession.State.ToString());
        }

        public bool Scan(SourceEnableMode sourceEnableMode, IntPtr parentHandle)
        {
            if (this.TwainSession.State != 4)
                this.throwException("Scan, State non previsto: " + this.TwainSession.State.ToString());

            ReturnCode rc = this.TwainSession.CurrentSource.Enable(sourceEnableMode, false, parentHandle);

            return (rc == ReturnCode.Success);
        }

        #endregion Metodi Pub

        #region     Twain Events

        private void TwainSession_StateChanged(object sender, EventArgs e)
        {
            if (this.TwainStateChanged != null)
            {
                TwainStateChangedEventArgs args = new TwainStateChangedEventArgs(this.TwainSession.State);
                this.TwainStateChanged(this, args);
            }

        }

        private void TwainSession_TransferReady(object sender, TransferReadyEventArgs e)
        {
        }

        private void TwainSession_SourceChanged(object sender, EventArgs e)
        {
        }

        private void TwainSession_SourceDisabled(object sender, EventArgs e)
        {
        }

        private void TwainSession_DataTransferred(object sender, DataTransferredEventArgs e)
        {
            Image img = null;
            if (e.NativeData != IntPtr.Zero)
            {
                var stream = e.GetNativeImageStream();
                if (stream != null)
                {
                    img = Image.FromStream(stream);
                }
            }
            else if (!string.IsNullOrEmpty(e.FileDataPath))
            {
                img = new Bitmap(e.FileDataPath);
            }

            if (this.ImageCaptured != null)
            {
                ImageCapturedEventArgs args = new ImageCapturedEventArgs(img);
                this.ImageCaptured(this, args);
            }

        }

        private void TwainSession_TransferError(object sender, TransferErrorEventArgs e)
        {
            throwException(e.Exception);
        }

        #endregion  Twain Events


        #region     Metodi Priv

        private void throwException(Exception ex)
        {
            UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
        }

        private void throwException(string error)
        {
            Exception ex = new Exception(error);
            this.throwException(ex);
        }



        #endregion  Metodi Priv

    }
}
