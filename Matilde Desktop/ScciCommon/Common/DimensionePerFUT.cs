using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.Scci
{

    [Serializable()]
    public class DimensionePerFUT
    {

        private string _nome = string.Empty;
        private DatoClinico _datoclinico = new DatoClinico();

        public DimensionePerFUT()
        {
        }

        public string Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }

        public DatoClinico DatoClinico
        {
            get
            {
                if (_datoclinico == null) _datoclinico = new DatoClinico();
                return _datoclinico;
            }
            set { _datoclinico = value; }
        }

    }

}
