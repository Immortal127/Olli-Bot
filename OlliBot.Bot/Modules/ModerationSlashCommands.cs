using Discord;
using Discord.Interactions;

namespace OlliBot.Bot.Modules;

[RequireDmOrGuildPermission(GuildPermission.Administrator)]
public class ModerationSlashCommands(
    ILogger<ModerationSlashCommands> logger,
    IDiscordClient discordClient) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("purge", "Purge a number of messages from a user in a text channel")]
    public async Task Purge([Summary("user", "Specified user")] IUser user, [Summary("amount", "amount of messages to delete")] int amount)
    {
        try
        {
            await Context.Interaction.DeferAsync(ephemeral: true);

            if (discordClient.CurrentUser.Id != user.Id && amount > 20)
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

            //Messages older than 14 days
            IEnumerable<IMessage> oldMessages = from m in delMessages where (DateTimeOffset.UtcNow - m.Timestamp).TotalDays >= 14 select m;

            //Every other message in delMessages not in oldMessages
            IEnumerable<IMessage> recentMessages = delMessages.Except(oldMessages);

            int delMessageCount = await DeleteMessages(delMessages, oldMessages, recentMessages);

            await Context.Interaction.ModifyOriginalResponseAsync(msg =>
            {
                msg.Content = $"Deleted {delMessageCount} messages by {user}";
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while deleting messages.");
            await Context.Interaction.ModifyOriginalResponseAsync(msg =>
            {
                msg.Content = $"Error occured: {ex.Message}";
            });
        }
    }

    private async Task<int> DeleteMessages(IEnumerable<IMessage> delMessages, IEnumerable<IMessage> oldMessages, IEnumerable<IMessage> recentMessages)
    {
        int delMessageCount = 0;

        if (Context.Channel is ITextChannel textChannel)
        {
            if (recentMessages.Any())
            {
                await textChannel.DeleteMessagesAsync(recentMessages);
                delMessageCount += recentMessages.Count();
            }

            foreach (IMessage message in oldMessages)
            {
                await message.DeleteAsync();
                delMessageCount++;
            }
        }
        else if (Context.Channel is IDMChannel dmChannel)
        {
            // DMs do not support bulk deletion.
            // A bot can only delete its own messages.
            foreach (IMessage message in delMessages)
            {
                if (message.Author.Id != discordClient.CurrentUser.Id)
                {
                    continue;
                }

                await message.DeleteAsync();
                delMessageCount++;
            }
        }

        return delMessageCount;
    }
}