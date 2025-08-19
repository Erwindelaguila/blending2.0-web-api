namespace Function.Blending.Core.Application.Common.Interfaces;

public interface IBaseEntityFilter
{
    string? Codigo { get; set; }
    string? Estado { get; set; }
    DateTime? FechaDesde { get; set; }
}
