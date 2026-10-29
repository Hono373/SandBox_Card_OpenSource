using System;
using Sirenix.OdinInspector;
using Newtonsoft.Json;

[Serializable]
public class GameSerialize
{
    const string saveName = "game.sav";
    const string configName = "config.cfg";

    static GameSerialize Instance;

    public static GameData Active { get => Instance._active; set => Instance._active = value; }
    public static GameConfig Config { get => Instance._config; set => Instance._config = value; }
    public static GameData Load { get => Instance._load; set => Instance._load = value; }
    [ShowInInspector, ReadOnly] GameData _active;
    [ShowInInspector, ReadOnly] GameConfig _config;
    [ShowInInspector, ReadOnly] GameData _load;

    public static void Create()
    {
        Instance = new GameSerialize();
        Load = JsonExtensions.LoadInPersistantDataPath<GameData>(saveName);
        Config = JsonExtensions.LoadInPersistantDataPath<GameConfig>(configName);
        Config ??= new GameConfig();
    }
    public static void CreateActive(GameInfo info)
    {
        Active = GameData.Create(info);
    }
    public static bool Loadable() => Load != null;
    public static void Save()
    {
        Active.Serialize(saveName);
        Load = Active;
    }
    public static void SaveConfig()
    {
        Config.Serialize(configName);
    }
    public static void LoadGame()
    {
        Active = Load;
    }
}
