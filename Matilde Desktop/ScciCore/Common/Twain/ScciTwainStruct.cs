using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.ScciCore
{

    public enum en_Twain_State
    {
        Undefined = -1,
        initial = 1,
        DsmLoaded = 2,
        DsmOpened = 3,
        DsOpened = 4,
        DsEnabled = 5,
        DsReadyToTransfer = 6,
        ImageTransferring = 7

    }

}
