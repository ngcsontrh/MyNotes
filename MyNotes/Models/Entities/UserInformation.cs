using Microsoft.AspNetCore.Identity;

namespace MyNotes.Models.Entities
{
    public class UserInformation
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string ApplicationUserId { get; set; } = null!;
      
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public ICollection<Note> Notes { get; set; } = null!;
    }
}
