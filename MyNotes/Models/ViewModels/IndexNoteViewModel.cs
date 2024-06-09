namespace MyNotes.Models.ViewModels
{
    public class IndexNoteViewModel<T>
    {
        public ICollection<T>? Items { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
