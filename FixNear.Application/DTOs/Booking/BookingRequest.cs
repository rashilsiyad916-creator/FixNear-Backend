namespace FixNear.Application.DTOs.Booking
{
    public class BookingRequest
    {
        public int ProviderId { get; set; }
        public int? ProviderServiceId { get; set; }
        public string Date { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Problem { get; set; } = string.Empty;
    }
}
