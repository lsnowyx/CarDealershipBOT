using ELD888TGBOT._1._Models.BotErrors;
using ELD888TGBOT._1._Models.Request;
using FluentResults;
using Microsoft.Extensions.Configuration;

namespace ELD888TGBOT._2._DAL
{
    public class CarDAL : ICarDAL
    {
        public async Task<Result<string>> BuyCar(DALRequestData dALRequestData)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {dALRequestData.JwtToken}");
            var response = await httpClient.PostAsync($"{GetUrl()}api/Sale/CreateSale", dALRequestData.StringContent);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail(new BotErrors(nameof(BuyCar), response.StatusCode));
            }
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<Result<string>> GetAllCars()
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{GetUrl()}api/Car/GetAllCars");
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail(new BotErrors(nameof(GetAllCars), response.StatusCode));
            }
            return await response.Content.ReadAsStringAsync();
        }
        private string GetUrl()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["Url:LocalHost"];
        }
    }
}