using System.ComponentModel.DataAnnotations;

namespace VideoGamesReboot24.Models.ViewModels
{
    public class UserCredentials
    {
        [Required(ErrorMessage = "Please enter a UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter a Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

    }
}
