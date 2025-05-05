using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Abstractions.Consts;


public static class Mapper
{
    public static List<DayOfWeek> DecodeFlaggedDays(DaysOfWeek flags)
    {
        var result = new List<DayOfWeek>();

        if (flags.HasFlag(DaysOfWeek.Monday)) result.Add(DayOfWeek.Monday);
        if (flags.HasFlag(DaysOfWeek.Tuesday)) result.Add(DayOfWeek.Tuesday);
        if (flags.HasFlag(DaysOfWeek.Wednesday)) result.Add(DayOfWeek.Wednesday);
        if (flags.HasFlag(DaysOfWeek.Thursday)) result.Add(DayOfWeek.Thursday);
        if (flags.HasFlag(DaysOfWeek.Friday)) result.Add(DayOfWeek.Friday);
        if (flags.HasFlag(DaysOfWeek.Saturday)) result.Add(DayOfWeek.Saturday);
        if (flags.HasFlag(DaysOfWeek.Sunday)) result.Add(DayOfWeek.Sunday);

        return result;
    }
}
