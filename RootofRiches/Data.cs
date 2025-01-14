using ECommons.ExcelServices.TerritoryEnumeration;
using System.Numerics;
using GrandCompany = ECommons.ExcelServices.GrandCompany;

namespace RootofRiches;

public static class Data
{
    #region Normal Raid Items
    public static int TotalExchangeItem = 0;
    public static int GordianTurnInCount = 0;
    public static int AlexandrianTurnInCount = 0;
    public static int DeltascapeTurnInCount = 0;

    // Deltascape Item IDs
    public static int DeltascapeLensID = 19111;
    public static int DeltascapeShaftID = 19112;
    public static int DeltascapeCrankID = 19113;
    public static int DeltascapeSpringID = 19114;
    public static int DeltascapePedalID = 19115;
    public static int DeltascapeBoltID = 19117;

    // Tarnished Gordian Item IDs
    public static int GordianLensID = 12674;
    public static int GordianShaftID = 12675;
    public static int GordianCrankID = 12676;
    public static int GordianSpringID = 12677;
    public static int GordianPedalID = 12678;
    public static int GordianBoltID = 12680;

    // Alexandrian Item IDs
    public static int AlexandrianLensID = 16546;
    public static int AlexandrianShaftID = 16547;
    public static int AlexandrianCrankID = 16548;
    public static int AlexandrianSpringID = 16549;
    public static int AlexandrianPedalID = 16550;
    public static int AlexandrianBoltID = 16552;

    private static int BoltBuyAmount = 1;
    private static int PedalBuyAmount = 2;
    private static int SpringBuyAmount = 4;
    private static int CrankBuyAmount = 2;
    private static int ShaftBuyAmount = 4;
    private static int LensBuyAmount = 2;

