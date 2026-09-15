using Microsoft.EntityFrameworkCore;
using TarjetaSube.Clases;

namespace TarjetaSube.Servicios;

public class TarjetaSubeDbContext : DbContext
{
    public TarjetaSubeDbContext() { }

    public TarjetaSubeDbContext(DbContextOptions<TarjetaSubeDbContext> opciones) : base(opciones) { }

    public DbSet<Tarjeta> Tarjetas => Set<Tarjeta>();
    public DbSet<Colectivo> Colectivos => Set<Colectivo>();
    public DbSet<Boleto> Boletos => Set<Boleto>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=tarjetasube.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tarjeta>()
            .Property(t => t.Saldo)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Boleto>()
            .Property(b => b.Monto)
            .HasPrecision(18, 2);
    }
}
