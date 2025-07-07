using Dalamud.Interface.Components;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Numerics;

namespace SamplePlugin.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;
    private static readonly Vector4 LightGreen = new Vector4(0, 199f / 255, 66f / 255, 1);

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("MyPF Configuration")
    {
        Flags = ImGuiWindowFlags.NoCollapse;

        Size = new Vector2(500, 200);
        SizeCondition = ImGuiCond.Always;

        Configuration = plugin.Configuration;
    }

    public void Dispose() { }


    public override void Draw()
    {        
        var hostName = Configuration.PartyFinderHostName;
        ImGui.TextColored(LightGreen, "Party Finder host player name:");
        ImGui.TextUnformatted("Leave this empty if you are hosting the PF you want to link");
        if (ImGui.InputTextWithHint(string.Empty, "John Finalfantasyxiv", ref hostName, 25))
        {
            Configuration.PartyFinderHostName = hostName;
            Configuration.Save();
        }
        ImGui.SameLine();
        ImGuiComponents.HelpMarker("MyPF will store the listing hosted by this player. Write it here if you're not the host");

        ImGui.Separator();
        var notifyUpdate = Configuration.NotifyRefresh;
        if (ImGui.Checkbox("Write to chat when the saved listing is refreshed", ref notifyUpdate))
        {
            Configuration.NotifyRefresh = notifyUpdate;
            Configuration.Save();
        }
        ImGui.Separator();
        ImGui.TextUnformatted("PF link placeholder: ");
        ImGui.SameLine();
        ImGui.TextColored(LightGreen, "<mypf>");
        ImGuiComponents.HelpMarker("Not a setting, just a reminder.");
    }
}
