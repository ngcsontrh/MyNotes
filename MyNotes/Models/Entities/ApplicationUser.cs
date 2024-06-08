using Microsoft.AspNetCore.Identity;

namespace MyNotes.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public UserInformation UserInformation { get; set; } = null!;
    }
}
