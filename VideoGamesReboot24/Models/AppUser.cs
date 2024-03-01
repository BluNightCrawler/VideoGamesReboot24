using Microsoft.AspNetCore.Identity;

namespace VideoGamesReboot24.Models
{
    public class AppUser : IdentityUser
    {
        public AppUser(string userName) : base(userName)
        {
        }

        public string ImagePath { get; set; }
    }
}
