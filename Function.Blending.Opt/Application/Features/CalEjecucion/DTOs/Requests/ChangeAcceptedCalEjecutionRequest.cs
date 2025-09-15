using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests;

public sealed record ChangeAcceptedCalEjecutionRequest()
{
  public IReadOnlyList<Guid>? Grupos { get; init; }
}
