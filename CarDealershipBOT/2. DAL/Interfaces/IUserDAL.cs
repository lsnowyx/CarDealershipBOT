using ELD888TGBOT._1._Models.Request;
using FluentResults;

namespace ELD888TGBOT._2._DAL
{
    public interface IUserDAL
    {
        Task<Result<string>> SignIn(DALRequestData dalRequestData);
        Task<Result<string>> CreateAccount(DALRequestData dALRequestData);
    }
}
