namespace Function.Blending.Core.Application.Interfaces.Services;

/// <summary>
/// Servicio de auditoría para registrar todas las acciones del usuario.
/// Registra automáticamente quién hizo qué y cuándo para compliance.
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Registra una acción de creación.
    /// </summary>
    /// <param name="entityName">Nombre de la entidad (ej: "AppParam")</param>
    /// <param name="entityId">ID de la entidad creada</param>
    /// <param name="data">Datos de la entidad creada</param>
    Task LogCreateAsync(string entityName, string entityId, object data);
    
    /// <summary>
    /// Registra una acción de actualización.
    /// </summary>
    /// <param name="entityName">Nombre de la entidad</param>
    /// <param name="entityId">ID de la entidad actualizada</param>
    /// <param name="oldData">Datos anteriores</param>
    /// <param name="newData">Datos nuevos</param>
    Task LogUpdateAsync(string entityName, string entityId, object oldData, object newData);
    
    /// <summary>
    /// Registra una acción de eliminación.
    /// </summary>
    /// <param name="entityName">Nombre de la entidad</param>
    /// <param name="entityId">ID de la entidad eliminada</param>
    /// <param name="data">Datos de la entidad eliminada</param>
    Task LogDeleteAsync(string entityName, string entityId, object data);
    
    /// <summary>
    /// Registra una acción de consulta.
    /// </summary>
    /// <param name="entityName">Nombre de la entidad consultada</param>
    /// <param name="filters">Filtros aplicados</param>
    Task LogReadAsync(string entityName, object? filters = null);
}
