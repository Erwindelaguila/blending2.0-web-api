using Function.Blending.Core.Application.Common.Commands;

namespace Function.Blending.Core.Application.AppParam.Commands;

/// <summary>
/// Command para actualizar un parámetro de aplicación
/// Hereda de BaseCommand para mantener el contexto necesario para autenticación
/// </summary>
public class UpdateAppParamCommand : BaseCommand<object>
{
    public string Key { get; } // Key actual de la URL
    public string? NewKey { get; } // Nuevo key del body (opcional)
    public string Value { get; }
    public string? Description { get; }
    public string? Category { get; }
    public string? Group { get; }
    public bool? IsActive { get; }
    public bool? IsInternal { get; }
    public bool? IsVisible { get; }
    public bool? IsDisableable { get; }
    public bool? IsRemovable { get; }

    public UpdateAppParamCommand(
        string key,
        string? newKey,
        string value,
        string? description,
        string? category,
        string? group,
        bool? isActive,
        bool? isInternal,
        bool? isVisible,
        bool? isDisableable,
        bool? isRemovable,
        object requestContext) : base(requestContext)
    {
        Key = key;
        NewKey = newKey;
        Value = value;
        Description = description;
        Category = category;
        Group = group;
        IsActive = isActive;
        IsInternal = isInternal;
        IsVisible = isVisible;
        IsDisableable = isDisableable;
        IsRemovable = isRemovable;
    }
}
