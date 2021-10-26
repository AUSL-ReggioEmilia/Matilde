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
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{

    [ModelStoredProcedure("MSP_SelExportDWHCartella", useXmlParamSelect: true)]
    public class MSP_SelExportDWHCartellaBuffer : FwModelBuffer<MSP_SelExportDWHCartella>
    {
        public static MSP_SelExportDWHCartellaBuffer Select(string connString, string idCartella, ScciAmbiente ambiente, EnumEntita tsCodEntita, EnumAzioni tsCodAzione)
        {
            TimeStamp ts = new TimeStamp(ambiente);
            string xmltimeStamp = ts.GetTimeStampXml(tsCodEntita.ToString(), tsCodAzione.ToString());

            MSP_SelExportDWHCartellaBuffer rows = new MSP_SelExportDWHCartellaBuffer();
            rows.SetConnectionString(connString);
            rows.AddParameter("IDCartella", idCartella, DbType.String);
            rows.AddParameter("TimeStamp", xmltimeStamp, DbType.String);

            rows.Select();
            return rows;
        }

        public MSP_SelExportDWHCartellaBuffer() : base(Database.ConnectionString)
        {
        }

    }


    [DataContract()]
    public class MSP_SelExportDWHCartella
    {
        public MSP_SelExportDWHCartella()
        {
        }

        [DataMember()]
        public DateTime DataSequenza { get; set; }

        [DataMember()]
        public Guid IDCartella { get; set; }

        [DataMember()]
        public String NumeroCartella { get; set; }

        [DataMember()]
        public String AziendaErogante { get; set; }

        [DataMember()]
        public String SistemaErogante { get; set; }

        [DataMember()]
        public String RepartoEroganteCodice { get; set; }

        [DataMember()]
        public String RepartoEroganteDescrizione { get; set; }

        [DataMember()]
        public String PazienteIDEsterno { get; set; }

        [DataMember()]
        public String PazienteCognome { get; set; }

        [DataMember()]
        public String PazienteNome { get; set; }

        [DataMember()]
        public DateTime? PazienteDataNascita { get; set; }

        [DataMember()]
        public String PazienteSesso { get; set; }

        [DataMember()]
        public String PazienteCodiceFiscale { get; set; }

        [DataMember()]
        public DateTime? DataReferto { get; set; }

        [DataMember()]
        public Guid IDReferto { get; set; }

        [DataMember()]
        public String NumeroNosologico { get; set; }

        [DataMember()]
        public UniFwByteArray PDFCartella { get; set; }

        [DataMember()]
        public String BarcodeCartella { get; set; }

    }

}
