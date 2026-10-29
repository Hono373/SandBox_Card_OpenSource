using System;
using UnityEngine;
public class GameTest : ScriptableObject
{
    [SerializeField] TestEnum testEnum;
    public DeckBuildBattleInfo battleInfo;
    /// <summary>
    /// 2026-10-23 第二场战斗。游戏目前一共两款战斗，结算面板的「下一场」就是从这里取。
    /// 为空表示没有下一场，结算面板会隐藏按钮。
    /// </summary>
    public DeckBuildBattleInfo nextBattleInfo;
    public static GameTest Create()
    {
        var gameTest = GameAsset.GetSingletonSo<GameTest>();
        return gameTest;
    }
    public void Invoke(GameManager instance)
    {
        switch (testEnum)
        {
            case TestEnum.None:
                CommonStart(instance);
                break;
            case TestEnum.Battle:
                BattleTest(instance);
                break;
            case TestEnum.Story:
                StoryTest(instance);
                break;
            case TestEnum.Scene:
                SceneTest(instance);
                break;
            default:
                Debug.LogError("Unknown test type selected.");
                break;

        }
    }
    void CommonStart(GameManager instance)
    {
        GameMenu.Enter();
    }
    public void BattleTest(GameManager instance)
    {
        BattleTest(instance, battleInfo);
    }
    /// <summary>
    /// 2026-10-23 指定战斗信息启动。结算面板的「下一场」需要切到另一场。
    /// 注意**不回写 battleInfo**：它和 nextBattleInfo 是「第一场 / 第二场」的静态配置，
    /// 谁都能改的话，「当前是不是最后一场」就判断不出来了。
    /// </summary>
    public void BattleTest(GameManager instance, DeckBuildBattleInfo info)
    {
        GameDebug.Log($"Testing Battle... info={info.key}");

        GameLoading.GetObj().SetActive(true);

        GameSerialize.CreateActive(GameInfo.Get());
        GameSerialize.Active.CreateDDB(info);
        GameSerialize.Save();
        new DeckBuildBattleStartFlow().Start(DeckBuildBattle.Active);
    }
    void SceneTest(GameManager instance)
    {
        GameDebug.Log("Testing Scene...");
        var active = GameSerialize.Active;
        var scenedata = active.sceneData = new();
        scenedata.Init(instance);

        //scenedata.Register(active.task);

        GameScene.Enter();
    }
    void StoryTest(GameManager instance)
    {
        GameDebug.Log("Testing Story...");
        var active = GameSerialize.Active;
        GameLoading.GetObj().SetActive(true);

        active.storyQueue.Clear();
        active.storyQueue.Enqueue("0");

        GameStory.Enter();
    }
}