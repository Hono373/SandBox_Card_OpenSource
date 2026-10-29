using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Transform container;

    [SerializeReference] public GameDB gameDB;

    public bool skipFlow = false;

    IEnumerator Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Instance = this;

        yield return GameAsset.Create();
        // 2026-10-23 挪到资源系统之后：GameDebug.Create 现在要读 DebugConfig 资源，
        // 原来在 GameAsset.Create() 之前调，package 还没初始化，LoadAssetSync 必炸。
        GameDebug.Create();
        gameDB = GameDB.Create();
        GameSerialize.Create();
        GameLanguage.Create();
        GameCamera.Create(Instance);
        GameLoading.Create(Instance);

        // 正式入口（菜单）尚未完成：GameMenuObj 预制体不存在，GameMenu.Enter() 无人调用，
        // 打包版会停在空画面。做正式菜单时恢复下面两行。
        // GameMenu.Create(Instance);
        // GameMenu.Enter();

        // 暂时复用测试入口（GameTest.testEnum = Battle）直接进战斗，保证构建产物可运行。
        var testManager = GameTest.Create();
        testManager.Invoke(Instance);
    }
}