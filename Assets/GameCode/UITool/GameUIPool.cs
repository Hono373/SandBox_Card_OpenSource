using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通用子元素 UI 对象池（T 须为挂 IGameUI 的 MonoBehaviour）。
/// 流程：Collect 遍历父节点子元素经 Clear() 清空入池 → Get 取用（池空则新建到 parent 下）→ DestroyExtra 销毁池内多余元素。
/// </summary>
public class GameUIPool<T> where T : MonoBehaviour, IGameUI
{
    readonly Transform parent;
    readonly Queue<T> pool = new();

    public GameUIPool(Transform parent)
    {
        this.parent = parent;
    }

    /// <summary>遍历父节点子元素，将现有 T 经 IGameUI.Clear() 清空后收进池。</summary>
    public void Collect()
    {
        foreach (Transform child in parent)
        {
            if (child.TryGetComponent<T>(out var ui))
            {
                ui.Clear();
                pool.Enqueue(ui);
            }
        }
    }

    /// <summary>取出复用一个元素，池空则新建（实例化到 parent 下）。</summary>
    public T Get()
    {
        return pool.Count > 0 ? pool.Dequeue() : GameAsset.Instantiate<T>(parent);
    }

    /// <summary>销毁池内剩余（多余）元素。</summary>
    public void DestroyExtra()
    {
        while (pool.Count > 0)
        {
            GameObject.Destroy(pool.Dequeue().gameObject);
        }
    }
}
