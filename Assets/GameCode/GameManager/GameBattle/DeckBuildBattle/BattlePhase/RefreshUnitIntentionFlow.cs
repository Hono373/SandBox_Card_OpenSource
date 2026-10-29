using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class RefreshUnitIntentionFlow : BattleFlow
{
    public BattleUnit unit;
    public BattleUnitUI ui;
    public RefreshUnitIntentionFlow Start(BattleUnit unit)
    {
        this.unit = unit;
        this.ui = DeckBuildBattle.GetUI().unitContainer.GetUnitUI(unit);
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        // 只刷 UI，不重算 unitSkill：这个 flow 被 ModifierAddFlow 复用，用来"按新数值重绘意图"，
        // 若在这里重算会顺手换掉敌人这一轮已经确定的招。要重抽请显式调 RefreshData。
        flow.Join(ui.intentionUI.Refresh(unit));
    }
    /// <summary>按当前 skillHistory 与条件重算 unitSkill（克隆意图树后加权随机）。构建期调用即可，前提是调用时历史已写全。</summary>
    public static void RefreshData(BattleUnit unit)
    {
        unit.unitSkill = unit.Info.intention.Create().GetResult(unit);
    }
    /// <summary>
    /// 播放时点同步刷新：重算 unitSkill + 复用意图 UI 的对象池刷显示。
    /// 给走不了 FlowBuild 构建期的场景用（UnitActionFlow 的行动回调），否则构建期算会少读一条本轮刚写入的历史。
    /// </summary>
    public static void RefreshNow(BattleUnit unit)
    {
        RefreshData(unit);
        DeckBuildBattle.GetUI().unitContainer.GetUnitUI(unit).intentionUI.Refresh(unit);
    }
}
