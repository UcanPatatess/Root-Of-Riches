using Dalamud.Interface.Windowing;
using ECommons.ImGuiMethods;
using ImGuiNET;
using Dalamud.Interface;
using OptimizedChocoboColoring.Ui.MainWindow;

namespace RootofRiches.Ui.MainWindow;

internal class MainWindow : Window
{
    public MainWindow() :
    base($"Optimized Chocobo Coloring {P.GetType().Assembly.GetName().Version} ###OCCMainWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(430, 300),
            MaximumSize = new(9999, 9999)
        };
        TitleBarButtons.Add(new()
        {
            Click = (m) => { if (m == ImGuiMouseButton.Left) P.settingsWindow.IsOpen = !P.settingsWindow.IsOpen; },
            Icon = FontAwesomeIcon.Cog,
            IconOffset = new(2, 2),
            ShowTooltip = () => ImGui.SetTooltip("Open settings window"),
        });
        P.windowSystem.AddWindow(this);
    }

    public void Dispose()
    {
    }

    public override void Draw()
    {
        ImGuiEx.EzTabBar("OCC tabbar",
            ("Color Menu", ColoringUi.Draw, null, true)

                        );
    }
}
