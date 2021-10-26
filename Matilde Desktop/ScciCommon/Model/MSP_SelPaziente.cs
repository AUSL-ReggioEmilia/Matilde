using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{
    public static partial class FwDataConnectionExt
    {
        public static DataTable MSP_SelPaziente(this FwDataConnection cnn, string idPaziente)
        {
            XmlParameter xp = new XmlParameter();

            xp.AddParameterIfNotEmpty("IDPaziente", idPaziente);

            DataTable result = cnn.Query<DataTable>("MSP_SelPaziente", xp.ToFwDataParametersList(), CommandType.StoredProcedure);

            return result;
        }
    }


    [ModelStoredProcedure("MSP_SelPaziente", useXmlParamSelect: true)]
    public class MSP_SelPazienteBuffer : FwModelBuffer<MSP_SelPaziente>
    {
        public MSP_SelPazienteBuffer() : base(Database.ConnectionString)
        {
        }

        public static MSP_SelPaziente SelectFirst(string idPaziente)
        {

            MSP_SelPazienteBuffer rows = new MSP_SelPazienteBuffer();
            rows.AddParameter("IDPaziente", idPaziente, DbType.String);
            rows.Select();

            return rows.First;
        }

    }


    [DataContract()]
    public class MSP_SelPaziente
    {
        public MSP_SelPaziente()
        {
        }

        [DataMember()]
        [ValidationStringLenght(16)]
        public Guid? IDPaziente { get; set; }

        [DataMember()]
        [ValidationStringLenght(50)]
        public String CodSac { get; set; }

        [DataMember()]
        [ValidationStringLenght(50)]
        public String CodSacFuso { get; set; }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(255)]
        public String Cognome { get; set; }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(255)]
        public String Nome { get; set; }

        [DataMember()]
        [ValidationStringLenght(1)]
        public String Sesso { get; set; }

        [DataMember()]
        [ValidationStringLenght(8)]
        public DateTime? DataNascita { get; set; }

        [DataMember()]
        [ValidationStringLenght(10)]
        public String CodComuneNascita { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String ComuneNascita { get; set; }

        [DataMember()]
        [ValidationStringLenght(10)]
        public String CodProvinciaNascita { get; set; }

        [DataMember()]
        [ValidationStringLenght(50)]
        public String ProvinciaNascita { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodiceFiscale { get; set; }

        [DataMember()]
        [ValidationStringLenght(2147483647)]
        public UniFwByteArray Foto { get; set; }

        [DataMember()]
        [ValidationStringLenght(837)]
        public String DescPaziente { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String IndirizzoResidenza { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String LocalitaResidenza { get; set; }

        [DataMember()]
        [ValidationStringLenght(10)]
        public String CodComuneResidenza { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String ComuneResidenza { get; set; }

        [DataMember()]
        [ValidationStringLenght(50)]
        public String ProvinciaResidenza { get; set; }

        [DataMember()]
        [ValidationStringLenght(50)]
        public String RegioneResidenza { get; set; }

        [DataMember()]
        [ValidationStringLenght(10)]
        public String CAPResidenza { get; set; }

        [DataMember()]
        [ValidationStringLenght(564)]
        public String Residenza { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String IndirizzoDomicilio { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String LocalitaDomicilio { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String ComuneDomicilio { get; set; }

        [DataMember()]
        [ValidationStringLenght(50)]
        public String ProvinciaDomicilio { get; set; }

        [DataMember()]
        [ValidationStringLenght(10)]
        public String CAPDomicilio { get; set; }

        [DataMember()]
        [ValidationStringLenght(564)]
        public String Domicilio { get; set; }

        [DataMember()]
        [ValidationStringLenght(255)]
        public String CognomeNomeMedicoBase { get; set; }

        [DataMember()]
        [ValidationStringLenght(20)]
        public String CodFiscMedicoBase { get; set; }

        [DataMember()]
        [ValidationStringLenght(2147483647)]
        public String ElencoEsenzioni { get; set; }

        [DataMember()]
        [ValidationStringLenght(8)]
        public DateTime? DataDecesso { get; set; }

        [DataMember()]
        [ValidationStringLenght(16)]
        public Guid? IDPazienteFuso { get; set; }

        [DataMember()]
        [ValidationRequired()]
        [ValidationStringLenght(20)]
        public String CodStatoConsensoCalcolato { get; set; }

    }
}
