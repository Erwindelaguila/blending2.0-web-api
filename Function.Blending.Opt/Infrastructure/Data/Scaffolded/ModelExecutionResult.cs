namespace Function.Blending.Opt.Infrastructure.Data.Scaffolded;

public partial class ModelExecutionResult
{
    public int Id { get; set; }

    public Guid ExecutionId { get; set; }

    public string ResultadoJson { get; set; } = null!;

    public string? ArchivoBlobUrl { get; set; }

    public DateTime FechaGeneracion { get; set; }

    public virtual ModelExecution Execution { get; set; } = null!;
}
