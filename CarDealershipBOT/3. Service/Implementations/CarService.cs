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
    public class CarService(ICarDAL carDAL) : ICarService
    {
        private readonly ICarDAL carDAL = carDAL;
        public async Task<Result<string>> BuyCar(ServiceRequestData serviceRequestData)
        {
            var serialNr = serviceRequestData.RequestData as string;
            var buyCar = new
            {
                SerialNumber = serialNr
            };
            var dal = new DALRequestData(serviceRequestData.JwtToken, new StringContent(JsonSerializer.Serialize(buyCar), Encoding.UTF8, "application/json"));
            return await carDAL.BuyCar(dal);
        }
        public async Task<Result<string>> GetAllCars()
        {
            return await carDAL.GetAllCars();
        }
    }
}