    public static bool IsThereTradeItem()
    {
        TotalExchangeItem = 0;
        for (int i = 0; i < SabinaTable.GetLength(0); i++)
        {
            int itemID = SabinaTable[i, 3];
            int count = GetItemCount(itemID);
            TotalExchangeItem += count;
        }

        // Add up counts from GelfradusTable
        for (int i = 0; i < GelfradusTable.GetLength(0); i++)
        {
            int itemID = GelfradusTable[i, 3];
            int count = GetItemCount(itemID);
            TotalExchangeItem += count;
        }
        // GORDIAN
        int GordianLensCount = GetItemCount(GordianLensID);
        int GordianShaftCount = GetItemCount(GordianShaftID);
        int GordianCrankCount = GetItemCount(GordianCrankID);
        int GordianSpringCount = GetItemCount(GordianSpringID);
        int GordianPedalCount = GetItemCount(GordianPedalID);
        int GordianBoltCount = GetItemCount(GordianBoltID);

        GordianTurnInCount =
            (int)Math.Floor((double)GordianLensCount / LensBuyAmount) +
            (int)Math.Floor((double)GordianShaftCount / ShaftBuyAmount) +
            (int)Math.Floor((double)GordianCrankCount / CrankBuyAmount) +
            (int)Math.Floor((double)GordianSpringCount / SpringBuyAmount) +
            (int)Math.Floor((double)GordianPedalCount / PedalBuyAmount) +
            (int)Math.Floor((double)GordianBoltCount / BoltBuyAmount);

        // ALEXANDRIAN
        int AlexandrianLensCount = GetItemCount(AlexandrianLensID);
        int AlexandrianShaftCount = GetItemCount(AlexandrianShaftID);
        int AlexandrianCrankCount = GetItemCount(AlexandrianCrankID);
        int AlexandrianSpringCount = GetItemCount(AlexandrianSpringID);
        int AlexandrianPedalCount = GetItemCount(AlexandrianPedalID);
        int AlexandrianBoltCount = GetItemCount(AlexandrianBoltID);

        AlexandrianTurnInCount =
            (int)Math.Floor((double)AlexandrianLensCount / LensBuyAmount) +
            (int)Math.Floor((double)AlexandrianShaftCount / ShaftBuyAmount) +
            (int)Math.Floor((double)AlexandrianCrankCount / CrankBuyAmount) +
            (int)Math.Floor((double)AlexandrianSpringCount / SpringBuyAmount) +
            (int)Math.Floor((double)AlexandrianPedalCount / PedalBuyAmount) +
            (int)Math.Floor((double)AlexandrianBoltCount / BoltBuyAmount);

        // DELTASCAPE
        int DeltascapeLensCount = GetItemCount(DeltascapeLensID);
        int DeltascapeShaftCount = GetItemCount(DeltascapeShaftID);
        int DeltascapeCrankCount = GetItemCount(DeltascapeCrankID);
        int DeltascapeSpringCount = GetItemCount(DeltascapeSpringID);
        int DeltascapePedalCount = GetItemCount(DeltascapePedalID);
        int DeltascapeBoltCount = GetItemCount(DeltascapeBoltID);

        DeltascapeTurnInCount =
            (int)Math.Floor((double)DeltascapeLensCount / LensBuyAmount) +
            (int)Math.Floor((double)DeltascapeShaftCount / ShaftBuyAmount) +
            (int)Math.Floor((double)DeltascapeCrankCount / CrankBuyAmount) +
            (int)Math.Floor((double)DeltascapeSpringCount / SpringBuyAmount) +
            (int)Math.Floor((double)DeltascapePedalCount / PedalBuyAmount) +
            (int)Math.Floor((double)DeltascapeBoltCount / BoltBuyAmount);

        // Final Decision
        return !(GordianTurnInCount < 1 && DeltascapeTurnInCount < 1 && AlexandrianTurnInCount < 1);
    }
    public static Dictionary<int, int> ItemIdArmoryTable { get; } = new Dictionary<int, int>
    {
        // ArmoryHead = 3201
        // Deltascape
        { 19437, 3201 }, { 19443, 3201 }, { 19449, 3201 }, { 19461, 3201 }, { 19455, 3201 },
        { 19467, 3201 }, { 19473, 3201 },
        // Gordian
        { 11450, 3201 }, { 11449, 3201 }, { 11448, 3201 }, { 11451, 3201 }, { 11452, 3201 },
        { 11453, 3201 }, { 11454, 3201 },
        // MIDAN Alexandrian
        { 16439, 3201 }, { 16433, 3201 }, { 16415, 3201 }, { 16409, 3201 }, { 16403, 3201 },
        { 16421, 3201 }, { 16427, 3201 },

        // ArmoryBody = 3202
        // Deltascape
        { 19474, 3202 }, { 19468, 3202 }, { 19462, 3202 }, { 19456, 3202 }, { 19438, 3202 },
        { 19444, 3202 }, { 19450, 3202 },
        // Gordian
        { 11461, 3202 }, { 11460, 3202 }, { 11459, 3202 }, { 11458, 3202 }, { 11455, 3202 },
        { 11456, 3202 }, { 11457, 3202 },
        // MIDAN Alexandrian
        { 16440, 3202 }, { 16434, 3202 }, { 16428, 3202 }, { 16422, 3202 }, { 16404, 3202 },
        { 16410, 3202 }, { 16416, 3202 },

        // ArmoryHands = 3203
        // Deltascape
        { 19475, 3203 }, { 19469, 3203 }, { 19463, 3203 }, { 19457, 3203 }, { 19439, 3203 },
        { 19445, 3203 }, { 19451, 3203 },
        // Gordian
        { 11468, 3203 }, { 11467, 3203 }, { 11466, 3203 }, { 11465, 3203 }, { 11462, 3203 },
        { 11463, 3203 }, { 11464, 3203 },
        // MIDAN Alexandrian
        { 16441, 3203 }, { 16435, 3203 }, { 16429, 3203 }, { 16423, 3203 }, { 16405, 3203 },
        { 16411, 3203 }, { 16417, 3203 },

        // ArmoryLegs = 3205
        // Deltascape
        { 19476, 3205 }, { 19470, 3205 }, { 19464, 3205 }, { 19458, 3205 }, { 19440, 3205 },
        { 19446, 3205 }, { 19452, 3205 },
        // Gordian
        { 11482, 3205 }, { 11481, 3205 }, { 11480, 3205 }, { 11479, 3205 }, { 11476, 3205 },
        { 11477, 3205 }, { 11478, 3205 },
        // MIDAN Alexandrian
        { 16442, 3205 }, { 16436, 3205 }, { 16430, 3205 }, { 16424, 3205 }, { 16406, 3205 },
        { 16412, 3205 }, { 16418, 3205 },

        // ArmoryFeets = 3206
        // Deltascape
        { 19477, 3206 }, { 19471, 3206 }, { 19465, 3206 }, { 19459, 3206 }, { 19441, 3206 },
        { 19447, 3206 }, { 19453, 3206 },
        // Gordian
        { 11489, 3206 }, { 11488, 3206 }, { 11487, 3206 }, { 11486, 3206 }, { 11483, 3206 },
        { 11484, 3206 }, { 11485, 3206 },
        // MIDAN Alexandrian
        { 16443, 3206 }, { 16437, 3206 }, { 16431, 3206 }, { 16425, 3206 }, { 16407, 3206 },
        { 16413, 3206 }, { 16419, 3206 },

        // ArmoryEar = 3207
        // Deltascape
        { 19479, 3207 }, { 19480, 3207 }, { 19481, 3207 }, { 19483, 3207 }, { 19482, 3207 },
        // Gordian
        { 11490, 3207 }, { 11491, 3207 }, { 11492, 3207 }, { 11494, 3207 }, { 11493, 3207 },
        // MIDAN Alexandrian
        { 16449, 3207 }, { 16448, 3207 }, { 16447, 3207 }, { 16445, 3207 }, { 16446, 3207 },

        // ArmoryNeck = 3208
        // Deltascape
        { 19484, 3208 }, { 19485, 3208 }, { 19486, 3208 }, { 19488, 3208 }, { 19487, 3208 },
        // Gordian
        { 11495, 3208 }, { 11496, 3208 }, { 11497, 3208 }, { 11499, 3208 }, { 11498, 3208 },
        // MIDAN Alexandrian
        { 16450, 3208 }, { 16451, 3208 }, { 16452, 3208 }, { 16454, 3208 }, { 16453, 3208 },

        // ArmoryWrist = 3209
        // Deltascape
        { 19489, 3209 }, { 19490, 3209 }, { 19491, 3209 }, { 19493, 3209 }, { 19492, 3209 },
        // Gordian
        { 11500, 3209 }, { 11501, 3209 }, { 11502, 3209 }, { 11504, 3209 }, { 11503, 3209 },
        // MIDAN Alexandrian
        { 16459, 3209 }, { 16458, 3209 }, { 16457, 3209 }, { 16455, 3209 }, { 16456, 3209 },

        // ArmoryRings = 3300
        // Deltascape
        { 19494, 3300 }, { 19495, 3300 }, { 19496, 3300 }, { 19498, 3300 }, { 19497, 3300 },
        // Gordian
        { 11509, 3300 }, { 11508, 3300 }, { 11507, 3300 }, { 11505, 3300 }, { 11506, 3300 },
        // MIDAN Alexandrian
        { 16464, 3300 }, { 16463, 3300 }, { 16462, 3300 }, { 16460, 3300 }, { 16461, 3300 }
    };
    /*
    Example usage
    int itemId = 19437;
    if (ItemIdArmoryTable.TryGetValue(itemId, out int category))
    {
        Console.WriteLine($"Item ID {itemId} belongs to category {category}.");
    }
    else
    {
        Console.WriteLine("Item ID not found.");
    }
    */

