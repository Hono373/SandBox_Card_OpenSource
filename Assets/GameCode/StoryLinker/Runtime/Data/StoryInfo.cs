using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
public class StoryInfo : ScriptableObject
{
    #region 字段
    public StartInfo startInfo;
    [SerializeReference] public List<BaseInfo> list = new();
    #endregion

    public void Init()
    {
        // 先初始化 startInfo，再清空 guidHash，避免 NewGuid() 时访问未初始化的 startInfo
        startInfo = new StartInfo().Init(this, new(200, 100));
        guidHash = null; // 清空 guidHash，让它重新构建
        list.Clear(); // 清空旧数据，避免重复累积
        new DialogueInfo().Init(this, new(200 + 240, 100));
        new OptionInfo().Init(this, new(200 + 240 * 2, 100));
        new EndInfo().Init(this, new(200 + 240 * 3, 100));
        new NoteInfo().Init(this, new(200, 400));
        Debug.Log($"Init 完成，list 中有 {list.Count} 个节点");
    }

    public string NewGuid()
    {
        var guid = Guid.NewGuid().ToString();
        // 安全检查：如果 guidHash 还没初始化，直接返回
        if (guidHash == null)
        {
            guidHash = new HashSet<string>();
            guidHash.Add(guid);  // 修复：使用 guidHash 而不是 GuidHash
            return guid;
        }
        while (guidHash.Contains(guid))  // 修复：使用 guidHash 而不是 GuidHash（避免递归）
        {
            Debug.Log("Guid Error");
            guid = Guid.NewGuid().ToString();
        }
        guidHash.Add(guid);  // 修复：使用 guidHash 而不是 GuidHash
        return guid;
    }

    public void Remove(BaseInfo info)
    {
        list.Remove(info);
        startInfo.Cleaner(info);
        foreach (var i in list)
        {
            i.Cleaner(info);
        }
    }

    /// <summary>
    /// 标记脏 + 写盘。不调用 Refresh()，避免全量扫描卡顿。
    /// </summary>
    public void Save()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
#endif
    }
    HashSet<string> guidHash;
    HashSet<string> GuidHash
    {
        get
        {
            if (guidHash == null)
            {
                guidHash = new HashSet<string>();
                // 安全检查：startInfo 可能为 null（如 Reset 时）
                if (startInfo != null)
                {
                    guidHash.Add(startInfo.inPort.guid);
                    guidHash.Add(startInfo.outPort.guid);
                }
                foreach (var info in list)
                {
                    if (info?.inPort != null) guidHash.Add(info.inPort.guid);
                    if (info?.outPort != null) guidHash.Add(info.outPort.guid);
                }
                return guidHash;
            }
            else
            {
                return guidHash;
            }
        }
    }
}