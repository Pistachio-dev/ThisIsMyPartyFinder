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
    private static readonly Vector4 LightGreen = new Vector4(0, 199f / 255, 66f / 255, 1);
    private static readonly Vector4 DeepBlue = new Vector4(0, 0, 150 / 255, 1);
    private static readonly Vector4 Yellow = new Vector4(1, 1, 0, 1);
    private Plugin Plugin;

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin)
        : base("MyPF", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(500, 430),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        Plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.TextUnformatted("This plugin saves your PF listing, and replaces");
        ImGui.SameLine();
        ImGui.TextColored(LightGreen, "<mypf>");
        ImGui.SameLine();
        ImGui.TextUnformatted("with a link to it.");
        ImGui.TextUnformatted("For instance:");
        ImGui.TextColored(Yellow, "Blackjack open! <mypf> Password: 7777");
        ImGui.TextUnformatted("Turns to:");
        ImGui.TextColored(Yellow, "Blackjack open! Looking for Party (John Fantasyxiv) Password: 7777");
        SavedListingInfo? savedListing = ChatReader.GetSavedListing;
        ImGui.Separator();
        ImGui.TextColored(LightGreen, "Saved Party Finder listing:");        
        DrawSavedListing(savedListing);
        ImGui.Spacing();    
        if (ImGuiComponents.IconButtonWithText(FontAwesomeIcon.Wrench, "Open settings"))
        {
            Plugin.ToggleConfigUI();
        }       
    }

    private void DrawSavedListing(SavedListingInfo? savedListing)
    {
        if (savedListing == null)
        {
            ImGui.TextUnformatted("None yet. Reopen Party Finder to refresh.");
            return;
        }

        ImGui.TextUnformatted("Listing ID: "); ImGui.SameLine(); ImGui.TextColored(LightGreen, savedListing.ListingId.ToString());
        ImGui.TextUnformatted("Host Player: "); ImGui.SameLine(); ImGui.TextColored(LightGreen, savedListing.HostName);
        ImGui.TextUnformatted("World: "); ImGui.SameLine(); ImGui.TextColored(LightGreen, savedListing.WorldName);
        ImGui.TextUnformatted("Cross-world?: "); ImGui.SameLine(); ImGui.TextColored(LightGreen, savedListing.IsCrossWorld ? "Yes" : "No");
    }
}
