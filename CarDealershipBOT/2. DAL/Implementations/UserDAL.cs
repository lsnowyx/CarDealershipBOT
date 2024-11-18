using ELD888TGBOT._1._Models.BotErrors;
using ELD888TGBOT._1._Models.Request;
using FluentResults;
using Microsoft.Extensions.Configuration;

namespace ELD888TGBOT._2._DAL.Implementations
{
    public class UserDAL : IUserDAL
    {
        public async Task<Result<string>> SignIn(DALRequestData dalRequestData)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync($"{GetUrl()}api/Account/Login", dalRequestData.StringContent);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail(new BotErrors(nameof(SignIn), response.StatusCode));
            }
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<Result<string>> CreateAccount(DALRequestData dALRequestData)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync($"{GetUrl()}api/Account/CreateCustomer", dALRequestData.StringContent);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail(new BotErrors(nameof(CreateAccount), response.StatusCode));
            }
            return await response.Content.ReadAsStringAsync();
        }
        private string GetUrl()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()["Url:LocalHost"];
        }
    }
}