using Dalamud.Game.Gui.PartyFinder.Types;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Utility;
using SamplePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPF.Services
{
    internal class PartyFinderListingListener
    {
        private readonly Configuration configuration;

        public PartyFinderListingListener(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public void Attach()
        {
            Plugin.PartyFinderGui.ReceiveListing += OnListing;
        }
        public void Dispose()
        {
            Plugin.PartyFinderGui.ReceiveListing -= OnListing;
        }

        private void OnListing(IPartyFinderListing listing, IPartyFinderListingEventArgs args)
        {
            uint listingId = listing.Id;
            bool isCrossWorld = listing.SearchArea.HasFlag(SearchAreaFlags.DataCentre)
                && !listing.SearchArea.HasFlag(SearchAreaFlags.World);
            string hostName = listing.Name.ToString();
            string hostWorld = listing.World.ValueNullable?.Name.ToString() ?? "Unknown world";
#if DEBUG
            Plugin.Log.Info($"PF Id {listingId} CW {isCrossWorld} Host {hostName}");
#endif
            if (GetChosenHostName().Equals(hostName, StringComparison.OrdinalIgnoreCase))
            {
                bool isNew = ChatReader.UpdatePartyFinderSavedInfo(listingId, hostName, hostWorld, isCrossWorld);
                if (isNew)
                {
                    Plugin.ChatGui.Print($"PF listing for {hostName} saved.");
                }
                else if (configuration.NotifyRefresh)
                {
                    Plugin.ChatGui.Print($"PF listing for {hostName} refreshed.");
                }
            }
        }

        private string GetChosenHostName()
        {
            if (!configuration.PartyFinderHost.IsNullOrWhitespace())
            {
                return configuration.PartyFinderHost;
            }

            return Plugin.ClientState.LocalPlayer?.Name.ToString() ?? string.Empty;
        }
    }
}
