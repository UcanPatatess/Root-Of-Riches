using ECommons.Configuration;
using System.Text.Json.Serialization;

namespace OptimizedChocoboColoring;


public class Config : IEzConfig
{
    [JsonIgnore]
    public const int CURRENT_CONFIG_VERSION = 4;
    public int Version = CURRENT_CONFIG_VERSION;

    public bool TeleportToFC { get; set; } = false;
    public bool MaxItem { get; set; } = false;
    public void Save()
    {
        EzConfig.Save();
    }
}

