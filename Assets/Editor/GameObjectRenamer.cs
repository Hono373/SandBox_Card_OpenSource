using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameObjectRenamer
{
    public static void RenameGameObject(GameObject gameObject)
    {
        if (gameObject == null) return;

        // 1. 优先检查 Image
        var image = gameObject.GetComponent<Image>();
        if (image != null)
        {
            gameObject.name = "img";
            return;
        }

        // 2. 检查 TextMeshProUGUI
        var tmp = gameObject.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            gameObject.name = "text";
            return;
        }

        // 3. 查找第一个自定义脚本（无命名空间的 MonoBehaviour）
        var behaviours = gameObject.GetComponents<MonoBehaviour>();
        foreach (var behaviour in behaviours)
        {
            if (behaviour == null) continue;
            
            var type = behaviour.GetType();
            // 无命名空间 = 自定义脚本（通常情况）
            if (string.IsNullOrEmpty(type.Namespace))
            {
                gameObject.name = type.Name;
                Debug.Log($"找到自定义脚本: {type.Name}，重命名为: {type.Name}");
                return;
            }
        }

        // 可选：什么都没找到时的处理
        Debug.Log($"{gameObject.name} 没有找到符合条件的组件");
    }
}