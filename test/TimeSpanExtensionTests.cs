using FluentAssertions;
using Soenneker.Tests.Unit;
using Xunit;

namespace Soenneker.Extensions.TimeSpan.Tests;

public class TimeSpanExtensionTests : UnitTest
{
    [Theory]
    [InlineData(0, 0, "12:00 AM")] // Midnight
    [InlineData(1, 15, "1:15 AM")] // Early morning
    [InlineData(12, 0, "12:00 PM")] // Noon
    [InlineData(13, 30, "1:30 PM")] // Early afternoon
    [InlineData(23, 59, "11:59 PM")] // Just before midnight
    [InlineData(24, 0, "12:00 AM")] // Exactly 24 hours (should roll over to midnight)
    [InlineData(25, 45, "1:45 AM")] // Past 24 hours, next day early morning
    [InlineData(-1, -15, "10:45 PM")] // Negative time, should calculate backwards from midnight
    public void ToShortTime_ShouldFormatCorrectly(int hours, int minutes, string expected)
    {
        // Arrange
        System.TimeSpan timeSpan = System.TimeSpan.FromHours(hours) + System.TimeSpan.FromMinutes(minutes);

        // Act
        string result = timeSpan.ToShortTime();

        // Assert
        result.Should().Be(expected);
    }
}