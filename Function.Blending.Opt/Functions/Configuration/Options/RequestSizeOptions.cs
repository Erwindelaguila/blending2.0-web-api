using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Blending.Opt.Functions.Configuration.Options;

public sealed class RequestSizeOptions
{
  public long MaxBytes { get; set; } = 2 * 1024 * 1024;
}