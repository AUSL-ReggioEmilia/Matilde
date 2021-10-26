using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Scci.Statics;



namespace UnicodeSrl.Scci.Model
{
    public enum en_TipoScreen
    {
        EPICAR = 0,
        EPIGRID = 1
    }



    [DataContract]
    [ModelTable("T_TipoScreen")]
    public class T_TipoScreenRow : FwModelRow<T_TipoScreenRow>
    {
        [DataMember]
        [ValidationStringLenght(20)]
        [ValidationKey]
        public String Codice { get; set; }

        [DataMember]
        [ValidationStringLenght(255)]
        public String Descrizione { get; set; }
    }
}
