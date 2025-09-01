namespace Function.Blending.Core.Application.Common.Exceptions;

public class BusinessRuleException : Exception
{
    public string ErrorCode { get; }

    public BusinessRuleException(string message, string errorCode = "BUSINESS_RULE_VIOLATION") 
        : base(message)
    {
        ErrorCode = errorCode;
    }
}

public class EntityInUseException : BusinessRuleException
{
    public EntityInUseException(string entityType, string reason) 
        : base($"No se puede inactivar/eliminar {entityType}: {reason}", "ENTITY_IN_USE")
    {
    }
}

public class DuplicateKeyException : BusinessRuleException
{
    public DuplicateKeyException(string entityType, string key) 
        : base($"Ya existe un {entityType} con el código '{key}'", "DUPLICATE_KEY")
    {
    }
}
