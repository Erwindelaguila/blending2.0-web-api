using System;
using System.Collections.Generic;

namespace Function.Blending.Core.Infrastructure.Persistence.Models;

public partial class CalEjecucion
{
    public Guid Id { get; set; }

    public Guid PlantaId { get; set; }

    public string? Codigo { get; set; }

    public string? Mensaje { get; set; }

    public Guid EstadoId { get; set; }

    public Guid CreadoPorId { get; set; }

    public DateTime CreadoEl { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public DateTime? ModificadoEl { get; set; }

    public virtual CalInpFiltro? CalInpFiltro { get; set; }

    public virtual ICollection<CalInpParametro> CalInpParametro { get; set; } = new List<CalInpParametro>();

    public virtual ICollection<CalOutDetalle> CalOutDetalle { get; set; } = new List<CalOutDetalle>();

    public virtual ICollection<CalOutResumen> CalOutResumen { get; set; } = new List<CalOutResumen>();

    public virtual Planta Planta { get; set; } = null!;
}
