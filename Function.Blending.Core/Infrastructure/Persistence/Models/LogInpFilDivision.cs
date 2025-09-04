using System;
using System.Collections.Generic;

namespace Function.Blending.Core.Infrastructure.Persistence.Models;

public partial class LogInpFilDivision
{
    public Guid Id { get; set; }

    public Guid FiltroId { get; set; }

    public string Ruma { get; set; } = null!;

    public string Division { get; set; } = null!;

    public virtual LogInpFiltro Filtro { get; set; } = null!;
}
