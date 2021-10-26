using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{

    public static partial class FwDataConnectionExt
    {

        public static FwDataBufferedList<T_MovOrdiniRicevutiRow> T_MovOrdiniRicevuti(this FwDataConnection cnn, string idordineoe)
        {
            string sql = $"Select * From T_MovOrdiniRicevuti Where IDOrdineOE = '{idordineoe}'";
            return cnn.Query<FwDataBufferedList<T_MovOrdiniRicevutiRow>>(sql, null, CommandType.Text);
        }

    }

    [DataContract()]
    [ModelTable("T_MovOrdiniRicevuti")]
    public class T_MovOrdiniRicevutiRow : FwModelRow<T_MovOrdiniRicevutiRow>
    {

        public T_MovOrdiniRicevutiRow()
        {
            this.FwDataConnection = new FwDataConnection(Database.ConnectionString);
        }

        [DataMember()]
        [ValidationKey()]
        public Guid? ID { get; set; }

        [DataFieldIgnore]
        public Int32 IDNum { get; set; }

        [DataMember()]
        [ValidationStringLenght(50)]
        public String IDOrdineOE { get; set; }

        [DataMember()]
        [ValidationRequired()]
        public String DatiOE { get; set; }

        [DataMember()]
        public DateTime? DataInserimento { get; set; }

        [DataMember()]
        public DateTime? DataInserimentoUTC { get; set; }

        [DataMember()]
        public DateTime? DataUltimaModifica { get; set; }

        [DataMember()]
        public DateTime? DataUltimaModificaUTC { get; set; }

    }

}