    #endregion

    #region Omega Raid Tables

    public static int[,] GelfradusTable = new int[,]
    {
        {0, DeltascapeShaftID, ShaftBuyAmount, 19438, 3, 0}, // x4
        {0, DeltascapeShaftID, ShaftBuyAmount, 19444, 4, 0},
        {0, DeltascapeShaftID, ShaftBuyAmount, 19450, 5, 0},
        {0, DeltascapeSpringID, SpringBuyAmount, 19440, 9, 0}, // x4
        {0, DeltascapeSpringID, SpringBuyAmount, 19446, 10, 0},
        {0, DeltascapeSpringID, SpringBuyAmount, 19452, 11, 0},
        {0, DeltascapeCrankID, CrankBuyAmount, 19439, 6, 0}, // x2
        {0, DeltascapeCrankID, CrankBuyAmount, 19445, 7, 0},
        {0, DeltascapeCrankID, CrankBuyAmount, 19451, 8, 0},
        {0, DeltascapePedalID, PedalBuyAmount, 19441, 12, 0}, // x2
        {0, DeltascapePedalID, PedalBuyAmount, 19447, 13, 0},
        {0, DeltascapePedalID, PedalBuyAmount, 19453, 14, 0},
        {0, DeltascapeLensID, LensBuyAmount, 19437, 0, 0}, // x2
        {0, DeltascapeLensID, LensBuyAmount, 19443, 1, 0},
        {0, DeltascapeLensID, LensBuyAmount, 19449, 2, 0},
        {0, DeltascapeBoltID, BoltBuyAmount, 19479, 15, 0}, // x1
        {0, DeltascapeBoltID, BoltBuyAmount, 19480, 16, 0},
        {0, DeltascapeBoltID, BoltBuyAmount, 19484, 17, 0},
        {0, DeltascapeBoltID, BoltBuyAmount, 19485, 18, 0},
        {0, DeltascapeBoltID, BoltBuyAmount, 19489, 19, 0},
        {0, DeltascapeBoltID, BoltBuyAmount, 19490, 20, 0},
        {0, DeltascapeBoltID, BoltBuyAmount, 19494, 21, 0},
        {0, DeltascapeBoltID, BoltBuyAmount, 19495, 22, 0},

        {1, DeltascapeShaftID, ShaftBuyAmount, 19462, 2, 1},
        {1, DeltascapeShaftID, ShaftBuyAmount, 19456, 3, 1},
        {1, DeltascapeSpringID, SpringBuyAmount, 19464, 6, 1},
        {1, DeltascapeSpringID, SpringBuyAmount, 19458, 7, 1},
        {1, DeltascapeCrankID, CrankBuyAmount, 19463, 4, 1},
        {1, DeltascapeCrankID, CrankBuyAmount, 19457, 5, 1},
        {1, DeltascapePedalID, PedalBuyAmount, 19465, 8, 1},
        {1, DeltascapePedalID, PedalBuyAmount, 19459, 9, 1},
        {1, DeltascapeLensID, LensBuyAmount, 19461, 0, 1},
        {1, DeltascapeLensID, LensBuyAmount, 19455, 1, 1},
        {1, DeltascapeBoltID, BoltBuyAmount, 19481, 10, 1},
        {1, DeltascapeBoltID, BoltBuyAmount, 19486, 11, 1},
        {1, DeltascapeBoltID, BoltBuyAmount, 19491, 12, 1},
        {1, DeltascapeBoltID, BoltBuyAmount, 19496, 13, 1},

        {2, DeltascapeShaftID, ShaftBuyAmount, 19474, 2, 2},
        {2, DeltascapeShaftID, ShaftBuyAmount, 19468, 3, 2},
        {2, DeltascapeSpringID, SpringBuyAmount, 19476, 6, 2},
        {2, DeltascapeSpringID, SpringBuyAmount, 19470, 7, 2},
        {2, DeltascapeCrankID, CrankBuyAmount, 19475, 4, 2},
        {2, DeltascapeCrankID, CrankBuyAmount, 19469, 5, 2},
        {2, DeltascapePedalID, PedalBuyAmount, 19477, 8, 2},
        {2, DeltascapePedalID, PedalBuyAmount, 19471, 9, 2},
        {2, DeltascapeLensID, LensBuyAmount, 19473, 0, 2},
        {2, DeltascapeLensID, LensBuyAmount, 19467, 1, 2},
        {2, DeltascapeBoltID, BoltBuyAmount, 19483, 10, 2},
        {2, DeltascapeBoltID, BoltBuyAmount, 19482, 11, 2},
        {2, DeltascapeBoltID, BoltBuyAmount, 19488, 12, 2},
        {2, DeltascapeBoltID, BoltBuyAmount, 19487, 13, 2},
        {2, DeltascapeBoltID, BoltBuyAmount, 19493, 14, 2},
        {2, DeltascapeBoltID, BoltBuyAmount, 19492, 15, 2},
        {2, DeltascapeBoltID, BoltBuyAmount, 19498, 16, 2},
        {2, DeltascapeBoltID, BoltBuyAmount, 19497, 17, 2},
    };

    #endregion

    #region Alexander Raid Tables

