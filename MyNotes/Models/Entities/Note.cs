using System.ComponentModel.DataAnnotations;

namespace MyNotes.Models.Entities
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public int UserInformationId { get; set; }
        public UserInformation UserInformation { get; set; } = null!;
    }
}
