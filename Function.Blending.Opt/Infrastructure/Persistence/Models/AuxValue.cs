using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class AuxValue
{
    public Guid Id { get; set; }

    public string Valor { get; set; } = null!;

    public Guid RowId { get; set; }

    public Guid PropId { get; set; }

    public bool Activo { get; set; }

    public Guid CreadoPorId { get; set; }

    public DateTime CreadoEl { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public DateTime? ModificadoEl { get; set; }

    public virtual AuxProp Prop { get; set; } = null!;

    public virtual AuxRow Row { get; set; } = null!;
}
