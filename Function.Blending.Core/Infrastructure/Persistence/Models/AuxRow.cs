using System;
using System.Collections.Generic;

namespace Function.Blending.Core.Infrastructure.Persistence.Models;

public partial class AuxRow
{
    public Guid Id { get; set; }

    public string Clave { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int Orden { get; set; }

    public Guid? PadreId { get; set; }

    public Guid TableId { get; set; }

    public bool Activo { get; set; }

    public Guid CreadoPorId { get; set; }

    public DateTime CreadoEl { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public DateTime? ModificadoEl { get; set; }

    public virtual ICollection<AuxValue> AuxValue { get; set; } = new List<AuxValue>();

    public virtual ICollection<AuxRow> InversePadre { get; set; } = new List<AuxRow>();

    public virtual AuxRow? Padre { get; set; }

    public virtual AuxTable Table { get; set; } = null!;
}
