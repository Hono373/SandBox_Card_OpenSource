using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 重振精神的表现流程：交给 ExhaustFlow 做消耗（移堆+动画+事件），这里只负责收尾加格挡。
/// </summary>
public class ExhaustHandShieldFlow : BattleFlow
{
    /// <summary>要消耗的卡。</summary>
    public List<BattleCard> exhausts = new();
    /// <summary>每张提供的格挡。</summary>
    public int shieldPerCard;
    public BattleUnit caster;
    public ExhaustFlow exhaustFlow = new();

    public ExhaustHandShieldFlow Start(List<BattleCard> cards, int shieldPerCard, BattleUnit caster)
    {
        exhausts.AddRange(cards);
        this.shieldPerCard = shieldPerCard;
        this.caster = caster;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        flow.Join(exhaustFlow.Start(exhausts).flow);

        // 每张消耗换成格挡
        caster.data.shield += exhausts.Count * shieldPerCard;
        var casterUI = DeckBuildBattle.GetUI().unitContainer.GetUnitUI(caster);
        flow.Append(casterUI.shieldUI.Refresh());
    }
}
