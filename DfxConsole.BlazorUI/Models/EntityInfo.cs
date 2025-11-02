using System.Text.Json.Serialization;

namespace DfxConsole.BlazorUI.Models;

public class EntityInfo
{
    public EntityIdInfo EntityId { get; set; }

    public string LastOperationTime { get; set; } = string.Empty;

    [JsonExtensionData]
    public Dictionary<string, object> JsonExtensionData { get; set; }

    public class EntityIdInfo
    {
        public string Name { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;
    }
}
