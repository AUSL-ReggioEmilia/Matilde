using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace UnicodeSrl.ScciEasy
{

    public static class MyStatics
    {

        private static Configurazione m_Configurazione;

        public static Configurazione Configurazione
        {
            get
            {
                if (m_Configurazione == null) m_Configurazione = new Configurazione();
                return m_Configurazione;
            }
            set
            {
                m_Configurazione = value;
            }
        }

    }

}
