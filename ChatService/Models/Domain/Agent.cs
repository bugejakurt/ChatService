namespace ChatService.Models.Domain
{
    public class Agent
    {
        public Agent(Seniority seniority, bool isOverflow)
        {
            Seniority = seniority;
            Capacity = (int)(10 * Seniority.Multiplier);
            Sessions = new(Capacity);
            IsOverflow = isOverflow;
        }

        public Seniority Seniority { get; }
        public int Capacity { get; }
        public FixedSizeList<Session> Sessions { get; }
        public bool IsOverflow { get; }
    }

    public readonly record struct Seniority(decimal Multiplier, int Rank)
    {
        public static readonly Seniority Junior = new(0.4m, 0);
        public static readonly Seniority MidLevel = new(0.6m, 1);
        public static readonly Seniority Senior = new(0.8m, 2);
        public static readonly Seniority TeamLead = new(0.5m, 3);
    }
}
