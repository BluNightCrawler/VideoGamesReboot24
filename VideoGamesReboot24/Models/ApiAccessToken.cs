using System.ComponentModel.DataAnnotations;

namespace VideoGamesReboot24.Models
{
    public class ApiAccessToken
    {
        [Key]
        public string ApiName { get; set; }
        public string AccessToken { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
