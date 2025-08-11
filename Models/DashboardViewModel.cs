namespace ArboxEmployeeMS.Models;
public class DashboardViewModel{
  public int TotalEmployees { get; set; }
  public List<(string Department, int Count)> EmployeesByDepartment { get; set; } = new();
  public List<Employee> RecentHires { get; set; } = new();
  public string? Search { get; set; }
}
