/// <summary>塞牌 / 挪牌流程：卡牌已入库，这里只同步牌堆计数 UI。</summary>
public class AddCardFlow : BattleFlow
{
    public PileEnum pile;
    public AddCardFlow Start(PileEnum pile)
    {
        this.pile = pile;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        // 2026-10-23 一次挪动会同时改动两个堆（头槌：弃牌堆 → 抽牌堆），
        // 所以不再只刷抽牌堆，两边一起重读。
        flow.onComplete += () => DeckBuildBattle.GetUI().uiContainer.RefreshPileCount();
    }
}
