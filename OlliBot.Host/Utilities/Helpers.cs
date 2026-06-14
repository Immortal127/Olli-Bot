using Discord;
using System.Text;
using System.Text.RegularExpressions;

namespace OlliBot.Host.Utilities;

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
        var sb = new StringBuilder();

        sb.AppendLine("Emote Usage Ranking:");

        foreach (KeyValuePair<GuildEmote, int> kv in emoteCounts.OrderByDescending(kv => kv.Value))
        {
            sb.AppendLine($"{kv.Key}  -  {kv.Value}");
        }

        return sb.ToString();
    }
    public static bool ContentExeedsLength(this IMessage message, int length)
    {
        return message.Content.Length > length;
    }
    public static bool IsAuthorOlliBot(this IMessage message, IDiscordClient discordClient)
    {
        return message.Author.Id == discordClient.CurrentUser.Id;
    }
}
