using System.Numerics;

namespace OptimizedChocoboColoring
{
    public class ColoringStuff
    {
        public static Dictionary<string, Vector3> predefinedColors = new Dictionary<string, Vector3>
    {
        { "Snow White", new Vector3(228f/255f, 223f/255f, 208f/255f) },
        { "Ash Grey", new Vector3(172f/255f, 168f/255f, 162f/255f) },
        { "Goobbue Grey", new Vector3(137f/255f, 135f/255f, 132f/255f) },
        { "Slate Grey", new Vector3(101f/255f, 101f/255f, 101f/255f) },
        { "Charcoal Grey", new Vector3(72f/255f, 71f/255f, 66f/255f) },
        { "Soot Black", new Vector3(43f/255f, 41f/255f, 35f/255f) },
        { "Rose Pink", new Vector3(230f/255f, 159f/255f, 150f/255f) },
        { "Lilac Purple", new Vector3(131f/255f, 105f/255f, 105f/255f) },
        { "Rolanberry Red", new Vector3(91f/255f, 23f/255f, 41f/255f) },
        { "Dalamud Red", new Vector3(120f/255f, 26f/255f, 26f/255f) },
        { "Rust Red", new Vector3(98f/255f, 34f/255f, 7f/255f) },
        { "Wine Red", new Vector3(69f/255f, 21f/255f, 17f/255f) },
        { "Coral Pink", new Vector3(204f/255f, 108f/255f, 94f/255f) },
        { "Blood Red", new Vector3(145f/255f, 59f/255f, 39f/255f) },
        { "Salmon Pink", new Vector3(228f/255f, 170f/255f, 138f/255f) },
        { "Sunset Orange", new Vector3(183f/255f, 92f/255f, 45f/255f) },
        { "Mesa Red", new Vector3(125f/255f, 57f/255f, 6f/255f) },
        { "Bark Brown", new Vector3(106f/255f, 75f/255f, 55f/255f) },
        { "Chocolate Brown", new Vector3(110f/255f, 61f/255f, 36f/255f) },
        { "Russet Brown", new Vector3(79f/255f, 45f/255f, 31f/255f) },
        { "Kobold Brown", new Vector3(48f/255f, 33f/255f, 27f/255f) },
        { "Cork Brown", new Vector3(201f/255f, 145f/255f, 86f/255f) },
        { "Qiqirn Brown", new Vector3(153f/255f, 110f/255f, 63f/255f) },
        { "Opo-opo Brown", new Vector3(123f/255f, 92f/255f, 45f/255f) },
        { "Aldgoat Brown", new Vector3(162f/255f, 135f/255f, 92f/255f) },
        { "Pumpkin Orange", new Vector3(197f/255f, 116f/255f, 36f/255f) },
        { "Acorn Brown", new Vector3(142f/255f, 88f/255f, 27f/255f) },
        { "Orchard Brown", new Vector3(100f/255f, 66f/255f, 22f/255f) },
        { "Chestnut Brown", new Vector3(61f/255f, 41f/255f, 13f/255f) },
        { "Gobbiebag Brown", new Vector3(185f/255f, 164f/255f, 137f/255f) },
        { "Shale Brown", new Vector3(146f/255f, 129f/255f, 108f/255f) },
        { "Mole Brown", new Vector3(97f/255f, 82f/255f, 69f/255f) },
        { "Loam Brown", new Vector3(63f/255f, 51f/255f, 41f/255f) },
        { "Bone White", new Vector3(235f/255f, 211f/255f, 160f/255f) },
        { "Ul Brown", new Vector3(183f/255f, 163f/255f, 112f/255f) },
        { "Desert Yellow", new Vector3(219f/255f, 180f/255f, 87f/255f) },
        { "Honey Yellow", new Vector3(250f/255f, 198f/255f, 43f/255f) },
        { "Millioncorn Yellow", new Vector3(228f/255f, 158f/255f, 52f/255f) },
        { "Coeurl Yellow", new Vector3(188f/255f, 136f/255f, 4f/255f) },
        { "Cream Yellow", new Vector3(242f/255f, 215f/255f, 112f/255f) },
        { "Halatali Yellow", new Vector3(165f/255f, 132f/255f, 48f/255f) },
        { "Raisin Brown", new Vector3(64f/255f, 51f/255f, 17f/255f) },
        { "Mud Green", new Vector3(88f/255f, 82f/255f, 48f/255f) },
        { "Sylph Green", new Vector3(187f/255f, 187f/255f, 138f/255f) },
        { "Lime Green", new Vector3(171f/255f, 176f/255f, 84f/255f) },
        { "Moss Green", new Vector3(112f/255f, 115f/255f, 38f/255f) },
        { "Meadow Green", new Vector3(139f/255f, 156f/255f, 99f/255f) },
        { "Olive Green", new Vector3(75f/255f, 82f/255f, 50f/255f) },
        { "Marsh Green", new Vector3(50f/255f, 54f/255f, 33f/255f) },
        { "Apple Green", new Vector3(155f/255f, 179f/255f, 99f/255f) },
        { "Cactuar Green", new Vector3(101f/255f, 130f/255f, 65f/255f) },
        { "Hunter Green", new Vector3(40f/255f, 75f/255f, 44f/255f) },
        { "Ochu Green", new Vector3(64f/255f, 99f/255f, 57f/255f) },
        { "Adamantoise Green", new Vector3(95f/255f, 117f/255f, 88f/255f) },
        { "Nophica Green", new Vector3(59f/255f, 77f/255f, 60f/255f) },
        { "Deepwood Green", new Vector3(30f/255f, 42f/255f, 33f/255f) },
        { "Celeste Green", new Vector3(150f/255f, 189f/255f, 185f/255f) },
        { "Turquoise Green", new Vector3(67f/255f, 114f/255f, 114f/255f) },
        { "Morbol Green", new Vector3(31f/255f, 70f/255f, 70f/255f) },
        { "Ice Blue", new Vector3(178f/255f, 196f/255f, 206f/255f) },
        { "Sky Blue", new Vector3(131f/255f, 176f/255f, 210f/255f) },
        { "Seafog Blue", new Vector3(100f/255f, 129f/255f, 160f/255f) },
        { "Peacock Blue", new Vector3(59f/255f, 104f/255f, 134f/255f) },
        { "Rhotano Blue", new Vector3(28f/255f, 61f/255f, 84f/255f) },
        { "Corpse Blue", new Vector3(142f/255f, 155f/255f, 172f/255f) },
        { "Ceruleum Blue", new Vector3(79f/255f, 87f/255f, 102f/255f) },
        { "Woad Blue", new Vector3(47f/255f, 56f/255f, 81f/255f) },
        { "Ink Blue", new Vector3(26f/255f, 31f/255f, 39f/255f) },
        { "Raptor Blue", new Vector3(91f/255f, 127f/255f, 192f/255f) },
        { "Othard Blue", new Vector3(47f/255f, 88f/255f, 137f/255f) },
        { "Storm Blue", new Vector3(35f/255f, 65f/255f, 114f/255f) },
        { "Void Blue", new Vector3(17f/255f, 41f/255f, 68f/255f) },
        { "Royal Blue", new Vector3(39f/255f, 48f/255f, 103f/255f) },
        { "Midnight Blue", new Vector3(24f/255f, 25f/255f, 55f/255f) },
        { "Shadow Blue", new Vector3(55f/255f, 55f/255f, 71f/255f) },
        { "Abyssal Blue", new Vector3(49f/255f, 45f/255f, 87f/255f) },
        { "Lavender Purple", new Vector3(135f/255f, 127f/255f, 174f/255f) },
        { "Gloom Purple", new Vector3(81f/255f, 69f/255f, 96f/255f) },
        { "Currant Purple", new Vector3(50f/255f, 44f/255f, 59f/255f) },
        { "Iris Purple", new Vector3(183f/255f, 158f/255f, 188f/255f) },
        { "Grape Purple", new Vector3(59f/255f, 42f/255f, 61f/255f) },
        { "Lotus Pink", new Vector3(254f/255f, 206f/255f, 245f/255f) },
        { "Colibri Pink", new Vector3(220f/255f, 155f/255f, 202f/255f) },
        { "Plum Purple", new Vector3(121f/255f, 82f/255f, 108f/255f) },
        { "Regal Purple", new Vector3(102f/255f, 48f/255f, 78f/255f) }
    };

