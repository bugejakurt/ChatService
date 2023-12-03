namespace ChatService.Models.Domain
{
    public class Session(Guid guid)
    {
        public Guid Id { get; } = guid;
        public DateTime DateCreated { get; } = DateTime.Now;
        public DateTime LastPingDate { get; set; } = DateTime.Now;
        public bool IsExpired => (DateTime.Now - LastPingDate).TotalSeconds > 3;
    }
}
