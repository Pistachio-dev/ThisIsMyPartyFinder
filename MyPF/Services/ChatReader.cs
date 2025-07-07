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

        internal static SavedListingInfo? GetSavedListing => PFHostPlayer == DefaultHostPlayer 
            ? null
            : new SavedListingInfo(PFListingId, PFHostPlayer, PFHostWorld, IsCrossWorld);

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

            return isNew;
        }

        public static void Attach()
        {
            Plugin.ChatGui.ChatMessage += SplicePFLink;
            Plugin.ChatGui.ChatMessage += DumpAllReceivedMessages;
        }

        public static void Dispose()
        {
            Plugin.ChatGui.ChatMessage -= SplicePFLink;
            Plugin.ChatGui.ChatMessage -= DumpAllReceivedMessages;
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

        public static void DumpAllReceivedMessages(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (type != XivChatType.Say && type != XivChatType.TellIncoming)
            {
                return;
            }
            Plugin.Log.Info("------------------------------------------------------------------------------------------------------------");
            Plugin.Log.Info($"Type: {type} Timestamp: {timestamp} IsHandled: {isHandled}");
            Plugin.Log.Info("Sender as interpreted: " + sender.GetSenderFullName(Plugin.ClientState));
            Plugin.Log.Info("Sender SeString dump---------------------------");
            DumpSeString(sender);
            Plugin.Log.Info("Message SeString dump---------------------------");
            DumpSeString(message);
            Plugin.Log.Info("------------------------------------------------------------------------------------------------------------");
        }

        public static void DumpSeString(SeString s)
        {
            int counter = 0;
            foreach (Payload payload in s.Payloads)
            {
                string embeddedInfoType = payload.Type.ToString();
                string text = "Unreadable";
                if (payload is ITextProvider)
                {
                    ITextProvider textProvider = (ITextProvider)payload;
                    text = textProvider.Text;
                }

                string output = $"Payload {counter} Type: {embeddedInfoType} Text: \"{text}\"";
                Plugin.Log.Info(output);
                counter++;
            }
        }
    }
}
