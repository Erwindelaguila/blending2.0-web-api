using System;
using System.Collections.Generic;

namespace Function.Blending.Core.Infrastructure.Persistence.Models;

public partial class Parametro
{
    public Guid Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    public Guid CreadoPorId { get; set; }

    public DateTime CreadoEl { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public DateTime? ModificadoEl { get; set; }

    public Guid? EliminadoPorId { get; set; }

    public DateTime? EliminadoEl { get; set; }

    public bool Eliminado { get; set; }

    public virtual ICollection<CalInpParametro> CalInpParametro { get; set; } = new List<CalInpParametro>();

    public virtual ICollection<CalidadParametro> CalidadParametro { get; set; } = new List<CalidadParametro>();

    public virtual ICollection<LogInpFilEmparejamiento> LogInpFilEmparejamiento { get; set; } = new List<LogInpFilEmparejamiento>();
}
