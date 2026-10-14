using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Data;
using SalesWebMvc.Models.Enums;

namespace SalesWebMvc.ViewComponents;

/// <summary>
/// Sino de notificações da barra superior: lista as vendas pendentes de faturamento mais recentes.
/// </summary>
public class NotificationsViewComponent : ViewComponent
{
    private readonly SalesWebMvcContext _context;

    public NotificationsViewComponent(SalesWebMvcContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var pending = _context.SalesRecord.AsNoTracking().Where(sr => sr.Status == SaleStatus.Pending);

        ViewData["PendingCount"] = await pending.CountAsync();
        var latest = await pending
            .Include(sr => sr.Seller)
            .OrderByDescending(sr => sr.Date).ThenByDescending(sr => sr.Id)
            .Take(5)
            .ToListAsync();

        return View(latest);
    }
}
