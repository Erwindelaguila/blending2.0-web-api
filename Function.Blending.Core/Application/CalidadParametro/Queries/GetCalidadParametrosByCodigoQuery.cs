using Function.Blending.Core.Application.CalidadParametro.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.CalidadParametro.Queries;

public class GetCalidadParametrosByCodigoQuery : IRequest<List<CalidadParametroItemDTO>>
{
    public string CodigoCalidad { get; }

    public GetCalidadParametrosByCodigoQuery(string codigoCalidad)
    {
        CodigoCalidad = codigoCalidad;
    }
}
