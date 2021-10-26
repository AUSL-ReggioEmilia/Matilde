using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.Scci.PluginClient
{

    public class RispostaCerca : IPluginClientResponseBase
    {

        public RispostaCerca()
        {
            Successo = false;
            Parameters = null;
            ex = null;
            DaEseguire = false;
            PluginDaScaricare = new Plugins();
            Plugin = new Plugins();
        }

        public bool Successo { get; set; }

        public object[] Parameters { get; set; }

        public Exception ex { get; set; }

        public bool DaEseguire { get; set; }

        public Plugins PluginDaScaricare { get; set; }

        public Plugins Plugin { get; set; }

    }

    public class Risposta : IPluginClientResponseBase
    {


        public Risposta()
        {
            Successo = false;
            Parameters = null;
            ex = null;
        }

        public bool Successo { get; set; }

        public object[] Parameters { get; set; }

        public Exception ex { get; set; }

        public void OnException(Exception ex)
        {
            UnicodeSrl.Framework.Diagnostics.Statics.AddDebugInfo(ex);
            this.Successo = false;
            this.ex = ex;
            this.Parameters = null;
        }

    }

}
