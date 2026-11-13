namespace FixNear.Application.DTOs.Category
{
    public class CategoryRequestDto
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string AdminNote { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string? ReviewedAt { get; set; }
    }

    public class CreateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class ReviewCategoryRequest
    {
        public string? Note { get; set; }
    }
}
