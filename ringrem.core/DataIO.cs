using System.Diagnostics;
using System.Text.Json;

static class DataIO
{
    public static List<T> LoadData<T>(string path)
    {
        if (!File.Exists(path))
            return new List<T>();
        
        string json = File.ReadAllText(path);

        var result = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();

        return result;
    }
    public static bool SaveData<T>(string path, List<T> newData, Models.ILog? log)
    {
        try
        {
            using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            JsonSerializer.Serialize(stream, newData);
        }
        catch (Exception ex)
        {
            log?.Log(ex.Message);
            return false;
        }
        return true;
    }
}