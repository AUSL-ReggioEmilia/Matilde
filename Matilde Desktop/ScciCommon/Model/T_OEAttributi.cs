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

        public static FwDataBufferedList<T_OEAttributiRow> T_OEAttributi(this FwDataConnection cnn)
        {

            string ssql = "Select * From T_OEAttributi";
            return cnn.T_OEAttributiAll(ssql);

        }
        public static FwDataBufferedList<T_OEAttributiRow> T_OEAttributi(this FwDataConnection cnn, string codentita, string codsistemarichiedente, string codagendarichiedente)
        {

            string ssql = $"Select * From T_OEAttributi " +
                            $"Where CodEntita = '{codentita}' " +
                            $"And CodSistemaRichiedente = '{codsistemarichiedente}' " +
                            $"And CodAgendaRichiedente = '{codagendarichiedente}'";
            return cnn.T_OEAttributiAll(ssql);

        }

        private static FwDataBufferedList<T_OEAttributiRow> T_OEAttributiAll(this FwDataConnection cnn, string sql)
        {
            return cnn.Query<FwDataBufferedList<T_OEAttributiRow>>(sql, null, CommandType.Text); ;
        }

    }

}

[DataContract()]
[ModelTable("T_OEAttributi")]
public class T_OEAttributiRow : FwModelRow<T_OEAttributiRow>
{
    public T_OEAttributiRow()
    {
        this.FwDataConnection = new FwDataConnection(Database.ConnectionString);
    }

    [DataMember()]
    [ValidationKey()]
    public Int32 ID { get; set; }

    [DataMember()]
    [ValidationRequired()]
    [ValidationStringLenght(20)]
    public String CodEntita { get; set; }

    [DataMember()]
    [ValidationRequired()]
    [ValidationStringLenght(20)]
    public String CodSistemaRichiedente { get; set; }

    [DataMember()]
    [ValidationRequired()]
    [ValidationStringLenght(50)]
    public String CodAgendaRichiedente { get; set; }

    [DataMember()]
    public String MappaturaOE { get; set; }

}

