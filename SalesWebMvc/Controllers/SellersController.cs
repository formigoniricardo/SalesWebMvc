using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Helpers;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Controllers;

public class SellersController : Controller
{
    private readonly SellerService _sellerService;
    private readonly DepartmentService _departmentService;

    public SellersController(SellerService sellerService, DepartmentService departmentService)
    {
        _sellerService = sellerService;
        _departmentService = departmentService;
    }

    public async Task<IActionResult> Index(string? search, int? departmentId, int page = 1, int pageSize = 10)
    {
        var viewModel = new SellerListViewModel
        {
            Sellers = await _sellerService.SearchAsync(search, departmentId, page, pageSize),
            Departments = await _departmentService.FindAllAsync(),
            Search = search,
            DepartmentId = departmentId
        };
        return View(viewModel);
    }

    public async Task<IActionResult> Create()
    {
        return View(await BuildFormAsync(new Seller { BirthDate = DateTime.Today.AddYears(-25) }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Email,BirthDate,BaseSalary,DepartmentId", Prefix = "Seller")] Seller seller)
    {
        await ValidateUniqueEmailAsync(seller);
        if (!ModelState.IsValid)
        {
            return View(await BuildFormAsync(seller));
        }

        await _sellerService.InsertAsync(seller);
        TempData.ToastSuccess($"Vendedor \"{seller.Name}\" cadastrado com sucesso.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFoundPage("Id não informado.");
        var obj = await _sellerService.FindByIdAsync(id.Value);
        if (obj == null) return NotFoundPage("Vendedor não encontrado.");
        return View(obj);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _sellerService.RemoveAsync(id);
            TempData.ToastSuccess("Vendedor excluído com sucesso.");
        }
        catch (ApplicationException e) when (e is IntegrityException or NotFoundException)
        {
            TempData.ToastError(e.Message);
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFoundPage("Id não informado.");
        var obj = await _sellerService.FindByIdWithSalesAsync(id.Value);
        if (obj == null) return NotFoundPage("Vendedor não encontrado.");
        return View(obj);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFoundPage("Id não informado.");
        var obj = await _sellerService.FindByIdAsync(id.Value);
        if (obj == null) return NotFoundPage("Vendedor não encontrado.");

        return View(await BuildFormAsync(obj));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,BirthDate,BaseSalary,DepartmentId", Prefix = "Seller")] Seller seller)
    {
        if (id != seller.Id) return BadRequest();

        await ValidateUniqueEmailAsync(seller);
        if (!ModelState.IsValid)
        {
            return View(await BuildFormAsync(seller));
        }

        try
        {
            await _sellerService.UpdateAsync(seller);
            TempData.ToastSuccess($"Vendedor \"{seller.Name}\" atualizado com sucesso.");
        }
        catch (NotFoundException e)
        {
            TempData.ToastError(e.Message);
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateUniqueEmailAsync(Seller seller)
    {
        if (!string.IsNullOrWhiteSpace(seller.Email) && await _sellerService.EmailExistsAsync(seller.Email, seller.Id))
        {
            ModelState.AddModelError("Seller.Email", "Já existe um vendedor com este e-mail.");
        }
    }

    private async Task<SellerFormViewModel> BuildFormAsync(Seller seller) => new()
    {
        Seller = seller,
        Departments = await _departmentService.FindAllAsync()
    };

    private IActionResult NotFoundPage(string message) =>
        RedirectToAction(nameof(HomeController.Error), "Home", new { message });
}
