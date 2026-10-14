using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Models;
using SalesWebMvc.Models.Enums;

namespace SalesWebMvc.Data;

public class SeedingService
{
    private readonly SalesWebMvcContext _context;

    public SeedingService(SalesWebMvcContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        _context.Database.Migrate();

        if (_context.Department.Any() || _context.Seller.Any() || _context.SalesRecord.Any())
        {
            return; // O banco já tem dados
        }

        // Criamos os objetos omitindo o "Id". O banco vai preencher isso sozinho.
        Department d1 = new Department { Name = "Computers" };
        Department d2 = new Department { Name = "Electronics" };
        Department d3 = new Department { Name = "Fashion" };
        Department d4 = new Department { Name = "Books" };

        Seller[] sellers =
        [
            new Seller { Name = "Bob Brown", Email = "bob@gmail.com", BirthDate = new DateTime(1998, 4, 21), BaseSalary = 1000m, Department = d1 },
            new Seller { Name = "Maria Green", Email = "maria@gmail.com", BirthDate = new DateTime(1979, 12, 31), BaseSalary = 3500m, Department = d2 },
            new Seller { Name = "Alex Grey", Email = "alex@gmail.com", BirthDate = new DateTime(1988, 1, 15), BaseSalary = 2200m, Department = d1 },
            new Seller { Name = "Martha Red", Email = "martha@gmail.com", BirthDate = new DateTime(1993, 11, 30), BaseSalary = 3000m, Department = d4 },
            new Seller { Name = "Donald Blue", Email = "donald@gmail.com", BirthDate = new DateTime(2000, 1, 9), BaseSalary = 4000m, Department = d3 },
            new Seller { Name = "Alex Pink", Email = "alexpink@gmail.com", BirthDate = new DateTime(1997, 3, 4), BaseSalary = 3000m, Department = d2 },
        ];

        // Vendas distribuídas pelos últimos ~14 meses, sempre relativas à data atual
        // (para o dashboard nunca ficar vazio). Semente fixa = dados reproduzíveis.
        var random = new Random(42);
        var today = DateTime.Today;
        var sales = new List<SalesRecord>();
        for (int i = 0; i < 160; i++)
        {
            var roll = random.Next(100);
            var status = roll < 75 ? SaleStatus.Billed : roll < 90 ? SaleStatus.Pending : SaleStatus.Canceled;
            sales.Add(new SalesRecord
            {
                Date = today.AddDays(-random.Next(0, 425)),
                Amount = Math.Round((decimal)(random.NextDouble() * 14000 + 1000), 2),
                Status = status,
                Seller = sellers[random.Next(sellers.Length)]
            });
        }

        _context.Department.AddRange(d1, d2, d3, d4);
        _context.Seller.AddRange(sellers);
        _context.SalesRecord.AddRange(sales);

        _context.SaveChanges();
    }
}
