using ELD888TGBOT._1._Models;
using ELD888TGBOT._1._Models.BotErrors;
using ELD888TGBOT._1._Models.Constants;
using ELD888TGBOT._3._Service.Interfaces;
using ELD888TGBOT._4._Bot.CommandHandler.Commands.Interfaces;
using Telegram.Bot;

namespace ELD888TGBOT._4._Bot.CommandHandler
{
    public class BuyCarHandler : ICommandHandler
    {
        private readonly ICarService carService;
        private readonly Dictionary<long, UserData> usersData;
        public BuyCarHandler(ICarService carService, Dictionary<long, UserData> usersData)
        {
            this.carService = carService;
            this.usersData = usersData;
        }
        public async Task StartProcess(ITelegramBotClient botClient, long chatId)
        {
            usersData[chatId].UserState = UserStates.WAITING_FOR_CARSERIALNR;
            await botClient.SendTextMessageAsync(chatId, "Insert the car SerialNumber:");
        }
        public async Task HandleUpdate(ITelegramBotClient botClient, long chatId, string messageText)
        {
            var writeHelper = new TextFormatHelper(botClient);
            var saleResult = await carService.BuyCar(
                new ServiceRequestData(usersData[chatId].JwtToken, messageText));
            if (saleResult.IsFailed)
            {
                var error = saleResult.Errors[0] as BotErrors;
                await writeHelper.WriteError(error, chatId);
                usersData[chatId].UserState = UserStates.AWAITING_COMMAND;
                return;
            }
            usersData[chatId].UserState = UserStates.AWAITING_COMMAND;
            await writeHelper.SendSaleDetailsAsync(chatId, writeHelper.DeserializeSales(saleResult.Value), botClient);
        }
    }
}