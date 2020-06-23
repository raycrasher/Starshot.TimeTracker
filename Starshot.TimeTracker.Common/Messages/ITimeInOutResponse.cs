namespace Starshot.TimeTracker.Messages
{
    public interface ITimeInOutResponse
    {
        bool Success { get; }
        string Message { get; }
    }
}
