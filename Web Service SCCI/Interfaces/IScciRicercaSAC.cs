using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;

using UnicodeSrl.Scci.DataContracts;

namespace WsSCCI
{

    [ServiceContract]
    public interface IScciRicercaSAC
    {

        [OperationContract]
        List<PazienteSac> RicercaPazientiSAC(string cognome, string nome, DateTime datanascita, string luogonascita, string codfiscale);

        [OperationContract]
        PazienteSac RicercaPazientiSACByID(string idsac);

        [OperationContract]
        PazienteSacDatiAggiuntivi PazienteSacDatiAggiuntiviByID(string idsac);

    }
  
}
