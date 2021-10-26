using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Serialization;

namespace UnicodeSrl.Scci
{

    [Serializable()]
    [XmlRoot(ElementName = "ValoriPVT")]
    public class ValoriPVT
    {

        public ValoriPVT()
        {
            this.Valori = new List<ValorePVT>();
        }

        [XmlElement(ElementName = "ValorePVT")]
        public List<ValorePVT> Valori { get; set; }


    }

}
