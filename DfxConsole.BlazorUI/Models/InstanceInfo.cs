using System.Text.Json.Serialization;

namespace DfxConsole.BlazorUI.Models;

public class InstanceInfo
{
    public string InstanceId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string RuntimeStatus { get; set; } = string.Empty;

    public string CreatedTime { get; set; } = string.Empty;

    public string LastUpdatedTime { get; set; } = string.Empty;

    [JsonExtensionData]
    public Dictionary<string, object> JsonExtensionData { get; set; }
}
