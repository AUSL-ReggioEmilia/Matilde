using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Scci.Statics;

namespace ScciCommon.Model
{

    [ModelTable("T_AssRuoliAzioni")]
    public class T_AssRuoliAzioniBuffer : FwModelBuffer<T_AssRuoliAzioni>
    {
        public T_AssRuoliAzioniBuffer() : base(Database.ConnectionString)
        {
        }

    }

    [DataContract()]
    [ModelTable("T_AssRuoliAzioni")]
    public class T_AssRuoliAzioni : FwModelRow<T_AssRuoliAzioni>
    {
        public T_AssRuoliAzioni() : base(Database.ConnectionString)
        {
        }

        [DataMember()]
        [ValidationKey()]
        [ValidationStringLenght(20)]
        public String CodRuolo { get; set; }

        [DataMember()]
        [ValidationKey()]
        [ValidationStringLenght(20)]
        public String CodEntita { get; set; }

        [DataMember()]
        [ValidationKey()]
        [ValidationStringLenght(20)]
        public String CodVoce { get; set; }

        [DataMember()]
        [ValidationKey()]
        [ValidationStringLenght(20)]
        public String CodAzione { get; set; }

        [DataMember()]
        public String Parametri { get; set; }

        [DataFieldIgnore]
        public ParametriCIV ParametriCIV
        {
            get
            {
                if (String.IsNullOrEmpty(this.Parametri) == false)
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<ParametriCIV>(this.Parametri);
                else
                    return new ParametriCIV();
            }
        }

    }

    public class ParametriCIV
    {

        public int GGCartelleInVisione { get; set; }

    }
}