    public static int[,] SabinaTable = new int[,]
    {
        {0, GordianShaftID, ShaftBuyAmount, 11455, 3, 0},
        {0, GordianShaftID, ShaftBuyAmount, 11456, 4, 0},
        {0, GordianShaftID, ShaftBuyAmount, 11457, 5, 0},
        {0, GordianSpringID, SpringBuyAmount, 11476, 9, 0},
        {0, GordianSpringID, SpringBuyAmount, 11477, 10, 0},
        {0, GordianSpringID, SpringBuyAmount, 11478, 11, 0},
        {0, GordianCrankID, CrankBuyAmount, 11462, 6, 0},
        {0, GordianCrankID, CrankBuyAmount, 11463, 7, 0},
        {0, GordianCrankID, CrankBuyAmount, 11464, 8, 0},
        {0, GordianPedalID, PedalBuyAmount, 11483, 12, 0},
        {0, GordianPedalID, PedalBuyAmount, 11484, 13, 0},
        {0, GordianPedalID, PedalBuyAmount, 11485, 14, 0},
        {0, GordianLensID, LensBuyAmount, 11448, 0, 0},
        {0, GordianLensID, LensBuyAmount, 11449, 1, 0},
        {0, GordianLensID, LensBuyAmount, 11450, 2, 0},
        {0, GordianBoltID, BoltBuyAmount, 11490, 15, 0},
        {0, GordianBoltID, BoltBuyAmount, 11491, 16, 0},
        {0, GordianBoltID, BoltBuyAmount, 11495, 17, 0},
        {0, GordianBoltID, BoltBuyAmount, 11496, 18, 0},
        {0, GordianBoltID, BoltBuyAmount, 11500, 19, 0},
        {0, GordianBoltID, BoltBuyAmount, 11501, 20, 0},
        {0, GordianBoltID, BoltBuyAmount, 11505, 21, 0},
        {0, GordianBoltID, BoltBuyAmount, 11506, 22, 0},

        {1, GordianShaftID, ShaftBuyAmount, 11459, 2, 0},
        {1, GordianShaftID, ShaftBuyAmount, 11458, 3, 0},
        {1, GordianSpringID, SpringBuyAmount, 11480, 6, 0},
        {1, GordianSpringID, SpringBuyAmount, 11479, 7, 0},
        {1, GordianCrankID, CrankBuyAmount, 11466, 4, 0},
        {1, GordianCrankID, CrankBuyAmount, 11465, 5, 0},
        {1, GordianPedalID, PedalBuyAmount, 11487, 8, 0},
        {1, GordianPedalID, PedalBuyAmount, 11486, 9, 0},
        {1, GordianLensID, LensBuyAmount, 11452, 0, 0},
        {1, GordianLensID, LensBuyAmount, 11451, 1, 0},
        {1, GordianBoltID, BoltBuyAmount, 11492, 10, 0},
        {1, GordianBoltID, BoltBuyAmount, 11497, 11, 0},
        {1, GordianBoltID, BoltBuyAmount, 11502, 12, 0},
        {1, GordianBoltID, BoltBuyAmount, 11507, 13, 0},

        {2, GordianShaftID, ShaftBuyAmount, 11461, 2, 0},
        {2, GordianShaftID, ShaftBuyAmount, 11460, 3, 0},
        {2, GordianSpringID, SpringBuyAmount, 11482, 6, 0},
        {2, GordianSpringID, SpringBuyAmount, 11481, 7, 0},
        {2, GordianCrankID, CrankBuyAmount, 11468, 4, 0},
        {2, GordianCrankID, CrankBuyAmount, 11467, 5, 0},
        {2, GordianPedalID, PedalBuyAmount, 11489, 8, 0},
        {2, GordianPedalID, PedalBuyAmount, 11488, 9, 0},
        {2, GordianLensID, LensBuyAmount, 11454, 0, 0},
        {2, GordianLensID, LensBuyAmount, 11453, 1, 0},
        {2, GordianBoltID, BoltBuyAmount, 11494, 10, 0},
        {2, GordianBoltID, BoltBuyAmount, 11493, 11, 0},
        {2, GordianBoltID, BoltBuyAmount, 11499, 12, 0},
        {2, GordianBoltID, BoltBuyAmount, 11498, 13, 0},
        {2, GordianBoltID, BoltBuyAmount, 11504, 14, 0},
        {2, GordianBoltID, BoltBuyAmount, 11503, 15, 0},
        {2, GordianBoltID, BoltBuyAmount, 11509, 16, 0},
        {2, GordianBoltID, BoltBuyAmount, 11508, 17, 0},

        {0, AlexandrianShaftID, ShaftBuyAmount, 16404, 3, 2},
        {0, AlexandrianShaftID, ShaftBuyAmount, 16410, 4, 2},
        {0, AlexandrianShaftID, ShaftBuyAmount, 16416, 5, 2},
        {0, AlexandrianSpringID, SpringBuyAmount, 16406, 9, 2},
        {0, AlexandrianSpringID, SpringBuyAmount, 16412, 10, 2},
        {0, AlexandrianSpringID, SpringBuyAmount, 16418, 11, 2},
        {0, AlexandrianCrankID, CrankBuyAmount, 16405, 6, 2},
        {0, AlexandrianCrankID, CrankBuyAmount, 16411, 7, 2},
        {0, AlexandrianCrankID, CrankBuyAmount, 16417, 8, 2},
        {0, AlexandrianPedalID, PedalBuyAmount, 16407, 12, 2},
        {0, AlexandrianPedalID, PedalBuyAmount, 16413, 13, 2},
        {0, AlexandrianPedalID, PedalBuyAmount, 16419, 14, 2},
        {0, AlexandrianLensID, LensBuyAmount, 16403, 0, 2},
        {0, AlexandrianLensID, LensBuyAmount, 16409, 1, 2},
        {0, AlexandrianLensID, LensBuyAmount, 16415, 2, 2},
        {0, AlexandrianBoltID, BoltBuyAmount, 16445, 15, 2},
        {0, AlexandrianBoltID, BoltBuyAmount, 16446, 16, 2},
        {0, AlexandrianBoltID, BoltBuyAmount, 16450, 17, 2},
        {0, AlexandrianBoltID, BoltBuyAmount, 16451, 18, 2},
        {0, AlexandrianBoltID, BoltBuyAmount, 16455, 19, 2},
        {0, AlexandrianBoltID, BoltBuyAmount, 16456, 20, 2},
        {0, AlexandrianBoltID, BoltBuyAmount, 16460, 21, 2},
        {0, AlexandrianBoltID, BoltBuyAmount, 16461, 22, 2},

        {1, AlexandrianShaftID, ShaftBuyAmount, 16428, 2, 2},
        {1, AlexandrianShaftID, ShaftBuyAmount, 16422, 3, 2},
        {1, AlexandrianSpringID, SpringBuyAmount, 16430, 6, 2},
        {1, AlexandrianSpringID, SpringBuyAmount, 16424, 7, 2},
        {1, AlexandrianCrankID, CrankBuyAmount, 16429, 4, 2},
        {1, AlexandrianCrankID, CrankBuyAmount, 16423, 5, 2},
        {1, AlexandrianPedalID, PedalBuyAmount, 16431, 8, 2},
        {1, AlexandrianPedalID, PedalBuyAmount, 16425, 9, 2},
        {1, AlexandrianLensID, LensBuyAmount, 16427, 0, 2},
        {1, AlexandrianLensID, LensBuyAmount, 16421, 1, 2},
        {1, AlexandrianBoltID, BoltBuyAmount, 16447, 10, 2},
        {1, AlexandrianBoltID, BoltBuyAmount, 16452, 11, 2},
        {1, AlexandrianBoltID, BoltBuyAmount, 16457, 12, 2},
        {1, AlexandrianBoltID, BoltBuyAmount, 16462, 13, 2},

        {2, AlexandrianShaftID, ShaftBuyAmount, 16440, 2, 2},
        {2, AlexandrianShaftID, ShaftBuyAmount, 16434, 3, 2},
        {2, AlexandrianSpringID, SpringBuyAmount, 16442, 6, 2},
        {2, AlexandrianSpringID, SpringBuyAmount, 16436, 7, 2},
        {2, AlexandrianCrankID, CrankBuyAmount, 16441, 4, 2},
        {2, AlexandrianCrankID, CrankBuyAmount, 16435, 5, 2},
        {2, AlexandrianPedalID, PedalBuyAmount, 16443, 8, 2},
        {2, AlexandrianPedalID, PedalBuyAmount, 16437, 9, 2},
        {2, AlexandrianLensID, LensBuyAmount, 16439, 0, 2},
        {2, AlexandrianLensID, LensBuyAmount, 16433, 1, 2},
        {2, AlexandrianBoltID, BoltBuyAmount, 16449, 10, 2},
        {2, AlexandrianBoltID, BoltBuyAmount, 16448, 11, 2},
        {2, AlexandrianBoltID, BoltBuyAmount, 16454, 12, 2},
        {2, AlexandrianBoltID, BoltBuyAmount, 16453, 13, 2},
        {2, AlexandrianBoltID, BoltBuyAmount, 16459, 14, 2},
        {2, AlexandrianBoltID, BoltBuyAmount, 16458, 15, 2},
        {2, AlexandrianBoltID, BoltBuyAmount, 16464, 16, 2},
        {2, AlexandrianBoltID, BoltBuyAmount, 16463, 17, 2},
    };

