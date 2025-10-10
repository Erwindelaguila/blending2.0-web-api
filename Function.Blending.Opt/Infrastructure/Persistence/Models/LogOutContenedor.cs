using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class LogOutContenedor
{
    public Guid Id { get; set; }

    public Guid EjecucionId { get; set; }

    public string Contenedor { get; set; } = null!;

    public string? Grupo { get; set; }

    public Guid? CreadoPorId { get; set; }

    public DateTime CreadoEl { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public DateTime? ModificadoEl { get; set; }

    public virtual LogEjecucion Ejecucion { get; set; } = null!;

    public virtual ICollection<LogOutConComposicion> LogOutConComposicion { get; set; } = new List<LogOutConComposicion>();

    public virtual ICollection<LogOutConDistribucion> LogOutConDistribucion { get; set; } = new List<LogOutConDistribucion>();
}
