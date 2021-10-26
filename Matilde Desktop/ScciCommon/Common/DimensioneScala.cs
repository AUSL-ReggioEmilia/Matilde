using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.Scci
{

    [Serializable()]
    public class DimensioneScala
    {

        private float _min = float.MinValue;
        private float _max = float.MinValue;

        public DimensioneScala()
        {
            _min = float.MinValue;
            _max = float.MinValue;
        }

        public float InizioScala
        {
            get { return _min; }
            set { _min = value; }
        }

        public float FineScala
        {
            get { return _max; }
            set { _max = value; }
        }

    }

}
