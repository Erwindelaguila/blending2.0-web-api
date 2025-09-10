using System;

namespace Function.Blending.Opt.Domain.Abstractions.Services;

public interface IExecutionCodeGenerator
{
  /// Código temporal seguro para creación previa al secuencial definitivo.
  string MakeTemp();

  /// Código final a partir del secuencial persistido.
  string MakeFinal(long secuencial);

  /// Código final a partir de formato y del secuencial persistido.
  string MakeFinal(string format, long secuencial);
}
