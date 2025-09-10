using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class AuxProp
{
    public Guid Id { get; set; }

    public string Clave { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string TipoDato { get; set; } = null!;

    public string? Opciones { get; set; }

    public int Orden { get; set; }

    public Guid TableId { get; set; }

    public bool Activo { get; set; }

    public Guid CreadoPorId { get; set; }

    public DateTime CreadoEl { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public DateTime? ModificadoEl { get; set; }

    public virtual ICollection<AuxValue> AuxValue { get; set; } = new List<AuxValue>();

    public virtual AuxTable Table { get; set; } = null!;
}
