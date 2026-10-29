using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GameObjectNamesFix
{
    [MenuItem("我的工具/场景内物体名-首字母大写")]
    public static void CapitalizeNames()
    {
        // 获取场景中的所有物体

        var stage = PrefabStageUtility.GetCurrentPrefabStage();

        if (stage == null)
        {
            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                 GameObjectRenamer.RenameGameObject(obj);
            }

            return;
        }

        Transform[] allPrefabObj = stage.FindComponentsOfType<Transform>();

        foreach (Transform t in allPrefabObj)
        {
            GameObjectRenamer.RenameGameObject(t.gameObject);
        }
        EditorUtility.SetDirty(stage.prefabContentsRoot);
        AssetDatabase.SaveAssetIfDirty(stage.prefabContentsRoot);
    }
}
