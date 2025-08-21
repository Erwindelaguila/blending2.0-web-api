using System;
using System.Collections.Generic;

namespace Function.Blending.Core.Infrastructure.Persistence.Models;

public partial class LogEjecucion
{
    public Guid Id { get; set; }

    public Guid PlantaId { get; set; }

    public string? Codigo { get; set; }

    public string? Mensaje { get; set; }

    public Guid EstadoId { get; set; }

    public string? Grupo { get; set; }

    public Guid? AsociadoId { get; set; }

    public bool Considerar { get; set; }

    public Guid? CreadoPorId { get; set; }

    public DateTime? CreadoEl { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public DateTime? ModificadoEl { get; set; }

    public virtual LogEjecucion? Asociado { get; set; }

    public virtual ICollection<LogEjecucion> InverseAsociado { get; set; } = new List<LogEjecucion>();

    public virtual ICollection<LogInpFiltro> LogInpFiltro { get; set; } = new List<LogInpFiltro>();

    public virtual ICollection<LogOutContenedor> LogOutContenedor { get; set; } = new List<LogOutContenedor>();

    public virtual Planta Planta { get; set; } = null!;
}
