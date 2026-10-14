using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;
using SalesWebMvc.Helpers;
using SalesWebMvc.Models.Enums;
using SalesWebMvc.Models.ViewModels;

namespace SalesWebMvc.Services;

/// <summary>
/// Agrega os indicadores da tela inicial. Todas as somas/contagens são feitas no banco (GroupBy/Sum traduzidos para SQL).
/// </summary>
public class DashboardService
{
    private readonly SalesWebMvcContext _context;

    public DashboardService(SalesWebMvcContext context)
    {
        _context = context;
    }

    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        var today = DateTime.Today;
        var yearStart = new DateTime(today.Year, 1, 1);
        var nextYearStart = yearStart.AddYears(1);
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var previousMonthStart = monthStart.AddMonths(-1);

        var sales = _context.SalesRecord.AsNoTracking();
        var yearSales = sales.Where(sr => sr.Date >= yearStart && sr.Date < nextYearStart);
        var yearBilled = yearSales.Where(sr => sr.Status == SaleStatus.Billed);

        // KPIs do ano
        var totalRevenue = await yearBilled.SumAsync(sr => (decimal?)sr.Amount) ?? 0;
        var billedCount = await yearBilled.CountAsync();
        var salesCount = await yearSales.CountAsync(sr => sr.Status != SaleStatus.Canceled);
        var activeSellers = await yearSales.Select(sr => sr.SellerId).Distinct().CountAsync();
        var totalSellers = await _context.Seller.CountAsync();

        // Mês atual até hoje x mesmo período do mês anterior (comparação justa no meio do mês)
        var current = await MonthStatsAsync(monthStart, today.AddDays(1));
        var previousEnd = previousMonthStart.AddDays(today.Day);
        var previous = await MonthStatsAsync(previousMonthStart, previousEnd < monthStart ? previousEnd : monthStart);

        // Faturamento mês a mês do ano corrente (meses sem venda aparecem zerados)
        var byMonth = await yearBilled
            .GroupBy(sr => sr.Date.Month)
            .Select(g => new { Month = g.Key, Total = g.Sum(sr => sr.Amount) })
            .ToDictionaryAsync(x => x.Month, x => x.Total);

        var monthly = Enumerable.Range(1, today.Month)
            .Select(m => new MonthlyPoint(
                Format.PtBr.DateTimeFormat.GetAbbreviatedMonthName(m).TrimEnd('.'),
                byMonth.GetValueOrDefault(m)))
            .ToList();

        var byDepartment = (await yearBilled
                .GroupBy(sr => sr.Seller!.Department!.Name)
                .Select(g => new { Department = g.Key, Amount = g.Sum(sr => sr.Amount) })
                .OrderByDescending(d => d.Amount)
                .ToListAsync())
            .Select(d => new DepartmentSlice(d.Department, d.Amount))
            .ToList();

        var recent = await sales
            .Include(sr => sr.Seller).ThenInclude(s => s!.Department)
            .OrderByDescending(sr => sr.Date).ThenByDescending(sr => sr.Id)
            .Take(8)
            .ToListAsync();

        return new DashboardViewModel
        {
            Year = today.Year,
            TotalRevenue = totalRevenue,
            SalesCount = salesCount,
            ActiveSellers = activeSellers,
            TotalSellers = totalSellers,
            AverageTicket = billedCount == 0 ? 0 : totalRevenue / billedCount,
            RevenueChange = Change(current.Revenue, previous.Revenue),
            SalesCountChange = Change(current.Count, previous.Count),
            AverageTicketChange = Change(current.Average, previous.Average),
            MonthlyRevenue = monthly,
            RevenueByDepartment = byDepartment,
            RecentSales = recent
        };
    }

    private async Task<(decimal Revenue, int Count, decimal Average)> MonthStatsAsync(DateTime start, DateTime end)
    {
        var monthSales = _context.SalesRecord.Where(sr => sr.Date >= start && sr.Date < end);
        var billed = monthSales.Where(sr => sr.Status == SaleStatus.Billed);
        var revenue = await billed.SumAsync(sr => (decimal?)sr.Amount) ?? 0;
        var billedCount = await billed.CountAsync();
        var count = await monthSales.CountAsync(sr => sr.Status != SaleStatus.Canceled);
        return (revenue, count, billedCount == 0 ? 0 : revenue / billedCount);
    }

    private static decimal? Change(decimal current, decimal previous) =>
        previous == 0 ? null : Math.Round((current - previous) / previous * 100, 1);
}
