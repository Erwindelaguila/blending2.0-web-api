using System.Security.Claims;

namespace Function.Blending.Opt.Shared.Constants;

public static class ClaimTypesKeys
{
  public const string Oid = "oid";
  public const string Sub = "sub";
  public const string Name = "name";
  public const string UniqueName = "unique_name";
  public const string PreferredUsername = "preferred_username";
  public const string GivenName = "given_name";
  public const string FamilyName = "family_name";
  public const string Upn = "upn";
  public const string Azp = "azp";
  public const string AppId = "appid";
  public const string ClientId = "clientid";
  public const string Scp = "scp";
  public const string Scope = "scope";
  public const string Groups = "groups";
  public const string Wids = "wids";
  public const string Roles = "roles";
  public const string AuthMode = "auth_mode";
}
