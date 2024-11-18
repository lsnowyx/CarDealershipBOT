namespace ELD888TGBOT._1._Models
{
    public class ServiceRequestData
    {
        public string JwtToken { get; set; }
        public object RequestData { get; set; }
        public ServiceRequestData(string JwtToken, object RequestData)
        {
            this.JwtToken = JwtToken;
            this.RequestData = RequestData;
        }
        public ServiceRequestData(object RequestData)
        {
            this.RequestData = RequestData;
        }
    }
}
