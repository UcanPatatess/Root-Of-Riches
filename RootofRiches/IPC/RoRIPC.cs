using ECommons.EzIpcManager;
using ECommons.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootofRiches.IPC;

internal class RoRIPC
{
    internal RoRIPC()
    {
        EzIPC.Init(this);
    }

    [EzIPC]
    public void WrathComboCallback(int reason, string additionalInfo) => WrathIPC.CancelActions(reason, additionalInfo);
}
