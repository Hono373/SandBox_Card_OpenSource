#if UNITY_EDITOR
# endif
using System.Collections;
using System.Threading.Tasks;

using UnityEngine;
using YooAsset;

public static class GameAsset
{
    public static ResourcePackage package;
    public static IEnumerator Create()
    {
        var packetName = "YooAsset";
        YooAssets.Initialize();
        package = YooAssets.CreatePackage(packetName);
        YooAssets.SetDefaultPackage(package);
#if UNITY_EDITOR
        var initParameters = new EditorSimulateModeParameters();
        var simulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(
            EDefaultBuildPipeline.BuiltinBuildPipeline, packetName);
        initParameters.SimulateManifestFilePath = simulateManifestFilePath;
#else
        var initParameters = new OfflinePlayModeParameters();
#endif
        var initOperation = package.InitializeAsync(initParameters);
        yield return initOperation;
        if (initOperation.Status == EOperationStatus.Succeed)
        {
            Debug.Log("资源包初始化成功！");
        }
        else
        {
            Debug.LogError($"资源包初始化失败：{initOperation.Error}");
        }
    }
    public static async Task<T> InstantiateAsync<T>(Transform parent) where T : class
    {
        string name = typeof(T).Name;
        AssetHandle handle = GameAsset.package.LoadAssetAsync<GameObject>(name);
        await handle.Task;
        GameObject go = handle.InstantiateSync(parent);
        return go.GetComponent<T>();
    }
    public static T Instantiate<T>(Transform parent) where T : class
    {
        string name = typeof(T).Name;
        AssetHandle handle = GameAsset.package.LoadAssetSync<GameObject>(name);
        GameObject go = handle.InstantiateSync(parent);
        return go.GetComponent<T>();
    }
    public static T Instantiate<T>(string name, Transform parent) where T : class
    {
        AssetHandle handle = GameAsset.package.LoadAssetSync<GameObject>(name);
        GameObject go = handle.InstantiateSync(parent);
        return go.GetComponent<T>();
    }
    public static async Task<T> GetObjectAsync<T>(string name) where T : Object
    {
        AssetHandle handle = GameAsset.package.LoadAssetAsync<Object>(name);
        await handle.Task;
        return handle.AssetObject as T;
    }
    static T GetObject<T>(this string name) where T : Object
    {
        AssetHandle handle = GameAsset.package.LoadAssetSync<Object>(name);
        if (handle.AssetObject == null)
            return null;
        return handle.AssetObject as T;
    }
    public static Sprite GetSprite(string name)
    {
        AssetHandle handle = GameAsset.package.LoadAssetSync<Sprite>(name);
        var asset = handle.AssetObject;
        GameDebug.Log($"GetSprite: name={name} asset={(asset == null ? "null" : asset.GetType().Name)}");
        if (asset is not Sprite sprite)
        {
            Debug.LogError($"[GameAsset] Sprite 加载失败: {name}，请检查资源是否存在/收集规则 Addressable 是否等于文件名");
            return null;
        }
        return sprite;
    }

    /// <summary>
    /// 通过资源 Key 和泛型类型从 YooAsset 同步获取 ScriptableObject 配置
    /// 编辑器环境下：若资源不存在，自动在指定路径创建并保存新的配置文件
    /// 非编辑器环境下：资源不存在会直接报错
    /// </summary>
    /// <typeparam name="T">要获取的 ScriptableObject 类型</typeparam>
    /// <param name="key">资源唯一标识 Key</param>
    /// <param name="noFullName">true= fullName = key;false=fullName = $"{typeName}_{key}";</param>
    /// <returns>获取或自动创建成功的 ScriptableObject 实例</returns>
    public static T GetSingletonSo<T>() where T : ScriptableObject
    {
        return GetSo<T>();
    }
    public static T GetInstanceSo<T>(string instanceKey) where T : ScriptableObject
    {
        return GetSo<T>(instanceKey);
    }
    public static T GetSo<T>(string key = "") where T : ScriptableObject
    {
        var typeName = typeof(T).Name;
        var path = $"Assets/GameRes/Info/{typeName}";
        var fullName = string.Empty;
        if (string.IsNullOrEmpty(key))
        {
            fullName = typeName;
        }
        else
        {
            fullName = $"{typeName}_{key}";
        }

        var info = GetObject<T>(fullName);
        if (info == null)
        {
            info = ScriptableObject.CreateInstance<T>();
            GameEditor.CreateAsset(info, path, fullName);
            Debug.Log($"{fullName} not found，but is created，Runtime only");
        }
        return info;
    }
}
