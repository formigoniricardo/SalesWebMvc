using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Helpers;
using SalesWebMvc.Models;
using SalesWebMvc.Services;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Controllers;

public class DepartmentsController : Controller
{
    private readonly DepartmentService _departmentService;

    public DepartmentsController(DepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _departmentService.FindAllWithStatsAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFoundPage("Id não informado.");
        var department = await _departmentService.FindByIdAsync(id.Value);
        if (department == null) return NotFoundPage("Departamento não encontrado.");
        return View(department);
    }

    public IActionResult Create()
    {
        return View(new Department());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name")] Department department)
    {
        await ValidateUniqueNameAsync(department);
        if (!ModelState.IsValid)
        {
            return View(department);
        }

        await _departmentService.InsertAsync(department);
        TempData.ToastSuccess($"Departamento \"{department.Name}\" criado com sucesso.");
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFoundPage("Id não informado.");
        var department = await _departmentService.FindByIdAsync(id.Value);
        if (department == null) return NotFoundPage("Departamento não encontrado.");
        return View(department);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Department department)
    {
        if (id != department.Id) return BadRequest();

        await ValidateUniqueNameAsync(department);
        if (!ModelState.IsValid)
        {
            return View(department);
        }

        try
        {
            await _departmentService.UpdateAsync(department);
            TempData.ToastSuccess($"Departamento \"{department.Name}\" atualizado com sucesso.");
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
        var department = await _departmentService.FindByIdAsync(id.Value);
        if (department == null) return NotFoundPage("Departamento não encontrado.");
        return View(department);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _departmentService.RemoveAsync(id);
            TempData.ToastSuccess("Departamento excluído com sucesso.");
        }
        catch (ApplicationException e) when (e is IntegrityException or NotFoundException)
        {
            TempData.ToastError(e.Message);
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateUniqueNameAsync(Department department)
    {
        if (!string.IsNullOrWhiteSpace(department.Name) && await _departmentService.NameExistsAsync(department.Name, department.Id))
        {
            ModelState.AddModelError(nameof(Department.Name), "Já existe um departamento com este nome.");
        }
    }

    private IActionResult NotFoundPage(string message) =>
        RedirectToAction(nameof(HomeController.Error), "Home", new { message });
}
