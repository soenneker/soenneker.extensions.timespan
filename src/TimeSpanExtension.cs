using System.Diagnostics.Contracts;
using Soenneker.Extensions.DateTime;

namespace Soenneker.Extensions.TimeSpan;

/// <summary>
/// A collection of helpful TimeSpan extension methods
/// </summary>
public static class TimeSpanExtension
{
    /// <summary>
    /// Converts a <see cref="System.TimeSpan"/> to a short time string representation.
    /// </summary>
    /// <param name="timeSpan">The <see cref="System.TimeSpan"/> to convert.</param>
    /// <returns>A string representing the time in short time format (e.g., "10:00 PM").</returns>
    /// <remarks>
    /// This method leverages <see cref="System.DateTime.ToShortTimeString"/> to format the <see cref="System.TimeSpan"/>
    /// as if it were a time of day today. This can be useful for displaying time spans that represent time of day in a user-friendly format.
    /// </remarks>
    [Pure]
    public static string ToShortTime(this System.TimeSpan timeSpan)
    {
        // Ensure the TimeSpan is within a 24-hour period to handle cases where TimeSpan is longer than a day.
        var totalMinutes = (int)((timeSpan.TotalMinutes % (24 * 60) + 24 * 60) % (24 * 60));
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;

        // Format hours in 12-hour format, adjusting 0 to 12 for readability.
        int displayHours = hours % 12;

        if (displayHours == 0) 
            displayHours = 12; // Adjust for 12 AM/PM readability

        // Determine AM or PM based on the original hour value.
        string amPm = hours < 12 ? "AM" : "PM";

        // Return the formatted string.
        return $"{displayHours}:{minutes:00} {amPm}";
    }

    /// <summary>
    /// Determines whether the specified <see cref="System.TimeSpan"/> falls within a given range, inclusive of the start and exclusive of the end.
    /// </summary>
    /// <param name="timeSpan">The <see cref="System.TimeSpan"/> to check.</param>
    /// <param name="start">The start of the range.</param>
    /// <param name="end">The end of the range.</param>
    /// <returns>True if <paramref name="timeSpan"/> is between <paramref name="start"/> and <paramref name="end"/>; otherwise, false.</returns>
    /// <remarks>
    /// This method accounts for ranges that cross over midnight, treating the start and end times as a continuous interval.
    /// </remarks>
    [Pure]
    public static bool IsBetween(this System.TimeSpan timeSpan, System.TimeSpan start, System.TimeSpan end)
    {
        // Normal case, e.g., 8am-2pm
        if (start < end || start == end)
            return start <= timeSpan && timeSpan < end;

        // Reverse case, e.g. 10pm-2am
        return start <= timeSpan || timeSpan < end;
    }

    /// <summary>
    /// Adjusts a <see cref="System.TimeSpan"/> from a specific time zone to UTC by subtracting the time zone's offset from UTC.
    /// </summary>
    /// <param name="timeSpan">The original <see cref="System.TimeSpan"/> to adjust.</param>
    /// <param name="utcNow">The current UTC <see cref="System.DateTime"/>, used to calculate the time zone's offset from UTC.</param>
    /// <param name="timeZoneInfo">The <see cref="System.TimeZoneInfo"/> representing the original time zone.</param>
    /// <returns>A <see cref="System.TimeSpan"/> adjusted to UTC.</returns>
    /// <remarks>
    /// This method calculates the difference between the time zone and UTC as of the specified <paramref name="utcNow"/>.
    /// It adjusts the <paramref name="timeSpan"/> to account for this difference, ensuring the result is within a 24-hour period.
    /// </remarks>
    [Pure]
    public static System.TimeSpan ToUtcFromTz(this System.TimeSpan timeSpan, System.DateTime utcNow, System.TimeZoneInfo timeZoneInfo)
    {
        int offset = utcNow.ToTzOffsetHours(timeZoneInfo);

        long newTicks = timeSpan.Ticks - System.TimeSpan.FromHours(offset).Ticks;

        // Ensure the TimeSpan stays within a 24-hour period
        long totalTicksInADay = System.TimeSpan.FromHours(24).Ticks;
        newTicks = (newTicks + totalTicksInADay) % totalTicksInADay;

        var rtnTimeSpan = new System.TimeSpan(newTicks);
        return rtnTimeSpan;
    }

