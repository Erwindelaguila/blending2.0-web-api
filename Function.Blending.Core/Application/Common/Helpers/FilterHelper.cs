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
    public static IQueryable<T> ApplyFechaRangeFilterConTipo<T>(this IQueryable<T> query, DateTime? fechaDesde, DateTime? fechaHasta, string? tipoFecha, Expression<Func<T, DateTime>> fechaCreadoSelector, Expression<Func<T, DateTime?>> fechaModificadoSelector)
    {
        if (!fechaDesde.HasValue && !fechaHasta.HasValue) return query;
        return tipoFecha?.ToLowerInvariant() switch
        {
            "todos" => ApplyFechaRangeFilterTodos(query, fechaDesde, fechaHasta, fechaCreadoSelector, fechaModificadoSelector),
            "modificados" => ApplyFechaRangeFilterModificados(query, fechaDesde, fechaHasta, fechaModificadoSelector),
            "creados" or null => query.ApplyFechaRangeFilter(fechaDesde, fechaHasta, fechaCreadoSelector),
            _ => throw new ArgumentException($"Tipo de fecha inválido: '{tipoFecha}'. Use 'creados', 'modificados' o 'todos'.", nameof(tipoFecha))
        };
    }
    private static IQueryable<T> ApplyFechaRangeFilterTodos<T>(IQueryable<T> query, DateTime? fechaDesde, DateTime? fechaHasta, Expression<Func<T, DateTime>> fechaCreadoSelector, Expression<Func<T, DateTime?>> fechaModificadoSelector)
    {
        var fechaDesdeStart = fechaDesde?.Date;
        var fechaHastaEnd = fechaHasta?.Date.AddDays(1);
        var parameter = fechaCreadoSelector.Parameters[0];
        Expression? condicionCreado = null;
        if (fechaDesdeStart.HasValue)
            condicionCreado = BuildGreaterThanOrEqualExpression(fechaCreadoSelector, fechaDesdeStart.Value).Body;
        if (fechaHastaEnd.HasValue)
        {
            var condicionCreadoHasta = BuildLessThanExpression(fechaCreadoSelector, fechaHastaEnd.Value).Body;
            condicionCreado = condicionCreado == null ? condicionCreadoHasta : Expression.AndAlso(condicionCreado, condicionCreadoHasta);
        }
        Expression? condicionModificado = null;
        var modificadoNotNull = Expression.NotEqual(fechaModificadoSelector.Body, Expression.Constant(null, typeof(DateTime?)));
        if (fechaDesdeStart.HasValue)
        {
            var modificadoDesde = BuildGreaterThanOrEqualExpression(Expression.Lambda<Func<T, DateTime>>(Expression.Convert(fechaModificadoSelector.Body, typeof(DateTime)), parameter), fechaDesdeStart.Value).Body;
            condicionModificado = Expression.AndAlso(modificadoNotNull, modificadoDesde);
        }
        if (fechaHastaEnd.HasValue)
        {
            var modificadoHasta = BuildLessThanExpression(Expression.Lambda<Func<T, DateTime>>(Expression.Convert(fechaModificadoSelector.Body, typeof(DateTime)), parameter), fechaHastaEnd.Value).Body;
            var condicionModificadoHasta = Expression.AndAlso(modificadoNotNull, modificadoHasta);
            condicionModificado = condicionModificado == null ? condicionModificadoHasta : Expression.AndAlso(condicionModificado, condicionModificadoHasta);
        }
        var condicionFinal = condicionCreado != null && condicionModificado != null ? Expression.OrElse(condicionCreado, condicionModificado) : condicionCreado ?? condicionModificado;
        if (condicionFinal != null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(condicionFinal, parameter);
            query = query.Where(lambda);
        }
        return query;
    }
    private static IQueryable<T> ApplyFechaRangeFilterModificados<T>(IQueryable<T> query, DateTime? fechaDesde, DateTime? fechaHasta, Expression<Func<T, DateTime?>> fechaModificadoSelector)
    {
        var parameter = fechaModificadoSelector.Parameters[0];
        var modificadoNotNull = Expression.Lambda<Func<T, bool>>(Expression.NotEqual(fechaModificadoSelector.Body, Expression.Constant(null, typeof(DateTime?))), parameter);
        query = query.Where(modificadoNotNull);
        if (fechaDesde.HasValue)
        {
            var fechaDesdeStart = fechaDesde.Value.Date;
            var modificadoDesdeCondition = Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(Expression.Convert(fechaModificadoSelector.Body, typeof(DateTime)), Expression.Constant(fechaDesdeStart)), parameter);
            query = query.Where(modificadoDesdeCondition);
        }
        if (fechaHasta.HasValue)
        {
            var fechaHastaEnd = fechaHasta.Value.Date.AddDays(1);
            var modificadoHastaCondition = Expression.Lambda<Func<T, bool>>(Expression.LessThan(Expression.Convert(fechaModificadoSelector.Body, typeof(DateTime)), Expression.Constant(fechaHastaEnd)), parameter);
            query = query.Where(modificadoHastaCondition);
        }
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
