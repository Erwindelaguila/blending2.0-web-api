using System.Text.Json.Serialization;

namespace Function.Blending.Core.Application.AppParam.DTOs;

public class CreateAppParamRequestDTO
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Group { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsInternal { get; set; } = false;
    public bool IsVisible { get; set; } = true;
    public bool IsDisableable { get; set; } = true;
    public bool IsRemovable { get; set; } = true;
}
