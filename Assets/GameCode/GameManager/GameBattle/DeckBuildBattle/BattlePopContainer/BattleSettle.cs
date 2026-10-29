using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class BattleSettle : MonoBehaviour
{
    DeckBuildBattle manager;
    [HideInInspector] public UIGroup uiGroup;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Button nextBattle;
    /// <summary>
    /// 2026-10-23 最后一场（第二场）是否隐藏「下一场」按钮。
    /// 默认 true：游戏目前一共两款战斗，第二场打完没有后续，按钮直接隐藏。
    /// 关掉它按钮就会一直在（点了没反应），只适合调试时固定位置。
    /// </summary>
    [SerializeField] bool hideOnLastBattle = true;

    internal static BattleSettle Create(DeckBuildBattle manager)
    {
        var settle = GameAsset.Instantiate<BattleSettle>(DeckBuildBattle.GetUI().settleContainer.transform);
        settle.Init(manager);

        // 2026-10-23 原来是 restart：清修正器后重开同一场。改成「下一场」，
        // 目标是 GameTest.nextBattleInfo 配的第二场战斗；没有下一场就把按钮藏起来。
        bool hasNext = settle.GetNextInfo() != null;
        settle.nextBattle.gameObject.SetActive(hasNext || !settle.hideOnLastBattle);
        settle.nextBattle.onClick.AddListener(settle.NextBattle);

        return settle;
    }

    public void Init(DeckBuildBattle cardBattleManager)
    {
        manager = cardBattleManager;
        UIGroup.Create(this, out uiGroup);
    }

    public void Enter(string msg)
    {
        text.text = msg;
    }

    /// <summary>
    /// 返回「下一场」是哪一份战斗信息，没有则 null。
    /// 判据是当前这场必须还是 GameTest.battleInfo（第一场）——
    /// 这样第二场打完不会绕回第一场，链有终点。
    /// </summary>
    DeckBuildBattleInfo GetNextInfo()
    {
        var test = GameAsset.GetSo<GameTest>();
        if (test.nextBattleInfo == null)
            return null;

        var current = manager.GetInfo();
        if (current.key != test.battleInfo.key)
            return null;

        return test.nextBattleInfo;
    }

    public void NextBattle()
    {
        var next = GetNextInfo();
        if (next == null)
        {
            Debug.LogError($"{nameof(BattleSettle)}.{nameof(NextBattle)}: 没有下一场战斗，按钮本不该可见");
            return;
        }

        GameModifyCenter.modifiers.Clear();
        GameObject.Destroy(DeckBuildBattle.GetUI().gameObject);
        GameAsset.GetSo<GameTest>().BattleTest(GameManager.Instance, next);
    }
}
