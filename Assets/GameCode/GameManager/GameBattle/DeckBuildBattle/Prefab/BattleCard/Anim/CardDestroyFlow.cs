using DG.Tweening;
using UnityEngine;

public class CardDestroyFlow : BattleFlow
{
    public BattleCardUI ui;
    public CardDestroyFlow Start(BattleCardUI ui)
    {
        this.ui = ui;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        BattleCardUI.Anim.destroy.Start(ui, out var destroySeq);
        destroySeq.onComplete += () => GameObject.Destroy(ui.gameObject);
        flow.Join(destroySeq);
    }
}
