using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using UnicodeSrl.DatiClinici.DC;
using UnicodeSrl.DatiClinici.Gestore;
using UnicodeSrl.Framework.Data;
using UnicodeSrl.Framework.Data.CustomAttributes;
using UnicodeSrl.Framework.Data.Model;
using UnicodeSrl.Scci.Model.Strutture;
using UnicodeSrl.Scci.Statics;

namespace UnicodeSrl.Scci.Model
{

    public class MSP_SelIndicatori
    {
        public static MSP_SelIndicatori Select(string codUtente, string codRuolo, string idPaziente = null, string idEpisodio = null, string idCartella = null)
        {
            MSP_SelIndicatori data = new MSP_SelIndicatori();

            XmlParameter xmlParameter = new XmlParameter();
            xmlParameter.AddParameter("CodUtente", codUtente);
            xmlParameter.AddParameter("CodRuolo", codRuolo);
            xmlParameter.AddParameterIfNotEmpty("IDPaziente", idPaziente);
            xmlParameter.AddParameterIfNotEmpty("IDEpisodio", idEpisodio);
            xmlParameter.AddParameterIfNotEmpty("IDCartella", idCartella);

            using (FwDataConnection fdc = new FwDataConnection(Database.ConnectionString))
            {
                FwDataParametersList plist = xmlParameter.ToFwDataParametersList();
                data = fdc.QueryFirst<MSP_SelIndicatori>("MSP_SelIndicatori", plist, CommandType.StoredProcedure);
            }

            return data;
        }

        public Int32 DiarioClinico { get; set; }
        public Int32 Allergie { get; set; }
        public Int32 Alert { get; set; }
        public Int32 EvidenzaClinica { get; set; }
        public Int32 Segnalibri { get; set; }
        public Int32 CartelleInVisione { get; set; }
        public Int32 PazientiInVisione { get; set; }
        public Int32 PazientiSeguiti { get; set; }
        public Int32 PazienteSeguito { get; set; }
        public Int32 PazientiSeguitiDaAltri { get; set; }
        public Int32 MatHome { get; set; }

    }

}
