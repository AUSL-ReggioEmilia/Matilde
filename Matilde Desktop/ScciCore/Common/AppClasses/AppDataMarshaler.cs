using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using UnicodeSrl.Framework.Serialization;
using UnicodeSrl.Scci;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Model;

namespace UnicodeSrl.ScciCore
{

    [DataContract]
    [KnownType(typeof(AppDataMarshaler))]
    [Serializable]
    public class AppDataMarshaler
        : FwSerializableContract
    {
        [DataMember]
        public string CodRuolo { get; set; }

        [DataMember]
        public string CodUA { get; set; }

        [DataMember]
        public string CodUA_Ambulatoriale { get; set; }

        [DataMember]
        public string CodUO { get; set; }

        public bool CartellaChiusa { get; set; }

        public string NumeroCartella { get; set; }

        public string CodStatoCartella { get; set; }

        public string DecrStatoCartella { get; set; }

        public string CodStatoTrasferimento { get; set; }

        [DataMember]
        public ScciAmbiente ScciAmbiente { get; set; }

        [DataMember]
        public MSP_CercaEpisodioRow DatiCercaEpisodio { get; set; }

        public TimeStamp GetTimeStamp(string codEntita, string codAzione)
        {
            TimeStamp ts = new TimeStamp(this.ScciAmbiente);
            ts.CodEntita = codEntita;
            ts.CodAzione = codAzione;

            return ts;
        }

        public String GetTimeStampXml(string codEntita, string codAzione)
        {
            TimeStamp ts = this.GetTimeStamp(codEntita, codAzione);

            string xmlString = ts.ToXmlString();


            XDocument doc = XDocument.Parse(xmlString);
            XElement root = doc.Root;

            foreach (XAttribute attr in root.Attributes())
            {
                attr.Remove();
            }

            return doc.ToString();
        }



    }
}
