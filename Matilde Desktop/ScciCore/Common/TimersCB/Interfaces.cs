using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UnicodeSrl.ScciCore.Common.TimersCB
{
    internal interface I_RefreshTimer_Controllo
    {
        SynchronizationContext SyncContext { get; }
        void RefreshData(object state);
    }

}
