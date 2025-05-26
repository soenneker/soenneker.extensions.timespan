using AwesomeAssertions;
using Soenneker.Tests.Unit;
using Soenneker.Utils.TimeZones;
using Xunit;

namespace Soenneker.Extensions.TimeSpan.Tests;

public class TimeSpanExtensionTests : UnitTest
{
    private readonly System.DateTime _utcNow;

    public TimeSpanExtensionTests()
    {
        _utcNow = System.DateTime.UtcNow;
    }

    [Theory]
    [InlineData(0, 0, "12:00 AM")] // Midnight
    [InlineData(1, 15, "1:15 AM")] // Early morning
    [InlineData(12, 0, "12:00 PM")] // Noon
    [InlineData(13, 30, "1:30 PM")] // Early afternoon
    [InlineData(23, 59, "11:59 PM")] // Just before midnight
    [InlineData(24, 0, "12:00 AM")] // Exactly 24 hours (should roll over to midnight)
    [InlineData(25, 45, "1:45 AM")] // Past 24 hours, next day early morning
    [InlineData(-1, -15, "10:45 PM")] // Negative time, should calculate backwards from midnight
    public void ToShortTime_ShouldReturnExpectedResult(int hours, int minutes, string expected)
    {
        // Arrange
        System.TimeSpan timeSpan = System.TimeSpan.FromHours(hours) + System.TimeSpan.FromMinutes(minutes);

        // Act
        string result = timeSpan.ToShortTime();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(4, 3, 5, true)] // Checking 4am is between 3am and 5am (All Eastern)
    [InlineData(6, 3, 5, false)] // Checking 6am is between 3am and 5am (All Eastern)
    [InlineData(14, 13, 15, true)] // Checking +12 hour times
    [InlineData(16, 13, 15, false)] // Checking +12 hour times
    [InlineData(22, 5, 21, false)] // Candidate and End time is in next day (UTC)
    [InlineData(21, 5, 22, true)] // Candidate and End time is in next day (UTC)
    [InlineData(3, 5, 22, false)] // End time is in next day (UTC)
    [InlineData(7, 5, 22, true)] // End time is in next day (UTC)
    [InlineData(3, 21, 23, false)] // Range clock is in next day (UTC)
    [InlineData(22, 21, 23, true)] // Entire clock is in next day (UTC)
    [InlineData(22, 20, 21, false)] // Entire clock is in next day (UTC), higher
    [InlineData(20, 20, 20, false)] // Equals
    public void IsBetween_ShouldReturnExpected(int candidateHourEastern, int startHourEastern, int endHourEastern, bool assertion)
    {
        System.TimeSpan startTime = new System.TimeSpan(startHourEastern, 0, 0).ToUtcFromTz(_utcNow, Tz.Eastern);
        System.TimeSpan endTime = new System.TimeSpan(endHourEastern, 0, 0).ToUtcFromTz(_utcNow, Tz.Eastern);
        System.TimeSpan candidate = new System.TimeSpan(candidateHourEastern, 0, 0).ToUtcFromTz(_utcNow, Tz.Eastern);

        bool result = candidate.IsBetween(startTime, endTime);

        result.Should().Be(assertion);
    }

    [Theory]
    [InlineData(6, 0, 1, 0)]
    [InlineData(15, 23, 10, 23)]
    [InlineData(2, 55, 21, 55)]
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

    [Theory]
    [InlineData(6, 0, 2, 0)]
    [InlineData(15, 23, 11, 23)]
    [InlineData(2, 55, 22, 55)]
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

    [Theory]
    [InlineData(6, 0, 11, 0)]
    [InlineData(15, 23, 20, 23)]
    [InlineData(20, 55, 1, 55)]
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

    [Theory]
    [InlineData(6, 0, 10, 0)]
    [InlineData(15, 23, 19, 23)]
    [InlineData(20, 55, 0, 55)]
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

    [Fact]
    public void ToShortTime_ShouldGiveExpectedResult()
    {
        var timeSpan = new System.TimeSpan(1, 1, 1, 1);
        string result = timeSpan.ToShortTime();
        result.Should().Be("1:01 AM");
    }
}