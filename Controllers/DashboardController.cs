using ArboxEmployeeMS.Models;
using ArboxEmployeeMS.Services;
using Microsoft.AspNetCore.Mvc;
namespace ArboxEmployeeMS.Controllers;
public class DashboardController : Controller
{
    private readonly IEmployeeService _employees;
    private readonly IDepartmentService _departments;
    public DashboardController(IEmployeeService e, IDepartmentService d)
    {
        _employees = e;
        _departments = d;
    }
    public async Task<IActionResult> Index(string? search = null)
    {
        var all = await _employees.GetAllAsync();
        var deps = await _departments.GetAllAsync();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            all = all.Where(e => e.FirstName.ToLower().Contains(s) || e.LastName.ToLower().Contains(s)).ToList();
        }
        var grouped = all.GroupBy(e => deps.FirstOrDefault(d => d.Id == e.DepartmentId)?.Name ?? "Unknown")
                         .Select(g => (Department: g.Key, Count: g.Count()))
                         .OrderByDescending(x => x.Count).ToList();
        var vm = new DashboardViewModel
        {
            TotalEmployees = all.Count,
            EmployeesByDepartment = grouped,
            RecentHires = all.Where(e => e.HireDate >= DateTime.UtcNow.Date.AddDays(-30)).OrderByDescending(e => e.HireDate).Take(10).ToList(),
            Search = search
        };
        return View(vm);
    }
    [HttpGet]
    public async Task<IActionResult> EmployeesByDepartmentData()
    {
        var all = await _employees.GetAllAsync();
        var deps = await _departments.GetAllAsync();
        var grouped = all.GroupBy(e => deps.FirstOrDefault(d => d.Id == e.DepartmentId)?.Name ?? "Unknown").Select(g => new { label = g.Key, value = g.Count() }).ToList();
        return Json(grouped);
    }
}
