using AwesomeAssertions;
using Soenneker.Tests.Unit;
using Soenneker.Utils.TimeZones;

namespace Soenneker.Extensions.TimeSpan.Tests;

public class TimeSpanExtensionTests : UnitTest
{
    private readonly System.DateTime _utcNow;

    public TimeSpanExtensionTests()
    {
        _utcNow = System.DateTime.UtcNow;
    }

    [Test]
    [Arguments(0, 0, "12:00 AM")] // Midnight
    [Arguments(1, 15, "1:15 AM")] // Early morning
    [Arguments(12, 0, "12:00 PM")] // Noon
    [Arguments(13, 30, "1:30 PM")] // Early afternoon
    [Arguments(23, 59, "11:59 PM")] // Just before midnight
    [Arguments(24, 0, "12:00 AM")] // Exactly 24 hours (should roll over to midnight)
    [Arguments(25, 45, "1:45 AM")] // Past 24 hours, next day early morning
    [Arguments(-1, -15, "10:45 PM")] // Negative time, should calculate backwards from midnight
    public void ToShortTime_ShouldReturnExpectedResult(int hours, int minutes, string expected)
    {
        // Arrange
        System.TimeSpan timeSpan = System.TimeSpan.FromHours(hours) + System.TimeSpan.FromMinutes(minutes);

        // Act
        string result = timeSpan.ToShortTime();

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    [Arguments(4, 3, 5, true)] // Checking 4am is between 3am and 5am (All Eastern)
    [Arguments(6, 3, 5, false)] // Checking 6am is between 3am and 5am (All Eastern)
    [Arguments(14, 13, 15, true)] // Checking +12 hour times
    [Arguments(16, 13, 15, false)] // Checking +12 hour times
    [Arguments(22, 5, 21, false)] // Candidate and End time is in next day (UTC)
    [Arguments(21, 5, 22, true)] // Candidate and End time is in next day (UTC)
    [Arguments(3, 5, 22, false)] // End time is in next day (UTC)
    [Arguments(7, 5, 22, true)] // End time is in next day (UTC)
    [Arguments(3, 21, 23, false)] // Range clock is in next day (UTC)
    [Arguments(22, 21, 23, true)] // Entire clock is in next day (UTC)
    [Arguments(22, 20, 21, false)] // Entire clock is in next day (UTC), higher
    [Arguments(20, 20, 20, false)] // Equals
    public void IsBetween_ShouldReturnExpected(int candidateHourEastern, int startHourEastern, int endHourEastern, bool assertion)
    {
        System.TimeSpan startTime = new System.TimeSpan(startHourEastern, 0, 0).ToUtcFromTz(_utcNow, Tz.Eastern);
        System.TimeSpan endTime = new System.TimeSpan(endHourEastern, 0, 0).ToUtcFromTz(_utcNow, Tz.Eastern);
        System.TimeSpan candidate = new System.TimeSpan(candidateHourEastern, 0, 0).ToUtcFromTz(_utcNow, Tz.Eastern);

        bool result = candidate.IsBetween(startTime, endTime);

        result.Should().Be(assertion);
    }

    [Test]
    [Arguments(6, 0, 1, 0)]
    [Arguments(15, 23, 10, 23)]
    [Arguments(2, 55, 21, 55)]
    public void ConvertUtcTimeSpanToEasternBeforeOffsetChange_ShouldMatchExpected(int candidateHourUtc, int candidateMinuteEasternUtc, int expectedHour, int expectedMinute)
    {
        var startTime = new System.TimeSpan(candidateHourUtc, candidateMinuteEasternUtc, 0);

        System.DateTime utcNow = new System.DateTime(_utcNow.Year, 1, 20);
        utcNow = System.DateTime.SpecifyKind(utcNow, System.DateTimeKind.Utc);

        System.TimeSpan result = startTime.ToTzFromUtc(utcNow, Tz.Eastern);

        result.Hours.Should().Be(expectedHour);
        result.Minutes.Should().Be(expectedMinute);
        result.Days.Should().Be(0);
    }

    [Test]
    [Arguments(6, 0, 2, 0)]
    [Arguments(15, 23, 11, 23)]
    [Arguments(2, 55, 22, 55)]
    public void ConvertUtcTimeSpanToEasternAfterOffsetChange_ShouldMatchExpected(int candidateHourUtc, int candidateMinuteEasternUtc, int expectedHour, int expectedMinute)
    {
        var timespan = new System.TimeSpan(candidateHourUtc, candidateMinuteEasternUtc, 0);

        System.DateTime utcNow = new System.DateTime(_utcNow.Year, 6, 20);
        utcNow = System.DateTime.SpecifyKind(utcNow, System.DateTimeKind.Utc);

        System.TimeSpan result = timespan.ToTzFromUtc(utcNow, Tz.Eastern);

        result.Hours.Should().Be(expectedHour);
        result.Minutes.Should().Be(expectedMinute);
        result.Days.Should().Be(0);
    }

    [Test]
    [Arguments(6, 0, 11, 0)]
    [Arguments(15, 23, 20, 23)]
    [Arguments(20, 55, 1, 55)]
    public void ConvertEasternTimeSpanToUtcBeforeOffsetChange_ShouldMatchExpected(int candidateHourEastern, int candidateMinuteEastern, int expectedHour, int expectedMinute)
    {
        var startTime = new System.TimeSpan(candidateHourEastern, candidateMinuteEastern, 0);

        System.DateTime utcNow = new System.DateTime(_utcNow.Year, 1, 20);
        utcNow = System.DateTime.SpecifyKind(utcNow, System.DateTimeKind.Utc);

        System.TimeSpan result = startTime.ToUtcFromTz(utcNow, Tz.Eastern);

        result.Hours.Should().Be(expectedHour);
        result.Minutes.Should().Be(expectedMinute);
        result.Days.Should().Be(0);
    }

    [Test]
    [Arguments(6, 0, 10, 0)]
    [Arguments(15, 23, 19, 23)]
    [Arguments(20, 55, 0, 55)]
    public void ConvertEasternTimeSpanToUtcAfterOffsetChange_ShouldMatchExpected(int candidateHourEastern, int candidateMinuteEastern, int expectedHour, int expectedMinute)
    {
        var startTime = new System.TimeSpan(candidateHourEastern, candidateMinuteEastern, 0);

        System.DateTime utcNow = new System.DateTime(_utcNow.Year, 6, 20);
        utcNow = System.DateTime.SpecifyKind(utcNow, System.DateTimeKind.Utc);

        System.TimeSpan result = startTime.ToUtcFromTz(utcNow, Tz.Eastern);

        result.Hours.Should().Be(expectedHour);
        result.Minutes.Should().Be(expectedMinute);
        result.Days.Should().Be(0);
    }

    [Test]
    public void ToShortTime_ShouldGiveExpectedResult()
    {
        var timeSpan = new System.TimeSpan(1, 1, 1, 1);
        string result = timeSpan.ToShortTime();
        result.Should().Be("1:01 AM");
    }
}
