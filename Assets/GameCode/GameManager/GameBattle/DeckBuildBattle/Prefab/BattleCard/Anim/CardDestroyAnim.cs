using DG.Tweening;
using UnityEngine;

public class CardDestroyAnim
{
    public float spd = 0.2f;
    public float usedScale = 1.5f;
    public Sequence Process(BattleCardUI cardObj)
    {
        var process = GameAnim.AutoPlay(nameof(CardDestroyAnim)).SetAutoKill(false);
        var obj = cardObj;
        var managerObj = DeckBuildBattle.GetUI();
        var rectT = cardObj.RectTransform();

        var doMove = rectT
            .DOAnchorPos(rectT.anchoredPosition
            + new Vector2(0, rectT.rect.height*1.5f), spd);
        var doScale = rectT.DOScale(usedScale, spd);
        var doFade = cardObj.CanvasGroup().DOFade(0, spd);

        process.Append(doMove);
        process.Append(doScale);
        process.Join(doFade);
        return process;
    }

    internal void Start(BattleCardUI cardObj, out Sequence cardDestory)
    {
        DeckBuildBattle.GetUI().handContainer.RecycleObj(cardObj.manager);
        DeckBuildBattle.GetUI().arrowContainer.arrowLine.Alpha(false);
        cardObj.Destroy();
        cardDestory = Process(cardObj);
    }
}