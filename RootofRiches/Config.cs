using ECommons.Configuration;
using System.Text.Json.Serialization;

namespace RootofRiches;


public class Config : IEzConfig
{
    [JsonIgnore]
    public const int CURRENT_CONFIG_VERSION = 4;
    public int Version = CURRENT_CONFIG_VERSION;

    public bool TeleportToFC { get; set; } = false;
    public bool MaxItem { get; set; } = false;
    public bool MaxArmory { get; set; } = false;
    public int MaxArmoryFreeSlot { get; set; } = 2;
    public bool VendorTurnIn { get; set; } = false;
    public bool SellOilCloth { get; set; } = false;
    public bool ChangeArmory { get; set; } = false;
    public bool EnableAutoRetainer { get; set; } = false;
    public bool EnableMountUp { get; set; } = true;
    public int A4NPcallValue { get; set; } = 0;
    public int O3NPcallValue { get; set; } = 0;
    public float RepairSlider { get; set; } = 30f;
    public bool EnableReturnInn { get; set; } = true;
    public bool EnableSubsMain { get; set; } = false;
    public string RaidOption { get; set; } = "Infinite";
    public uint InnDataID { get; set; } = 177;
    public string InnOption { get; set; } = "Limsa";
    public bool ShowSettingsInWindow { get; set; } = false;
    public int RaidSelected { get; set; } = 0;

    public string RepairMode { get; set; } = "Self Repair";
    public string RepairOption { get; set; } = "Self Repair";
    public bool EnableRepair { get; set; } = true;

    public Stats Stats { get; set; } = new Stats();
    public Stats SessionStats { get; set; } = new Stats();
    public bool HasUpdatedStats = false;
    public void UpdateStats(Action<Stats> updateAction)
    {
        updateAction(Stats);
        updateAction(SessionStats);
    }
    public void Save()
    {
        EzConfig.Save();
    }
}

