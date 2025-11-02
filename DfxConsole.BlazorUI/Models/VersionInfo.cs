namespace DfxConsole.BlazorUI.Models;

public sealed class VersionInfo
{
    public required string ApplicationVersion { get; init; }

    public required string FrameworkDescription { get; init; }

    public required string OSDescription { get; init; }
}