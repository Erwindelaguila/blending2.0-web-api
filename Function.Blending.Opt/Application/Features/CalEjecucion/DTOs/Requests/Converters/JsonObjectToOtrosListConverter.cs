using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Converters;

public sealed class JsonObjectToOtrosListConverter
        : JsonConverter<IReadOnlyList<CalOutDetOtrosDto>?>
{
  public override IReadOnlyList<CalOutDetOtrosDto>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    if (reader.TokenType == JsonTokenType.Null) return null;
    if (reader.TokenType != JsonTokenType.StartObject)
      throw new JsonException("Se esperaba un objeto para 'otrosParam'.");

    var list = new List<CalOutDetOtrosDto>();

    while (reader.Read())
    {
      if (reader.TokenType == JsonTokenType.EndObject) break;
      if (reader.TokenType != JsonTokenType.PropertyName)
        throw new JsonException("Se esperaba nombre de propiedad en 'otrosParam'.");

      var codigo = reader.GetString() ?? string.Empty;

      if (!reader.Read())
        throw new JsonException("Valor faltante en 'otrosParam'.");

      string valor = reader.TokenType switch
      {
        JsonTokenType.String => reader.GetString() ?? string.Empty,
        JsonTokenType.Number => reader.TryGetDecimal(out var dec)
            ? dec.ToString(CultureInfo.InvariantCulture)
            : reader.GetDouble().ToString(CultureInfo.InvariantCulture),
        JsonTokenType.True => "true",
        JsonTokenType.False => "false",
        JsonTokenType.Null => "",
        // Para objetos/arreglos/otros: capturamos el JSON crudo
        _ => JsonDocument.ParseValue(ref reader).RootElement.GetRawText()
      };

      list.Add(new CalOutDetOtrosDto { Codigo = codigo, Valor = valor });
    }

    return list;
  }

  public override void Write(Utf8JsonWriter writer, IReadOnlyList<CalOutDetOtrosDto>? value, JsonSerializerOptions options)
  {
    if (value is null) { writer.WriteNullValue(); return; }

    writer.WriteStartObject();
    foreach (var item in value)
    {
      writer.WritePropertyName(item.Codigo);
      writer.WriteStringValue(item.Valor);
    }
    writer.WriteEndObject();
  }
}