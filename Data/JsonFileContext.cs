using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
namespace ArboxEmployeeMS.Data;
public class JsonFileContext
{
    private readonly string _dataDir;
    private readonly ReaderWriterLockSlim _lock = new();
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    public JsonFileContext(IWebHostEnvironment env)
    {
        _dataDir = Path.Combine(env.ContentRootPath, "App_Data");
        Directory.CreateDirectory(_dataDir);
    }
    public T Read<T>(string file) where T : new()
    {
        _lock.EnterReadLock();
        try
        {
            var p = Path.Combine(_dataDir, file); if (!File.Exists(p))
                return new T(); var json = File.ReadAllText(p);
            return JsonSerializer.Deserialize<T>(json, _options) ?? new T();
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }
    public void Write<T>(string file, T data)
    {
        _lock.EnterWriteLock();
        try
        {
            var p = Path.Combine(_dataDir, file);
            var json = JsonSerializer.Serialize(data, _options);
            File.WriteAllText(p, json);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }
}
