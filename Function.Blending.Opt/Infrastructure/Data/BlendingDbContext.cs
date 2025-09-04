using Function.Blending.Opt.Infrastructure.Data.Scaffolded;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Opt.Infrastructure.Data;

public partial class BlendingDbContext : DbContext
{
    public BlendingDbContext(DbContextOptions<BlendingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ModelExecution> ModelExecutions { get; set; }

    public virtual DbSet<ModelExecutionLog> ModelExecutionLogs { get; set; }

    public virtual DbSet<ModelExecutionResult> ModelExecutionResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ModelExecution>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ModelExe__3214EC076774FD69");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("EN_PROCESO");
            entity.Property(e => e.FechaInicio).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Planta).HasMaxLength(100);
            entity.Property(e => e.TipoModelo).HasMaxLength(50);
        });

        modelBuilder.Entity<ModelExecutionLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ModelExe__3214EC0786995C6F");

            entity.Property(e => e.Evento).HasMaxLength(100);
            entity.Property(e => e.FechaEvento).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Execution).WithMany(p => p.ModelExecutionLogs)
                .HasForeignKey(d => d.ExecutionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Logs_Execution");
        });

        modelBuilder.Entity<ModelExecutionResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ModelExe__3214EC07A562FED4");

            entity.Property(e => e.ArchivoBlobUrl).HasMaxLength(500);
            entity.Property(e => e.FechaGeneracion).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Execution).WithMany(p => p.ModelExecutionResults)
                .HasForeignKey(d => d.ExecutionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Results_Execution");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}