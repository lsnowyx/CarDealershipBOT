using ELD888TGBOT._1._Models;
using ELD888TGBOT._1._Models.BotErrors;
using ELD888TGBOT._1._Models.Constants;
using ELD888TGBOT._1._Models.Request;
using ELD888TGBOT._3._Service.Interfaces;
using ELD888TGBOT._4._Bot.CommandHandler.Commands.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
namespace ELD888TGBOT._4._Bot.CommandHandler
{
    public class SignInHandler : ICommandHandlerWithCallback
    {
        private readonly IUserService userService;
        private readonly Dictionary<long, UserData> usersData;
        public SignInHandler(IUserService userService, Dictionary<long, UserData> usersData)
        {
            this.userService = userService;
            this.usersData = usersData;
        }
        public async Task StartProcess(ITelegramBotClient botClient, long chatId)
        {
            usersData[chatId].CacheData = new SignInRequest();

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
            new[] { InlineKeyboardButton.WithCallbackData("Email", "email") },
            new[] { InlineKeyboardButton.WithCallbackData("Password", "password") },
            new[] { InlineKeyboardButton.WithCallbackData("Done", "done") },
            new[] { InlineKeyboardButton.WithCallbackData("Cancel", "cancel") }
        });

            await botClient.SendTextMessageAsync(chatId,
                "Please provide your email and password:" +
                "\nClick again on 'Email' or 'Password' to change it's value" +
                "\nClick 'Done' once ur finished" +
                "\nClick 'Cancel' to cancel the sign-in process",
                replyMarkup: inlineKeyboard);

        }

        public async Task HandleCallback(ITelegramBotClient botClient, long chatId, string callbackData)
        {
            switch (callbackData)
            {
                case "email":
                    usersData[chatId].UserState = UserStates.WAITING_FOR_EMAIL;
                    await botClient.SendTextMessageAsync(chatId, "Please enter your email:");
                    break;
                case "password":
                    usersData[chatId].UserState = UserStates.WAITING_FOR_PASSWORD;
                    await botClient.SendTextMessageAsync(chatId, "Please enter your password:");
                    break;
                case "done":
                    await ProcessSignIn(botClient, chatId);
                    break;
                case "cancel":
                    usersData[chatId].UserState = UserStates.AWAITING_COMMAND;
                    await botClient.SendTextMessageAsync(chatId, "Sign-in process canceled.");
                    break;
            }
        }

        public async Task HandleUpdate(ITelegramBotClient botClient, long chatId, string messageText)
        {
            var signInRequest = usersData[chatId].CacheData as SignInRequest;
            switch (usersData[chatId].UserState)
            {
                case UserStates.WAITING_FOR_EMAIL:
                    signInRequest.Email = messageText;
                    await botClient.SendTextMessageAsync(chatId, "Email received.");
                    break;
                case UserStates.WAITING_FOR_PASSWORD:
                    signInRequest.Password = messageText;
                    await botClient.SendTextMessageAsync(chatId, "Password received.");
                    break;
            }
            usersData[chatId].UserState = UserStates.SIGNIN_STARTED;
        }

        private async Task ProcessSignIn(ITelegramBotClient botClient, long chatId)
        {
            var signInResult = await userService.SignIn(new ServiceRequestData(usersData[chatId].CacheData as SignInRequest));
            var writeHelper = new TextFormatHelper(botClient);
            if (signInResult.IsFailed)
            {
                var error = signInResult.Errors[0] as BotErrors;
                await writeHelper.WriteError(error, chatId);
                usersData[chatId].UserState = UserStates.AWAITING_COMMAND;
                return;
            }
            usersData[chatId].JwtToken = signInResult.Value;
            usersData[chatId].UserState = UserStates.AWAITING_COMMAND;
            await botClient.SendTextMessageAsync(chatId, $"Successfully signed in.");
        }
    }
}