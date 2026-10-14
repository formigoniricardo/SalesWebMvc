using System.Text;
using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;
using SalesWebMvc.Helpers;
using SalesWebMvc.Models;
using SalesWebMvc.Models.Enums;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Services;

public class SalesRecordService
{
    private const int MaxExportRows = 50_000;

    private readonly SalesWebMvcContext _context;

    public SalesRecordService(SalesWebMvcContext context)
    {
        _context = context;
    }

    public async Task<PagedList<SalesRecord>> SearchAsync(SalesRecordFilter filter)
    {
        var query = ApplySort(BuildQuery(filter), filter.Sort);
        return await PagedList<SalesRecord>.CreateAsync(query, filter.Page, filter.PageSize);
    }

    /// <summary>Totais (faturado / pendente) de todo o resultado filtrado, calculados no banco.</summary>
    public async Task<(decimal Billed, decimal Pending)> TotalsAsync(SalesRecordFilter filter)
    {
        var totals = await BuildQuery(filter)
            .GroupBy(sr => sr.Status)
            .Select(g => new { Status = g.Key, Total = g.Sum(sr => sr.Amount) })
            .ToListAsync();

        return (
            totals.Where(t => t.Status == SaleStatus.Billed).Sum(t => t.Total),
            totals.Where(t => t.Status == SaleStatus.Pending).Sum(t => t.Total));
    }

    public async Task<SalesRecord?> FindByIdAsync(int id)
    {
        return await _context.SalesRecord
            .Include(sr => sr.Seller).ThenInclude(s => s!.Department)
            .FirstOrDefaultAsync(sr => sr.Id == id);
    }

    public async Task InsertAsync(SalesRecord obj)
    {
        _context.Add(obj);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(SalesRecord obj)
    {
        if (!await _context.SalesRecord.AnyAsync(sr => sr.Id == obj.Id))
        {
            throw new NotFoundException("Venda não encontrada.");
        }

        _context.Update(obj);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(int id)
    {
        var obj = await _context.SalesRecord.FindAsync(id) ?? throw new NotFoundException("Venda não encontrada.");
        _context.SalesRecord.Remove(obj);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Gera um CSV (UTF-8 com BOM, separador ";") com TODO o resultado filtrado, pronto para abrir no Excel pt-BR.
    /// </summary>
    public async Task<byte[]> ExportCsvAsync(SalesRecordFilter filter)
    {
        var rows = await ApplySort(BuildQuery(filter), filter.Sort)
            .Take(MaxExportRows)
            .Select(sr => new
            {
                sr.Id,
                sr.Date,
                Seller = sr.Seller!.Name,
                Department = sr.Seller.Department!.Name,
                sr.Amount,
                sr.Status
            })
            .ToListAsync();

        var csv = new StringBuilder();
        csv.AppendLine("Id;Data;Vendedor;Departamento;Valor;Status");
        foreach (var r in rows)
        {
            csv.AppendJoin(';',
                r.Id,
                r.Date.ToString("dd/MM/yyyy"),
                CsvField(r.Seller),
                CsvField(r.Department),
                r.Amount.ToString("0.00", Format.PtBr),
                r.Status.DisplayName());
            csv.AppendLine();
        }

        return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
    }

    private IQueryable<SalesRecord> BuildQuery(SalesRecordFilter filter)
    {
        var query = _context.SalesRecord
            .AsNoTracking()
            .Include(sr => sr.Seller).ThenInclude(s => s!.Department)
            .AsQueryable();

        if (filter.MinDate.HasValue)
            query = query.Where(sr => sr.Date >= filter.MinDate.Value.Date);

        if (filter.MaxDate.HasValue)
            query = query.Where(sr => sr.Date < filter.MaxDate.Value.Date.AddDays(1));

        if (filter.DepartmentId.HasValue)
            query = query.Where(sr => sr.Seller!.DepartmentId == filter.DepartmentId);

        if (filter.Status.HasValue)
            query = query.Where(sr => sr.Status == filter.Status);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(sr => sr.Seller!.Name.Contains(term));
        }

        return query;
    }

    private static IQueryable<SalesRecord> ApplySort(IQueryable<SalesRecord> query, string? sort) => sort switch
    {
        "date" => query.OrderBy(sr => sr.Date).ThenBy(sr => sr.Id),
        "amount" => query.OrderBy(sr => sr.Amount),
        "amount_desc" => query.OrderByDescending(sr => sr.Amount),
        "seller" => query.OrderBy(sr => sr.Seller!.Name).ThenByDescending(sr => sr.Date),
        _ => query.OrderByDescending(sr => sr.Date).ThenByDescending(sr => sr.Id)
    };

    private static string CsvField(string value)
    {
        // Neutraliza "CSV injection" (fórmulas executadas pelo Excel) e escapa aspas/separadores
        if (value.Length > 0 && "=+-@".Contains(value[0])) value = "'" + value;
        return value.IndexOfAny([';', '"', '\n', '\r']) >= 0
            ? "\"" + value.Replace("\"", "\"\"") + "\""
            : value;
    }
}
