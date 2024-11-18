using Telegram.Bot;

namespace ELD888TGBOT._4._Bot.CommandHandler.Commands.Interfaces
{
    public interface ICommandHandler
    {
        Task StartProcess(ITelegramBotClient botClient, long chatId);
        Task HandleUpdate(ITelegramBotClient botClient, long chatId, string messageText);
    }
}
