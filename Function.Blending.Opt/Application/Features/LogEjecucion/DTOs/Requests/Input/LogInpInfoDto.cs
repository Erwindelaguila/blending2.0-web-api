using System;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

public sealed record LogInpInfoDto(
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
