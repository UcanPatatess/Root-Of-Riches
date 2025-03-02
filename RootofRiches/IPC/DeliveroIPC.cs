using ECommons.EzIpcManager;

#nullable disable

namespace RootofRiches.IPC
{
    public class DeliverooIPC
    {
        public const string Name = "Deliveroo";
        public const string Repo = "https://git.carvel.li/liza/";
        public DeliverooIPC() => EzIPC.Init(this, Name, SafeWrapper.AnyException);
        public bool Installed => PluginInstalled(Name);

        [EzIPC] public Func<bool> IsTurnInRunning;
    }
}
