using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Utility;
using System;
using System.Linq;

namespace MyPF.Extensions
{
    public static class PlayerCharacterExtensions
    {
        public static (string Name, string World) SplitFullName(this string fullName)
        {
            var split = fullName.Split('@');
            if (split.Length > 1)
            {
                return (split[0], split[1]);
            }
            return (split[0], string.Empty);
        }

        public static bool Matches(this IPlayerCharacter testedPlayer, string searchedName, string searchedWorld)
        {
            return testedPlayer.Name.ToString().Equals(searchedName, StringComparison.InvariantCultureIgnoreCase)
                && testedPlayer.GetWorldName().Equals(searchedWorld, StringComparison.InvariantCultureIgnoreCase);
        }

        public static string GetWorldName(this IPlayerCharacter player)
        {
            return player?.HomeWorld.ValueNullable?.Name.ToString() ?? string.Empty;
        }

        public static string GetFullName(this IPlayerCharacter player)
        {
            var world = player.GetWorldName();
            if (world.IsNullOrEmpty())
            {
                return player?.Name?.ToString() ?? "None";
            }
            return $"{player?.Name ?? "None"}@{world}";
        }

        public static string WithoutWorldName(this string name)
        {
            return name.Split("@").First();
        }

        public static string GetFirstName(this string name)
        {
            return name.Split(' ').First();
        }
    }
}
