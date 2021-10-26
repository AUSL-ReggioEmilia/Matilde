using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci;

namespace WsSCCI
{
    public class ScciPrescrizioni : IScciPrescrizioni
    {
                                                        public bool GeneraTaskDaPrescrizione(string connectionString, string idPrescrizione, string idPrescrizionetempi, 
                                                UnicodeSrl.Scci.Enums.EnumCodSistema sistema, 
                                                string codtipotaskinfermieristico, ScciAmbiente amb)
        {
            try
            {
                UnicodeSrl.Scci.Statics.Database.ConnectionString = connectionString;
                MovPrescrizioneTempi movprt = new MovPrescrizioneTempi(idPrescrizionetempi, idPrescrizione, amb);
                movprt.CreaTaskInfermieristici(sistema, codtipotaskinfermieristico, UnicodeSrl.Scci.Enums.EnumTipoRegistrazione.A);
            }
            catch (Exception ex)
            {
                ErrorHandler.AddException(ex);
            }

            return false;
        }

    }
}
