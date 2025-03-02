using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Numerics;
using ECommons.ImGuiMethods;

namespace RootofRiches.Ui.Debugwindow;

internal class DebugWindow : Window
{
    public new static readonly string WindowName = "OCC Debug###OCCDebugWindow";
    public DebugWindow() :
        base(WindowName)
    {
        Flags = ImGuiWindowFlags.NoCollapse;
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
        ImGuiEx.EzTabBar("ROR Debug Tabs"
                        
        );
    }
}
