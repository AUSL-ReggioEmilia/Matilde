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
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{
    public static partial class FwDataConnectionExt
    {
        public static FwDataBufferedList<MSP_ControlloPermessiSchedeRow> MSP_ControlloPermessiSchede(this FwDataConnection cnn, string codRuolo, string codUA, List<String> schede)
        {
            FwDataBufferedList<MSP_ControlloPermessiSchedeRow> result = null;

            XmlParameter xp = new XmlParameter();

            xp.AddParameterIfNotEmpty("CodRuolo", codRuolo);
            xp.AddParameterIfNotEmpty("CodUA", codUA);

            xp.AddParameter("IDScheda", schede.ToArray());

            result = cnn.Query<FwDataBufferedList<MSP_ControlloPermessiSchedeRow>>("MSP_ControlloPermessiSchede", xp.ToFwDataParametersList(), CommandType.StoredProcedure);

            return result;
        }

        public static FwDataBufferedList<MSP_ControlloPermessiSchedeRow> MSP_ControlloPermessiSchede(this FwDataConnection cnn, string codRuolo, string codUA, string scheda)
        {
            FwDataBufferedList<MSP_ControlloPermessiSchedeRow> result = null;

            XmlParameter xp = new XmlParameter();

            xp.AddParameterIfNotEmpty("CodRuolo", codRuolo);
            xp.AddParameterIfNotEmpty("CodUA", codUA);

            xp.AddParameterIfNotEmpty("IDScheda", scheda);

            result = cnn.Query<FwDataBufferedList<MSP_ControlloPermessiSchedeRow>>("MSP_ControlloPermessiSchede", xp.ToFwDataParametersList(), CommandType.StoredProcedure);

            return result;
        }
    }

    [DataContract]
    public class MSP_ControlloPermessiSchedeRow
        : FwModelRow<MSP_ControlloPermessiSchedeRow>
    {
        public MSP_ControlloPermessiSchedeRow()
        {
            this.FwDataConnection = new FwDataConnection(Database.ConnectionString);
        }

        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public int PermessoModifica { get; set; }

        [DataMember]
        public int PermessoCancella { get; set; }

    }
}
