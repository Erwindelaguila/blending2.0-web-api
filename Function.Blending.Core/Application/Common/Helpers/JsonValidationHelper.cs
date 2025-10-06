using System.Text.Json;

namespace Function.Blending.Core.Application.Common.Helpers;


public static class JsonValidationHelper
{
    public static (bool IsValid, string? ErrorField) ValidateBooleanProperties(string jsonBody, params string[] booleanFields)
    {
        try
        {
            using var jsonDoc = JsonDocument.Parse(jsonBody);
            var root = jsonDoc.RootElement;

            foreach (var field in booleanFields)
            {
                if (root.TryGetProperty(field, out var prop))
                {
                    if (prop.ValueKind != JsonValueKind.True &&
                        prop.ValueKind != JsonValueKind.False &&
                        prop.ValueKind != JsonValueKind.Null)
                    {
                        return (false, field);
                    }
                }
            }

            return (true, null);
        }
        catch
        {
            return (false, "JSON inválido");
        }
    }
}