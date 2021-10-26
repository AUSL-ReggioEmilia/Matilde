using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;

using UnicodeSrl.Scci.DataContracts;
using UnicodeSrl.Scci.Enums;

namespace WsSCCI
{
        [ServiceContract]
    public interface IScciRefertiDWH
    {
        [OperationContract]
        List<RefertoDWH> RicercaRefertiDWH(string idsac, DateTime datainizio, DateTime datafine, string tipoevidenzaclinica, string statoevidenzaclinica);

        [OperationContract]
        List<AllegatoRefertoDWH> CaricaRefertoDWH(string idreferto);

        [OperationContract]
        RefertoDWHDetailed CaricaRefertoDWHDettaglio(string idreferto);

        [OperationContract]
        Dictionary<string, string> RicercaContenutiReferto(string idreferto, EnumTipoContenutiReferto tipo);

        [OperationContract]
        string[] CaricaContenutiDaListaID(string idreferto, string[] idcontenuti, EnumTipoContenutiReferto tipo);

        [OperationContract]
        List<RefertoDWH> RicercaRefertiDWHTipi(string idsac, DateTime datainizio, DateTime datafine, List<string> tipievidenzaclinica, List<string> statievidenzaclinica);

    }
}
