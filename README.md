[![](https://img.shields.io/nuget/v/soenneker.extensions.timespan.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.timespan/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.timespan/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.timespan/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.timespan.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.timespan/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.timespan/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.timespan/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.TimeSpan
Format durations and convert clock-time `TimeSpan` values across time zones.

## Installation

```bash
dotnet add package Soenneker.Extensions.TimeSpan
```

## Format a duration

```csharp
using Soenneker.Extensions.TimeSpan;

TimeSpan elapsed = TimeSpan.FromMinutes(65);

string compact = elapsed.ToDisplayFormat();       // "1h 5m"
string readable = elapsed.ToDisplayFormat(false); // "1 hour, 5 minutes"
```

`ToDisplayFormat()` chooses the two most significant units and treats a year as 365 days. Durations below one millisecond, including negative values, return `"0s"`.

For fixed component formats, `ToHourMinuteSecondString()` uses `hh:mm:ss` and wraps hours after 23; `ToMinuteSecondString()` uses `mm:ss` and wraps minutes after 59. These are component displays, not total-hour or total-minute formats.

## Work with times of day

```csharp
TimeSpan localClock = new(9, 30, 0);
DateTime utcInstant = DateTime.UtcNow;

TimeSpan utcClock = localClock.ToUtcFromTz(utcInstant, timeZone);
TimeSpan roundTrip = utcClock.ToTzFromUtc(utcInstant, timeZone);
```

The conversion helpers interpret the `TimeSpan` as a clock time, apply the zone offset at the supplied instant (including daylight-saving and fractional-hour offsets), and normalize the result into `[00:00:00, 24:00:00)`. Pass a UTC `DateTime`/`DateTimeOffset` that represents the date whose offset you need; a time zone cannot be converted correctly without a date.

`ToShortTime()` also treats the value as a clock time, normalizes it to one day, and emits an invariant 12-hour value such as `"9:30 AM"`.

## Test a clock range

```csharp
bool open = new TimeSpan(23, 0, 0).IsBetween(
    start: new TimeSpan(22, 0, 0),
    end: new TimeSpan(2, 0, 0)); // true
```

`IsBetween()` is start-inclusive and end-exclusive and supports ranges that cross midnight. Equal start and end values represent an empty range, not a full day.
