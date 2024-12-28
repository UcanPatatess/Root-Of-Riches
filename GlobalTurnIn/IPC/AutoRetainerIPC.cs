using System;
using ECommons.EzIpcManager;

namespace GlobalTurnIn.IPC
{
    public class AutoRetainerIPC
    {
        public const string Name = "AutoRetainer";
        public const string Repo = "https://love.puni.sh/ment.json";
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public AutoRetainerIPC() => EzIPC.Init(this, Name);
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        [EzIPC("PluginState.%m")] public readonly Func<bool> IsBusy;
        [EzIPC("PluginState.%m")] public readonly Func<int> GetInventoryFreeSlotCount;
        [EzIPC] public readonly Func<bool> GetMultiModeEnabled;
        [EzIPC] public readonly Action<bool> SetMultiModeEnabled;
        [EzIPC] public readonly Func<bool> GetSuppressed;
        [EzIPC] public readonly Action<bool> SetSuppressed;
    }
}
