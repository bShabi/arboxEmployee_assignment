using ArboxEmployeeMS.Models;
namespace ArboxEmployeeMS.Data;
public interface IEmployeeRepository
{
    Task<PagedResult<Employee>> GetPagedAsync(int page, int pageSize, string? search, string? sortBy, bool asc);
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee> AddAsync(Employee e);
    Task UpdateAsync(Employee e);
    Task DeleteAsync(int id);
    Task<int> NextIdAsync();
    Task<List<Employee>> GetAllAsync();
    Task<List<Employee>> GetRecentHiresAsync(int days);
}
public interface IDepartmentRepository
{
    Task<List<Department>> GetAllAsync();
    Task<Department?> GetByIdAsync(int id);
    Task<Department> AddAsync(Department d);
    Task UpdateAsync(Department d);
    Task DeleteAsync(int id);
    Task<int> NextIdAsync();
}
internal class EmployeeRepository : IEmployeeRepository
{
    private readonly JsonFileContext _ctx; 
    private const string FileName = "employees.json";
    public EmployeeRepository(JsonFileContext ctx)
    {
        _ctx = ctx;
    }
    public Task<List<Employee>> GetAllAsync()
    {
        var list = _ctx.Read<List<Employee>>(FileName);
        return Task.FromResult(list.OrderBy(e => e.Id).ToList());
    }
    public async Task<PagedResult<Employee>> GetPagedAsync(int page, int pageSize, string? search, string? sortBy, bool asc)
    {
        var data = await GetAllAsync();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            data = data.Where(e => e.FirstName.ToLower().Contains(s) || e.LastName.ToLower().Contains(s) || e.Email.ToLower().Contains(s)).ToList();
        }
        data = (sortBy?.ToLower()) switch
        {
            "firstname" => (asc ? data.OrderBy(e => e.FirstName) : data.OrderByDescending(e => e.FirstName)).ToList(),
            "lastname" => (asc ? data.OrderBy(e => e.LastName) : data.OrderByDescending(e => e.LastName)).ToList(),
            "email" => (asc ? data.OrderBy(e => e.Email) : data.OrderByDescending(e => e.Email)).ToList(),
            "salary" => (asc ? data.OrderBy(e => e.Salary) : data.OrderByDescending(e => e.Salary)).ToList(),
            "hiredate" => (asc ? data.OrderBy(e => e.HireDate) : data.OrderByDescending(e => e.HireDate)).ToList(),
            _ => (asc ? data.OrderBy(e => e.Id) : data.OrderByDescending(e => e.Id)).ToList(),
        };
        var total = data.Count;
        var items = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResult<Employee> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }
    public Task<Employee?> GetByIdAsync(int id) {
        var list = _ctx.Read<List<Employee>>(FileName); 
        return Task.FromResult(list.FirstOrDefault(e => e.Id == id));
    }
    public async Task<Employee> AddAsync(Employee e) {
        var list = await GetAllAsync();
        e.Id = await NextIdAsync();
        list.Add(e); _ctx.Write(FileName, list);
        return e; 
    }
    public async Task UpdateAsync(Employee e) { 
        var list = await GetAllAsync();
        var i = list.FindIndex(x => x.Id == e.Id);
        if (i == -1) throw new KeyNotFoundException("Employee not found"); 
        list[i] = e; 
        _ctx.Write(FileName, list);
    }
    public async Task DeleteAsync(int id) {
        var list = await GetAllAsync(); 
        list = list.Where(x => x.Id != id).ToList();
        _ctx.Write(FileName, list);
    }
    public async Task<int> NextIdAsync() {
        var list = await GetAllAsync(); 
        return list.Count == 0 ? 1 : list.Max(e => e.Id) + 1; 
    }
    public async Task<List<Employee>> GetRecentHiresAsync(int days) { 
        var list = await GetAllAsync(); 
        var t = DateTime.UtcNow.Date.AddDays(-days); 
        return list.Where(e => e.HireDate.Date >= t).OrderByDescending(e => e.HireDate).ToList();
    }
}
internal class DepartmentRepository : IDepartmentRepository
{
    private readonly JsonFileContext _ctx;
    private const string FileName = "departments.json";
    public DepartmentRepository(JsonFileContext ctx) {
        _ctx = ctx;
    }
    public Task<List<Department>> GetAllAsync() {
        var list = _ctx.Read<List<Department>>(FileName);
        return Task.FromResult(list.OrderBy(d => d.Name).ToList());
    }
    public Task<Department?> GetByIdAsync(int id) {
        var list = _ctx.Read<List<Department>>(FileName);
        return Task.FromResult(list.FirstOrDefault(d => d.Id == id));
    }
    public async Task<Department> AddAsync(Department d) {
        var list = await GetAllAsync();
        d.Id = await NextIdAsync();
        list.Add(d); _ctx.Write(FileName, list);
        return d;
    }
    public async Task UpdateAsync(Department d) {
        var list = await GetAllAsync();
        var i = list.FindIndex(x => x.Id == d.Id);
        if (i == -1) throw new KeyNotFoundException("Department not found");
        list[i] = d;
        _ctx.Write(FileName, list);
    }
    public async Task DeleteAsync(int id) { 
        var list = await GetAllAsync();
        list = list.Where(x => x.Id != id).ToList();
        _ctx.Write(FileName, list);
    }
    public async Task<int> NextIdAsync() {
        var list = await GetAllAsync();
        return list.Count == 0 ? 1 : list.Max(d => d.Id) + 1;
    }
}
