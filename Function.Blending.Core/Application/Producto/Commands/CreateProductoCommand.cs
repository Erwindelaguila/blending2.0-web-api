using MediatR;
using Function.Blending.Core.Application.Producto.DTOs;
using System;

namespace Function.Blending.Core.Application.Producto.Commands;

public class CreateProductoCommand : IRequest<ProductoDTO>
{
    public string Codigo { get; }
    public string Nombre { get; }
    public string Descripcion { get; }
    public Guid CalidadId { get; }
    public Guid TipoProduccionId { get; }
    public bool? Activo { get; }
    public Guid CreadoPorId { get; }

    public CreateProductoCommand(
        string codigo,
        string nombre,
        string descripcion,
        Guid calidadId,
        Guid tipoProduccionId,
        bool? activo,
        Guid creadoPorId)
    {
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        CalidadId = calidadId;
        TipoProduccionId = tipoProduccionId;
        Activo = activo;
        CreadoPorId = creadoPorId;
    }
}
