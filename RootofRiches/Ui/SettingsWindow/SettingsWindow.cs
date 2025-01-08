using Dalamud.Interface.Windowing;
using ECommons.ImGuiMethods;
using System.Numerics;

namespace RootofRiches.Ui.SettingsWindow;

internal class SettingsWindow : Window
{
    public SettingsWindow():
    base ("Root of Riches Settings") 
    {
        SizeConstraints = new()
        {
            MinimumSize = new(250, 100),
            MaximumSize = new(9999, 9999)
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
                        ("RaidFarm Settings",NRaidFarmSettings.Draw,null,true)

        );
    }
}