        // Define the fruits and their RGB adjustments
        public static readonly (string Name, int R, int G, int B)[] Fruits = new[]
        {
        ("Xelphatol Apple", 5, -5, -5),
        ("Mamook Pear", -5, 5, -5),
        ("O'Ghomoro Berries", -5, -5, 5),
        ("Doman Plum", -5, 5, 5),
        ("Valfruit", 5, -5, 5),
        ("Cieldalaes Pineapple", 5, 5, -5)
    };

        // Clamp RGB values to the range [0, 250]
        public static int Clamp(int value)
        {
            return Math.Clamp(value, 0, 250);
        }

        // Calculate the Euclidean distance between two colors
        public static double Distance((int R, int G, int B) color1, (int R, int G, int B) color2)
        {
            int dR = color1.R - color2.R;
            int dG = color1.G - color2.G;
            int dB = color1.B - color2.B;
            return Math.Sqrt(dR * dR + dG * dG + dB * dB);
        }

        // Apply a fruit's adjustments to a color and clamp the result
        public static (int R, int G, int B) ApplyFruit((int R, int G, int B) color, (string Name, int R, int G, int B) fruit)
        {
            return (
                Clamp(color.R + fruit.R),
                Clamp(color.G + fruit.G),
                Clamp(color.B + fruit.B)
            );
        }

        // Find the sequence of fruits to transform startColor into targetColor
        public static List<string> FindFruitSequence((int R, int G, int B) startColor, (int R, int G, int B) targetColor)
        {
            var fruits = new List<string>();
            var currentColor = startColor;

            while (true)
            {
                // Generate all possible paths (sequences of fruits)
                var allPaths = Fruits.Select(fruit => new List<(string Name, int R, int G, int B)> { fruit }).ToList();

                // Sort paths by the distance after applying the first fruit in the path
                var pathsSorted = allPaths.OrderBy(path =>
                {
                    var newColor = ApplyFruit(currentColor, path[0]);
                    return Distance(newColor, targetColor);
                }).ToList();

                // Get the best path (the one that minimizes the distance)
                var bestPath = pathsSorted.First();

                // If no path improves the distance, stop
                if (bestPath.Count == 0 || Distance(ApplyFruit(currentColor, bestPath[0]), targetColor) >= Distance(currentColor, targetColor))
                {
                    return fruits;
                }

                // Otherwise, add the first fruit in the best path to the list and update the current color
                var bestFruit = bestPath[0];
                fruits.Add(bestFruit.Name);
                currentColor = ApplyFruit(currentColor, bestFruit);
            }
        }

        public static (int R, int G, int B) ConvertToRGB(Vector3 color)
        {
            return (
                R: (int)(color.X * 255),
                G: (int)(color.Y * 255),
                B: (int)(color.Z * 255)
            );
        }

    }
}
