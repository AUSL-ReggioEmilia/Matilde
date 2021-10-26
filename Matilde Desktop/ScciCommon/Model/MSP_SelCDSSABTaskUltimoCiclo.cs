using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;


namespace UnicodeSrl.Scci.Model
{
    [ModelStoredProcedure("MSP_SelCDSSABTaskUltimoCiclo", useXmlParamSelect: true)]
    public class MSP_SelCDSSABTaskUltimoCiclo : FwModelBuffer<MSP_SelCDSSABTaskUltimoCicloData>
    {
        public MSP_SelCDSSABTaskUltimoCiclo() : base(Database.ConnectionString)
        { }

        public static MSP_SelCDSSABTaskUltimoCiclo Select(string iDPrescrizione, string iDPrescrizioneTempi, string codTipoTaskInfermieristico, string codProtocollo, string codUA
            , ScciAmbiente scciAmbiente, EnumAzioni azione, EnumEntita entita)
        {
            MSP_SelCDSSABTaskUltimoCiclo rows = new MSP_SelCDSSABTaskUltimoCiclo();
            rows.AddParameter("IDPrescrizione", iDPrescrizione, System.Data.DbType.String);
            rows.AddParameter("IDPrescrizioneTempi", iDPrescrizioneTempi, System.Data.DbType.String);
            rows.AddParameter("CodTipoTaskInfermieristico", codTipoTaskInfermieristico, System.Data.DbType.String);
            rows.AddParameter("CodProtocollo", codProtocollo, System.Data.DbType.String);
            rows.AddParameter("CodUA", codUA, System.Data.DbType.String);

            TimeStamp ts = new TimeStamp(scciAmbiente);
            ts.CodEntita = entita.ToString();
            ts.CodAzione = azione.ToString();

            string tsString = ts.ToXmlString();
            rows.AddParameter("TimeStamp", tsString, System.Data.DbType.String);

            rows.Select();

            return rows;
        }

    }


    [DataContract()]
    public class MSP_SelCDSSABTaskUltimoCicloData : FwModelRow<MSP_SelCDSSABTaskUltimoCicloData>
    {
        public MSP_SelCDSSABTaskUltimoCicloData() : base(Database.ConnectionString)
        {
        }

        [DataMember()]
        public Guid ID { get; set; }

        [DataMember()]
        public string NGiorno { get; set; }


    }
}
