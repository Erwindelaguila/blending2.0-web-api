using System.Globalization;
using Function.Blending.Upload.Models;

namespace Function.Blending.Upload.Helpers;

public static class ParsedRowValidator
{
  /// <summary>
  /// Valida si la fila tiene al menos el código de material.
  /// </summary>
  public static bool EsValido(ParsedRowDto row)
  {
    return row.Fijos.TryGetValue("RumaNro", out var uniqueKey) && uniqueKey != null && !string.IsNullOrEmpty(uniqueKey.Trim());
  }

  /// <summary>
  /// Intenta obtener un valor decimal desde un diccionario.
  /// </summary>
  public static decimal? ObtenerDecimal(Dictionary<string, string> dict, string key)
  {
    if (dict.TryGetValue(key, out var valor) &&
        decimal.TryParse(valor.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
      return result;

    return null;
  }

  /// <summary>
  /// Intenta obtener un valor entero desde un diccionario.
  /// </summary>
  public static int? ObtenerEntero(Dictionary<string, string> dict, string key)
  {
    if (dict.TryGetValue(key, out var valor) &&
        int.TryParse(valor, out var result))
      return result;

    return null;
  }
  
  public static double? ObtenerDouble(Dictionary<string, string> dict, string key)
  {
    if (dict.TryGetValue(key, out var valor) &&
        double.TryParse(valor, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var result))
      return result;

    return null;
  }
  
  
  /// <summary>
  /// Intenta obtener un valor booleano desde un diccionario.
  /// </summary>
  public static bool? ObtenerBooleano(Dictionary<string, string> dict, string key)
  {
    if (dict.TryGetValue(key, out var valor) &&
        bool.TryParse(valor, out var result))
      return result;

    return null;
  }

  /// <summary>
  /// Intenta obtener una fecha desde un diccionario.
  /// </summary>
  public static DateTime? ObtenerFecha(Dictionary<string, string> dict, string key)
  {
    if (dict.TryGetValue(key, out var valor) &&
        DateTime.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
      return result;

    return null;
  }

  /// <summary>
  /// Devuelve el valor plano de una columna o null si no existe.
  /// </summary>
  public static string? ObtenerTexto(Dictionary<string, string> dict, string key)
  {
    return dict.TryGetValue(key, out var valor) ? valor : null;
  }
  

    /// <summary>
    /// Valida si un valor cumple con el tipo definido. Si no lo cumple, retorna un mensaje de error.
    /// </summary>
    public static string? ObtenerErrorDeTipo(string dataType, object? valor)
    {
      if (valor == null)
        return null; // Puedes cambiar esto si deseas forzar que no haya valores nulos

      var actualType = valor.GetType().Name;
      var expectedType = dataType.ToLower();

      switch (expectedType)
      {
        case "string":
          if (valor is string) return null;
          return $"Valor \"{valor}\" no es tipo string (es {actualType})";

        case "number":
          if (valor is int || valor is float || valor is double || valor is decimal) return null;
          return $"Valor \"{valor}\" no es tipo number (es {actualType})";

        default:
          return null; // Si no se reconoce el tipo, lo aceptamos
      }
    }
  
}
