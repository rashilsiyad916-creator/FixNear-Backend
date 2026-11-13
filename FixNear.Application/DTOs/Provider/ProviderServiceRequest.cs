namespace FixNear.Application.DTOs.Provider
{
    public class ProviderServiceRequest
    {
        public int CategoryId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
