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
    public interface IScciRicoveriDWH
    {
                
        [OperationContract]
        RicoveroDWH RicoveroPerId(string idricovero);

                
        [OperationContract]
        List<RicoveroDWHSintetico> RicercaRicoveriDWH(string idsac, DateTime datainizio, DateTime datafine);

                
        [OperationContract]
        List<RisultatiLab> RicercaDatiLabDWH(string idsac, DateTime datainizio, DateTime datafine);

        [OperationContract]
        List<RisultatiLabUM> RicercaDatiLabDWHUM(string idsac, DateTime datainizio, DateTime datafine);

        [OperationContract]
        List<RisultatiLabAll> RicercaDatiLabDWHAll(string idsac, DateTime datainizio, DateTime datafine);

        
    }
}
