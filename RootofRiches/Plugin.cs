using AutoRetainerAPI;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ECommons;
using ECommons.Automation.NeoTaskManager;
using ECommons.Configuration;
using ECommons.DalamudServices;
using ECommons.SimpleGui;
using RootofRiches.IPC;
using RootofRiches.Scheduler;
using RootofRiches.Scheduler.Handlers;
using RootofRiches.Windows;
using System.Diagnostics;


namespace RootofRiches;

public class Plugin : IDalamudPlugin
{
    public string Name => "RootofRiches";
    internal static Plugin P = null!;
    private Config config;
    
    public static Config C => P.config;

    // internal window names
    internal WindowSystem windowSystem;
    internal MainWindow mainWindow;
    internal DebugWindow debugWindow;
    internal SettingMenu settingMenu;

    // IPC's/Internals
    internal AutoRetainerApi autoRetainerApi;
    internal TaskManager taskManager;
    internal AutoRetainerIPC autoRetainer;
    internal DeliverooIPC deliveroo;
    internal LifestreamIPC lifestream;
    internal NavmeshIPC navmesh;
    internal BossModIPC bossmod;

    // Timers
    internal Stopwatch stopwatch;
    internal TimeSpan totalRunTime;
    internal TimeSpan fastestRun;



    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        P = this;
        ECommonsMain.Init(pluginInterface, P, ECommons.Module.DalamudReflector, ECommons.Module.ObjectFunctions);
        new ECommons.Schedulers.TickScheduler(Load);
    }

    public void Load() 
    {
        EzConfig.Migrate<Config>();
        config = EzConfig.Init<Config>();

        // IPC's
        taskManager = new();
        autoRetainer = new();
        deliveroo = new();
        lifestream = new();
        navmesh = new();
        bossmod = new();

        // Windows
        windowSystem = new();
        mainWindow = new();
        debugWindow = new();
        settingMenu = new();

        Svc.PluginInterface.UiBuilder.Draw += windowSystem.Draw;
        Svc.PluginInterface.UiBuilder.OpenMainUi += () =>
        {
            mainWindow.IsOpen = true;
        };
        Svc.PluginInterface.UiBuilder.OpenConfigUi += () =>
        {
            settingMenu.IsOpen = true;
        };
        EzCmd.Add("/rootofriches", OnCommand, """
                   Open plugin interface
                   /ror - alias for /rootofriches
                   /rootofriches s|settings - Opens the turnin settings
                   """);
        EzCmd.Add("/ror", OnCommand);

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
        Safe(() => Svc.PluginInterface.UiBuilder.Draw -= windowSystem.Draw);
        ECommonsMain.Dispose();
        Safe(TextAdvanceManager.UnlockTA);
        Safe(YesAlreadyManager.Unlock);
    }
    private void OnCommand(string command, string args)
    {
        if (args.EqualsIgnoreCaseAny("d", "debug"))
        {
            debugWindow.IsOpen = !debugWindow.IsOpen;
        }
        else if (args.EqualsIgnoreCaseAny("s", "settings"))
        {
            settingMenu.IsOpen = !settingMenu.IsOpen;
        }
        else
        {
            mainWindow.IsOpen = !mainWindow.IsOpen;
        }
    }
}
