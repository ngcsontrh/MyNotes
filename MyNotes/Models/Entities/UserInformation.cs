using Microsoft.AspNetCore.Identity;

namespace MyNotes.Models.Entities
{
    public class UserInformation
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public string ApplicationUserId { get; set; } = null!;
      
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
