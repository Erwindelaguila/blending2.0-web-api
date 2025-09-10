using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Shared.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Queries.GetById;

public sealed record GetLogEjecucionByIdQuery(Guid Id) : IRequest<Result<LogEjecucionResponse>>;