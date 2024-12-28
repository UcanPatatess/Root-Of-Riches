using ECommons.EzIpcManager;
using System;

#nullable disable

namespace GlobalTurnIn.IPC
{
    public class DeliverooIPC
    {
        public const string Name = "Deliveroo";
        public const string Repo = "https://git.carvel.li/liza/";
        public DeliverooIPC() => EzIPC.Init(this, Name, SafeWrapper.AnyException);

        [EzIPC] public Func<bool> IsTurnInRunning;
    }
}