    #endregion

    #region Vendor Sell Dictionary

    public class VendorSellData
    {
        public int CurrentItemCount { get; set; }
        public int AvailableArmory { get; set; }
    }

    public static string AltBossMod = "BossModReborn";

    public static Dictionary<int, VendorSellData> VendorSellDict = new Dictionary<int, VendorSellData>
    {
        // Tarnished Gordian Item IDs
        { GordianLensID, new VendorSellData { CurrentItemCount = GetItemCount(GordianLensID) } },
        { GordianShaftID, new VendorSellData { CurrentItemCount = GetItemCount(GordianShaftID) } },
        { GordianCrankID, new VendorSellData { CurrentItemCount = GetItemCount(GordianCrankID) } },
        { GordianSpringID, new VendorSellData { CurrentItemCount = GetItemCount(GordianSpringID) } },
        { GordianPedalID, new VendorSellData { CurrentItemCount = GetItemCount(GordianPedalID) } },
        { GordianBoltID, new VendorSellData { CurrentItemCount = GetItemCount(GordianBoltID) } },

        // Deltascape Item IDs
        { DeltascapeLensID, new VendorSellData { CurrentItemCount = GetItemCount(DeltascapeLensID) } },
        { DeltascapeShaftID, new VendorSellData { CurrentItemCount = GetItemCount(DeltascapeShaftID) } },
        { DeltascapeCrankID, new VendorSellData { CurrentItemCount = GetItemCount(DeltascapeCrankID) } },
        { DeltascapeSpringID, new VendorSellData { CurrentItemCount = GetItemCount(DeltascapeSpringID) } },
        { DeltascapePedalID, new VendorSellData { CurrentItemCount = GetItemCount(DeltascapePedalID) } },
        { DeltascapeBoltID, new VendorSellData { CurrentItemCount = GetItemCount(DeltascapeBoltID) } },

        // Alexandrian Item IDs
        { AlexandrianLensID, new VendorSellData { CurrentItemCount = GetItemCount(AlexandrianLensID) } },
        { AlexandrianShaftID, new VendorSellData { CurrentItemCount = GetItemCount(AlexandrianShaftID) } },
        { AlexandrianCrankID, new VendorSellData { CurrentItemCount = GetItemCount(AlexandrianCrankID) } },
        { AlexandrianSpringID, new VendorSellData { CurrentItemCount = GetItemCount(AlexandrianSpringID) } },
        { AlexandrianPedalID, new VendorSellData { CurrentItemCount = GetItemCount(AlexandrianPedalID) } },
        { AlexandrianBoltID, new VendorSellData { CurrentItemCount = GetItemCount(AlexandrianBoltID) } }
    };

