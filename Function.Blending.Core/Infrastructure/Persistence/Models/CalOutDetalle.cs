using System;
using System.Collections.Generic;

namespace Function.Blending.Core.Infrastructure.Persistence.Models;

public partial class CalOutDetalle
{
    public Guid Id { get; set; }

    public Guid EjecucionId { get; set; }

    public string? Grupo { get; set; }

    public string? Ruma { get; set; }

    public int? KilosUsados { get; set; }

    public int? Cantidad { get; set; }

    public string? Codigo { get; set; }

    public string? DescripcionMaterial { get; set; }

    public string? CentroUbicacion { get; set; }

    public string? AlmacenUbicacion { get; set; }

    public string? FechaContabilizacion { get; set; }

    public DateTime? NuevaFechaFabricacion { get; set; }

    public Guid CreadoPorId { get; set; }

    public DateTime CreadoEl { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public DateTime? ModificadoEl { get; set; }

    public bool Aceptado { get; set; }

    public virtual ICollection<CalOutDetOtros> CalOutDetOtros { get; set; } = new List<CalOutDetOtros>();

    public virtual ICollection<CalOutDetParametro> CalOutDetParametro { get; set; } = new List<CalOutDetParametro>();

    public virtual CalEjecucion Ejecucion { get; set; } = null!;
}
