using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public static class EditorExtension
{
    public static void RegisterSaveEvent(this Editor editor, VisualElement root, Object target)
    {
        root.RegisterCallback<FocusOutEvent>(evt =>
        {
            Debug.Log($"Saving [{target.name}] changes...");
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssetIfDirty(target);
        });
    }
}
