using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;

namespace UnicodeSrl.Scci.PluginClient
{

    public interface IPluginClientResponseBase
    {
        bool Successo { get; set; }
        object[] Parameters { get; set; }
        Exception ex { get; set; }
    }

    public interface IPluginClient
    {
        Risposta Execute(object[] parameters, string modalita);
        Task<Risposta> AsyncExecute(object[] parameters, string modalita);
    }

}
