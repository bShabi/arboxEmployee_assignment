using System.Text.Json;
namespace ArboxEmployeeMS.Middleware;
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;
    public ErrorHandlingMiddleware(RequestDelegate next, IWebHostEnvironment env)
    {
        _next = next;
        _env = env;
    }
    public async Task Invoke(HttpContext context)
    {
        try { await _next(context); }
        catch (Exception ex)
        {
            if (_env.IsDevelopment())
            {
                throw;
            }
            await LogExceptionAsync(ex, context);
            context.Response.Redirect("/Home/Error");
        }
    }
    private async Task LogExceptionAsync(Exception ex, HttpContext ctx)
    {
        var logsDir = Path.Combine(_env.ContentRootPath, "Logs"); Directory.CreateDirectory(logsDir);
        var file = Path.Combine(logsDir, $"log-{DateTime.UtcNow:yyyy-MM-dd}.json");
        var log = new { timestamp = DateTime.UtcNow, path = ctx.Request.Path.ToString(), method = ctx.Request.Method, message = ex.Message, stackTrace = ex.StackTrace };
        var json = JsonSerializer.Serialize(log) + Environment.NewLine;
        await File.AppendAllTextAsync(file, json);
    }
}
