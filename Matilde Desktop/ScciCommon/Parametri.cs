using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci
{
    [Serializable()]
    public class Parametri : IXmlSerializable
    {

        private TimeStamp _TimeStamp = null;
        private MovScheda _MovScheda = null;

        public Parametri()
        {
            Parametro = new Dictionary<string, string>();
            ParametroRipetibile = new Dictionary<string, string[]>();
            _TimeStamp = new TimeStamp(new DataContracts.ScciAmbiente());
            _MovScheda = null;
        }

        public Parametri(DataContracts.ScciAmbiente env)
        {
            Parametro = new Dictionary<string, string>();
            ParametroRipetibile = new Dictionary<string, string[]>();
            _TimeStamp = new TimeStamp(env);
            _MovScheda = null;
        }

        public string ToXmlString()
        {
            Parametri op = this;
            return XmlProcs.XmlSerializeToString(op);
        }

        public Dictionary<string, string> Parametro { get; set; }

        public Dictionary<string, string[]> ParametroRipetibile { get; set; }

        public TimeStamp TimeStamp
        {
            get
            {
                return _TimeStamp;
            }
            set
            {
                _TimeStamp = value;
            }
        }

        public MovScheda MovScheda
        {
            get
            {
                return _MovScheda;
            }
            set
            {
                _MovScheda = value;
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {

        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {

            foreach (var pair in Parametro)
            {
                writer.WriteElementString(pair.Key, pair.Value);
            }

            foreach (var pairripetibile in ParametroRipetibile)
            {

                foreach (string valore in pairripetibile.Value)
                {
                    writer.WriteElementString(pairripetibile.Key, valore);
                }

            }

            if (_MovScheda != null)
            {

                writer.WriteStartElement("SchedaMovimento");

                writer.WriteElementString("CodUA", _MovScheda.CodUA);
                writer.WriteElementString("CodScheda", _MovScheda.CodScheda);
                writer.WriteElementString("Versione", _MovScheda.Versione.ToString());
                writer.WriteElementString("Dati", _MovScheda.DatiXML);
                writer.WriteElementString("AnteprimaRTF", _MovScheda.AnteprimaRTFxParamStored);
                writer.WriteElementString("AnteprimaTXT", _MovScheda.AnteprimaTXTxParamStored);

                writer.WriteEndElement();

            }

            var serializer = new XmlSerializer(_TimeStamp.GetType());
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            serializer.Serialize(writer, _TimeStamp, namespaces);

        }

    }
}
