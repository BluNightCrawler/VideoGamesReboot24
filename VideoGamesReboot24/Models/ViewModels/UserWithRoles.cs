using System.ComponentModel.DataAnnotations;

namespace VideoGamesReboot24.Models.ViewModels
{
    public class UserWithRoles
    {
        [Required]  
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public IEnumerable<string> RoleNames { get; set; }

    }
}
