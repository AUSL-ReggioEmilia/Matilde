using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;

namespace UnicodeSrl.Scci.Model
{

    [DataContract()]
    [ModelTable("Paziente")]
    public class SS_Sez02 : FwModelRow<SS_Sez02>
    {

        public SS_Sez02() : base()
        {
        }

        [DataMember()]
        public String Sez02 { get; set; }

        [DataMember()]
        public String Descrizione { get; set; }

        [DataMember()]
        public String NumeroScheda { get; set; }

        [DataMember()]
        public String NumeroTotaleSchede { get; set; }

        [DataMember()]
        public String AnteprimaRTF { get; set; }

        [DataMember()]
        public String DataCreazione { get; set; }

        [DataMember()]
        public String DataUltimaModifica { get; set; }

        [DataMember()]
        public String CodUtenteUltimaModifica { get; set; }

        [DataMember()]
        public String DescrUtenteUltimaModifica { get; set; }

        [DataMember()]
        public String DescrUtenteValidazione { get; set; }

        [DataMember()]
        public String DataValidazione { get; set; }

        [DataMember()]
        public Guid? IDScheda { get; set; }

        [DataMember()]
        public Guid? IDSchedaPadre { get; set; }

    }

}
