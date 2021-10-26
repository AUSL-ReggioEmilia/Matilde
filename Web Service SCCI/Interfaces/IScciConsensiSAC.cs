using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WsSCCI
{
        [ServiceContract]
    public interface IScciConsensiSAC
    {

        [OperationContract]
        string AggiungiConsenso(string idsac, string pazientecognome, string pazientenome, string pazientecodicefiscale,
                                string operatoreid, string operatorecognome, string operatorenome, string operatorecomputer,
                                string tipoconsenso);

                
    }
}
