using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using MyPF.Model;
using SamplePlugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyPF.Services
{
    public static class ChatReader
    {
        private const string DefaultHostPlayer = "Place Holder6888a89f-4c13-4ed1-bc42-5d7e20466d2c";
        public const string ReplacementToken = "<mypf>";
        private static uint PFListingId = 0;
        private static bool IsCrossWorld = true;
        private static string PFHostPlayer = DefaultHostPlayer;
        private static string PFHostWorld = "Unknown World";
        private static DateTime SavedAt = DateTime.MinValue;
        private static DateTime LasRefreshedAt = DateTime.MinValue;

        internal static SavedListingInfo? GetSavedListing => PFHostPlayer == DefaultHostPlayer 
            ? null
            : new SavedListingInfo(PFListingId, PFHostPlayer, PFHostWorld, IsCrossWorld, SavedAt, LasRefreshedAt);

        public static void ForgetListing()
        {
            PFHostPlayer = DefaultHostPlayer;
        }

        public static bool UpdatePartyFinderSavedInfo(uint listingId, string hostPlayerNoWorld, string hostWorld, bool isCrossworld)
        {
            bool isNew = PFListingId != listingId
                || PFHostPlayer != hostPlayerNoWorld
                || IsCrossWorld != isCrossworld
                || PFHostWorld != hostWorld;
            PFListingId = listingId;
            PFHostPlayer = hostPlayerNoWorld;
            PFHostWorld = hostWorld;
            IsCrossWorld = isCrossworld;
            LasRefreshedAt = DateTime.UtcNow;
            if (isNew)
            {
                SavedAt = DateTime.UtcNow;
            }

            return isNew;
        }

        public static void Attach()
        {
            Plugin.ChatGui.ChatMessage += SplicePFLink;
        }

        public static void Dispose()
        {
            Plugin.ChatGui.ChatMessage -= SplicePFLink;
        }

        private static void SplicePFLink(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (sender.GetSenderFullName(Plugin.ClientState) != (Plugin.ClientState.LocalPlayer?.GetFullName() ?? string.Empty))
            {
                return;
            }            
            Payload? payloadWithMark = message.Payloads.FirstOrDefault(p => p is ITextProvider textPayload && textPayload.Text.Contains(ReplacementToken, StringComparison.OrdinalIgnoreCase));
            if (payloadWithMark == null)
            {
                Plugin.Log.Verbose($"Message did not contain the {ReplacementToken} mark.");
                return;
            }

            if (PFHostPlayer == DefaultHostPlayer)
            {
                Plugin.Log.Warning("Splicing attempted before saving a PF.");
                Plugin.ChatGui.Print("You have no saved PF! Make sure to open PF and flip to the page that has your duty, so the plugin can save it.");
                    return;
            }

            var text = (payloadWithMark as ITextProvider)!.Text;
            int cutoffPoint = text.IndexOf(ReplacementToken);
            int cutoffPoint2 = cutoffPoint + ReplacementToken.Length;

            // Add the part before the mark to the new message
            List<Payload> splicedPayloads = new();
            int i;
            for (i = 0; i < message.Payloads.Count; i++)
            {
                if (message.Payloads[i] == payloadWithMark)
                {
                    break;
                }
                splicedPayloads.Add(message.Payloads[i]);
            }

            splicedPayloads.Add(new TextPayload(text.Substring(0, cutoffPoint)));

            var pflink = SeString.CreatePartyFinderLink(PFListingId, PFHostPlayer, true);
            splicedPayloads.AddRange(pflink.Payloads);

            // Add the rest of the original message
            splicedPayloads.Add(new TextPayload(text.Substring(cutoffPoint2)));
            i++; // Skip the payload we took apart
            for (; i < message.Payloads.Count; ++i)
            {
                splicedPayloads.Add(message.Payloads[i]);
            }
            message.Payloads.Clear();
            message.Payloads.AddRange(splicedPayloads);

            Plugin.Log.Info("Message rearranged!");
        }
    }
}
