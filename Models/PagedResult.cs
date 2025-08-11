namespace ArboxEmployeeMS.Models;
public class PagedResult<T>{
  public required IEnumerable<T> Items { get; set; }
  public required int TotalCount { get; set; }
  public required int Page { get; set; }
  public required int PageSize { get; set; }
}
