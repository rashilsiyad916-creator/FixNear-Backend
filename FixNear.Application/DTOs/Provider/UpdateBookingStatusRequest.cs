namespace FixNear.Application.DTOs.Provider
{
    public class UpdateBookingStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    public class UpdateAvailabilityRequest
    {
        public bool Available { get; set; }
    }
}
