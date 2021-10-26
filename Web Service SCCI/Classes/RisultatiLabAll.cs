using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WsSCCI
{
                                [DataContract(Name = "RisultatiLabAll")]
    public class RisultatiLabAll
    {
        #region Constructor

        public RisultatiLabAll()
        {
            IdReferto = string.Empty;
            CodSezione = string.Empty;
            DescrSezione = string.Empty;
            CodPrescrizione = string.Empty;
            DescPrescrizione = string.Empty;
            Data = DateTime.MinValue;
            Quantita = 0;
            UM = string.Empty;
        }

        #endregion

        #region Property

        [DataMember]
        public string IdReferto { get; set; }
        [DataMember]
        public string CodSezione { get; set; }
        [DataMember]
        public string DescrSezione { get; set; }
        [DataMember]
        public string CodPrescrizione { get; set; }
        [DataMember]
        public string DescPrescrizione { get; set; }
        [DataMember]
        public DateTime Data { get; set; }
        [DataMember]
        public Double Quantita { get; set; }
        [DataMember]
        public string UM { get; set; }

        [DataMember]
        public string Risultato { get; set; }

        [DataMember]
        public string Commenti { get; set; }

        [DataMember]
        public bool RisultatoNumericoAssente { get; set; }

        #endregion

    }
}