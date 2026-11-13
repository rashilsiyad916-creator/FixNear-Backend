namespace FixNear.Application.DTOs.Provider
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public string Author { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
    }
}
