using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.ScciCore.WebSvc;

namespace UnicodeSrl.ScciCore.Common.MT
{
    internal class ScciWebSvcMT :
IDisposable
    {
        public event DelegateDatatableCompleted DatatableCompleted;
        public event DelegateThreadStarted ThreadStarted;
        public event DelegateThreadCompleted ThreadCompleted;
        public event DelegateThreadCancelled ThreadCancelled;
        public event DelegateLastThreadException LastThreadException;

        private bool m_cancel; private ThreadPriority m_priority; private bool m_isbkg; private string m_name;
        private Exception m_ex; private object m_result; private Thread m_WorkerThread;
        private EventWaitHandle m_evh = new ManualResetEvent(false);

        private object[] m_tparams;

        #region     Constructors and defaults

        public ScciWebSvcMT()
        {
            setDefaults();
        }

        public ScciWebSvcMT(ThreadPriority tp = ThreadPriority.Normal, bool isBackground = false, string name = "")
        {
            setDefaults();
            this.IsBackground = isBackground;
            this.Name = name;
            this.Priority = tp;
        }

        private void setDefaults()
        {
            m_cancel = false;
            this.Priority = ThreadPriority.Normal;
            this.IsBackground = true;
            this.Name = "";

        }


        ~ScciWebSvcMT()
        {
            Dispose();
        }

        #endregion 

        #region IDisposable

        public void Dispose()
        {
            if (DatatableCompleted != null) DatatableCompleted = null;
            if (ThreadStarted != null) ThreadStarted = null;
            if (ThreadCompleted != null) ThreadCompleted = null;
            if (ThreadCancelled != null) ThreadCancelled = null;

            m_result = null;
            m_WorkerThread = null;
        }

        #endregion IDisposable

        #region     Props

        public System.Threading.ThreadPriority Priority
        {
            get
            {
                return m_priority;
            }
            set
            {
                m_priority = value;
            }
        }
        public Thread ThreadObj
        {
            get { return m_WorkerThread; }
        }
        public bool IsBackground
        {
            get
            {
                return m_isbkg;
            }
            set
            {
                m_isbkg = value;
            }
        }
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }
        public Int32 ManagedThreadID
        {
            get { return m_WorkerThread.ManagedThreadId; }
        }
        public Exception LastException
        {
            get { return m_ex; }
            private set { m_ex = value; }
        }

        #endregion  Props



        #region         "Functions"


        public void Cancel()
        {
            m_cancel = true;
            m_evh.Set();

            if (this.ThreadCancelled != null)
                this.ThreadCancelled(this);
        }

        private void beforeExecute(ParameterizedThreadStart ts)
        {
            this.LastException = null;

            m_WorkerThread = new Thread(ts);
            m_WorkerThread.Priority = this.Priority;
            m_WorkerThread.IsBackground = this.IsBackground;
            m_WorkerThread.Name = this.Name;

            m_result = null;
            m_evh.Reset();

        }


        public void GetRisLabPaziente(string idSAC, DateTime datainizio, DateTime datafine)
        {
            m_tparams = new object[3];
            m_tparams[0] = idSAC;
            m_tparams[1] = datainizio;
            m_tparams[2] = datafine;

            beforeExecute(this._RicercaDatiLabDWH);

            SynchronizationContext _syncObj = SynchronizationContext.Current;
            m_WorkerThread.Start(_syncObj);

        }


        #endregion      "Functions"


        #region PostCallbacks

        private void _RicercaDatiLabDWH(object sync)
        {
            SvcRicoveriDWH.ScciRicoveriDWHClient dwhclnt = null;
            SynchronizationContext _syncObj = sync as SynchronizationContext;

            if (this.ThreadStarted != null) this.ThreadStarted.Invoke(this);

            try
            {
                List<SvcRicoveriDWH.RisultatiLabAll> oDwhListUM = new List<SvcRicoveriDWH.RisultatiLabAll>();

                string idSAC = m_tparams[0] as string;
                DateTime datainizio = Convert.ToDateTime(m_tparams[1]);
                DateTime datafine = Convert.ToDateTime(m_tparams[2]);

                dwhclnt = ScciSvcRef.GetSvcRicoveriDWH();
                oDwhListUM.AddRange(dwhclnt.RicercaDatiLabDWHAll(idSAC, datainizio, datafine));

                DataTable dt = CoreStatics.CreateDataTable<SvcRicoveriDWH.RisultatiLabAll>();
                CoreStatics.FillDataTable<SvcRicoveriDWH.RisultatiLabAll>(dt, oDwhListUM);

                m_result = dt;

                if (this.DatatableCompleted != null)
                    _syncObj.Send(this.OnDatatableCompleted, dt);

            }
            catch (Exception ex)
            {
                this.LastException = ex;
                if (this.LastThreadException != null)
                    _syncObj.Post(this.OnLastThreadException, ex);
            }
            finally
            {
                try
                {
                    if (dwhclnt != null
                        && (dwhclnt.State == System.ServiceModel.CommunicationState.Opened || dwhclnt.State == System.ServiceModel.CommunicationState.Opening))
                        dwhclnt.Close();

                    dwhclnt = null;

                    if (m_evh != null)
                        m_evh.Set();

                    if (this.ThreadCompleted != null) this.ThreadCompleted.Invoke(this);
                }
                catch
                {
                }
            }

        }


        private void OnLastThreadException(object state)
        {
            Exception ex = state as Exception;
            this.LastThreadException(this, ex);
        }


        private void OnDatatableCompleted(object state)
        {
            if (DatatableCompleted != null)
            {
                DataTable dt = state as DataTable;
                this.DatatableCompleted(this, dt);
            }

        }

        #endregion PostCallbacks







    }
}
