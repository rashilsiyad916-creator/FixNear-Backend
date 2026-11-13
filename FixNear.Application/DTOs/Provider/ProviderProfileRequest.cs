namespace FixNear.Application.DTOs.Provider
{
    public class ProviderProfileRequest
    {
        public string BusinessName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ServiceArea { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Image { get; set; }
        public string? Phone { get; set; }
        public int Experience { get; set; }
        public bool? Available { get; set; }

        // Optional first service, used during onboarding.
        public int? CategoryId { get; set; }
        public string? ServiceName { get; set; }
        public string? ServiceDescription { get; set; }
        public decimal? BasePrice { get; set; }
    }
}
