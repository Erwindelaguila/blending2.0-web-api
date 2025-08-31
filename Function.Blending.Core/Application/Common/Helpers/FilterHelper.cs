using System.Linq.Expressions;

namespace Function.Blending.Core.Application.Common.Helpers;

public static class FilterHelper
{
    public static IQueryable<T> ApplyCodigoFilter<T>(this IQueryable<T> query, string? codigo, Expression<Func<T, string>> codigoSelector)
    {
        if (string.IsNullOrWhiteSpace(codigo)) return query;
        var trimmedCodigo = codigo.Trim();
        return query.Where(BuildContainsExpression(codigoSelector, trimmedCodigo));
    }
    public static IQueryable<T> ApplyEstadoFilter<T>(this IQueryable<T> query, string? estado, Expression<Func<T, bool>> activoSelector)
    {
        if (string.IsNullOrWhiteSpace(estado)) return query;
        var estadoTrimmed = estado.Trim();
        return estadoTrimmed switch
        {
            "1" => query.Where(activoSelector),
            "0" => query.Where(BuildNotExpression(activoSelector)),
            _ => throw new ArgumentException($"Parámetro 'estado' inválido: '{estado}'. Use '1' para activos o '0' para inactivos.", nameof(estado))
        };
    }
    public static IQueryable<T> ApplyFechaRangeFilter<T>(this IQueryable<T> query, DateTime? fechaDesde, DateTime? fechaHasta, Expression<Func<T, DateTime>> fechaSelector)
    {
        if (fechaDesde.HasValue)
            query = query.Where(BuildGreaterThanOrEqualExpression(fechaSelector, fechaDesde.Value.Date));
        if (fechaHasta.HasValue)
            query = query.Where(BuildLessThanExpression(fechaSelector, fechaHasta.Value.Date.AddDays(1)));
        return query;
    }
    private static Expression<Func<T, bool>> BuildContainsExpression<T>(Expression<Func<T, string>> propertySelector, string value)
    {
        var parameter = propertySelector.Parameters[0];
        var property = propertySelector.Body;
        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
        var valueExpression = Expression.Constant(value);
        var containsCall = Expression.Call(property, containsMethod, valueExpression);
        return Expression.Lambda<Func<T, bool>>(containsCall, parameter);
    }
    private static Expression<Func<T, bool>> BuildNotExpression<T>(Expression<Func<T, bool>> expression)
    {
        var parameter = expression.Parameters[0];
        var notExpression = Expression.Not(expression.Body);
        return Expression.Lambda<Func<T, bool>>(notExpression, parameter);
    }
    private static Expression<Func<T, bool>> BuildGreaterThanOrEqualExpression<T>(Expression<Func<T, DateTime>> propertySelector, DateTime value)
    {
        var parameter = propertySelector.Parameters[0];
        var property = propertySelector.Body;
        var valueExpression = Expression.Constant(value);
        var comparison = Expression.GreaterThanOrEqual(property, valueExpression);
        return Expression.Lambda<Func<T, bool>>(comparison, parameter);
    }
    private static Expression<Func<T, bool>> BuildLessThanExpression<T>(Expression<Func<T, DateTime>> propertySelector, DateTime value)
    {
        var parameter = propertySelector.Parameters[0];
        var property = propertySelector.Body;
        var valueExpression = Expression.Constant(value);
        var comparison = Expression.LessThan(property, valueExpression);
        return Expression.Lambda<Func<T, bool>>(comparison, parameter);
    }
}
