using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WsSCCI
{
    [DataContract(Name = "BarcodeFarmaco")]
    public class BarcodeFarmaco
    {
        #region Constructor

        public BarcodeFarmaco()
        {
            this.Sequenza = string.Empty;
            this.Barcode = string.Empty;
        }

        #endregion

        #region Properties

        [DataMember]
        public string Sequenza { get; set; }

        [DataMember]
        public string Barcode { get; set; } 

        #endregion

    }
}