using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{
    [ModelStoredProcedure("MSP_SelMovTaskInfermieristiciGruppo", useXmlParamSelect:true )]
    public class MSP_SelMovTaskInfermieristiciGruppo_Buffer : FwModelBuffer<MSP_SelMovTaskInfermieristiciGruppo>
    {
        public static MSP_SelMovTaskInfermieristiciGruppo_Buffer Select(ScciAmbiente scciAmbiente, EnumAzioni codAzione, EnumEntita codEntita, string iDGruppo, string codTipoTaskInfermieristico)
        {
            MSP_SelMovTaskInfermieristiciGruppo_Buffer buffer = new MSP_SelMovTaskInfermieristiciGruppo_Buffer(scciAmbiente);

            TimeStamp ts = new TimeStamp(scciAmbiente);
            ts.CodEntita = codEntita.ToString();
            ts.CodAzione = codAzione.ToString();

            string tsString = ts.ToXmlString();
            buffer.AddParameter("TimeStamp", tsString, System.Data.DbType.String);
            buffer.AddParameter("IDGruppo", iDGruppo, System.Data.DbType.String);
            buffer.AddParameter("CodTipoTaskInfermieristico", codTipoTaskInfermieristico, System.Data.DbType.String);

            buffer.Select();

            return buffer;
        }

        private ScciAmbiente ScciAmbiente { get; set; }

        public MSP_SelMovTaskInfermieristiciGruppo_Buffer(ScciAmbiente scciAmbiente) : base(Database.ConnectionString)
        {
            this.ScciAmbiente = scciAmbiente;
        }

    }

    public class MSP_SelMovTaskInfermieristiciGruppo 
    {
        public MSP_SelMovTaskInfermieristiciGruppo() 
        { }

        public Guid ID { get; set; }
    }


}
