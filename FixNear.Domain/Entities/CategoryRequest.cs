namespace FixNear.Domain.Entities
{
    /// <summary>
    /// A provider-proposed service category. It becomes bookable only after an
    /// administrator approves it; approval creates (or re-activates) the
    /// matching <see cref="Category"/> row.
    /// </summary>
    public class CategoryRequest
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";

        public int CategoryRequestId { get; set; }
        public int ProviderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = Pending;
        public string? AdminNote { get; set; }
        public int? ReviewedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }

        public Provider Provider { get; set; } = null!;
        public User? ReviewedBy { get; set; }
    }
}
