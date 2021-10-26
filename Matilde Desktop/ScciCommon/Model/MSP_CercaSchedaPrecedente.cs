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
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{

    public static partial class FwDataConnectionExt
    {

        public static FwDataBufferedList<MSP_CercaSchedaPrecedente> MSP_CercaSchedaPrecedente(this FwDataConnection cnn, string codScheda, string idPaziente, ScciAmbiente ambiente)
        {
            FwDataBufferedList<MSP_CercaSchedaPrecedente> result = null;

            XmlParameter xp = new XmlParameter();

            xp.AddParameterIfNotEmpty("CodScheda", codScheda);
            xp.AddParameterIfNotEmpty("IgnoraVersione", "1");
            xp.AddParameterIfNotEmpty("IDPaziente", idPaziente);

            TimeStamp ts = new TimeStamp(ambiente);
            string xmlString = ts.GetTimeStampXml("SCH", "VIS");

            xp.AddParameter("TimeStamp", xmlString);

            result = cnn.Query<FwDataBufferedList<MSP_CercaSchedaPrecedente>>("MSP_CercaSchedaPrecedente", xp.ToFwDataParametersList(), CommandType.StoredProcedure);

            return result;
        }


    }

    [DataContract]
    public class MSP_CercaSchedaPrecedente
        : FwModelRow<MSP_ControlloPermessiSchedeRow>
    {
        public MSP_CercaSchedaPrecedente()
        {
            this.FwDataConnection = new FwDataConnection(Database.ConnectionString);
            this.SelectedForCopy = false;
        }

        [DataFieldIgnore]
        public bool SelectedForCopy { get; set; }


        [DataMember]
        public Guid IDScheda { get; set; }

        [DataMember]
        public DateTime DataRiferimento { get; set; }

        [DataMember]
        public String CodTipoScheda { get; set; }

        [DataMember]
        public String DescrTipoScheda
        { get; set; }

        [DataMember]
        public String CodScheda { get; set; }

        [DataMember]
        public String DescrScheda
        { get; set; }

        [DataMember]
        public DateTime DataCreazione
        { get; set; }

        [DataMember]
        public String CodUtenteRilevazione
        { get; set; }

        [DataMember]
        public String DescrUtenteRilevazione
        { get; set; }

        [DataMember]
        public DateTime DataUltimaModifica
        { get; set; }

        [DataMember]
        public String CodUtenteUltimaModifica
        { get; set; }

        [DataMember]
        public String DescrUltimaModifica
        { get; set; }

        [DataMember]
        public String AnteprimaRTF
        { get; set; }

    }
}
