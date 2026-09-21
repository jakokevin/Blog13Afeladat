namespace BlogApi.Models.DTOs
{
    public class AddNewBlogpostDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int BlogId { get; set; }
    }
}
