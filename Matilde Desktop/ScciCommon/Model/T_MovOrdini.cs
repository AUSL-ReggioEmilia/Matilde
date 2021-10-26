using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Scci.Statics;

namespace ScciCommon.Model
{

    public static partial class FwDataConnectionExt
    {

        public static FwDataBufferedList<T_MovOrdiniRow> T_MovOrdiniRow(this FwDataConnection cnn, string id)
        {

            string ssql = $"Select * From T_MovOrdini\n" +
                            $"Where ID = '{id}'";

            return cnn.T_MovOrdiniAll(ssql);

        }

        public static FwDataBufferedList<T_MovOrdiniRow> T_MovOrdiniRow(this FwDataConnection cnn, string codsistema, string idsistema)
        {

            string ssql = $"Select * From T_MovOrdini\n" +
                            $"Where CodSistema = '{codsistema}'\n" +
                            $"And IDSistema = '{idsistema}'";

            return cnn.T_MovOrdiniAll(ssql);

        }

        private static FwDataBufferedList<T_MovOrdiniRow> T_MovOrdiniAll(this FwDataConnection cnn, string sql)
        {
            return cnn.Query<FwDataBufferedList<T_MovOrdiniRow>>(sql, null, CommandType.Text); ;
        }

    }

    [ModelTable("T_MovOrdini")]
    public class T_MovOrdiniBuffer : FwModelBuffer<T_MovOrdiniRow>
    {

        public T_MovOrdiniBuffer() : base(Database.ConnectionString)
        {
        }

    }

    [DataContract()]
    [ModelTable("T_MovOrdini")]
    public class T_MovOrdiniRow : FwModelRow<T_MovOrdiniRow>
    {

        public T_MovOrdiniRow() : base(Database.ConnectionString)
        {
        }

        [DataMember()]
        [ValidationKey()]
        public Guid ID { get; set; }

        [DataMember()]
        [ValidationRequired()]
        public Int32 IDNum { get; set; }

        [DataMember()]
        public Guid? IDPaziente { get; set; }

        [DataMember()]
        public Guid? IDEpisodio { get; set; }

        [DataMember()]
        public Guid? IDTrasferimento { get; set; }

        [DataMember()]
        [ValidationStringLenght(50)]
        public String IDOrdineOE { get; set; }

        [DataMember()]
        [ValidationStringLenght(50)]
        public String NumeroOrdineOE { get; set; }

        [DataMember()]
        public String XMLOE { get; set; }

        [DataMember()]
        public String Eroganti { get; set; }

        [DataMember()]
        public String Prestazioni { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodStatoOrdine { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String CodUtenteInserimento { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String CodUtenteUltimaModifica { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String CodUtenteInoltro { get; set; }

        [DataMember()]
        public DateTime? DataProgrammazioneOE { get; set; }

        [DataMember()]
        public DateTime? DataProgrammazioneOEUTC { get; set; }

        [DataMember()]
        public DateTime? DataInserimento { get; set; }

        [DataMember()]
        public DateTime? DataInserimentoUTC { get; set; }

        [DataMember()]
        public DateTime? DataUltimaModifica { get; set; }

        [DataMember()]
        public DateTime? DataUltimaModificaUTC { get; set; }

        [DataMember()]
        public DateTime? DataInoltro { get; set; }

        [DataMember()]
        public DateTime? DataInoltroUTC { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodUAInserimento { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodUAUltimaModifica { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodPriorita { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodSistema { get; set; }

        [DataMember()]
        [ValidationStringLenght(50)]
        public String IDSistema { get; set; }

        [DataMember()]
        [ValidationStringLenght(50)]
        public String IDGruppo { get; set; }

        [DataMember()]
        [ValidationStringLenght(50)]
        public String InfoSistema { get; set; }

        [DataMember()]
        public String StrutturaDatiAccessori { get; set; }

        [DataMember()]
        public String DatiDatiAccessori { get; set; }

        [DataMember()]
        public String LayoutDatiAccessori { get; set; }

        [DataMember()]
        [ValidationStringLenght(50)]
        public String InfoSistema2 { get; set; }

    }

}
