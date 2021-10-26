using System;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data.CustomAttributes;


namespace UnicodeSrl.Scci.Model
{
    [DataContract]
    public class SimpleViewSelMovScheda
    {
        public String CodEntita { get; set; }

        [DataMember]
        [ValidationKey]
        public Guid ID { get; set; }

        [DataMember]
        public int IDNum { get; set; }

        [DataMember]
        public Guid? IDPaziente { get; set; }

        [DataMember]
        public Guid? IDEpisodio { get; set; }

        [DataMember]
        public Guid? IDTrasferimento { get; set; }

        [DataMember]
        public String CodScheda { get; set; }

        [DataMember]
        public String Descrizione { get; set; }

        [DataMember]
        public String AnteprimaRTF { get; set; }

        [DataMember]
        public DateTime? DataCreazione { get; set; }

        [DataMember]
        public DateTime? DataUltimaModifica { get; set; }

        [DataFieldIgnore]
        public DateTime DataUpdate
        {
            get
            {
                if (this.DataUltimaModifica != null)
                    return this.DataUltimaModifica.Value;
                else
                    return this.DataCreazione.Value;
            }

        }

        public String CodUtenteRilevazione { get; set; }
        public String CodUtenteUltimaModifica { get; set; }
        public String DescrUtenteCreazione { get; set; }
        public String DescrUtenteUltimaModifica { get; set; }


        public String UtenteUpdate
        {
            get
            {
                if ((this.DescrUtenteUltimaModifica != null) && (this.DescrUtenteUltimaModifica != ""))
                    return this.DescrUtenteUltimaModifica;
                else
                    return this.DescrUtenteCreazione;
            }
        }


    }

    [DataContract]
    public class SimpleViewDatiMancanti
    {

        [DataMember]
        [ValidationKey]
        public Guid ID { get; set; }


        [DataMember]
        public String Descrizione { get; set; }

        [DataMember]
        public String AnteprimaRTF { get; set; }


        [DataMember]
        public String DatiObbligatoriMancantiRTF { get; set; }




    }
}
