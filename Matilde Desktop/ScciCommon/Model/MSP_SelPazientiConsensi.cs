using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;

namespace UnicodeSrl.Scci.Model
{
    [ModelStoredProcedure("MSP_SelPazientiConsensi", useXmlParamSelect: false)]
    public class MSP_SelPazientiConsensiBuffer : FwModelBuffer<MSP_SelPazientiConsensi>
    {
        public static MSP_SelPazientiConsensiBuffer Select(string connString, string idPaziente, string codTipoConsenso)
        {
            XmlParameter xp = new XmlParameter();

            xp.AddParameterIfNotEmpty("IDPaziente", idPaziente);
            xp.AddParameterIfNotEmpty("CodTipoConsenso", codTipoConsenso);

            var param = xp.ToFwDataParametersList();

            MSP_SelPazientiConsensiBuffer data = new MSP_SelPazientiConsensiBuffer(connString);
            data.Parameters = param;
            data.Select();

            return data;
        }

        public MSP_SelPazientiConsensiBuffer(string connString) : base(connString)
        {
        }

    }


    [DataContract()]
    public class MSP_SelPazientiConsensi
    {
        public MSP_SelPazientiConsensi()
        {
        }

        [DataMember()]
        public Guid IDPaziente { get; set; }

        [DataMember()]
        public String CodTipoConsenso { get; set; }

        [DataMember()]
        public String CodSistemaProvenienza { get; set; }

        [DataMember()]
        public String IDProvenienza { get; set; }

        [DataMember()]
        public String CodStatoConsenso { get; set; }

        [DataMember()]
        public DateTime? DataConsenso { get; set; }

        [DataMember()]
        public DateTime? DataDisattivazione { get; set; }

        [DataMember()]
        public String CodOperatore { get; set; }

        [DataMember()]
        public String CognomeOperatore { get; set; }

        [DataMember()]
        public String NomeOperatore { get; set; }

        [DataMember()]
        public String ComputerOperatore { get; set; }

    }
}
