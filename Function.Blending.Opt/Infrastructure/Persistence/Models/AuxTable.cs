using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class AuxTable
{
    public Guid Id { get; set; }

    public string Clave { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int Orden { get; set; }

    public Guid? PadreId { get; set; }

    public bool Activo { get; set; }

    public Guid CreadoPorId { get; set; }

    public DateTime CreadoEl { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public DateTime? ModificadoEl { get; set; }

    public virtual ICollection<AuxProp> AuxProp { get; set; } = new List<AuxProp>();

    public virtual ICollection<AuxRow> AuxRow { get; set; } = new List<AuxRow>();

    public virtual ICollection<AuxTable> InversePadre { get; set; } = new List<AuxTable>();

    public virtual AuxTable? Padre { get; set; }
}
