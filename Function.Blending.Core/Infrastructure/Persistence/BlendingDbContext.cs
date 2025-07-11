using System;
using System.Collections.Generic;
using Function.Blending.Core.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Core.Infrastructure.Persistence;

public partial class BlendingDbContext : DbContext
{
    public BlendingDbContext(DbContextOptions<BlendingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agregado> Agregados { get; set; }

    public virtual DbSet<Calidad> Calidads { get; set; }

    public virtual DbSet<CalidadParametro> CalidadParametros { get; set; }

    public virtual DbSet<LineaProduccion> LineaProduccions { get; set; }

    public virtual DbSet<Parametro> Parametros { get; set; }

    public virtual DbSet<Plantum> Planta { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<TipoProduccion> TipoProduccions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agregado>(entity =>
        {
            entity.ToTable("Agregado");

            entity.HasIndex(e => new { e.Activo, e.Nombre }, "IX_Agregado_Activo_Nombre_COVERING").HasFilter("([Activo]=(1))");

            entity.HasIndex(e => e.Codigo, "UQ_Agregado_Codigo").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Calidad>(entity =>
        {
            entity.ToTable("Calidad");

            entity.HasIndex(e => new { e.Activo, e.Nombre }, "IX_Calidad_Activo_Nombre_COVERING").HasFilter("([Activo]=(1))");

            entity.HasIndex(e => new { e.CodigoMaterial, e.Activo }, "IX_Calidad_CodigoMaterial_Activo").HasFilter("([CodigoMaterial] IS NOT NULL)");

            entity.HasIndex(e => new { e.NoConforme, e.Activo }, "IX_Calidad_NoConforme_Activo");

            entity.HasIndex(e => e.Codigo, "UQ_Calidad_Codigo").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CodigoMaterial)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CalidadParametro>(entity =>
        {
            entity.ToTable("CalidadParametro");

            entity.HasIndex(e => new { e.CalidadId, e.Activo }, "IX_CalidadParametro_CalidadId_Activo");

            entity.HasIndex(e => new { e.CalidadId, e.ParametroId, e.Activo }, "IX_CalidadParametro_Calidad_Parametro_COVERING").HasFilter("([Activo]=(1))");

            entity.HasIndex(e => new { e.CalidadId, e.ParametroId }, "IX_CalidadParametro_Calidad_Parametro_UNIQUE")
                .IsUnique()
                .HasFilter("([Activo]=(1))");

            entity.HasIndex(e => new { e.ParametroId, e.Activo }, "IX_CalidadParametro_ParametroId_Activo");

            entity.HasIndex(e => new { e.Valor, e.Activo }, "IX_CalidadParametro_Valor_Activo");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Valor).HasColumnType("decimal(10, 4)");

            entity.HasOne(d => d.Calidad).WithMany(p => p.CalidadParametros)
                .HasForeignKey(d => d.CalidadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CalidadParametro_CalidadId");

            entity.HasOne(d => d.Parametro).WithMany(p => p.CalidadParametros)
                .HasForeignKey(d => d.ParametroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CalidadParametro_ParametroId");
        });

        modelBuilder.Entity<LineaProduccion>(entity =>
        {
            entity.ToTable("LineaProduccion");

            entity.HasIndex(e => new { e.Activo, e.Codigo }, "IX_LineaProduccion_Activo_Codigo").HasFilter("([Activo]=(1))");

            entity.HasIndex(e => e.CreadoEl, "IX_LineaProduccion_CreadoEl_DESC").IsDescending();

            entity.HasIndex(e => e.Codigo, "UQ_LineaProduccion_Codigo").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Parametro>(entity =>
        {
            entity.ToTable("Parametro");

            entity.HasIndex(e => new { e.Activo, e.Nombre }, "IX_Parametro_Activo_Nombre_COVERING").HasFilter("([Activo]=(1))");

            entity.HasIndex(e => new { e.CreadoEl, e.ModificadoEl }, "IX_Parametro_CreadoEl_ModificadoEl").IsDescending();

            entity.HasIndex(e => e.Codigo, "UQ_Parametro_Codigo").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Plantum>(entity =>
        {
            entity.HasIndex(e => new { e.Activo, e.Nombre }, "IX_Planta_Activo_Nombre").HasFilter("([Activo]=(1))");

            entity.HasIndex(e => new { e.CreadoEl, e.CreadoPorId }, "IX_Planta_CreadoEl_CreadoPorId").IsDescending(true, false);

            entity.HasIndex(e => new { e.NumeroRuma, e.Activo }, "IX_Planta_NumeroRuma_Activo");

            entity.HasIndex(e => e.Codigo, "UQ_Planta_Codigo").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Producto");

            entity.HasIndex(e => new { e.Activo, e.Nombre }, "IX_Producto_Activo_Nombre_COVERING").HasFilter("([Activo]=(1))");

            entity.HasIndex(e => new { e.CalidadId, e.Activo }, "IX_Producto_CalidadId_Activo");

            entity.HasIndex(e => new { e.CalidadId, e.TipoProduccionId, e.Activo }, "IX_Producto_Calidad_TipoProduccion_Activo");

            entity.HasIndex(e => new { e.CreadoEl, e.ModificadoEl }, "IX_Producto_CreadoEl_ModificadoEl").IsDescending();

            entity.HasIndex(e => new { e.TipoProduccionId, e.Activo }, "IX_Producto_TipoProduccionId_Activo");

            entity.HasIndex(e => e.Codigo, "UQ_Producto_Codigo").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Calidad).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CalidadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_CalidadId");

            entity.HasOne(d => d.TipoProduccion).WithMany(p => p.Productos)
                .HasForeignKey(d => d.TipoProduccionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_TipoProduccionId");
        });

        modelBuilder.Entity<TipoProduccion>(entity =>
        {
            entity.ToTable("TipoProduccion");

            entity.HasIndex(e => new { e.AgregadoId, e.Activo }, "IX_TipoProduccion_AgregadoId_Activo");

            entity.HasIndex(e => new { e.CreadoEl, e.Activo }, "IX_TipoProduccion_CreadoEl_Activo").IsDescending(true, false);

            entity.HasIndex(e => new { e.LineaProduccionId, e.Activo }, "IX_TipoProduccion_LineaProduccionId_Activo");

            entity.HasIndex(e => new { e.LineaProduccionId, e.AgregadoId, e.Activo }, "IX_TipoProduccion_LineaProduccion_Agregado_Activo");

            entity.HasIndex(e => e.Codigo, "UQ_TipoProduccion_Codigo").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Agregado).WithMany(p => p.TipoProduccions)
                .HasForeignKey(d => d.AgregadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoProduccion_AgregadoId");

            entity.HasOne(d => d.LineaProduccion).WithMany(p => p.TipoProduccions)
                .HasForeignKey(d => d.LineaProduccionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoProduccion_LineaProduccionId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
