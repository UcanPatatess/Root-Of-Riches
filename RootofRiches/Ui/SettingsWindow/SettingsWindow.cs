using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ECommons.ImGuiMethods;
using ImGuiNET;
using System.Numerics;

namespace RootofRiches.Ui.SettingsWindow;

internal class SettingsWindow : Window
{
    public SettingsWindow():
    base ("Root of Riches Settings") 
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(100, 100),
            MaximumSize = new Vector2(800, 600)
        };
        P.windowSystem.AddWindow(this);
    }
    public void Dispose()
    {
    }

    public override void Draw()
    {
        ImGuiEx.EzTabBar("RoR Settings Tabs",
                        ("TurnIn Settings",TurninSettingsUi.Draw,null,true),
                        ("RaidFarm Settings",NormalRaidFarmSettingsUi.Draw,null,true)
            );
    }
}
