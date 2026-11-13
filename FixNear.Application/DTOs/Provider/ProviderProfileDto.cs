namespace FixNear.Application.DTOs.Provider
{
    public class ProviderProfileDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ServiceArea { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int Experience { get; set; }
        public bool Verified { get; set; }
        public bool Available { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public int ActiveServices { get; set; }
    }
}
