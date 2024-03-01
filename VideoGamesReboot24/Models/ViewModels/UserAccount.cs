using System.ComponentModel.DataAnnotations;

namespace VideoGamesReboot24.Models.ViewModels
{
    public class UserAccount
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string ImagePath { get; set; }

        public bool IsAdmin { get; set; }

    }
}
