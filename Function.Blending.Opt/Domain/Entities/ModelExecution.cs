namespace Function.Blending.Opt.Domain.Entities;

public class ModelExecution
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Planta { get; set; }
    public string TipoModelo { get; set; }
    public string Estado { get; set; } = "EN_PROCESO";
    public DateTime FechaInicio { get; set; } = DateTime.UtcNow;
}

