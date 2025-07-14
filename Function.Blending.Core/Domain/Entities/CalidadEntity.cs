namespace Function.Blending.Core.Domain.Entities;

public class CalidadEntity
{
    public Guid Id { get;  set; }

    public string Codigo { get;  set; }

    public string Nombre { get;  set; }

    public string CodigoMaterial { get;  set; }

    public string? Descripcion { get;  set; }

    public bool NoConforme { get;  set; }

    public bool Activo { get;  set; }

    public Guid CreadoPorId { get;  set; }

    public DateTime CreadoEl { get;  set; }

    public Guid? ModificadoPorId { get;  set; }

    public DateTime? ModificadoEl { get;  set; }
    
}
