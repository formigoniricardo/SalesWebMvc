using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Models;

namespace SalesWebMvc.Data;

public class SalesWebMvcContext : DbContext
{
    public SalesWebMvcContext(DbContextOptions<SalesWebMvcContext> options) : base(options) { }

    public DbSet<Department> Department { get; set; }
    public DbSet<Seller> Seller { get; set; }
    public DbSet<SalesRecord> SalesRecord { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Valores monetários com precisão fixa (double gera erros de arredondamento)
        modelBuilder.Entity<Seller>().Property(s => s.BaseSalary).HasPrecision(18, 2);
        modelBuilder.Entity<SalesRecord>().Property(sr => sr.Amount).HasPrecision(18, 2);

        // Não apagar em cascata: excluir um departamento/vendedor com histórico deve falhar
        // e ser tratado como IntegrityException nos services.
        modelBuilder.Entity<Seller>()
            .HasOne(s => s.Department)
            .WithMany(d => d.Sellers)
            .HasForeignKey(s => s.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SalesRecord>()
            .HasOne(sr => sr.Seller)
            .WithMany(s => s.Sales)
            .HasForeignKey(sr => sr.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices para os filtros mais usados no dashboard e na busca de vendas
        modelBuilder.Entity<SalesRecord>().HasIndex(sr => sr.Date);
        modelBuilder.Entity<SalesRecord>().HasIndex(sr => sr.Status);

        // Unicidade garantida também no banco (além da checagem amigável nos controllers)
        modelBuilder.Entity<Department>().HasIndex(d => d.Name).IsUnique();
        modelBuilder.Entity<Seller>().HasIndex(s => s.Email).IsUnique();
    }
}
