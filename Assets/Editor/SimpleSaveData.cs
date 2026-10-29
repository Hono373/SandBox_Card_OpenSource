using UnityEditor;

#if ODIN_INSPECTOR
// 2026-10-30 开源准备：OdinEditorWindow 属于商业插件，未安装时不注册菜单，避免整包编译失败。
public class SimpleSaveData : Sirenix.OdinInspector.Editor.OdinEditorWindow
{
    [MenuItem("我的工具/存档检视器")]
    public static void ShowWindow()
    {
        EditorGUIUtility.PingObject(GameAsset.GetSingletonSo<GameTest>());
    }
}
#endif
