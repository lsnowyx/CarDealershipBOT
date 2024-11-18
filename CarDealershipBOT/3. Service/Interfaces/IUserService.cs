using ELD888TGBOT._1._Models;
using FluentResults;

namespace ELD888TGBOT._3._Service.Interfaces
{
    public interface IUserService
    {
        Task<Result<string>> SignIn(ServiceRequestData serviceRequestData);
        Task<Result<string>> CreateAccount(ServiceRequestData serviceRequestData);
    }
}
