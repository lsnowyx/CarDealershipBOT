using CarDealershipBOT._1._Models.Request;
using ELD888TGBOT._1._Models;
using ELD888TGBOT._1._Models.BotErrors;
using ELD888TGBOT._1._Models.Request;
using ELD888TGBOT._2._DAL;
using ELD888TGBOT._3._Service.Interfaces;
using FluentResults;
using System.Text;
using System.Text.Json;

namespace ELD888TGBOT._3._Service.Implementations
{
    public class UserService(IUserDAL userDAL) : IUserService
    {
        private readonly IUserDAL userDAL = userDAL;
        public async Task<Result<string>> SignIn(ServiceRequestData serviceRequestData)
        {
            var signInRequest = serviceRequestData.RequestData as SignInRequest;
            if (string.IsNullOrEmpty(signInRequest.Email) && string.IsNullOrEmpty(signInRequest.Password))
            {
                return Result.Fail(BotErrors.IncompleteRequest);
            }
            return await userDAL.SignIn(new DALRequestData(
                new StringContent(JsonSerializer.Serialize(signInRequest), Encoding.UTF8, "application/json")));
        }
        public async Task<Result<string>> CreateAccount(ServiceRequestData serviceRequestData)
        {
            var userLoginInfoRequest = serviceRequestData.RequestData as UserLoginInfoRequest;
            var createAccountRequest = new CustomerAddRequest() { UserLoginInfo = userLoginInfoRequest, Type = GetRandomNumber() };
            return await userDAL.CreateAccount(new DALRequestData(
                new StringContent(JsonSerializer.Serialize(createAccountRequest), Encoding.UTF8, "application/json")));
        }
        private int GetRandomNumber()
        {
            Random random = new Random();
            int[] options = { 1, 1, 1, 1, 3 };
            return options[random.Next(0, options.Length)];
        }
    }
}
