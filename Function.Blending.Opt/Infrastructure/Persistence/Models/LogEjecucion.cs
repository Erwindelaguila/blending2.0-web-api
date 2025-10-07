using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class LogEjecucion
{
    public Guid Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string? Mensaje { get; set; }

    public Guid EstadoId { get; set; }

    public string? Grupo { get; set; }

    public Guid? AsociadoId { get; set; }

    public bool Confirmado { get; set; }

    public Guid? CreadoPorId { get; set; }

    public DateTime? CreadoEl { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public DateTime? ModificadoEl { get; set; }

    public long Secuencial { get; set; }

    public string? NombreArchivo { get; set; }

    public string? UrlArchivo { get; set; }

    public virtual LogEjecucion? Asociado { get; set; }

    public virtual ICollection<LogEjecucion> InverseAsociado { get; set; } = new List<LogEjecucion>();

    public virtual LogInpDemanda? LogInpDemanda { get; set; }

    public virtual LogInpFiltro? LogInpFiltro { get; set; }

    public virtual LogInpInfo? LogInpInfo { get; set; }

    public virtual ICollection<LogInpOferta> LogInpOferta { get; set; } = new List<LogInpOferta>();

    public virtual ICollection<LogOutContenedor> LogOutContenedor { get; set; } = new List<LogOutContenedor>();
}
