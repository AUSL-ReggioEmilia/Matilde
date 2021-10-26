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
    [ModelTable("PrescrizioniFarmaci")]
    public class SF_Sez02 : FwModelRow<SF_Sez02>
    {
        public SF_Sez02() : base()
        {
        }

        [DataMember()]
        public String Sez02 { get; set; }

        [DataMember()]
        public String DataValidazione { get; set; }

        [DataMember()]
        public String DataSospensione { get; set; }

        [DataMember()]
        public String TipoPrescrizione { get; set; }

        [DataMember()]
        public String UtenteValidatore { get; set; }

        [DataMember()]
        public String AnteprimaRTF { get; set; }

        [DataMember()]
        public String DataSomministrazione { get; set; }

        [DataMember()]
        public String Posologia { get; set; }

        [DataMember()]
        public String ViaSomministrazione { get; set; }

    }

}
