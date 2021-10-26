using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace UnicodeSrl.Scci
{

    public delegate void InattivitaEventHandler(object sender, EventArgs e);

    public class FilterMess : IMessageFilter
    {

        private System.Timers.Timer m_timer = null;
        private bool m_attivato = false;
        public event InattivitaEventHandler InattivitaEvent;

        public FilterMess(double interval)
        {

            m_attivato = false;
            m_timer = new System.Timers.Timer();
            m_timer.Interval = interval;
            m_timer.AutoReset = false;
            m_timer.Elapsed += new System.Timers.ElapsedEventHandler(m_timer_Elapsed);

        }

        public bool PreFilterMessage(ref Message m)
        {

            bool mouse = (m.Msg >= 0x200 & m.Msg <= 0x20d) | (m.Msg >= 0xa0 & m.Msg <= 0xad);

            bool kbd = (m.Msg >= 0x100 & m.Msg <= 0x109);

            if (m_attivato == false)
            {
                if (mouse | kbd)
                {
                    m_timer.Stop();
                }
                else
                {
                    m_timer.Start();
                }
            }

            return false;

        }

        public bool Attivato
        {
            get { return m_attivato; }
            set { m_attivato = value; }
        }

        internal void m_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            if (m_timer != null)
            {

                m_attivato = true;
                m_timer.Stop();

                try
                {

                    if (InattivitaEvent != null)
                    {
                        InattivitaEvent(sender, new EventArgs());
                    }
                    else
                    {
                        MessageBox.Show("Inattivo!!!");
                    }

                }
                catch (Exception ex)
                {
                    Framework.Diagnostics.Statics.AddDebugInfo(ex);
                }

            }

        }

    }

}
