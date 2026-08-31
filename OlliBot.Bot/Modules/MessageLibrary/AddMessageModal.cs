using Discord.Interactions;
using OlliBot.Domain.Enums;

namespace OlliBot.Bot.Modules.MessageLibrary;
public class AddMessageModal : IModal
{
    public string Title => "Add Message";

    [InputLabel("Title")]
    [ModalTextInput(
    "message_title",
    maxLength: 50)]
    [RequiredInput(false)]
    public string? MessageTitle { get; set; }

    [InputLabel("Message Type")]
    [ModalSelectMenu("message_type")]
    [ModalSelectMenuOption("Meme", "Meme")]
    [ModalSelectMenuOption("Quote", "Quote")]
    [ModalSelectMenuOption("Other", "Other")]
    [RequiredInput(false)]
    public string? MessageType { get; set; }
}
