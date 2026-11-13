namespace FixNear.Domain.Entities
{
    public class ProviderService
    {
        public int ProviderServiceId { get; set; }
        public int ProviderId { get; set; }
        public int CategoryId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; } = true;

        public Provider Provider { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}

