using ArboxEmployeeMS.Models;
using ArboxEmployeeMS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace ArboxEmployeeMS.Controllers;
public class EmployeesController : Controller
{
    private readonly IEmployeeService _employees; private readonly IDepartmentService _departments;
    public EmployeesController(IEmployeeService e, IDepartmentService d)
    {
        _employees = e; _departments = d;
    }
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, string? sortBy = null, bool asc = true)
    {

        var result = await _employees.GetPagedAsync(page, pageSize, search, sortBy, asc);
        ViewBag.Search = search;
        ViewBag.SortBy = sortBy;
        ViewBag.Asc = asc; ViewBag.Page = page; ViewBag.PageSize = pageSize;
        return View(result);
    }
    public async Task<IActionResult> Create()
    {
        await PopulateDepartments();
        return View(new Employee { HireDate = DateTime.UtcNow.Date });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee e)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDepartments();
            return View(e);
        }
        var (ok, err) = await _employees.CreateAsync(e);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, err!);
            await PopulateDepartments();
            return View(e);
        }
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int id)
    {
        var e = await _employees.GetByIdAsync(id);
        if (e is null) return NotFound();
        await PopulateDepartments();
        return View(e);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Employee e)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDepartments();
            return View(e);
        }
        var (ok, err) = await _employees.UpdateAsync(e);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, err!);
            await PopulateDepartments(); return View(e);
        }
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _employees.GetByIdAsync(id);
        if (e is null) return NotFound();
        return View(e);
    }
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _employees.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
    private async Task PopulateDepartments()
    {
        var deps = await _departments.GetAllAsync();
        ViewBag.Departments = new SelectList(deps, "Id", "Name");
    }
}
