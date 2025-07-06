using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyPF.Services
{
    public static class SeStringExtensions
    {
        public static string GetSenderFullName(this SeString chatMessageSender, IClientState gameClient)
        {
            Payload? playerPayload = chatMessageSender.Payloads.FirstOrDefault(p => p.Type == PayloadType.Player);
            string playerName = "Uninitialized";
            if (playerPayload != null)
            {
                var groups = new Regex("Player - PlayerName: ([^\\s,]+ [^\\s,]+), ServerId.*ServerName: (\\S+)").Match(playerPayload.ToString()!).Groups;
                if (groups.Count < 3)
                {
                    playerName = "Could not be captured";
                }
                else
                {
                    playerName = $"{groups[1]}@{groups[2]}";
                }
            }
            else
            {
                // I'm going to assume this is always the mod runner, since it only applies to me when testing.
                playerName = $"{gameClient.LocalPlayer?.GetFullName() ?? "Nobody"}";
            }

            return playerName;
        }
    }
}
