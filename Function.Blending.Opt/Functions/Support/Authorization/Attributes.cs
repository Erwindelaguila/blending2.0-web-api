using Function.Blending.Opt.Functions.Support.Http;
using System;

namespace Function.Blending.Opt.Functions.Support.Authorization;

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

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class ValidateHmacAttribute : Attribute
{
  public string HeaderName { get; }
  public ValidateHmacAttribute(string headerName = HmacKeys.XSignatureHeaderKey) => HeaderName = headerName;
}
