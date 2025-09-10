using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class AppParam
{
    public string Key { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string? Description { get; set; }

    public string? Category { get; set; }

    public string? Group { get; set; }

    public bool IsActive { get; set; }

    public bool IsInternal { get; set; }

    public bool IsVisible { get; set; }

    public bool IsDisableable { get; set; }

    public bool IsRemovable { get; set; }

    public Guid CreadoPorId { get; set; }

    public DateTime CreadoEl { get; set; }

    public Guid? ModificadoPorId { get; set; }

    public DateTime? ModificadoEl { get; set; }
}