    // tempItem = VendorSellDict[12675].CurrentItemCount
    // 200
    // go through buying process
    // subtract how many item you buy from slot
    // VendorSellDict[12675].CurrentItemCount = tempItem - itemBuyAmount
    public static int[] RaidItemIDs =
    {
        GordianLensID, GordianShaftID, GordianCrankID, GordianSpringID, GordianPedalID, GordianBoltID,
        DeltascapeLensID, DeltascapeShaftID, DeltascapeCrankID, DeltascapeSpringID, DeltascapePedalID, DeltascapeBoltID,
        AlexandrianLensID , AlexandrianShaftID, AlexandrianCrankID , AlexandrianSpringID , AlexandrianPedalID , AlexandrianBoltID
    };

    #endregion

    #region Normal Raid Data Points

    // Alexander - The Burder of the Father (A4N) 
    public static uint A4NMapID = 445;
    public static ulong RightForeleg = 4107;
    public static ulong LeftForeleg = 4108;
    public static ulong Manipulator = 3902;
    public static ulong A4NChest1 = 438;
    public static ulong A4NChest2 = 480;
    public static ulong A4NChest3 = 479;
    public static Vector3 RightForeLegPos = new(-6.53f, 10.54f, -7.56f);
    public static Vector3 A4NChestCenter = new Vector3(-0.08f, 10.6f, -6.46f);

    // Deltascape V3.0 (O3N)
    public static uint O3NMapID = 693;
    public static ulong Halicarnassus = 7700;
    public static ulong O3NChest1 = 930;
    public static ulong O3NChest2 = 929;
    public static Vector3 O3NChestCenter = new Vector3(-0.01f, 0.00f, -5.42f);

    // Retainer
    public static ulong SummoningBell = 2000403;

    // TurnIn Shop
    public static ulong Sabina = 1012227;
    public static ulong Gelfradus = 1019452;

    #endregion

    #region Zone Info

    #region Main Cities
    // City -> Inn -> ReapirNPC -> 
    public static uint LimsaUpper = 128; // we need limsa id too which is 129 for the teleport task :d
    public static uint LimsaLower = 129;
    public static uint LimsaAether = 8;
    public static uint LimsaInn = 177;
    public static ulong LimsaInnNPC = 1000974;
    public static Vector3 LimsaInnNPCPos = new(15.43f, 40f, 12.47f);
    public static ulong LimsaRepairNPC = 1003251;
    public static Vector3 LimsaRepairNPCPos = new(17.72f, 40.2f, 3.95f);
    public static ulong LimsaInnDoor = 2001010;
    public static Vector3 LimsaInnDoorPos = new(-0.02f, 1.45f, 7.52f);

    public static uint UlDah = 130;
    public static uint UlDahAether = 9;
    public static uint UlDahInn = 178;
    public static ulong UlDahInnNPC = 1001976;
    public static Vector3 UlDahInnNPCPos = new(29.5f, 7.45f, -78.32f);
    public static ulong UlDahRepairNPC = 1004416;
    public static Vector3 UlDahRepairNPCPos = new(32.85f, 7f, -81.32f);
    public static ulong UlDahInnDoor = 2001011;
    public static Vector3 UlDahInnDoorPos = new(0.02f, 1.97f, 8.13f);

    public static uint Gridania = 132;
    public static uint GridaniaAether = 2;
    public static uint GridaniaInn = 179;
    public static ulong GridaniaInnNPC = 1000102;
    public static Vector3 GridaniaInnNPCPos = new(23.7f, -8.1f, 100.05f);
    public static ulong GridaniaRepairNPC = 1000394;
    public static Vector3 GridaniaRepairNPCPos = new(24.83f, -8f, 93.19f);
    public static ulong GridaniaInnDoor = 2000087;
    public static Vector3 GridaniaInnDoorPos = new(0.02f, 1.45f, 7.71f);

    public static uint Ishguard = 418;
    public static uint IshguardAether = 70;
    public static uint IshguardInn = 429;

    public static uint Kugane = 628;
    public static uint KuganeAether = 111;
    public static uint KuganeInn = 629;

    public static uint Crystarium = 819;
    public static uint CrystariumAether = 133;
    public static uint CrystariumInn = 843;

    public static uint OldShar = 962;
    public static uint OldSharAether = 182;
    public static uint OldSharInn = 990;

    public static uint Tuliyollai = 1185;
    public static uint TuliyollaAether = 216;
    public static uint TuliyollaiInn = 1205;
    #endregion

    // Turnin Locations
    public static uint Idyllshire = 478;
    public static uint IdyllshireAether = 75;
    public static Vector3 IdyllshireNPCPos = new(-20.98f, 211.00f, -37.74f);
    public static Vector3 IdyllshireNPCPos1 = new(-19.43f, 211.00f, -35.29f);
    public static Vector3 IdyllshireNPCPos2 = new(-18.19f, 211.00f, -36.49f);
    public static Vector3 IdyllshireNPCPos3 = new(-18.12f, 211.00f, -34.53f);
    public static Vector3 IdyllshireBellPos = new(34.78f, 208.15f, -50.86f); // Actual bell pos
    public static Vector3 IdyllshireBellPos1 = new(35.37f, 208.17f, -51.95f); //triangle values
    public static Vector3 IdyllshireBellPos2 = new(33.41f, 208.24f, -50.55f); //triangle values
    public static Vector3 IdyllshireBellPos3 = new(33.46f, 208.36f, -52.60f); //triangle values

