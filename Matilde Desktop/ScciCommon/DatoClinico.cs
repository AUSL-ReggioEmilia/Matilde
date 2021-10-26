using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.Scci
{
    [Serializable()]
    public class DatoClinico
    {

        private string _Riga = string.Empty;
        private string _Sequenza = string.Empty;
        private string _Campo = string.Empty;

        public DatoClinico()
        {
        }

        public string Riga
        {
            get { return _Riga; }
            set { _Riga = value; }
        }

        public string Sequenza
        {
            get { return _Sequenza; }
            set { _Sequenza = value; }
        }

        public string Campo
        {
            get { return _Campo; }
            set { _Campo = value; }
        }

    }
}
