namespace Lummo.Domain.Settings;

public class GuestFeedbacksCacheSettings
{
    public int AbsoluteExpirationTimeInSeconds { get; init; }
    public int SlidingExpirationTimeInSeconds { get; init; }
}
