namespace Function.Blending.Opt.Infrastructure.Data.Scaffolded;

public partial class ModelExecution
{
    public Guid Id { get; set; }

    public string Planta { get; set; } = null!;

    public string TipoModelo { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public string? MensajeResultado { get; set; }

    public virtual ICollection<ModelExecutionLog> ModelExecutionLogs { get; set; } = new List<ModelExecutionLog>();

    public virtual ICollection<ModelExecutionResult> ModelExecutionResults { get; set; } = new List<ModelExecutionResult>();
}
