using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Lumina.Excel.Sheets;
using MyPF.Model;
using MyPF.Services;
using System;
using System.Numerics;

namespace SamplePlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private static readonly Vector4 LightGreen = new Vector4(156f / 255, 0, 143f / 255, 1);
    private static readonly Vector4 DeepBlue = new Vector4(0, 0, 66 / 255, 1);
    private Plugin Plugin;

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin)
        : base("MyPF", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        Plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        SavedListingInfo? savedListing = ChatReader.GetSavedListing;

        ImGui.TextColored(LightGreen, "Saved Party Finder listing:");
        ImGui.Separator();
        using (var child = ImRaii.Child("Listing"))
        {
            if (child.Success)
            {
                DrawSavedListing(savedListing);
            }            
        }
            

        if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Wrench, "Open settings", DeepBlue))
        {
            Plugin.ToggleConfigUI();
        }       
    }

    private void DrawSavedListing(SavedListingInfo? savedListing)
    {
        if (savedListing == null)
        {
            ImGui.TextUnformatted("None yet.");
            return;
        }

        ImGui.TextUnformatted("Listing ID: "); ImGui.SameLine(); ImGui.TextColored(LightGreen, savedListing.ListingId.ToString());
        ImGui.TextUnformatted("Host Player: "); ImGui.SameLine(); ImGui.TextColored(LightGreen, savedListing.HostName);
        ImGui.TextUnformatted("World: "); ImGui.SameLine(); ImGui.TextColored(LightGreen, savedListing.WorldName);
        ImGui.TextUnformatted("Cross-world?: "); ImGui.SameLine(); ImGui.TextColored(LightGreen, savedListing.IsCrossWorld ? "Yes" : "No");
    }
}
