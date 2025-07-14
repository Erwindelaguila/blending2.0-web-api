using Function.Blending.Core.Application.Calidad.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Commands;

public class UpdateCalidadCommand : IRequest<CalidadDTO>
{
    public Guid Id { get; }
    public string? Codigo { get; }
    public string? Nombre { get; }
    public string? CodigoMaterial { get; }
    public string? Descripcion { get; }
    public bool? NoConforme { get; }
    public bool? Activo { get; }
    public Guid ModificadoPorId { get; }
    

    public UpdateCalidadCommand(
        Guid id,    
        string codigo,
        string nombre,
        string codigoMaterial,
        string? descripcion,
        bool? noConforme,
        bool? activo,
        Guid modificadoPorId
        )
    {
        Id = id;
        Codigo = codigo;
        Nombre = nombre;
        CodigoMaterial = codigoMaterial;
        Descripcion = descripcion;
        NoConforme = noConforme;
        Activo = activo;
        ModificadoPorId = modificadoPorId;
    }
}