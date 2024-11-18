using ELD888TGBOT._1._Models.Request;
using FluentResults;

namespace ELD888TGBOT._2._DAL
{
    public interface ICarDAL
    {
        Task<Result<string>> BuyCar(DALRequestData dALRequestData);
        Task<Result<string>> GetAllCars();
    }
}
