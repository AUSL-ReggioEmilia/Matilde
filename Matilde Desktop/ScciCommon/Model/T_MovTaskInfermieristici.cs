using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{
    [ModelTable("T_MovTaskInfermieristici")]
    public class T_MovTaskInfermieristiciBuffer : FwModelBuffer<T_MovTaskInfermieristici>
    {
        public T_MovTaskInfermieristiciBuffer() : base(Database.ConnectionString)
        {
        }

        public static T_MovTaskInfermieristiciBuffer Select(string idGruppo, string codTipoTaskInfermieristico, string orderBy)
        {
            T_MovTaskInfermieristiciBuffer rows = new T_MovTaskInfermieristiciBuffer();
            rows.AddParameter("IDGruppo", idGruppo, System.Data.DbType.String);
            rows.AddParameter("CodTipoTaskInfermieristico", codTipoTaskInfermieristico, System.Data.DbType.String);

            rows.Select(orderBy: orderBy);

            return rows;
        }

        public static T_MovTaskInfermieristiciBuffer Select(string idGruppo, List<string> listCodTask, string orderBy)
        {
            T_MovTaskInfermieristiciBuffer rows = new T_MovTaskInfermieristiciBuffer();

            string whereIN = "";

            foreach (string cod in listCodTask)
            {
                whereIN = whereIN + $"'{cod}', ";
            }

            whereIN = whereIN.TrimEnd();
            whereIN = whereIN.Substring(0, whereIN.Length - 1);

            string where = $"IDGruppo='{idGruppo}' AND CodTipoTaskInfermieristico IN ({whereIN})";

            rows.Select(where: where, orderBy: orderBy);

            return rows;
        }

    }


    [DataContract()]
    [ModelTable("T_MovTaskInfermieristici")]
    public class T_MovTaskInfermieristici : FwModelRow<T_MovTaskInfermieristici>
    {
        public T_MovTaskInfermieristici() : base(Database.ConnectionString)
        {
        }

        [DataMember()]
        [ValidationKey()]
        public Guid ID { get; set; }

        [DataMember()]
        [ValidationRequired()]
        public Int32 IDNum { get; set; }

        [DataMember()]
        public Guid? IDEpisodio { get; set; }

        [DataMember()]
        public Guid? IDTrasferimento { get; set; }

        [DataMember()]
        public DateTime? DataEvento { get; set; }

        [DataMember()]
        public DateTime? DataEventoUTC { get; set; }

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
        public Guid? IDTaskIniziale { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodTipoTaskInfermieristico { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodStatoTaskInfermieristico { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodTipoRegistrazione { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodProtocollo { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodProtocolloTempo { get; set; }

        [DataMember()]
        public DateTime? DataProgrammata { get; set; }

        [DataMember()]
        public DateTime? DataProgrammataUTC { get; set; }

        [DataMember()]
        public DateTime? DataErogazione { get; set; }

        [DataMember()]
        public DateTime? DataErogazioneUTC { get; set; }

        [DataMember()]
        [ValidationStringLenght(512)]
        public String Sottoclasse { get; set; }

        [DataMember()]
        [ValidationStringLenght(4000)]
        public String Note { get; set; }

        [DataMember()]
        [ValidationStringLenght(500)]
        public String DescrizioneFUT { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String CodUtenteRilevazione { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String CodUtenteUltimaModifica { get; set; }

        [DataMember()]
        public DateTime? DataUltimaModifica { get; set; }

        [DataMember()]
        public DateTime? DataUltimaModificaUTC { get; set; }

        [DataMember()]
        [ValidationStringLenght(2000)]
        public String PosologiaEffettiva { get; set; }

        [DataMember()]
        [ValidationStringLenght(2000)]
        public String Alert { get; set; }

        [DataMember()]
        [ValidationStringLenght(100)]
        public String Barcode { get; set; }

    }

    [DataContract()]
    [ModelTable("T_MovTaskInfermieristici")]
    public class T_MovTaskInfermieristici_Keys : FwModelRow<T_MovTaskInfermieristici_Keys>
    {
        public T_MovTaskInfermieristici_Keys() : base(Database.ConnectionString)
        {
        }

        [DataMember()]
        [ValidationKey()]
        public Guid ID { get; set; }


    }

}
