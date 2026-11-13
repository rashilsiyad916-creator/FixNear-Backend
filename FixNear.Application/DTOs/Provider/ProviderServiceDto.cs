namespace FixNear.Application.DTOs.Provider
{
    public class ProviderServiceDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; }
        public int BookingCount { get; set; }
    }
}
