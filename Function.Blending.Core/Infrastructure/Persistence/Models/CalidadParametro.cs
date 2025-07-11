using System;
using System.Collections.Generic;

namespace Function.Blending.Core.Infrastructure.Persistence.Models;

public partial class CalidadParametro
{
    public Guid Id { get; set; }

    public Guid CalidadId { get; set; }

    public Guid ParametroId { get; set; }

    public decimal Valor { get; set; }

    public bool Activo { get; set; }

    public Guid CreadoPorId { get; set; }

    public DateTime CreadoEl { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public DateTime? ModificadoEl { get; set; }

    public virtual Calidad Calidad { get; set; } = null!;

    public virtual Parametro Parametro { get; set; } = null!;
}