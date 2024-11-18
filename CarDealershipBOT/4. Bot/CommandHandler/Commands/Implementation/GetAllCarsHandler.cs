using CarDealershipBOT._1._Models.Result;
using ELD888TGBOT._1._Models;
using ELD888TGBOT._1._Models.BotErrors;
using ELD888TGBOT._1._Models.Constants;
using ELD888TGBOT._3._Service.Interfaces;
using ELD888TGBOT._4._Bot.CommandHandler.Commands.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
namespace ELD888TGBOT._4._Bot.CommandHandler
{
    public class GetAllCarsHandler : ICommandHandlerWithCallback
    {
        private readonly ICarService carService;
        private readonly Dictionary<long, UserData> usersData;

        public GetAllCarsHandler(ICarService carService, Dictionary<long, UserData> usersData)
        {
            this.carService = carService;
            this.usersData = usersData;
        }
        public async Task StartProcess(ITelegramBotClient botClient, long chatId)
        {
            await botClient.SendTextMessageAsync(chatId, "Fetching cars...");
            usersData[chatId].UserState = UserStates.WAITING_FOR_GETALLCARS;
            var getLoadsResult = await carService.GetAllCars();
            var writeHelper = new TextFormatHelper(botClient);
            if (getLoadsResult.IsFailed)
            {
                var error = getLoadsResult.Errors[0] as BotErrors;
                await writeHelper.WriteError(error, chatId);
                usersData[chatId].UserState = UserStates.AWAITING_COMMAND;
                return;
            }
            var result = writeHelper.DeserializeCars(getLoadsResult.Value);
            usersData[chatId].UserState = UserStates.WAITING_FOR_CAR_CHOOSING;
            usersData[chatId].CacheData = result;
            await botClient.SendTextMessageAsync(chatId, "Choose a car\nOr just type in the car SerialNumber", replyMarkup: ShowButtons(result));
        }
        private ReplyKeyboardMarkup ShowButtons(List<CarGetResult> loads)
        {
            return new ReplyKeyboardMarkup(loads
                .Select(x => new KeyboardButton($"{x.SerialNumber}. Price: {x.Price}\nName: {x.Name}"))
                .Select(x => new List<KeyboardButton> { x })
                .ToList())
            { ResizeKeyboard = true, OneTimeKeyboard = true };
        }
        public async Task HandleUpdate(ITelegramBotClient botClient, long chatId, string messageText)
        {
            switch (usersData[chatId].UserState)
            {
                case UserStates.WAITING_FOR_CAR_CHOOSING:
                    usersData[chatId].UserState = UserStates.AWAITING_COMMAND;
                    var writeHelper = new TextFormatHelper(botClient);
                    var carToShowList = usersData[chatId].CacheData as List<CarGetResult>;

                    string carToShowSerialNr = messageText.Split('.')[0].Trim();

                    var carToShow = carToShowList.Where(x => x.SerialNumber == carToShowSerialNr).FirstOrDefault();
                    if (carToShow == null)
                    {
                        await botClient.SendTextMessageAsync(chatId, $"The input string {messageText} was not in a correct format.",
                            replyMarkup: new ReplyKeyboardRemove());
                        return;
                    }
                    usersData[chatId].CacheData = carToShow;
                    await writeHelper.SendCarDetailsAsync(chatId, carToShow);
                    break;
                case UserStates.WAITING_BUYCAR_NOW:
                    if (messageText != "No :(( D:" || messageText != "Yes! :D")
                    {
                        await botClient.SendTextMessageAsync(chatId, $"The input string {messageText} was not in a correct format.",
                            replyMarkup: new ReplyKeyboardRemove());
                        usersData[chatId].UserState = UserStates.AWAITING_COMMAND;
                    }
                    if (messageText == "No :(( D:")
                    {
                        await botClient.SendTextMessageAsync(chatId, $"meh...",
                            replyMarkup: new ReplyKeyboardRemove());
                        usersData[chatId].UserState = UserStates.AWAITING_COMMAND;
                        return;
                    }
                    if (messageText == "Yes! :D")
                    {
                        var carToBeBoughtNow = usersData[chatId].CacheData as CarGetResult;
                        var buyCarHandler = new BuyCarHandler(carService, usersData);
                        await buyCarHandler.HandleUpdate(botClient, chatId, carToBeBoughtNow.SerialNumber);
                    }
                    break;
            }
        }

        public async Task HandleCallback(ITelegramBotClient botClient, long chatId, string callbackData)
        {
            switch (callbackData)
            {
                case "buynow":
                    usersData[chatId].UserState = UserStates.WAITING_BUYCAR_NOW;
                    await botClient.SendTextMessageAsync(chatId, "Are you sure?",
                        replyMarkup: new ReplyKeyboardMarkup(new List<KeyboardButton>() { new KeyboardButton("Yes! :D"), new KeyboardButton("No :(( D:") }));
                    break;
            }
        }
    }
}