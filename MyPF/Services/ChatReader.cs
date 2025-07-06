using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using SamplePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPF.Services
{
    public static class ChatReader
    {
        public const string ReplacementToken = "<mypf>";
        private static uint PFListingId = 0;
        private static bool IsCrossWorld = true;
        private static string PFHostPlayer = "Place Holder";

        public static void Attach()
        {
            Plugin.ChatGui.ChatMessage += InsertPFLink;
            Plugin.ChatGui.ChatMessage += DumpAllReceivedMessages;
        }

        private static void InsertPFLink(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            // TODO: CHECK THAT THE SENDER IS THE LOCAL PLAYER
            Payload? payloadWithMark = message.Payloads.FirstOrDefault(p => p is ITextProvider textPayload && textPayload.Text.Contains(ReplacementToken, StringComparison.OrdinalIgnoreCase));
            if (payloadWithMark == null)
            {
                Plugin.Log.Info("Message did not contain the mark.");
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
