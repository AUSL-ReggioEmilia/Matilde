using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.ScciCore.Common.TimersCB;

namespace UnicodeSrl.ScciCore
{
    public class ScciMain : IDisposable
    {

        private System.Timers.Timer m_timer = null;
        private double _timerInterval = -1;
        internal const double C_TIMER = 2500;

        public delegate void TimerRefreshBottomEvtHandler();
        public delegate void TimerRefreshAmbEvtHandler(DateTime ora);

        public event TimerRefreshBottomEvtHandler TimerRefreshBottom;

        internal List<I_RefreshTimer_Controllo> RefreshControllo_Subscribers;

        public ScciMain()
        {
            try
            {

                RefreshControllo_Subscribers = new List<I_RefreshTimer_Controllo>();

                m_timer = new System.Timers.Timer();
                m_timer.Interval = timerInterval();
                m_timer.AutoReset = true;
                m_timer.Elapsed += new System.Timers.ElapsedEventHandler(m_timer_Elapsed);


            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            }
        }

        public void Dispose()
        {
            if (m_timer != null)
            {
                m_timer.Elapsed -= new System.Timers.ElapsedEventHandler(m_timer_Elapsed);
                m_timer = null;
            }

            if (RefreshControllo_Subscribers != null)
            {
                RefreshControllo_Subscribers.Clear();
                RefreshControllo_Subscribers = null;
            }

        }

        #region Method

        internal double timerInterval()
        {

            try
            {

                if (_timerInterval < 0)
                {
                    string sValue = UnicodeSrl.Scci.Statics.Database.GetConfigTable(EnumConfigTable.TimerApplicazione);
                    if (sValue.Trim() != "")
                    {
                        if (!double.TryParse(sValue.Trim(), out _timerInterval)) _timerInterval = C_TIMER;
                    }
                }
                if (_timerInterval < 0) _timerInterval = C_TIMER;

                return _timerInterval;

            }
            catch
            {
                return C_TIMER;
            }

        }

        internal void StartTimers()
        {

            try
            {

                try
                {
                    if (timerInterval() > 0)
                    {
                        m_timer.Enabled = true;
                        m_timer.Start();
                    }

                }
                catch (Exception ex)
                {
                    UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                }

            }
            catch (ThreadAbortException ex1)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex1);
                this.StopTimers();
            }

        }

        internal void StopTimers()
        {

            try
            {
                if (m_timer != null)
                {
                    m_timer.Stop();
                    m_timer.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

        }

        internal void Controllo()
        {

            try
            {
                if (this.RefreshControllo_Subscribers == null) return;
                if (this.RefreshControllo_Subscribers.Count == 0) return;

                TimersCB_Controllo_Data cbData = new TimersCB_Controllo_Data();
                DBUtils.get_Controllo(ref cbData);

                CoreStatics.CoreApplication.AggiornaIndicatori(cbData);

                foreach (I_RefreshTimer_Controllo sub in this.RefreshControllo_Subscribers)
                {
                    SynchronizationContext ctx = sub.SyncContext;
                    ctx.Post(sub.RefreshData, null);
                }


            }
            catch (Exception ex)
            {
                UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
            }

        }

        #endregion

        #region Events

        void m_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            if (m_timer != null)
            {

                try
                {
                    m_timer.Stop();
                    Controllo();
                    m_timer.Start();
                }
                catch (Exception ex)
                {
                    UnicodeSrl.Framework.Diagnostics.DiagnosticStatics.AddDebugInfo(ex);
                }

            }

        }

        #endregion


    }
}
