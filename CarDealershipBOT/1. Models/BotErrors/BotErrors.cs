using FluentResults;
using System.Net;

namespace ELD888TGBOT._1._Models.BotErrors
{
    public class BotErrors : Error
    {
        public HttpStatusCode? StatusCode { get; }
        public BotErrors(string message) : base(message) { }
        public BotErrors(string message, HttpStatusCode? statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
        public static BotErrors NotSignedIn
        {
            get
            {
                return new BotErrors("Please sign in first using /signin.");
            }
        }
        public static BotErrors NoCompanyId
        {
            get
            {
                return new BotErrors("Please insert companyId in header using the /companyid command");
            }
        }
        public static BotErrors InvalidId
        {
            get
            {
                return new BotErrors("Invalid ID. Please enter a valid number:");
            }
        }
        public static BotErrors InvalidDate
        {
            get
            {
                return new BotErrors("Invalid Date. Please enter a valid date in the format yyyy-MM-dd:");
            }
        }
        public static BotErrors IncompleteRequest
        {
            get
            {
                return new BotErrors("Incomplete request.");
            }
        }
    }
}