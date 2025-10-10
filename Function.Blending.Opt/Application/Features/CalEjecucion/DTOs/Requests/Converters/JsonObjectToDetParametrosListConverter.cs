using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Converters;

public sealed class JsonObjectToDetParametrosListConverter
        : JsonConverter<IReadOnlyList<CalOutDetParametroDto>?>
{
  public override IReadOnlyList<CalOutDetParametroDto>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    if (reader.TokenType == JsonTokenType.Null) return null;
    if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Se esperaba un objeto para 'parametros' (Detalle).");

    var list = new List<CalOutDetParametroDto>();

    while (reader.Read())
    {
      if (reader.TokenType == JsonTokenType.EndObject) break;
      if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();

      var codigo = reader.GetString() ?? "";
      if (!reader.Read()) throw new JsonException();

      decimal valor = 0;
      if (reader.TokenType == JsonTokenType.Number)
      {
        if (!reader.TryGetDecimal(out valor)) throw new JsonException();
      }
      else if (reader.TokenType == JsonTokenType.String)
      {
        var s = reader.GetString();
        if (!decimal.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out valor))
          throw new JsonException($"Valor no decimal para '{codigo}': {s}");
      }
      else
      {
        // otros tipos => 0
      }

      list.Add(new CalOutDetParametroDto { CodigoParametro = codigo, Valor = valor });
    }

    return list;
  }

  public override void Write(Utf8JsonWriter writer, IReadOnlyList<CalOutDetParametroDto>? value, JsonSerializerOptions options)
  {
    if (value is null) { writer.WriteNullValue(); return; }

    writer.WriteStartObject();
    foreach (var p in value)
    {
      writer.WritePropertyName(p.CodigoParametro);
      writer.WriteNumberValue(p.Valor);
    }
    writer.WriteEndObject();
  }
}