using Newtonsoft.Json;
using VrRetreat.Core;

namespace VrRetreat.Infrastructure;

public class JsonConfiguration : IConfiguration
{
    public static JsonConfiguration FromFile(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var result = JsonConvert.DeserializeObject<JsonConfiguration>(json);

        if (result is null)
            throw new Exception("JSON file resulted in a null object.");

        return result;
    }

    public string VrChatUsername { get; set; } = string.Empty;

    public string VrChatPassword { get; set; } = string.Empty;
}
