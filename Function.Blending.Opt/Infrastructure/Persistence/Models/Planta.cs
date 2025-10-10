using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class Planta
{
    public Guid Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int NumeroRuma { get; set; }

    public bool Activo { get; set; }

    public Guid CreadoPorId { get; set; }

    public DateTime CreadoEl { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public DateTime? ModificadoEl { get; set; }

    public Guid? EliminadoPorId { get; set; }

    public DateTime? EliminadoEl { get; set; }

    public bool Eliminado { get; set; }

    public virtual ICollection<CalEjecucion> CalEjecucion { get; set; } = new List<CalEjecucion>();
}
