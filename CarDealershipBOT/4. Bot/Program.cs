using ELD888TGBOT._1._Models;
using ELD888TGBOT._1._Models.BotErrors;
using ELD888TGBOT._1._Models.Constants;
using ELD888TGBOT._2._DAL;
using ELD888TGBOT._2._DAL.Implementations;
using ELD888TGBOT._3._Service.Implementations;
using ELD888TGBOT._3._Service.Interfaces;
using ELD888TGBOT._4._Bot.CommandHandler;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

Dictionary<long, UserData> usersData = new Dictionary<long, UserData>();
IUserDAL userDAL = new UserDAL();
IUserService userService = new UserService(userDAL);
ICarDAL carDAL = new CarDAL();
ICarService carService = new CarService(carDAL);

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var telegramBotToken = config["TelegramBot:Token"];
var botClient = new TelegramBotClient(telegramBotToken);
using var cts = new CancellationTokenSource();
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>()
};
botClient.StartReceiving(
    HandleUpdateAsync,
    HandlePollingErrorAsync,
    receiverOptions,
    cancellationToken: cts.Token
);
var commands = new List<BotCommand>
{
    new BotCommand { Command = Constants.SIGNIN, Description = "Sign in to your account" },
    new BotCommand { Command = Constants.BUYCAR, Description = "Buy the car of your dreams" },
    new BotCommand { Command = Constants.GETALLCARS, Description = "Get the list of all the cars in stock" },
    new BotCommand { Command = Constants.CREATEACCOUNT, Description = "Create an account to buy cars" }
};
await botClient.SetMyCommandsAsync(commands);

var me = await botClient.GetMeAsync();
Console.WriteLine($"Bot {me.Username} is running...");
Console.ReadLine();
await cts.CancelAsync();


async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    await Console.Out.WriteLineAsync($"Polling error: {exception.Message}");
}
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    try
    {
        if (update.Type == UpdateType.Message && update.Message != null)
        {
            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;
            InitializeUser(chatId);
            if (usersData[chatId].UserState == UserStates.AWAITING_COMMAND)
            {
                await HandleCommandAsync(botClient, chatId, messageText);
            }
            else
            {
                await ContinueOngoingProcessAsync(botClient, chatId, messageText);
            }
        }
        else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
        {
            var chatId = update.CallbackQuery.Message.Chat.Id;
            if (usersData.ContainsKey(chatId))
            {
                await HandleCallbackQueryAsync(botClient, chatId, update.CallbackQuery.Data);
            }
        }
    }
    catch (Exception ex)
    {
        await Console.Out.WriteLineAsync($"Error in HandleUpdateAsync: {ex.Message}");
    }
}


async Task HandleCommandAsync(ITelegramBotClient botClient, long chatId, string command)
{
    usersData[chatId].CacheData = null;
    switch (command)
    {
        case Constants.SIGNIN:
            usersData[chatId].UserState = UserStates.SIGNIN_STARTED;
            var signInHandler = new SignInHandler(userService, usersData);
            await signInHandler.StartProcess(botClient, chatId);
            break;
        case Constants.BUYCAR:
            if (await CheckHeaderData(botClient, chatId)) break;
            usersData[chatId].UserState = UserStates.WAITING_FOR_CARSERIALNR;
            var companyIdHandler = new BuyCarHandler(carService, usersData);
            await companyIdHandler.StartProcess(botClient, chatId);
            break;
        case Constants.GETALLCARS:
            usersData[chatId].UserState = UserStates.WAITING_FOR_GETALLCARS;
            var getLoadsHandler = new GetAllCarsHandler(carService, usersData);
            await getLoadsHandler.StartProcess(botClient, chatId);
            break;
        case Constants.CREATEACCOUNT:
            usersData[chatId].UserState = UserStates.CREATEACCOUNT_STARTED;
            var createLoadHandler = new CreateAccountHandler(usersData, userService);
            await createLoadHandler.StartProcess(botClient, chatId);
            break;
        default: await botClient.SendTextMessageAsync(chatId, "Unknown command."); break;
    }
}
async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, long chatId, string callbackData)
{
    if (callbackData == "buynow")
    {
        await Console.Out.WriteLineAsync(chatId + " Pressed BUYNOW!");
        usersData[chatId].UserState = UserStates.WAITING_BUYCAR_NOW;
    }
    switch (usersData[chatId].UserState)
    {
        case UserStates.SIGNIN_STARTED:
            var signInHandler = new SignInHandler(userService, usersData);
            await signInHandler.HandleCallback(botClient, chatId, callbackData);
            break;
        case UserStates.CREATEACCOUNT_STARTED:
            var createLoadHandler = new CreateAccountHandler(usersData, userService);
            await createLoadHandler.HandleCallback(botClient, chatId, callbackData);
            break;
        case UserStates.WAITING_BUYCAR_NOW:
            if (await CheckHeaderData(botClient, chatId)) break;
            var buyCarNow = new GetAllCarsHandler(carService, usersData);
            await buyCarNow.HandleCallback(botClient, chatId, callbackData);
            break;
    }
}
async Task ContinueOngoingProcessAsync(ITelegramBotClient botClient, long chatId, string messageText)
{
    switch (usersData[chatId].UserState)
    {
        case UserStates.WAITING_FOR_EMAIL:
        case UserStates.WAITING_FOR_PASSWORD:
            var signInHandler = new SignInHandler(userService, usersData);
            await signInHandler.HandleUpdate(botClient, chatId, messageText);
            break;
        case UserStates.WAITING_FOR_CARSERIALNR:
            var companyIdHandler = new BuyCarHandler(carService, usersData);
            await companyIdHandler.HandleUpdate(botClient, chatId, messageText);
            break;
        case UserStates.WAITING_FOR_CAR_CHOOSING:
        case UserStates.WAITING_BUYCAR_NOW:
            var getLoadsHandler = new GetAllCarsHandler(carService, usersData);
            await getLoadsHandler.HandleUpdate(botClient, chatId, messageText);
            break;
        case UserStates.WAITING_FOR_EMAIL_CREATE:
        case UserStates.WAITING_FOR_PASSWORD_CREATE:
            var createLoadHandler = new CreateAccountHandler(usersData, userService);
            await createLoadHandler.HandleUpdate(botClient, chatId, messageText);
            break;
    }
}
void InitializeUser(long chatId)
{
    if (!usersData.ContainsKey(chatId))
    {
        usersData[chatId] = new UserData
        {
            JwtToken = string.Empty,
            UserState = UserStates.AWAITING_COMMAND
        };
    }
}
async Task<bool> CheckHeaderData(ITelegramBotClient botClient, long chatId)
{
    if (string.IsNullOrEmpty(usersData[chatId].JwtToken))
    {
        await botClient.SendTextMessageAsync(chatId, BotErrors.NotSignedIn.Message);
        return true;
    }
    return false;
}
