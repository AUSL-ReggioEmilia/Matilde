using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.Scci
{

    [Serializable()]
    public class ValorePVT
    {

        private string _nome = string.Empty;
        private string _valore = string.Empty;

        public ValorePVT()
        {
        }

        public string Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }

        public string Valore
        {
            get
            {
                return _valore;
            }
            set { _valore = value; }
        }

    }

}
