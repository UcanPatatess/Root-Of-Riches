

using ImGuiNET;
using System.Numerics;

namespace OptimizedChocoboColoring.Ui.MainWindow
{
    internal class ColoringUi
    {
        /*
          bunu kullanıp renk denemsi yapmam lazım startrgb oyundan alınacak bakılması lazım target oyuncu dropdown menuden seçicek
        // Example start and target colors
        var startColor = (R: 100, G: 150, B: 200);
        var targetColor = (R: 120, G: 130, B: 180);

        // Find the fruit sequence
        var sequence = FindFruitSequence(startColor, targetColor);

        // Output the result
        Console.WriteLine("Fruit Sequence:");
        foreach (var fruit in sequence)
        {
            Console.WriteLine(fruit);
        }
         */

        private static string SearchFilter = "";
        private static string SelectedColorName = "Desert Yellow";
        private static Vector3 SelectedColor = ColoringStuff.predefinedColors[SelectedColorName];

        public static void Draw()
        {
            // Input box for typing to filter the color names
            ImGui.InputText("Search Color", ref SearchFilter, 100);

            // Dropdown to select predefined colors with search functionality
            if (ImGui.BeginCombo("Predefined Colors", SelectedColorName))
            {
                foreach (var color in ColoringStuff.predefinedColors)
                {
                    // Filter colors based on the search input
                    if (!color.Key.Contains(SearchFilter, StringComparison.OrdinalIgnoreCase))
                        continue;

                    bool isSelected = SelectedColorName == color.Key;
                    if (ImGui.Selectable(color.Key, isSelected))
                    {
                        SelectedColorName = color.Key;
                        SelectedColor = color.Value;
                    }
                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                }
                ImGui.EndCombo();
            }

            // Display the selected color
            ImGui.Text("Selected Color:");
            ImGui.SameLine();
            ImGui.ColorButton("##SelectedColor", new Vector4(SelectedColor.X, SelectedColor.Y, SelectedColor.Z, 1.0f), ImGuiColorEditFlags.NoPicker | ImGuiColorEditFlags.NoTooltip, new Vector2(20, 20));

            // Display RGB values of the selected color
            ImGui.Text($"RGB: {(int)(SelectedColor.X * 255)}, {(int)(SelectedColor.Y * 255)}, {(int)(SelectedColor.Z * 255)}");

            // Reset button to revert to the default color
            if (ImGui.Button("Reset to Default"))
            {
                SelectedColorName = "Desert Yellow"; // Default color name
                SelectedColor = ColoringStuff.predefinedColors[SelectedColorName]; // Default color value
            }

            // Display the sequence in an ImGui window
            var StartColor = ColoringStuff.predefinedColors["Desert Yellow"];
            var TargetColor = ColoringStuff.predefinedColors[SelectedColorName];
            
            if (ImGui.Button("Calculate Fruit Needed"))
            {
                // Find the fruit sequence
                var rgbS = ColoringStuff.ConvertToRGB(StartColor);
                var rgbT = ColoringStuff.ConvertToRGB(TargetColor);
                var sequence = ColoringStuff.FindFruitSequence(rgbS, rgbT);

                var fruitCounts = new Dictionary<string, int>();
                foreach (var fruit in sequence)
                {
                    if (fruitCounts.ContainsKey(fruit))
                    {
                        fruitCounts[fruit]++;
                    }
                    else
                    {
                        fruitCounts[fruit] = 1;
                    }
                }
                // Output the result
                PLogDebug("Fruit Sequence:");
                foreach (var kvp in fruitCounts)
                {
                    PLogDebug($"{kvp.Value}×{kvp.Key}");
                }
                foreach (var fruit in sequence)
                {
                    PLogDebug(fruit);
                }
            }
        }
    }
}
