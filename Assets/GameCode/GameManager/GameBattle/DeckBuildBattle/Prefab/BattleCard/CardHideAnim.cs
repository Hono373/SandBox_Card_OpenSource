using DG.Tweening;

public class CardHideAnim
{
    float spd = 1f;
    public Sequence Process(BattleCardUI cardObj)
    {
        var process = GameAnim.AutoPlay(nameof(CardHideAnim));

        cardObj.CanvasGroup().DOFade(0, spd);

        return process;
    }

}

