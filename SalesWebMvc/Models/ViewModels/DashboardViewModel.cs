namespace SalesWebMvc.Models.ViewModels;

public class DashboardViewModel
{
    public int Year { get; set; }

    public decimal TotalRevenue { get; set; }
    public int SalesCount { get; set; }
    public int ActiveSellers { get; set; }
    public int TotalSellers { get; set; }
    public decimal AverageTicket { get; set; }

    // Variação percentual do mês atual contra o mês anterior (null quando não há base de comparação)
    public decimal? RevenueChange { get; set; }
    public decimal? SalesCountChange { get; set; }
    public decimal? AverageTicketChange { get; set; }

    public IReadOnlyList<MonthlyPoint> MonthlyRevenue { get; set; } = [];
    public IReadOnlyList<DepartmentSlice> RevenueByDepartment { get; set; } = [];
    public IReadOnlyList<SalesRecord> RecentSales { get; set; } = [];
}

public record MonthlyPoint(string Label, decimal Billed);

public record DepartmentSlice(string Department, decimal Amount);
