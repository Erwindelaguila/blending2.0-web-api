using Function.Blending.Core.Application.Common.Wrappers;

namespace Function.Blending.Core.Application.Planta.DTOs;

public class PlantaDTO
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int NumeroRuma { get; set; }
    public bool Activo { get; set; }
    public Guid CreadoPorId { get; set; }
    public DateTime CreadoEl { get; set; }
    public Guid? ModificadoPorId { get; set; }
    public DateTime? ModificadoEl { get; set; }
}

public class PlantaShortDTO
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
}




public class PlantaResponseDTO
{
    public PagedResponse<PlantaDTO>? PlantaPaginate { get; set; } = null!;
    public List<PlantaShortDTO>? PlantaShortList { get; set; } = null!;
}
