using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;

namespace UnicodeSrl.ScciManagement.Model
{
    [ModelStoredProcedure("MSP_SchedeVersioni_Select")]
    public class MSP_SchedeVersioni_Select : FwModelBuffer<MSP_SchedeVersioni>
    {
        #region     Factory

                                                                public static MSP_SchedeVersioni Select(string connString, string codScheda, int? versione = null, DateTime? dataVal = null)
        {
            MSP_SchedeVersioni_Select data = new MSP_SchedeVersioni_Select(conn: connString);
            data.AddParameter(@"@codScheda", codScheda, DbType.String);
            if (versione != null) data.AddParameter(@"@versione", versione.Value, DbType.Int32);
            if (dataVal != null) data.AddParameter(@"@dataVal", dataVal.Value, DbType.DateTime);

            data.Select();
            return data.First;
        }

        #endregion  Factory


        public MSP_SchedeVersioni_Select() : base()
        {
        }

        public MSP_SchedeVersioni_Select(string conn) : base(conn)
        {
        }
    }


    [DataContract()]
    public class MSP_SchedeVersioni
    {
        public MSP_SchedeVersioni()
        {
        }

        [DataMember()]
        public String CodScheda { get; set; }

        [DataMember()]
        public Int32 Versione { get; set; }

        [DataMember()]
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
