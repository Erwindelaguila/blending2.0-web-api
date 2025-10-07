using System;

namespace Function.Blending.Opt.Domain.ValueObjects;
public sealed record LogInpInfo(
  string Contrato,
  string PedidoVenta,
  DateTime FechaCarguio,
  string PlantaCodigo,
  string PlantaDescripcion,
  string AlmacenCodigo,
  string AlmacenDescripcion,
  string Cliente,
  string Asistente,
  string Supervisora,
  string PaisDestino,
  decimal CantidadRuma,
  string UnidadMedidaRuma,
  int NumeroMovimientos
);
