namespace FixNear.Application.DTOs.Booking
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public string Service { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool HasReview { get; set; }
    }
}
