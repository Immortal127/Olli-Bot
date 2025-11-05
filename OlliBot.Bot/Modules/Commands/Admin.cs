using Discord;
using Discord.Interactions;

namespace OlliBot.Modules;

[RequireUserPermission(GuildPermission.Administrator)]
[RequireContext(ContextType.Guild)]
public class AdminCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ILogger<AdminCommands> _logger;

    public AdminCommands(ILogger<AdminCommands> logger)
    {
        _logger = logger;
    }
    [SlashCommand("purge", "Purge a number of messages from a user in a text channel")]
    public async Task Purge([Summary("user", "Specified user")] IUser user, [Summary("amount", "amount of messages to delete")] int amount)
    {
        try
        {
            await Context.Interaction.DeferAsync(ephemeral: true);

            if (amount > 20)
            {
                await Context.Interaction.ModifyOriginalResponseAsync(msg =>
                {
                    msg.Content = "Cannot delete more than 20 messages at once";
                });
                return;
            }

            IEnumerable<IMessage> messages = (await Context.Channel.GetMessagesAsync(100).FlattenAsync()).Where(m => (m.Flags & MessageFlags.Ephemeral) == 0); //exclude Ephemeral messages

            IEnumerable<IMessage> filteredMessages = from m in messages
                                                     where m.Author.Id == user.Id
                                                     select m;

            IEnumerable<IMessage> delMessages = filteredMessages.Take(amount);


            //Messages older than 14 days can't be bulk deleted so we split old and recent messages into two data collections and delete them using appropriate methods

            //Messages older than 13.5 days
            IEnumerable<IMessage> oldMessages = from m in delMessages where (DateTimeOffset.UtcNow - m.Timestamp).TotalDays > 13.5 select m;

            //Every other message in delMessages not in oldMessages
            IEnumerable<IMessage> recentMessages = delMessages.Except(oldMessages);

            var textChannel = (ITextChannel)Context.Channel;


            int delMessageCount = 0;

            if (recentMessages.Any())
            {
                delMessageCount += recentMessages.Count();
                await textChannel.DeleteMessagesAsync(recentMessages);
            }

            foreach (IMessage m in oldMessages)
            {
                delMessageCount += 1;
                await Context.Channel.DeleteMessageAsync(m);
            }
            await Context.Interaction.ModifyOriginalResponseAsync(msg =>
            {
                msg.Content = $"Deleted {(recentMessages.Count() + oldMessages.Count())} messages by {user}";
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting messages.");
            await Context.Interaction.ModifyOriginalResponseAsync(msg =>
            {
                msg.Content = $"Error occured: {ex.Message}";
            });
        }
    }
}