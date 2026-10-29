using Newtonsoft.Json;
using ProjectLumiyaDB;

public class TaskData
{
    public string key = string.Empty;
    public void Set(string v)
    {
        key = v;
    }
    [JsonIgnore] public TaskInfo Info => null;
}
