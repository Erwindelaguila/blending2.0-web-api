namespace Function.Blending.Core.Application.AppParam.DTOs;

/// <summary>
/// DTO de respuesta para AppParam - incluye TODOS los campos incluyendo auditoría.
/// Se usa para GET/consultas donde necesitas ver quién y cuándo creó/modificó.
/// </summary>
public class AppParamResponse
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Group { get; set; }
    public bool IsActive { get; set; }
    public bool IsInternal { get; set; }
    public bool IsVisible { get; set; }
    public bool IsDisableable { get; set; }
    public bool IsRemovable { get; set; }
    
    // ✅ CAMPOS DE AUDITORÍA (para consultas/GET)
    public Guid CreadoPorId { get; set; }
    public DateTime CreadoEl { get; set; }
    public Guid? ModificadoPorId { get; set; }
    public DateTime? ModificadoEl { get; set; }
}
