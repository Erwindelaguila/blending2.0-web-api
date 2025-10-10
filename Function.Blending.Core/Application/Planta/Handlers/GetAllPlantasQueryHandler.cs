using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Application.Planta.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Handlers;

public class GetAllPlantasQueryHandler : IRequestHandler<GetAllPlantasQuery, PlantaResponseDTO>
{
    private readonly IPlantaRepository _plantaRepository;

    public GetAllPlantasQueryHandler(IPlantaRepository plantaRepository)
    {
        _plantaRepository = plantaRepository;
    }

    public async Task<PlantaResponseDTO> Handle(GetAllPlantasQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var plantasQuery = _plantaRepository.GetQueryable();

            if (request.Filters != null)
            {
                plantasQuery = plantasQuery.ApplyCodigoFilter(
                    request.Filters.Codigo,
                    x => x.Codigo);

                plantasQuery = plantasQuery.ApplyEstadoFilter(
                    request.Filters.Estado,
                    x => x.Activo);

                if (request.Filters.FechaDesde.HasValue)
                {
                    var start = request.Filters.FechaDesde.Value.Date;
                    var end = start.AddDays(1);
                    plantasQuery = plantasQuery.Where(x =>
                        (x.CreadoEl >= start && x.CreadoEl < end) ||
                        (x.ModificadoEl.HasValue && x.ModificadoEl.Value >= start && x.ModificadoEl.Value < end)
                    );
                }
            }

            plantasQuery = request.Filters?.Estado switch
            {
                "1" => plantasQuery.OrderByDescending(x => x.Activo).ThenBy(x => x.CreadoEl),
                "0" => plantasQuery.OrderBy(x => x.Activo).ThenBy(x => x.CreadoEl),
                _ => plantasQuery.OrderBy(x => x.CreadoEl)
            };

            if (request.IsHarina)
            {
                var plantasShort = plantasQuery.Select(planta => new PlantaShortDTO()
                {
                    Id = planta.Id,
                    Codigo = planta.Codigo,
                    Nombre = planta.Nombre,
                    NumeroRuma = planta.NumeroRuma
                });
                
                var listPlantaShort = plantasShort.ToList();

                return new PlantaResponseDTO
                {
                    PlantaShortList = listPlantaShort
                };

            }

            var plantasProjected = plantasQuery.Select(planta => new PlantaDTO
            {
                Id = planta.Id,
                Codigo = planta.Codigo,
                Nombre = planta.Nombre,
                Descripcion = planta.Descripcion,
                NumeroRuma = planta.NumeroRuma,
                Activo = planta.Activo,
                CreadoPorId = planta.CreadoPorId,
                CreadoEl = planta.CreadoEl,
                ModificadoPorId = planta.ModificadoPorId,
                ModificadoEl = planta.ModificadoEl
            });

            
            
            var pagedResult = await plantasProjected.ToPagedResultAsync(request.Page, request.Size, cancellationToken);
            
            return new PlantaResponseDTO
            {
                PlantaPaginate = pagedResult.ToPagedResponse()
            };
            
        }
        catch (ArgumentException)
        {
            throw;
        }
    }

}
