using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaTramites.Domain.Entities;
using SistemaTramites.Infrastructure.Identity;

namespace SistemaTramites.Infrastructure.Data;

/// <summary>
/// Capa de datos (SQLite) — persistencia de trámites, pagos, citas,
/// documentos y archivo protocolizado, según el DER del sistema.
/// También almacena las cuentas de acceso (login) vía ASP.NET Core Identity.
/// </summary>
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Ciudadano> Ciudadanos => Set<Ciudadano>();
    public DbSet<TipoTramite> TiposTramite => Set<TipoTramite>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Tramite> Tramites => Set<Tramite>();
    public DbSet<Cita> Citas => Set<Cita>();
    public DbSet<DocumentoAdjunto> Documentos => Set<DocumentoAdjunto>();
    public DbSet<Notificacion> Notificaciones => Set<Notificacion>();
    public DbSet<Protocolo> Protocolos => Set<Protocolo>();
    public DbSet<Pago> Pagos => Set<Pago>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ciudadano 1..N Tramite (solicita)
        modelBuilder.Entity<Tramite>()
            .HasOne(t => t.Ciudadano)
            .WithMany(c => c.Tramites)
            .HasForeignKey(t => t.CiudadanoId)
            .OnDelete(DeleteBehavior.Restrict);

        // TipoTramite 1..N Tramite (clasifica)
        modelBuilder.Entity<Tramite>()
            .HasOne(t => t.TipoTramite)
            .WithMany(tt => tt.Tramites)
            .HasForeignKey(t => t.TipoTramiteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Usuario 1..N Tramite (atiende, como oficial) - opcional
        modelBuilder.Entity<Tramite>()
            .HasOne(t => t.Oficial)
            .WithMany(u => u.TramitesAtendidos)
            .HasForeignKey(t => t.OficialId)
            .OnDelete(DeleteBehavior.Restrict);

        // Tramite 1..N Cita (agenda)
        modelBuilder.Entity<Cita>()
            .HasOne(c => c.Tramite)
            .WithMany(t => t.Citas)
            .HasForeignKey(c => c.TramiteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Tramite 1..N DocumentoAdjunto (incluye)
        modelBuilder.Entity<DocumentoAdjunto>()
            .HasOne(d => d.Tramite)
            .WithMany(t => t.Documentos)
            .HasForeignKey(d => d.TramiteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Tramite 1..N Notificacion (envía)
        modelBuilder.Entity<Notificacion>()
            .HasOne(n => n.Tramite)
            .WithMany(t => t.Notificaciones)
            .HasForeignKey(n => n.TramiteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Tramite 0..1 Protocolo (archiva)
        modelBuilder.Entity<Protocolo>()
            .HasOne(p => p.Tramite)
            .WithOne(t => t.Protocolo)
            .HasForeignKey<Protocolo>(p => p.TramiteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Tramite 1..N Pago (genera)
        modelBuilder.Entity<Pago>()
            .HasOne(p => p.Tramite)
            .WithMany(t => t.Pagos)
            .HasForeignKey(p => p.TramiteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Usuario 1..N Pago (registra, como cajero)
        modelBuilder.Entity<Pago>()
            .HasOne(p => p.Cajero)
            .WithMany(u => u.PagosRegistrados)
            .HasForeignKey(p => p.CajeroId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Tramite>()
            .HasIndex(t => t.CodigoSeguimiento)
            .IsUnique();
    }
}
