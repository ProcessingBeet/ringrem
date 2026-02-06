using System.Text.Json;

using ringrem.models;
using ringrem.interfaces;



public static class DataIO
{

    public static List<T> LoadData<T>(string path, ILog? log)
    {
        if (!File.Exists(path))
        {
            log?.Log($"File {path} does not exist.");
            return new List<T>();
        }
            
        try
        {
           string json = File.ReadAllText(path);
           var result = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
           log?.Log($"{path} loaded succesfully.");
           return result;
        }
        catch (Exception ex)
        {
            log?.Log(ex.Message);
            return new List<T>();
        }

        
    }
    public static bool SaveData<T>(string path, List<T> newData, ILog? log)
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

    public static bool SaveState(State state, ILog? log)
    {
        if( SaveData(state.peoplePath, state.people, log) &&
            SaveData(state.groupsPath, state.groups, log))
            return true;
        return false;
    }

    public static Dictionary<string, int> LoadConfig(string path, ILog? log)
    {
        if (!File.Exists(path))
        {
            log?.Log($"File {path} does not exist.");
            return new Dictionary<string, int>();
        }
            
        try
        {
           string json = File.ReadAllText(path);
           var result = JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? new Dictionary<string, int>();
           log?.Log($"{path} loaded succesfully.");
           return result;
        }
        catch (Exception ex)
        {
            log?.Log(ex.Message);
            return new Dictionary<string, int>();
        }
    }
}