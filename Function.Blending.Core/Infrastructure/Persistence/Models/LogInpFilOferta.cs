using System;
using System.Collections.Generic;

namespace Function.Blending.Core.Infrastructure.Persistence.Models;

public partial class LogInpFilOferta
{
    public Guid Id { get; set; }

    public Guid FiltroId { get; set; }

    public int Pos { get; set; }

    public string Material { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public int CantidadAsignada { get; set; }

    public int UMVta { get; set; }

    public int CantidadAsignadaTotal { get; set; }

    public int UMAlmac { get; set; }

    public decimal Tolerancia { get; set; }

    public virtual LogInpFiltro Filtro { get; set; } = null!;

    public virtual ICollection<LogInpFilOfeParametro> LogInpFilOfeParametro { get; set; } = new List<LogInpFilOfeParametro>();
}
