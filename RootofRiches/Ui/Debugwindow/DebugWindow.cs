using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using RootofRiches.Scheduler.Tasks;
using ImGuiNET;
using System.Globalization;
using System.Numerics;
using ECommons;
using ECommons.ExcelServices.TerritoryEnumeration;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;

namespace RootofRiches.Ui.Debugwindow;

internal class DebugWindow : Window
{
    public new static readonly string WindowName = "RoR Debug###RORDebugWindow";
    public DebugWindow() :
        base(WindowName)
    {
        Flags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse;
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(400, 100),
            MaximumSize = new Vector2(1000, 1000)
        };
        P.windowSystem.AddWindow(this);
    }
    public void Dispose() { }

    public override void Draw()
    {
        ImGuiEx.EzTabBar("ROR Debug Tabs",
                        ("ROR Turnin Debug", TurninDebug.Draw, null, true),
                        ("Normal Raid Debug", NormalRaidDebug.Draw, null, true),
                        ("Targeting Debug", TargetingDebug.Draw, null, true),
                        ("Misc Debug", MiscDebug.Draw, null, true)
        );
    }
}
