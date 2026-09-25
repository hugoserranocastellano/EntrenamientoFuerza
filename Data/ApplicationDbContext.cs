using EntrenamientoFuerza.Models;
using Microsoft.EntityFrameworkCore;

namespace EntrenamientoFuerza.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Ejercicio> Ejercicios => Set<Ejercicio>();
    public DbSet<GrupoMuscular> GruposMusculares => Set<GrupoMuscular>();
    public DbSet<Rutina> Rutinas => Set<Rutina>();
    public DbSet<RutinaEjercicio> RutinaEjercicios => Set<RutinaEjercicio>();
    public DbSet<SesionDiario> SesionesDiario => Set<SesionDiario>();
    public DbSet<SesionEjercicio> SesionEjercicios => Set<SesionEjercicio>();
    public DbSet<SesionSerie> SesionSeries => Set<SesionSerie>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("usuarios");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
            e.Property(x => x.NombreUsuario).HasColumnName("nombre_usuario").HasMaxLength(50).IsRequired();
            e.Property(x => x.PasswordHash).HasColumnName("password_hash").IsRequired();
            e.Property(x => x.Rol).HasColumnName("rol").HasMaxLength(30).IsRequired();
            e.Property(x => x.Activo).HasColumnName("activo").HasDefaultValue(true);
            e.Property(x => x.FechaAlta).HasColumnName("fecha_alta").HasDefaultValueSql("now()");
            e.HasIndex(x => x.NombreUsuario).IsUnique();
        });

        modelBuilder.Entity<Ejercicio>(e =>
        {
            e.ToTable("ejercicios");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
            e.Property(x => x.Descripcion).HasColumnName("descripcion");
            e.Property(x => x.VideoUrl).HasColumnName("video_url").HasMaxLength(500);
            e.Property(x => x.GrupoMuscularId).HasColumnName("grupo_muscular_id");
            e.Property(x => x.Activo).HasColumnName("activo").HasDefaultValue(true);
            e.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion").HasDefaultValueSql("now()");

            e.HasOne(x => x.GrupoMuscular)
                .WithMany(g => g.Ejercicios)
                .HasForeignKey(x => x.GrupoMuscularId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<GrupoMuscular>(e =>
        {
            e.ToTable("grupos_musculares");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.Nombre).IsUnique();
        });

        modelBuilder.Entity<Rutina>(e =>
        {
            e.ToTable("rutinas");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
            e.Property(x => x.Descripcion).HasColumnName("descripcion");
            e.Property(x => x.CreadoPorId).HasColumnName("creado_por_id");
            e.Property(x => x.Activa).HasColumnName("activa").HasDefaultValue(true);
            e.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion").HasDefaultValueSql("now()");

            e.HasOne(x => x.CreadoPor)
                .WithMany(u => u.Rutinas)
                .HasForeignKey(x => x.CreadoPorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RutinaEjercicio>(e =>
        {
            e.ToTable("rutina_ejercicios");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.RutinaId).HasColumnName("rutina_id");
            e.Property(x => x.EjercicioId).HasColumnName("ejercicio_id");
            e.Property(x => x.Orden).HasColumnName("orden");
            e.Property(x => x.Series).HasColumnName("series");
            e.Property(x => x.Repeticiones).HasColumnName("repeticiones");
            e.Property(x => x.DuracionSegundos).HasColumnName("duracion_segundos");
            e.Property(x => x.DescansoSegundos).HasColumnName("descanso_segundos");

            e.HasOne(x => x.Rutina)
                .WithMany(r => r.RutinaEjercicios)
                .HasForeignKey(x => x.RutinaId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Ejercicio)
                .WithMany(ej => ej.RutinaEjercicios)
                .HasForeignKey(x => x.EjercicioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SesionDiario>(e =>
        {
            e.ToTable("sesiones_diario");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.UsuarioId).HasColumnName("usuario_id");
            e.Property(x => x.RutinaId).HasColumnName("rutina_id");
            e.Property(x => x.Fecha).HasColumnName("fecha");
            e.Property(x => x.Notas).HasColumnName("notas");
            e.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion").HasDefaultValueSql("now()");

            e.HasOne(x => x.Usuario)
                .WithMany(u => u.Sesiones)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Rutina)
                .WithMany(r => r.Sesiones)
                .HasForeignKey(x => x.RutinaId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<SesionEjercicio>(e =>
        {
            e.ToTable("sesion_ejercicios");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.SesionDiarioId).HasColumnName("sesion_diario_id");
            e.Property(x => x.EjercicioId).HasColumnName("ejercicio_id");
            e.Property(x => x.Orden).HasColumnName("orden");
            e.Property(x => x.Notas).HasColumnName("notas");

            e.HasOne(x => x.SesionDiario)
                .WithMany(s => s.SesionEjercicios)
                .HasForeignKey(x => x.SesionDiarioId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Ejercicio)
                .WithMany(ej => ej.SesionEjercicios)
                .HasForeignKey(x => x.EjercicioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SesionSerie>(e =>
        {
            e.ToTable("sesion_series");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.SesionEjercicioId).HasColumnName("sesion_ejercicio_id");
            e.Property(x => x.NumeroSerie).HasColumnName("numero_serie");
            e.Property(x => x.RepeticionesRealizadas).HasColumnName("repeticiones_realizadas");
            e.Property(x => x.DuracionSegundosReal).HasColumnName("duracion_segundos_real");
            e.Property(x => x.PesoKg).HasColumnName("peso_kg").HasColumnType("numeric(6,2)");
            e.Property(x => x.Completada).HasColumnName("completada").HasDefaultValue(false);

            e.HasOne(x => x.SesionEjercicio)
                .WithMany(se => se.Series)
                .HasForeignKey(x => x.SesionEjercicioId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
