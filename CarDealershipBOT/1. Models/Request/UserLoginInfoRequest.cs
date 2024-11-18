namespace CarDealershipBOT._1._Models.Request
{
    public class UserLoginInfoRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class CustomerAddRequest
    {
        public UserLoginInfoRequest UserLoginInfo { get; set; }
        public int Type { get; set; }
    }
}
