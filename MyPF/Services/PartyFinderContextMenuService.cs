using Dalamud.Game.Gui.ContextMenu;
using Dalamud.Game.Gui.PartyFinder.Types;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using SamplePlugin;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MyPF.Services
{
    internal static class PartyFinderContextMenuService
    {
        private const string PartyFinderAddonName = "LookingForGroup";
        public static void AddPartyFinderSaving()
        {
            Plugin.ContextMenu.OnMenuOpened += AddPFSavingToContextMenu;
        }

        public static void Dispose()
        {
            Plugin.ContextMenu.OnMenuOpened -= AddPFSavingToContextMenu;
        }

        private static void AddPFSavingToContextMenu(IMenuOpenedArgs args)
        {
            // Can't finf what I need here. Either search another example, or get it from, maybe, a first relay.
            if (args.AddonName != PartyFinderAddonName) return;
            if (args.Target is MenuTargetDefault def)
            {
                args.AddMenuItem(new MenuItem()
                {
                    OnClicked = (arg) =>
                    {

                        uint contentId = def.TargetObject?.EntityId ?? 0;
                        string playerNameNoWorld = def.TargetCharacter?.Name ?? "Someone very cool";
                        var subObjectId = 0;
                        Plugin.Log.Info("Content Id: " + contentId.ToString());
                        Plugin.Log.Info("Object id: " + def.TargetObjectId);
                        Plugin.Log.Info("Object id: " + def.TargetHomeWorld);
                        Plugin.Log.Info("Player name" + playerNameNoWorld);
                        Plugin.Log.Info("Target name: " + def.TargetName);
                        Plugin.Log.Info("Target object name: " + def.TargetObject?.Name ?? "null");
                        Plugin.Log.Info("SubAddonName " + arg.AddonName);
                        Plugin.Log.Info("Sub Menu type: " + arg.MenuType.ToString());
                        if (arg.Target is MenuTargetDefault subDef)
                        {
                            Plugin.Log.Info("Sub content id: " + subDef.TargetContentId);
                            Plugin.Log.Info("Sub object id: " + subDef.TargetObjectId);
                            Plugin.Log.Info("Sub homeworld id: " + subDef.TargetHomeWorld);
                            Plugin.Log.Info("Sub-content name: " + subDef.TargetName);
                        }
                        

                    },
                    Name = new SeStringBuilder().Append("Save party finder for <mypf>").BuiltString,
                    Prefix = SeIconChar.BoxedLetterM,
                });
            }

        }
    }
}
