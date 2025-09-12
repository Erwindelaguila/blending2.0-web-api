using System.Text.Json;

namespace Function.Blending.Upload.Helpers;

public class JsonHelper
{
    public static T? Deserialize<T>(string json)
    {
        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<T>(json, options);
    }
}