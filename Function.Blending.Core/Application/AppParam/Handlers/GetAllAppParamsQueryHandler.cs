using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.AppParam.Queries;
using Function.Blending.Core.Application.AppParam.DTOs;
using Function.Blending.Core.Application.Common.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Core.Application.AppParam.Handlers;

public class GetAllAppParamsQueryHandler : IRequestHandler<GetAllAppParamsQuery, AppParamResponseDTO>
{
    private readonly IAppParamRepository _appParamRepository;

    public GetAllAppParamsQueryHandler(IAppParamRepository appParamRepository)
    {
        _appParamRepository = appParamRepository;
    }

    public async Task<AppParamResponseDTO> Handle(GetAllAppParamsQuery request, CancellationToken cancellationToken)
    {
        var queryable = _appParamRepository.GetQueryable();

        
        queryable = queryable.Where(ap => ap.IsVisible);

   
        if (request.Filters != null)
        {
            if (!string.IsNullOrWhiteSpace(request.Filters.Key))
            {
                queryable = queryable.Where(ap => ap.Key.Contains(request.Filters.Key));
            }

            if (request.Filters.IsActive.HasValue)
            {
                queryable = queryable.Where(ap => ap.IsActive == request.Filters.IsActive.Value);
            }

  
            var fechaFiltro = request.Filters.FechaDesde;
            if (fechaFiltro.HasValue)
            {
                var fechaInicio = fechaFiltro.Value.Date;
                var fechaFin = fechaInicio.AddDays(1);
                queryable = queryable.Where(ap => ap.CreadoEl >= fechaInicio && ap.CreadoEl < fechaFin);
            }
        }

        if (request.IsGlobal)
        {
            var itemsShort = await queryable
                .Select(ap => new AppParamSortDTO
                {
                    Key = ap.Key,
                    Value = ap.Value,
                })
                .ToListAsync(cancellationToken);
            
            return new AppParamResponseDTO
            {
                AppParamShortList = itemsShort,
            };
            
        }

      
        var total = await queryable.CountAsync(cancellationToken);

     
        var items = await queryable
            .OrderBy(ap => ap.CreadoEl)
            .Skip((request.Page - 1) * request.Size)
            .Take(request.Size)
            .Select(ap => new AppParamDTO
            {
                Key = ap.Key,
                Value = ap.Value,
                Description = ap.Description,
                Category = ap.Category,
                Group = ap.Group,
                IsActive = ap.IsActive,
                IsInternal = ap.IsInternal,
                IsVisible = ap.IsVisible,
                IsDisableable = ap.IsDisableable,
                IsRemovable = ap.IsRemovable,
                CreadoPorId = ap.CreadoPorId,
                CreadoEl = ap.CreadoEl,
                ModificadoPorId = ap.ModificadoPorId,
                ModificadoEl = ap.ModificadoEl
            })
            .ToListAsync(cancellationToken);

        return new AppParamResponseDTO
        {
            AppParamPaginate = new PagedResponse<AppParamDTO>
            {
                Items = items,
                Pagination = new PaginationInfo
                {
                    CurrentPage = request.Page,
                    PageSize = request.Size,
                    TotalCount = total,
                    TotalPages = (int)Math.Ceiling((double)total / request.Size),
                    HasPrevious = request.Page > 1,
                    HasNext = request.Page < (int)Math.Ceiling((double)total / request.Size),
                    PreviousPage = request.Page > 1 ? request.Page - 1 : null,
                    NextPage = request.Page < (int)Math.Ceiling((double)total / request.Size) ? request.Page + 1 : null
                }
            }
        };
        
    }
}
