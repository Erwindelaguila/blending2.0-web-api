using System;
using System.Collections.Generic;

namespace Function.Blending.Core.Infrastructure.Persistence.Models;

public partial class LogInpFiltro
{
    public Guid Id { get; set; }

    public Guid EjecucionId { get; set; }

    public int PesoContenedor { get; set; }

    public bool ActualizarCapacidad { get; set; }

    public string? Parametros { get; set; }

    public bool? HabilitarDivision { get; set; }

    public string? Division { get; set; }

    public int? TiempoEspera { get; set; }

    public virtual LogEjecucion Ejecucion { get; set; } = null!;

    public virtual ICollection<LogInpFilCapacidad> LogInpFilCapacidad { get; set; } = new List<LogInpFilCapacidad>();

    public virtual ICollection<LogInpFilDivision> LogInpFilDivision { get; set; } = new List<LogInpFilDivision>();

    public virtual ICollection<LogInpFilEmparejamiento> LogInpFilEmparejamiento { get; set; } = new List<LogInpFilEmparejamiento>();

    public virtual ICollection<LogInpFilOferta> LogInpFilOferta { get; set; } = new List<LogInpFilOferta>();
}
