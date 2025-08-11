using System.ComponentModel.DataAnnotations;
namespace ArboxEmployeeMS.Models;
public class Employee{
  public int Id { get; set; }
  [Required, StringLength(50)] public string FirstName { get; set; } = string.Empty;
  [Required, StringLength(50)] public string LastName { get; set; } = string.Empty;
  [Required, EmailAddress, StringLength(200)] public string Email { get; set; } = string.Empty;
  [DataType(DataType.Date)] public DateTime HireDate { get; set; } = DateTime.UtcNow.Date;
  [Range(0.01, double.MaxValue)] public decimal Salary { get; set; }
  [Required] public int DepartmentId { get; set; }
}
