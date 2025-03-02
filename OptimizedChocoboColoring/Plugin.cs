using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ECommons;
using ECommons.Automation.NeoTaskManager;
using ECommons.Configuration;
using ECommons.DalamudServices;
using OptimizedChocoboColoring.IPC;
using OptimizedChocoboColoring.Scheduler.Handlers;
using RootofRiches.Ui.Debugwindow;
using RootofRiches.Ui.MainWindow;
using RootofRiches.Ui.SettingsWindow;


namespace OptimizedChocoboColoring;

public class Plugin : IDalamudPlugin
{
    public string Name => "OptimizedChocoboColoring";
    internal static Plugin P = null!;
    private Config config;
    
    public static Config C => P.config;

    // internal window names
    internal WindowSystem windowSystem;
    internal MainWindow mainWindow;
    internal SettingsWindow settingsWindow;
    internal DebugWindow debugWindow;

    // IPC's/Internals
    internal PandoraIPC pandora;
    internal NavmeshIPC navmesh;
    internal TaskManager taskManager;

#pragma warning disable CS8618
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

        //taskmanager
        taskManager = new();

        // ipcs
        navmesh = new();
        pandora = new();

        // Windows
        windowSystem = new();
        mainWindow = new();
        settingsWindow = new();
        debugWindow = new();

        Svc.PluginInterface.UiBuilder.Draw += windowSystem.Draw;
        
        Svc.PluginInterface.UiBuilder.OpenMainUi += () =>
        {
            mainWindow.IsOpen = true;
        };
        Svc.PluginInterface.UiBuilder.OpenConfigUi += () =>
        {
            settingsWindow.IsOpen = !settingsWindow.IsOpen;
        };
        
        EzCmd.Add("/OptimizedChocoboColoring", OnCommand, """
                   Open plugin interface
                   /occ - alias for /OptimizedChocoboColoring
                   /OptimizedChocoboColoring s|settings - Opens the settings
                   """);
        EzCmd.Add("/occ", OnCommand);

        Svc.Framework.Update += Tick;
    }

    private void Tick(object _)
    {
        if (Scheculer.SchedulerMain.DoWeTick && Svc.ClientState.LocalPlayer != null)
        {
            Scheculer.SchedulerMain.Tick();
        }
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
        else if (args.EqualsIgnoreCaseAny("s", "settings", "setting"))
        {
            settingsWindow.IsOpen = !settingsWindow.IsOpen;
        }
        else
        {
            mainWindow.IsOpen = !mainWindow.IsOpen;
        }
    }
}
