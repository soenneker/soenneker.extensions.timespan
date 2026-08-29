[![](https://img.shields.io/nuget/v/soenneker.extensions.timespan.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.timespan/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.timespan/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.timespan/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.timespan.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.timespan/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.timespan/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.timespan/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.TimeSpan
A collection of helpful TimeSpan extension methods.

## Installation

```bash
dotnet add package Soenneker.Extensions.TimeSpan
```

## Quick start

```csharp
using Soenneker.Extensions.TimeSpan;

TimeSpan timeSpan = TimeSpan.FromMinutes(5);
var result = timeSpan.ToShortTime();
```

## Common operations

- `ToShortTime()` - Converts a `System.TimeSpan` to a short time string representation. Returns a string representing the time in short time format (e.g., "10:00 PM").
- `ToUtcFromTz()` - Adjusts a `System.TimeSpan` from a specific time zone to UTC by subtracting the time zone's offset from UTC. DateTimeOffset overload.
- `ToTzFromUtc()` - Converts a UTC `System.TimeSpan` to a specific time zone by adding the time zone's offset from UTC. DateTimeOffset overload.
- `IsBetween()` - Determines whether the specified `System.TimeSpan` falls within a given range, inclusive of the start and exclusive of the end.
- `ToDisplayFormat()` - Formats the duration in its most useful unit; `compact` selects abbreviated versus long unit names.
- `ToHourMinuteSecondString()` - Converts a time span into a string formatted as hours, minutes, and seconds. Returns a string representation of the time span in the format 'hh:mm:ss'.
- `ToMinuteSecondString()` - Converts a time span into a string formatted as hours, minutes, and seconds. Returns a string representation of the time span in the format 'hh:mm:ss'.
