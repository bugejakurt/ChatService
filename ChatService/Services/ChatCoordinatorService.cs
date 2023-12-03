using ChatService.Models.Domain;
using ChatService.Program;
using Microsoft.Extensions.Options;
using System.Timers;

namespace ChatService.Services;

public class ChatCoordinatorService : IDisposable
{
    private readonly ChatServiceOptions _chatServiceOptions;
    private readonly System.Timers.Timer _refreshTimer;
    private Shift? _activeShift;
    private Shift? _previousShift;
    private static readonly object lockObj = new object();

    public ChatCoordinatorService(IOptions<ChatServiceOptions> chatServiceOptions)
    {
        _chatServiceOptions = chatServiceOptions.Value;

        _refreshTimer = new System.Timers.Timer();
        _refreshTimer.Interval = 1000;
        _refreshTimer.Elapsed += RefreshChat;
        _refreshTimer.Start();
    }

    public Guid CreateSession()
    {
        var sessionId = Guid.NewGuid();
        var session = new Session(sessionId);

        RefreshShift();

        if (_activeShift == null)
            throw new Exception("All shifts have ended");

        if (!_activeShift.TryEnqueue(session))
            throw new Exception("Sorry but the queue is full");

        return sessionId;
    }

    public bool PingSession(Guid guid)
    {
        if (_activeShift == null)
            return false;

        var session = _activeShift.FindSession(guid);
        if (session == null)
            return false;

        session.LastPingDate = DateTime.Now;
        return true;
    }

    private void RefreshChat(object? sender, ElapsedEventArgs e)
    {
        RefreshShift();
        ReassignSessions();
    }

    private void ReassignSessions()
    {
        if (_previousShift != null && _previousShift.ActiveSessionCount == 0)
            // Previous shift has ended
            _previousShift = null;

        if (_activeShift == null)
            return;

        // Remove expired sessions from previous shift
        if (_previousShift != null)
            _previousShift.ReassignSessions();

        // Dequeue sessions from queue to active shift agents
        _activeShift.ReassignSessions();
    }

    /// <summary>
    /// NOTE: This can be DDD friendly by moving it to the Shift class or to a factory method, so that each shift can maintain it's own lifecycle and if needed there could be simultanious shifts.
    /// </summary>
    private void RefreshShift()
    {
        lock (lockObj)
        {
            var dateTimeNow = DateTime.Now.TimeOfDay;

            // This can be further improved to handle cases when for example teams' shifts bridge into one another,
            // but this was not part of the scope.
            var teamOptions = _chatServiceOptions.Teams.FirstOrDefault(t => dateTimeNow >= t.TimeStart && dateTimeNow < t.TimeEnd);
            if (teamOptions == null)
            {
                if (_activeShift != null)
                {
                    _activeShift.IsActive = false;
                    _previousShift = _activeShift;
                    _activeShift = null;
                }
                return;
            }

            if (_activeShift != null)
            {
                if (_activeShift.Team.Name == teamOptions.Name)
                    return;
                else
                    // Shift is changing
                    _previousShift = _activeShift;
            }

            var team = new Team(teamOptions.Name,
                _chatServiceOptions.OfficeHoursStart,
                _chatServiceOptions.OfficeHoursEnd,
                _chatServiceOptions.OverflowMembers);

            // Create new shift
            team.AssignAgents(teamOptions.Members.Junior, Seniority.Junior);
            team.AssignAgents(teamOptions.Members.MidLevel, Seniority.MidLevel);
            team.AssignAgents(teamOptions.Members.Senior, Seniority.Senior);
            team.AssignAgents(teamOptions.Members.TeamLead, Seniority.TeamLead);

            var shift = new Shift(team);

            _activeShift = shift;
        }
    }

    public void Dispose()
    {
        _refreshTimer.Stop();
        _refreshTimer.Dispose();
    }
}
