using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;
using Function.Blending.Opt.Shared.Constants;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Converters;

/// <summary>
/// Convierte el JSON del modelo (execution_id|executionId, status, message, resultado{grupos,rumas})
/// en tu DTO existente CompleteCalEjecucionRequest (Id, EstadoId, Mensaje, Resumenes, Detalles).
/// Mapea 'status' -> EstadoId resolviendo GUIDs desde variables/config:
///   - Catalog_QualityExecutionStatus_Procesado
///   - Catalog_QualityExecutionStatus_Error
///   - Catalog_QualityExecutionStatus_Cancelado
/// También soporta variables con "__" (Azure): Catalog__QualityExecutionStatus__Procesado, etc.
/// </summary>
public sealed class CompleteCalEjecucionRequestConverter
    : JsonConverter<CompleteCalEjecucionRequest>
{
  private const string Success = "SUCCESS";
  private const string Ok = "OK";
  private const string Error = "ERROR";
  private const string Failed = "FAILED";
  private const string Fail = "FAIL";
  private const string Canceled = "CANCELED";
  private const string Cancelled = "CANCELLED";
  private const string Cancelado = "CANCELADO";

  public override CompleteCalEjecucionRequest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    using var doc = JsonDocument.ParseValue(ref reader);
    var root = doc.RootElement;

    // === 1) Id
    var id = ReadGuid(root, "execution_id")
          ?? ReadGuid(root, "executionId")
          ?? ReadGuid(root, "Id")
          ?? Guid.Empty;

    // === 2) Status -> EstadoId
    var statusRaw = ReadString(root, "status")?.Trim().ToUpperInvariant();
    var estadoId = MapStatusToEstadoId(statusRaw);

    // === 3) Mensaje
    var mensaje = ReadString(root, "message");

    // === 4) Resumenes / Detalles
    IReadOnlyList<CalOutResumenDto>? resumenes = null;
    IReadOnlyList<CalOutDetalleDto>? detalles = null;

    if (root.TryGetProperty("resultado", out var resultadoEl) && resultadoEl.ValueKind == JsonValueKind.Object)
    {
      if (resultadoEl.TryGetProperty("grupos", out var gruposEl) && gruposEl.ValueKind == JsonValueKind.Array)
      {
        resumenes = JsonSerializer.Deserialize<IReadOnlyList<CalOutResumenDto>>(gruposEl.GetRawText(), options);
      }
      if (resultadoEl.TryGetProperty("rumas", out var rumasEl) && rumasEl.ValueKind == JsonValueKind.Array)
      {
        detalles = JsonSerializer.Deserialize<IReadOnlyList<CalOutDetalleDto>>(rumasEl.GetRawText(), options);
      }
    }

    return new CompleteCalEjecucionRequest
    {
      Id = id,
      EstadoId = estadoId,
      Mensaje = mensaje,
      Resumenes = resumenes,
      Detalles = detalles
    };
  }

  public override void Write(Utf8JsonWriter writer, CompleteCalEjecucionRequest value, JsonSerializerOptions options)
  {
    writer.WriteStartObject();
    writer.WriteString("Id", value.Id);
    writer.WriteString("EstadoId", value.EstadoId);
    if (!string.IsNullOrWhiteSpace(value.Mensaje)) writer.WriteString("Mensaje", value.Mensaje);
    if (value.Resumenes is not null)
    {
      writer.WritePropertyName("Resumenes");
      JsonSerializer.Serialize(writer, value.Resumenes, options);
    }
    if (value.Detalles is not null)
    {
      writer.WritePropertyName("Detalles");
      JsonSerializer.Serialize(writer, value.Detalles, options);
    }
    writer.WriteEndObject();
  }

  // === helpers ===

  private static Guid? ReadGuid(JsonElement el, string name)
  {
    if (!el.TryGetProperty(name, out var p)) return null;
    if (p.ValueKind == JsonValueKind.String && Guid.TryParse(p.GetString(), out var g)) return g;
    return null;
  }

  private static string? ReadString(JsonElement el, string name)
  {
    if (!el.TryGetProperty(name, out var p)) return null;
    return p.ValueKind == JsonValueKind.String ? p.GetString() : null;
    // (si algún día llega como number/bool, puedes ampliar aquí)
  }

  private static Guid MapStatusToEstadoId(string? status)
  {
    // 1) Leer GUIDs desde configuración/env
    var guidProc = GetGuidFromConfig(ConfigurationKeys.Catalog.QualityExecutionStatus.Procesado);
    var guidErr = GetGuidFromConfig(ConfigurationKeys.Catalog.QualityExecutionStatus.Error);
    var guidCan = GetGuidFromConfig(ConfigurationKeys.Catalog.QualityExecutionStatus.Cancelado);

    // 2) Normalizar status
    var s = status ?? "";
    return s switch
    {
      Success or Ok => guidProc,
      Canceled or Cancelled or Cancelado => guidCan,
      Error or Failed or Fail => guidErr,
      _ => guidErr
    };
  }

  private static Guid GetGuidFromConfig(string keyColon)
  {
    var keyDoubleUnderscore = keyColon.Replace(":", "__");
    // Azure Functions expone ambas formas como variables de entorno
    var v = Environment.GetEnvironmentVariable(keyColon) ?? Environment.GetEnvironmentVariable(keyDoubleUnderscore);

    return Guid.TryParse(v, out var g) ? g : Guid.Empty;
  }
}