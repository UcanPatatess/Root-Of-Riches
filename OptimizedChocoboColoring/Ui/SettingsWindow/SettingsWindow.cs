using Dalamud.Interface.Windowing;
using ECommons.ImGuiMethods;

namespace RootofRiches.Ui.SettingsWindow;

internal class SettingsWindow : Window
{
    public SettingsWindow():
    base ("Optimized Chocobo Coloring Settings ###OCCSettingsWindow") 
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
        ImGuiEx.EzTabBar("OCC Settings Tabs"
                        

        );
    }
}
