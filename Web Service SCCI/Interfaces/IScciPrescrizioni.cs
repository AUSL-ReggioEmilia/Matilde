using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using UnicodeSrl.Scci.DataContracts;

namespace WsSCCI
{
    [ServiceContract]
    public interface IScciPrescrizioni
    {
        [OperationContract]
        Boolean GeneraTaskDaPrescrizione(string connectionString, string idPrescrizione, string idPrescrizionetempi,
                                            UnicodeSrl.Scci.Enums.EnumCodSistema sistema,
                                            string codtipotaskinfermieristico, ScciAmbiente amb);

    }
}
