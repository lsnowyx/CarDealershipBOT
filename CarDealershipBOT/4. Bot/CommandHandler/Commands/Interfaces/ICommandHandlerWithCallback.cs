using Telegram.Bot;

namespace ELD888TGBOT._4._Bot.CommandHandler.Commands.Interfaces
{
    public interface ICommandHandlerWithCallback : ICommandHandler
    {
        Task HandleCallback(ITelegramBotClient botClient, long chatId, string callbackData);
    }
}
