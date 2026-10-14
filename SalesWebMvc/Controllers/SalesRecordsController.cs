using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Helpers;
using SalesWebMvc.Models;
using SalesWebMvc.Models.Enums;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Controllers;

public class SalesRecordsController : Controller
{
    private readonly SalesRecordService _salesRecordService;
    private readonly SellerService _sellerService;
    private readonly DepartmentService _departmentService;

    public SalesRecordsController(SalesRecordService salesRecordService, SellerService sellerService, DepartmentService departmentService)
    {
        _salesRecordService = salesRecordService;
        _sellerService = sellerService;
        _departmentService = departmentService;
    }

    public async Task<IActionResult> Index([FromQuery] SalesRecordFilter filter)
    {
        var (billed, pending) = await _salesRecordService.TotalsAsync(filter);
        var viewModel = new SalesRecordListViewModel
        {
            Filter = filter,
            Records = await _salesRecordService.SearchAsync(filter),
            Departments = await _departmentService.FindAllAsync(),
            FilteredBilledTotal = billed,
            FilteredPendingTotal = pending
        };
        return View(viewModel);
    }

    /// <summary>Exporta todo o resultado da busca atual (mesmos filtros da tela) em CSV.</summary>
    public async Task<IActionResult> Export([FromQuery] SalesRecordFilter filter)
    {
        var bytes = await _salesRecordService.ExportCsvAsync(filter);
        return File(bytes, "text/csv; charset=utf-8", $"vendas_{DateTime.Now:yyyyMMdd_HHmm}.csv");
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFoundPage("Id não informado.");
        var obj = await _salesRecordService.FindByIdAsync(id.Value);
        if (obj == null) return NotFoundPage("Venda não encontrada.");
        return View(obj);
    }

    public async Task<IActionResult> Create()
    {
        return View(await BuildFormAsync(new SalesRecord { Date = DateTime.Today, Status = SaleStatus.Pending }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Date,Amount,Status,SellerId", Prefix = "SalesRecord")] SalesRecord salesRecord)
    {
        if (!ModelState.IsValid)
        {
            return View(await BuildFormAsync(salesRecord));
        }

        await _salesRecordService.InsertAsync(salesRecord);
        TempData.ToastSuccess($"Venda de {Format.Money(salesRecord.Amount)} registrada com sucesso.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFoundPage("Id não informado.");
        var obj = await _salesRecordService.FindByIdAsync(id.Value);
        if (obj == null) return NotFoundPage("Venda não encontrada.");
        return View(await BuildFormAsync(obj));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Amount,Status,SellerId", Prefix = "SalesRecord")] SalesRecord salesRecord)
    {
        if (id != salesRecord.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            return View(await BuildFormAsync(salesRecord));
        }

        try
        {
            await _salesRecordService.UpdateAsync(salesRecord);
            TempData.ToastSuccess($"Venda #{salesRecord.Id} atualizada com sucesso.");
        }
        catch (NotFoundException e)
        {
            TempData.ToastError(e.Message);
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFoundPage("Id não informado.");
        var obj = await _salesRecordService.FindByIdAsync(id.Value);
        if (obj == null) return NotFoundPage("Venda não encontrada.");
        return View(obj);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _salesRecordService.RemoveAsync(id);
            TempData.ToastSuccess($"Venda #{id} excluída com sucesso.");
        }
        catch (NotFoundException e)
        {
            TempData.ToastError(e.Message);
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<SalesRecordFormViewModel> BuildFormAsync(SalesRecord salesRecord) => new()
    {
        SalesRecord = salesRecord,
        Sellers = await _sellerService.FindAllAsync()
    };

    private IActionResult NotFoundPage(string message) =>
        RedirectToAction(nameof(HomeController.Error), "Home", new { message });
}
