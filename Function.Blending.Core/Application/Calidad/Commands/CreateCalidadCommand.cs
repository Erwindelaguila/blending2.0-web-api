using Function.Blending.Core.Application.Calidad.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Commands;

public class CreateCalidadCommand : IRequest<object>
{
    public string Codigo { get; }
    public string Nombre { get; }
    public string CodigoMaterial { get; }
    public string? Descripcion { get; }
    public bool? NoConforme { get; }  // opcional
    public bool? Activo { get; }      // opcional
    public Guid CreadoPorId { get; }

    public CreateCalidadCommand(
        string codigo,
        string nombre,
        string codigoMaterial,
        string? descripcion,
        bool? noConforme,
        bool? activo,
        Guid creadoPorId)
    {
        Codigo = codigo;
        Nombre = nombre;
        CodigoMaterial = codigoMaterial;
        Descripcion = descripcion;
        NoConforme = noConforme;
        Activo = activo;
        CreadoPorId = creadoPorId;
    }
}