    /// <summary>
    /// Converts a UTC <see cref="System.TimeSpan"/> to a specific time zone by adding the time zone's offset from UTC.
    /// </summary>
    /// <param name="timeSpan">The UTC <see cref="System.TimeSpan"/> to convert.</param>
    /// <param name="utcNow">The current UTC <see cref="System.DateTime"/>, used to calculate the time zone's offset from UTC.</param>
    /// <param name="timeZoneInfo">The <see cref="System.TimeZoneInfo"/> representing the target time zone.</param>
    /// <returns>A <see cref="System.TimeSpan"/> adjusted to the specified time zone, normalized to a 24-hour period.</returns>
    /// <remarks>
    /// This method calculates the difference between UTC and the specified time zone as of the <paramref name="utcNow"/> provided. 
    /// It then adjusts the <paramref name="timeSpan"/> by this offset, ensuring the result does not exceed a 24-hour period, 
    /// effectively normalizing days in the process. This is particularly useful for converting times between zones without 
    /// changing dates or handling cases where time zone differences may lead to a time span exceeding a single day's length.
    /// </remarks>
    [Pure]
    public static System.TimeSpan ToTzFromUtc(this System.TimeSpan timeSpan, System.DateTime utcNow, System.TimeZoneInfo timeZoneInfo)
    {
        int offset = utcNow.ToTzOffsetHours(timeZoneInfo);

        long newTicks = timeSpan.Ticks + System.TimeSpan.FromHours(offset).Ticks;
        // Ensure the TimeSpan stays within a 24-hour period
        long totalTicksInADay = System.TimeSpan.FromHours(24).Ticks;
        newTicks = (newTicks + totalTicksInADay) % totalTicksInADay;

        return new System.TimeSpan(newTicks);
    }

    [Pure]
    public static string ToDisplayFormat(this System.TimeSpan timeSpan, bool compact = true)
    {
        if (timeSpan.TotalMilliseconds < 1)
            return "0s";

        if (timeSpan.TotalSeconds < 1)
            return compact ? $"{timeSpan.TotalMilliseconds}ms" : $"{timeSpan.TotalMilliseconds} milliseconds";

        if (timeSpan.TotalMinutes < 1)
            return compact
                ? $"{timeSpan.Seconds}s"
                : $"{timeSpan.Seconds} {(timeSpan.Seconds == 1 ? "second" : "seconds")}";

        if (timeSpan.TotalHours < 1)
            return compact
                ? $"{timeSpan.Minutes}m {timeSpan.Seconds}s"
                : $"{timeSpan.Minutes} {(timeSpan.Minutes == 1 ? "minute" : "minutes")}, {timeSpan.Seconds} {(timeSpan.Seconds == 1 ? "second" : "seconds")}";

        if (timeSpan.TotalDays < 1)
            return compact
                ? $"{timeSpan.Hours}h {timeSpan.Minutes}m"
                : $"{timeSpan.Hours} {(timeSpan.Hours == 1 ? "hour" : "hours")}, {timeSpan.Minutes} {(timeSpan.Minutes == 1 ? "minute" : "minutes")}";

        if (timeSpan.TotalDays < 365)
            return compact
                ? $"{timeSpan.Days}d {timeSpan.Hours}h"
                : $"{timeSpan.Days} {(timeSpan.Days == 1 ? "day" : "days")}, {timeSpan.Hours} {(timeSpan.Hours == 1 ? "hour" : "hours")}";

        int years = timeSpan.Days / 365;
        int days = timeSpan.Days % 365;

        return compact
            ? $"{years}y {days}d"
            : $"{years} {(years == 1 ? "year" : "years")}, {days} {(days == 1 ? "day" : "days")}";
    }
}
