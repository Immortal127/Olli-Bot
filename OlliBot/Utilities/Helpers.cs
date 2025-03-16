using Discord;
using System.Text;
using System.Text.RegularExpressions;

namespace OlliBot.Utilities
{
    public static class Helpers
    {
        public static bool HasURL(string? input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            var regex = new Regex(@"https?://[^\s/$.?#].[^\s]*");
            return regex.IsMatch(input);
        }
        public static string FormatEmoteRankings(Dictionary<GuildEmote, int> emoteCounts)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Emote Usage Ranking:");

            foreach (KeyValuePair<GuildEmote, int> kv in emoteCounts.OrderByDescending(kv => kv.Value))
            {
                sb.AppendLine($"{kv.Key}  -  {kv.Value}");
            }

            return sb.ToString();
        }
    }
}
