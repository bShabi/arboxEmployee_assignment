using ArboxEmployeeMS.Models;
using ArboxEmployeeMS.Services;
using Microsoft.AspNetCore.Mvc;
namespace ArboxEmployeeMS.Controllers;
public class DepartmentsController : Controller
{
    private readonly IDepartmentService _departments;
    public DepartmentsController(IDepartmentService d)
    {
        _departments = d;
    }
    public async Task<IActionResult> Index()
    {
        var list = await _departments.GetAllAsync();
        return View(list);
    }
    public IActionResult Create() => View(new Department());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Department d)
    {
        var (ok, err) = await _departments.CreateAsync(d);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, err!);
            return View(d);
        }
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int id)
    {
        var d = await _departments.GetByIdAsync(id);
        if (d is null) return NotFound();
        return View(d);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Department d)
    {
        var (ok, err) = await _departments.UpdateAsync(d);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, err!);
            return View(d);
        }
        return
            RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int id)
    {
        var d = await _departments.GetByIdAsync(id);
        if (d is null) return NotFound();
        return View(d);
    }
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var (ok, err) = await _departments.DeleteAsync(id);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, err!);
            var d = await _departments.GetByIdAsync(id);
            return View("Delete", d);
        }
        return RedirectToAction(nameof(Index));
    }
}
