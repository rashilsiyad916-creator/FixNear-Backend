namespace FixNear.Domain.Entities
{
    public class Provider
    {
        public int ProviderId { get; set; }
        public int UserId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ServiceArea { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public int Experience { get; set; }
        public string? Image { get; set; }
        public bool Available { get; set; } = true;

        public User User { get; set; } = null!;
        public ICollection<ProviderService> Services { get; set; } = new List<ProviderService>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}

