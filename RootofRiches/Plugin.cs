using Dalamud.Plugin;
using ECommons;
using ECommons.Automation.NeoTaskManager;
using ECommons.Configuration;
using ECommons.DalamudServices;
using ECommons.SimpleGui;
using GlobalTurnIn.IPC;
using GlobalTurnIn.Scheduler;
using GlobalTurnIn.Scheduler.Handlers;
using GlobalTurnIn.Windows;
using System.Diagnostics;


namespace GlobalTurnIn;

public class Plugin : IDalamudPlugin
{
    private const string Command = "/globalt";
    private static string[] Aliases => ["/pgt", "/pglobal"];
    public string Name => "GlobalTurnIn";
    internal static Plugin P = null!;
    private readonly Config config;
    
    public static Config C => P.config;

    internal TaskManager taskManager;
    internal AutoRetainerIPC autoRetainer;
    internal DeliverooIPC deliveroo;
    internal LifestreamIPC lifestream;
    internal NavmeshIPC navmesh;
    internal BossModIPC bossmod;
    internal Stopwatch stopwatch;
    internal TimeSpan totalRunTime;
    internal TimeSpan fastestRun;



    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        P = this;
        ECommonsMain.Init(pluginInterface, P, ECommons.Module.DalamudReflector, ECommons.Module.ObjectFunctions);
        config = EzConfig.Init<Config>();

        EzConfigGui.Init(new MainWindow().Draw);
        EzConfigGui.WindowSystem.AddWindow(new SettingMenu());
        EzConfigGui.WindowSystem.AddWindow(new DebugWindow());
        EzCmd.Add(Command, OnCommand, "Open Interface");
        Aliases.ToList().ForEach(a => EzCmd.Add(a, OnCommand, $"{Command} Alias"));

        taskManager = new();
        autoRetainer = new();
        deliveroo = new();
        lifestream = new();
        navmesh = new();
        bossmod = new();
        stopwatch = new();
        Svc.Framework.Update += Tick;
        C.SessionStats.Reset();
    }
    private void Tick(object _)
    {
        _ = IsThereTradeItem();
        if (SchedulerMain.DoWeTick && Svc.ClientState.LocalPlayer != null)
        {
            SchedulerMain.Tick();
        }
        GenericManager.Tick();
        TextAdvanceManager.Tick();
        YesAlreadyManager.Tick();
    }
    public void Dispose()
    {
        Safe(() => Svc.Framework.Update -= Tick);
        ECommonsMain.Dispose();
        Safe(TextAdvanceManager.UnlockTA);
        Safe(YesAlreadyManager.Unlock);
    }
    private void OnCommand(string command, string args)
    {
        if (args == "debug")
        {
            EzConfigGui.WindowSystem.Windows.FirstOrDefault(w => w.WindowName == DebugWindow.WindowName)!.IsOpen ^= true;
        }
        else if (args.StartsWith("s"))
            EzConfigGui.WindowSystem.Windows.First(w => w is SettingMenu).IsOpen ^= true;
        else
            EzConfigGui.Window.IsOpen = !EzConfigGui.Window.IsOpen;
    }
}
