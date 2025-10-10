using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Persistence.Models;

public partial class SysParam
{
    public string Key { get; set; } = null!;

    public Guid Id { get; set; }

    public string? Description { get; set; }

    public string? Category { get; set; }

    public string? Group { get; set; }

    public bool IsActive { get; set; }
}
