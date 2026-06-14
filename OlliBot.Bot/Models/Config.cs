using System.Text.Json.Serialization;


namespace OlliBot.Bot.Models;

public class Config
{
    //Properties that can't be modified through commands
    [JsonPropertyName("OwnerID")]
    public ulong OwnerID { get; init; }
    [JsonPropertyName("BotID")]
    public ulong BotID { get; init; }


    //Properties that can be modified through commands
    [JsonPropertyName("BotChannel")]
    public ulong? BotChannel { get; set; }
}