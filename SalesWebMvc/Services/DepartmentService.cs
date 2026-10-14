using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;
using SalesWebMvc.Models;
using SalesWebMvc.Models.Enums;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Services;

public class DepartmentService
{
    private readonly SalesWebMvcContext _context;

    public DepartmentService(SalesWebMvcContext context)
    {
        _context = context;
    }

    public async Task<List<Department>> FindAllAsync()
    {
        return await _context.Department.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<List<DepartmentListItem>> FindAllWithStatsAsync()
    {
        return await _context.Department
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentListItem
            {
                Department = d,
                SellersCount = d.Sellers.Count,
                BilledTotal = d.Sellers.SelectMany(s => s.Sales)
                    .Where(sr => sr.Status == SaleStatus.Billed)
                    .Sum(sr => (decimal?)sr.Amount) ?? 0
            })
            .ToListAsync();
    }

    public async Task<Department?> FindByIdAsync(int id)
    {
        return await _context.Department
            .Include(d => d.Sellers.OrderBy(s => s.Name))
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<bool> NameExistsAsync(string name, int? ignoreId = null)
    {
        var normalized = name.Trim();
        return await _context.Department.AnyAsync(d => d.Name == normalized && d.Id != ignoreId);
    }

    public async Task InsertAsync(Department obj)
    {
        obj.Name = obj.Name.Trim();
        _context.Add(obj);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Department obj)
    {
        if (!await _context.Department.AnyAsync(d => d.Id == obj.Id))
        {
            throw new NotFoundException("Departamento não encontrado.");
        }

        obj.Name = obj.Name.Trim();
        _context.Update(obj);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(int id)
    {
        var obj = await _context.Department.FindAsync(id) ?? throw new NotFoundException("Departamento não encontrado.");
        try
        {
            _context.Department.Remove(obj);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new IntegrityException("Não é possível excluir este departamento porque ele possui vendedores vinculados.");
        }
    }
}
