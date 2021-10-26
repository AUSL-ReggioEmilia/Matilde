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
using UnicodeSrl.Framework.Types;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{
    [DataContract()]
    [ModelStoredProcedure("MSP_SelCDSSABDatiPaziente")]
    public class MSP_SelCDSSABDatiPaziente : FwModelRow<MSP_SelCDSSABDatiPaziente>
    {
        public static MSP_SelCDSSABDatiPaziente Select(Guid idEpisodio)
        {
            MSP_SelCDSSABDatiPaziente row = new MSP_SelCDSSABDatiPaziente();
            row.IDEpisodio = idEpisodio;
            bool exist = row.TrySelect();

            if (exist) return row;
            else return null;
        }

        public MSP_SelCDSSABDatiPaziente() : base(Database.ConnectionString)
        {
        }

        [DataFieldParam("idEpisodio", SqlDbType.UniqueIdentifier, ParameterDirection.Input, useOnSelect: true)]
        [DataFieldIgnore]
        public Guid IDEpisodio { get; set; }

        [DataMember()]
        public String Cognome { get; set; }

        [DataMember()]
        public String Nome { get; set; }

        [DataMember()]
        public DateTime? DataNascita { get; set; }

        [DataMember()]
        public String Sesso { get; set; }

        [DataMember()]
        public String CodiceFiscale { get; set; }

        [DataMember()]
        public String UltimoCodSAC { get; set; }

        [DataMember()]
        public String NumeroNosologico { get; set; }

        [DataMember()]
        public String CodAzi { get; set; }

    }
}
