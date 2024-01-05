namespace WebApplication3.Data.ViewModels
{
    public class TokenResponse
    {

        public string access_token { get; set; }
        public int expires_on { get; set; }
        public string id_token { get; set; }
        public string token_type { get; set; }


    }
}
