namespace Lummo.Domain.Brokers;

public class RandomDateTimeProvider
{
    public DateTime Generate(DateTime? start, DateTime? end)
    {
        start ??= DateTime.UnixEpoch;
        end ??= DateTime.Now;

        if (start > end)
            throw new ArgumentException("Start date cannot be greater than end date.");

        var random = new Random();
        var range = end - start;
        var randTimeSpan = new TimeSpan((long)(random.NextDouble() * range.Value.Ticks));
        return start.Value + randTimeSpan;
    }
}
