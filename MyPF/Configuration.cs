using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace SamplePlugin;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public string? PartyFinderHostName { get; set; } = string.Empty;

    public bool NotifyRefresh { get; set; } = false;    

    // the below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
