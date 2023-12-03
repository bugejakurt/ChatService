namespace ChatService.Models.Domain
{
    public class Shift(Team team)
    {
        private readonly object lockObj = new object();
        private FixedSizedQueue<Session> _sessionQueue = new(team.Capacity);

        public Team Team => team;
        public int ActiveSessionCount => _sessionQueue.Count + team.Agents.Sum(a => a.Sessions.Count);
        public int Count => _sessionQueue.Count;
        public bool IsActive { get; set; } = true;

        public Session? FindSession(Guid guid)
        {
            // Check on agents
            Session? session;
            foreach (var agent in team.Agents)
            {
                session = agent.Sessions.FirstOrDefault(s => s.Id == guid);
                if (session != null)
                    return session;
            }

            // Check on session queue
            session = _sessionQueue.FirstOrDefault(s => s.Id == guid);
            if (session != null)
                return session;

            return null;
        }

        public bool TryEnqueue(Session obj)
        {
            var rankedAgents = team.Agents.OrderBy(a => a.Seniority.Rank);

            // Try to enqueue session on the agent first
            var agent = rankedAgents.FirstOrDefault(a => a.Sessions.TryAdd(obj));

            // If all agents are busy, enqueue on the session queue
            if (agent == null)
            {
                lock (lockObj)
                {
                    if (_sessionQueue.TryEnqueue(obj))
                    {
                        // Once max queue is reached and during office hours enable overflow team
                        if (_sessionQueue.Count == _sessionQueue.Size)
                        {
                            if (team.SetOverflowMode(true))
                            {
                                // Resize session queue to updated team capacity.
                                // This should always be greater than the current size
                                _sessionQueue.Resize(team.Capacity);
                                ReassignSessions();
                            }
                        }
                    }
                    else
                        return false;
                }
            }

            return true;
        }

        public void ReassignSessions()
        {
            lock (lockObj)
            {
                // Dequeue sessions from queue to active shift agents
                foreach (var agent in team.Agents)
                {
                    int clearedSessionCount = agent.Sessions.RemoveAll(s => s.IsExpired);

                    // Re-assign queued sessions
                    if (IsActive && _sessionQueue.Count > 0 && agent.Sessions.Count < agent.Sessions.Size)
                    {
                        int freeSessionCount = agent.Sessions.Size - agent.Sessions.Count;
                        var newSessions = _sessionQueue.Dequeue(freeSessionCount).ToList();
                        if (newSessions.Count == 0)
                            break;

                        agent.Sessions.AddRange(newSessions);
                    }
                }
            }
        }

        public IEnumerable<Session> Dequeue(int maxItems)
        {
            return _sessionQueue.Dequeue(maxItems);
        }
    }
}
