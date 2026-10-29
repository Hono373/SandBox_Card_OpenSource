using DG.Tweening;
using UnityEngine;

/// <summary>
/// Choice 交互按下时的手牌上移动画：卡牌移到手牌区中部。
/// 时间轴由 GameFlow.Flow() 创建，动画在 FlowBuild() 内组装。
/// </summary>
public class CardChoiceFlow : BattleFlow
{
    public BattleCardUI ui;
    public CardChoiceFlow Start(BattleCardUI ui)
    {
        this.ui = ui;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        var anim = BattleCardUI.Anim;
        var y = -DeckBuildBattle.GetUI().handContainer.RectT().rect.height / 2 + anim.cardHeight / 2;
        flow.Join(ui.Motion(new Vector2(0, y), Quaternion.identity, 1f * Vector3.one, anim.choice.spd));
    }
}