    public static uint Rhalgr = 635;
    public static uint RhalgrAether = 104;
    public static Vector3 RhalgrNPCPos = new Vector3(128.40f, 0.68f, 41.70f);
    public static Vector3 RhalgrNPCPos1 = new Vector3(125.41f, 0.65f, 39.55f);
    public static Vector3 RhalgrNPCPos2 = new Vector3(124.71f, 0.65f, 41.31f);
    public static Vector3 RhalgrNPCPos3 = new Vector3(123.59f, 0.65f, 39.49f);
    public static Vector3 RhalgrBellPos = new Vector3(-57.27f, 0f, 48.57f); // Actual bell pos
    public static Vector3 RhalgrBellPos1 = new Vector3(-53.83f, 0f, 46.92f);
    public static Vector3 RhalgrBellPos2 = new Vector3(-55.33f, 0f, 48.68f);
    public static Vector3 RhalgrBellPos3 = new Vector3(-57.03f, 0f, 47.42f);

    public static readonly HashSet<uint> innZones = new HashSet<uint> { LimsaInn, UlDahInn, GridaniaInn, IshguardInn, KuganeInn, CrystariumInn, OldSharInn, TuliyollaiInn };

    #endregion

    #region Grand Company Info

    public static readonly Dictionary<GrandCompany, Vector3> CompanyNPCPoints = new()
    {
        [GrandCompany.ImmortalFlames] = new Vector3(-140.6f, 4.1f, -105.6f),
        [GrandCompany.Maelstrom] = new Vector3(93.0f, 40.3f, 75.6f),
        [GrandCompany.TwinAdder] = new Vector3(-67.2f, -0.5f, -7.8f),
    };

    //21069	Maelstrom aetheryte ticket
    //21070	Twin Adder aetheryte ticket
    //21071	Immortal Flames aetheryte ticket
    public static readonly Dictionary<GrandCompany, uint> CompanyItem = new()
    {
        [GrandCompany.ImmortalFlames] = 21071,
        [GrandCompany.Maelstrom] = 21069,
        [GrandCompany.TwinAdder] = 21070,
    };

    #endregion

    #region Armor/Gil Calculator

    public static double TotalFCPoints = 0;
    public static int TotalGCBase = 0;
    public static int TotalGC10 = 0;
    public static int TotalGC15 = 0;
    public static int OilclothBase = 0;
    public static int Oilcloth10 = 0;
    public static int Oilcloth15 = 0;
    public static int VendorGil = 0;
    public static int OilclothBuy = 600;
    public static int OilclothSell = 360;
    public static double FCPointCalc = 1.5;

    public static int TotalA4NGear = 0;
    public static int A4NJewelry = 0;
    public static int A4NHead = 0;
    public static int A4NHand = 0;
    public static int A4NShoes = 0;
    public static int A4NBody = 0;
    public static int A4NLeg = 0;
    public static int A4NBodySell = 978;
    public static int A4NLegSell = 978;
    public static int A4NShoeSell = 587;
    public static int A4NHandSell = 587;
    public static int A4NJewelrySell = 445;
    public static int A4NiLvl = 190;
    public static int A4NSealBase = 1093;
    public static int A4NSeal10 = 1203;
    public static int A4NSeal15 = 1257;

    /*
    public static int TotalA12NGear = 0;
    public static int A12NJewelry = 0;
    public static int A12NHand = 0;
    public static int A12NShoes = 0;
    public static int A12NBody = 0;
    public static int A12NLeg = 0;
    public static int A12NBodySell = 978;
    public static int A12NLegSell = 978;
    public static int A12NShoeSell = 587;
    public static int A12NHandSell = 587;
    public static int A12NJewelrySell = 445;
    public static int A12NiLvl = 190;
    public static int A12NSealBase = 1093;
    public static int A12NSeal10 = 1203;
    public static int A12NSeal15 = 1257;
    */

    public static int TotalO3NGear = 0;
    public static int O3NJewelry = 0;
    public static int O3NHand = 0;
    public static int O3NShoes = 0;
    public static int O3NBody = 0;
    public static int O3NLeg = 0;
    public static int O3NHead = 0;
    public static int O3NBodySell = 1493;
    public static int O3NLegSell = 1493;
    public static int O3NHeadSell = 896;
    public static int O3NShoeSell = 896;
    public static int O3NHandSell = 896;
    public static int O3NJewelrySell = 679;
    public static int O3NiLvl = 320;
    public static int O3NSealBase = 1390;
    public static int O3NSeal10 = 1529;
    public static int O3NSeal15 = 1598;

    public static int TotalJewelry = 0;
    public static int TotalHead = 0;
    public static int TotalBody = 0;
    public static int TotalHand = 0;
    public static int TotalLeg = 0;
    public static int TotalShoes = 0;
    public static int TotalGear = 0;
    #endregion

    #region Inn Dictionaries

    public class InnData
    {
        public required uint MainCity { get; set; }
        public ulong RepairNPC { get; set; }
        public Vector3 RepairNPCPos { get; set; }
        public required ulong InnNPC { get; set; }
        public required Vector3 InnNPCPos { get; set; }
        public required ulong InnDoor { get; set; }
        public required Vector3 InnDoorPos { get; set; }
        public uint MainCity2 { get; set; }
        public uint MainAether { get; set; }
    }

