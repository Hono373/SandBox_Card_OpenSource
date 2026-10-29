using DG.Tweening;
using Newtonsoft.Json;

public class RefreshDrawPilecheckUIFlow : BattleFlow
{
    public int total;
    public int index;
    public int drawPileCountCache;
    [JsonIgnore] public int drawPileCount;
    public RefreshDrawPilecheckUIFlow Start(int drawNumb, int index, int drawPileCountCache)
    {
        this.total = drawNumb;
        this.index = index;
        this.drawPileCountCache = drawPileCountCache;
        Init();
        return this;
    }
    protected override void FlowBuild()
    {
        var drawPileCheckBtn = DeckBuildBattle.GetUI().uiContainer.drawPilecheckBtn;
        var resut = drawPileCountCache + total - index;
        flow.onComplete += () =>
           {
               drawPileCheckBtn.text.text = resut.ToString();
           };
    }
}
