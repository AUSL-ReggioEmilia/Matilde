using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Framework.Types;
using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{

    public class MSP_AggMovPrescrizioniTempi
    {
        public MSP_AggMovPrescrizioniTempi()
        { }

        public static void Execute(string codStatoPrescrizioneTempi, string iDPrescrizioneTempi
            , ScciAmbiente scciAmbiente, EnumAzioni azione, EnumEntita entita)
        {
            XmlParameter xmlParameter = new XmlParameter();

            xmlParameter.AddParameter("IDPrescrizioneTempi", iDPrescrizioneTempi);
            xmlParameter.AddParameter("CodStatoPrescrizioneTempi", codStatoPrescrizioneTempi);


            TimeStamp ts = new TimeStamp(scciAmbiente);
            ts.CodEntita = entita.ToString();
            ts.CodAzione = azione.ToString();

            string tsString = ts.ToXmlString();
            xmlParameter.AddParameter("TimeStamp", tsString);

            using (FwDataConnection fdc = new FwDataConnection(Database.ConnectionString))
            {
                FwDataParametersList plist = xmlParameter.ToFwDataParametersList();
                fdc.ExecuteStored("MSP_AggMovPrescrizioniTempi", ref plist);
            }

        }

    }
}
