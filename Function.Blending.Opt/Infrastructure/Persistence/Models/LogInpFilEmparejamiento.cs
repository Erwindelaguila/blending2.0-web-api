using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class LogInpFilEmparejamiento
{
    public Guid Id { get; set; }

    public Guid FiltroId { get; set; }

    public string Grupo { get; set; } = null!;

    public string CodigoParametro { get; set; } = null!;

    public decimal Valor { get; set; }

    public virtual LogInpFiltro Filtro { get; set; } = null!;
}
