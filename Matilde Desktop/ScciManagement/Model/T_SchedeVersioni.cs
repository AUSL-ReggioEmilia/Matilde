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

namespace UnicodeSrl.ScciManagement.Model
{

    public static partial class FwDataConnectionExt
    {

        public static FwDataBufferedList<T_SchedeVersioniRow> T_SchedeVersioni(this FwDataConnection cnn, string codscheda = "")
        {
            string ssql = "Select * From T_SchedeVersioni" + string.Format(" Where CodScheda = '{0}'", codscheda);
            return cnn.T_SchedeVersioniAll(ssql);
        }
        public static FwDataBufferedList<T_SchedeVersioniRow> T_SchedeVersioniAttive(this FwDataConnection cnn, string codscheda = "")
        {
            string ssql = "Select * From T_SchedeVersioni" + string.Format(" Where CodScheda = '{0}' And FlagAttiva = 1 Order By DtValI desc, DtValF", codscheda);
            return cnn.T_SchedeVersioniAll(ssql);
        }
        private static FwDataBufferedList<T_SchedeVersioniRow> T_SchedeVersioniAll(this FwDataConnection cnn, string sql)
        {
            return cnn.Query<FwDataBufferedList<T_SchedeVersioniRow>>(sql, null, CommandType.Text); ;
        }

    }

    [ModelTable("T_SchedeVersioni")]
    public class T_SchedeVersioniBuffer : FwModelBuffer<T_SchedeVersioniRow>
    {
        public T_SchedeVersioniBuffer() : base(MyStatics.Configurazione.ConnectionString)
        {
        }

    }

    [DataContract()]
    [ModelTable("T_SchedeVersioni")]
    public class T_SchedeVersioniRow : FwModelRow<T_SchedeVersioniRow>
    {
        public T_SchedeVersioniRow() : base(MyStatics.Configurazione.ConnectionString)
        {
            this.FlagAttiva = false;
            this.Pubblicato = false;
        }

        [DataMember()]
        [ValidationKey()]
        [ValidationStringLenght(20)]
        public String CodScheda { get; set; }

        [DataMember()]
        [ValidationKey()]
        public Int32 Versione { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String Descrizione { get; set; }

        [DataMember()]
        public Boolean? FlagAttiva { get; set; }

        [DataMember()]
        public Boolean? Pubblicato { get; set; }

        [DataMember()]
        public DateTime? DtValI { get; set; }

        [DataMember()]
        public DateTime? DtValF { get; set; }

        [DataMember()]
        public String Struttura { get; set; }

        [DataMember()]
        public String Layout { get; set; }

        [DataMember()]
        public String StrutturaV3 { get; set; }

        [DataMember()]
        public String LayoutV3 { get; set; }

    }

}
