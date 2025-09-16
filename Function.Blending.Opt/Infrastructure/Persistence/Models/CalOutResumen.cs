using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class CalOutResumen
{
    public Guid Id { get; set; }

    public Guid EjecucionId { get; set; }

    public string? Grupo { get; set; }

    public decimal Toneladas { get; set; }

    public string NuevaFechaFabricacion { get; set; } = null!;

    public string? CodigoCalidadObjetivo { get; set; }

    public string? CodigoCalidadResultante { get; set; }

    public decimal ValorInicial { get; set; }

    public decimal ValorFinal { get; set; }

    public decimal ValorAgregado { get; set; }

    public bool Aceptado { get; set; }

    public Guid CreadoPorId { get; set; }

    public DateTime CreadoEl { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public DateTime? ModificadoEl { get; set; }

    public virtual ICollection<CalOutResParametro> CalOutResParametro { get; set; } = [];

    public virtual CalEjecucion Ejecucion { get; set; } = null!;
}
