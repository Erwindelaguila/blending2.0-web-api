using Function.Blending.Core.Application.AuxRow.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.AuxRow.Queries;

public class GetAllStatusQualityQuery : IRequest<List<StatusRowDTO>>
{
}