    public static Dictionary<uint, InnData> InnDict = new Dictionary<uint, InnData>
    {
        {LimsaInn, 
            new InnData{ 
                MainCity = LimsaLower, 
                MainAether = LimsaAether,
                RepairNPC = LimsaRepairNPC, 
                RepairNPCPos = LimsaRepairNPCPos, 
                InnNPC = LimsaInnNPC, 
                InnNPCPos = LimsaInnNPCPos,
                InnDoor = LimsaInnDoor, 
                MainCity2 = LimsaUpper,
                InnDoorPos=LimsaInnDoorPos 
            } 
        },
        {UlDahInn, 
            new InnData{ 
                MainCity = UlDah, 
                MainAether = UlDahAether,
                RepairNPC = UlDahRepairNPC, 
                RepairNPCPos = UlDahRepairNPCPos, 
                InnNPC = UlDahInnNPC, 
                InnNPCPos = UlDahInnNPCPos,
                InnDoor = UlDahInnDoor,
                InnDoorPos=UlDahInnDoorPos 
            }
        },
        {GridaniaInn, 
            new InnData{
                MainCity = Gridania, 
                MainAether = GridaniaAether, 
                RepairNPC = GridaniaRepairNPC, 
                RepairNPCPos = GridaniaRepairNPCPos, 
                InnNPC = GridaniaInnNPC, 
                InnNPCPos = GridaniaInnNPCPos,
                InnDoor = GridaniaInnDoor,
                InnDoorPos=GridaniaInnDoorPos
            } 
        },
    };

    #endregion

    // variables to be used across the place
    public static bool RunInfinite = true;
    public static int RunAmount = 1;

    public static uint[] Workshops = [Houses.Company_Workshop_Empyreum, Houses.Company_Workshop_The_Goblet, Houses.Company_Workshop_Mist, Houses.Company_Workshop_Shirogane, Houses.Company_Workshop_The_Lavender_Beds];
    public static uint[] PrivateHouses = [
        Houses.Private_Cottage_Mist,
        Houses.Private_House_Mist,
        Houses.Private_Mansion_Mist,
        Houses.Private_Cottage_The_Lavender_Beds,
        Houses.Private_House_The_Lavender_Beds,
        Houses.Private_Mansion_The_Lavender_Beds,
        Houses.Private_Cottage_The_Goblet,
        Houses.Private_House_The_Goblet,
        Houses.Private_Mansion_The_Goblet,
        Houses.Private_Cottage_Shirogane,
        Houses.Private_House_Shirogane,
        Houses.Private_Mansion_Shirogane,
        Houses.Private_Cottage_Empyreum,
        Houses.Private_House_Empyreum,
        Houses.Private_Mansion_Empyreum,
        Houses.Private_Cottage_Minimalist,
        Houses.Private_House_Minimalist,
        Houses.Private_Mansion_Minimalist,
        ];

    #region Turnin Dictionaries

    public class TurnInData
    {
        public ulong TurnInNpc { get; set; }
        public Vector3 NpcPos { get; set; }
        public Vector3 NpcPos1 { get; set; }
        public Vector3 NpcPos2 { get; set; }
        public Vector3 NpcPos3 { get; set; }
        public Vector3 BellPos { get; set; }
        public Vector3 BellPos1 { get; set; }
        public Vector3 BellPos2 { get; set; }
        public Vector3 BellPos3 { get; set; }
    }
    public static Dictionary<uint, TurnInData> TurnInDict = new Dictionary<uint, TurnInData>
    {
        {Idyllshire, 
            new TurnInData{
                TurnInNpc = Sabina,
                NpcPos = IdyllshireNPCPos,
                NpcPos1 = IdyllshireNPCPos1,
                NpcPos2 = IdyllshireNPCPos2,
                NpcPos3 = IdyllshireNPCPos3,
                BellPos = IdyllshireBellPos,
                BellPos1 = IdyllshireBellPos1,
                BellPos2 = IdyllshireBellPos2,
                BellPos3 = IdyllshireBellPos3
            }
        },
        {Rhalgr, 
            new TurnInData{
                TurnInNpc = Gelfradus,
                NpcPos = RhalgrNPCPos,
                NpcPos1 = RhalgrNPCPos1,
                NpcPos2 = RhalgrNPCPos2,
                NpcPos3 = RhalgrNPCPos3,
                BellPos = RhalgrBellPos,
                BellPos1 = RhalgrBellPos1,
                BellPos2 = RhalgrBellPos2,
                BellPos3 = RhalgrBellPos3
            } 
        },
    };

    #endregion

    #region Normal Raid Dictionaries

    public class NRaidData
    {
        public required Vector3 CenterofChest {get; set; }
        public required ulong[] ListofChest { get; set; }
        public required ulong BossID { get; set; }
        public required uint DutyID { get; set; }
        public required int PcallID { get; set; }
    }

    public static Dictionary<uint, NRaidData> NRaidDict = new Dictionary<uint, NRaidData>
    {
        { A4NMapID, // A4N Selected
            new NRaidData
            {
                CenterofChest = A4NChestCenter,
                ListofChest = [A4NChest1, A4NChest2, A4NChest3],
                BossID = RightForeleg,
                DutyID = 115,
                PcallID = C.A4NPcallValue
            }
        },
        { O3NMapID, // O3N
            new NRaidData
            {
                CenterofChest = O3NChestCenter,
                ListofChest = [O3NChest1, O3NChest2],
                BossID = Halicarnassus,
                DutyID = 254, // was set to 0 originally
                PcallID = C.O3NPcallValue
            }
        }
    };

    #endregion
}
