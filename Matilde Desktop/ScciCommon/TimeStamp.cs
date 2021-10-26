using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using UnicodeSrl.Framework.Serialization;
using UnicodeSrl.Scci.DataContracts;

namespace UnicodeSrl.Scci
{
    [Serializable()]
    [DataContract(Name = "TimeStamp", Namespace = "")]
    public class TimeStamp : FwSerializableContract
    {
        public static String GetTimeStampXml(ScciAmbiente scciAmbiente, string codEntita, string codAzione, string idEntita = null)
        {
            TimeStamp time = new TimeStamp(scciAmbiente);
            time.IDEntita = idEntita;
            return time.GetTimeStampXml(codEntita, codAzione);
        }

        public TimeStamp()
        {

        }

        public TimeStamp(ScciAmbiente env)
        {
            if (env != null)
            {
                this.CodLogin = env.Codlogin;
                this.CodRuolo = env.Codruolo;

                this.NomePC = env.Nomepc;
                this.IndirizzoIP = env.Indirizzoip;

                this.IDPaziente = env.Idpaziente;

                this.IDEpisodio = env.Idepisodio;

                this.IDTrasferimento = env.IdTrasferimento;
            }
        }

        public String GetTimeStampXml(string codEntita, string codAzione)
        {
            this.CodEntita = codEntita;
            this.CodAzione = codAzione;

            string xmlString = this.ToXmlString();

            XDocument doc = XDocument.Parse(xmlString);
            XElement root = doc.Root;

            foreach (XAttribute attr in root.Attributes())
            {
                attr.Remove();
            }

            return doc.ToString();
        }

        [DataMember]
        public string CodLogin { get; set; }

        [DataMember]
        public string CodRuolo { get; set; }

        [DataMember]
        public string NomePC { get; set; }

        [DataMember]
        public string IndirizzoIP { get; set; }

        [DataMember]
        public string CodEntita { get; set; }

        [DataMember]
        public string IDEntita { get; set; }

        [DataMember]
        public string CodAzione { get; set; }

        [DataMember]
        public string IDPaziente { get; set; }

        [DataMember]
        public string IDEpisodio { get; set; }

        [DataMember]
        public string IDTrasferimento { get; set; }

    }
}
