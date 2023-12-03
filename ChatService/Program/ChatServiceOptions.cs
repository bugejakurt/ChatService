using ChatService.Models;

namespace ChatService.Program
{
    public record ChatServiceOptions
    {
        public required IEnumerable<ChatServiceOptions_Team> Teams { get; init; }
        public required DateTime OfficeHoursStart { get; init; }
        public required DateTime OfficeHoursEnd { get; init; }
        public required int OverflowMembers { get; init; }
    }
    public record ChatServiceOptions_Team
    {
        public required string Name { get; init; }
        public required ChatServiceOptions_Members Members { get; init; }
        public required TimeSpan TimeStart { get; init; }
        public required TimeSpan TimeEnd { get; init; }
    }
    public record ChatServiceOptions_Members
    {
        public int Junior { get; init; }
        public int MidLevel { get; init; }
        public int Senior { get; init; }
        public int TeamLead { get; init; }
    };
}
