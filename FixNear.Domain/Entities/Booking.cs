namespace FixNear.Domain.Entities
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public int ProviderServiceId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string Address { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string ProblemDescription { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User Customer { get; set; } = null!;
        public ProviderService ProviderService { get; set; } = null!;
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}

