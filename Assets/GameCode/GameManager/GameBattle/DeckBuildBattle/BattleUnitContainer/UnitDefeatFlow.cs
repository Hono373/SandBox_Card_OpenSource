using DG.Tweening;
using UnityEngine;

/// <summary>
/// 单位阵亡动画：抖动 + 淡出 + 泛红，结束后销毁 UI 并从容器回收。
/// 时间轴由 GameFlow.Flow() 创建，动画在 FlowBuild() 内组装。
/// </summary>
public class UnitDefeatFlow : BattleFlow
{
    public BattleUnit unit;
    public UnitDefeatFlow Start(BattleUnit unit)
    {
        this.unit = unit;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        var container = DeckBuildBattle.GetUI().unitContainer;
        var obj = container.GetUnitUI(unit);
        // 2026-10-21 死亡立即断射线：尸体淡出后 GameObject 还在，CanvasGroup.blocksRaycasts 默认仍为 true，
        // 拖卡时会被 UnitMask 上的 BattleUnitSelectedUI 命中，从而把死单位当成可选目标。
        obj.GetCanvasGroup().Ray(false);
        flow.Join(obj.RectT().DOShakeAnchorPos(unit.deadTime, 20, 10, 90));
        flow.Join(obj.GetCanvasGroup().DOFade(0, unit.deadTime));
        flow.Join(obj.skeleton.graphic.DOColor(Color.red, unit.deadTime));
    }
}
