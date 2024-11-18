using CarDealershipBOT._1._Models.Result;
using ELD888TGBOT._1._Models.BotErrors;
using ELD888TGBOT._1._Models.Constants;
using System.Reflection.Metadata;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ELD888TGBOT._4._Bot
{
    public class TextFormatHelper
    {
        private readonly ITelegramBotClient botClient;
        public TextFormatHelper(ITelegramBotClient botClient)
        {
            this.botClient = botClient;
        }
        public SaleResult DeserializeSales(string jsonResponse)
        {
            return JsonSerializer.Deserialize<SaleResult>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        public async Task SendSaleDetailsAsync(long chatId, SaleResult sale, ITelegramBotClient botClient)
        {
            if (sale == null)
            {
                await botClient.SendTextMessageAsync(chatId, "Sale not found.", replyMarkup: new ReplyKeyboardRemove());
                return;
            }

            try
            {
                if (!string.IsNullOrEmpty(sale.Car.ImageURL))
                {
                    await botClient.SendPhotoAsync(
                        chatId: chatId,
                        photo: new Telegram.Bot.Types.InputFiles.InputOnlineFile(sale.Car.ImageURL)
                    );
                }
                var message = new List<string>
                {
                        $"Sale Details:",
                        $"Price: {sale.Price} USD",
                        $"Date Sold: {sale.DateSold:yyyy-MM-dd}",
                        $"Car Details:",
                        $"  - Name: {sale.Car.Name}",
                        $"  - Model: {sale.Car.Model}",
                        $"  - Type: {sale.Car.Type}",
                        $"  - Price: {sale.Car.Price} USD",
                        $"  - Serial Number: {sale.Car.SerialNumber}",
                        $"  - Description: {sale.Car.Description ?? "N/A"}"
                };

                const int MaxMessageLength = 4096;
                var fullMessage = string.Join("\n", message);
                if (fullMessage.Length > MaxMessageLength)
                {
                    var parts = SplitMessage(fullMessage, MaxMessageLength);
                    foreach (var part in parts)
                    {
                        await botClient.SendTextMessageAsync(chatId, part, replyMarkup: new ReplyKeyboardRemove());
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, fullMessage, replyMarkup: new ReplyKeyboardRemove());
                }
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(chatId, $"Failed to load image or send details. Error: {ex.Message}");
            }
        }
        public List<CarGetResult> DeserializeCars(string jsonResponse)
        {
            return JsonSerializer.Deserialize<List<CarGetResult>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task SendCarDetailsAsync(long chatId, CarGetResult car)
        {
            if (car == null)
            {
                await botClient.SendTextMessageAsync(chatId, "Car not found.", replyMarkup: new ReplyKeyboardRemove());
                return;
            }

            var message = new List<string>
            {
                $"🚗 *Car Details* 🚗",
                $"*Name:* {car.Name}",
                $"*Model:* {car.Model}",
                $"*Type:* {car.Type}",
                $"*Price:* ${car.Price}",
                $"*Serial Number:* `{car.SerialNumber}`",
                $"*Description:* {car.Description ?? "N/A"}",
            };

            const int MaxMessageLength = 4096;
            var fullMessage = string.Join("\n", message);

            if (fullMessage.Length > MaxMessageLength)
            {
                var parts = SplitMessage(fullMessage, MaxMessageLength);
                foreach (var part in parts)
                {
                    await botClient.SendTextMessageAsync(chatId, part, replyMarkup: new ReplyKeyboardRemove(), parseMode: ParseMode.Markdown);
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(chatId, fullMessage, replyMarkup: new ReplyKeyboardRemove(), parseMode: ParseMode.Markdown);
            }

            if (!string.IsNullOrWhiteSpace(car.ImageURL))
            {
                try
                {
                    await botClient.SendPhotoAsync(chatId, photo: new Telegram.Bot.Types.InputFiles.InputOnlineFile(car.ImageURL), 
                        replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("BUY NOW!!!", "buynow")));
                }
                catch (Exception ex)
                {
                    await botClient.SendTextMessageAsync(chatId, $"Failed to load image. URL: {car.ImageURL}. Error: {ex.Message}");
                }
            }
        }
        public List<string> SplitMessage(string message, int maxLength)
        {
            var result = new List<string>();
            for (int i = 0; i < message.Length; i += maxLength)
            {
                var part = message.Substring(i, Math.Min(maxLength, message.Length - i));
                result.Add(part);
            }
            return result;
        }
        public async Task WriteError(BotErrors error, long chatId)
        {
            string statusCodeText = null;
            if (error.StatusCode != null)
            {
                statusCodeText = "\nStatusCode: " + error.StatusCode.ToString();
            }
            await botClient.SendTextMessageAsync(chatId, $"Error: {error.Message}" + statusCodeText);
        }
    }
}
