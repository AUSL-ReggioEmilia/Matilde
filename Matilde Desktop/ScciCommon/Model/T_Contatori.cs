using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{

    public static partial class FwDataConnectionExt
    {

        public static FwDataBufferedList<T_Contatori> T_ContatoriScaduti(this FwDataConnection cnn, DateTime datascadenza)
        {
            string sql = $"Select * From T_Contatori Where DataScadenza < {Database.dataOraSQL(datascadenza)}";
            return cnn.Query<FwDataBufferedList<T_Contatori>>(sql, null, CommandType.Text);
        }

    }

    [ModelTable("T_Contatori")]
    public class T_ContatoriBuffer : FwModelBuffer<T_Contatori>
    {
        public T_ContatoriBuffer() : base(Database.ConnectionString)
        {
        }

    }

    [DataContract()]
    [ModelTable("T_Contatori")]
    public class T_Contatori : FwModelRow<T_Contatori>
    {
        public T_Contatori() : base(Database.ConnectionString)
        {
        }

        [DataMember()]
        [ValidationKey()]
        [ValidationStringLenght(20)]
        public String Codice { get; set; }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(200)]
        public String Descrizione { get; set; }

        [DataMember()]
        public Int64? Valore { get; set; }

        [DataMember()]
        public DateTime? DataImpostazione { get; set; }

        [DataMember()]
        public DateTime? DataImpostazioneUTC { get; set; }

        [DataMember()]
        public DateTime? DataUltimaModifica { get; set; }

        [DataMember()]
        public DateTime? DataUltimaModificaUTC { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String CodUtenteImpostazione { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String CodUtenteUltimaModifica { get; set; }

        [DataMember()]
        public DateTime? DataScadenza { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodUnitaScadenza { get; set; }

        [DataMember()]
        public Boolean? Sistema { get; set; }

    }
}
