using DG.Tweening;
using UnityEngine;

/// <summary>
/// 单位行动位移：施法者向前顶出一段再回到原位，用于攻击演出的打击感。
/// 只有攻击类（卡牌 TypeEnum.Attack / 单位 EffectTypeEnum.Dmg）才调用本 flow，
/// 技能与能力不位移，判定写在各自的调用点。
/// 位移对象是 Spine 骨架本体（血条、意图气泡不动）；方向按阵营：Mate 向右、Enemy 向左。
/// 回位终点取位移开始时的绝对坐标，不走相对偏移，重复触发也不会累积漂移。
/// </summary>
public class BattleUnitActionMove : BattleFlow
{
    public BattleUnit unit;
    public Vector2 distance = new Vector2(60, 0);
    public float forwardTime = 0.12f;
    public float backTime = 0.18f;
    public BattleUnitActionMove Start(BattleUnit unit)
    {
        this.unit = unit;
        Init();
        return this;
    }
    // 2026-10-20 新增：出牌 / 单位行动的前冲回位，两个入口共用这一个 flow。
    protected override void FlowBuild()
    {
        var ui = DeckBuildBattle.GetUI().unitContainer.GetUnitUI(unit);
        var rect = ui.skeleton.graphic.rectTransform;
        var origin = rect.anchoredPosition;
        var forward = unit.camp.camp == CampEnum.Mate ? 1f : -1f;

        flow.Append(rect.DOAnchorPos(origin + new Vector2(distance.x * forward, distance.y), forwardTime).SetEase(Ease.OutQuad));
        flow.Append(rect.DOAnchorPos(origin, backTime).SetEase(Ease.InQuad));
    }
}
