namespace FixNear.Domain.Entities
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public int ProviderId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Booking Booking { get; set; } = null!;
        public User Customer { get; set; } = null!;
        public Provider Provider { get; set; } = null!;
    }
}

