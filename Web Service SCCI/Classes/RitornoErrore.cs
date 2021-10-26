using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WsSCCI
{
    [DataContract(Name = "RitornoErrore")]
    public class RitornoErrore
    {
        #region Constructor

        public RitornoErrore()
        {
            this.PresenteErrore = false;
            this.DettaglioErroreStringa = string.Empty;
        }

        #endregion

        #region Property

        [DataMember]
        public bool PresenteErrore { get; set; }
        [DataMember]
        public string DettaglioErroreStringa { get; set; }

        #endregion

    }
}