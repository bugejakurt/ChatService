using System;

namespace ChatService.Models.Domain
{
    public class Team(string name, DateTime officeHoursStart, DateTime officeHoursEnd, int overflowMembers)
    {
        private List<Agent> _agents = [];

        public IEnumerable<Agent> Agents => _agents;
        public string Name => name;
        public int Capacity => (int)(_agents.Sum(a => a.Capacity) * 1.5);
        public bool OverflowMode { get; private set; }

        public void AssignAgents(int count, Seniority seniority, bool isOverflow = false)
        {
            for (int i = 0; i < count; i++)
            {
                _agents.Add(new Agent(seniority, isOverflow));
            }
        }

        public bool SetOverflowMode(bool mode)
        {
            if (OverflowMode == mode)
                return OverflowMode;

            var dateTimeNow = DateTime.Now;
            if (mode && dateTimeNow >= officeHoursStart && dateTimeNow < officeHoursEnd)
            {
                AssignAgents(overflowMembers, Seniority.Junior, true);
            }
            else
            {
                _agents.RemoveAll(a => a.IsOverflow);
                mode = false;
            }

            return OverflowMode = mode;
        }
    }
}
