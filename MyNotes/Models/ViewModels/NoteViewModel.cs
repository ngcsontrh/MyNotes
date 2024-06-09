using System.ComponentModel.DataAnnotations;

namespace MyNotes.Models.ViewModels
{
    public class NoteViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
    }
}
