using DfxConsole.BlazorUI.Models;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DfxConsole.BlazorUI.Services;

public class VersionService
{
    private readonly VersionInfo _versionInfo;

    public VersionService()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var informationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        var version = assembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version;
        
        _versionInfo = new VersionInfo
        {
            ApplicationVersion = informationalVersion ?? version ?? "Unknown",
            FrameworkDescription = RuntimeInformation.FrameworkDescription,
            OSDescription = RuntimeInformation.OSDescription,
        };
    }

    public VersionInfo GetVersionInfo() => _versionInfo;
}