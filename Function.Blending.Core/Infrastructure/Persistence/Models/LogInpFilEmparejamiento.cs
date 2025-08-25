using System;
using System.Collections.Generic;

namespace Function.Blending.Core.Infrastructure.Persistence.Models;

public partial class LogInpFilEmparejamiento
{
    public Guid Id { get; set; }

    public Guid FiltroId { get; set; }

    public string Grupo { get; set; } = null!;

    public Guid ParametroId { get; set; }

    public decimal Valor { get; set; }

    public virtual LogInpFiltro Filtro { get; set; } = null!;

    public virtual Parametro Parametro { get; set; } = null!;
}
