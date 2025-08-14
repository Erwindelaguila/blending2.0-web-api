namespace Function.Blending.Opt.Infrastructure.Data.Scaffolded;

public partial class ModelExecutionLog
{
    public int Id { get; set; }

    public Guid ExecutionId { get; set; }

    public string Evento { get; set; } = null!;

    public string? Mensaje { get; set; }

    public DateTime FechaEvento { get; set; }

    public virtual ModelExecution Execution { get; set; } = null!;
}
