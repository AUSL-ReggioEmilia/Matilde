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

    [ModelTable(@"T_MovTrasferimenti")]
    public class T_MovTrasferimentiBuffer : FwModelBuffer<T_MovTrasferimenti>
    {
        public T_MovTrasferimentiBuffer() : base(Database.ConnectionString)
        {
        }

    }

    [DataContract()]
    [ModelTable(@"T_MovTrasferimenti")]
    public class T_MovTrasferimenti : FwModelRow<T_MovTrasferimenti>
    {
        public T_MovTrasferimenti() : base(Database.ConnectionString)
        {
        }

        [DataMember()]
        [ValidationKey()]
        public Guid ID { get; set; }

        [DataMember()]
        [ValidationRequired()]
        [DataFieldIdentity()]
        public Int32 IDNum { get; set; }

        [DataMember()]
        public Guid? IDEpisodio { get; set; }

        [DataMember()]
        public Decimal? Sequenza { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodUA { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodStatoTrasferimento { get; set; }

        [DataMember()]
        public DateTime? DataIngresso { get; set; }

        [DataMember()]
        public DateTime? DataIngressoUTC { get; set; }

        [DataMember()]
        public DateTime? DataUscita { get; set; }

        [DataMember()]
        public DateTime? DataUscitaUTC { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodUO { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String DescrUO { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodSettore { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String DescrSettore { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodStanza { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String DescrStanza { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodLetto { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String DescrLetto { get; set; }

        [DataMember()]
        public Guid? IDCartella { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodAziTrasferimento { get; set; }

    }

    public class T_MovTrasferimentiEx : T_MovTrasferimenti
    {

        public T_MovTrasferimentiEx()
        {
        }

        [DataFieldIgnore()]
        public int Link { get; set; }

    }

}
