using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.Scci
{

    [Serializable()]
    public class DimensionePerGrafico
    {

        #region Declare

        private string _nome = string.Empty;
        private DimensioneScala _scala = new DimensioneScala();
        private DatoClinico _datoclinico = new DatoClinico();

        #endregion

        #region Constructor

        public DimensionePerGrafico()
        {
        }

        #endregion

        #region Property

        public string Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }

        public DimensioneScala Scala
        {
            get
            {
                if (_scala == null) _scala = new DimensioneScala();
                return _scala;
            }
            set { _scala = value; }
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

        #endregion
    }

}
