namespace Function.Blending.Core.Infrastructure.Security.Support.Authorization;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class AllowAnonymousAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class WebhookAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class RequireScopesAttribute : Attribute
{
  public string[] Scopes { get; }
  public RequireScopesAttribute(params string[] scopes) => Scopes = scopes ?? Array.Empty<string>();
}
