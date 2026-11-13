namespace FixNear.Application.DTOs.Provider
{
    public class ProviderStatsDto
    {
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int AcceptedBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int TodayBookings { get; set; }
        public decimal TotalEarnings { get; set; }
        public int ActiveServices { get; set; }
        public int TotalServices { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
