using System;
using System.Data;

namespace UnicodeSrl.ScciCore.Common.MT
{


    public delegate void DelegateDatatableCompleted(object sender, DataTable dt);


    public delegate void DelegateThreadStarted(object sender);

    public delegate void DelegateThreadCompleted(object sender);

    public delegate void DelegateThreadCancelled(object sender);

    public delegate void DelegateLastThreadException(object sender, Exception ex);


}
