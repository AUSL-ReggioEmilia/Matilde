using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.Scci
{
    public interface IScciPlugin :
        IDisposable
    {
        object Esegui(Dictionary<string, object> dict_pars);
    }
}
