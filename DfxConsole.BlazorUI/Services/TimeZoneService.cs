using Microsoft.JSInterop;
using System.Globalization;

namespace DfxConsole.BlazorUI.Services;

public class TimeZoneService
{
    private readonly IJSRuntime _jsRuntime;

    public TimeZoneService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Converts a local DateTime to UTC using the browser's time zone offset
    /// </summary>
    public async Task<DateTime> ConvertLocalToUtcAsync(DateTime localDateTime)
    {
        if (localDateTime.Kind == DateTimeKind.Utc)
        {
            return localDateTime;
        }

        // Get the browser's time zone offset in minutes
        int offsetMinutes = await GetBrowserTimeZoneOffsetAsync();

        return localDateTime.AddMinutes(offsetMinutes);
    }

    public DateTime ConvertUtcToLocal(string dateTime, int offsetMinutes)
    {
        // check last character is Z
        // convert using dateTime
        if (!dateTime.EndsWith("Z", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("The provided dateTime string is not in UTC format (missing 'Z' suffix).");
        }

        DateTime utcDateTime = DateTime.Parse(dateTime, null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
        return utcDateTime.AddMinutes(-offsetMinutes);
    }

    /// <summary>
    /// Gets the browser's time zone offset in minutes from UTC
    /// </summary>
    public async Task<int> GetBrowserTimeZoneOffsetAsync()
    {
        return await _jsRuntime.InvokeAsync<int>("getTimezoneOffset");
    }
}