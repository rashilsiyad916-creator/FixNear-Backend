using System.Collections.Generic;

namespace FixNear.Application.DTOs.Provider
{
    public class ProviderDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public int Experience { get; set; }
        public double Rating { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Available { get; set; }
        public bool Verified { get; set; }
        public List<ProviderServiceSummaryDto> Services { get; set; } = new();
    }

    public class ProviderServiceSummaryDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal BasePrice { get; set; }
    }
}
