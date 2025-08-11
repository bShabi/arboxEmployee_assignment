using System.ComponentModel.DataAnnotations;
using ArboxEmployeeMS.Data;
using ArboxEmployeeMS.Models;
namespace ArboxEmployeeMS.Services;
public interface IEmployeeService
{
    Task<PagedResult<Employee>> GetPagedAsync(int page, int pageSize, string? search, string? sortBy, bool asc);
    Task<Employee?> GetByIdAsync(int id);
    Task<(bool ok, string? error)> CreateAsync(Employee e);
    Task<(bool ok, string? error)> UpdateAsync(Employee e);
    Task<(bool ok, string? error)> DeleteAsync(int id);
    Task<List<Employee>> GetAllAsync();
    Task<List<Employee>> GetRecentHiresAsync(int days);
}
public interface IDepartmentService
{
    Task<List<Department>> GetAllAsync();
    Task<Department?> GetByIdAsync(int id);
    Task<(bool ok, string? error)> CreateAsync(Department d);
    Task<(bool ok, string? error)> UpdateAsync(Department d);
    Task<(bool ok, string? error)> DeleteAsync(int id);
}
internal class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employees;
    private readonly IDepartmentRepository _departments;
    public EmployeeService(IEmployeeRepository employees, IDepartmentRepository departments)
    {
        _employees = employees;
        _departments = departments;
    }
    public Task<PagedResult<Employee>> GetPagedAsync(int page, int pageSize, string? search, string? sortBy, bool asc) => _employees.GetPagedAsync(page, pageSize, search, sortBy, asc);
    public Task<Employee?> GetByIdAsync(int id) => _employees.GetByIdAsync(id);
    public async Task<(bool ok, string? error)> CreateAsync(Employee e)
    {
        var err = await ValidateEmployee(e);
        if (err != null) return (false, err);
        await _employees.AddAsync(e);
        return (true, null);
    }
    public async Task<(bool ok, string? error)> UpdateAsync(Employee e)
    {
        var err = await ValidateEmployee(e, true);
        if (err != null) return (false, err);
        await _employees.UpdateAsync(e);
        return (true, null);
    }
    public async Task<(bool ok, string? error)> DeleteAsync(int id)
    {
        await _employees.DeleteAsync(id);
        return (true, null);
    }
    public Task<List<Employee>> GetAllAsync() => _employees.GetAllAsync();
    public Task<List<Employee>> GetRecentHiresAsync(int days) => _employees.GetRecentHiresAsync(days);
    private async Task<string?> ValidateEmployee(Employee e, bool isUpdate = false)
    {
        var ctx = new ValidationContext(e);
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(e, ctx, results, true))
            return string.Join("; ", results.Select(r => r.ErrorMessage));
        if (e.HireDate.Date > DateTime.UtcNow.Date)
            return "Hire date cannot be in the future.";
        if (e.Salary <= 0)
            return "Salary must be greater than 0.";
        var dep = await _departments.GetByIdAsync(e.DepartmentId);
        if (dep is null) return "DepartmentId must reference an existing department.";
        return null;
    }
}
internal class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departments;
    private readonly IEmployeeRepository _employees;
    public DepartmentService(IDepartmentRepository d, IEmployeeRepository e)
    {
        _departments = d;
        _employees = e;
    }
    public Task<List<Department>> GetAllAsync() => _departments.GetAllAsync();
    public Task<Department?> GetByIdAsync(int id) => _departments.GetByIdAsync(id);
    public async Task<(bool ok, string? error)> CreateAsync(Department d)
    {
        if (string.IsNullOrWhiteSpace(d.Name))
            return (false, "Name is required");
        await _departments.AddAsync(d);
        return (true, null);
    }
    public async Task<(bool ok, string? error)> UpdateAsync(Department d)
    {
        if (string.IsNullOrWhiteSpace(d.Name))
            return (false, "Name is required");
        await _departments.UpdateAsync(d);
        return (true, null);
    }
    public async Task<(bool ok, string? error)> DeleteAsync(int id)
    {
        var emps = await _employees.GetAllAsync();
        if (emps.Any(e => e.DepartmentId == id))
            return (false, "Cannot delete department that still has employees.");
        await _departments.DeleteAsync(id);
        return (true, null);
    }
}
