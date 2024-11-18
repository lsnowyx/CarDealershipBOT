namespace ELD888TGBOT._1._Models.Request
{
    public class DALRequestData
    {
        public string JwtToken { get; set; }
        public StringContent StringContent { get; set; }
        public DALRequestData(StringContent StringContent)
        {
            this.StringContent = StringContent;
        }
        public DALRequestData(string JwtToken, StringContent StringContent)
        {
            this.JwtToken = JwtToken;
            this.StringContent = StringContent;
        }
    }
}
