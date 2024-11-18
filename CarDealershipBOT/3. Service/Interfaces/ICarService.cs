using ELD888TGBOT._1._Models;
using FluentResults;

namespace ELD888TGBOT._3._Service.Interfaces
{
    public interface ICarService
    {
        Task<Result<string>> BuyCar(ServiceRequestData serviceRequestData);
        Task<Result<string>> GetAllCars();
    }
}
