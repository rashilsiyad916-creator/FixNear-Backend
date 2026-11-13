namespace FixNear.Domain.Entities
{
    public class Favorite
    {
        public int UserId { get; set; }
        public int ProviderId { get; set; }

        public User User { get; set; } = null!;
        public Provider Provider { get; set; } = null!;
    }
}
