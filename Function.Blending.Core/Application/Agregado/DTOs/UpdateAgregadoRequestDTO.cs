using System.ComponentModel.DataAnnotations;

namespace Function.Blending.Core.Application.Agregado.DTOs;

/// <summary>
/// DTO para requests de actualización de agregado
/// </summary>
public class UpdateAgregadoRequestDTO
{
    [Required(ErrorMessage = "El ID es obligatorio")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "El código es obligatorio")]
    [StringLength(50, ErrorMessage = "El código no puede exceder los 50 caracteres")]
    public string Codigo { get; set; } = null!;

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    public string Nombre { get; set; } = null!;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
    public string? Descripcion { get; set; }

    public bool? Activo { get; set; }
}
