using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;
using SalesWebMvc.Helpers;
using SalesWebMvc.Models;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Services;

public class SellerService
{
    private readonly SalesWebMvcContext _context;

    public SellerService(SalesWebMvcContext context)
    {
        _context = context;
    }

    public async Task<List<Seller>> FindAllAsync()
    {
        return await _context.Seller.AsNoTracking().OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<PagedList<Seller>> SearchAsync(string? search, int? departmentId, int page, int pageSize)
    {
        var query = _context.Seller.AsNoTracking().Include(s => s.Department).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(s => s.Name.Contains(term) || s.Email.Contains(term));
        }

        if (departmentId.HasValue)
        {
            query = query.Where(s => s.DepartmentId == departmentId);
        }

        return await PagedList<Seller>.CreateAsync(query.OrderBy(s => s.Name), page, pageSize);
    }

    public async Task InsertAsync(Seller obj)
    {
        _context.Add(obj);
        await _context.SaveChangesAsync();
    }

    public async Task<Seller?> FindByIdAsync(int id)
    {
        return await _context.Seller.Include(obj => obj.Department).FirstOrDefaultAsync(obj => obj.Id == id);
    }

    public async Task<Seller?> FindByIdWithSalesAsync(int id)
    {
        return await _context.Seller
            .AsNoTracking()
            .Include(s => s.Department)
            .Include(s => s.Sales.OrderByDescending(sr => sr.Date).Take(10))
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> EmailExistsAsync(string email, int? ignoreId = null)
    {
        var normalized = email.Trim();
        return await _context.Seller.AnyAsync(s => s.Email == normalized && s.Id != ignoreId);
    }

    public async Task RemoveAsync(int id)
    {
        var obj = await _context.Seller.FindAsync(id) ?? throw new NotFoundException("Vendedor não encontrado.");
        try
        {
            _context.Seller.Remove(obj);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new IntegrityException("Não é possível excluir este vendedor porque ele possui vendas registradas no sistema.");
        }
    }

    public async Task UpdateAsync(Seller obj)
    {
        bool hasAny = await _context.Seller.AnyAsync(x => x.Id == obj.Id);
        if (!hasAny)
        {
            throw new NotFoundException("Vendedor não encontrado no banco de dados.");
        }

        _context.Update(obj);
        await _context.SaveChangesAsync();
    }
}
