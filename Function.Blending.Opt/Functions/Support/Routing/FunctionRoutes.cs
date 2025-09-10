namespace Function.Blending.Opt.Functions.Support.Routing;

public static class FunctionRoutes
{
  // El host antepone "api" por el routePrefix por defecto. Aquí no ponemos "api/"
  public const string ApiBase = "";

  public static class Health
  {
    public const string Check = $"{ApiBase}health";
  }

  public static class Quality
  {
    public const string Start = $"{ApiBase}quality/homogenization/start";
    public const string Webhook = $"{ApiBase}quality/homogenization/webhook";
    public const string GetById = $"{ApiBase}quality/executions/{{id:guid}}";
    public const string History = $"{ApiBase}quality/executions/history";
    public const string ToggleState = $"{ApiBase}quality/executions/{{id:guid}}/state/toggle";
  }

  public static class Logistics
  {
    public const string Start = $"{ApiBase}logistics/homogenization/start";
    public const string Webhook = $"{ApiBase}logistics/homogenization/webhook";
    public const string GetById = $"{ApiBase}logistics/executions/{{id:guid}}";
    public const string History = $"{ApiBase}logistics/executions/history";
    public const string ToggleConfirmed = $"{ApiBase}logistics/executions/{{id:guid}}/confirmed/toggle";
  }
}
