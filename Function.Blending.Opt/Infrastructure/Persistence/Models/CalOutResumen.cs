using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class CalOutResumen
{
    public Guid Id { get; set; }

    public Guid EjecucionId { get; set; }

    public string? Grupo { get; set; }

    public int? Toneladas { get; set; }

    public DateTime? NuevaFechaFabricacion { get; set; }

    public string? CodigoCalidadObjetivo { get; set; }

    public string? CodigoCalidadResultante { get; set; }

    public int? ValorInicial { get; set; }

    public int? ValorFinal { get; set; }

    public int? ValorAgregado { get; set; }

    public bool Aceptado { get; set; }

    public Guid CreadoPorId { get; set; }

    public DateTime CreadoEl { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public DateTime? ModificadoEl { get; set; }

    public virtual ICollection<CalOutResParametro> CalOutResParametro { get; set; } = new List<CalOutResParametro>();

    public virtual CalEjecucion Ejecucion { get; set; } = null!;
}
