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

    public virtual DbSet<Agregado> Agregado { get; set; }

    public virtual DbSet<AppParam> AppParam { get; set; }

    public virtual DbSet<AuxProp> AuxProp { get; set; }

    public virtual DbSet<AuxRow> AuxRow { get; set; }

    public virtual DbSet<AuxTable> AuxTable { get; set; }

    public virtual DbSet<AuxValue> AuxValue { get; set; }

    public virtual DbSet<CalEjecucion> CalEjecucion { get; set; }

    public virtual DbSet<CalInpFiltro> CalInpFiltro { get; set; }

    public virtual DbSet<CalInpParametro> CalInpParametro { get; set; }

    public virtual DbSet<CalOutDetOtros> CalOutDetOtros { get; set; }

    public virtual DbSet<CalOutDetParametro> CalOutDetParametro { get; set; }

    public virtual DbSet<CalOutDetalle> CalOutDetalle { get; set; }

    public virtual DbSet<CalOutResParametro> CalOutResParametro { get; set; }

    public virtual DbSet<CalOutResumen> CalOutResumen { get; set; }

    public virtual DbSet<Calidad> Calidad { get; set; }

    public virtual DbSet<CalidadParametro> CalidadParametro { get; set; }

    public virtual DbSet<LineaProduccion> LineaProduccion { get; set; }

    public virtual DbSet<LogEjecucion> LogEjecucion { get; set; }

    public virtual DbSet<LogInpFilCapacidad> LogInpFilCapacidad { get; set; }

    public virtual DbSet<LogInpFilDivision> LogInpFilDivision { get; set; }

    public virtual DbSet<LogInpFilEmparejamiento> LogInpFilEmparejamiento { get; set; }

    public virtual DbSet<LogInpFiltro> LogInpFiltro { get; set; }

    public virtual DbSet<LogInpInfo> LogInpInfo { get; set; }

    public virtual DbSet<LogInpOfeParametro> LogInpOfeParametro { get; set; }

    public virtual DbSet<LogInpOferta> LogInpOferta { get; set; }

    public virtual DbSet<LogOutConComposicion> LogOutConComposicion { get; set; }

    public virtual DbSet<LogOutConDistribucion> LogOutConDistribucion { get; set; }

    public virtual DbSet<LogOutContenedor> LogOutContenedor { get; set; }

    public virtual DbSet<Parametro> Parametro { get; set; }

    public virtual DbSet<Planta> Planta { get; set; }

    public virtual DbSet<Producto> Producto { get; set; }

    public virtual DbSet<SysLog> SysLog { get; set; }

    public virtual DbSet<SysParam> SysParam { get; set; }

    public virtual DbSet<TipoProduccion> TipoProduccion { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agregado>(entity =>
        {
            entity.HasIndex(e => e.Codigo, "UQ_Agregado_Codigo_Activo")
                .IsUnique()
                .HasFilter("([Eliminado]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ModificadoPorId).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AppParam>(entity =>
        {
            entity.HasKey(e => e.Key);

            entity.HasIndex(e => e.Category, "IX_AppParam_Category");

            entity.HasIndex(e => new { e.Category, e.Group }, "IX_AppParam_Category_Group");

            entity.HasIndex(e => e.Group, "IX_AppParam_Group");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Description).HasMaxLength(150);
            entity.Property(e => e.Group).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDisableable).HasDefaultValue(true);
            entity.Property(e => e.IsRemovable).HasDefaultValue(true);
            entity.Property(e => e.IsVisible).HasDefaultValue(true);
            entity.Property(e => e.Value)
                .HasMaxLength(250)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AuxProp>(entity =>
        {
            entity.HasIndex(e => e.Nombre, "IX_AuxProp_Nombre");

            entity.HasIndex(e => new { e.TableId, e.Orden }, "IX_AuxProp_Table_Orden");

            entity.HasIndex(e => new { e.TableId, e.Clave }, "UQ_AuxProp_TableId_Clave").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Clave).HasMaxLength(15);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.TipoDato).HasMaxLength(25);

            entity.HasOne(d => d.Table).WithMany(p => p.AuxProp)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuxProp_AuxTable");
        });

        modelBuilder.Entity<AuxRow>(entity =>
        {
            entity.HasIndex(e => e.Nombre, "IX_AuxRow_Nombre");

            entity.HasIndex(e => e.PadreId, "IX_AuxRow_PadreId");

            entity.HasIndex(e => new { e.TableId, e.Orden }, "IX_AuxRow_Table_Orden");

            entity.HasIndex(e => new { e.TableId, e.Clave }, "UQ_AuxRow_TableId_Clave").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Clave).HasMaxLength(15);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(100);

            entity.HasOne(d => d.Padre).WithMany(p => p.InversePadre)
                .HasForeignKey(d => d.PadreId)
                .HasConstraintName("FK_AuxRow_AuxRow");

            entity.HasOne(d => d.Table).WithMany(p => p.AuxRow)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuxRow_AuxTable");
        });

        modelBuilder.Entity<AuxTable>(entity =>
        {
            entity.HasIndex(e => e.Clave, "UQ_AuxTable_Clave").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Clave).HasMaxLength(15);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(100);

            entity.HasOne(d => d.Padre).WithMany(p => p.InversePadre)
                .HasForeignKey(d => d.PadreId)
                .HasConstraintName("FK_AuxTable_AuxTable");
        });

        modelBuilder.Entity<AuxValue>(entity =>
        {
            entity.HasIndex(e => e.PropId, "IX_AuxValue_Prop");

            entity.HasIndex(e => e.RowId, "IX_AuxValue_Row");

            entity.HasIndex(e => new { e.RowId, e.PropId }, "IX_AuxValue_Row_Prop");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Valor).HasMaxLength(250);

            entity.HasOne(d => d.Prop).WithMany(p => p.AuxValue)
                .HasForeignKey(d => d.PropId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuxValue_AuxProp");

            entity.HasOne(d => d.Row).WithMany(p => p.AuxValue)
                .HasForeignKey(d => d.RowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuxValue_AuxRow");
        });

        modelBuilder.Entity<CalEjecucion>(entity =>
        {
            entity.HasIndex(e => e.EstadoId, "IX_CalEjecucion_EstadoId");

            entity.HasIndex(e => e.PlantaId, "IX_CalEjecucion_PlantaId");

            entity.HasIndex(e => e.Codigo, "UQ_CalEjecucion_Codigo").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Codigo).HasMaxLength(20);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Mensaje).HasMaxLength(250);

            entity.HasOne(d => d.Planta).WithMany(p => p.CalEjecucion)
                .HasForeignKey(d => d.PlantaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CalEjecucion_Planta");
        });

        modelBuilder.Entity<CalInpFiltro>(entity =>
        {
            entity.HasIndex(e => e.EjecucionId, "IX_CalInpFiltro_EjecucionId");

            entity.HasIndex(e => e.EjecucionId, "UQ_CalInpFiltro_EjecucionId").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BorrarCalidades).HasMaxLength(200);
            entity.Property(e => e.CentroProduccion).HasMaxLength(200);
            entity.Property(e => e.CentroUbicacion).HasMaxLength(200);
            entity.Property(e => e.QuitarRumasPH).HasDefaultValue(true);
            entity.Property(e => e.TipoProduccion).HasMaxLength(200);
            entity.Property(e => e.UbicacionAlmacen).HasMaxLength(200);
            entity.Property(e => e.ValorCadmioAlto).HasColumnType("decimal(10, 4)");

            entity.HasOne(d => d.Ejecucion).WithOne(p => p.CalInpFiltro)
                .HasForeignKey<CalInpFiltro>(d => d.EjecucionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CalInpFiltro_Ejecucion");
        });

        modelBuilder.Entity<CalInpParametro>(entity =>
        {
            entity.HasIndex(e => e.ParametroId, "IX_CalInpParametro_ParametroId");

            entity.HasIndex(e => new { e.EjecucionId, e.CalidadId, e.ParametroId }, "UQ_CalInpParametro_Ejecucion_Calidad_Parametro").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Valor).HasColumnType("decimal(10, 4)");

            entity.HasOne(d => d.Calidad).WithMany(p => p.CalInpParametro)
                .HasForeignKey(d => d.CalidadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CalInpParametro_Calidad");

            entity.HasOne(d => d.Ejecucion).WithMany(p => p.CalInpParametro)
                .HasForeignKey(d => d.EjecucionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CalInpParametro_Ejecucion");

            entity.HasOne(d => d.Parametro).WithMany(p => p.CalInpParametro)
                .HasForeignKey(d => d.ParametroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CalInpParametro_Parametro");
        });

        modelBuilder.Entity<CalOutDetOtros>(entity =>
        {
            entity.HasIndex(e => e.DetalleId, "IX_CalOutDetOtros_DetalleId");

            entity.HasIndex(e => new { e.DetalleId, e.Codigo }, "UQ_CalInpParametro_DetalleId_Codigo").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Codigo).HasMaxLength(20);
            entity.Property(e => e.Valor).HasMaxLength(50);

            entity.HasOne(d => d.Detalle).WithMany(p => p.CalOutDetOtros)
                .HasForeignKey(d => d.DetalleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CalOutDetOtros_Detalle");
        });

        modelBuilder.Entity<CalOutDetParametro>(entity =>
        {
            entity.HasIndex(e => e.DetalleId, "IX_CalOutDetParametro_DetalleId");

            entity.HasIndex(e => new { e.DetalleId, e.CodigoParametro }, "UQ_CalInpParametro_DetalleId_CodigoParametro").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CodigoParametro).HasMaxLength(20);
            entity.Property(e => e.Valor).HasColumnType("decimal(10, 4)");

            entity.HasOne(d => d.Detalle).WithMany(p => p.CalOutDetParametro)
                .HasForeignKey(d => d.DetalleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CalOutDetParametro_Detalle");
        });

        modelBuilder.Entity<CalOutDetalle>(entity =>
        {
            entity.HasIndex(e => e.EjecucionId, "IX_CalOutDetalle_EjecucionId");

            entity.HasIndex(e => e.Grupo, "IX_CalOutDetalle_Grupo");

            entity.HasIndex(e => e.Ruma, "IX_CalOutDetalle_Ruma");

            entity.HasIndex(e => new { e.EjecucionId, e.Ruma, e.Grupo }, "UQ_CalInpParametro_EjecucionId_Ruma_Grupo").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AlmacenUbicacion).HasMaxLength(200);
            entity.Property(e => e.CentroUbicacion).HasMaxLength(200);
            entity.Property(e => e.Codigo).HasMaxLength(20);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.DescripcionMaterial).HasMaxLength(200);
            entity.Property(e => e.FechaContabilizacion).HasMaxLength(100);
            entity.Property(e => e.Grupo).HasMaxLength(20);
            entity.Property(e => e.Ruma).HasMaxLength(20);

            entity.HasOne(d => d.Ejecucion).WithMany(p => p.CalOutDetalle)
                .HasForeignKey(d => d.EjecucionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CalOutDetalle_Ejecucion");
        });

        modelBuilder.Entity<CalOutResParametro>(entity =>
        {
            entity.HasIndex(e => e.ResumenId, "IX_CalOutResParametro_ResumenId");

            entity.HasIndex(e => new { e.ResumenId, e.CodigoParametro }, "UQ_CalInpParametro_ResumenId_CodigoParametro").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CodigoParametro).HasMaxLength(20);
            entity.Property(e => e.Valor).HasColumnType("decimal(10, 4)");

            entity.HasOne(d => d.Resumen).WithMany(p => p.CalOutResParametro)
                .HasForeignKey(d => d.ResumenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CalOutResParametro_Resumen");
        });

        modelBuilder.Entity<CalOutResumen>(entity =>
        {
            entity.HasIndex(e => e.EjecucionId, "IX_CalOutResumen_EjecucionId");

            entity.HasIndex(e => e.Grupo, "IX_CalOutResumen_Grupo");

            entity.HasIndex(e => new { e.EjecucionId, e.Grupo }, "UQ_CalInpParametro_EjecucionId_Grupo").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CodigoCalidadObjetivo).HasMaxLength(20);
            entity.Property(e => e.CodigoCalidadResultante).HasMaxLength(20);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Grupo).HasMaxLength(20);

            entity.HasOne(d => d.Ejecucion).WithMany(p => p.CalOutResumen)
                .HasForeignKey(d => d.EjecucionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CalOutResumen_Ejecucion");
        });

        modelBuilder.Entity<Calidad>(entity =>
        {
            entity.HasIndex(e => e.Codigo, "UQ_Calidad_Codigo_Activo")
                .IsUnique()
                .HasFilter("([Eliminado]=(0))");

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
            entity.Property(e => e.ModificadoPorId).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CalidadParametro>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Valor).HasColumnType("decimal(10, 4)");

            entity.HasOne(d => d.Calidad).WithMany(p => p.CalidadParametro)
                .HasForeignKey(d => d.CalidadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CalidadParametro_CalidadId");

            entity.HasOne(d => d.Parametro).WithMany(p => p.CalidadParametro)
                .HasForeignKey(d => d.ParametroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CalidadParametro_ParametroId");
        });

        modelBuilder.Entity<LineaProduccion>(entity =>
        {
            entity.HasIndex(e => e.Codigo, "UQ_LineaProduccion_Codigo_Activo")
                .IsUnique()
                .HasFilter("([Eliminado]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ModificadoPorId).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<LogEjecucion>(entity =>
        {
            entity.HasIndex(e => e.AsociadoId, "IX_LogEjecucion_AsociadoId");

            entity.HasIndex(e => e.EstadoId, "IX_LogEjecucion_EstadoId");

            entity.HasIndex(e => e.PlantaId, "IX_LogEjecucion_PlantaId");

            entity.HasIndex(e => e.Codigo, "UQ_LogEjecucion_Codigo").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Codigo).HasMaxLength(20);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Grupo).HasMaxLength(20);
            entity.Property(e => e.Mensaje).HasMaxLength(250);

            entity.HasOne(d => d.Asociado).WithMany(p => p.InverseAsociado)
                .HasForeignKey(d => d.AsociadoId)
                .HasConstraintName("FK_LogEjecucion_Asociado");

            entity.HasOne(d => d.Planta).WithMany(p => p.LogEjecucion)
                .HasForeignKey(d => d.PlantaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogEjecucion_Planta");
        });

        modelBuilder.Entity<LogInpFilCapacidad>(entity =>
        {
            entity.HasIndex(e => e.FiltroId, "IX_LogInpFilCapacidad_FiltroId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Filtro).WithMany(p => p.LogInpFilCapacidad)
                .HasForeignKey(d => d.FiltroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogInpFilCapacidad_Filtro");
        });

        modelBuilder.Entity<LogInpFilDivision>(entity =>
        {
            entity.HasIndex(e => e.FiltroId, "IX_LogInpFilDivision_FiltroId");

            entity.HasIndex(e => new { e.FiltroId, e.Ruma }, "UQ_LogInpFilDivision_FiltroId_Ruma").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Division).HasMaxLength(50);
            entity.Property(e => e.Ruma).HasMaxLength(20);

            entity.HasOne(d => d.Filtro).WithMany(p => p.LogInpFilDivision)
                .HasForeignKey(d => d.FiltroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogInpFilDivision_Filtro");
        });

        modelBuilder.Entity<LogInpFilEmparejamiento>(entity =>
        {
            entity.HasIndex(e => e.ParametroId, "IX_LogInpFilEmparejamiento_ParametroId");

            entity.HasIndex(e => new { e.FiltroId, e.Grupo, e.ParametroId }, "UQ_LogInpFilEmparejamiento_FiltroId_Grupo_ParametroId").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Grupo).HasMaxLength(20);
            entity.Property(e => e.Valor).HasColumnType("decimal(10, 4)");

            entity.HasOne(d => d.Filtro).WithMany(p => p.LogInpFilEmparejamiento)
                .HasForeignKey(d => d.FiltroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogInpFilEmparejamiento_Filtro");

            entity.HasOne(d => d.Parametro).WithMany(p => p.LogInpFilEmparejamiento)
                .HasForeignKey(d => d.ParametroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogInpFilEmparejamiento_Parametro");
        });

        modelBuilder.Entity<LogInpFiltro>(entity =>
        {
            entity.HasIndex(e => e.EjecucionId, "IX_LogInpFiltro_EjecucionId");

            entity.HasIndex(e => e.EjecucionId, "UQ_LogInpFiltro_EjecucionId").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Division).HasMaxLength(20);
            entity.Property(e => e.Parametros).HasMaxLength(200);

            entity.HasOne(d => d.Ejecucion).WithOne(p => p.LogInpFiltro)
                .HasForeignKey<LogInpFiltro>(d => d.EjecucionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogInpFiltro_Ejecucion");
        });

        modelBuilder.Entity<LogInpInfo>(entity =>
        {
            entity.HasIndex(e => e.AlmacenCodigo, "IX_LogInpInfo_AlmacenCodigo");

            entity.HasIndex(e => e.Clienta, "IX_LogInpInfo_Clienta");

            entity.HasIndex(e => e.Contrato, "IX_LogInpInfo_Contrato");

            entity.HasIndex(e => e.FechaCarguio, "IX_LogInpInfo_FechaCarguio");

            entity.HasIndex(e => e.PaisDestino, "IX_LogInpInfo_PaisDestino");

            entity.HasIndex(e => e.PlantaCodigo, "IX_LogInpInfo_PlantaCodigo");

            entity.HasIndex(e => e.EjecucionId, "UQ_LogInpInfo_EjecucionId").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AlmacenCodigo).HasMaxLength(20);
            entity.Property(e => e.AlmacenDescripcion).HasMaxLength(100);
            entity.Property(e => e.Asistente).HasMaxLength(100);
            entity.Property(e => e.Clienta).HasMaxLength(100);
            entity.Property(e => e.Contrato).HasMaxLength(50);
            entity.Property(e => e.PaisDestino).HasMaxLength(50);
            entity.Property(e => e.PedidoVenta).HasMaxLength(20);
            entity.Property(e => e.PlantaCodigo).HasMaxLength(20);
            entity.Property(e => e.PlantaDescripcion).HasMaxLength(100);
            entity.Property(e => e.Supervisora).HasMaxLength(100);
            entity.Property(e => e.UnidadMedidaRuma).HasMaxLength(10);

            entity.HasOne(d => d.Ejecucion).WithOne(p => p.LogInpInfo)
                .HasForeignKey<LogInpInfo>(d => d.EjecucionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogInpInfo_EjecucionId");
        });

        modelBuilder.Entity<LogInpOfeParametro>(entity =>
        {
            entity.HasIndex(e => e.OfertaId, "IX_LogInpOfeParametro_OfertaId");

            entity.HasIndex(e => new { e.OfertaId, e.CodigoParametro }, "UQ_LogInpOfeParametro_OfertaId_CodigoParametro").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CodigoParametro).HasMaxLength(20);
            entity.Property(e => e.Valor).HasColumnType("decimal(10, 4)");

            entity.HasOne(d => d.Oferta).WithMany(p => p.LogInpOfeParametro)
                .HasForeignKey(d => d.OfertaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogInpOfeParametro_Oferta");
        });

        modelBuilder.Entity<LogInpOferta>(entity =>
        {
            entity.HasIndex(e => e.EjecucionId, "UQ_LogInpOferta_EjecucionId").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Descripcion).HasMaxLength(50);
            entity.Property(e => e.Material).HasMaxLength(20);
            entity.Property(e => e.Tolerancia).HasColumnType("decimal(10, 4)");

            entity.HasOne(d => d.Ejecucion).WithOne(p => p.LogInpOferta)
                .HasForeignKey<LogInpOferta>(d => d.EjecucionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogInpOferta_EjecucionId");
        });

        modelBuilder.Entity<LogOutConComposicion>(entity =>
        {
            entity.HasIndex(e => e.ContenedorId, "IX_LogOutConComposicion_ContenedorId");

            entity.HasIndex(e => new { e.ContenedorId, e.CodigoParametro }, "UQ_LogOutConComposicion_ContenedorId_CodigoParametro").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CodigoParametro).HasMaxLength(20);
            entity.Property(e => e.Valor).HasColumnType("decimal(10, 4)");

            entity.HasOne(d => d.Contenedor).WithMany(p => p.LogOutConComposicion)
                .HasForeignKey(d => d.ContenedorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogOutConComposicion_Contenedor");
        });

        modelBuilder.Entity<LogOutConDistribucion>(entity =>
        {
            entity.HasIndex(e => e.ContenedorId, "IX_LogOutConDistribucion_ContenedorId");

            entity.HasIndex(e => new { e.ContenedorId, e.Ruma }, "UQ_LogOutConDistribucion_ContenedorId_Ruma").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Ruma).HasMaxLength(20);

            entity.HasOne(d => d.Contenedor).WithMany(p => p.LogOutConDistribucion)
                .HasForeignKey(d => d.ContenedorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogOutConDistribucion_Contenedor");
        });

        modelBuilder.Entity<LogOutContenedor>(entity =>
        {
            entity.HasIndex(e => e.Contenedor, "IX_LogOutContenedor_Contenedor");

            entity.HasIndex(e => e.EjecucionId, "IX_LogOutContenedor_EjecucionId");

            entity.HasIndex(e => new { e.EjecucionId, e.Contenedor }, "UQ_LogOutContenedor_EjecucionId_Contenedor").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Contenedor).HasMaxLength(20);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Grupo).HasMaxLength(20);

            entity.HasOne(d => d.Ejecucion).WithMany(p => p.LogOutContenedor)
                .HasForeignKey(d => d.EjecucionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LogOutContenedor_Ejecucion");
        });

        modelBuilder.Entity<Parametro>(entity =>
        {
            entity.HasIndex(e => e.Codigo, "UQ_Parametro_Codigo_Activo")
                .IsUnique()
                .HasFilter("([Eliminado]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ModificadoPorId).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Planta>(entity =>
        {
            entity.HasIndex(e => e.Codigo, "UQ_Planta_Codigo_Activo")
                .IsUnique()
                .HasFilter("([Eliminado]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ModificadoPorId).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasIndex(e => e.Codigo, "UQ_Producto_Codigo_Activo")
                .IsUnique()
                .HasFilter("([Eliminado]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ModificadoPorId).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Calidad).WithMany(p => p.Producto)
                .HasForeignKey(d => d.CalidadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_CalidadId");

            entity.HasOne(d => d.TipoProduccion).WithMany(p => p.Producto)
                .HasForeignKey(d => d.TipoProduccionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_TipoProduccionId");
        });

        modelBuilder.Entity<SysLog>(entity =>
        {
            entity.HasIndex(e => e.FunctionInvocationId, "IX_SysLog_FunctionInvocationId");

            entity.HasIndex(e => e.Level, "IX_SysLog_Level");

            entity.HasIndex(e => e.RequestInvocationId, "IX_SysLog_RequestInvocationId");

            entity.HasIndex(e => e.UserId, "IX_SysLog_UserId");

            entity.HasIndex(e => e.Username, "IX_SysLog_Username");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ClassName).HasMaxLength(100);
            entity.Property(e => e.Level)
                .HasMaxLength(10)
                .HasDefaultValue("error");
            entity.Property(e => e.MethodName).HasMaxLength(100);
            entity.Property(e => e.NameSpace)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<SysParam>(entity =>
        {
            entity.HasKey(e => e.Key);

            entity.HasIndex(e => e.Category, "IX_SysParam_Category");

            entity.HasIndex(e => new { e.Category, e.Group }, "IX_SysParam_Category_Group");

            entity.HasIndex(e => e.Group, "IX_SysParam_Group");

            entity.Property(e => e.Key).HasMaxLength(100);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(150);
            entity.Property(e => e.Group).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<TipoProduccion>(entity =>
        {
            entity.HasIndex(e => e.Codigo, "UQ_TipoProduccion_Codigo_Activo")
                .IsUnique()
                .HasFilter("([Eliminado]=(0))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Codigo)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreadoEl).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ModificadoPorId).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Agregado).WithMany(p => p.TipoProduccion)
                .HasForeignKey(d => d.AgregadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoProduccion_AgregadoId");

            entity.HasOne(d => d.LineaProduccion).WithMany(p => p.TipoProduccion)
                .HasForeignKey(d => d.LineaProduccionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TipoProduccion_LineaProduccionId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
